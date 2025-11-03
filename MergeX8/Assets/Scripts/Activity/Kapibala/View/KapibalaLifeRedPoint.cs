using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class KapibalaLifeRedPoint : MonoBehaviour
{
    private StorageKapibala Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventKapibalaLifeChange>(OnLifeChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKapibalaLifeChange>(OnLifeChange);
    }

    public void OnLifeChange(EventKapibalaLifeChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageKapibala storage)
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