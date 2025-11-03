using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStarrySkyCompassMainController
{
    public Button ShopBtn;
    public LocalizeTextMeshProUGUI ScoreNumText;
    public StarrySkyCompassShopRedPoint ShopRedPoint;
    public int ViewScore;
    public List<int> AddScoreList = new List<int>();
    public Transform StarIcon;
    public void InitShopEntrance()
    {
        ShopBtn = GetItem<Button>("Root/ButtonShop");
        ShopBtn.onClick.AddListener(() =>
        {
            UIStarrySkyCompassShopController.Open(Storage);
        });
        ShopRedPoint = transform.Find("Root/ButtonShop/RedPoint").gameObject.AddComponent<StarrySkyCompassShopRedPoint>();
        ShopRedPoint.gameObject.SetActive(true);
        ShopRedPoint.Init(Storage);
        ScoreNumText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonShop/NumText");
        StarIcon = transform.Find("Root/ButtonShop/Icon");
        ViewScore = Storage.Score;
        ScoreNumText.SetText(ViewScore.ToString());
        EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassScoreChange>(OnScoreChange);
        EventDispatcher.Instance.AddEvent<EventStarrySkyCompassScoreChange>(OnScoreChange);
    }
    public void OnScoreChange(EventStarrySkyCompassScoreChange evt)
    {
        if (evt.NeedWait)
        {
            AddScoreList.Add(evt.ChangeValue);
        }
        else
        {
            ViewScore += evt.ChangeValue;
            ScoreNumText.SetText(ViewScore.ToString());
        }
    }
    public void PerformAddScore(int addValue)
    {
        if (AddScoreList.Contains(addValue))
        {
            AddScoreList.Remove(addValue);
            ViewScore += addValue;
            ScoreNumText.SetText(ViewScore.ToString());
        }
    }
    
    public void FlyCarrot(Vector3 startPos,int addValue,Action callback = null)
    {
        Transform target = StarIcon;
        int count = addValue/5;
        if (count == 0)
            count = 1;
        if (count > 10)
            count = 10;
        float delayTime = 0.05f;
        var triggerAddValue = false;
        for (int i = 0; i < count; i++)
        {
            int index = i;

            Vector3 position = target.position;

            FlyGameObjectManager.Instance.FlyObject(target.gameObject, startPos, position, true, 0.5f,
                delayTime * i, () =>
                {
                    if (!triggerAddValue)
                    {
                        triggerAddValue = true;
                        PerformAddScore(addValue);
                    }
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                    ShakeManager.Instance.ShakeLight();
                    if (index == count - 1)
                    {
                        callback?.Invoke();
                    }
                });
        }
    }
    public class StarrySkyCompassShopRedPoint:MonoBehaviour
    {
        private StorageStarrySkyCompass Storage;
        private LocalizeTextMeshProUGUI NumText;
        private void Awake()
        {
            NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEvent<EventStarrySkyCompassBuyShopItem>(OnBuyStoreItem);
            EventDispatcher.Instance.AddEvent<EventStarrySkyCompassScoreChange>(OnScoreChange);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassBuyShopItem>(OnBuyStoreItem);
            EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassScoreChange>(OnScoreChange);
        }
    
        public void OnBuyStoreItem(EventStarrySkyCompassBuyShopItem evt)
        {
            UpdateUI();
        }
        public void OnScoreChange(EventStarrySkyCompassScoreChange evt)
        {
            UpdateUI();
        }
        public void Init(StorageStarrySkyCompass storage)
        {
            Storage = storage;
            UpdateUI();
        }

        private StarrySkyCompassModel Model => StarrySkyCompassModel.Instance;
        public void UpdateUI()
        {
            var count = 0;
            for (var i = 0; i < Model.ShopConfig.Count; i++)
            {
                var storeItem = Model.ShopConfig[i];
                if (Storage.Score >= storeItem.Price)
                {
                    count++;
                }
            }
            gameObject.SetActive(count > 0);
            NumText?.SetText(count.ToString());
        }
    }
}