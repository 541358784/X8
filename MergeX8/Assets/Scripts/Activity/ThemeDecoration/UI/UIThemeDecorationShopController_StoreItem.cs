using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIThemeDecorationShopController
{
    public class StoreItemLevel : MonoBehaviour
    {
        private ThemeDecorationStoreLevelConfig StoreLevelConfig;
        private StorageThemeDecoration Storage;
        Transform DefaultStoreItem;
        private LocalizeTextMeshProUGUI TitleText;
        private List<StoreItem> ItemList = new List<StoreItem>();
        private Transform CollectFlag;
        private Button CollectBtn;
        Transform DefaultRewardItem;
        private Animator Lock;
        private List<LevelCompleteRewardItem> RewardItemList = new List<LevelCompleteRewardItem>();
        private void Awake()
        {
            TitleText = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
            DefaultStoreItem = transform.Find("ItemGroup/Item");
            DefaultStoreItem.gameObject.SetActive(false);
            DefaultRewardItem = transform.Find("Reward/Item");
            DefaultRewardItem.gameObject.SetActive(false);
            CollectFlag = transform.Find("Reward/Finish");
            CollectBtn = transform.Find("Reward").GetComponent<Button>();
            CollectBtn.onClick.AddListener(() =>
            {
                if (Storage.CanCollectLevelCompleteReward(StoreLevelConfig))
                {
                    Storage.CollectLevelCompleteReward(StoreLevelConfig);
                }
            });
            Lock = transform.Find("Title/Lock").GetComponent<Animator>();
            EventDispatcher.Instance.AddEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.AddEvent<EventThemeDecorationCollectStoreCompleteReward>(OnCollectStoreLevelCompleteReward);
        }
        public void OnBuyStoreItem(EventThemeDecorationBuyStoreItem evt)
        {
            if (StoreLevelConfig.StoreItemList.Contains(evt.StoreItemConfig.Id))
                RefreshCompleteReward();
            // gameObject.SetActive( StoreLevelConfig.Id >= Storage.GetCurStoreLevel().Id);
        }
        public void OnCollectStoreLevelCompleteReward(EventThemeDecorationCollectStoreCompleteReward evt)
        {
            if (StoreLevelConfig == evt.StoreLevelConfig)
                RefreshCompleteReward();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.RemoveEvent<EventThemeDecorationCollectStoreCompleteReward>(OnCollectStoreLevelCompleteReward);
        }

        public void InitStoreItemLevel(StorageThemeDecoration storage,ThemeDecorationStoreLevelConfig storeLevelConfig)
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
                var itemConfig = ThemeDecorationModel.Instance.StoreItemConfig[StoreLevelConfig.StoreItemList[i]];
                var storeItem = Instantiate(DefaultStoreItem, DefaultStoreItem.parent).gameObject
                    .AddComponent<StoreItem>();
                storeItem.gameObject.SetActive(true);
                storeItem.InitStoreItem(storage, itemConfig,StoreLevelConfig);
                ItemList.Add(storeItem);
            }
            TitleText.SetTermFormats(StoreLevelConfig.Id.ToString());
            for (var i = 0; i < RewardItemList.Count; i++)
            {
                Destroy(RewardItemList[i].gameObject);
            }
            RewardItemList.Clear();
            var completeReward = CommonUtils.FormatReward(StoreLevelConfig.RewardId, StoreLevelConfig.RewardNum);
            for (var i = 0; i < completeReward.Count; i++)
            {
                var reward = completeReward[i];
                var storeItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject
                    .AddComponent<LevelCompleteRewardItem>();
                storeItem.gameObject.SetActive(true);
                storeItem.Init(reward);
                RewardItemList.Add(storeItem);
            }
            RefreshCompleteReward();
            Lock.gameObject.SetActive(StoreLevelConfig.Id > Storage.GetCurStoreLevel().Id);
            LayoutRebuilder.ForceRebuildLayoutImmediate(DefaultStoreItem.parent as RectTransform);
        }

        public void RefreshCompleteReward()
        {
            var canCollect = Storage.CanCollectLevelCompleteReward(StoreLevelConfig);
            var hasCollect = Storage.HasCollectLevelCompleteReward(StoreLevelConfig);
            for (var i = 0; i < RewardItemList.Count; i++)
            {
                RewardItemList[i].gameObject.SetActive(!hasCollect);
                RewardItemList[i].SetCanCollect(canCollect);
            }
            CollectFlag.gameObject.SetActive(hasCollect);
        }
        public async void PerformUnlock(Action callback)
        {
            if (StoreLevelConfig.Id <= Storage.GetCurStoreLevel().Id && Lock.gameObject.activeSelf)
            {
                Lock.PlayAnimation("open", () =>
                {
                    Lock.gameObject.SetActive(false);
                });
            }
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

    public class LevelCompleteRewardItem : MonoBehaviour
    {
        public Transform CanCollectFlag;
        private Image Icon;
        private LocalizeTextMeshProUGUI Text;
        public void Init(ResData resData)
        {
            CanCollectFlag = transform.Find("Receive");
            Icon = transform.Find("Icon").GetComponent<Image>();
            Text = transform.Find("Icon/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Icon.sprite = UserData.GetResourceIcon(resData.id, UserData.ResourceSubType.Big);
            Text.gameObject.SetActive(resData.count>1);
            Text.SetText(resData.count.ToString());
        }

        public void SetCanCollect(bool flag)
        {
            CanCollectFlag.gameObject.SetActive(flag);
        }
    }
    public class StoreItem : MonoBehaviour
    {
        private ThemeDecorationStoreLevelConfig StoreLevelConfig;
        private ThemeDecorationStoreItemConfig StoreItemConfig;
        private StorageThemeDecoration Storage;
        private List<ItemStateBase> ItemStateList = new List<ItemStateBase>();
        private ItemStateLock Lock;
        private ItemStateNormal Normal;
        private ItemStateNoMoney NoMoney;
        private ItemStateFinish Finish;
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.AddEvent<EventThemeDecorationScoreChange>(OnScoreChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
            EventDispatcher.Instance.RemoveEvent<EventThemeDecorationScoreChange>(OnScoreChange);
        }

        public void OnBuyStoreItem(EventThemeDecorationBuyStoreItem evt)
        {
            UpdateUI();
        }
        public void OnScoreChange(EventThemeDecorationScoreChange evt)
        {
            UpdateUI();
        }
        public void InitStoreItem(StorageThemeDecoration storage, ThemeDecorationStoreItemConfig storeItemConfig,ThemeDecorationStoreLevelConfig storeLevelConfig)
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
        public ThemeDecorationStoreItemConfig StoreItemConfig;
        public ThemeDecorationStoreLevelConfig StoreLevelConfig;
        public StorageThemeDecoration Storage;
        private Image Icon;
        private LocalizeTextMeshProUGUI IconText;

        public virtual void Awake()
        {
            Icon = transform.Find("Icon").GetComponent<Image>();
            IconText = transform.Find("Icon/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public abstract bool ShouldShow();
        public void InitStoreItemState(StorageThemeDecoration storage, ThemeDecorationStoreItemConfig storeItemConfig,ThemeDecorationStoreLevelConfig storeLevelConfig)
        {
            Storage = storage;
            StoreItemConfig = storeItemConfig;
            StoreLevelConfig = storeLevelConfig;
        }

        public virtual void UpdateUI()
        {
            if ((ThemeDecorationStoreItemType) StoreItemConfig.Type == ThemeDecorationStoreItemType.BuildItem)
            {
                Icon.sprite =
                    ResourcesManager.Instance.GetSpriteVariant(Storage.GetAtlasNameWithSkinName(), StoreItemConfig.Image);
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
            UIPopupThemeDecorationShopBuyController.Open(StoreItemConfig,Storage);
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
            UIPopupThemeDecorationShopBuyController.Open(StoreItemConfig,Storage);
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
}