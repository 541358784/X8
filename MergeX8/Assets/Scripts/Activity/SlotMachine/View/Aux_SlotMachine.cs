using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_SlotMachine : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private SlotMachineRedPoint RedPoint;
    private StorageSlotMachine Storage;
    public void SetStorage(StorageSlotMachine storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<SlotMachineRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(SlotMachineModel.Instance.CurStorage);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage != null && Storage.ShowEntrance());
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(SlotMachineModel.Instance.GetActivityLeftTimeString());
    }
    
    protected override void OnButtonClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SlotMachineAuxItem);
        base.OnButtonClick();
        if (!Storage.ShowEntrance())
            return;
        UIPopupSlotMachineMainController.Open(Storage);
    }
    private void OnDestroy()
    {
    }
}
