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

public class MergeZuma : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private ZumaDiceRedPoint RedPoint;
    private LocalizeTextMeshProUGUI NumText;
    
    private StorageZuma Storage;
    private void SetStorage(StorageZuma storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
        RefreshView();
    }

    private void Awake()
    {
        NumText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<ZumaDiceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        SetStorage(ZumaModel.Instance.CurStorageZumaWeek);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ZumaEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupZumaPreviewController.Open(Storage);
        }
        else
        {
            if (Storage.IsStart)
            {
                UIZumaMainController.Open(Storage);
            }
            else
            {
                ZumaModel.CanShowStartPopup();
            }
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
        if (!IsLockNum)
            NumText.SetText("x"+Storage.BallCount);
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (Storage.GetPreheatLeftTime() > 0)
        {
            _countDownTime.SetText(Storage.GetPreheatLeftTimeText());
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
        NumText.SetText("x"+Storage.BallCount);
    }
    private void OnDestroy()
    {
    }
}