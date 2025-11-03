using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGuildLevelUpController:UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    
    public static UIPopupGuildLevelUpController Instance;
    public static UIPopupGuildLevelUpController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildLevelUp) as UIPopupGuildLevelUpController;
        return Instance;
    }

    private LocalizeTextMeshProUGUI PriceText;
    private LocalizeTextMeshProUGUI PriceTextGray;
    
    private TeamData TeamData;
    private int curLevel;
    TableTeamLevelConfig curLevelConfig=>TeamConfigManager.Instance.LevelConfigList.Find(a => a.Id == curLevel);
    TableTeamLevelConfig nextLevelConfig=>TeamConfigManager.Instance.LevelConfigList.Find(a => a.Id == curLevel+1);
    private LocalizeTextMeshProUGUI LevelTextNow;
    private LocalizeTextMeshProUGUI LevelTextNext;
    private TeamIconNode TeamIcon;
    private Text TeamNameText;
    private LocalizeTextMeshProUGUI MemberNow;
    private LocalizeTextMeshProUGUI MemberNext;
    private LocalizeTextMeshProUGUI ShopItemNow;
    private LocalizeTextMeshProUGUI ShowItemNext;
    private LocalizeTextMeshProUGUI TaskPercentNow;
    private LocalizeTextMeshProUGUI TaskPercentNext;
    private Button levelUpBtn;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        TeamData = TeamManager.Instance.MyTeamInfo;
        curLevel = TeamData.ExtraData.TeamLevel;
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(() => { AnimCloseWindow();});
        PriceText = transform.Find("Root/LvUp/ButtonGroup/ButtonLvUp/Text").GetComponent<LocalizeTextMeshProUGUI>();
        PriceTextGray = transform.Find("Root/LvUp/ButtonGroup/ButtonLvUp/GreyText").GetComponent<LocalizeTextMeshProUGUI>();
        var itemChangeReason =
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TeamConsume);
        levelUpBtn = transform.Find("Root/LvUp/ButtonGroup/ButtonLvUp").GetComponent<Button>();
        levelUpBtn.onClick.AddListener(() =>
        {
            if (!CanUpgrade())
                return;
            UserData.Instance.ConsumeRes(UserData.ResourceId.Coin,curLevelConfig.UpPrice,itemChangeReason);
            curLevel++;
            TeamData.ExtraData.TeamLevel = curLevel;
            TeamManager.Instance.PushMyTeamInfo();
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTeamLevelup,data1:TeamData.TeamId.ToString() ,data2:curLevel.ToString());
            UpdateView();
            if (curLevelConfig.UpPrice <=0)
                AnimCloseWindow();
        });
        levelUpBtn.interactable = CanUpgrade();
        LevelTextNow = transform.Find("Root/LvUp/Lv/LvText1").GetComponent<LocalizeTextMeshProUGUI>();
        LevelTextNext = transform.Find("Root/LvUp/Lv/LvText2").GetComponent<LocalizeTextMeshProUGUI>();
        TeamIcon = TeamIconNode.BuildTeamIconNode(transform.Find("Root/LvUp/Lv/GuildIcon") as RectTransform,
            TeamData.GetViewState());
        transform.Find("Root/LvUp/Lv/GuildBG").gameObject.SetActive(false);
        TeamNameText = transform.Find("Root/LvUp/Lv/Text").GetComponent<Text>();
        MemberNow = transform.Find("Root/LvUp/1/NumText1").GetComponent<LocalizeTextMeshProUGUI>();
        MemberNext = transform.Find("Root/LvUp/1/NumText2").GetComponent<LocalizeTextMeshProUGUI>();
        ShopItemNow = transform.Find("Root/LvUp/2/NumText1").GetComponent<LocalizeTextMeshProUGUI>();
        ShowItemNext = transform.Find("Root/LvUp/2/NumText2").GetComponent<LocalizeTextMeshProUGUI>();
        TaskPercentNow = transform.Find("Root/LvUp/3/NumText1").GetComponent<LocalizeTextMeshProUGUI>();
        TaskPercentNext = transform.Find("Root/LvUp/3/NumText2").GetComponent<LocalizeTextMeshProUGUI>();
        UpdateView();

        DisableShieldBtn();
    }

    public bool CanUpgrade()
    {
        return curLevelConfig.UpPrice > 0 &&
               UserData.Instance.CanAford(UserData.ResourceId.Coin, curLevelConfig.UpPrice);
    }

    public void UpdateView()
    {
        PriceText.SetTermFormats(curLevelConfig.UpPrice.ToString());
        PriceTextGray.SetTermFormats(curLevelConfig.UpPrice.ToString());
        TeamIcon.SetTeamIconViewState(TeamData.GetViewState());
        TeamNameText.text = TeamData.Name;
        LevelTextNow.SetText("Lv"+curLevel);
        MemberNow.SetText(curLevelConfig.MaxMember.ToString());
        ShopItemNow.SetText(curLevelConfig.ShopContentCount.ToString());
        TaskPercentNow.SetText(((int)(curLevelConfig.TaskAddition*100f))+"%");
        if (nextLevelConfig != null)
        {
            LevelTextNext.SetText("Lv"+(curLevel+1));
            MemberNext.SetText(nextLevelConfig.MaxMember.ToString());
            ShowItemNext.SetText(nextLevelConfig.ShopContentCount.ToString());
            TaskPercentNext.SetText(((int)(nextLevelConfig.TaskAddition*100f))+"%");
        }
        else
        {
            LevelTextNext.SetTerm("UI_max");
            MemberNext.SetTerm("UI_max");
            ShowItemNext.SetTerm("UI_max");
            TaskPercentNext.SetTerm("UI_max");
            PriceText.SetTerm("UI_max");
            PriceTextGray.SetTerm("UI_max");
            levelUpBtn.gameObject.SetActive(false);
        }
        levelUpBtn.interactable = CanUpgrade();
    }
}