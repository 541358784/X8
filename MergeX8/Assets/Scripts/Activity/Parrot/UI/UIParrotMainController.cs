using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIParrotMainController:UIWindowController
{
    public static UIParrotMainController Instance;

    public static UIParrotMainController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIParrotMain) as UIParrotMainController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        HelpBtn = transform.Find("Root/ButtonHelp").GetComponent<Button>();
        HelpBtn.onClick.AddListener(() =>
        {
            UIParrotHelpController.Open();
        });
        MergeBtn = transform.Find("Root/Button").GetComponent<Button>();
        MergeBtn.gameObject.SetActive(true);
        MergeBtn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ParrotPlay);
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/TimeGroup/TimeText");
    }

    public StorageParrot Storage => ParrotModel.Instance.Storage;
    public Button CloseBtn;
    public Button HelpBtn;
    public Button MergeBtn;
    public LocalizeTextMeshProUGUI TimeText;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        InitTopGroup();
        InitLevelGroup();
        AwakeRank();
        InvokeRepeating("UpdateTime",0,1);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ParrotDes, null))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(MergeBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ParrotPlay, MergeBtn.transform as RectTransform,
                topLayer: topLayer);
        }
        else
        {
            CheckLeaderBaordGuide();
        }
    }

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetLeftTimeText());
        if (Storage.IsTimeOut())
        {
            CancelInvoke("UpdateTime");
            AnimCloseWindow();
        }
    }
    
    
    public List<Tuple<ParrotLevelState, ParrotLevelState>> PerformList =
        new List<Tuple<ParrotLevelState, ParrotLevelState>>();
    public bool InPerform = false;
    public async Task PerformJump(ParrotLevelState oldState,ParrotLevelState newState)
    {
        PerformList.Add(new Tuple<ParrotLevelState, ParrotLevelState>(oldState,newState));
        if (!InPerform)
        {
            InPerform = true;
            while (PerformList.Count > 0)
            {
                var performState = PerformList[0];
                PerformList.RemoveAt(0);
                SetState(performState.Item1);
                SetTopGroupState(performState.Item1);
                await XUtility.WaitSeconds(1f);
                if (!this)
                    return;
                TopGroupPerformToMax(performState.Item1);
                await GroupDic[performState.Item1.Group].PerformJump(performState.Item1);
                if (!this)
                    return;
                SetState(performState.Item2);
                SetTopGroupState(performState.Item2);
            }
            InPerform = false;
        }
    }
    private const float JumpTime = 2f;
}