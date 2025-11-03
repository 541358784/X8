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

public class Aux_CardCollectionTheme : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_CardCollectionTheme Instance;
    private CardCollectionEntranceThemeRedPoint _redPoint;
    private CardCollectionCardThemeState Theme;
    public void SetTheme(CardCollectionCardThemeState theme)
    {
        Theme = theme;
        _redPoint.SetTheme(Theme);
    }
    protected override void Awake()
    {
        Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint").gameObject.AddComponent<CardCollectionEntranceThemeRedPoint>();
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(CardCollectionActivityModel.Instance.IsOpened() && CardCollectionModel.Instance.IsResReady(Theme));
        if (gameObject.activeSelf)
        {
            var openTheme = CardCollectionActivityModel.Instance.GetThemeOnOpened();
            if (openTheme != Theme && openTheme.GetUpGradeTheme() != Theme)//不是本期卡册就隐藏
            {
                gameObject.SetActive(false);
            }
            if (Theme.GetUpGradeTheme() != Theme)//变金卡册也隐藏
            {
                gameObject.SetActive(false);
            }
        }
        if (Theme.CardThemeConfig.NeedUnLock && 
            !CardCollectionModel.Instance.StorageCardCollection.UnlockThemeList.Contains(Theme.CardThemeConfig.Id))//未解锁卡册也隐藏
        {
            gameObject.SetActive(false);
        }
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(CardCollectionActivityModel.Instance.GetActivityLeftTimeString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UICardMainController.Open(Theme);
    }
    private void OnDestroy()
    {
    }
}