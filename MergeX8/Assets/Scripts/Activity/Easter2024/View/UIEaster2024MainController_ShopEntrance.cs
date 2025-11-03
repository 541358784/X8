using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using MagneticScrollView;
using UnityEngine;
using UnityEngine.UI;

public partial class UIEaster2024MainController
{
    private bool InitShopEntranceFlag = false;
    public ShopEntrance ShopGroup;
    public void InitShopEntrance()
    {
        if (InitShopEntranceFlag)
            return;
        InitShopEntranceFlag = true;
        ShopGroup = transform.Find("Root/ButtonShop").gameObject.AddComponent<ShopEntrance>();
        ShopGroup.Init(Storage,this);
    }

    public class ShopEntrance : MonoBehaviour
    {
        private Button ShopBtn;
        private LocalizeTextMeshProUGUI NumText;
        private ShopEntranceRedPoint RedPoint;
        public Transform Icon;
        private void Awake()
        {
            ShopBtn = transform.GetComponent<Button>();
            ShopBtn.onClick.AddListener(OnClickShopBtn);
            NumText = transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>();
            RedPoint = transform.Find("RedPoint").gameObject.AddComponent<ShopEntranceRedPoint>();
            Icon = transform.Find("Icon");
        }

        private StorageEaster2024 Storage;
        private UIEaster2024MainController MainUI;
        private int ShowScore;
        public void Init(StorageEaster2024 storage,UIEaster2024MainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            RedPoint.Init(Storage);
            ShowScore = Storage.Score;
            UpdateUI();
            EventDispatcher.Instance.AddEvent<EventEaster2024ScoreChange>(OnScoreChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventEaster2024ScoreChange>(OnScoreChange);
        }

        private List<int> WaitAddValueList = new List<int>();
        public void OnScoreChange(EventEaster2024ScoreChange evt)
        {
            if (evt.NeedWait)
            {
                WaitAddValueList.Add(evt.ChangeValue);
            }
            else
            {
                ShowScore += evt.ChangeValue;
            }
            UpdateUI();
        }

        public void TriggerWaitAddValue()
        {
            if (WaitAddValueList.Count > 0)
            {
                var addValue = WaitAddValueList[0];
                WaitAddValueList.RemoveAt(0);
                var alReadyAddValue = 0;
                var addValueF = (float) addValue;
                DOTween.To(() => 0f, (v) =>
                {
                    var curV = (int) v;
                    if (curV != alReadyAddValue)
                    {
                        var distance = curV - alReadyAddValue;
                        alReadyAddValue = curV;
                        ShowScore += distance;
                        UpdateUI();
                    }
                }, addValueF, 0.5f).OnComplete(() =>
                {
                    if (addValue != alReadyAddValue)
                    {
                        var distance = addValue - alReadyAddValue;
                        alReadyAddValue = addValue;
                        ShowScore += distance;
                        UpdateUI();
                    }
                }).SetTarget(transform);
            }
        }

        public void UpdateUI()
        {
            NumText.SetText(ShowScore.ToString());
        }

        public void OnClickShopBtn()
        {
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024MainStore);
            if (MainUI.Game.IsPlaying())
                return;
            UIEaster2024ShopController.Open(Storage);
        }
    }

    public class ShopEntranceRedPoint : MonoBehaviour
    {
        private StorageEaster2024 Storage;
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventEaster2024BuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.AddEvent<EventEaster2024ScoreChange>(OnScoreChange);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventEaster2024BuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.RemoveEvent<EventEaster2024ScoreChange>(OnScoreChange);
        }

        public void OnBuyStoreItem(EventEaster2024BuyStoreItem evt)
        {
            UpdateUI();
        }
        public void OnScoreChange(EventEaster2024ScoreChange evt)
        {
            UpdateUI();
        }
        public void Init(StorageEaster2024 storage)
        {
            Storage = storage;
            UpdateUI();
        }

        public void UpdateUI()
        {
            var curStoreLevel = Storage.GetCurStoreLevel();
            var showState = false;
            for (var i = 0; i < curStoreLevel.StoreItemList.Count; i++)
            {
                var storeItem = Easter2024Model.Instance.StoreItemConfig[curStoreLevel.StoreItemList[i]];
                if (!Storage.FinishStoreItemList.Contains(storeItem.Id) && Storage.Score >= storeItem.Price)
                {
                    showState = true;
                    break;
                }
            }
            gameObject.SetActive(showState);
        }
    }
}