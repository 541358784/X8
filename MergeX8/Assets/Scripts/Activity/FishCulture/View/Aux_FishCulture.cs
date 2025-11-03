using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_FishCulture : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private FishCultureRedPoint RedPoint;

    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<FishCultureRedPoint>();
        RedPoint.Init(FishCultureModel.Instance.CurStorageFishCultureWeek);
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(FishCultureModel.Instance.CurStorageFishCultureWeek.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        if (FishCultureModel.Instance.CurStorageFishCultureWeek.GetPreheatLeftTime() > 0)
        {
            _timeText.SetText(FishCultureModel.Instance.CurStorageFishCultureWeek.GetPreheatLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
        else
        {
            _timeText.SetText(FishCultureModel.Instance.CurStorageFishCultureWeek.GetLeftPreEndTimeText());
            RedPoint.UpdateUI();
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FishCultureEntrance);
        if (FishCultureModel.Instance.CurStorageFishCultureWeek.GetPreheatLeftTime() > 0)
        {
            UIPopupFishCulturePreviewController.Open(FishCultureModel.Instance.CurStorageFishCultureWeek);
        }
        else
        {
            if (FishCultureModel.Instance.CurStorageFishCultureWeek.IsStart)
            {
                UIFishCultureMainController.Open();
            }
            else
            {
                FishCultureModel.CanShowStartPopup();
            }
        }
    }
    private void OnDestroy()
    {
    }
}
