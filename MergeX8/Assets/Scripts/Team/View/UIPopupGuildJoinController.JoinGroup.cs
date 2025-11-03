using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Scripts.UI;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGuildJoinController
{
    public class JoinGroup : MonoBehaviour
    {
        public InputField TeamName;
        private ScrollTopDetector scrollRectTrigger;
        public void Init()
        {
            defaultItem = transform.Find("Scroll View/Viewport/Content/1");
            defaultItem.gameObject.SetActive(false);
            TeamName = transform.Find("Search/Input").GetComponent<InputField>();
            TeamName.characterLimit = 20;
            transform.Find("Search/X").GetComponent<Button>().onClick.AddListener(() =>
            {
                TeamName.SetTextWithoutNotify("");
            });
            transform.Find("Search/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (TeamName.text.IsEmptyString())
                {
                    TeamManager.Instance.GetRecommendTeamList((s) =>
                    {
                        if (!s)
                            Debug.LogError("推荐公会获取失败，请重新获取");
                        else
                        {
                            UpdateTeamList(TeamManager.Instance.RecommendTeamList);
                        }
                    });
                }
                else
                {
                    TeamManager.Instance.SearchTeams(TeamName.text, (s) =>
                    {
                        if (!s)
                            Debug.LogError("查找公会失败，请重新查找");
                        else
                        {
                            UpdateTeamList(TeamManager.Instance.SearchTeamList);
                        }
                    });
                }
            });
            UpdateTeamList(TeamManager.Instance.RecommendTeamList);
            TeamName.SetTextWithoutNotify("");
            TeamManager.Instance.GetRecommendTeamList((s) =>
            {
                if (!s)
                    Debug.LogError("推荐公会获取失败，请重新获取");
                else
                {
                    UpdateTeamList(TeamManager.Instance.RecommendTeamList);
                }
            });
            
            scrollRectTrigger = transform.Find("Scroll View").gameObject.AddComponent<ScrollTopDetector>();
            scrollRectTrigger.OnOverBottom(() =>
            {
                LoadMore();
            });
        }

        public List<TeamSimpleView> TeamViewList = new List<TeamSimpleView>();
        public int PageItemCount = 20;
        private Transform defaultItem;
        private List<TeamInfo> TeamList;
        public void UpdateTeamList(List<TeamInfo> teamList)
        {
            TeamList = teamList;
            foreach (var teamView in TeamViewList)
            {
                DestroyImmediate(teamView.gameObject);
            }
            TeamViewList.Clear();
            for (var i = 0; i < PageItemCount; i++)
            {
                if (i >= TeamList.Count)
                    break;
                var item = CreateItem(TeamList[i]);
                TeamViewList.Add(item);
            }
        }

        public TeamSimpleView CreateItem(TeamInfo info)
        {
            var teamViewItem = Instantiate(defaultItem, defaultItem.parent);
            teamViewItem.gameObject.SetActive(true);
            var teamView = teamViewItem.gameObject.AddComponent<TeamSimpleView>();
            teamView.SetTeamInfo(info);
            return teamView;
        }

        public void LoadMore()
        {
            var startIndex = TeamViewList.Count;
            var endIndex = startIndex + PageItemCount;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (i >= TeamList.Count)
                    break;
                var item = CreateItem(TeamList[i]);
                TeamViewList.Add(item);
            }
        }

        public class TeamSimpleView : MonoBehaviour
        {
            private TeamIconNode GuildIcon;
            private Text NameText;
            private LocalizeTextMeshProUGUI NumText;
            private Button Button;
            private TeamInfo TeamInfo;
            private TeamDataExtra ExtraData;
            public void Awake()
            {
                GuildIcon = TeamIconNode.BuildTeamIconNode(transform.Find("GuildIcon") as RectTransform,TeamInfo.GetViewState());
                NameText = transform.Find("NameText").GetComponent<Text>();
                NumText = transform.Find("Num/NumText").GetComponent<LocalizeTextMeshProUGUI>();
                Button = transform.Find("Button").GetComponent<Button>();
                Button.onClick.AddListener(() =>
                {
                    if (ExtraData.MemberMaxCount > TeamInfo.MemberCount)
                    {
                        WaitingManager.Instance.OpenWindow(5f);
                        TeamManager.Instance.GetOtherTeamDetailInfo(TeamInfo.TeamId, (s,teamData) =>
                        {
                            WaitingManager.Instance.CloseWindow();
                            if (s)
                                UIPopupGuildJoinPreviewController.Open(teamData);
                            else
                            {
                                Debug.LogError("公会详情信息获取失败，请重新获取");
                            }
                        });
                    }
                    else
                    {
                        Debug.LogError("人满了，不让进");
                    }
                });
            }

            public void SetTeamInfo(TeamInfo teamInfo)
            {
                TeamInfo = teamInfo;
                ExtraData = TeamDataExtra.FromJson(TeamInfo.TeamExtra);
                GuildIcon.SetTeamIconViewState(TeamInfo.GetViewState());
                NameText.text = TeamInfo.Name;
                NumText.SetText(TeamInfo.MemberCount + "/" + ExtraData.MemberMaxCount);
            }
        }
    }
}