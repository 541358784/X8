using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.UI
{
    public partial class TeamManager
    {
        public ulong MinChatID { get; set; }
        public ulong MaxChatID { get; set; }
        public ulong UpdateMinChatID { get; set; }
        public ulong UpdateMaxChatID { get; set; }
        private long LastChatTimestamp { get; set; }
        
        public List<RecvContent> ChatMessages { get; set; } // 聊天消息列表
        private List<ulong> ExistChatMessages { get; set; }
        // public bool GetTeamChatImmediately { get; set; } // 是否立即拉取聊天消息
        public bool IsChatMessageListUpdate { get; set; }
        public List<PassGiftData> PassGiftInfoList { get; set; } // 战令礼包列表
        public List<ulong> RemovedPassGiftChatMessage { get; set; }
        
        
        private void ChatsInit()
        {
            MinChatID = ulong.MaxValue;
            MaxChatID = ulong.MinValue;
            UpdateMinChatID = ulong.MaxValue;
            UpdateMaxChatID = ulong.MinValue;
            LastChatTimestamp = 1;
            
            ChatMessages = new List<RecvContent>();
            ExistChatMessages = new List<ulong>();
            // GetTeamChatImmediately = false;
            IsChatMessageListUpdate = false;
            
            PassGiftInfoList = new List<PassGiftData>();
            RemovedPassGiftChatMessage = new List<ulong>();
            CanReceivePassGiftCount = 0;
        }

        public bool UpdateChatMessageList(RepeatedField<RecvContent> chatList)
        {
            bool hasChange = false;
            
            for (int i = 0; i < chatList.Count; i++)
            {
                UpdateMinChatID = Math.Min(UpdateMinChatID, chatList[i].ChatId);
                UpdateMaxChatID = Math.Max(UpdateMaxChatID, chatList[i].ChatId);
            }
            
            var list = SystemChatFilter(chatList);
            ulong newMinId = ulong.MaxValue;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].ChatId < newMinId)
                {
                    newMinId = list[i].ChatId;
                }
            }

            bool cleared = false;

            if (newMinId > MaxChatID && list.Count >= 20)
            {
                // 当前最小的id都比之前最大的要大，说明不连续，需要清空当前记录，直接用最新的
                ClearAllChats();
                cleared = true;
            }
            
            long newPersonnelChangesMessageTimestamp = 0;
            for (var i = 0; i < list.Count; i++)
            {
                RecvContent msg = list[i];
                if (!ExistChatMessages.Contains(msg.ChatId))
                {
                    hasChange = true;
                    ExistChatMessages.Add(msg.ChatId);
                    ChatMessages.Add(msg);
                    MinChatID = Math.Min(MinChatID, msg.ChatId);
                    MaxChatID = Math.Max(MaxChatID, msg.ChatId);
                    UpdateMinChatID = MinChatID;
                    UpdateMaxChatID = MaxChatID;
                    if (msg.ChatType == ChatType.SystemMessage)
                    {
                        if (msg.ContentType is 2 or 3 or 10 && msg.Timestamp > newPersonnelChangesMessageTimestamp)
                            newPersonnelChangesMessageTimestamp = msg.Timestamp;
                    }
                }
            }

            if (newPersonnelChangesMessageTimestamp > Storage.LastPersonnelChangesChatMessageTimestamp)
            {
                //如果有人事变动消息，就重新拉取本公会玩家信息列表
                Storage.LastPersonnelChangesChatMessageTimestamp = newPersonnelChangesMessageTimestamp;
                GetMyTeamDetailInfo(value =>
                {
                    EventDispatcher.Send(new EventOnTeamInfoChange(value, MyTeamInfo));
                });
            }

            if (cleared)
            {
                
                //清空之后，再次拼接战令礼包信息
                var passGiftChatList = FormatPassGiftMessage();
                for (var i = 0; i < passGiftChatList.Count; i++)
                {
                    RecvContent msg = passGiftChatList[i];
                    if (!ExistChatMessages.Contains(msg.ChatId) && !RemovedPassGiftChatMessage.Contains(msg.ChatId))
                    {
                        ExistChatMessages.Add(msg.ChatId);
                        ChatMessages.Add(msg);
                    }
                }
                
                hasChange = true;
            }

            if (hasChange)
            {
                SortAllChatMessage();
                AlignPlayerInfo();
            }

            return hasChange;
        }

        private List<RecvContent> SystemChatFilter(RepeatedField<RecvContent> chatList)
        {
            List<RecvContent> newList = new List<RecvContent>();
            var systemTimeLimit = SystemChatTimeLimit();
            long systemTimeLimitMillisecond = systemTimeLimit * 60 * 60 * 1000;
            long currentTime = (long)APIManager.Instance.GetServerTime();
            foreach (var chat in chatList)
            {
                if (chat.ChatType == ChatType.SystemMessage)
                {
                    if (currentTime > chat.Timestamp * 1000 + systemTimeLimitMillisecond)
                    {
                        // Debug.LogError($"系统消息{chat.ChatId}过期");
                        continue;
                    }
                }

                newList.Add(chat);
            }

            return newList;
        }

        private void ClearAllChats()
        {
            LastChatTimestamp = 1;
            ChatMessages.Clear();
            ExistChatMessages.Clear();
            MaxChatID = ulong.MinValue;
            MinChatID = ulong.MaxValue;
            UpdateMaxChatID = ulong.MinValue;
            UpdateMinChatID = ulong.MaxValue;
        }
        
        private void ClearAllPassGifts()
        {
            PassGiftInfoList.Clear();
            RemovedPassGiftChatMessage.Clear();
        }
        
        private void SortAllChatMessage()
        {
            ChatMessages.Sort(delegate(RecvContent a, RecvContent b) { return (int) (a.Timestamp - b.Timestamp); });
        }

        private void AlignPlayerInfo()
        {
            Dictionary<ulong, RecvContent> tempDic = new Dictionary<ulong, RecvContent>();
            for(int i = ChatMessages.Count - 1; i >= 0; i--)
            {
                var chatMessage = ChatMessages[i];
                if (tempDic.ContainsKey(chatMessage.UserId))
                {
                    chatMessage.UserHead = tempDic[chatMessage.UserId].UserHead;
                    chatMessage.UserName = tempDic[chatMessage.UserId].UserName;
                    chatMessage.Extra = tempDic[chatMessage.UserId].Extra;
                }
                else
                {
                    tempDic.Add(chatMessage.UserId, chatMessage);
                }
            }
        }

        private void IsKickedHandle()
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTeamQuit,data1:Storage.LastTeamId.ToString(),data2:"Kick");
            MyTeamID = 0;
            Storage.LastTeamId = 0;
            Storage.LastTeamName = "";
            MyTeamInfo = null;
            MyTeamName = "";
            MyTeamBadgeID = 0;
            ClearAllChats();
            ClearAllPassGifts();
            EventDispatcher.Send(new EventOnTeamKicked());
            CheckTeamStateFromUpdate = false;
        }

        
        #region 战令礼包消息处理
        public ulong ConvertPassGiftIdToChatId(long helpId)
        {
            return (ulong)(8000000000000000000 + helpId);
        }

        public long ConvertChatIdToPassGiftId(ulong chatId)
        {
            return (long)(chatId - 8000000000000000000);
        }
        
        /// <summary>
        /// 更新礼包信息列表，并且模拟出聊天信息加到聊天信息列表中
        /// </summary>
        /// <param name="passList"></param>
        /// <returns></returns>
        public bool UpdateBattlePassList(RepeatedField<TeamPassGiftInfo> giftList)
        {
            if (giftList == null)
                giftList = PassGiftListLocal;
            else
                PassGiftListLocal = giftList;
            
            bool hasModify = false;
            ClearClaimCardState(giftList);
            var passGifts = SortAndFilterPassList(giftList);
            
            List<long> removedGiftIds = new List<long>();
            List<long> insertGiftIds = new List<long>();
        
            for (int i=0; i< PassGiftInfoList.Count; i++)
            {
                removedGiftIds.Add(PassGiftInfoList[i].GiftId);
            }

            for (int i = 0; i < passGifts.Count; i++)
            {
                var giftData = passGifts[i];
                if (removedGiftIds.Contains(giftData.GiftId))
                {
                    removedGiftIds.Remove(giftData.GiftId);
                }

                var oldGiftInfo = GetPassGift(giftData.GiftId);
                if (oldGiftInfo == null)
                {
                    var newGift = new PassGiftData(giftData);
                    PassGiftInfoList.Add(newGift);
                    if (!insertGiftIds.Contains(newGift.GiftId))
                    {
                        insertGiftIds.Add(newGift.GiftId);
                    }
                }
                else
                {
                    bool isChange = oldGiftInfo.UpdatePassGiftData(giftData);
                    if (isChange)
                    {
                        hasModify = true;
                    }
                }
            }

            bool needSortChat = false;
            if (removedGiftIds.Count > 0)
            {
                for(int i = 0; i < removedGiftIds.Count; i++)
                {
                    var passGiftChatId = ConvertPassGiftIdToChatId(removedGiftIds[i]);
                    if (ExistChatMessages.Contains(passGiftChatId))
                    {
                        RemovePassGiftInfo(removedGiftIds[i]);
                    }
                }
                hasModify = true;
            } 

            if (insertGiftIds.Count > 0)
            {
                for(int i = 0; i < insertGiftIds.Count; i++)
                {
                    var giftChatId = ConvertPassGiftIdToChatId(insertGiftIds[i]);
                    var giftInfo = GetPassGift(insertGiftIds[i]);
                    if (!ExistChatMessages.Contains(giftChatId) && giftInfo != null)
                    {
                        InsertPassGiftChatMessage(giftInfo);
                        needSortChat = true;
                    }
                }
                hasModify = true;
            }

            if (needSortChat)
            {
                SortAllChatMessage();
                AlignPlayerInfo();
            }
            
            return hasModify;
        }
        
        public PassGiftData GetPassGift(long giftId)
        {
            return PassGiftInfoList.Find(x => x.GiftId == giftId);
        }
        
        private void RemovePassGiftChatMessage(long giftId)
        {
            var chatId = ConvertPassGiftIdToChatId(giftId);
            if (!ExistChatMessages.Contains(chatId)) 
                return;

            ExistChatMessages.Remove(chatId);
            RemovedPassGiftChatMessage.Add(chatId);

            for (var i = 0; i < ChatMessages.Count; i++)
            {
                if (ChatMessages[i].ChatId == chatId)
                {
                    ChatMessages.RemoveAt(i);
                    break;
                }
            }
        }
        
        private void InsertPassGiftChatMessage(PassGiftData gift)
        {
            RecvContent msg = new RecvContent();
            var giftChatId = ConvertPassGiftIdToChatId(gift.GiftId);
            msg.ChatId = giftChatId;
            msg.UserId = (ulong)gift.PlayerId;
            msg.ChatType = ChatType.TeamPassGift;
            msg.Timestamp = gift.Timestamp / 1000;
            msg.UserName = gift.PlayerName;
            msg.UserHead = gift.PlayerInfo.AvatarIcon.ToString();
            msg.Extra = JsonConvert.SerializeObject(gift.PlayerInfo);

            if (!RemovedPassGiftChatMessage.Contains(msg.ChatId))
            {
                ExistChatMessages.Add(giftChatId);
                ChatMessages.Add(msg);
            }
        }
        
        private List<RecvContent> FormatPassGiftMessage()
        {
            List<RecvContent> retList = new List<RecvContent>();
            for(int i = 0; i < PassGiftInfoList.Count; i++)
            {
                var gift = PassGiftInfoList[i];
                RecvContent msg = new RecvContent();
                msg.ChatId = ConvertPassGiftIdToChatId(gift.GiftId);
                msg.UserId = (ulong)gift.PlayerId;
                msg.ChatType = ChatType.TeamPassGift;
                msg.Timestamp = gift.Timestamp / 1000;
                msg.UserName = gift.PlayerName;
                msg.UserHead = gift.PlayerInfo.AvatarIcon.ToString();
                msg.Extra = JsonConvert.SerializeObject(gift.PlayerInfo);
                retList.Add(msg);
            }

            return retList;
        }
        
        public void InsertPassGiftInfo(TeamPassGiftInfo passGiftInfo)
        {
            var gift = GetPassGift(passGiftInfo.GiftId);
            if (gift == null)
            {
                var newGift = new PassGiftData(passGiftInfo);
                PassGiftInfoList.Add(newGift);

                var giftChatId = ConvertPassGiftIdToChatId(newGift.GiftId);
                if (!ExistChatMessages.Contains(giftChatId))
                {
                    InsertPassGiftChatMessage(newGift);
                }
            }
            else
            {
                gift.UpdatePassGiftData(passGiftInfo);
            }
            
            SortAllChatMessage();
            AlignPlayerInfo();
        }
        
        /// <summary>
        /// 排序和筛选：玩家的消息列表中，未领取的共享礼物，有多少都展示多少，若数量小于5个，则会按时间最近的已领取的共享礼物也会展示在列表中。已领取最大展示5个。
        /// </summary>
        /// <param name="passList"></param>
        /// <returns></returns>
        private List<TeamPassGiftInfo> SortAndFilterPassList(RepeatedField<TeamPassGiftInfo> list)
        {
            List<TeamPassGiftInfo> passList = list.ToList();
            Dictionary<long, List<TeamPassGiftInfo>> userDic = new Dictionary<long, List<TeamPassGiftInfo>>();
            Dictionary<long, float> passValueDic = new Dictionary<long, float>();//记录value避免重复转json
            List<TeamPassGiftInfo> notClaimList = new List<TeamPassGiftInfo>();
            var curTheme = CardCollectionModel.Instance.ThemeInUse.GetUpGradeTheme();
            if (curTheme == null)
                return notClaimList;
            // ulong playerId = MyPlayerId;
            CanReceivePassGiftCount = 0;
            var curTime = (long)APIManager.Instance.GetServerTime();
            for (int i = 0; i < passList.Count; i++)
            {
                var pass = passList[i];
                var cardData = CardPackageExtra.FromJson(pass.Extra);
                passValueDic.Add(pass.GiftId, cardData.CardTotalValue);
                var cardThemeId = CardCollectionModel.Instance.TableCardPackage[cardData.CardPackageId].ThemeId;
                var themeLink = CardCollectionModel.Instance.GetCardThemeLink(cardThemeId);
                if (!themeLink.Contains(curTheme.CardThemeConfig.Id))//不属于当期卡册的过滤掉
                    continue;
                // if (curTime > pass.ExpireTime || curTime < (pass.ExpireTime-(long)XUtility.DayTime))//生效时间过滤
                // {
                //     continue;
                // }

                userDic.TryAdd(pass.PlayerId, new List<TeamPassGiftInfo>());
                userDic[pass.PlayerId].Add(pass);
            }

            foreach (var pair in userDic)//每个成员选出价值最高的卡包
            {
                var pass = pair.Value[0];
                for (var i = 1; i < pair.Value.Count; i++)
                {
                    if (passValueDic[pair.Value[i].GiftId] > passValueDic[pass.GiftId])
                    {
                        pass = pair.Value[i];
                    }
                }

                var giftKey = pass.TeamId+"_"+pass.GiftId+"_"+pass.ExpireTime;
                if (Storage.ClaimCardState.Contains(giftKey))//已领取
                {
                    continue;
                }
                notClaimList.Add(pass);
            }
            notClaimList.Sort((x1, x2) => x2.Timestamp.CompareTo(x1.Timestamp));
            EventDispatcher.Send(new EventOnTeamRedPointRefresh());
            return notClaimList;
        }

        public void ClearClaimCardState(RepeatedField<TeamPassGiftInfo> field)
        {
            var list = field.ToList();
            var left = new List<string>();

            var myTeamId = TeamManager.Instance.MyTeamID;
            var curTime = (long)APIManager.Instance.GetServerTime();
            for (var i = 0; i < Storage.ClaimCardState.Count; i++)
            {
                var parts = Storage.ClaimCardState[i].Split('_');
                if (parts.Length < 3)
                    continue;
                var teamId = long.Parse(parts[0]);
                var giftId = long.Parse(parts[1]);
                var expireTime = long.Parse(parts[2]);
                if (curTime > expireTime)
                    continue;
                if (teamId != myTeamId)
                {
                    left.Add(Storage.ClaimCardState[i]);
                    continue;
                }
                var gift = list.Find(a => a.GiftId == giftId);
                if (gift != null)
                {
                    left.Add(Storage.ClaimCardState[i]);
                }
            }
            Storage.ClaimCardState.Clear();
            foreach (var giftId in left)
            {
                Storage.ClaimCardState.Add(giftId);
            }
        }
        
        public bool RemovePassGiftInfo(long giftId)
        {
            var giftData = GetPassGift(giftId);
            if (giftData != null)
            {
                PassGiftInfoList.Remove(giftData);
                RemovePassGiftChatMessage(giftId);
                return true;
            }

            return false;
        }
        
        // public bool UpdatePassGiftClaimState(TeamPassGiftInfo info, long playerId)
        // {
        //     bool claimEnd = false;
        //     var giftData = GetPassGift(info.GiftId);
        //     if (giftData != null)
        //     {
        //         claimEnd = giftData.MorgeClaimedPlayer(info.ClaimedPlayers, playerId);
        //     }
        //
        //     return claimEnd || info.TotalCount <= 0;
        // }
        
        #endregion
    }
}