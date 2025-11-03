using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_NewDailyPack : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    public static Aux_NewDailyPack Instance;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.NewDaily_Pack_Begin, OnPackBegin);
        NewDailyPackModel.Instance.RefreshPack();
    }
    private void OnPackBegin(BaseEvent e)
    {
        UpdateUI();
    }
    public override void UpdateUI()
    {
        // if (!this)
        //     EventDispatcher.Instance.RemoveEventListener(EventEnum.NewDaily_Pack_Begin, OnPackBegin);
        gameObject.SetActive(NewDailyPackModel.Instance.IsOpen());
        if (gameObject.activeSelf)
        {
            int leftTime = NewDailyPackModel.Instance.GetPackCoolTime() * 1000;
            if (leftTime <= 0)
                NewDailyPackModel.Instance.RefreshPack();
            _timeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
        }
    }
    protected override void OnButtonClick()
    {
        base.OnButtonClick();   
        NewDailyPackModel.Instance.RefreshPack();
        UIManager.Instance.OpenUI(UINameConst.UIPopupNewDailyGift, "pack");
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NewDaily_Pack_Begin, OnPackBegin);
    }
}
