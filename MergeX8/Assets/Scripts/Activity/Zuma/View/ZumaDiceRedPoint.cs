using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class ZumaDiceRedPoint : MonoBehaviour
{
    private StorageZuma Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventZumaDiceCountChange>(OnDiceCountChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventZumaDiceCountChange>(OnDiceCountChange);
    }

    public void OnDiceCountChange(EventZumaDiceCountChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageZuma storage)
    {
        Storage = storage;
        UpdateUI();
    }

    public void UpdateUI()
    {
        var count = Storage.BallCount;
        gameObject.SetActive(count > 0);
        NumText?.SetText(count.ToString());
    }
}