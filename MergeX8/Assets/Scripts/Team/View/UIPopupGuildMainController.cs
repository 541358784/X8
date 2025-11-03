using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Network.API.Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGuildMainController : UIWindowController
{
    public static UIPopupGuildMainController Instance;
    public static UIPopupGuildMainController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        if (TeamManager.Instance.MyTeamInfo == null)
        {
            Debug.LogError("没有公会，无法打开公会界面");
            return null;   
        }
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildMain) as UIPopupGuildMainController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        
    }
    public enum TabState
    {
        Chat,
        Member,
    }
    public class ViewGroup
    {
        public TabState State;
        public Transform Group;
        public Transform LabelNormal;
        public Transform LabelSelect;
    }
    
    private TabState State = TabState.Chat;
    private Dictionary<TabState, ViewGroup> GroupDic = new Dictionary<TabState, ViewGroup>();
    public ChatGroup Chat;
    public MemberGroup Member;
    private TeamData TeamData;
    private TeamIconNode TeamIcon;
    private Text NameText;
    
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(()=>AnimCloseWindow());
        InitTab(TabState.Chat, "Chat");
        InitTab(TabState.Member, "Member");
        TeamData = TeamManager.Instance.MyTeamInfo;
        Chat = transform.Find("Root/Chat").gameObject.AddComponent<ChatGroup>();
        Chat.Init(this);
        Member = transform.Find("Root/Member").gameObject.AddComponent<MemberGroup>();
        Member.Init(TeamData);

        NameText = transform.Find("Root/BGGroup/TitleText").GetComponent<Text>();
        if (NameText)
            NameText.text = TeamData.Name;
        transform.Find("Root/BGGroup/NumText").GetComponent<LocalizeTextMeshProUGUI>().SetText(TeamData.PlayerList.Count+"/"+TeamData.MemberMaxCount);
        TeamIcon = TeamIconNode.BuildTeamIconNode(transform.Find("Root/GuildIcon") as RectTransform,TeamData.GetViewState());
        transform.Find("Root/GuildIcon").GetComponent<Button>().onClick.AddListener(() =>
        {
            UIPopupGuildSetController.Open(TeamData);
        });
        EventDispatcher.Register<EventOnTeamInfoChange>(OnTeamInfoChange);

        // XUtility.WaitSeconds(0.3f, CheckCardPackageGuide);
    }

    public void CheckLifeGuide(int life)
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TeamLifeDesc))
            return;

        var heartGroup = Chat.transform.Find("Heart");
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(heartGroup);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TeamLifeDesc, heartGroup as RectTransform,
            topLayer: topLayer);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TeamLifeDesc, null))
        {
            TeamManager.Instance.AddLife(life,"BuyCardGuide");
        }
    }
    public void CheckCardPackageGuide()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TeamCardPackageClick))
            return;
        var messageList = Chat.ScrollRecycleController.DataList;
        ChatGroup.MessageItemCard cardMessageItem = null;
        RecvContent cardMessage = null;
        int index = 0;
        foreach (var messageObject in messageList)
        {
            var messageItem = messageObject as RecvContent;
            if (messageItem.ChatType == ChatType.TeamPassGift && messageItem.UserId != TeamManager.Instance.MyPlayerId)
            {
                cardMessage = messageItem;
                break;
            }
            index++;
        }

        if (cardMessage != null)
        {
            Chat.ScrollRecycleController.RebuildWithFocusIndex(index,1);
            foreach (var visibleItem in Chat.MessageItemVisiblePool)
            {
                var cardItem = visibleItem as ChatGroup.MessageItemCard;
                if (cardItem != null && cardItem.MessageContent.ChatId == cardMessage.ChatId)
                {
                    cardMessageItem = cardItem;
                    break;
                }
            }
        }
        
        if (cardMessageItem == null)
            return;
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(cardMessageItem.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TeamCardPackageClick, cardMessageItem.transform as RectTransform,
            topLayer: topLayer);
        cardMessageItem.GetComponent<Button>().onClick.AddListener(() =>
        {
            // Chat.scrollRectTrigger.scrollRect.enabled = true;
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TeamCardPackageClick);
        });
        
        
        if (GuideSubSystem.Instance.IsShowingGuide())
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TeamCardPackageClick);
            XUtility.CleanGuideList(new List<int>() { 4585 });
        }

        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TeamCardPackageClick, null))
        {
            
        }
    }

    public void OnTeamInfoChange(EventOnTeamInfoChange evt)
    {
        UpdateTeamView();
    }

    private void OnDestroy()
    {
        EventDispatcher.UnRegister<EventOnTeamInfoChange>(OnTeamInfoChange);
    }

    public void UpdateTeamView()
    {
        TeamData = TeamManager.Instance.MyTeamInfo;
        if (TeamData == null)
        {
            AnimCloseWindow();
            return;
        }
        TeamIcon.SetTeamIconViewState(TeamData.GetViewState());
        if (NameText)
            NameText.text = TeamData.Name;
        transform.Find("Root/BGGroup/NumText").GetComponent<LocalizeTextMeshProUGUI>().SetText(TeamData.PlayerList.Count+"/"+TeamData.MemberMaxCount);
        Member.Init(TeamData);
    }
    
    public void InitTab(TabState state,string name)
    {
        var viewState = new ViewGroup();
        viewState.State = state;
        viewState.Group = transform.Find("Root/"+name);
        viewState.LabelNormal = transform.Find("Root/Lable/"+name+"/Normal");
        viewState.LabelSelect = transform.Find("Root/Lable/"+name+"/Selected");
        GroupDic.Add(state,viewState);
            
        transform.Find("Root/Lable/"+name).GetComponent<Button>().onClick.AddListener(() =>
        {
            if (State != state)
            {
                GroupDic[State].Group.gameObject.SetActive(false);
                GroupDic[State].LabelSelect.gameObject.SetActive(false);
                GroupDic[State].LabelNormal .gameObject.SetActive(true);
                State = state;
                GroupDic[State].Group.gameObject.SetActive(true);
                GroupDic[State].LabelSelect.gameObject.SetActive(true);
                GroupDic[State].LabelNormal .gameObject.SetActive(false);
            }
        });
        if (State == viewState.State)
        {
            viewState.Group.gameObject.SetActive(true);
            viewState.LabelSelect.gameObject.SetActive(true);
            viewState.LabelNormal.gameObject.SetActive(false);
        }
        else
        {
            viewState.Group.gameObject.SetActive(false);
            viewState.LabelSelect.gameObject.SetActive(false);
            viewState.LabelNormal.gameObject.SetActive(true);
        }
    }
}