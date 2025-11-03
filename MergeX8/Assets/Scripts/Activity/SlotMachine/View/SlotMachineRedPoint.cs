using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class SlotMachineRedPoint : MonoBehaviour
{
    private StorageSlotMachine Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventSlotMachineScoreChange>(OnScoreChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventSlotMachineScoreChange>(OnScoreChange);
    }
    public void OnScoreChange(EventSlotMachineScoreChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageSlotMachine storage)
    {
        Storage = storage;
        UpdateUI();
    }

    public void UpdateUI()
    {
        var count = Storage.SpinCount;
        gameObject.SetActive(count > 0);
        if (NumText)
        {
            NumText.gameObject.SetActive(count > 0);
            NumText.SetText(count.ToString());
        }
    }
}