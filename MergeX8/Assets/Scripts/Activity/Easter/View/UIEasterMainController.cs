using System;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UIEasterMainController:UIWindowController
{

    private Button _buttonClose;
    private Button _buttonBuy;
    private Button _buttonHelp;
    private Button _buttonGame;
    private Slider _topSlider;
    private LocalizeTextMeshProUGUI _topSliderText;
    private LocalizeTextMeshProUGUI _lvText;
    private Slider _rewardSlider;
    private LocalizeTextMeshProUGUI _timeGroupText;
    private List<EasterMainCell> _rewardItems=null;
    private Transform _rewardItem;

    private RectTransform _content;
    private SkeletonGraphic _skeletonGraphic;
    public override void PrivateAwake()
    {
        _rewardItems = new List<EasterMainCell>();
        _buttonClose = GetItem<Button>("Root/ButtonGroup/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);

        _buttonBuy = GetItem<Button>("Root/ButtonGroup/BuyButton");
        _buttonBuy.onClick.AddListener(OnBuyBtn);

        _buttonGame = GetItem<Button>("Root/ButtonGroup/GameButton");
        _buttonGame.onClick.AddListener(OnGameBtn);
        _buttonHelp = GetItem<Button>("Root/ButtonGroup/HelpButton");
        _buttonHelp.onClick.AddListener(OnHelpBtn);

        _topSlider = GetItem<Slider>("Root/Slider");
        _topSliderText= GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        _lvText= GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Lv/Text");
        
        _timeGroupText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _rewardSlider = GetItem<Slider>("Root/ScrollView/Viewport/Content/Slider");
        _rewardItem = transform.Find("Root/ScrollView/Viewport/Content/EasterMainCell");
        _rewardItem.gameObject.SetActive(false);
        _content=transform.Find("Root/ScrollView/Viewport/Content") as RectTransform;
        
        InvokeRepeating("UpdateTime", 0, 1);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.EASTER_CLAIM,OnClaim);

    }

    private void OnHelpBtn()
    {
        var rewards = EasterModel.Instance.GetEasterReward();
        List<int> itemList = new List<int>();
        foreach (var reward in rewards)
        {
            if(reward.IsBuild)
                itemList.AddRange(reward.RewardId);
        }
        AnimCloseWindow(() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game || FarmModel.Instance.IsFarmModel())
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,DecoOperationType.Preview ,itemList);
            }
            else
            {
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW,itemList);
            }
        });
    }

    private void OnGameBtn()
    {
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        {
            AnimCloseWindow();
        }
        else
        {  AnimCloseWindow(() =>
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.Game);
            });
        }            

    }

    private void OnBuyBtn()
    {
        UIManager.Instance.OpenUI(UINameConst.UIEasterShop);
    }

    private void OnClaim(BaseEvent obj)
    {
        RefreshUI();
      
    }

    protected override void OnOpenWindow(params object[] objs)  
    {
        base.OnOpenWindow(objs);
        var configs = EasterModel.Instance.GetEasterReward();
        for (int i = 0; i < configs.Count; i++)
        {
            var obj=Instantiate(_rewardItem, _content);
            obj.gameObject.SetActive(true);
            var cell=obj.gameObject.AddComponent<EasterMainCell>();
            _rewardItems.Add(cell);
            
        }

        int index = EasterModel.Instance.GetCanClaimIndex();
        if (index >= 0)
        {
            _content.localPosition -=new Vector3( 150*(index-1),0,0);
        }
        else
        {
            var curData= EasterModel.Instance.GetCurIndexData();
            _content.localPosition -=new Vector3( 150*(curData.Id-1),0,0);
        }
        RefreshUI();
    }

    public void RefreshUI()
    {
        var rewards = EasterModel.Instance.GetEasterReward();
        for (int i = 0; i < rewards.Count; i++)
        {
            EasterMainCell.RewardStatus status = EasterMainCell.RewardStatus.None;
            if (EasterModel.Instance.GetScore() >= rewards[i].Score)
            {
                if (!EasterModel.Instance.IsClaimed(rewards[i].Id))
                {
                    status=EasterMainCell.RewardStatus.CanClaim;
                }
                else
                {
                    status=EasterMainCell.RewardStatus.Finish;
                }
            }
            _rewardItems[i].Init(rewards[i],status);
        }
        var curData= EasterModel.Instance.GetCurIndexData();
        int stateScore = EasterModel.Instance.GetIndexStageScore();
        int curScore = Math.Max(0, EasterModel.Instance.GetCurStageScore());
        _topSliderText.SetText(curScore + "/" + stateScore);
        _lvText.SetText(curData.Id.ToString());
        _topSlider.maxValue = 1;
        _topSlider.value = (float) curScore / stateScore;
        _rewardSlider.maxValue = rewards.Count;
        _rewardSlider.minValue = 1;
        _rewardSlider.value= curData.Id-1;
        if (curData.Id == rewards.Count && curScore >= stateScore)
            _rewardSlider.value = rewards.Count;
       
    }

    private void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
        });
    }


    public  void UpdateTime()
    {
        if (EasterModel.Instance.GetActivityLeftTime() <= 0)
        {
            AnimCloseWindow(() =>
            {
                UIManager.Instance.OpenUI(UINameConst.UIEasterEnd,EasterModel.Instance.StorageEaster);
            });
        }
        _timeGroupText.SetText(EasterModel.Instance.GetActivityLeftTimeString());
    }

    public void HideButton()
    {
        foreach (var cell in _rewardItems)
        {
            cell.HideGetBtn();
        }
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

    private static string coolTimeKey = "easter";
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Easter))
            return false;
            
        if (!EasterModel.Instance.IsOpened())
            return false;
        
        EasterModel.Instance.UpdateActivity();
        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIEasterEnd) != null)
            return true;
        
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            if (EasterModel.Instance.IsPreheating())
            {
                UIManager.Instance.OpenUI(UINameConst.UIEasterStart);
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterAdvance);
            }
            else
            {
                if (!EasterModel.Instance.IsShowStart())
                {
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPop);
                    UIManager.Instance.OpenUI(UINameConst.UIEasterStart);
                }
                else
                {
                    UIManager.Instance.OpenUI(UINameConst.UIEasterMain);
                }
            }
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

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.EASTER_CLAIM,OnClaim);
    }
}
