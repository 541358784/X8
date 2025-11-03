using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DragonPlus.Config.Screw;
using Screw;
using Screw.Module;
using Screw.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class EventEnum
{
    public const string ScrewUpdateShopViewGroup = "ScrewUpdateShopViewGroup";
}
public class EventScrewUpdateShopViewGroup : BaseEvent
{
    public UIScrewShop.ShopViewGroupType GroupType;
    public EventScrewUpdateShopViewGroup() : base(EventEnum.ScrewUpdateShopViewGroup) { }
    public EventScrewUpdateShopViewGroup(UIScrewShop.ShopViewGroupType groupType) : base(EventEnum.ScrewUpdateShopViewGroup)
    {
        GroupType = groupType;
    }
}
namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/Main/UIScrewShop")]
    public partial class UIScrewShop : UIWindow
    {
        public enum ShopViewGroupType
        {
            None=0,
            Coin=1,
            DailyPackage=2,
        }
        private Transform DefaultDailyPackageItem;
        // private List<DailyPackageItem> DailyPackageItems = new List<DailyPackageItem>();
        private Transform DefaultCoinItem;
        // private List<StoreCoinItem> CoinItems = new List<StoreCoinItem>();
        private Button CloseBtn;
        private ResBarMono _energyResBar;
        private ResBarMono _coinResBar;
        private Dictionary<ShopViewGroupType, Transform> GroupNodeDic = new Dictionary<ShopViewGroupType, Transform>();
        private Dictionary<ShopViewGroupType, List<Transform>> GroupItemDic =
            new Dictionary<ShopViewGroupType, List<Transform>>();

        private RectTransform Content;
        private RectTransform Viewport;
        private float MinContentY = 0;
        private float MaxConetntY;
        public static void Open(ShopViewGroupType position)
        {
            UIModule.Instance.ShowUI(typeof(UIScrewShop),position);
        }
        public override void PrivateAwake()
        {
            Content = transform.Find("Root/Scroll View/Viewport/Content").GetComponent<RectTransform>();
            Viewport = Content.parent as RectTransform;
            _energyResBar = transform.Find("Root/UIScrewResourcesGroup/Root/ResourcesGroup/ResourcesBarGroup1").gameObject.AddComponent<EnergyResBar>();
            _coinResBar = transform.Find("Root/UIScrewResourcesGroup/Root/ResourcesGroup/ResourcesBarGroup2").gameObject.AddComponent<CoinResBar>();
            _coinResBar.SetAddButtonActive(false);
            DefaultDailyPackageItem = transform.Find("Root/Scroll View/Viewport/Content/PackageItem/Item");
            DefaultDailyPackageItem.gameObject.SetActive(false);
            DefaultCoinItem = transform.Find("Root/Scroll View/Viewport/Content/NailItem/Item");
            DefaultCoinItem.gameObject.SetActive(false);
            CloseBtn = transform.Find("Root/UIScrewResourcesGroup/Root/ResourcesGroup/CloseButton").GetComponent<Button>();
            CloseBtn.onClick.AddListener(() =>
            {
                AnimCloseWindow();
            });
            GroupNodeDic.Add(ShopViewGroupType.Coin,DefaultCoinItem.parent);
            GroupNodeDic.Add(ShopViewGroupType.DailyPackage,DefaultDailyPackageItem.parent);
            EventDispatcher.Instance.AddEvent<EventScrewUpdateShopViewGroup>(EventCheckGroupVisible);
            
            CommonUtils.NotchAdapte(transform.Find("Root/UIScrewResourcesGroup/Root"));
        }

        public void AddGroupItem(ShopViewGroupType type,Transform item)
        {
            if (!GroupItemDic.ContainsKey(type))
                GroupItemDic.Add(type,new List<Transform>());
            GroupItemDic[type].Add(item);
        }

        public void EventCheckGroupVisible(EventScrewUpdateShopViewGroup evt)
        {
            CheckGroupVisible(evt.GroupType);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventScrewUpdateShopViewGroup>(EventCheckGroupVisible);
        }

        public void CheckGroupVisible(ShopViewGroupType type)
        {
            if (!GroupNodeDic.TryGetValue(type, out var groupNode))
            {
                return;
            }
            if (!GroupItemDic.TryGetValue(type, out var items))
            {
                groupNode.gameObject.SetActive(false);
                return;
            }
            var visible = false;
            foreach (var item in items)
            {
                if (item.gameObject.activeSelf)
                {
                    visible = true;
                    break;
                }
            }
            groupNode.gameObject.SetActive(visible);
        }

        public void FocusOn(ShopViewGroupType groupType)
        {
            if (!GroupNodeDic.ContainsKey(groupType))
                return;
            var targetGroup = GroupNodeDic[groupType] as RectTransform;
            var targetY = -targetGroup.anchoredPosition.y;
            if (targetY < MinContentY)
                targetY = MinContentY;
            if (targetY > MaxConetntY)
                targetY = MaxConetntY;
            Content.SetAnchorPositionY(targetY);    
        }

        private bool IsInit = false;
        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            var startPosition = (ShopViewGroupType)objs[0];
            XUtility.WaitFrames(1, () =>
            {
                MaxConetntY = Content.rect.height - Viewport.rect.height;
                FocusOn(startPosition);
            });
            if (IsInit)
                return;
            IsInit = true;
            var shopConfigs = StoreModel.Instance.ConfigList;
            foreach (var shopConfig in shopConfigs)
            {
                var groupType = (ShopViewGroupType)shopConfig.ShopViewGroupType;
                switch (groupType)
                {
                    case ShopViewGroupType.None:
                        break;
                    case ShopViewGroupType.Coin:
                    {
                        
                        var coinItem = Instantiate(DefaultCoinItem, DefaultCoinItem.parent).gameObject
                            .AddComponent<StoreCoinItem>();
                        coinItem.gameObject.SetActive(true);
                        var coinShopConfig =  DragonPlus.Config.Screw.GameConfigManager.Instance.TableCoinShopConfigList.Find(a=>a.Id == shopConfig.Id);
                        coinItem.Init(coinShopConfig);
                        AddGroupItem(groupType, coinItem.transform);
                        break;
                    }
                    case ShopViewGroupType.DailyPackage:
                    {
                        var dailyPackageItem = Instantiate(DefaultDailyPackageItem, DefaultDailyPackageItem.parent).gameObject
                            .AddComponent<DailyPackageItem>();
                        dailyPackageItem.gameObject.SetActive(true);
                        var dailyPackageConfig =  DragonPlus.Config.Screw.GameConfigManager.Instance.TableDailyPackageConfigList.Find(a=>a.Id == shopConfig.Id);
                        dailyPackageItem.Init(dailyPackageConfig);
                        AddGroupItem(groupType, dailyPackageItem.transform);
                        break;
                    }
                }
            }
        }
    }
}