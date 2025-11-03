using System;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.UI
{
    public class PlayerData
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        public long PlayerId { get; set; }

        /// <summary>
        /// 玩家等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 玩家透传信息
        /// </summary>
        public PlayerInfoExtra PlayerInfo;

        /// <summary>
        /// 周帮助次数
        /// </summary>
        public int WeeklyHelps { get; set; }

        /// <summary>
        /// 玩家上次在线时间
        /// </summary>
        public long LastOnlineTimstamp { get; set; }

        /// <summary>
        /// 加入工会时间
        /// </summary>
        public long JoinTimestamp { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// 玩家名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工会中职位
        /// </summary>
        public TeamMemberType PlayerType { get; set; }

        public PlayerData(TeamMemberInfo info)
        {
            PlayerId = info.PlayerId;
            Level = info.Level;
            WeeklyHelps = info.WeeklyHelps;
            LastOnlineTimstamp = info.LastOnlineTimestamp;
            JoinTimestamp = info.JoinTimestamp;
            Score = info.Score;
            Name = info.Name;
            PlayerType = info.MemberType;

            try
            {
                PlayerInfo = PlayerInfoExtra.FromJson(info.PlayerExtra);
            }
            catch (Exception e)
            {
                PlayerInfo = PlayerInfoExtra.GetNormalPlayerInfoExtra();
            }
        }
    }
}