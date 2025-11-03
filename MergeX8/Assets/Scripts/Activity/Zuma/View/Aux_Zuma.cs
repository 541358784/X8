using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_Zuma : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private ZumaDiceRedPoint RedPoint;
    private StorageZuma Storage;
    public void SetStorage(StorageZuma storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<ZumaDiceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(ZumaModel.Instance.CurStorageZumaWeek);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage != null && Storage.ShowAuxItem());
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
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ZumaEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupZumaPreviewController.Open(Storage);
        }
        else
        {
            if (Storage.IsStart)
            {
                UIZumaMainController.Open(Storage);
            }
            else
            {
                ZumaModel.CanShowStartPopup();
            }
        }
    }
    private void OnDestroy()
    {
    }
}
