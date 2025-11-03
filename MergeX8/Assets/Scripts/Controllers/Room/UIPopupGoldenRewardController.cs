using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Game.Config;
using Gameplay;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupGoldenRewardController : UIWindowController
{
    private Button _buttonClose;
    private List<UIPopupGoldRewardItem> _rewardItems;
    private LocalizeTextMeshProUGUI timeText;
    private string coolTimeKey = "RvRewardCookTimeKey";
    private static string coolTimeLevelKey = "RvRewardCookTimeLevelKey";

    private Image uiSliderImage = null;
    private GameObject contentObj = null;

    private GameObject gameItem = null;

    private int sliderLength = 0;

    public override void PrivateAwake()
    {
        CommonUtils.TweenOpen(GetItem("Root").transform);

        contentObj = GetItem("Root/MiddleGroup/Scroll View/Viewport/Content");

        uiSliderImage = GetItem<Image>("Root/MiddleGroup/Scroll View/Viewport/Content/UISlider");

        gameItem = GetItem("Root/MiddleGroup/Scroll View/Viewport/Content/item0");
        gameItem.SetActive(false);

        _rewardItems = new List<UIPopupGoldRewardItem>();

        // GetItem<ScrollRect>("Root/MiddleGroup/Scroll View").onValueChanged.AddListener(arg0 =>
        // {
        //     if(arg0.y <= 0 || arg0.y >= 1)
        //         return;
        //     
        //     foreach (var kv in _rewardItems)
        //     {
        //         kv.RestUI();
        //     }
        // });

        //var rvRewards=  M3ConfigManager.Instance.GoldenConfig;
        //
        // int rewardIndex = 0;
        // int allLength = (rvRewards.Count-4)*175 + (rvRewards.Count-3)*8;
        //
        //  for (int i = rvRewards.Count-1; i >= 0; i--)
        //  {
        //      GameObject rewardItem = Instantiate(gameItem, contentObj.transform);
        //      rewardItem.gameObject.SetActive(true);
        //      var mono = rewardItem.gameObject.AddComponent<UIPopupGoldRewardItem>();
        //      mono.SetData(rvRewards[i], i);
        //      
        //      _rewardItems.Add(mono);
        //      
        //      if(GoldCollectionModule.Instance.GetCurConfigIndex() > 1)
        //          sliderLength += 175+8;
        //  }

        //allLength -= 50;
        //AnchorPosY(contentObj.transform as RectTransform, allLength);

        uiSliderImage.rectTransform.SetHeight(sliderLength);

        _buttonClose = GetItem<Button>("Root/UIBg/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
    }

    private void OnBtnClose()
    {
        CloseWindowWithinUIMgr(true);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
    }

    private void OnDestroy()
    {
    }

    public RectTransform AnchorPosY(RectTransform selfRectTrans, float anchorPosY)
    {
        var anchorPos = selfRectTrans.anchoredPosition;
        anchorPos.y = anchorPosY;
        selfRectTrans.anchoredPosition = anchorPos;
        return selfRectTrans;
    }
}