using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class KapiTileLifeRedPoint : MonoBehaviour
{
    private StorageKapiTile Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventKapiTileLifeChange>(OnLifeChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKapiTileLifeChange>(OnLifeChange);
    }

    public void OnLifeChange(EventKapiTileLifeChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageKapiTile storage)
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