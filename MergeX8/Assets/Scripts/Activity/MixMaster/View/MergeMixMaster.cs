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

public class MergeMixMaster : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private MixMasterEntranceRedPoint RedPoint;
    
    private StorageMixMaster Storage;
    private void SetStorage(StorageMixMaster storage)
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
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<MixMasterEntranceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        SetStorage(MixMasterModel.Instance.Storage);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MixMasterEntrance);
        if (Storage.GetPreheatTime() > 0)
        {
            UIPopupMixMasterPreviewController.Open(Storage);
        }
        else
        {
            UIMixMasterMainController.Open(Storage);
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (Storage.GetPreheatTime() > 0)
        {
            _countDownTime.SetText(Storage.GetPreheatTimeText());
        }
        else
        {
            _countDownTime.SetText(Storage.GetLeftTimeText());
        }
    }
    private void OnDestroy()
    {
    }
}