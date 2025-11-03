using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFlowerFieldMainController:UIWindowController
{
    public static UIFlowerFieldMainController Instance;

    public static UIFlowerFieldMainController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIFlowerFieldMain) as UIFlowerFieldMainController;
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
            UIFlowerFieldHelpController.Open();
        });
        MergeBtn = transform.Find("Root/Button").GetComponent<Button>();
        MergeBtn.gameObject.SetActive(true);
        MergeBtn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FlowerFieldPlay);
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

    public StorageFlowerField Storage => FlowerFieldModel.Instance.Storage;
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
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
        InvokeRepeating("UpdateTime",0,1);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.FlowerFieldDes, null))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(MergeBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.FlowerFieldPlay, MergeBtn.transform as RectTransform,
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
    
    
    public List<Tuple<FlowerFieldLevelState, FlowerFieldLevelState>> PerformList =
        new List<Tuple<FlowerFieldLevelState, FlowerFieldLevelState>>();
    public bool InPerform = false;
    public async Task PerformJump(FlowerFieldLevelState oldState,FlowerFieldLevelState newState)
    {
        PerformList.Add(new Tuple<FlowerFieldLevelState, FlowerFieldLevelState>(oldState,newState));
        if (!InPerform)
        {
            InPerform = true;
            while (PerformList.Count > 0)
            {
                var performState = PerformList[0];
                PerformList.RemoveAt(0);
                await XUtility.WaitFrames(15);
                FocusOn(performState.Item1.GroupInnerIndex-1);
                SetState(performState.Item1);
                SetTopGroupState(performState.Item1);
                await XUtility.WaitSeconds(1f);
                if (!this)
                    return;
                TopGroupPerformToMax(performState.Item1,performState.Item2);
                await PerformFly();

                {
                    var rewards = oldState.Rewards;
                    var popRewardTask = new TaskCompletionSource<bool>();
                    CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                        false, animEndCall:
                        () =>
                        {
                            popRewardTask.SetResult(true);
                        });   
                }
                
                if (!this)
                    return;
                SetState(performState.Item2,true);
                SetTopGroupState(performState.Item2);
            }
            InPerform = false;
        }
    }
    private const float JumpTime = 2f;
}