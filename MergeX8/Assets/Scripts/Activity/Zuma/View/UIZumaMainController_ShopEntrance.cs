using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using MagneticScrollView;
using UnityEngine;
using UnityEngine.UI;

public partial class UIZumaMainController
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
        // if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaGuideShopEntrance))
        // {
        //     // List<Transform> topLayer = new List<Transform>();
        //     // topLayer.Add(ShopGroup.transform);
        //     // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaGuideShopEntrance, ShopGroup.transform as RectTransform,
        //     //     topLayer: topLayer);
        //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaGuideShopEntrance,
        //         ShopGroup.transform as RectTransform);
        // }
    }

    public class ShopEntrance : MonoBehaviour
    {
        private Button ShopBtn;
        private LocalizeTextMeshProUGUI NumText;
        private ZumaShopRedPoint RedPoint;
        public Transform Icon;
        private void Awake()
        {
            ShopBtn = transform.GetComponent<Button>();
            ShopBtn.onClick.AddListener(OnClickShopBtn);
            NumText = transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>();
            RedPoint = transform.Find("RedPoint").gameObject.AddComponent<ZumaShopRedPoint>();
            Icon = transform.Find("Icon");
        }

        private StorageZuma Storage;
        private UIZumaMainController MainUI;
        private int ShowScore;
        public void Init(StorageZuma storage,UIZumaMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            RedPoint.Init(Storage);
            ShowScore = Storage.Score;
            UpdateUI();
            EventDispatcher.Instance.AddEvent<EventZumaScoreChange>(OnScoreChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventZumaScoreChange>(OnScoreChange);
        }

        private List<int> WaitAddValueList = new List<int>();
        public void OnScoreChange(EventZumaScoreChange evt)
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
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ZumaMainStore);
            if (MainUI.IsPlaying())
                return;
            UIZumaShopController.Open(Storage);
        }
    }
}