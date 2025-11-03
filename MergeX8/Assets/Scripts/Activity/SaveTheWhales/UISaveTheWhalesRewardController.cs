using System.Collections.Generic;
using Activity.SaveTheWhales;
using DragonPlus;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UISaveTheWhalesRewardController : UIWindowController
{
    private Button _buttonClose;
    private Transform _rewardItem;
    private Slider _slider;
    private LocalizeTextMeshProUGUI _sliderText;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _btnText;
    private Button _button;
    private bool _isCompleteAll;
    private SkeletonGraphic hero;
    private SkeletonGraphic horse;
    private LocalizeTextMeshProUGUI TipText;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);   
        _button = GetItem<Button>("Root/Button");
        _btnText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text");
        _button.onClick.AddListener(OnBtnClose);
        _rewardItem = transform.Find("Root/RewardGroup/Item");
        _rewardItem.gameObject.SetActive(false);
        _slider = GetItem<Slider>("Root/Slider");
        _sliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        hero = GetItem<SkeletonGraphic>("Root/BGGroup/SkeletonGraphic (hero2)");
        horse = GetItem<SkeletonGraphic>("Root/BGGroup/SkeletonGraphic (horse)");
        InvokeRepeating("RefreshCountDown", 0, 1f);
        TipText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/Text");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length >= 1)
        {
            _isCompleteAll = true;
        }
        var config= SaveTheWhalesModel.Instance.SaveTheWhalesActivityConfig;
        for (int i = 0; i < config.RewardId.Count; i++)
        {
            var obj = Instantiate(_rewardItem, _rewardItem.parent);
            obj.gameObject.SetActive(true);
            InitItem(obj,config.RewardId[i],config.RewardNum[i]);
        }
        _sliderText.SetText(SaveTheWhalesModel.Instance.GetWater()+"/"+config.CollectCount);
        _slider.value = (float)SaveTheWhalesModel.Instance.GetWater() / config.CollectCount;
        if (_isCompleteAll)
        {
            _btnText.SetTerm("UI_button_ok");
            PlaySkeletonAnimation("happy", hero);
            PlaySkeletonAnimation("happy", horse);
            AudioManager.Instance.PlaySound(169);
            TipText.SetTerm("UI_ExtraOrder_Finish");
        }
        else
        {
            AudioManager.Instance.PlaySound(168);

        }

    }

    private void InitItem(Transform item,int itemID,int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID))
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID,UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else
            {
                Debug.LogError("Get MergeItemConfig---null " + itemID);
            }
        }

        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
        bool isRes = UserData.Instance.IsResource(itemID);
        var infoBtn = item.transform.Find("TipsBtn");
        if (infoBtn != null)
        {
            infoBtn.gameObject.SetActive(!isRes);
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
                itemButton.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemID, null,_isShowProbability:true);
                    UIPopupMergeInformationController controller =
                        UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                    if (controller != null)
                        controller.SetResourcesActive(false);
                });
            }
        }
    }
    private void AnimEndLogic()
    {
        if(!_isCompleteAll)
            return;
        
        var config =SaveTheWhalesModel.Instance.SaveTheWhalesActivityConfig;
        if (config == null)
            return;
        
        List<ResData> resDatas = new List<ResData>();
        for(int i = 0;i < config.RewardId.Count; i++)
        {
            ResData resData = new ResData(config.RewardId[i], config.RewardNum[i]);
            resDatas.Add(resData);
        }
        CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController,true);
    }
    private void RefreshCountDown()
    { 
        _timeText.SetText(SaveTheWhalesModel.Instance.GetLeftTimeStr());
    }
    private void OnBtnClose()
    {
        AnimCloseWindow(AnimEndLogic);
    }
    
    private float PlaySkeletonAnimation(string animName ,SkeletonGraphic _skeletonGraphic)
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
