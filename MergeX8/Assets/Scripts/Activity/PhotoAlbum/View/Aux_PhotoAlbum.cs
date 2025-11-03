using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_PhotoAlbum : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private PhotoAlbumRedPoint RedPoint;
    private StoragePhotoAlbum Storage;
    public void SetStorage(StoragePhotoAlbum storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<PhotoAlbumRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(PhotoAlbumModel.Instance.Storage);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage != null && Storage.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        if (Storage.GetPreheatLeftTime() > 0)
        {
            _timeText.SetText(Storage.GetPreheatLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
        else
        {
            _timeText.SetText(Storage.GetLeftPreEndTimeText());
            RedPoint.UpdateUI();
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
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
    private void OnDestroy()
    {
    }
}
