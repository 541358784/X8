using System;
using System.Collections.Generic;
using Deco.Item;
using Deco.World;
using DragonPlus;
using DragonPlus.Config.HappyGo;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using GamePool;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupHappyGoRewardController : UIWindowController
{
    
    private Slider _topSlider;
    private LocalizeTextMeshProUGUI _sliderText;
    private Image _icon1;
    private LocalizeTextMeshProUGUI _lvText;
    private Image _icon2;
    private LocalizeTextMeshProUGUI _rewardNum;

    private LocalizeTextMeshProUGUI _timeText;
    private Button _closeBtn;
    private Button _btnGo;
    private Slider _mainSlider;
    private HGVDLevel _currentLevel;
    private HGVDLevel _nextLevel;
    private Transform _rewrdItem;
    private List<HappyGoRewardItem> _happyGoRewardList;
    private RectTransform _content;
    private Button _helpBtn;
    public override void PrivateAwake()
    {
        _topSlider = transform.Find("Root/TopGroup/Slider").GetComponent<Slider>();
        _sliderText = transform.Find("Root/TopGroup/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _icon1 = transform.Find("Root/TopGroup/Icon1").GetComponent<Image>();
        _icon2 = transform.Find("Root/TopGroup/Icon2").GetComponent<Image>();
        _lvText = transform.Find("Root/TopGroup/LvGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardNum = transform.Find("Root/TopGroup/RewardNum").GetComponent<LocalizeTextMeshProUGUI>();
      
        _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _closeBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
        _btnGo = transform.Find("Root/Button").GetComponent<Button>();
        _mainSlider = transform.Find("Root/Scroll View/Viewport/Content/Slider").GetComponent<Slider>();
        _rewrdItem = transform.Find("Root/Scroll View/Viewport/Content/Reward");
        _rewrdItem.gameObject.SetActive(false);
        _happyGoRewardList = new List<HappyGoRewardItem>();
        _content=transform.Find("Root/Scroll View/Viewport/Content") as RectTransform;
        _helpBtn = transform.Find("Root/HelpButton").GetComponent<Button>();
        _helpBtn.onClick.AddListener(OnHelp);
        EventDispatcher.Instance.AddEventListener(EventEnum.HAPPYGO_CLAIM_REWARD, OnClaimReward);
        _closeBtn.onClick.AddListener(OnBtnClose);
        _btnGo.onClick.AddListener(OnBtnClose);
        InvokeRepeating("UpdateTime",0f,1);
    }
    
    protected override void OnOpenWindow(params object[] objs)
    {
        GuideSubSystem.Instance.ClearTarget(GuideTargetType.HappyGoLevelUpReward);
        base.OnOpenWindow(objs);
        Init();
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.HappyGoLevelUpReward, HappyGoModel.Instance.GetLevel().ToString());
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdRewards);
    }
    
    public void Init()
    {
        _topSlider.value = HappyGoModel.Instance.GetNextLevelProgress();
        _lvText.SetText(HappyGoModel.Instance.GetLevel().ToString());
        _currentLevel= HappyGoModel.Instance.GetCurrentLevel();
        _nextLevel= HappyGoModel.Instance.GetNextLevelConfig();
        
        _rewardNum.SetText(_nextLevel.amount[0].ToString());
        
        if (UserData.Instance.IsResource(_nextLevel.reward[0]))
        {
            _icon2.sprite = UserData.GetResourceIcon(_nextLevel.reward[0]);
        }
        else
        {
            var itemConfig= GameConfigManager.Instance.GetItemConfig(_nextLevel.reward[0]);
            if(itemConfig != null)
                _icon2.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            else
            {
                DecoItem decoItem = null;
                if (DecoWorld.ItemLib.ContainsKey(_nextLevel.reward[0]))
                    decoItem = DecoWorld.ItemLib[_nextLevel.reward[0]];

                if (decoItem != null)
                {
                    _icon2.sprite = ResourcesManager.Instance.GetSpriteVariant("HappyGoAtlas", decoItem.Config.buildingIcon);
                    
                    _rewardNum.gameObject.SetActive(false);
                } 
            }
        }
        _sliderText.SetText(HappyGoModel.Instance.GetProgressStr());

        var levels= HappyGoModel.Instance.HappyGoLevelList;
        for (int i = 0; i < levels.Count; i++)
        {
           var item=  Instantiate(_rewrdItem, _rewrdItem.parent);
           item.gameObject.SetActive(true);
           var itemScr=  item.gameObject.AddComponent<HappyGoRewardItem>();
           itemScr.Init(levels[i]);
           _happyGoRewardList.Add(itemScr);
        }
        
        UpdateReward();
        _content.localPosition = new Vector3(0, HappyGoModel.Instance.storageHappy.ClaimLevel*170, 0);
    }
    
   
    private void OnClaimReward(BaseEvent obj)
    {
        UpdateReward();
    }
    
    public void UpdateReward()
    {
        foreach (var rewardItem in _happyGoRewardList)
        {
            rewardItem.UpdateStatus();
        }

        _mainSlider.maxValue = HappyGoModel.Instance.HappyGoLevelList.Count-1;
        _mainSlider.value = HappyGoModel.Instance.storageHappy.ClaimLevel-1;
    }

    public void UpdateTime()
    {
        _timeText.SetText(HappyGoModel.Instance.GetActivityLeftTimeString());
    }
    private void OnHelp()
    {
        UIManager.Instance.OpenUI(UINameConst.UIHappyGoHelp);
    }

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HAPPYGO_CLAIM_REWARD, OnClaimReward);
    }
}
