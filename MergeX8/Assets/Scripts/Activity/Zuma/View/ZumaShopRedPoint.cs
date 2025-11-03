using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class ZumaShopRedPoint : MonoBehaviour
{
    private StorageZuma Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventZumaBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.AddEvent<EventZumaScoreChange>(OnScoreChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventZumaBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.RemoveEvent<EventZumaScoreChange>(OnScoreChange);
    }

    public void OnBuyStoreItem(EventZumaBuyStoreItem evt)
    {
        UpdateUI();
    }
    public void OnScoreChange(EventZumaScoreChange evt)
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
        var curStoreLevel = Storage.GetCurStoreLevel();
        var count = 0;
        for (var i = 0; i < curStoreLevel.StoreItemList.Count; i++)
        {
            var storeItem = ZumaModel.Instance.StoreItemConfig[curStoreLevel.StoreItemList[i]];
            if (!Storage.FinishStoreItemList.Contains(storeItem.Id) && Storage.Score >= storeItem.Price)
            {
                count++;
            }
        }
        gameObject.SetActive(count > 0);
        NumText?.SetText(count.ToString());
    }
}