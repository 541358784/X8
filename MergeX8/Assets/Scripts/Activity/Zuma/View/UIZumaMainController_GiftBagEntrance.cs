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
    private bool InitGiftBagEntranceFlag = false;
    public GiftBagEntrance GiftBagGroup;
    public void InitGiftBagEntrance()
    {
        if (InitGiftBagEntranceFlag)
            return;
        InitShopEntranceFlag = true;
        GiftBagGroup = transform.Find("Root/ButtonGift").gameObject.AddComponent<GiftBagEntrance>();
        GiftBagGroup.Init(Storage,this);
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

    public class GiftBagEntrance : MonoBehaviour
    {
        private Button Btn;
        // private LocalizeTextMeshProUGUI NumText;
        // private ZumaShopRedPoint RedPoint;
        // public Transform Icon;
        private void Awake()
        {
            Btn = transform.GetComponent<Button>();
            Btn.onClick.AddListener(OnClickShopBtn);
            // NumText = transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>();
            // RedPoint = transform.Find("RedPoint").gameObject.AddComponent<ZumaShopRedPoint>();
            // Icon = transform.Find("Icon");
            transform.Find("RedPoint").gameObject.SetActive(false);
        }

        private StorageZuma Storage;
        private UIZumaMainController MainUI;
        // private int ShowScore;
        public void Init(StorageZuma storage,UIZumaMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            // RedPoint.Init(Storage);
            // ShowScore = Storage.Score;
        }

       public void OnClickShopBtn()
        {
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ZumaMainStore);
            if (MainUI.IsPlaying())
                return;
            UIZumaGiftBagController.Open();
        }
    }
}