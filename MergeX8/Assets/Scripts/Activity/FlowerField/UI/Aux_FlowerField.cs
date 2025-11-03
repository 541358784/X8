using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_FlowerField : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_FlowerField Instance;
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
        gameObject.SetActive(FlowerFieldModel.Instance.IsPrivateOpened());
        if (!gameObject.activeSelf)
            return;
        if (FlowerFieldModel.Instance.IsStart())
        {
            _timeText.SetText(FlowerFieldModel.Instance.GetActivityLeftTimeString());
        }
        else
        {
            _timeText.SetText(FlowerFieldModel.Instance.Storage.GetPreheatLeftTimeText());
        }
        RedPoint.gameObject.SetActive(false);
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FlowerFieldStart);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FlowerFieldLeaderBoardHomeEntrance);
        if (FlowerFieldModel.Instance.IsStart())
        {
            // if (!FlowerFieldModel.CanShowStartPopup())
                UIFlowerFieldMainController.Open();
        }
        else
        {
            UIPopupFlowerFieldPreviewController.Open();
        }
    }
    private void OnDestroy()
    {
    }
}
