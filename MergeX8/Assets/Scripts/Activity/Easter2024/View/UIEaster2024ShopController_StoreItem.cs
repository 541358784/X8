using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIEaster2024ShopController
{
    public class StoreItemLevel : MonoBehaviour
    {
        private Easter2024StoreLevelConfig StoreLevelConfig;
        private StorageEaster2024 Storage;
        Transform DefaultStoreItem;
        private LocalizeTextMeshProUGUI TitleText;
        private List<StoreItem> ItemList = new List<StoreItem>();

        private void Awake()
        {
            TitleText = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
            DefaultStoreItem = transform.Find("ItemGroup/Item");
            DefaultStoreItem.gameObject.SetActive(false);
            EventDispatcher.Instance.AddEvent<EventEaster2024BuyStoreItem>(OnBuyStoreItem);
        }
        public void OnBuyStoreItem(EventEaster2024BuyStoreItem evt)
        {
            // gameObject.SetActive( StoreLevelConfig.Id >= Storage.GetCurStoreLevel().Id);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventEaster2024BuyStoreItem>(OnBuyStoreItem);
        }

        public void InitStoreItemLevel(StorageEaster2024 storage,Easter2024StoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreLevelConfig = storeLevelConfig;
            for (var i = 0; i < ItemList.Count; i++)
            {
                Destroy(ItemList[i].gameObject);
            }
            ItemList.Clear();
            for (var i = 0; i < StoreLevelConfig.StoreItemList.Count; i++)
            {
                var itemConfig = Easter2024Model.Instance.StoreItemConfig[StoreLevelConfig.StoreItemList[i]];
                var storeItem = Instantiate(DefaultStoreItem, DefaultStoreItem.parent).gameObject
                    .AddComponent<StoreItem>();
                storeItem.gameObject.SetActive(true);
                storeItem.InitStoreItem(storage, itemConfig,StoreLevelConfig);
                ItemList.Add(storeItem);
            }
            TitleText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_EasterEggActive_Shop_Stage",StoreLevelConfig.Id.ToString()));
            LayoutRebuilder.ForceRebuildLayoutImmediate(DefaultStoreItem.parent as RectTransform);
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
        private Easter2024StoreLevelConfig StoreLevelConfig;
        private Easter2024StoreItemConfig StoreItemConfig;
        private StorageEaster2024 Storage;
        private List<ItemStateBase> ItemStateList = new List<ItemStateBase>();
        private ItemStateLock Lock;
        private ItemStateNormal Normal;
        private ItemStateNoMoney NoMoney;
        private ItemStateFinish Finish;
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
        public void InitStoreItem(StorageEaster2024 storage, Easter2024StoreItemConfig storeItemConfig,Easter2024StoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreItemConfig = storeItemConfig;
            StoreLevelConfig = storeLevelConfig;
            Lock = transform.Find("Lock").gameObject.AddComponent<ItemStateLock>();
            Lock.InitStoreItemState(Storage,StoreItemConfig,storeLevelConfig);
            ItemStateList.Add(Lock);
            Normal = transform.Find("Normal").gameObject.AddComponent<ItemStateNormal>();
            Normal.InitStoreItemState(Storage,StoreItemConfig,storeLevelConfig);
            ItemStateList.Add(Normal);
            NoMoney = transform.Find("NoMoney").gameObject.AddComponent<ItemStateNoMoney>();
            NoMoney.InitStoreItemState(Storage,StoreItemConfig,storeLevelConfig);
            ItemStateList.Add(NoMoney);
            Finish = transform.Find("Finish").gameObject.AddComponent<ItemStateFinish>();
            Finish.InitStoreItemState(Storage,StoreItemConfig,storeLevelConfig);
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

    public abstract class ItemStateBase:MonoBehaviour
    {
        public Easter2024StoreItemConfig StoreItemConfig;
        public Easter2024StoreLevelConfig StoreLevelConfig;
        public StorageEaster2024 Storage;
        private Image Icon;

        public virtual void Awake()
        {
            Icon = transform.Find("Icon").GetComponent<Image>();
        }

        public abstract bool ShouldShow();
        public void InitStoreItemState(StorageEaster2024 storage, Easter2024StoreItemConfig storeItemConfig,Easter2024StoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreItemConfig = storeItemConfig;
            StoreLevelConfig = storeLevelConfig;
        }

        public virtual void UpdateUI()
        {
            Awake();
            if ((Easter2024StoreItemType) StoreItemConfig.Type == Easter2024StoreItemType.BuildItem)
            {
                Icon.sprite =
                    ResourcesManager.Instance.GetSpriteVariant(AtlasName.Easter2024Atlas, StoreItemConfig.Image);
            }
            else
            {
                Icon.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[0]);
                var text = Icon.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                var count = StoreItemConfig.RewardNum[0];
                text.gameObject.SetActive(count > 1);
                text.SetText(count.ToString());
            }
            gameObject.SetActive(ShouldShow());
        }
    }

    public class ItemStateLock : ItemStateBase
    {
        private Animator Animator;
        public override void Awake()
        {
            base.Awake();
            Animator = transform.GetComponent<Animator>();
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

        public void PerformUnlock()
        {
            if (StoreLevelConfig.Id <= Storage.GetCurStoreLevel().Id && gameObject.activeSelf)
            {
                Animator.PlayAnimation("open", () =>
                {
                    gameObject.SetActive(false);
                });
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
            Text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            BuyBtn = transform.gameObject.GetComponent<Button>();
            BuyBtn.onClick.AddListener(OnClickBuyBtn);
        }

        public void OnClickBuyBtn()
        {
            UIPopupEaster2024ShopBuyController.Open(StoreItemConfig,Storage);
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
            Text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            BuyBtn = transform.gameObject.GetComponent<Button>();
            BuyBtn.onClick.AddListener(OnClickBuyBtn);
        }
        public void OnClickBuyBtn()
        {
            UIPopupEaster2024ShopBuyController.Open(StoreItemConfig,Storage);
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

    public class LeaderBoardLevel : MonoBehaviour
    {
        private StorageEaster2024 Storage;
        LocalizeTextMeshProUGUI PercentText;
        LocalizeTextMeshProUGUI PercentText2;
        private LocalizeTextMeshProUGUI TitleText;
        private void Awake()
        {
            PercentText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            TitleText = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
            PercentText2 = transform.Find("NumGroup/Text")?.GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void InitLeaderBoardLevel(StorageEaster2024 storage)
        {
            Storage = storage;
            UpdateUI();
            // Storage.LeaderBoardStorage.SortController().BindRankChangeAction(OnRankChange);
        }

        // private void OnDestroy()
        // {
        //     Storage.LeaderBoardStorage.SortController().UnBindRankChangeAction(OnRankChange);
        // }

        // public void OnRankChange(Easter2024LeaderBoardPlayer player)
        // {
        //     if (player.IsMe)
        //     {
        //         UpdateUI();
        //     }
        // }
        public void UpdateUI()
        {
            TitleText.SetTerm("UI_EasterEggActive_Ranking_Title");
            PercentText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_EasterEggActive_Shop_Ranking_Desc",Easter2024LeaderBoardModel.Instance.LeastEnterBoardScore.ToString()));
            if(PercentText2)
                PercentText2.SetText(Storage.TotalScore+"/"+Easter2024LeaderBoardModel.Instance.LeastEnterBoardScore);
            // if (Storage.LeaderBoardStorage.IsInitFromServer())
            // {
            //     PercentText.SetText("No."+Storage.LeaderBoardStorage.SortController().MyRank);   
            // }
            // else
            // {
            //     PercentText.SetText(Storage.LeaderBoardStorage.StarCount + "/" + Easter2024LeaderBoardModel.Instance.LeastEnterBoardScore);
            // }
        }
    }
}