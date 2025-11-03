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

public class MergeSlotMachine : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    private SlotMachineRedPoint RedPoint;
    
    private StorageSlotMachine Storage;
    private void SetStorage(StorageSlotMachine storage)
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
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<SlotMachineRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);

        SetStorage(SlotMachineModel.Instance.CurStorage);
    }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SlotMachineInfo);
        if (!Storage.ShowEntrance())
            return;
        UIPopupSlotMachineMainController.Open(Storage);
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowEntrance());
        if (!gameObject.activeSelf)
            return;
        
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        _countDownTime.SetText(SlotMachineModel.Instance.GetActivityLeftTimeString());
    }
    public void SetText(int oldValue)
    {
        
    }

    private void OnDestroy()
    {
    }
}