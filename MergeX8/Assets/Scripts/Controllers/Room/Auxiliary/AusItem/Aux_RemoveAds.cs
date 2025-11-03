using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;

public class Aux_RemoveAds : Aux_ItemBase
{
    protected override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener(EventEnum.OnIAPItemPaid, UpdateRvReward);
    }

    public override void Init(UIHomeMainController mainController)
    {
        base.Init(mainController);
        UpdateUI();
    }

    public override void UpdateUI()
    {
        // if (LevelGroupSystem.Instance.GetCurLevelCountIdx() < GlobalConfigManager.Instance.GetNumValue("RemoveAdsOpenLevel"))
        // {
        //     gameObject.SetActive(false);
        //     return;
        // }
        gameObject.SetActive(StorageManager.Instance.GetStorage<StorageHome>().ShopData.GotNoAds == false);
    }

    private void UpdateRvReward(BaseEvent e)
    {
        UpdateUI();
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIManager.Instance.OpenUI(UINameConst.UISKIPADS);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OnIAPItemPaid, UpdateRvReward);
    }
}