using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DG.Tweening;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class ExperienceProgress : MonoBehaviour
{
    Slider sliderImage;
    LocalizeTextMeshProUGUI countText;
    bool isInit = false;
    public Button expButton;
    private Image expBackGround;

    public Image ExpBackGround
    {
        get { return expBackGround; }
    }

    private Animator _animator;

    
    // private Image _headImage;
    // private Image _headFrameImage;
    private RectTransform _headIconRoot;
    private HeadIconNode HeadIcon;
    
    private void Awake()
    {
        sliderImage = transform.GetComponentDefault<Slider>("StarNode/Slider");
        countText = this.transform.GetComponentDefault<LocalizeTextMeshProUGUI>("StarNode/Slider/Star/Text");
        expButton = transform.GetComponentDefault<Button>("StarNode/Button");
        expBackGround = transform.GetComponentDefault<Image>("StarNode/Slider/StarIcon");

        _animator = transform.Find("StarNode").GetComponent<Animator>();
        
        expButton.onClick.AddListener(OnExpBtnClick);
        
        transform.Find("StarNode/HeadGroup/BG").gameObject.SetActive(false);
        _headIconRoot = transform.Find("StarNode/HeadGroup/Head") as RectTransform;
        
        EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_HEAD, UpdateHeadIcon);
    }

    private void Start()
    {
        DestroyImmediate(ExperenceModel.Instance.gameObject);
        EventDispatcher.Instance.AddEventListener(MergeEvent.DO_REFRESH_EXPERENCE, OnRefreshExp);
        sliderImage.value = ExperenceModel.Instance.GetPercentExp();
        countText.SetText(ExperenceModel.Instance.GetLevel().ToString());
        CheckLevelUp(null);
        UpdateHeadIcon(null);
    }

    private void OnEnable()
    {
        CheckLevelUp(null);
    }

    private void OnExpBtnClick()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupSet1);
        // if (ExperenceModel.Instance.IsCanLevelUp())
        // {
        //     UIManager.Instance.OpenUI(UINameConst.UIPopupMergeLevelTipsShow);
        // }
        // else
        // {
        //     UIManager.Instance.OpenUI(UINameConst.UIPopupMergeLevelTips);
        // }
    }

    private void OnDisable()
    {
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.DO_REFRESH_EXPERENCE, OnRefreshExp);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD, UpdateHeadIcon);
    }

    private void UpdateHeadIcon(BaseEvent e)
    {
        if (_headIconRoot)
        {
            if (HeadIcon)
            {
                HeadIcon.SetAvatarViewState(HeadIconUtils.GetMyViewState());
            }
            else
            {
                HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot,HeadIconUtils.GetMyViewState());
            }
        }
    }  
    
    private void OnRefreshExp(BaseEvent e)
    {
        if (e == null || e.datas == null)
            return;

        bool isLevelUp = (bool) e.datas[0];
        if (isLevelUp)
        {
            float time1 = (ExperenceModel.Instance.GetPercentExp() - sliderImage. value) / 0.1f;
            sliderImage.DOValue(1, ReMapTime(1)).OnComplete(() =>
            {
                _animator?.Play("appear", -1, 0);
                sliderImage.value = 0;
                sliderImage.DOValue(ExperenceModel.Instance.GetPercentExp(),
                    ReMapTime(ExperenceModel.Instance.GetPercentExp()));
                countText.SetText(ExperenceModel.Instance.GetLevel().ToString());
            });
        }
        else
        {
            sliderImage.DOValue(ExperenceModel.Instance.GetPercentExp(), ReMapTime(ExperenceModel.Instance.GetPercentExp()));
        }
    }

    public void CheckLevelUp(Action callFunc)
    {
        StopAllCoroutines();
        StartCoroutine(WaitLevelUp(callFunc));
    }

    private IEnumerator WaitLevelUp(Action callFunc)
    {
        bool isCanLevelUp = ExperenceModel.Instance.IsCanLevelUp();
        if (!isCanLevelUp)
        {
            callFunc?.Invoke();
            yield break;
        }

        var enableEventSystem = UIRoot.Instance.EnableEventSystem;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            UIRoot.Instance.EnableEventSystem = true;
            UIManager.Instance.OpenUI(UINameConst.UIPopupMergeLevelTipsShow);
            while (true)
            {
                var dlg = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeLevelTipsShow);
                if (dlg == null)
                    break;

                yield return new WaitForEndOfFrame();
            }

            if (!ExperenceModel.Instance.IsCanLevelUp())
                break;
            
            yield return new WaitForEndOfFrame();
        }
        UIRoot.Instance.EnableEventSystem = enableEventSystem;
        yield return new WaitForSeconds(1f);
        callFunc?.Invoke();
    }

    private float ReMapTime(float value)
    {
        float offset = (value - sliderImage.value) / 0.1f;
        float time = offset > 1 ? 1 : offset;
        return time;
    }

    public void UpdateText(int subNum, float time, System.Action callBack = null)
    {
        sliderImage.DOValue(ExperenceModel.Instance.GetPercentExp(), ReMapTime(ExperenceModel.Instance.GetPercentExp()))
            .OnComplete(() =>
            {
                TriggerGuide();
                callBack?.Invoke();
            });
    }

    public void TriggerGuide()
    {
        int decNum = 0;
        int rvNum = 0;
        int toalDecNum = 0;
        // if (RoomManager.Instance.GetRoomDecorationRate(RoomManager.Instance.CurRoomId, ref decNum, ref rvNum) *
        //     100 < 100)
        // {
        //     if (ExperenceModel.Instance.IsCanLevelUp() &&
        //         !GuideSubSystem.Instance.isFinished(GuideTrigger.CheckLevelUp))
        //     {
        //         if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        //         {
        //             GuideSubSystem.Instance.Trigger(GuideTrigger.LevelUp, null);
        //         }
        //         else
        //         {
        //             GuideSubSystem.Instance.Trigger(GuideTrigger.CheckLevelUp, null);
        //         }
        //     }
        // }
    }
}