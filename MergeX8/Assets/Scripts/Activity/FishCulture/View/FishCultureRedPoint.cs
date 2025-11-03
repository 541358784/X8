using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class FishCultureRedPoint : MonoBehaviour
{
    private StorageFishCulture Storage => FishCultureModel.Instance.CurStorageFishCultureWeek;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        NumText.gameObject.SetActive(false);
        EventDispatcher.Instance.AddEvent<EventFishCultureScoreChange>(OnDiceCountChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventFishCultureScoreChange>(OnDiceCountChange);
    }

    public void OnDiceCountChange(EventFishCultureScoreChange evt)
    {
        UpdateUI();
    }
    public void Init(StorageFishCulture storage)
    {
        // Storage = storage;
        UpdateUI();
    }

    public void UpdateUI()
    {
        var count = Storage.CurScore;
        var nextFish = FishCultureModel.Instance.GetNextFish();
        if (nextFish == null)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(count >= nextFish.Price);
        }
        // NumText?.SetText(count.ToString());
    }
}