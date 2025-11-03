using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupButterflyWorkShopStartController:UIWindowController
{
    private Button _playBtn;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _playBtn = GetItem<Button>("Root/Button");
        _playBtn.onClick.AddListener(OnPlayBtn);
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnCloseBtn);
        ButterflyWorkShopModel.Instance.StorageButterflyWorkShop.IsStart = true;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_playBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ButterFlyWorkShopStart, _playBtn.transform as RectTransform, topLayer: topLayer);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ButterFlyWorkShopStart, null);   
    }

    public void OnPlayBtn()
    {
        AnimCloseWindow(() =>
        {
            ButterflyWorkShopModel.Instance.OpenMainPopup();
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ButterFlyWorkShopStart);
        });
    }

    public void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
           
        });
    }
    
    
}