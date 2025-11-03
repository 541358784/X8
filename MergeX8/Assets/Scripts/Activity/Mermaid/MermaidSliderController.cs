
using Decoration;
using DG.Tweening;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public class MermaidSliderController : UIWindowController
{
    private Slider _slider;
    private GameObject _sliderEff;
    public override void PrivateAwake()
    {
        _slider = GetItem<Slider>("Root/Slider/Slider");
        _sliderEff = GetItem("Root/Slider/Slider/Fill Area/Fill/VFX_Slider");
      
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Init(MermaidModel.Instance.GetExchangeCount()-1);
    }

    private void Init(int exchangeCount)
    {
        var stageRewards = MermaidModel.Instance.GetStageRewards();
        var exchangeRewards = MermaidModel.Instance.GetExchangeRewards();
        _slider.maxValue=exchangeRewards.Count;
        _slider.value = exchangeCount;
        for (int i = 0; i < stageRewards.Count; i++)
        {
            var image = transform.Find("Root/Slider/"+(i+1)+"/Image").GetComponent<Image>();
            if (exchangeCount >= stageRewards[i].ExchangeTimes)
            {
                image.transform.parent.gameObject.SetActive(false);
                continue;
            }
            var mergeItem = GameConfigManager.Instance.GetItemConfig(stageRewards[i].RewardId);
            if (mergeItem != null)
            {
                image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image); 
            }
            else
            {
                var item = DecoManager.Instance.FindItem(stageRewards[i].RewardId);
                if(item != null)
                    image.sprite =  ResourcesManager.Instance.GetSpriteVariant("MermaidAtlas", item.Config.buildingIcon);  
            }
        }
        _sliderEff?.gameObject.SetActive(true);
        _slider.DOValue(exchangeCount + 1, 1.5f).onComplete += () =>
        {   
            _sliderEff?.gameObject.SetActive(false);
            bool isHaveReward=MermaidModel.Instance.ClaimStateReward(() =>
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidMain);
            });
            AnimCloseWindow();
         
        };
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        MermaidModel.Instance.ClaimStateReward(() =>
        {
        });
        
        AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true);
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, true);
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY);
        
    }
}
