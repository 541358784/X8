using System.Collections.Generic;
using DragonPlus;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGuildJoinPreviewController : UIWindowController
{
    public static UIPopupGuildJoinPreviewController Instance;
    public static UIPopupGuildJoinPreviewController Open(TeamData teamData)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildJoinPreview,teamData) as UIPopupGuildJoinPreviewController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        DescText = GetItem<Text>("Root/Guild/Text");
        Icon = TeamIconNode.BuildTeamIconNode(GetItem<RectTransform>("Root/Guild/GuildIcon"),TeamData.GetViewState());
        LevelText = GetItem<LocalizeTextMeshProUGUI>("Root/Guild/LvText");
        RequireLevelText = GetItem<LocalizeTextMeshProUGUI>("Root/Guild/Lv/Text (2)");
        NameText = GetItem<Text>("Root/Guild/NameText");
        TeamIdText = GetItem<LocalizeTextMeshProUGUI>("Root/Guild/Id/Text (2)");
        MemberCountText = GetItem<LocalizeTextMeshProUGUI>("Root/Guild/Num/Text (2)");
        JoinBtn = GetItem<Button>("Root/Guild/Button");
        JoinBtn.onClick.AddListener(() =>
        {
            UIPopupGuildJoinTipController.Open((b) =>
            {
                if (!b)
                    return;
                WaitingManager.Instance.OpenWindow(5f);
                TeamManager.Instance.JoinTeam(TeamData.TeamId, (s) =>
                {
                    WaitingManager.Instance.CloseWindow();
                    if (!s)
                    {
                        Debug.LogError("加入公会失败");
                    }
                    else
                    {
                        Instance.AnimCloseWindow();
                        UIPopupGuildJoinController.Instance.AnimCloseWindow();
                        UIPopupGuildMainController.Open();
                    }
                });
            });
        });
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }

    public TeamData TeamData;
    private Text DescText;
    private TeamIconNode Icon;
    private LocalizeTextMeshProUGUI LevelText;
    private LocalizeTextMeshProUGUI RequireLevelText;
    private Text NameText;
    private LocalizeTextMeshProUGUI TeamIdText;
    private LocalizeTextMeshProUGUI MemberCountText;
    private Button JoinBtn;
    private List<TeamMemberItem> MemberItemList = new List<TeamMemberItem>();
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        TeamData = objs[0] as TeamData;
        DescText.text = TeamData.Description;
        Icon.SetTeamIconViewState(TeamData.GetViewState());
        LevelText.SetText(TeamData.TeamLevel.ToString());
        RequireLevelText.SetText(TeamData.RequireLevel.ToString());
        NameText.text = TeamData.Name;
        TeamIdText.SetText(TeamData.TeamId.ToString());
        MemberCountText.SetText(TeamData.PlayerList.Count + "/" + TeamData.MemberMaxCount);
        JoinBtn.interactable = TeamData.PlayerList.Count < TeamData.MemberMaxCount;
        var defaultItem = transform.Find("Root/Scroll View/Viewport/Content/1");
        defaultItem.gameObject.SetActive(false);
        foreach (var playerData in TeamData.PlayerList)
        {
            var item = Instantiate(defaultItem, defaultItem.parent);
            item.gameObject.SetActive(true);
            var memberItem = item.gameObject.AddComponent<TeamMemberItem>();
            memberItem.Init(playerData);
            MemberItemList.Add(memberItem);
        }
    }
}