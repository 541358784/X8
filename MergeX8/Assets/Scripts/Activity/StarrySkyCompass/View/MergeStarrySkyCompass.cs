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

public class MergeStarrySkyCompass : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private StarrySkyCompassEntranceRedPoint RedPoint;
    private LocalizeTextMeshProUGUI NumText;
    
    private StorageStarrySkyCompass Storage;
    private void SetStorage(StorageStarrySkyCompass storage)
    {
        Storage = storage;
        RedPoint.Init(Storage,true,true);
        RefreshView();
    }

    private void Awake()
    {
        NumText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<StarrySkyCompassEntranceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        SetStorage(StarrySkyCompassModel.Instance.Storage);
    }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StarrySkyCompassEntrance);
        if (Storage.GetPreheatTime() > 0)
        {
            UIPopupStarrySkyCompassPreviewController.Open(Storage);
        }
        else
        {
            UIStarrySkyCompassMainController.Open(Storage);
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
        if (!IsLockNum)
            NumText.SetText("x"+Storage.RocketCount);
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

    private bool IsLockNum = false;
    public void LockNum()
    {
        IsLockNum = true;
    }
    public void SetText(int num)
    {
        IsLockNum = false;
        NumText.SetText("x"+Storage.RocketCount);
    }
    private void OnDestroy()
    {
    }
}