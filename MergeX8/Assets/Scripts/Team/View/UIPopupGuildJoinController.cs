using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGuildJoinController:UIWindowController
{
    public static UIPopupGuildJoinController Instance;
    public static UIPopupGuildJoinController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        if (TeamManager.Instance.MyTeamInfo != null)
        {
            Debug.LogError("有公会了，无法创建或加入公会");
            return null;   
        }
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildJoin) as UIPopupGuildJoinController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        
    }

    public enum TabState
    {
        Join,
        Create,
    }

    public class ViewGroup
    {
        public TabState State;
        public Transform Group;
        public Transform LabelNormal;
        public Transform LabelSelect;
    }
    private TabState State = TabState.Join;
    private Dictionary<TabState, ViewGroup> GroupDic = new Dictionary<TabState, ViewGroup>();
    private JoinGroup Join;
    private CreateGroup Create;
    
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(()=>AnimCloseWindow());
        InitTab(TabState.Join, "Join");
        InitTab(TabState.Create, "Create");
        
        Join = transform.Find("Root/Join").gameObject.AddComponent<JoinGroup>();
        Join.Init();
        Create = transform.Find("Root/Create").gameObject.AddComponent<CreateGroup>();
        Create.Init();

        CheckGuide();
    }

    public void CheckGuide()//公会介绍
    {
        if (!GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TeamDesc))
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TeamDesc, null);
        }
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
        
        viewState.Group.gameObject.SetActive(state == State);
        viewState.LabelSelect.gameObject.SetActive(state == State);
        viewState.LabelNormal .gameObject.SetActive(state != State);
    }
}