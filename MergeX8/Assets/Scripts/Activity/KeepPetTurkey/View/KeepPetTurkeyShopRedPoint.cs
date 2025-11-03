using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class KeepPetTurkeyShopRedPoint : MonoBehaviour
{
    private StorageKeepPetTurkey Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventKeepPetTurkeyBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.AddEvent<EventKeepPetTurkeyScoreChange>(OnScoreChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetTurkeyBuyStoreItem>(OnBuyStoreItem);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetTurkeyScoreChange>(OnScoreChange);
    }

    public void OnBuyStoreItem(EventKeepPetTurkeyBuyStoreItem evt)
    {
        UpdateUI();
    }
    public void OnScoreChange(EventKeepPetTurkeyScoreChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageKeepPetTurkey storage)
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
            var storeItem = KeepPetTurkeyModel.Instance.StoreItemConfig[curStoreLevel.StoreItemList[i]];
            if (!Storage.FinishStoreItemList.Contains(storeItem.Id) && Storage.Score >= storeItem.Price)
            {
                count++;
            }
        }
        gameObject.SetActive(count > 0);
        NumText?.SetText(count.ToString());
    }
}