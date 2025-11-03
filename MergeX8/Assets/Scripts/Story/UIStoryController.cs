using System;
using System.Collections;
using DG.Tweening;
using DragonU3DSDK;
using Gameplay;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class UIStoryController : UIWindow
{
    private Transform _dialogTransform;
    private Transform _movieTransform;
    private TableStory _currentStoryConfig;
    public static bool AreaFinished = false;

    private TableStory _firstStepConfig;//组内第一个
    private Button _continueButton;
    private ScrollRect _scrollRect;
    public static void StartStory(TableStory config)
    {
        var dlg = (UIStoryController)UIManager.Instance.OpenUI(UINameConst.UIStory);
        dlg._firstStepConfig = config;
        dlg?.startStory(config);
        StoryFtueBiManager.Instance.SendFtueBi(StoryFtueBiManager.SendType.Start,config.id);
    }

    public static void SwitchToGuideRootParent()
    {
        var dlg = (UIStoryController)UIManager.Instance.GetOpenedUIByPath<UIStoryController>(UINameConst.UIStory);
        dlg.transform.ChangeParent(UIRoot.Instance.transform.Find("GuideRoot"));
    }

    public static void ExitStory(Action onExit)
    {
        var dlg = (UIStoryController)UIManager.Instance.GetOpenedUIByPath<UIStoryController>(UINameConst.UIStory);
        dlg?.checkGuide();
        dlg?.exitStory(onExit);
    }

    private void checkGuide()
    {
    }

    ////////////////////////////////////////////////////////////

    public override void PrivateAwake()
    {
        isPlayDefaultAudio = false;
        _dialogTransform = transform.Find("Dialog");
        _movieTransform = transform.Find("Movie");
        _continueButton = transform.Find("Dialog/BottomGroup/ContinueButton").GetComponent<Button>();
        _continueButton.gameObject.SetActive(false);
        _continueButton.onClick.AddListener(OnClickContinue);
        _scrollRect = transform.Find("Dialog/MiddleGroup/Scroll View").GetComponent<ScrollRect>();
        _scrollRect.vertical = false;
        privateAwakeDialog();
        StartCoroutine(CommonUtils.DelayWork(2f, () =>
        {
            UIRoot.Instance.RemoveTouchBlock();
        }));

    }


    private void OnDestroy()
    {
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        CommonUtils.SetShieldButTime(gameObject, 0.1f);
        resetDialog();
    }

    private void startStory(TableStory config)
    {
        if (config == null)
            return;
     
        if (!m_skip.gameObject.activeSelf)
        {
            m_skip.gameObject.SetActive(true);
        }
        _dialogTransform.gameObject.SetActive(true);
        _movieTransform.gameObject.SetActive(false);
        startDialog(config);

    }

    private void tryNext()
    {
        DragonPlus.AudioManager.Instance.PlaySound("sxf_ui_common_dialogue");

        if (_currentStoryConfig != null && CommonUtils.IsIdValid(_currentStoryConfig.next_id))
        {
            var nextConfig = GlobalConfigManager.Instance.GetTableStory(_currentStoryConfig.next_id);
            if (nextConfig != null || _currentStoryConfig.next_id <= 0)
            {
                startStory(nextConfig);
            }
            else
            {
                ShowContinue();
            }
        }
        else
        {
            ShowContinue();
        }
    }
    
    private void skip()
    {
        StorySubSystem.Instance.ExitStory();
        DragonPlus.AudioManager.Instance.PlaySound("sxf_ui_common_dialogue");
        if (_firstStepConfig != null)
        {
            StoryFtueBiManager.Instance.SendFtueBi(StoryFtueBiManager.SendType.Skip,_firstStepConfig.id);

        }

    }

    private void ShowContinue()
    {
        _scrollRect.vertical = true;
        _continueButton.gameObject.SetActive(true);
    }
    private void OnClickContinue()
    {
        StorySubSystem.Instance.ExitStory();
        if (_firstStepConfig != null)
        {
            StoryFtueBiManager.Instance.SendFtueBi(StoryFtueBiManager.SendType.TipContinue,_firstStepConfig.id);
        }
        CommonUtils.DicSafeSet(StorageManager.Instance.GetStorage<StorageHome>().DialogData.SkipInfoDic, _firstStepConfig.id, 0);
    }

    private void sendSkipBI()
    {
        try
        {
            var step = _currentStoryConfig.id % 10;

            CommonUtils.DicSafeSet(StorageManager.Instance.GetStorage<StorageHome>().DialogData.SkipInfoDic, _firstStepConfig.id, step);
        }
        catch (System.Exception e)
        {
            DebugUtil.LogError("Guide BI Exception:" + e.ToString());
        }
    }

    private void exitStory(Action onExit)
    {
        _currentStoryConfig = null;
        StopAllCoroutines();
        stopAllDialogTween();

        StartCoroutine(lateHideStory(onExit));
    }

    private IEnumerator lateHideStory(Action onExit)
    {
        for (int i = 0; i < _dialog.childCount; i++)
        {
            Animator animator = _dialog.GetChild(i).GetComponent<Animator>();
            if(animator == null)
                continue;
                    
            animator?.Play("disappear");
        }
        yield return new WaitForSeconds(0.33f);
        this.gameObject.GetComponent<Animator>().Play("UIStory_disappear");
        yield return new WaitForSeconds(0.2f);
        CloseWindowWithinUIMgr(true);
        UIStoryController.AreaFinished = false;
        onExit?.Invoke();
    }
}