using System;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_BlindBox : Aux_ItemBase
{
    public static Aux_BlindBox Instance;
    private BlingBoxRedPoint RedPoint;
    private Transform FreeTag;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
        InvokeRepeating("UpdateUI", 0, 1);
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<BlingBoxRedPoint>();
        RedPoint.Init(true,true);
        FreeTag = transform.Find("FreeTag");
        FreeTag.gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(BlindBoxModel.Instance.IsUnlock);
        if (!gameObject.activeSelf)
            return;
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIBlindBoxMainController.Open();
    }
    private void OnDestroy()
    {
    }
}
