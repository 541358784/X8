
using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeCoinCompetition : MonoBehaviour
{
    private Image _slider;
    private LocalizeTextMeshProUGUI _sliderText;
    private Image _sliderIcon;
    private List<GameObject> _sliderIcons;

    private LocalizeTextMeshProUGUI _timeText;
    
    private Button _button;
    private SkeletonGraphic _skeletonGraphic;
    private int _currentLevel;
    private int _currentMaxValue;
    private int _currentValue;
    private void Awake()
    {
        _slider = transform.Find("Slider").GetComponent<Image>();
        _sliderText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _sliderIcon = transform.Find("Slider/RewardGroup/Item1").GetComponent<Image>();
        _sliderIcon.gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(OnBtnCoinCompetition);
        _button = transform.Find("Button").GetComponent<Button>();
        _skeletonGraphic = transform.Find("Person/PortraitSpine/PortraitSpine1/PortraitSpine22 (3)").GetComponent<SkeletonGraphic>();
        _button.onClick.AddListener(OnBtnClick);
        InvokeRepeating("UpdateTimeText",0,1);
        UpdateTimeText();
        EventDispatcher.Instance.AddEventListener(EventEnum.ADD_COIN, UpdateUI);
        EventDispatcher.Instance.AddEventListener(EventEnum.MERGE_COINCOMPETITION_REFRESH, OnRefresh);
        _sliderIcons = new List<GameObject>();
 
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ADD_COIN, UpdateUI);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.MERGE_COINCOMPETITION_REFRESH, OnRefresh);
    }

    private void OnRefresh(BaseEvent obj)
    {
        UpdateUIOnLevelUp();
    }

    private void OnBtnCoinCompetition()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CoinCompetitionTask);
        UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionMain);
    }

    public void UpdateUI(BaseEvent obj)
    {
        int store=(int) obj.datas[0];
        PerformAddValue(store, 1.5f);
    }

    private void OnEnable()
    {
        if(CoinCompetitionModel.Instance.GetIsInit())
            InitUI();
        StopAllCoroutines();
    }
    private void OnBtnClick()
    {
        CoinCompetitionModel.Instance.Claim(null);
        InitUI();
    }

    private void UpdateTimeText()
    {
        gameObject.SetActive(CoinCompetitionModel.Instance.ShowEntrance());
        if(!gameObject.activeSelf)
            return;
        
        _timeText.SetText(CoinCompetitionModel.Instance.GetActivityLeftTimeString());
        if (CoinCompetitionModel.Instance.GetIsInit() && CoinCompetitionModel.Instance.GetActivityLeftTime() <= 0)
        {
            if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UICoinCompetitionMain) == null && !CoinCompetitionModel.Instance.StorageCompetition.IsShowEndView )
            {
                UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionMain,true);
            }
        }
    }

    private void InitUI()
    {
        transform.DOKill(true);
        var currentReward = CoinCompetitionModel.Instance.GetCurrentReward();
        _currentLevel = currentReward.Id;
        _currentMaxValue = CoinCompetitionModel.Instance.GetLevelNeedStore(_currentLevel);
        _currentValue = CoinCompetitionModel.Instance.GetLevelHaveStore(_currentLevel);
        _button.gameObject.SetActive(CoinCompetitionModel.Instance.IsCanClaim());
        if (CoinCompetitionModel.Instance.IsCanClaim())
        {
            PlaySkeletonAnimation("happy");
        }
        else
        {
            PlaySkeletonAnimation("normal");
        }
        UpdateSlider();
    }

    public void UpdateUIOnLevelUp()
    {
        _currentValue -= _currentMaxValue;
        var currentReward = CoinCompetitionModel.Instance.GetCurrentReward();
        _currentLevel = currentReward.Id;
        _currentMaxValue = CoinCompetitionModel.Instance.GetLevelNeedStore(_currentLevel);
        _button.gameObject.SetActive(CoinCompetitionModel.Instance.IsCanClaim());
        maxState2 = _currentValue >= _currentMaxValue;
        if (CoinCompetitionModel.Instance.IsCanClaim())
        {
            PlaySkeletonAnimation("happy");
        }
        else
        {
            PlaySkeletonAnimation("normal");
        }
        UpdateSlider();
    }
    public void UpdateSlider()
    {
        for (int i = _sliderIcons.Count-1; i >=0; i--)
        {
            GameObject.Destroy(_sliderIcons[i]);
        }
        _sliderIcons.Clear();
        var cur = CoinCompetitionModel.Instance.GetCurrentReward();
        _slider.fillAmount = CoinCompetitionModel.Instance.GetCurrentProgress();
        _sliderText.SetText(CoinCompetitionModel.Instance.GetCurrentProgressStr());
        for (int i = 0; i < cur.RewardId.Count; i++)
        {
            if (cur.RewardShowIndex == cur.RewardId[i])
            {
                var item= Instantiate(_sliderIcon, _sliderIcon.transform.parent);
                 item.gameObject.SetActive(true);
                 _sliderIcons.Add(item.gameObject);
                 if (UserData.Instance.IsResource(cur.RewardId[i]))
                 {
                     item.sprite = UserData.GetResourceIcon(cur.RewardId[i]);
                 }
                 else
                 {
                     var itemConfig= GameConfigManager.Instance.GetItemConfig(cur.RewardId[i]);
                     item.sprite=  MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);   
                 }
            }
        }
    }
    public void PerformAddValue(int subNum, float time, System.Action callBack = null)
    {
        transform.DOKill(true);
        int lastUpdateSubNum = 0;
        DOTween.To(() => 0f, curSubNumF =>
        {
            var curSubNum = (int) Mathf.Floor(curSubNumF);
            var addValue = curSubNum - lastUpdateSubNum;
            lastUpdateSubNum = curSubNum;
            _currentValue += addValue;
            OnChangeValue();
        }, subNum, time).OnComplete(() =>
        {
            var addValue = subNum - lastUpdateSubNum;
            lastUpdateSubNum = addValue;
            _currentValue += addValue;
            OnChangeValue();
            callBack?.Invoke();
        }).SetTarget(transform);
    }
    private bool maxState = false;
    private bool maxState2 = false;
    public void OnChangeValue()
    {
        _sliderText.SetText(_currentValue + "/" + _currentMaxValue);
        _slider.fillAmount = Mathf.Min(_currentValue / (float) _currentMaxValue, 1f);
        var curMaxState = _currentValue >= _currentMaxValue;
        if (maxState != curMaxState)
        {
            maxState = false;
            _button.gameObject.SetActive(curMaxState);
            PlaySkeletonAnimation("happy");
            // _completeEffect.gameObject.SetActive(curMaxState);
        }

        if (maxState2 != curMaxState)
        {
            maxState2 = curMaxState;
            if (curMaxState)
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-transform.localPosition.x+220, 0);
                }           
            }
        }
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
        if (trackEntry == null)
            return 0;
        return trackEntry.AnimationEnd;
    }
}
