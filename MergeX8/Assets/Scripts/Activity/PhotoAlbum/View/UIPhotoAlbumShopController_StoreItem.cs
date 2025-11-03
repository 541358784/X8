using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPhotoAlbumShopController
{
    public class StoreItemLevel : MonoBehaviour
    {
        public PhotoAlbumStoreLevelConfig StoreLevelConfig;

        public StoragePhotoAlbum Storage;

        // Transform DefaultStoreItem;
        // private LocalizeTextMeshProUGUI TitleText;
        private List<StoreItem> ItemList = new List<StoreItem>();

        private void Awake()
        {
            // TitleText = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
            // DefaultStoreItem = transform.Find("ItemGroup/Item");
            // DefaultStoreItem.gameObject.SetActive(false);
            EventDispatcher.Instance.AddEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
        }

        public void OnBuyStoreItem(EventPhotoAlbumBuyStoreItem evt)
        {
            // gameObject.SetActive( StoreLevelConfig.Id >= Storage.GetCurStoreLevel().Id);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
        }

        public void InitStoreItemLevel(StoragePhotoAlbum storage, PhotoAlbumStoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreLevelConfig = storeLevelConfig;
            for (var i = 0; i < ItemList.Count; i++)
            {
                Destroy(ItemList[i].gameObject);
            }

            ItemList.Clear();
            // for (var i = 0; i < StoreLevelConfig.StoreItemList.Count; i++)
            // {
            var itemConfig = PhotoAlbumModel.Instance.StoreItemConfig[StoreLevelConfig.StoreItemList[0]];
            var storeItem = gameObject.AddComponent<StoreItem>();
            // var storeItem = Instantiate(DefaultStoreItem, DefaultStoreItem.parent).gameObject
            //     .AddComponent<StoreItem>();
            storeItem.gameObject.SetActive(true);
            storeItem.InitStoreItem(storage, itemConfig, StoreLevelConfig);
            ItemList.Add(storeItem);
            // }
            // TitleText.SetTermFormats(StoreLevelConfig.Id.ToString());
            // LayoutRebuilder.ForceRebuildLayoutImmediate(DefaultStoreItem.parent as RectTransform);
        }

        public async void PerformUnlock(Action callback)
        {
            for (var i = 0; i < ItemList.Count; i++)
            {
                ItemList[i].PerformUnlock();
                await XUtility.WaitSeconds(1f);
                if (!this)
                    return;
            }

            callback?.Invoke();
        }
    }

    public class StoreItem : MonoBehaviour
    {
        private PhotoAlbumStoreLevelConfig StoreLevelConfig;
        private PhotoAlbumStoreItemConfig StoreItemConfig;
        private StoragePhotoAlbum Storage;
        private List<ItemStateBase> ItemStateList = new List<ItemStateBase>();
        private ItemStateLock Lock;
        private ItemStateNormal Normal;
        private ItemStateNoMoney NoMoney;
        private ItemStateFinish Finish;

        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.AddEvent<EventPhotoAlbumScoreChange>(OnScoreChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.RemoveEvent<EventPhotoAlbumScoreChange>(OnScoreChange);
        }

        public void OnBuyStoreItem(EventPhotoAlbumBuyStoreItem evt)
        {
            UpdateUI();
        }

        public void OnScoreChange(EventPhotoAlbumScoreChange evt)
        {
            UpdateUI();
        }

        public void InitStoreItem(StoragePhotoAlbum storage, PhotoAlbumStoreItemConfig storeItemConfig,
            PhotoAlbumStoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreItemConfig = storeItemConfig;
            StoreLevelConfig = storeLevelConfig;
            Lock = transform.Find("Lock").gameObject.AddComponent<ItemStateLock>();
            Lock.InitStoreItemState(Storage, StoreItemConfig, storeLevelConfig);
            ItemStateList.Add(Lock);
            Normal = transform.Find("Normal").gameObject.AddComponent<ItemStateNormal>();
            Normal.InitStoreItemState(Storage, StoreItemConfig, storeLevelConfig);
            ItemStateList.Add(Normal);
            NoMoney = transform.Find("NoMoney").gameObject.AddComponent<ItemStateNoMoney>();
            NoMoney.InitStoreItemState(Storage, StoreItemConfig, storeLevelConfig);
            ItemStateList.Add(NoMoney);
            Finish = transform.Find("Finish").gameObject.AddComponent<ItemStateFinish>();
            Finish.InitStoreItemState(Storage, StoreItemConfig, storeLevelConfig);
            ItemStateList.Add(Finish);
            UpdateUI();
        }

        public void UpdateUI()
        {
            for (var i = 0; i < ItemStateList.Count; i++)
            {
                ItemStateList[i].UpdateUI();
            }
        }

        public void PerformUnlock()
        {
            Lock.PerformUnlock();
        }
    }

    public abstract class ItemStateBase : MonoBehaviour
    {
        public PhotoAlbumStoreItemConfig StoreItemConfig;
        public PhotoAlbumStoreLevelConfig StoreLevelConfig;
        public StoragePhotoAlbum Storage;
        private Image Icon;
        private LocalizeTextMeshProUGUI IconText;
        private Image Icon2;
        private LocalizeTextMeshProUGUI IconText2;
        private Image Icon3;
        private LocalizeTextMeshProUGUI IconText3;

        public virtual void Awake()
        {
            Icon = transform.Find("Item/Icon")?.GetComponent<Image>();
            IconText = transform.Find("Item/Text")?.GetComponent<LocalizeTextMeshProUGUI>();
            
            Icon2 = transform.Find("Icon2")?.GetComponent<Image>();
            IconText2 = transform.Find("Icon2/Text")?.GetComponent<LocalizeTextMeshProUGUI>();
            Icon2?.gameObject.SetActive(false);
            Icon3 = transform.Find("Icon3")?.GetComponent<Image>();
            IconText3 = transform.Find("Icon3/Text")?.GetComponent<LocalizeTextMeshProUGUI>();
            Icon3?.gameObject.SetActive(false);
        }

        public abstract bool ShouldShow();

        public void InitStoreItemState(StoragePhotoAlbum storage, PhotoAlbumStoreItemConfig storeItemConfig,
            PhotoAlbumStoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreItemConfig = storeItemConfig;
            StoreLevelConfig = storeLevelConfig;
        }

        public virtual void UpdateUI()
        {
            if ((PhotoAlbumStoreItemType)StoreItemConfig.Type == PhotoAlbumStoreItemType.BuildItem)
            {
                if (Icon)
                    Icon.sprite =
                        ResourcesManager.Instance.GetSpriteVariant("PhotoAlbumAtlas",
                            "PhotoAlbumMain14");
                if (IconText)
                    IconText.gameObject.SetActive(false);
            }
            else
            {
                if (Icon)
                    Icon.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[0], UserData.ResourceSubType.Big);
                if (IconText)
                {
                    if (StoreItemConfig.RewardNum[0] == 1)
                    {
                        IconText.gameObject.SetActive(false);
                    }
                    else
                    {
                        IconText.gameObject.SetActive(true);
                        IconText.SetText(StoreItemConfig.RewardNum[0].ToString());
                    }
                }
                
                if (StoreItemConfig.RewardId.Count > 1 && Icon2 && IconText2)
                {
                    Icon2.gameObject.SetActive(true);
                    Icon2.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[1],UserData.ResourceSubType.Big);
                    if (StoreItemConfig.RewardNum[1] == 1)
                    {
                        IconText2.gameObject.SetActive(false);
                    }
                    else
                    {
                        IconText2.gameObject.SetActive(true);
                        IconText2.SetText(StoreItemConfig.RewardNum[1].ToString());   
                    }
                }
                if (StoreItemConfig.RewardId.Count > 2 && Icon3 && IconText3)
                {
                    Icon3.gameObject.SetActive(true);
                    Icon3.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[2],UserData.ResourceSubType.Big);
                    if (StoreItemConfig.RewardNum[2] == 1)
                    {
                        IconText3.gameObject.SetActive(false);
                    }
                    else
                    {
                        IconText3.gameObject.SetActive(true);
                        IconText3.SetText(StoreItemConfig.RewardNum[2].ToString());   
                    }
                }
            }

            gameObject.SetActive(ShouldShow());
        }
    }

    public class ItemStateLock : ItemStateBase
    {
        private Animator Animator;
        private LocalizeTextMeshProUGUI Text;

        public override void Awake()
        {
            base.Awake();
            Animator = transform.Find("Image").GetComponent<Animator>();
            Text = transform.Find("NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public override bool ShouldShow()
        {
            if (StoreLevelConfig.Id > Storage.GetCurStoreLevel().Id)
            {
                return true;
            }

            if (!Storage.UnLockStoreLevel.Contains(StoreLevelConfig.Id))
            {
                return true;
            }

            return false;
        }
        public override void UpdateUI()
        {
            base.UpdateUI();
            Text.SetText(StoreItemConfig.Price.ToString());
        }

        public void PerformUnlock()
        {
            if (StoreLevelConfig.Id <= Storage.GetCurStoreLevel().Id && gameObject.activeSelf)
            {
                Animator.PlayAnimation("open", () => { gameObject.SetActive(false); });
            }
        }
    }

    public class ItemStateNormal : ItemStateBase
    {
        private LocalizeTextMeshProUGUI Text;
        private Button BuyBtn;

        public override void Awake()
        {
            base.Awake();
            Text = transform.Find("NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            BuyBtn = transform.gameObject.GetComponent<Button>();
            BuyBtn.onClick.AddListener(OnClickBuyBtn);
        }

        public void OnClickBuyBtn()
        {
            UIPopupPhotoAlbumShopBuyController.Open(StoreItemConfig, Storage);
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            Text.SetText(StoreItemConfig.Price.ToString());
        }

        public override bool ShouldShow()
        {
            if (StoreLevelConfig.Id == Storage.GetCurStoreLevel().Id &&
                !Storage.FinishStoreItemList.Contains(StoreItemConfig.Id) &&
                Storage.Score >= StoreItemConfig.Price)
            {
                return true;
            }

            return false;
        }
    }

    public class ItemStateNoMoney : ItemStateBase
    {
        private LocalizeTextMeshProUGUI Text;
        private Button BuyBtn;

        public override void Awake()
        {
            base.Awake();
            Text = transform.Find("NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            BuyBtn = transform.gameObject.GetComponent<Button>();
            BuyBtn.onClick.AddListener(OnClickBuyBtn);
        }

        public void OnClickBuyBtn()
        {
            UIPopupPhotoAlbumShopBuyController.Open(StoreItemConfig, Storage);
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            Text.SetText(StoreItemConfig.Price.ToString());
        }

        public override bool ShouldShow()
        {
            if (StoreLevelConfig.Id == Storage.GetCurStoreLevel().Id &&
                !Storage.FinishStoreItemList.Contains(StoreItemConfig.Id) &&
                Storage.Score < StoreItemConfig.Price)
            {
                return true;
            }

            return false;
        }
    }

    public class ItemStateFinish : ItemStateBase
    {
        public override bool ShouldShow()
        {
            if (Storage.FinishStoreItemList.Contains(StoreItemConfig.Id))
            {
                return true;
            }

            return false;
        }
    }

    // public class LeaderBoardLevel : MonoBehaviour
    // {
    //     private StoragePhotoAlbum Storage;
    //     private PhotoAlbumStoreLevelConfig StoreLevelConfig;
    //     private StoreItem FinalStoreItem;
    //     public void InitLeaderBoardLevel(StoragePhotoAlbum storage,PhotoAlbumStoreLevelConfig storeLevelConfig)
    //     {
    //         StoreLevelConfig = storeLevelConfig;
    //         Storage = storage;
    //         var itemConfig = PhotoAlbumModel.Instance.StoreItemConfig[StoreLevelConfig.StoreItemList[0]];
    //         var storeItem = gameObject.AddComponent<StoreItem>();
    //         storeItem.gameObject.SetActive(true);
    //         storeItem.InitStoreItem(storage, itemConfig,StoreLevelConfig); 
    //         UpdateUI();
    //     }
    //     public void UpdateUI()
    //     {
    //     }
    //     private void Awake()
    //     {
    //         EventDispatcher.Instance.AddEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
    //     }
    //     public void OnBuyStoreItem(EventPhotoAlbumBuyStoreItem evt)
    //     {
    //         
    //     }
    //
    //     private void OnDestroy()
    //     {
    //         EventDispatcher.Instance.RemoveEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
    //     }
    // }
}