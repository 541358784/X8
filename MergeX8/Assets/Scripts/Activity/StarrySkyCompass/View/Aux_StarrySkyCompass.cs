using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_StarrySkyCompass : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private StorageStarrySkyCompass Storage;
    private StarrySkyCompassEntranceRedPoint RedPoint;
    public void SetStorage(StorageStarrySkyCompass storage)
    {
        Storage = storage;
        RedPoint.Init(Storage,true,true);
    }
    protected override void Awake()
    {
        base.Awake();
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<StarrySkyCompassEntranceRedPoint>();
        RedPoint.gameObject.SetActive(true);

        SetStorage(StarrySkyCompassModel.Instance.Storage);
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
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StarrySkyCompassEntrance);
        if (Storage.GetPreheatTime() > 0)
        {
            UIPopupStarrySkyCompassPreviewController.Open(Storage);
        }
        else
        {
            UIStarrySkyCompassMainController.Open(Storage);
        }
    }
    private void OnDestroy()
    {
    }
}
