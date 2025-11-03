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

public class MergeTurtlePang : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private TurtlePangEntranceRedPoint RedPoint;
    private LocalizeTextMeshProUGUI NumText;
    
    private StorageTurtlePang Storage;
    private void SetStorage(StorageTurtlePang storage)
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
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<TurtlePangEntranceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        NumText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        SetStorage(TurtlePangModel.Instance.Storage);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TurtlePangEntrance);
        if (Storage.GetPreheatTime() > 0)
        {
            UIPopupTurtlePangPreviewController.Open(Storage);
        }
        else
        {
            UITurtlePangMainController.Open(Storage);
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage!= null && Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
        if (!IsLockNum)
            NumText.SetText("x"+Storage.PackageCount);
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
        NumText.SetText("x"+Storage.PackageCount);
    }
    private void OnDestroy()
    {
    }
}