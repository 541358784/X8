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

public class MergePhotoAlbum : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private PhotoAlbumRedPoint RedPoint;
    private LocalizeTextMeshProUGUI NumText;
    
    private StoragePhotoAlbum Storage;
    private void SetStorage(StoragePhotoAlbum storage)
    {
        Storage = storage;
        RedPoint?.Init(Storage);
        RefreshView();
    }

    private void Awake()
    {
        NumText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("Root/RedPoint")?.gameObject.AddComponent<PhotoAlbumRedPoint>();
        RedPoint?.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        SetStorage(PhotoAlbumModel.Instance.Storage);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PhotoAlbumEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupPhotoAlbumPreviewController.Open(Storage);
        }
        else
        {
            UIPhotoAlbumShopController.Open(Storage);
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
        if (!IsLockNum)
            NumText.SetText("x"+Storage.Score);
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
            _countDownTime.SetText(Storage.GetLeftPreEndTimeText());
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
        NumText.SetText("x"+Storage.Score);
    }
    private void OnDestroy()
    {
    }
}