using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class KapiScrewLifeRedPoint : MonoBehaviour
{
    private StorageKapiScrew Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventKapiScrewLifeChange>(OnLifeChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKapiScrewLifeChange>(OnLifeChange);
    }

    public void OnLifeChange(EventKapiScrewLifeChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageKapiScrew storage)
    {
        Storage = storage;
        UpdateUI();
    }

    public void UpdateUI()
    {
        var count = Storage.Life;
        gameObject.SetActive(count > 0);
        NumText?.SetText(count.ToString());
    }
}