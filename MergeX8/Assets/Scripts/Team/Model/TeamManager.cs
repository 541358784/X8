using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Merge.Order;
using SomeWhere;
using SRF;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.UI
{
    public partial class TeamManager
    {
        public static TeamManager Instance { get; } = new();

        public StorageTeam Storage => StorageManager.Instance.GetStorage<StorageHome>().Team;
        private TeamConfig RemoteTeamConfig { get; set; }
        public long MyTeamID { get; set; }
        public long NextAskHelpTimestamp { get; set; }
        public int CanHelpCount { get; set; }
        public InitNetworkState InitNetwork { get; set; }
        public List<TeamInfo> RecommendTeamList { get; set; }
        public List<TeamInfo> OriginalRecommendTeamList { get; set; }
        public TeamData MyTeamInfo { get; set; }
        public List<TeamInfo> SearchTeamList { get; set; }
        public ulong MyPlayerId => StorageManager.Instance.GetStorage<StorageCommon>().PlayerId;
        public int MyPortraitId => StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId;
        public int MyHeadFrameId => StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconFrameId;
        public string MyName => StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName;
        public int MyLevel => ExperenceModel.Instance.GetLevel();

        public TableTeamLevelConfig MyTeamLevelConfig => (HasTeam() && MyTeamInfo != null)
            ? TeamConfigManager.Instance.LevelConfigList.Find(a => a.Id == MyTeamInfo.ExtraData.TeamLevel):null;
        public string MyTeamName { get; set; }
        public int MyTeamBadgeID { get; set; }
        public int CanReceivePassGiftCount { get; set; }
        public bool CheckTeamStateFromUpdate;
        private float _deltaTime;
        public TeamManager()
        {
        }

        public void Init()
        {
            ChatsInit();
            CheckTeamStateFromUpdate = false;
            _deltaTime = 0;
            InitNetwork = InitNetworkState.Nothing;
            RecommendTeamList = new List<TeamInfo>();
            SearchTeamList = new List<TeamInfo>();
            GetMyTeamInfo();
            EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD,OnEventPlayerInfoChange);
            EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_HEAD,OnEventPlayerInfoChange);
            TMatch.Timer.Register(1, UpdateLife, null, true);
            TMatch.Timer.Register(1, UpdateShop, null, true);
            TMatch.Timer.Register(1, UpdateTask, null, true);
        }

        private bool LastHasTeam;
        public void UpdateTask()
        {
            if (InitNetwork == InitNetworkState.Success)
            {
                if (!HasTeam() && LastHasTeam)
                {
                    MainOrderManager.Instance.RemoveOrder(MainOrderType.Team);
                }
                LastHasTeam = HasTeam();
            }
        }
        public bool HasTeam()
        {
            return MyTeamID > 0;
        }

        public bool HasOrder()
        {
            return HasTeam() && MyTeamLevelConfig.ShopContentCount > 0;
        }

        public string GetOrderRefreshTime()
        {
            var left = (long)Storage.RefreshOrderTime - (long)APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;

            return CommonUtils.FormatLongToTimeStr(left);
        }
        // public Sprite GetBadgeBackSprite(int badgeID)
        // {
        //     var iconConfig = IconConfigList.Find(a => a.id == badgeID);
        //     return ResourcesManager.Instance.GetSpriteVariant(iconConfig.iconAtlas, iconConfig.iconName);
        // }

        public Sprite GetBadgeIconSprite(int badgeID)
        {
            var iconConfig = TeamConfigManager.Instance.IconConfigList.Find(a => a.Id == badgeID);
            return ResourcesManager.Instance.GetSpriteVariant(iconConfig.HeadIconAtlas, iconConfig.HeadIconName);
        }

        public bool IsTeamLeader()
        {
            return MyTeamID > 0 && MyTeamInfo.LeaderId == (long)MyPlayerId;
        }
        public bool IsMyTeam(long teamId)
        {
            return MyTeamID > 0 && MyTeamID == teamId;
        }

        public bool IsOwnPlayer(long teamId, long playerId)
        {
            return MyTeamID > 0 && MyTeamID == teamId && (ulong)playerId == MyPlayerId;
        }

        public int GetOfflineDay(long lastTime)
        {
            int day = 0;
            long cur = (long)APIManager.Instance.GetServerTime();
            if (cur > lastTime) //保证现在日期比上次日期大，不然返回0天
            {
                DateTime lastTimeDate = new DateTime(1970, 1, 1).AddMilliseconds(lastTime);
                DateTime curTimeDate = new DateTime(1970, 1, 1).AddMilliseconds(cur);
                if (curTimeDate.Year == lastTimeDate.Year) //没有跨年
                {
                    day = curTimeDate.DayOfYear - lastTimeDate.DayOfYear;
                }
                else //跨年了
                {
                    if (curTimeDate.Year - lastTimeDate.Year == 1)
                    {
                        DateTime lastDayOfLastYear = new DateTime(lastTimeDate.Year, 12, 31); // 获取这一年的最后一天
                        day = lastDayOfLastYear.DayOfYear - lastTimeDate.DayOfYear + curTimeDate.DayOfYear;
                    }
                    else if (curTimeDate.Year - lastTimeDate.Year > 1) //大于1年不计算返回99天
                    {
                        day = 99;
                    }
                    else //保险
                    {
                        day = 0;
                    }
                }
            }

            return day;
        }

        public string GetAskCountDownString(ulong leftTime)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;

            leftTime = leftTime < 0 ? 0 : leftTime;
            second = (int)(leftTime / 1000);

            if (second >= 60)
            {
                minute = second / 60;
                second = second % 60;
            }

            if (minute >= 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }

            // Debug.LogError($"{day}day:{hour}hour:{minute}minute:{second}second");
            return $"{hour:D2}:{minute:D2}:{second:D2}";
        }

        private void OnEventPlayerInfoChange(BaseEvent e)
        {
            if (MyTeamInfo != null)
            {
                var playerId = (long)MyPlayerId;
                for (int i = 0; i < MyTeamInfo.PlayerList.Count; i++)
                {
                    if (MyTeamInfo.PlayerList[i].PlayerId == playerId)
                    {
                        MyTeamInfo.PlayerList[i].Level = MyLevel;
                        MyTeamInfo.PlayerList[i].Name = MyName;
                        MyTeamInfo.PlayerList[i].PlayerInfo = PlayerInfoExtra.GetMyPlayerInfoExtra();
                        break;
                    }
                }

                MyTeamInfo.Sort();
            }
        }

        public bool IsNetworkValid()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public void TryPingTeamServer()
        {
            if (CommonUtils.IsSameDay(APIManager.Instance.GetServerTime(),(ulong)Storage.LastPingTeamTimestamp))
            {
                SendTeamPing(value =>
                {
                    if (value)
                    {
                        Storage.LastPingTeamTimestamp = (long)APIManager.Instance.GetServerTime();
                    }
                });
            }
        }

        public ulong GetAskCountDown()
        {
            ulong lastTime = 0;
            if ((ulong)NextAskHelpTimestamp > APIManager.Instance.GetServerTime())
            {
                lastTime = (ulong)NextAskHelpTimestamp - APIManager.Instance.GetServerTime();
            }

            return lastTime;
        }

        public bool TeamIsUnlock()
        {
            return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Team);
        }

        public bool TeamRedPointEnable()
        {
            bool enable = false;
            if (TeamIsUnlock())
            {
                if (HasTeam())
                {
                    if (CanHelpCount > 0)
                        enable = true;
                    if (CanReceivePassGiftCount > 0)
                        enable = true;
                    if (GetAskCountDown() <= 0)
                        enable = true;
                }
                else
                {
                    enable = true;
                }
            }

            return enable;
        }

        private void ShowErrorNotice(ErrorCode errNo, string errMsg)
        {
#if UNITY_EDITOR
            string titleMegKey = "UI_common_tittle_Lost";
            switch (errNo)
            {
                case ErrorCode.TeamNotExistError:
                    titleMegKey = "公会不存在";
                    break;
                case ErrorCode.TeamMemberFullError:
                    titleMegKey = "没座";
                    break;
                case ErrorCode.TeamNameDuplicateError:
                    titleMegKey = "重名";
                    break;
            }
            Debug.LogError("公会请求失败,错误信息:"+errNo +"------"+errMsg);
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                TitleString = titleMegKey,
                DescString = errNo +"------"+ errMsg,
                HasCancelButton = false,
                HasCloseButton = false,
            });
#endif
        }

        public int GetRandomBadgeId()
        {
            var freeList = new List<int>();
            foreach (var iconConfig in TeamConfigManager.Instance.IconConfigList)
            {
                if (!iconConfig.IsNeedCollect)
                {
                    freeList.Add(iconConfig.Id);
                }
            }
            return freeList.RandomPickOne();
        }

        public void FixedUpdate(float deltaTime)
        {
            _deltaTime += deltaTime;
            if (_deltaTime >= 60)
            {
                if(CheckTeamStateFromUpdate)
                    CheckMyTeamInfo();
                _deltaTime = 0;
            }
        }
    }
}