
using DragonPlus;
using DragonU3DSDK.Storage;

public class Aux_ChristmasBlindBox : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private StorageChristmasBlindBox Storage;
    public void SetStorage(StorageChristmasBlindBox storage)
    {
        Storage = storage;
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        transform.Find("RedPoint").gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(ChristmasBlindBoxModel.Instance.Storage);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(Storage != null && Storage.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(Storage.GetLeftTimeText());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIPopupChristmasBlindBoxController.Open();
    }
    private void OnDestroy()
    {
    }
}
