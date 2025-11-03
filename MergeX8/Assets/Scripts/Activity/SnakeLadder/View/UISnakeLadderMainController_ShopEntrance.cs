using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using MagneticScrollView;
using UnityEngine;
using UnityEngine.UI;

public partial class UISnakeLadderMainController
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
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderGuideShopEntrance))
        {
            // List<Transform> topLayer = new List<Transform>();
            // topLayer.Add(ShopGroup.transform);
            // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderGuideShopEntrance, ShopGroup.transform as RectTransform,
            //     topLayer: topLayer);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderGuideShopEntrance,
                ShopGroup.transform as RectTransform);
        }
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

        private StorageSnakeLadder Storage;
        private UISnakeLadderMainController MainUI;
        private int ShowScore;
        public void Init(StorageSnakeLadder storage,UISnakeLadderMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            RedPoint.Init(Storage);
            ShowScore = Storage.Score;
            UpdateUI();
            EventDispatcher.Instance.AddEvent<EventSnakeLadderScoreChange>(OnScoreChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderScoreChange>(OnScoreChange);
        }

        private List<int> WaitAddValueList = new List<int>();
        public void OnScoreChange(EventSnakeLadderScoreChange evt)
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
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SnakeLadderMainStore);
            if (MainUI.IsPlaying())
                return;
            UISnakeLadderShopController.Open(Storage);
        }
    }

    public class ShopEntranceRedPoint : MonoBehaviour
    {
        private StorageSnakeLadder Storage;
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventSnakeLadderBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderScoreChange>(OnScoreChange);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderScoreChange>(OnScoreChange);
        }

        public void OnBuyStoreItem(EventSnakeLadderBuyStoreItem evt)
        {
            UpdateUI();
        }
        public void OnScoreChange(EventSnakeLadderScoreChange evt)
        {
            UpdateUI();
        }
        public void Init(StorageSnakeLadder storage)
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
                var storeItem = SnakeLadderModel.Instance.StoreItemConfig[curStoreLevel.StoreItemList[i]];
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