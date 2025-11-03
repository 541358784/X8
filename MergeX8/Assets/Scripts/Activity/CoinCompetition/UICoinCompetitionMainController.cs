using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UICoinCompetitionMainController : UIWindowController
{
    private Button _closeBtn;
    
    // private GameObject _newTaskGroup;
    // private Image _newTaskImage;
    // private LocalizeTextMeshProUGUI _newTaskText;

    private List<UICoinCompetitionRewardItemExtra> _extraRewardItems;
    private RectTransform _rewardGroupContent;
    private List<UICoinCompetitionRewardItem> _rewardItems;

    private LocalizeTextMeshProUGUI _timeText;
    
    private LocalizeTextMeshProUGUI _buttonText;
    private Button _button;
    
    private Slider _slider;
    private LocalizeTextMeshProUGUI _sliderText;
    private List<Image> _sliderIcons;
    // private Animator _sliderAnimator;
    
    // private Slider _newSlider;
    // private LocalizeTextMeshProUGUI _newSliderText;
    // private List<Image> __newSliderIcons;

    private GameObject _finishGroup;
    private GameObject _failGroup;
    private SkeletonGraphic _skeletonGraphic;
    private CoinCompetitionOpenType curOpenType;
    private LocalizeTextMeshProUGUI _descText;
    public enum CoinCompetitionOpenType
    {
        Normal,
        ExtendAppear,
        Extend,        
        Finish,
        Fail
    }
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnClose);
        // _newTaskGroup = GetItem("Root/Slider/NewTask");
        // _newTaskImage = GetItem<Image>("Root/Slider/NewTask/Star");
        // _newTaskText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/NewTask/Text");
        _descText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        _rewardGroupContent = GetItem("Root/RewardGroup/Content").transform as RectTransform;
        _rewardItems = new List<UICoinCompetitionRewardItem>();
        for (int i = 1; i <= 10; i++)
        {
            var item=GetItem("Root/RewardGroup/Content/" + i).AddComponent<UICoinCompetitionRewardItem>();
            _rewardItems.Add(item);
        }
        _extraRewardItems = new List<UICoinCompetitionRewardItemExtra>();
        for (int i = 1; i <= 1; i++)
        {
            var item=GetItem("Root/RewardGroup/Content/Extra").AddComponent<UICoinCompetitionRewardItemExtra>();
            _extraRewardItems.Add(item);
        }
        
        _skeletonGraphic = transform.Find("Root/BG/Mask/SkeletonGraphic (heroine_UI)").GetComponent<SkeletonGraphic>();

        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _buttonText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text");
        _button = GetItem<Button>("Root/Button");
        _button.onClick.AddListener(OnBtnGo);
        _slider = GetItem<Slider>("Root/Slider");
        // _sliderAnimator = _slider.GetComponent<Animator>();
        _sliderIcons = new List<Image>();
        for (int i = 1; i < 4; i++)
        {
            _sliderIcons.Add( GetItem<Image>("Root/Slider/RewardGroup/Icon"+i));
        }
        _sliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        
        // _newSlider = GetItem<Slider>("Root/NewSlider");
        // __newSliderIcons = new List<Image>();
        // for (int i = 1; i < 4; i++)
        // {
        //     __newSliderIcons.Add( GetItem<Image>("Root/NewSlider/RewardGroup/Icon"+i));
        // }
        // _newSliderText = GetItem<LocalizeTextMeshProUGUI>("Root/NewSlider/Text");

        _finishGroup = GetItem("Root/Finish");
        _failGroup = GetItem("Root/Fail");
   
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CoinCompetitionPreview,transform as RectTransform);
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add( _button.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CoinCompetitionPlay, _button.transform as RectTransform, topLayer: topLayer);

    }

    private bool IsOpen = false;
    protected override void OnOpenWindow(params object[] objs)
    {
        if (IsOpen)
        {
            curOpenType = CoinCompetitionOpenType.Normal;
            UpdateUI();
            if (objs!=null && objs.Length>0)
            {
                if ((bool) objs[0] == true)
                {
                    SetFail();
                }
            }
            return;
        }
        IsOpen = true;
        InvokeRepeating("UpdateTimeText",1,1);
        UpdateTimeText();
        var rewards= CoinCompetitionModel.Instance.GetCoinCompetitionRewards();
        var rewardCount = 0;
        for (int i = 0; i < _rewardItems.Count; i++)
        {
            _rewardItems[i].Init(rewards[i]);
            rewardCount++;
        }
        for (int i = 0; i < _extraRewardItems.Count; i++)
        {
            var rewardIndex = i + rewardCount;
            _extraRewardItems[i].Init(rewardIndex < rewards.Count?rewards[rewardIndex]:null);
        }

        curOpenType = CoinCompetitionOpenType.Normal;
        UpdateUI();
        if (objs!=null && objs.Length>0)
        {
            if ((bool) objs[0] == true)
            {
                SetFail();
            }
        }
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CoinCompetitionPreview, null);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CoinCompetitionPlay, null);

    }

    private void SetFail()
    {
        _button.gameObject.SetActive(false);
        _failGroup.gameObject.SetActive(false);
        _descText.gameObject.SetActive(true);
        _descText.SetTerm("ui_goldmatch_desc6");
        _timeText.transform.parent.gameObject.SetActive(false);
        // PlaySkeletonAnimation("fightingwill");
        CoinCompetitionModel.Instance.StorageCompetition.IsShowEndView=true;

    }

    private void SetCurType()
    {
        curOpenType = CoinCompetitionOpenType.Normal;
        var currentReward = CoinCompetitionModel.Instance.GetCurrentReward();
        if (CoinCompetitionModel.Instance.IsExchangeAll())
        {
            curOpenType = CoinCompetitionOpenType.Finish;
        }
        else
        {
            if (currentReward.Id == 11)
            {
                if(CoinCompetitionModel.Instance.StorageCompetition.IsPlayEntendAppear)
                    curOpenType = CoinCompetitionOpenType.Extend;
                else
                {
                    curOpenType = CoinCompetitionOpenType.ExtendAppear;
                }
            }
        }

    }
    
    private void UpdateUI()
    {
        SetCurType();
        var currentReward = CoinCompetitionModel.Instance.GetCurrentReward();
        switch (curOpenType)
        {
            case CoinCompetitionOpenType.Normal:
                _descText.SetTerm("ui_goldmatch_desc3");
                // PlaySkeletonAnimation("fightingwill");
                _finishGroup.SetActive(false);
                _failGroup.SetActive(false);
                // _newSlider.gameObject.SetActive(false);
                _extraRewardItems[0].PlayLockAnimation();
                UpdateSlider(currentReward);
                UpdateBtnStatus();
                break;
            case CoinCompetitionOpenType.Extend:
                _descText.SetTerm("ui_goldmatch_desc4");
                // _sliderAnimator.Play("normal1", 0);
                // PlaySkeletonAnimation("angry");
                _finishGroup.SetActive(false);
                _failGroup.SetActive(false);
                // for (int i = 0; i < _rewardItems.Count; i++)
                // {
                //     _rewardItems[i].HideReward();;
                // }
                // _newSlider.gameObject.SetActive(true);
                // UpdateNewSlider();
                _extraRewardItems[0].PlayUnLockAnimation();
                UpdateSlider(currentReward);
                break;     
            case CoinCompetitionOpenType.ExtendAppear:
                _descText.SetTerm("ui_goldmatch_desc4");
                // PlaySkeletonAnimation("angry");            
           
                _finishGroup.SetActive(false);
                _failGroup.SetActive(false);
                // for (int i = 0; i < _rewardItems.Count; i++)
                // {
                //     _rewardItems[i].HideReward();;
                // } 
                // _newSlider.gameObject.SetActive(false);
                var rewardConfigByLevel = CoinCompetitionModel.Instance.GetRewardConfigByLevel(10);
                UpdateSlider(rewardConfigByLevel);
                // UpdateNewSlider();
                CoinCompetitionModel.Instance.StorageCompetition.IsPlayEntendAppear = true;
                _extraRewardItems[0].PlayLockAnimation();
                CommonUtils.DelayedCall(1.5f, () =>
                {
                    _extraRewardItems[0].PlayAppearAnimation(() =>
                    {
                        UpdateSlider(currentReward);
                    });
                    // StartCoroutine(    CommonUtils.PlayAnimation(_sliderAnimator, "appear", "", () =>
                    // {
                    //     _newTaskGroup.SetActive(true);
                    //     _newSlider.gameObject.SetActive(true);
                    //
                    // }));
                });
                break;
            case CoinCompetitionOpenType.Finish:
                _descText.SetTerm("ui_goldmatch_desc5");
                // _sliderAnimator.Play("normal1", 0);
                // PlaySkeletonAnimation("thinking");
                _finishGroup.SetActive(false);
                _failGroup.SetActive(false);
                // for (int i = 0; i < _rewardItems.Count; i++)
                // {
                //     _rewardItems[i].HideReward();;
                // } 
                // _newTaskGroup.SetActive(false);
                // _newSlider.gameObject.SetActive(false);
                _extraRewardItems[0].PlayUnLockAnimation();
                _slider.gameObject.SetActive(false);
                _button.gameObject.SetActive(false);
                CoinCompetitionModel.Instance.StorageCompetition.IsShowEndView=true;
                break;
       
        }
        var rewards= CoinCompetitionModel.Instance.GetCoinCompetitionRewards();
        var rewardCount = 0;
        for (int i = 0; i < _rewardItems.Count; i++)
        {
            _rewardItems[i].UpdateStatus(rewards[i]);
            rewardCount++;
        }
        for (int i = 0; i < _extraRewardItems.Count; i++)
        {
            var rewardIndex = i + rewardCount;
            _extraRewardItems[i].UpdateStatus(rewardIndex < rewards.Count?rewards[rewardIndex]:null);
        }
        UpdateBtnStatus();
        var rewardIndx = 0;
        for (var i = 0; i < rewards.Count; i++)
        {
            if (currentReward == rewards[i])
            {
                rewardIndx = i;
                break;
            }
        }
        SetContentPosition(rewardIndx);
    }
    public void UpdateSlider(CoinCompetitionReward cur)
    {
        for (int i = _sliderIcons.Count-1; i >=0; i--)
        {   
            _sliderIcons[i].gameObject.SetActive(false);
        }

        int need = CoinCompetitionModel.Instance.GetLevelNeedStore(cur.Id);
        _slider.value = (float)(CoinCompetitionModel.Instance.StorageCompetition.TotalScore - cur.Score+need) / (need);;
        _sliderText.SetText((CoinCompetitionModel.Instance.StorageCompetition.TotalScore - cur.Score+need) +"/"+ need);
        for (int i = 0; i < cur.RewardId.Count; i++)
        {
            _sliderIcons[i].gameObject.SetActive(true);
            if (UserData.Instance.IsResource(cur.RewardId[i]))
            {
                _sliderIcons[i].sprite = UserData.GetResourceIcon(cur.RewardId[i]);
            }
            else
            {
                var itemConfig= GameConfigManager.Instance.GetItemConfig(cur.RewardId[i]);
                _sliderIcons[i].sprite=  MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);   
            }
        }
    }
    // public void UpdateNewSlider()
    // {
    //     for (int i = __newSliderIcons.Count-1; i >=0; i--)
    //     {   
    //         __newSliderIcons[i].gameObject.SetActive(false);
    //     }
    //     var cur = CoinCompetitionModel.Instance.GetCurrentReward();
    //     _newSlider.value = CoinCompetitionModel.Instance.GetCurrentProgress();
    //     _newSliderText.SetText(CoinCompetitionModel.Instance.GetCurrentProgressStr());
    //     for (int i = 0; i < cur.RewardId.Count; i++)
    //     {
    //         __newSliderIcons[i].gameObject.SetActive(true);
    //         if (UserData.Instance.IsResource(cur.RewardId[i]))
    //         {
    //             __newSliderIcons[i].sprite = UserData.GetResourceIcon(cur.RewardId[i]);
    //         }
    //         else
    //         {
    //             var itemConfig= GameConfigManager.Instance.GetItemConfig(cur.RewardId[i]);
    //             __newSliderIcons[i].sprite=  MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
    //         }
    //     }
    // }
    private void UpdateBtnStatus()
    {
        if(CoinCompetitionModel.Instance.IsCanClaim())
            _buttonText.SetTerm("UI_button_claim");
        else
        {
            _buttonText.SetTerm("ui_goldmatch_button1");
        }
    }

    private void OnBtnGo()
    {
   
        if(CoinCompetitionModel.Instance.IsCanClaim())
            CoinCompetitionModel.Instance.Claim(() =>
            {
                UpdateUI();
            },true);
        else
        {
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CoinCompetitionPlay, null);
                }
            });
           
        }
    }
    private void UpdateTimeText()
    {
        _timeText.SetText(CoinCompetitionModel.Instance.GetActivityLeftTimeString());
        if (CoinCompetitionModel.Instance.GetActivityPreheatLeftTime() <= 0)
            SetFail();
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
    
    private float PlaySkeletonAnimation(string animName)
    {
        if (_skeletonGraphic == null)
            return 0;

        TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
        if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
            return trackEntry.AnimationEnd;

        _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
        _skeletonGraphic.Update(0);

        return trackEntry.AnimationEnd;
    }

    public void SetContentPosition(int rewardIndex)
    {
        var contentX = rewardIndex * -140;
        contentX = Math.Max(contentX, -958);
        _rewardGroupContent.anchoredPosition = new Vector2(contentX, 0);
    }
}