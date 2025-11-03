using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_ThemeDecoration : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private ThemeDecorationShopRedPoint RedPoint;
    private StorageThemeDecoration Storage;
    public void SetStorage(StorageThemeDecoration storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<ThemeDecorationShopRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);
        SetStorage(ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage.ShowEntrance());
        if (!gameObject.activeSelf)
            return;
        if (!Storage.IsPreheat())
        {
            _timeText.SetText(Storage.GetPreheatLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
        else if(!Storage.IsTotalTimeOut())
        {
            _timeText.SetText(Storage.GetTotalLeftTimeText());
            RedPoint.UpdateUI();
        }
        else if (Storage.CanBuyEnd())
        {
            _timeText.SetText(Storage.GetPreEndBuyLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!Storage.IsResExist())
            return;
        if (!Storage.IsPreheat())
        {
            UIPopupThemeDecorationPreviewController.Open(Storage);
        }
        else if(!Storage.IsTotalTimeOut())
        {
            UIThemeDecorationShopController.Open(Storage);
        }
        else if (Storage.CanBuyEnd())
        {
            UIPopupThemeDecorationBuyPreEndController.Open(Storage);
        }
    }
    private void OnDestroy()
    {
    }
}
