using System;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_Easter2024 : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_Easter2024 Instance;
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
        RedPointText.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(Easter2024Model.Instance.IsPrivateOpened());
        if (!gameObject.activeSelf)
            return;
        if (Easter2024Model.Instance.IsStart())
        {
            _timeText.SetText(Easter2024Model.Instance.GetActivityLeftTimeString());   
        }
        else
        {
            _timeText.SetText(Easter2024Model.Instance.CurStorageEaster2024Week.GetPreheatLeftTimeText());
        }
        RedPoint.gameObject.SetActive(Easter2024Model.Instance.GetEgg() > 0);
        RedPointText.SetText(Easter2024Model.Instance.GetEgg().ToString());
    }
    
    protected override void OnButtonClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024GuideStart);
        base.OnButtonClick();
        if (Easter2024Model.Instance.IsStart())
        {
            Easter2024Model.CanShowMainPopup();   
        }
        else
        {
            Easter2024Model.CanShowPreheatPopup();
        }
    }
    private void OnDestroy()
    {
    }
}
