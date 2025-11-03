
using DragonPlus;
using DragonU3DSDK.Storage;

public class Aux_KapiScrew : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private KapiScrewLifeRedPoint RedPoint;
    private StorageKapiScrew Storage;
    public void SetStorage(StorageKapiScrew storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<KapiScrewLifeRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);
        
        SetStorage(KapiScrewModel.Instance.Storage);
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
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KapiScrewEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupKapiScrewPreviewController.Open(Storage);
        }
        else
        {
            UIKapiScrewMainController.Open(Storage);
            // if (Storage.IsStart)
            // {
            //     UIKapiScrewMainController.Open(Storage);
            // }
            // else
            // {
            //     KapiScrewModel.CanShowStart();
            // }
        }
    }
    private void OnDestroy()
    {
    }
}
