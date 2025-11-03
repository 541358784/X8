
using DragonPlus;
using DragonU3DSDK.Storage;

public class Aux_KapiTile : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private KapiTileLifeRedPoint RedPoint;
    private StorageKapiTile Storage;
    public void SetStorage(StorageKapiTile storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<KapiTileLifeRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(KapiTileModel.Instance.Storage);
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
            _timeText.SetText(Storage.GetLeftTimeText());
            RedPoint.UpdateUI();
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KapiTileEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupKapiTilePreviewController.Open(Storage);
        }
        else
        {
            UIKapiTileMainController.Open(Storage);
            // if (Storage.IsStart)
            // {
            //     UIKapiTileMainController.Open(Storage);
            // }
            // else
            // {
            //     KapiTileModel.CanShowStart();
            // }
        }
    }
    private void OnDestroy()
    {
    }
}
