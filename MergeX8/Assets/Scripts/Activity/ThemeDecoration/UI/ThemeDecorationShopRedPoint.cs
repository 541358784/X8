using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class ThemeDecorationShopRedPoint : MonoBehaviour
{
    private StorageThemeDecoration Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.AddEvent<EventThemeDecorationScoreChange>(OnScoreChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.RemoveEvent<EventThemeDecorationScoreChange>(OnScoreChange);
    }

    public void OnBuyStoreItem(EventThemeDecorationBuyStoreItem evt)
    {
        UpdateUI();
    }
    public void OnScoreChange(EventThemeDecorationScoreChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageThemeDecoration storage)
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
            var storeItem = ThemeDecorationModel.Instance.StoreItemConfig[curStoreLevel.StoreItemList[i]];
            if (!Storage.FinishStoreItemList.Contains(storeItem.Id) && Storage.Score >= storeItem.Price)
            {
                count++;
            }
        }
        gameObject.SetActive(count > 0);
        NumText?.SetText(count.ToString());
    }
}