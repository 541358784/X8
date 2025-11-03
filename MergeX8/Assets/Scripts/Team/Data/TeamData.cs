using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.UI
{
    public class TeamData
    {
        /// <summary>
        /// 工会ID
        /// </summary>
        public long TeamId { get; set; }

        /// <summary>
        /// 工会徽章ID（十位是底色ID，个位是图章ID）
        /// </summary>
        public int Badge { get; set; }

        /// <summary>
        /// 加入工会的最低要求等级
        /// </summary>
        public int RequireLevel { get; set; }

        /// <summary>
        /// 周帮助数
        /// </summary>
        public int WeeklyHelps { get; set; }

        /// <summary>
        /// 队长ID
        /// </summary>
        public long LeaderId { get; set; }

        /// <summary>
        /// 工会名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工会描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 工会加入类型
        /// </summary>
        public TeamJoinType JoinType { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// 会员信息
        /// </summary>
        public List<PlayerData> PlayerList { get; set; }
        
        /// <summary>
        /// 我的信息
        /// </summary>
        public PlayerData MyData { get; set; }

        /// <summary>
        /// 我的信息在队列中的位置
        /// </summary>
        public int MyDataIndex { get; set; }

        public int MemberMaxCount => ExtraData.MemberMaxCount;
        public int TeamLevel => ExtraData.TeamLevel;
        public int BadgeFrame => ExtraData.BadgeFrame;
        // public int Exp => ExtraData.Exp;
        public TeamDataExtra ExtraData;
        public int ServerMemberMaxCount { get; set; }
        public TeamData(TeamInfoWithMembers info)
        {
            TeamId = info.TeamId;
            Badge = info.Badge;
            RequireLevel = info.RequireLevel;
            WeeklyHelps = info.WeeklyHelps;
            LeaderId = info.LeaderId;
            Name = info.Name;
            Description = info.Description;
            JoinType = info.JoinType;
            Score = 0;
            ExtraData = TeamDataExtra.FromJson(info.TeamExtra);
            PlayerList = new List<PlayerData>();
            MyData = null;
            MyDataIndex = 0;
            for (int i = 0; i < info.Members.Count; i++)
            {
                var playerData = new PlayerData(info.Members[i]);
                Score += playerData.Level;
                PlayerList.Add(playerData);
                if (playerData.PlayerId == (long)TeamManager.Instance.MyPlayerId)
                {
                    MyData = playerData;
                    MyDataIndex = i;
                }
            }
            ServerMemberMaxCount = info.MemberCountMax;
        }

        public void RemovePlayer(long playerId)
        {
            var playerData = PlayerList.Find(x => x.PlayerId == playerId);
            if (playerData != null)
            {
                Score -= playerData.Level;
                PlayerList.Remove(playerData);
            }
            
            MyData = null;
            MyDataIndex = 0;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].PlayerId == (long)TeamManager.Instance.MyPlayerId)
                {
                    MyData = PlayerList[i];
                    MyDataIndex = i;
                }
            }
        }

        public void Sort()
        {
            PlayerList.Sort((x1, x2) => { return x2.Level - x1.Level; });
            MyData = null;
            MyDataIndex = 0;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].PlayerId == (long)TeamManager.Instance.MyPlayerId)
                {
                    MyData = PlayerList[i];
                    MyDataIndex = i;
                }
            }
        }

        public bool IsKicked(long playerId)
        {
            var playerData = PlayerList.Find(x => x.PlayerId == playerId);
            return playerData == null;
        }

        public bool IsMyTeam()
        {
            return MyData != null;
        }
    }
}
