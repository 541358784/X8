using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangShopController
{
    public class StoreItemLevel : MonoBehaviour
    {
        private TurtlePangStoreLevelConfig StoreLevelConfig;
        private StorageTurtlePang Storage;
        Transform DefaultStoreItem;
        private LocalizeTextMeshProUGUI TitleText;
        private List<StoreItem> ItemList = new List<StoreItem>();

        private void Awake()
        {
            TitleText = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
            DefaultStoreItem = transform.Find("ItemGroup/Item");
            DefaultStoreItem.gameObject.SetActive(false);
            EventDispatcher.Instance.AddEvent<EventTurtlePangBuyStoreItem>(OnBuyStoreItem);
        }
        public void OnBuyStoreItem(EventTurtlePangBuyStoreItem evt)
        {
            // gameObject.SetActive( StoreLevelConfig.Id >= Storage.GetCurStoreLevel().Id);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventTurtlePangBuyStoreItem>(OnBuyStoreItem);
        }

        public void InitStoreItemLevel(StorageTurtlePang storage,TurtlePangStoreLevelConfig storeLevelConfig)
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
                var itemConfig = TurtlePangModel.Instance.StoreItemConfig[StoreLevelConfig.StoreItemList[i]];
                var storeItem = Instantiate(DefaultStoreItem, DefaultStoreItem.parent).gameObject
                    .AddComponent<StoreItem>();
                storeItem.gameObject.SetActive(true);
                storeItem.InitStoreItem(storage, itemConfig,StoreLevelConfig);
                ItemList.Add(storeItem);
            }
            TitleText.SetTermFormats(StoreLevelConfig.Id.ToString());
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
        private TurtlePangStoreLevelConfig StoreLevelConfig;
        private TurtlePangStoreItemConfig StoreItemConfig;
        private StorageTurtlePang Storage;
        private List<ItemStateBase> ItemStateList = new List<ItemStateBase>();
        private ItemStateLock Lock;
        private ItemStateNormal Normal;
        private ItemStateNoMoney NoMoney;
        private ItemStateFinish Finish;
        private void Awake()
        {
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
        public void InitStoreItem(StorageTurtlePang storage, TurtlePangStoreItemConfig storeItemConfig,TurtlePangStoreLevelConfig storeLevelConfig)
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
        public TurtlePangStoreItemConfig StoreItemConfig;
        public TurtlePangStoreLevelConfig StoreLevelConfig;
        public StorageTurtlePang Storage;
        private Image Icon;
        private LocalizeTextMeshProUGUI IconText;

        public virtual void Awake()
        {
            Icon = transform.Find("Icon").GetComponent<Image>();
            IconText = transform.Find("Icon/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public abstract bool ShouldShow();
        public void InitStoreItemState(StorageTurtlePang storage, TurtlePangStoreItemConfig storeItemConfig,TurtlePangStoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreItemConfig = storeItemConfig;
            StoreLevelConfig = storeLevelConfig;
        }

        public virtual void UpdateUI()
        {
            if ((TurtlePangUtils.TurtlePangStoreItemType) StoreItemConfig.Type == TurtlePangUtils.TurtlePangStoreItemType.BuildItem)
            {
                Icon.sprite =
                    ResourcesManager.Instance.GetSpriteVariant("TurtlePangAtlas", StoreItemConfig.Image);
                IconText.gameObject.SetActive(false);
            }
            else
            {
                Icon.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[0],UserData.ResourceSubType.Big);
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
            UIPopupTurtlePangShopBuyController.Open(StoreItemConfig,Storage);
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
            UIPopupTurtlePangShopBuyController.Open(StoreItemConfig,Storage);
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
    //     private StorageTurtlePang Storage;
    //     LocalizeTextMeshProUGUI PercentText;
    //     private LocalizeTextMeshProUGUI TitleText;
    //     private LocalizeTextMeshProUGUI BtnText;
    //     private void Awake()
    //     {
    //         PercentText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
    //         TitleText = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
    //         BtnText = transform.Find("NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
    //     }
    //
    //     public void InitLeaderBoardLevel(StorageTurtlePang storage)
    //     {
    //         Storage = storage;
    //         UpdateUI();
    //         // Storage.LeaderBoardStorage.SortController().BindRankChangeAction(OnRankChange);
    //     }
    //
    //     // private void OnDestroy()
    //     // {
    //     //     Storage.LeaderBoardStorage.SortController().UnBindRankChangeAction(OnRankChange);
    //     // }
    //
    //     // public void OnRankChange(TurtlePangLeaderBoardPlayer player)
    //     // {
    //     //     if (player.IsMe)
    //     //     {
    //     //         UpdateUI();
    //     //     }
    //     // }
    //     public void UpdateUI()
    //     {
    //         // TitleText.SetTerm("UI_EasterEggActive_Ranking_Title");
    //         PercentText.SetTermFormats(TurtlePangModel.Instance.GlobalConfig.LeastEnterBoardScore.ToString());
    //         BtnText.SetText(TurtlePangModel.Instance.GlobalConfig.LeastEnterBoardScore.ToString());
    //         // if (Storage.LeaderBoardStorage.IsInitFromServer())
    //         // {
    //         //     PercentText.SetText("No."+Storage.LeaderBoardStorage.SortController().MyRank);   
    //         // }
    //         // else
    //         // {
    //         //     PercentText.SetText(Storage.LeaderBoardStorage.StarCount + "/" + TurtlePangLeaderBoardModel.Instance.LeastEnterBoardScore);
    //         // }
    //     }
    // }
}