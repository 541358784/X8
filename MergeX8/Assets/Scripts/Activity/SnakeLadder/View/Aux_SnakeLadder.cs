using System;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_SnakeLadder : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_SnakeLadder Instance;
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
        gameObject.SetActive(SnakeLadderModel.Instance.IsPrivateOpened());
        if (!gameObject.activeSelf)
            return;
        if (SnakeLadderModel.Instance.IsStart())
        {
            _timeText.SetText(SnakeLadderModel.Instance.GetActivityLeftTimeString());   
        }
        else
        {
            _timeText.SetText(SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.GetPreheatLeftTimeText());
        }
        RedPoint.gameObject.SetActive(SnakeLadderModel.Instance.GetTurntableCount() > 0);
        RedPointText.SetText(SnakeLadderModel.Instance.GetTurntableCount().ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (SnakeLadderModel.Instance.IsStart())
        {
            SnakeLadderModel.CanShowMainPopup();   
        }
        else
        {
            SnakeLadderModel.CanShowPreheatPopup();
        }
    }
    private void OnDestroy()
    {
    }
}
