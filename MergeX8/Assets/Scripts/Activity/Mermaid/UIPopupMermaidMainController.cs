
using System.Collections.Generic;
using Deco.Item;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using Farm.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMermaidMainController : UIWindowController
{
    private LocalizeTextMeshProUGUI _topText;
    private LocalizeTextMeshProUGUI _timeText;

    private Slider _slider;

    private GameObject _rewardItem;
    private Button _buttonClose;
    private Button _buttonHelp;
    private Button _button;
    private LocalizeTextMeshProUGUI _buttonText;
    private List<Button> _rewardButtons = new List<Button>();
    private List<int> rewards = new List<int>();
    private List<StageReward> _stageRewards;

    public override void PrivateAwake()
    {
        _topText=GetItem<LocalizeTextMeshProUGUI>("Root/CurrencyGroup/Text");
        _timeText=GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _slider = GetItem<Slider>("Root/SliderGroup/Slider");
        _rewardItem = GetItem("Root/Scroll View/Viewport/Content/1");
        _rewardItem.SetActive(false);
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonHelp = GetItem<Button>("Root/ButtonHelp");
        _button = GetItem<Button>("Root/Button");
        _buttonText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text");

        for (int i = 1; i <= 3; i++)
        {
            int index = i - 1;
            _rewardButtons.Add(transform.Find("Root/SliderGroup/" + i).transform.GetComponent<Button>());
            _rewardButtons[i-1].onClick.AddListener(() =>
            {
                OnButtonReward(index);
            });
        }
        
        var topLayer = new List<Transform>();
        topLayer.Add(_rewardButtons[_rewardButtons.Count-1].transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MermaidPreview, _rewardButtons[_rewardButtons.Count-1].transform as RectTransform, topLayer: topLayer);
        
        var topLayer1 = new List<Transform>();
        topLayer1.Add(_button.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MermaidPlay, _button.transform as RectTransform, topLayer: topLayer1);
        EventDispatcher.Instance.AddEventListener(EventEnum.MERMAID_PURCHASE_SUCCESS,PurchaseSuccess);

    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonHelp.onClick.AddListener(OnBtnHelp);
        _button.onClick.AddListener(OnBtnPlay);

        var configs = MermaidModel.Instance.GetExchangeRewards();
        List<MermaidRewardItem> rewardItems = new List<MermaidRewardItem>();
        for (int i = 0; i < configs.Count; i++)
        {
           var item=  Instantiate(_rewardItem, _rewardItem.transform.parent);
           item.gameObject.SetActive(true);
           var script=item.AddComponent<MermaidRewardItem>();
           script.Init(configs[i],this);
           rewards.Add(configs[i].RewardId);
           rewardItems.Add(script);
        }
        for (int i = 0; i < configs.Count; i++)
        {
            if(MermaidModel.Instance.IsClaimed(configs[i].RewardId))
                rewardItems[i].transform.SetAsLastSibling();
        }
        _topText.SetText(MermaidModel.Instance.GetScore().ToString());
        _slider.maxValue = configs.Count;
        var exchangeCount = MermaidModel.Instance.GetExchangeCount();
        _slider.value = exchangeCount;
        
        _stageRewards = MermaidModel.Instance.GetStageRewards();
        for (int i = 0; i < _stageRewards.Count; i++)
        {
            var image = transform.Find("Root/SliderGroup/"+(i+1)+"/Image").GetComponent<Image>();
            if (exchangeCount >= _stageRewards[i].ExchangeTimes)
            {
                image.transform.parent.gameObject.SetActive(false);
                continue;
            }
            var mergeItem = GameConfigManager.Instance.GetItemConfig(_stageRewards[i].RewardId);
            if (mergeItem != null)
            {
                image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image); 
            }
            else
            {
                var item = DecoManager.Instance.FindItem(_stageRewards[i].RewardId);
                if(item != null)
                    image.sprite =  ResourcesManager.Instance.GetSpriteVariant("MermaidAtlas", item.Config.buildingIcon);  
            }
        }
        
        rewards.Insert(0, _stageRewards[_stageRewards.Count-1].RewardId);
        if (!MermaidModel.Instance.IsExchangeAll())
        {
            InvokeRepeating("UpdateTimeText",1,1);
        }
        UpdateTimeText();

        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MermaidStart, null);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MermaidPreview, null);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MermaidPlay, null);
        UpdateBtnState();
    }

    private void UpdateBtnState()
    {
        if (MermaidModel.Instance.IsWaitBuyExtendDay())
        {
            _buttonText.SetTerm("ui_event_mermaid_extend_time_button2");
        }
        else
        {
            _buttonText.SetTerm("UI_button_play");
        }
    }
    private void UpdateTimeText()
    {
        if (MermaidModel.Instance.IsExchangeAll())
        {
            _timeText.SetTerm("ui_cleanup_desc9");
        }
        else
        {
            if(MermaidModel.Instance.GetActivityLeftTime()<=0)
                _timeText.SetTerm("ui_event_mermaid_extend_time_button2");
            else
            {
                _timeText.SetText(MermaidModel.Instance.GetActivityLeftTimeString());

            }
        }
    }
    private void OnBtnPlay()
    {
        if (MermaidModel.Instance.IsWaitBuyExtendDay())
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidAddDay);
        }
        else
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MermaidPlay);
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        }
   
    }
    private void PurchaseSuccess(BaseEvent obj)
    {
        UpdateBtnState();
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
    private void OnBtnHelp()
    {
        UIManager.Instance.OpenUI(UINameConst.MermaidHelp);
    }

    private void OnButtonReward(int index)
    {
        if (index == 2)
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MermaidPreview);
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game|| FarmModel.Instance.IsFarmModel())
                {
                    SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,DecoOperationType.Preview ,rewards);
                }
                else
                {
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW, rewards);
                }
            });
        }
        else
        {
            MergeInfoView.Instance.OpenMergeInfo(_stageRewards[index].RewardId, isShowGetResource:false);

        }
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.MERMAID_PURCHASE_SUCCESS,PurchaseSuccess);
    }
}
