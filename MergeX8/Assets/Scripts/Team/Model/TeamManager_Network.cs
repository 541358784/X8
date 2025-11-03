using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.UI
{
    public partial class TeamManager
    {
        /// <summary>
        /// 获取推荐队伍列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetRecommendTeamList(Action<bool> callback)
        {
            CGetRecommendTeams req = new CGetRecommendTeams();
            req.Page = 0;
            req.Limit = 250;
            req.PlayerLevel = MyLevel;

            APIManager.Instance.Send(req, (SGetRecommendTeams resp) =>
            {
                RecommendTeamList = new List<TeamInfo>();
                OriginalRecommendTeamList = new List<TeamInfo>();
                for (int i = 0; i < resp.Teams.Count; i++)
                {
                    var team = resp.Teams[i];
                    var teamExtra = TeamDataExtra.FromJson(team.TeamExtra);
                    if(team.MemberCount < teamExtra.MemberMaxCount && team.RequireLevel <= req.PlayerLevel)
                        RecommendTeamList.Add(team);
                    OriginalRecommendTeamList.Add(team);
                }
                RecommendTeamList.Sort((x1, x2) =>
                {
                    if (x1.WeeklyHelps != x2.WeeklyHelps)
                        return x2.WeeklyHelps - x1.WeeklyHelps;
                    return x2.MemberCount - x1.MemberCount;
                });
                callback?.Invoke(true);
            }, (errNo, errMsg, resp) =>
            {
                ShowErrorNotice(errNo, errMsg);
                callback?.Invoke(false);
            });
        }

        /// <summary>
        /// 根据关键字查询队伍
        /// </summary>
        /// <param name="searchName"></param>
        /// <param name="callback"></param>
        public void SearchTeams(string searchName,Action<bool> callback)
        {
            CQueryTeamList req = new CQueryTeamList();
            req.Name = searchName;
            APIManager.Instance.Send(req, (SQueryTeamList resp) =>
            {
                SearchTeamList = new List<TeamInfo>();
                for (int i = 0; i < resp.Teams.Count; i++)
                {
                    SearchTeamList.Add(resp.Teams[i]);
                }
                callback?.Invoke(true);
                
            }, (errNo, errMsg, resp) =>
            {
                ShowErrorNotice(errNo, errMsg);
                callback?.Invoke(false);
            });
        }
        
        /// <summary>
        /// 创建队伍
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="desc"></param>
        /// <param name="badge"></param>
        /// <param name="requireLevel"></param>
        /// <param name="callback"></param>
        public void CreateNewTeam(string teamName, string desc, Int32 badge, Int32 requireLevel,Action<bool> callback)
        {
            CCreateTeam req = new CCreateTeam();
            req.Name = teamName;
            req.Description = desc;
            req.Badge = badge;
            req.RequireLevel = requireLevel;
            req.PlayerLevel = MyLevel;
            req.PlayerName = MyName;
            req.PlayerExtra = PlayerInfoExtra.GetMyPlayerInfoExtra().ToString();
            var teamDataExtra = new TeamDataExtra();
            var levelConfig = TeamConfigManager.Instance.LevelConfigList.First();
            teamDataExtra.TeamLevel = levelConfig.Id;
            teamDataExtra.MemberMaxCount = levelConfig.MaxMember;
            teamDataExtra.BadgeFrame = 0;
            // teamDataExtra.Exp = 0;
            req.TeamExtra = teamDataExtra.ToString();
            req.MaxMemberCount = levelConfig.MaxMember;

            APIManager.Instance.Send(req, (SCreateTeam resp) =>
            {
                if (resp.TeamInfo.TeamId > 0)
                {
                    ClearAllChats();
                    ClearAllPassGifts();
                    MyTeamID = resp.TeamInfo.TeamId;
                    Storage.LastTeamId = MyTeamID;
                    MyTeamInfo = new TeamData(resp.TeamInfo);
                    MyTeamName = resp.TeamInfo.Name;
                    Storage.LastTeamName = MyTeamName;
                    MyTeamBadgeID = resp.TeamInfo.Badge;
                    callback?.Invoke(true);
                    SendTeamPing(value =>
                    {
                        if (!value)
                        {
                            Storage.LastPingTeamTimestamp = 0;
                        }
                    });
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTeamCreate,data1:resp.TeamInfo.TeamId.ToString());
                }
                else
                {
                    callback?.Invoke(false);
                }
                
                // BiUtil.SendGameEvent(BiEventProjectMatch.Types.GameEventType.GameEventCreateTeamSuccess, resp.TeamInfo.Name);
                
            }, (errNo, errMsg, resp) =>
            {
                ShowErrorNotice(errNo, errMsg);
                callback?.Invoke(false);
            });
        }
        
        /// <summary>
        /// 获取队伍红点信息
        /// </summary>
        /// <param name="noChat"></param>
        public void GetMyTeamInfo(bool noChat = false)
        {
            CGetMyTeamInfo req = new CGetMyTeamInfo();
            req.NoChat = noChat;

            APIManager.Instance.Send(req, (SGetMyTeamInfo resp) =>
                {
                    var lastState = InitNetwork;
                    InitNetwork = InitNetworkState.Success;
                    MyTeamID = resp.TeamId;
                    RemoteTeamConfig = resp.Config;
                    NextAskHelpTimestamp = resp.NextAskHelpTimestamp;
                    CanHelpCount = resp.CanHelpCount;
                    MyTeamName = resp.TeamName;
                    MyTeamBadgeID = resp.TeamBadge;
                    
                    if (MyTeamID > 0)
                    {
                        GetMyTeamDetailInfo(null);
                        GetTeamChats(LoadChatsType.Forward,null);
                        GetBattlePassGiftList(null);
                        UploadMyInfo();
                    }
                    EventDispatcher.Send(new EventOnTeamRedPointRefresh());
                    if(lastState == InitNetworkState.Nothing)
                    {
                        EventDispatcher.Send(new EventOnTeamServerInitSuccess());
                    }

                    if (Storage.LastTeamId > 0 && MyTeamID <= 0)
                    {
                        IsKickedHandle();
                    }

                    Storage.LastTeamId = MyTeamID;
                    Storage.LastTeamName = MyTeamName;
                },
                (errNo, errMsg, resp) =>
                {
                    if(InitNetwork == InitNetworkState.Nothing)
                        InitNetwork = InitNetworkState.Fail;
                    ShowErrorNotice(errNo, errMsg);
                }
            );
        }
        
        /// <summary>
        /// 检查我的队伍信息（主要检查是否被踢了）
        /// </summary>
        public void CheckMyTeamInfo()
        {
            CGetMyTeamInfo req = new CGetMyTeamInfo();
            req.NoChat = true;

            APIManager.Instance.Send(req, (SGetMyTeamInfo resp) =>
                {
                    MyTeamID = resp.TeamId;
                    RemoteTeamConfig = resp.Config;
                    NextAskHelpTimestamp = resp.NextAskHelpTimestamp;
                    CanHelpCount = resp.CanHelpCount;
                    MyTeamName = resp.TeamName;
                    MyTeamBadgeID = resp.TeamBadge;
                    if (Storage.LastTeamId > 0 && MyTeamID <= 0)
                    {
                        IsKickedHandle();
                    }
                    Storage.LastTeamId = MyTeamID;
                    Storage.LastTeamName = MyTeamName;
                },
                (errNo, errMsg, resp) =>
                {
                    ShowErrorNotice(errNo, errMsg);
                }
            );
        }
        
        /// <summary>
        /// 获取我的队伍的详情
        /// </summary>
        /// <param name="callback"></param>
        public void GetMyTeamDetailInfo(Action<MyTeamRequestResult> callback)
        {
            if (MyTeamID <= 0)
            {
                callback?.Invoke(MyTeamRequestResult.Fail);
                return;
            }
            
            CGetTeamDetail req = new CGetTeamDetail();
            req.TeamId = MyTeamID;

            APIManager.Instance.Send(req, (SGetTeamDetail resp) =>
            {
                if (resp.TeamInfo != null)
                {
                    var teamInfo = new TeamData(resp.TeamInfo);
                    if (teamInfo.IsKicked((long) MyPlayerId))
                    {
                        IsKickedHandle();
                        callback?.Invoke(MyTeamRequestResult.Kicked);
                    }
                    else
                    {
                        MyTeamName = teamInfo.Name;
                        MyTeamBadgeID = teamInfo.Badge;
                        MyTeamInfo = teamInfo;
                        callback?.Invoke(MyTeamRequestResult.Success);
                        if (MyTeamInfo.LeaderId == (long)MyPlayerId && MyTeamInfo.ServerMemberMaxCount != MyTeamInfo.MemberMaxCount)
                        {
                            PushMyTeamInfo();
                        }
                    }
                }
                else
                {
                    IsKickedHandle();
                    callback?.Invoke(MyTeamRequestResult.Kicked);
                }
            }, (errNo, errMsg, resp) =>
            {
                callback?.Invoke(MyTeamRequestResult.Fail);
                ShowErrorNotice(errNo, errMsg);
            });
        }
        
        /// <summary>
        /// 获取其它指定队伍的详情
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="callback"></param>
        public void GetOtherTeamDetailInfo(long teamId, Action<bool,TeamData> callback)
        {
            CGetTeamDetail req = new CGetTeamDetail();
            req.TeamId = teamId;

            APIManager.Instance.Send(req, (SGetTeamDetail resp) =>
            {
                if (resp.TeamInfo != null)
                {
                    if (resp.TeamInfo.TeamId > 0)
                    {
                        callback?.Invoke(true, new TeamData(resp.TeamInfo));
                        return;
                    }
                }

                callback?.Invoke(false, null);
                Debug.LogError("未找到公会 id="+teamId);
                
            }, (errNo, errMsg, resp) =>
            {
                callback?.Invoke(false, null);
                ShowErrorNotice(errNo, errMsg);
            });
        }

        public void PushMyTeamInfo(Action<bool> callback = null)
        {
            TeamData managerTeamData = TeamManager.Instance.MyTeamInfo;
            var levelConfig =
                TeamConfigManager.Instance.LevelConfigList.Find(a => a.Id == managerTeamData.ExtraData.TeamLevel);
            ModifyTeamInfo(managerTeamData.Description,
                managerTeamData.RequireLevel,
                managerTeamData.Name,
                levelConfig,
                managerTeamData.Badge,callback);
        }
        /// <summary>
        /// 修改队伍信息
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="requireLevel"></param>
        /// <param name="teamName"></param>
        /// <param name="callback"></param>
        public void ModifyTeamInfo(string desc, Int32 requireLevel,string teamName,TableTeamLevelConfig levelConfig,int badge,Action<bool> callback)
        {
            if (MyTeamID <= 0)
            {
                callback?.Invoke(true);
                return;
            }

            // if (MyTeamInfo != null && MyTeamInfo.Description == desc && MyTeamInfo.RequireLevel == requireLevel && MyTeamInfo.Name == teamName)
            // {
            //     // 没有变化，直接切换到队伍信息界面
            //     callback?.Invoke(true);
            //     return;
            // }

            CModifyTeamInfo req = new CModifyTeamInfo();
            req.TeamId = MyTeamID;
            req.Badge = badge;
            req.Description = desc;
            req.RequireLevel = requireLevel;
            req.TeamName = teamName;
            var teamDataExtra = new TeamDataExtra();
            teamDataExtra.TeamLevel = levelConfig.Id;
            teamDataExtra.MemberMaxCount = levelConfig.MaxMember;
            teamDataExtra.BadgeFrame = 0;
            req.TeamExtra = teamDataExtra.ToString();
            req.MaxMemberCount = levelConfig.MaxMember;

            APIManager.Instance.Send(req, (SModifyTeamInfo resp) =>
            {
                MyTeamInfo.Description = desc;
                MyTeamInfo.RequireLevel = requireLevel;
                MyTeamInfo.Name = teamName;
                MyTeamInfo.ExtraData = teamDataExtra;
                callback?.Invoke(true);
                EventDispatcher.Send(new EventOnTeamInfoChange(MyTeamRequestResult.Success, MyTeamInfo));
            }, (errNo, errMsg, resp) =>
            {
                callback?.Invoke(false);
                ShowErrorNotice(errNo, errMsg);
            });
        }
        
        /// <summary>
        /// 上传玩家数据
        /// </summary>
        public void UploadMyInfo()
        {
            if (!HasTeam())
                return;
            CUpdatePlayerInfo req = new CUpdatePlayerInfo();
            req.PlayerLevel = MyLevel;
            req.PlayerName = MyName;
            req.PlayerExtra = PlayerInfoExtra.GetMyPlayerInfoExtra().ToString();
            APIManager.Instance.Send(req, (SUpdatePlayerInfo resp) =>
                {
                    Debug.LogError("玩家信息上传成功");
                },
                (errNo, errMsg, resp) =>
            {
                ShowErrorNotice(errNo, errMsg);
            });
        }
        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="callback"></param>
        public void JoinTeam(long teamId,Action<bool> callback)
        {
            CJoinTeam req = new CJoinTeam();
            req.TeamId = teamId;
            req.PlayerLevel = MyLevel;
            req.PlayerName = MyName;
            req.PlayerExtra = PlayerInfoExtra.GetMyPlayerInfoExtra().ToString();
            
            APIManager.Instance.Send(req, (SJoinTeam resp) =>
            {
                ClearAllChats();
                ClearAllPassGifts();
                MyTeamID = teamId;
                Storage.LastTeamId = MyTeamID;
                MyTeamName = resp.TeamInfo.Name;
                Storage.LastTeamName = MyTeamName;
                MyTeamBadgeID = resp.TeamInfo.Badge;
                MyTeamInfo = new TeamData(resp.TeamInfo);
                callback?.Invoke(true);
                SendTeamPing(value =>
                {
                    if (!value)
                    {
                        Storage.LastPingTeamTimestamp = 0;
                    }
                });
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTeamJoin,data1:teamId.ToString());

            }, (errNo, errMsg, resp) =>
            {
                callback?.Invoke(false);
                ShowErrorNotice(errNo, errMsg);
            });
        }
        
        /// <summary>
        /// 离开队伍
        /// </summary>
        /// <param name="callback"></param>
        public void LeaveTeam(Action<bool> callback)
        {
            if (MyTeamID <= 0)
            {
                callback?.Invoke(false);
                return;
            }

            CLeaveTeam req = new CLeaveTeam();
            req.TeamId = MyTeamID;

            APIManager.Instance.Send(req, (SLeaveTeam resp) =>
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTeamQuit,data1:Storage.LastTeamId.ToString(),data2:"Leave");
                MyTeamID = 0;
                Storage.LastTeamId = 0;
                Storage.LastTeamName = "";
                MyTeamInfo = null;
                MyTeamName = "";
                MyTeamBadgeID = 0;
                ClearAllChats();
                ClearAllPassGifts();
                callback?.Invoke(true);
            
            }, (errNo, errMsg, resp) =>
            {
                callback?.Invoke(false);
                ShowErrorNotice(errNo, errMsg);
            });
        }
        
        /// <summary>
        /// 踢出成员
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="callback"></param>
        public void KickMember(long playerId,Action<bool> callback)
        {
            if (MyTeamID <= 0)
            {
                callback?.Invoke(false);
                return;
            }

            CKickMember req = new CKickMember();
            req.TeamId = MyTeamID;
            req.PlayerId = playerId;

            APIManager.Instance.Send(req, (SKickMember resp) =>
            {
                if (MyTeamInfo != null)
                {
                    MyTeamInfo.RemovePlayer(playerId);
                    // GetTeamChatImmediately = true;
                    callback?.Invoke(true);
                    GetTeamChats(LoadChatsType.Forward,null);
                    
                    return;
                }
                callback?.Invoke(false);

            }, (errNo, errMsg, resp) =>
            {
                ShowErrorNotice(errNo, errMsg);
                callback?.Invoke(false);
            });
        }

        /// <summary>
        /// 获取对话ID
        /// </summary>
        /// <returns></returns>
        private ChannelID GetChatChannelId()
        {
            var channelId = new ChannelID();
            // channelId.AppId = ConfigurationController.Instance.ChatAppId;
            channelId.ChannelType = ChannelType.Team;
            channelId.TargetId = MyTeamID;
            return channelId;
        }
        
        /// <summary>
        /// 获取对话列表
        /// </summary>
        /// <param name="loadType"></param>
        /// <param name="callback"></param>
        public void GetTeamChats(LoadChatsType loadType,Action<bool> callback)
        {
            if (MyTeamID <= 0)
            {
                callback?.Invoke(true);
                return;
            }

            CGetChannelChats req = new CGetChannelChats();
            req.ChannelId = GetChatChannelId();
            req.LoadType = loadType;
            req.MinChatId = UpdateMinChatID;
            req.MaxChatId = UpdateMaxChatID;
            req.LastTimestamp = LastChatTimestamp;

            APIManager.Instance.Send(req, (SGetChannelChats resp) =>
                {
                    bool added = false;
                    bool isChange = false;
                    if (resp.UpdatedContents.Count > 0)
                    {
                        isChange |= UpdateChatMessageList(resp.UpdatedContents);
                        added = true;
                    }

                    if (resp.AddedContents.Count > 0)
                    {
                        isChange |= UpdateChatMessageList(resp.AddedContents);
                        added = true;
                    }

                    if (added)
                    {
                        LastChatTimestamp = resp.Timestamp;
                        IsChatMessageListUpdate = true;
                    }

                    EventDispatcher.Instance.SendEventImmediately(new EventTeamChatUpdate(isChange));
                    callback?.Invoke(isChange);
                },
                (errNo, errMsg, resp) =>
                {
                    callback?.Invoke(false);
                    ShowErrorNotice(errNo, errMsg);
                    if (errMsg.Contains("CHAT_CHANNEL_NOT_EXIST_ERROR"))
                    {
                        GetMyTeamDetailInfo(value =>
                        {
                            EventDispatcher.Send(new EventOnTeamInfoChange(value, MyTeamInfo));
                        });
                    }
                }
            );
        }
        
        /// <summary>
        /// 发送对话消息
        /// </summary>
        /// <param name="message"></param>
        public void SendTeamChat(string message, Action<bool> callback=null)
        {
            if (MyTeamID <= 0)
            {
                return;
            }

            CSendChannelChat req = new CSendChannelChat();
            req.Content = new SendContent();
            req.Content.UserId = MyPlayerId;
            req.Content.Content = message;
            req.ChannelId = GetChatChannelId();
            req.UserHead = MyPortraitId.ToString();
            req.UserName = MyName;
            req.ChatType = ChatType.UserMessage;
            req.ContentType = (int)ContentType.Text;
            req.Extra = PlayerInfoExtra.GetMyPlayerInfoExtra().ToString();

            APIManager.Instance.Send(req, (SSendChannelChat resp) =>
                {
                    LastChatTimestamp = resp.Timestamp;
                    callback?.Invoke(true);
                },
                (errNo, errMsg, resp) =>
                {
                    ShowErrorNotice(errNo, errMsg);
                    callback?.Invoke(false);
                }
            );
        }
        
        /// <summary>
        /// 给team服务器发送ping消息,刷新最后在线时间用
        /// </summary>
        public void SendTeamPing(Action<bool> callback)
        {
            CTeamPing req = new CTeamPing();
            APIManager.Instance.Send(req, (STeamPing resp) =>
                {
                    callback?.Invoke(true);
                },
                (errNo, errMsg, resp) =>
                {
                    callback?.Invoke(false);
                    ShowErrorNotice(errNo, errMsg);
                }
            );
        }
        
        /// <summary>
        /// 创建奖励请求
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="totalCount"></param>
        public void CreateBattlePassGift(string activityId, CardPackageExtra extra)
        {
            if (!HasTeam() || !TeamIsUnlock())
            {
                return;
            }

            var curTime = APIManager.Instance.GetServerTime();
            CSendTeamPassGift req = new CSendTeamPassGift();
            req.TeamId = MyTeamID;
            req.ActivityId = activityId + curTime;
            // req.TotalCount = totalCount;
            req.Extra = extra.ToString();
            var dayId = curTime/XUtility.DayTime;
            req.ExpireTime = (dayId+2) * XUtility.DayTime;//明天0点生效,后天0点过时
            
            APIManager.Instance.Send(req, (SSendTeamPassGift resp) =>
                {
                    // InsertPassGiftInfo(resp.Info);
                    GetBattlePassGiftList(null);
                },
                (errNo, errMsg, resp) =>
                {
                    ShowErrorNotice(errNo, errMsg);
                    DebugUtil.LogError($"CreateBattlePassGift error:{errNo},{errMsg}");
                }
            );
        }
        
        /// <summary>
        /// 获取本队伍战令礼包列表
        /// </summary>
        public void GetBattlePassGiftList(Action<bool> callback)
        {
            if (!HasTeam() || !TeamIsUnlock())
            {
                callback?.Invoke(true);
                return;
            }

            CGetPassGiftList req = new CGetPassGiftList();
            req.TeamId = this.MyTeamID;

            APIManager.Instance.Send(req, (SGetPassGiftList resp) =>
            {
                var hasChange = UpdateBattlePassList(resp.GiftList);
                callback?.Invoke(hasChange);
            }, (errNo, errMsg, resp) =>
            {
                callback?.Invoke(false);
                ShowErrorNotice(errNo, errMsg);
            });
        }

        private RepeatedField<TeamPassGiftInfo> PassGiftListLocal = new RepeatedField<TeamPassGiftInfo>();
    }
}