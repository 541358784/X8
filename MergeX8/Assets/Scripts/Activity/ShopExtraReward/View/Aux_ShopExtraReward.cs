using System;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_ShopExtraReward : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_ShopExtraReward Instance;
    private Transform RedPoint;
    private LocalizeTextMeshProUGUI RedPointText;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint");
        RedPoint.gameObject.SetActive(false);
        RedPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        RedPointText.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(ShopExtraRewardModel.Instance.ShowEntrance());
        if (!gameObject.activeSelf)
            return;
        if (ShopExtraRewardModel.Instance.IsPrivateOpened())
        {
            _timeText.SetText(ShopExtraRewardModel.Instance.GetActivityLeftTimeString());
            // var leftTimes = ShopExtraRewardModel.Instance.GetLeftBuyTimes();
            // RedPointText.SetText(leftTimes.ToString());
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (ShopExtraRewardModel.Instance.IsPrivateOpened())
        {
            UIPopupShopExtraRewardStartController.Open();
        }
    }
    private void OnDestroy()
    {
    }
}
