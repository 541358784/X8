using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.DogHope;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UIDogMainController:UIWindowController
{
    public enum DogMainShowType
    {
        Open,
        Failed,
    }
    private Button _buttonClose;
    private Button _buttonStart;
    private Slider _topSlider;
    private LocalizeTextMeshProUGUI _topSliderText;
    private Slider _rewardSlider;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _timeGroupText;
    private List<DogHopeRewardItem> _rewardItems=null;
    
    private Image _iconFrom;
    private Image _iconTo;
    private Image _iconTo2;

    private DogMainShowType _showType;
    private RectTransform _content;
    
    private SkeletonGraphic _skeletonGraphic;
    private LocalizeTextMeshProUGUI _text1;
    public override void PrivateAwake()
    {
        _rewardItems = new List<DogHopeRewardItem>();
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _buttonStart = GetItem<Button>("Root/Button");
        _buttonStart.onClick.AddListener(OnStartBtn);

        _topSlider = GetItem<Slider>("Root/Slider");
        _topSliderText= GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        _iconFrom = GetItem<Image>("Root/Slider/Image (1)");
        _iconTo = GetItem<Image>("Root/Slider/IconGroup/Icon");
        _iconTo2 = GetItem<Image>("Root/Slider/IconGroup/Icon2");
        _timeGroup = GetItem<Transform>("Root/TimeGroup");
        _timeGroupText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/Text");
        _rewardSlider = GetItem<Slider>("Root/ScrollView/Viewport/Content/RewardSlider");
        _text1 = GetItem<LocalizeTextMeshProUGUI>("Root/Text1");
        var startIndex = 1;
        while (true)
        {
            var rewardItemTrans =
                transform.Find("Root/ScrollView/Viewport/Content/RewardSlider/RewardGroup/" + startIndex);
            if (!rewardItemTrans)
                break;
            startIndex++;
            var rewardItem= rewardItemTrans.gameObject.AddComponent<DogHopeRewardItem>();
            _rewardItems.Add(rewardItem);
        }

        _skeletonGraphic = transform.Find("Root/BGGroup/PortraitSpine").GetComponent<SkeletonGraphic>();

        _content=transform.Find("Root/ScrollView/Viewport/Content") as RectTransform;
        
        InvokeRepeating("UpdateTime", 0, 1);
    }

    protected override void OnOpenWindow(params object[] objs)  
    {
        base.OnOpenWindow(objs);
        InitLeaderBoardEntrance();
        _showType = DogMainShowType.Open;
        if (objs != null && objs.Length > 0)
        {
            _showType=(DogMainShowType)objs[0] ;
        }
        RefreshUI();

        string animName = "normal";
        animName = _showType == DogMainShowType.Failed
            ? "sad"
            : (DogHopeModel.Instance.IsComplete() ? "happy" : animName);
        
        string textKey= _showType == DogMainShowType.Failed ? "story_105" :  "story_77";
        _text1.SetTerm(textKey);
        PlaySkeletonAnimation(animName);
        
        DogHopeModel.Instance.ShowStartView();
    }

    public void RefreshUI()
    {
        var rewards = DogHopeConfigManager.Instance.GetConfig<DogHopeReward>();
        for (int i = 0; i < rewards.Count; i++)
        {
            DogHopeRewardItem.RewardStatus status = DogHopeRewardItem.RewardStatus.None;
            if (DogHopeModel.Instance.GetScore() >= rewards[i].Score)
            {
                status=DogHopeRewardItem.RewardStatus.Finish;
            }
            else if(_showType== DogMainShowType.Failed)
            {
                status=DogHopeRewardItem.RewardStatus.Err;
            }
            _rewardItems[i].Init(rewards[i].RewardId,rewards[i].RewardNum,status);
        }

        for (var i = rewards.Count; i < _rewardItems.Count; i++)
        {
            _rewardItems[i].gameObject.SetActive(false);
        }
      
        
        var curData= DogHopeModel.Instance.GetCurIndexData();
        if (curData != null)
        {
            if (UserData.Instance.IsResource(curData.RewardId[0]))
            {
                _iconTo.sprite = UserData.GetResourceIcon(curData.RewardId[0], UserData.ResourceSubType.Reward);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(curData.RewardId[0]);
                _iconTo.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }   
            if (curData.RewardId.Count > 1)
            {
                _iconTo2.gameObject.SetActive(true);
                if (UserData.Instance.IsResource(curData.RewardId[1]))
                {
                    _iconTo2.sprite = UserData.GetResourceIcon(curData.RewardId[1], UserData.ResourceSubType.Reward);
                }
                else
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(curData.RewardId[1]);
                    _iconTo2.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                }
            }
        }
        else
        {
            _iconTo.gameObject.SetActive(false);
            _iconTo2.gameObject.SetActive(false);
        }
        
        int stateScore = DogHopeModel.Instance.GetIndexStageScore();
        int curScore = Math.Max(0, DogHopeModel.Instance.GetCurStageScore());
        if (curData!=null)
        {
            _topSliderText.SetText(curScore + "/" + stateScore);    
            _topSlider.value = (float) curScore / stateScore;
            _rewardSlider.value= curData.Id-1;
            if (curScore >= stateScore)
                _rewardSlider.value = curData.Id;
            _content.localPosition -=new Vector3( 150*(curData.Id-1),0,0);
        }
        else
        {
            _topSliderText.SetText(DogHopeModel.Instance.CurStorageDogHopeWeek.TotalScore.ToString());
            _topSlider.value = 1f;
            _rewardSlider.value = _rewardSlider.maxValue;
            _content.localPosition -=new Vector3( 150*(DogHopeConfigManager.Instance.GetConfig<DogHopeReward>().Count-1),0,0);
        }
        _buttonStart.gameObject.SetActive(!DogHopeModel.Instance.IsComplete());
        
        switch (_showType)
        {
            case DogMainShowType.Failed:
                _timeGroup.gameObject.SetActive(false);
                _buttonStart.gameObject.SetActive(false);
                DogHopeModel.Instance.EndActivity();
                break;
        }
    }
    private void OnStartBtn()
    {
        AnimCloseWindow(() =>
        {
            SceneFsm.mInstance.TransitionGame();
        });
    }

    private void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
        });
    }

    public void SetStartButtonStatus(bool isShow)
    {
        _buttonStart.gameObject.SetActive(isShow);
    }
    
    public  void UpdateTime()
    {
        if (DogHopeModel.Instance.GetActivityLeftTime()<=0)
        {
            _showType = DogMainShowType.Failed;
            RefreshUI();
            return;
        }
        _timeGroupText.SetText(DogHopeModel.Instance.GetActivityLeftTimeString());
    }


    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow(() =>
        {
           
        });
    }

    private static string coolTimeKey = "dogHope";
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DogHope))
            return false;

        if (!DogHopeModel.Instance.IsOpenActivity())
            return false;

        if (DogHopeModel.Instance.CanShowStartView())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenUI(UINameConst.UIDogStart);
            return true;
        }

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenUI(UINameConst.UIDogMain);
            return true;
        }
        return false;
    }
    
    private void PlaySkeletonAnimation(string animName)
    {
        if(_skeletonGraphic == null)
            return;

        TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
        if(trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
            return;
        
        _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
        _skeletonGraphic.Update(0);
    }
}
