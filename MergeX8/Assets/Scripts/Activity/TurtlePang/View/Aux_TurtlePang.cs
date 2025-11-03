using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_TurtlePang : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private StorageTurtlePang Storage;
    private TurtlePangEntranceRedPoint RedPoint;
    public void SetStorage(StorageTurtlePang storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<TurtlePangEntranceRedPoint>();
        RedPoint.gameObject.SetActive(true);

        SetStorage(TurtlePangModel.Instance.Storage);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage != null && Storage.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        if (Storage.GetPreheatTime() > 0)
        {
            _timeText.SetText(Storage.GetPreheatTimeText());
        }
        else
        {
            _timeText.SetText(Storage.GetLeftTimeText());   
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TurtlePangEntrance);
        if (Storage.GetPreheatTime() > 0)
        {
            UIPopupTurtlePangPreviewController.Open(Storage);
        }
        else
        {
            UITurtlePangMainController.Open(Storage);
        }
    }
    private void OnDestroy()
    {
    }
}
