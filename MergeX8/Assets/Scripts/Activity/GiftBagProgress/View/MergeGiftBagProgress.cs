using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeGiftBagProgress : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    private GiftBagProgressRedPoint RedPoint;
    private StorageGiftBagProgress Storage;
    private Image Slider;
    private LocalizeTextMeshProUGUI SliderText;
    private Button BuyButton;
    
    private void SetStorage(StorageGiftBagProgress storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
        RefreshView();
    }

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<GiftBagProgressRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        Slider = transform.Find("Root/Slider").GetComponent<Image>();
        SliderText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        BuyButton = transform.Find("Root/Button").GetComponent<Button>();
        BuyButton.onClick.AddListener(() =>
        {
            if (Storage.BuyState)
                return;
            StoreModel.Instance.Purchase(GiftBagProgressModel.Instance.GlobalConfig.ShopId);
        });
        BuyButton.gameObject.SetActive(false);
        
        SetStorage(GiftBagProgressModel.Instance.Storage);
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GiftBagProgressInfo);
        UIPopupGiftBagProgressTaskController.Open(Storage);
    }
    
    public void RefreshView()
    {
        if (Storage == null)
            return;
        gameObject.SetActive(Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
        var totalTaskCount = GiftBagProgressModel.Instance.DailyTaskConfig.Count;
        var completeCount = Storage.CanCollectLevels.Count;
        var collectCount = Storage.AlreadyCollectLevels.Count;
        var progress = (float) (completeCount + collectCount) / totalTaskCount;
        Slider.fillAmount = progress;
        SliderText.SetText((completeCount + collectCount)+"/"+totalTaskCount);
        BuyButton.gameObject.SetActive(!Storage.BuyState);
    }
    private void RefreshCountDown()
    {
        _countDownTime.SetText(Storage.GetLeftTimeText());
        RedPoint.UpdateUI();
    }

    private void OnDestroy()
    {
    }
}