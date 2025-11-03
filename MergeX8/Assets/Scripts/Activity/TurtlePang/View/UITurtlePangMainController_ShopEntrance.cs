using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangMainController
{
    public Button ShopBtn;
    public TurtlePangShopRedPoint ShopRedPoint;
    public LocalizeTextMeshProUGUI ScoreText;
    public Image ScoreIcon;
    public void InitShopEntrance()
    {
        ShopBtn = GetItem<Button>("Root/ButtonShop");
        ShopBtn.onClick.AddListener(() =>
        {
            UITurtlePangShopController.Open(Storage);
        });
        ShopRedPoint = transform.Find("Root/ButtonShop/RedPoint").gameObject.AddComponent<TurtlePangShopRedPoint>();
        ShopRedPoint.gameObject.SetActive(true);
        ShopRedPoint.Init(Storage);
        ScoreText = transform.Find("Root/ButtonShop/NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        ScoreText.SetText(Storage.Score.ToString());
        EventDispatcher.Instance.AddEvent<EventTurtlePangScoreChange>(OnScoreChange);
        ScoreIcon = transform.Find("Root/ButtonShop/NumGroup/Image").GetComponent<Image>();
    }

    public void OnScoreChange(EventTurtlePangScoreChange evt)
    {
        ScoreText.SetText(Storage.Score.ToString());
    }
    private void OnDestroy()
    {
        AudioManager.Instance.PlayMusic(1, true);
        EventDispatcher.Instance.RemoveEvent<EventTurtlePangScoreChange>(OnScoreChange);
    }
    public class TurtlePangShopRedPoint:MonoBehaviour
    {
        private StorageTurtlePang Storage;
        private LocalizeTextMeshProUGUI NumText;
        private void Awake()
        {
            NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEvent<EventTurtlePangBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.AddEvent<EventTurtlePangScoreChange>(OnScoreChange);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventTurtlePangBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.RemoveEvent<EventTurtlePangScoreChange>(OnScoreChange);
        }
    
        public void OnBuyStoreItem(EventTurtlePangBuyStoreItem evt)
        {
            UpdateUI();
        }
        public void OnScoreChange(EventTurtlePangScoreChange evt)
        {
            UpdateUI();
        }
        public void Init(StorageTurtlePang storage)
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
                var storeItem = TurtlePangModel.Instance.StoreItemConfig[curStoreLevel.StoreItemList[i]];
                if (!Storage.FinishStoreItemList.Contains(storeItem.Id) && Storage.Score >= storeItem.Price)
                {
                    count++;
                }
            }
            gameObject.SetActive(count > 0);
            NumText?.SetText(count.ToString());
        }
    }
}