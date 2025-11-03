using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using MagneticScrollView;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFishCultureMainController
{
    private bool InitShopEntranceFlag = false;
    public ShopEntrance ShopGroup;
    public void InitShopEntrance()
    {
        if (InitShopEntranceFlag)
            return;
        InitShopEntranceFlag = true;
        ShopGroup = transform.Find("Root/ButtonBuyFish").gameObject.AddComponent<ShopEntrance>();
        ShopGroup.Init(Storage,this);
        // if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.FishCultureGuideShopEntrance))
        // {
        //     // List<Transform> topLayer = new List<Transform>();
        //     // topLayer.Add(ShopGroup.transform);
        //     // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.FishCultureGuideShopEntrance, ShopGroup.transform as RectTransform,
        //     //     topLayer: topLayer);
        //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.FishCultureGuideShopEntrance,
        //         ShopGroup.transform as RectTransform);
        // }
    }

    public void GuideFishShop()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.FishCultureStart))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(ShopGroup.ShopBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.FishCultureStart, ShopGroup.ShopBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.FishCultureStart, null))
            {
                
            }
        }
    }

    public class ShopEntrance : MonoBehaviour
    {
        public Button ShopBtn;
        private LocalizeTextMeshProUGUI NumText;
        private FishCultureRedPoint RedPoint;
        private Transform FishTrans;
        private FishCultureRewardConfig FishConfig;
        private Transform FishNode;
        
        private void Awake()
        {
            ShopBtn = transform.GetComponent<Button>();
            ShopBtn.onClick.AddListener(OnClickShopBtn);
            NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            RedPoint = transform.Find("RedPoint").gameObject.AddComponent<FishCultureRedPoint>();
            transform.Find("RedPoint").gameObject.SetActive(false);
            var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/FishCulture/FishViewNode");
            FishNode = Instantiate(asset).transform;
        }
        

        private StorageFishCulture Storage;
        private UIFishCultureMainController MainUI;
        public void Init(StorageFishCulture storage,UIFishCultureMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            // RedPoint.Init(Storage);
            FishConfig = FishCultureModel.Instance.GetNextFish();
            RedPoint.Init(Storage);
            UpdateUI();
            EventDispatcher.Instance.AddEvent<EventFishCultureGetNewFish>(OnGetNewFish);
        }

        public void OnGetNewFish(EventFishCultureGetNewFish evt)
        {
            FishConfig = FishCultureModel.Instance.GetNextFish();
            UpdateUI();
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventFishCultureGetNewFish>(OnGetNewFish);
            Destroy(FishNode.gameObject);
        }

        public void UpdateUI()
        {
            if (FishConfig == null)
            {
                gameObject.SetActive(false);
                return;
            }
            NumText.SetText(FishConfig.Price.ToString());
            RedPoint.UpdateUI();
            if (FishTrans)
                Destroy(FishTrans.gameObject);
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(FishConfig.GetFishAssetPath());
            FishTrans = Instantiate(asset, FishNode).transform;
            
        }

        public void OnClickShopBtn()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FishCultureStart);
            if (MainUI.IsPlaying())
                return;
            if (Storage.CurScore >= FishConfig.Price)
            {
                UIPopupFishCultureShopBuyController.Open(FishConfig, Storage);
            }
            else
            {
                UIPopupFishCultureNoDiceController.Open(Storage);
            }
        }
    }
}