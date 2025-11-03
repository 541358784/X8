using System;
using System.Collections.Generic;
using Activity.TreasureHuntModel;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_ButterflyWorkShop : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointLabel;
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointLabel = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ButterFlyWorkShopStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(ButterflyWorkShopModel.Instance.IsOpened());
        if (!gameObject.activeSelf)
            return;
        
        _timeText.SetText(ButterflyWorkShopModel.Instance.GetActivityLeftTimeString());
        _redPoint.gameObject.SetActive(ButterflyWorkShopModel.Instance.UnSetItemsCount>0 );
        _redPointLabel.gameObject.SetActive(ButterflyWorkShopModel.Instance.UnSetItemsCount>0);
        _redPointLabel.SetText(ButterflyWorkShopModel.Instance.UnSetItemsCount.ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ButterFlyWorkShopStart);
        UIManager.Instance.OpenUI(UINameConst.UIButterflyWorkShopMain);
    }
  
}
