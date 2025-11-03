using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeBuyDiamondTicket : MonoBehaviour
{
    private Button _btn;
    private LocalizeTextMeshProUGUI TimeText;
    private BuyDiamondTicketModel Model => BuyDiamondTicketModel.Instance;

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("RefreshView", 0, 1f);
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        UIBuyDiamondTicketController.Open(Model.GetActiveTicket());
    }

    private void RefreshView()
    {
        gameObject.SetActive(Model.ShowTaskEntrance());
        if (Model.GetActiveTicket() != null)
            TimeText.SetText(Model.GetActiveTicket().GetTicketLeftTimeText());
    }
}