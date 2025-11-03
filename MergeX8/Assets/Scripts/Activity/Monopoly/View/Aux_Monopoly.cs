using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_Monopoly : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private MonopolyDiceRedPoint RedPoint;
    private StorageMonopoly Storage;
    public void SetStorage(StorageMonopoly storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<MonopolyDiceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(MonopolyModel.Instance.CurStorageMonopolyWeek);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage!= null && Storage.ShowEntrance());
        if (!gameObject.activeSelf)
            return;
        if (Storage.GetPreheatLeftTime() > 0)
        {
            _timeText.SetText(Storage.GetPreheatLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
        else
        {
            _timeText.SetText(Storage.GetLeftTimeText());
            RedPoint.UpdateUI();
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyAuxItem);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupMonopolyPreviewController.Open(Storage);
        }
        else
        {
            UIMonopolyMainController.Open(Storage);
        }
    }
    private void OnDestroy()
    {
    }
}
