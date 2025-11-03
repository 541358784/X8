using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_BiuBiu : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_BiuBiu Instance;
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
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BiuBiuStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(BiuBiuModel.Instance.IsPrivateOpened());
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(BiuBiuModel.Instance.GetActivityLeftTimeString());
        RedPoint.gameObject.SetActive(BiuBiuModel.Instance.Storage.UnSetItems.Count > 0);
        RedPointText.SetText(BiuBiuModel.Instance.Storage.UnSetItems.Count.ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BiuBiuStart);
        if (BiuBiuModel.Instance.IsPrivateOpened())
        {
            UIBiuBiuMainController.Open();
        }
    }
    private void OnDestroy()
    {
    }
}
