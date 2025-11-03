using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_GiftBagProgress : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private GiftBagProgressRedPoint RedPoint;
    private StorageGiftBagProgress Storage;
    public void SetStorage(StorageGiftBagProgress storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<GiftBagProgressRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);
        SetStorage(GiftBagProgressModel.Instance.Storage);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(Storage.GetLeftTimeText());
        RedPoint.UpdateUI();
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GiftBagProgressAuxItem);
        UIPopupGiftBagProgressTaskController.Open(Storage);
    }
    private void OnDestroy()
    {
    }
}
