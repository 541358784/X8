using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    private bool InitBuyDiceNodeFlag = false;
    public void InitBuyDiceNode()
    {
        if (InitBuyDiceNodeFlag)
            return;
        InitBuyDiceNodeFlag = true;
        var buyDiceNode = transform.Find("Root/ButtonBuySieve").gameObject.AddComponent<BuyDiceNode>();
        buyDiceNode.MainUI = this;
    }
    public class BuyDiceNode:MonoBehaviour
    {
        public UIMonopolyMainController MainUI;
        private Button Btn;
        
        private void Awake()
        {
            Btn = transform.gameObject.GetComponent<Button>();
            Btn.onClick.AddListener(OnClick);
        }
        public void OnClick()
        {
            if (MainUI.isPlaying)
                return;
            UIPopupMonopolyNoDiceController.Open(MainUI.Storage);
        }
    }
}