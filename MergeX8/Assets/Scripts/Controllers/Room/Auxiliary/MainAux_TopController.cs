using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Game;
using Gameplay;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MainAux_TopController : SuperMainController
{
    private List<Aux_ItemBase> _auxItems = new List<Aux_ItemBase>();
    public Button _setButton;
    private Button _mailButton;

    private void Start()
    {
        _mailButton = CommonUtils.Find<Button>(transform, "Root/Views/MainGroup/TopGroup/ButtonMail");
        
        _setButton = CommonUtils.Find<Button>(transform, "Root/Views/MainGroup/TopGroup/ButtonSet");
        _setButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(SfxNameConst.button_s);
            UIManager.Instance.OpenUI(UINameConst.UIPopupSet1, false);
        });
        var redPoint = _setButton.transform.Find("RedPoint").gameObject.AddComponent<UIPopupSetRedPoint>();
        redPoint.Init();
        _auxItems.Clear();
   
    
        Aux_ItemBase auxItem = CommonUtils.FindOrCreate<Aux_Mail>(transform, "Root/Views/MainGroup/TopGroup/ButtonMail");
        if (auxItem != null)
            _auxItems.Add(auxItem); 
        
        auxItem = CommonUtils.FindOrCreate<Aux_DailyBonus>(transform, "Root/Views/MainGroup/TopGroup/ButtonDailyBouns");
        if (auxItem != null)
            _auxItems.Add(auxItem);

        auxItem = CommonUtils.FindOrCreate<Aux_Shop>(transform, "Root/Views/MainGroup/ButtonShop");
        if (auxItem != null)
            _auxItems.Add(auxItem);  
        
        transform.Find("Root/Views/MainGroup/LeftGroup/Content/ButtonTaskCenter").gameObject.SetActive(false);
        
        _auxItems.ForEach(item => { item.Init(mainController); });
        UpdateUI();

        InvokeRepeating("UpdateUI", 0, 1);
    }

    public void UpdateState()
    {
        if(_auxItems != null)
            _auxItems.ForEach(item => { item.UpdateUI(); });

        UpdateUI();
    }
    public override void Show()
    {
        _auxItems.ForEach(item => { item.UpdateUI(); });
        UpdateUI();
    }
    public  void UpdateUI()
    {
        if (!gameObject.activeSelf)
            return;
        
         _mailButton?.gameObject.SetActive(MailDataModel.Instance.HasNoReadMails() );
    }
    
    public override void Hide()
    {
    }

    public override void MoneyAnim(UserData.ResourceId resId, int subNum, float time)
    {
    }

    public override void InitMoney(UserData.ResourceId resId, int money)
    {
    }
}