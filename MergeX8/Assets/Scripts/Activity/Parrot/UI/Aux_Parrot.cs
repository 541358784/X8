using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_Parrot : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_Parrot Instance;
    private Transform RedPoint;
    private LocalizeTextMeshProUGUI RedPointText;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint");
        RedPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        RedPointText.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(ParrotModel.Instance.IsPrivateOpened());
        if (!gameObject.activeSelf)
            return;
        if (ParrotModel.Instance.IsStart())
        {
            _timeText.SetText(ParrotModel.Instance.GetActivityLeftTimeString());
        }
        else
        {
            _timeText.SetText(ParrotModel.Instance.Storage.GetPreheatLeftTimeText());
        }
        RedPoint.gameObject.SetActive(false);
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ParrotStart);
        if (ParrotModel.Instance.IsStart())
        {
            if (!ParrotModel.CanShowStartPopup())
                UIParrotMainController.Open();
        }
        else
        {
            UIPopupParrotPreviewController.Open();
        }
    }
    private void OnDestroy()
    {
    }
}
