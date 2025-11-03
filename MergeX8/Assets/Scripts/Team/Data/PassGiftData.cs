using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.Protocol;
using Google.Protobuf.Collections;
using Newtonsoft.Json;

namespace Scripts.UI
{
    public class PassGiftData
    {
        /// <summary>
        /// 礼包唯一ID
        /// </summary>
        public long GiftId { get; set; }
        /// <summary>
        /// 工会ID
        /// </summary>
        public long TeamId { get; set; }
        /// <summary>
        /// 发送礼包的玩家Id
        /// </summary>
        public long PlayerId { get; set; }
        /// <summary>
        /// 发送礼包的玩家名字
        /// </summary>
        public string PlayerName { get; set; }
        
        public long ExpireTime{ get; set; }
        // /// <summary>
        // /// 礼包数量
        // /// </summary>
        // public int TotalCount { get; set; }
        // /// <summary>
        // /// 已经领取过的玩家id列表
        // /// </summary>
        // public List<long> ClaimedPlayerList { get; set; }
        /// <summary>
        /// 礼包发送的时间
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 玩家透传信息
        /// </summary>
        public PlayerInfoExtra PlayerInfo;
        /// <summary>
        /// 所属活动ID
        /// </summary>
        public string ActivityId { get; set; }
        
        /// <summary>
        ///  卡包透传数据
        /// </summary>
        public CardPackageExtra ExtraData;

        public PassGiftData(TeamPassGiftInfo info)
        {
            GiftId = info.GiftId;
            TeamId = info.TeamId;
            PlayerId = info.PlayerId;
            PlayerName = info.PlayerName;
            // TotalCount = info.TotalCount;
            Timestamp = info.Timestamp;
            ActivityId = info.ActivityId;
            ExpireTime = info.ExpireTime;
            
            try
            {
                PlayerInfo = PlayerInfoExtra.FromJson(info.PlayerExtra);
            }
            catch (Exception e)
            {
                PlayerInfo = PlayerInfoExtra.GetNormalPlayerInfoExtra();
            }

            ExtraData = CardPackageExtra.FromJson(info.Extra);

            // ClaimedPlayerList = new List<long>();
            // for (int i = 0; i < info.ClaimedPlayers.Count; i++)
            // {
            //     ClaimedPlayerList.Add((long) info.ClaimedPlayers[i]);
            // }
        }
        
        public bool UpdatePassGiftData(TeamPassGiftInfo info)
        {
            var giftListChange = false;
            // for (int i = 0; i < info.ClaimedPlayers.Count; i++)
            // {
            //     long playerId = (long) info.ClaimedPlayers[i];
            //     if (!ClaimedPlayerList.Contains(playerId))
            //     {
            //         ClaimedPlayerList.Add(playerId);
            //         giftListChange = true;
            //     }
            // }
            
            PlayerName = info.PlayerName;
            try
            {
                PlayerInfo = PlayerInfoExtra.FromJson(info.PlayerExtra);
            }
            catch (Exception e)
            {
                PlayerInfo = PlayerInfoExtra.GetNormalPlayerInfoExtra();
            }
            ExtraData = CardPackageExtra.FromJson(info.Extra);
            return giftListChange;
        }
        
        public bool IsClaimed()
        {
            var giftKey = TeamId+"_"+GiftId+"_"+ ExpireTime;
            return TeamManager.Instance.Storage.ClaimCardState.Contains(giftKey);
        }
        
        // public bool MorgeClaimedPlayer(RepeatedField<ulong> claimedPlayers, long playerId)
        // {
        //     for (int i = 0; i < claimedPlayers.Count; i++)
        //     {
        //         if (!ClaimedPlayerList.Contains((long) claimedPlayers[i]))
        //         {
        //             ClaimedPlayerList.Add((long) claimedPlayers[i]);
        //         }
        //     }
        //     
        //     if (!ClaimedPlayerList.Contains(playerId))
        //     {
        //         ClaimedPlayerList.Add(playerId);
        //     }
        //     // 这里约定 领完了的情况TotalCount 返回的-1
        //     return ClaimedPlayerList.Count >= TotalCount || TotalCount <= 0;
        // }
    }
}