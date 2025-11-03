using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIEndlessEnergyGiftBagController : UIWindowController
{
    public static UIEndlessEnergyGiftBagController Instance;

    public static UIEndlessEnergyGiftBagController Open()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEndlessEnergyGiftBagPopup,"open");
        Instance = UIManager.Instance.OpenUI(UINameConst.UIEndlessEnergyGiftBag) as UIEndlessEnergyGiftBagController;
        return Instance;
    }

    private const int ItemCount = 3;
    private List<GiftBagItem> ItemList = new List<GiftBagItem>();

    private List<GiftBagItem> Pool = new List<GiftBagItem>();

    public GiftBagItem GetItem()
    {
        GiftBagItem item = null;
        if (Pool.Count > 0)
        {
            item = Pool.Pop();
            item.gameObject.SetActive(true);
            return item;
        }
        item = Instantiate(DefaultItem, DefaultItem.parent).gameObject.AddComponent<GiftBagItem>();
        item.gameObject.SetActive(true);
        return item;
    }

    public void RecycleItem(GiftBagItem item)
    {
        Destroy(item.gameObject);//动画有问题，不回收了
        // Pool.Add(item);
        // item.gameObject.SetActive(false);
    }

    public class GiftBagItem : MonoBehaviour
    {
        public Animator Animator;
        private Animator LockFree;
        private Animator LockBuy;
        private Button BtnFree;
        private Button BtnBuy;
        public bool IsFree => Config.price <= 0;
        public Animator Lock => IsFree ? LockFree : LockBuy;
        public Button Btn => IsFree ? BtnFree : BtnBuy;
        private LocalizeTextMeshProUGUI PriceText;
        private List<CommonRewardItem> Rewards = new List<CommonRewardItem>();
        private TableEndlessEnergyGiftBagReward Config;

        private bool isAwake = false;

        private void Awake()
        {
            if (isAwake)
                return;
            isAwake = true;
            Animator = transform.Find("UIBgNormal").GetComponent<Animator>();
            LockFree = transform.Find("UIBgNormal/FreeButton/Root/LockIcon").GetComponent<Animator>();
            LockBuy = transform.Find("UIBgNormal/BuyButton/Root/LockIcon").GetComponent<Animator>();
            BtnFree = transform.Find("UIBgNormal/FreeButton").GetComponent<Button>();
            BtnFree.onClick.AddListener(OnCollect);
            BtnBuy = transform.Find("UIBgNormal/BuyButton").GetComponent<Button>();
            BtnBuy.onClick.AddListener(OnCollect);
            PriceText = transform.Find("UIBgNormal/BuyButton/Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public async void OnCollect()
        {
            var taskSource = EndlessEnergyGiftBagModel.Instance.Collect(Config);
            var result = await taskSource.Task;
            if (result)
            {
                UIEndlessEnergyGiftBagController.Instance?.OnCollect();
            }
        }

        public void Init(TableEndlessEnergyGiftBagReward config)
        {
            Awake();
            Config = config;
            foreach (var reward in Rewards)
            {
                Destroy(reward.gameObject);
            }

            var rewards = CommonUtils.FormatReward(Config.rewardId, Config.rewardNum);
            var defaultItem = transform.Find("UIBgNormal/Rewards/Item");
            defaultItem.gameObject.SetActive(false);
            foreach (var reward in rewards)
            {
                var item = Instantiate(defaultItem, defaultItem.parent).gameObject.AddComponent<CommonRewardItem>();
                item.gameObject.SetActive(true);
                item.Init(reward);
                Rewards.Add(item);
            }

            Animator.PlayAnimation("normal");
            transform.Find("UIBgNormal/BuyButton").gameObject.SetActive(!IsFree);
            transform.Find("UIBgNormal/FreeButton").gameObject.SetActive(IsFree);
            if (!IsFree)
            {
                PriceText.SetText(Config.price.ToString());
            }

            Btn.interactable = false;
        }

        public void PlayUnlock(bool fast = false)
        {
            Btn.interactable = true;
            if (fast)
            {
                Lock.gameObject.SetActive(false);
                return;
            }

            Lock.PlayAnimation("disappear", () => { Lock.gameObject.SetActive(false); });
        }

        public void PlayShow()
        {
            Animator.PlayAnimation("appear", () => { Animator.PlayAnimation("normal"); });
        }

        public Task PlayCollect()
        {
            return Animator.PlayAnimationAsync("disappear");
        }
    }

    private LocalizeTextMeshProUGUI TimeText;
    private Transform DefaultItem;
    public StorageEndlessEnergyGiftBag Storage => EndlessEnergyGiftBagModel.Instance.Storage;
    private Button rvBtn;
    public override void PrivateAwake()
    {
        TimeText = transform.Find("Root/TopGroup/TimeGroup/TextTime").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime", 0, 1);
        var closeBtn = transform.Find("Root/TopGroup/CloseSelectButton").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnClose);
        DefaultItem = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content/item0");
        var startPosY = (DefaultItem as RectTransform).anchoredPosition.y;
        var height = (DefaultItem as RectTransform).rect.height;
        var level = Storage.Level;
        for (var i = 0; i < ItemCount; i++)
        {
            var item = GetItem();
            item.Init(EndlessEnergyGiftBagModel.Instance.GetRewardConfig(level + i));
            (item.transform as RectTransform).SetAnchorPositionY(startPosY - height * i);
            ItemList.Add(item);
            item.transform.SetSiblingIndex(i);
        }

        ItemList[0].PlayUnlock(true);

        rvBtn = transform.Find("Root/BottomGroup/Button").GetComponent<Button>();
        var rvEnergyText = transform.Find("Root/BottomGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        var rvAd = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, ADConstDefine.RV_GET_ENERGY);
        var bs = AdConfigHandle.Instance.GetBonus(rvAd.Bonus);
        int rvGetCount = bs == null && bs.Count <= 0 ? 10 : bs[0].count;
        
        rvEnergyText.SetTermFormats(rvGetCount);
        
        
        UIAdRewardButton.Create(ADConstDefine.RV_GET_ENERGY, UIAdRewardButton.ButtonStyle.Disable, rvBtn.gameObject,
            (s) =>
            {
                try
                {
                    // if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_ENERGY) -
                    //     AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_ENERGY) > 0 &&
                    //     AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_ENERGY) > 0)
                    //     rvText?.SetTerm(AdSubSystem.Instance.GetRvText(ADConstDefine.RV_GET_ENERGY));
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLackEnergyRv);
                }
                catch (Exception e)
                {
                }
                // EnergyModel.Instance.AddEnergy(rvGetCount, new GameBIManager.ItemChangeReasonArgs() { reason=BiEventAdventureIslandMerge.Types.ItemChangeReason.AdEnergy},false,false);
                // FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(), UserData.ResourceId.Energy, rvGetCount, transform.position, 0.8f, true, true, 0.15f);
            }, false, null, () =>
            {
                // if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_ENERGY) -
                //     AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_ENERGY) <= 1)
                //     OnCloseClick();
            });
        
    }

    public async void OnCollect()
    {
        var removeItem = ItemList[0];
        removeItem.PlayCollect().AddCallBack(() => { RecycleItem(removeItem); }).WrapErrors();
        ItemList.RemoveAt(0);
        var startPosY = (DefaultItem as RectTransform).anchoredPosition.y;
        var height = (DefaultItem as RectTransform).rect.height;
        var newItem = GetItem();
        newItem.Init(EndlessEnergyGiftBagModel.Instance.GetRewardConfig(Storage.Level + ItemCount - 1));
        (newItem.transform as RectTransform).SetAnchorPositionY(startPosY - height * ItemCount);
        ItemList.Add(newItem);
        newItem.PlayShow();
        await XUtility.WaitSeconds(0.8f);
        for (var i = 0; i < ItemCount; i++)
        {
            var item = ItemList[i];
            (item.transform as RectTransform).DOAnchorPosY(startPosY - height * i, 0.2f).SetEase(Ease.Linear);
            item.transform.SetSiblingIndex(i);
        }

        await XUtility.WaitSeconds(0.2f);
        ItemList[0].PlayUnlock();
    }

    public void UpdateTime()
    {
        transform.Find("Root/BottomGroup").gameObject.SetActive(rvBtn.interactable);
        TimeText.SetText(EndlessEnergyGiftBagModel.Instance.GetLeftTimeStr());
        if (EndlessEnergyGiftBagModel.Instance.GetLeftTime() <= 0)
        {
            CancelInvoke("UpdateTime");
            AnimCloseWindow();
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEndlessEnergyGiftBagPopup,"close",HasBuy?"yes":"no");
        }
    }

    public bool HasBuy = false;

    public void OnClose()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEndlessEnergyGiftBagPopup,"close",HasBuy?"yes":"no");
        AnimCloseWindow(() =>
        {
            if (!HasBuy)
            {
                if (!EnergyPackageModel.Instance.CanShow())
                    UIPopupExchangeEnergyController.OpenUI();

                if (ExperenceModel.Instance.GetLevel() < 4)
                {
                    MergeTaskTipsController.Instance._mergeDailyTaskItem?.GuideArrow2?.ShowForTime(15f);
                    MergeTaskTipsController.Instance._mergeDailyTaskItem?.GuideArrow2?.SetSortingOrder(
                        MergeMainController.Instance.canvas.sortingOrder + 1);
                }
                else if (ExperenceModel.Instance.GetLevel() == 4)
                {
                    if (!StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.ContainsKey("showArrow4"))
                    {
                        StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.Add("showArrow4", 1);
                        MergeMainController.Instance?.BackBtnGuideArrow?.ShowForTime(5f);
                        MergeMainController.Instance?.BackBtnGuideArrow?.SetSortingOrder(MergeMainController.Instance.canvas
                            .sortingOrder + 1);
                    }
                }
                else
                {
                    if (!StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.ContainsKey("showArrow5"))
                    {
                        StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.Add("showArrow5", 1);
                        MergeMainController.Instance?.BackBtnGuideArrow?.ShowForTime(5f);
                        MergeMainController.Instance?.BackBtnGuideArrow?.SetSortingOrder(MergeMainController.Instance.canvas
                            .sortingOrder + 1);
                    }
                }
            } 
        });
    }
}