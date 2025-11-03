
using DragonPlus;
using DragonU3DSDK.Storage;

public class Aux_Kapibala : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private KapibalaLifeRedPoint RedPoint;
    private StorageKapibala Storage;
    public void SetStorage(StorageKapibala storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<KapibalaLifeRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("UpdateUI", 0, 1);
        SetStorage(KapibalaModel.Instance.Storage);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage.ShowAuxItem());
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
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KapibalaEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupKapibalaPreviewController.Open(Storage);
        }
        else
        {
            UIKapibalaMainController.Open(Storage);
            // if (Storage.IsStart)
            // {
            //     UIKapibalaMainController.Open(Storage);
            // }
            // else
            // {
            //     KapibalaModel.CanShowStart();
            // }
        }
    }
    private void OnDestroy()
    {
    }
}
