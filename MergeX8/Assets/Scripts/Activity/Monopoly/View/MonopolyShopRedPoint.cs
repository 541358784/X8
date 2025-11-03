using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class MonopolyShopRedPoint : MonoBehaviour
{
    private StorageMonopoly Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventMonopolyBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.AddEvent<EventMonopolyScoreChange>(OnScoreChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventMonopolyBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.RemoveEvent<EventMonopolyScoreChange>(OnScoreChange);
    }

    public void OnBuyStoreItem(EventMonopolyBuyStoreItem evt)
    {
        UpdateUI();
    }
    public void OnScoreChange(EventMonopolyScoreChange evt)
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
        var curStoreLevel = Storage.GetCurStoreLevel();
        var count = 0;
        for (var i = 0; i < curStoreLevel.StoreItemList.Count; i++)
        {
            var storeItem = MonopolyModel.Instance.StoreItemConfig[curStoreLevel.StoreItemList[i]];
            if (!Storage.FinishStoreItemList.Contains(storeItem.Id) && Storage.Score >= storeItem.Price)
            {
                count++;
            }
        }
        gameObject.SetActive(count > 0);
        NumText?.SetText(count.ToString());
    }
}