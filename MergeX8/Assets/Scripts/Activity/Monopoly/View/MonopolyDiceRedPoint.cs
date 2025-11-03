using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class MonopolyDiceRedPoint : MonoBehaviour
{
    private StorageMonopoly Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventMonopolyDiceCountChange>(OnDiceCountChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventMonopolyDiceCountChange>(OnDiceCountChange);
    }

    public void OnDiceCountChange(EventMonopolyDiceCountChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageMonopoly storage)
    {
        Storage = storage;
        UpdateUI();
    }

    public void UpdateUI()
    {
        var count = Storage.DiceCount;
        gameObject.SetActive(count > 0);
        NumText?.SetText(count.ToString());
    }
}