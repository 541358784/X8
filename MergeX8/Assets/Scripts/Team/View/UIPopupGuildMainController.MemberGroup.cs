using System.Collections.Generic;
using DragonPlus;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGuildMainController
{
    public class MemberGroup : MonoBehaviour
    {
        private List<MemberItem> MemberList = new List<MemberItem>();
        public TeamData TeamData;
        public void Init(TeamData teamData)
        {
            TeamData = teamData;
            var defaultItem = transform.Find("Scroll View/Viewport/Content/1");
            defaultItem.gameObject.SetActive(false);
            foreach (var member in MemberList)
            {
                Destroy(member.gameObject);
            }
            MemberList.Clear();
            var playersList = TeamData.PlayerList.DeepCopy();
            var leader = playersList.Find(a => a.PlayerId == TeamData.LeaderId);
            playersList.Remove(leader);
            playersList.Insert(0,leader);
            foreach (var player in playersList)
            {
                var item = Instantiate(defaultItem, defaultItem.parent);
                item.gameObject.SetActive(true);
                var memberItem = item.gameObject.AddComponent<MemberItem>();
                memberItem.Init(player,this);
                MemberList.Add(memberItem);
            }
        }

        public void OnKickMember(PlayerData playerData)
        {
            for (var i = 0; i < MemberList.Count; i++)
            {
                var member = MemberList[i];
                if (member.PlayerData.PlayerId == playerData.PlayerId)
                {
                    MemberList.RemoveAt(i);
                    Destroy(member.gameObject);
                    break;
                }
            }
        }

        public class MemberItem : MonoBehaviour
        {
            public PlayerData PlayerData;
            HeadIconNode HeadIconNode;
            private Text NameText;
            private LocalizeTextMeshProUGUI LevelText;
            private Button KickBtn;
            private Button ViewBtn;
            public void Init(PlayerData playerData,MemberGroup group)
            {
                PlayerData = playerData;
                var avatarData = new AvatarViewState(PlayerData.PlayerInfo.AvatarIcon, PlayerData.PlayerInfo.AvatarFrameIcon, PlayerData.PlayerInfo.UserName, false);
                HeadIconNode = HeadIconNode.BuildHeadIconNode(transform.Find("HeadGroup/Head") as RectTransform, avatarData);
                transform.Find("HeadGroup/BG").gameObject.SetActive(false);
                NameText = transform.Find("NameText").GetComponent<Text>();
                NameText.text = PlayerData.PlayerInfo.UserName;
                LevelText = transform.Find("Lv/LvText").GetComponent<LocalizeTextMeshProUGUI>();
                LevelText.SetText(PlayerData.PlayerInfo.Level.ToString());
                KickBtn = transform.Find("ButtonKick").GetComponent<Button>();
                KickBtn.gameObject.SetActive(TeamManager.Instance.IsTeamLeader());
                KickBtn.onClick.AddListener(() =>
                {
                    UIPopupGuildKickTipController.Open((b) =>
                    {
                        if (!b)
                            return;
                        if (PlayerData.PlayerId != (long)TeamManager.Instance.MyPlayerId)
                        {
                            TeamManager.Instance.KickMember(PlayerData.PlayerId, (s) =>
                            {
                                if (!s)
                                    Debug.LogError("踢人失败");
                                else
                                {
                                    group.OnKickMember(PlayerData);
                                }
                            });   
                        }
                        else
                        {
                            TeamManager.Instance.LeaveTeam((s) =>
                            {
                                if (!s)
                                    Debug.LogError("踢人失败");
                                else
                                {
                                    Instance?.AnimCloseWindow();
                                }
                            });
                        }
                    });
                });
                ViewBtn = transform.Find("ButtonView").GetComponent<Button>();
                ViewBtn.onClick.AddListener(() =>
                {
                    UIPopupTeamMemberInfoController.Open(PlayerData.PlayerInfo);
                });
                transform.Find("BG").gameObject.SetActive(group.TeamData.LeaderId != PlayerData.PlayerId);
                transform.Find("BGBoss").gameObject.SetActive(group.TeamData.LeaderId == PlayerData.PlayerId);
                transform.Find("Lable").gameObject.SetActive(!TeamManager.Instance.IsTeamLeader());
                transform.Find("Lable/Boss").gameObject.SetActive(group.TeamData.LeaderId == PlayerData.PlayerId);
                transform.Find("Lable/Me").gameObject.SetActive((long)TeamManager.Instance.MyPlayerId == PlayerData.PlayerId);
            }
        }
    }
}