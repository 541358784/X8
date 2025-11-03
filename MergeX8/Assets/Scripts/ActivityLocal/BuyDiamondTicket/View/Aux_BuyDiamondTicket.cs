using System;
using DragonPlus;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_BuyDiamondTicket : Aux_ItemBase
{
    public static Aux_BuyDiamondTicket Instance;
    private LocalizeTextMeshProUGUI TimeText;
    private BuyDiamondTicketModel Model => BuyDiamondTicketModel.Instance;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
        InvokeRepeating("UpdateUI", 0, 1);
        transform.Find("RedPoint").gameObject.SetActive(false);
        TimeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(Model.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        TimeText.SetText(Model.GetActiveTicket().GetTicketLeftTimeText());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIBuyDiamondTicketController.Open(Model.GetActiveTicket());
    }
    private void OnDestroy()
    {
    }
}
