using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class GiftBagProgressRedPoint : MonoBehaviour
{
    private StorageGiftBagProgress Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventGiftBagProgressBuyStateChange>(OnBuyStateChange);
        EventDispatcher.Instance.AddEvent<EventGiftBagProgressCollectTask>(OnCollectTask);
        EventDispatcher.Instance.AddEvent<EventGiftBagProgressCompleteTask>(OnCompleteTask);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventGiftBagProgressBuyStateChange>(OnBuyStateChange);
        EventDispatcher.Instance.RemoveEvent<EventGiftBagProgressCollectTask>(OnCollectTask);
        EventDispatcher.Instance.RemoveEvent<EventGiftBagProgressCompleteTask>(OnCompleteTask);
    }

    public void OnBuyStateChange(EventGiftBagProgressBuyStateChange evt)
    {
        UpdateUI();
    }
    public void OnCollectTask(EventGiftBagProgressCollectTask evt)
    {
        UpdateUI();
    }
    public void OnCompleteTask(EventGiftBagProgressCompleteTask evt)
    {
        UpdateUI();
    }
    public void Init(StorageGiftBagProgress storage)
    {
        Storage = storage;
        UpdateUI();
    }

    public void UpdateUI()
    {
        var buyState = Storage.BuyState;
        if (buyState)
        {
            gameObject.SetActive(Storage.CanCollectLevels.Count > 0);
            NumText.gameObject.SetActive(true);
            NumText.SetText(Storage.CanCollectLevels.Count.ToString());
        }
        else
        {
            gameObject.SetActive(Storage.CanCollectLevels.Count >= GiftBagProgressModel.Instance.GlobalConfig.TaskCompleteShowCount);
            NumText.gameObject.SetActive(false);
        }
    }
}