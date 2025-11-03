using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_GiftBagDouble : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private StorageGiftBagDouble Storage;
    public void SetStorage(StorageGiftBagDouble storage)
    {
        Storage = storage;
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        transform.Find("RedPoint")?.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(GiftBagDoubleModel.Instance.Storage);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage != null && Storage.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(Storage.GetLeftTimeText());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GiftBagDoubleAuxItem);
        UIPopupGiftBagDoubleMainController.Open(Storage);
    }
    private void OnDestroy()
    {
    }
}
