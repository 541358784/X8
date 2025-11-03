using System;
// using Activity.Christmas;
using DG.Tweening;
using DragonPlus;
// using DragonPlus.Config.Game;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
// using Store;
// using TipBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus.Config.TMatchShop;

namespace TMatch
{


    public class ItemBar : MonoBehaviour
    {
        /// <summary>
        /// ItemBar Data
        /// </summary>
        public class Data
        {
            /// <summary>
            /// 资源ID
            /// </summary>
            public int Id;

            /// <summary>
            /// 是否显示AddBtn
            /// 默认显示
            /// </summary>
            public bool IsShowAddBtn = true;

            /// <summary>
            /// 是否显示物品动画
            /// 默认显示
            /// </summary>
            public bool IsShowFlyAnim = true;

            /// <summary>
            /// Add点击回调
            /// 默认打开商城
            /// </summary>
            public Action OnAddBtnClick;

            /// <summary>
            /// 默认数值
            /// </summary>
            public int DefaultValue;

            public Data(int id, bool isShowAddBtn = true, bool isShowFlyAnim = true, Action onAddBtnClick = null,
                int defaultValue = -1)
            {
                Id = id;
                IsShowAddBtn = isShowAddBtn;
                IsShowFlyAnim = isShowFlyAnim;
                OnAddBtnClick = onAddBtnClick;
                DefaultValue = defaultValue;
            }
        }

        protected Data data;
        private ItemConfig cfg;

        private Image imgIcon;
        private Button btnAdd;
        private RectTransform rectBg;
        private LocalizeTextMeshProUGUI txtNum;
        private int num = 0;
        private int tNum = 0;
        private int tNumDelta = 0;
        private float tCurNum = 0f;
        private float tNumTime = 0.5f;
        private Sequence _scaleSequence;

        private float infinityTime;

        private bool isShort
        {
            get { return isEnergy || data.Id == 501; }
        }

        private bool isShowMax
        {
            get { return isEnergy; }
        }

        private bool isEnergy => false;//cfg.type == (int) ItemType.Energy;
        private bool isInfinity;

        /// <summary>
        /// 资源id
        /// </summary>
        public int Id => data.Id;

        private void Awake()
        {
            OnInit();
        }

        protected virtual void OnInit()
        {
            rectBg = transform.Find("BgImage").GetComponent<RectTransform>();
            btnAdd = transform.Find("BgImage/btnAdd").GetComponent<Button>();
            txtNum = transform.Find("BgImage/textCount").GetComponent<LocalizeTextMeshProUGUI>();
            imgIcon = transform.Find("BgImage/Icon").GetComponent<Image>();

            ButtonUtils.Add(btnAdd, OnAddClick);
            // btnAdd.onClick.AddListener(OnAddClick);

            Refresh();
        }

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
            if (ShowInfinityTime()) return;
            if (tNum == num) return;
            OnNumAnimPlaying();
        }

        private bool ShowInfinityTime()
        {
            return false;
            // if (!isEnergy) return false; //不是体力就不做处理
            // ulong timer = ItemModel.Instance.GetCountdown(data.Id);
            // if (timer <= 0 && !isInfinity) return false;
            // infinityTime -= Time.deltaTime;
            // if (infinityTime > 0) return true;
            // if (timer > 0)
            // {
            //     infinityTime = 1;
            //     txtNum.SetText($"{CommonUtils.GetLimitTimeNumText((int) timer / 1000)}");
            //     if (!isInfinity)
            //     {
            //         imgIcon.sprite = ResUtil.GetIcon(ResourceId.Energy_Infinity);
            //         isInfinity = true;
            //     }
            // }
            // else
            // {
            //     txtNum.SetText(isShowMax ? $"{num}/{ItemModel.Instance.GetItemMax(data.Id)}" : $"{num:N0}");
            //     if (isInfinity)
            //     {
            //         imgIcon.sprite = ResUtil.GetIcon(ResourceId.Energy);
            //         isInfinity = false;
            //     }
            // }
            //
            // return timer > 0;
        }

        private void OnNumAnimPlaying()
        {
            tCurNum += tNumDelta * Time.deltaTime / tNumTime;
            int curNum = (int) tCurNum;
            if (tNumDelta > 0 && curNum >= tNum || tNumDelta < 0 && curNum <= tNum)
            {
                curNum = tNum;
            }

            if (curNum == num) return;
            SetNum(curNum);
        }

        private void OnAddClick()
        {
            AudioManager.Instance.PlayBtnTap();

            if (data.OnAddBtnClick != null)
            {
                data.OnAddBtnClick.Invoke();
                return;
            }

            // if (isEnergy)
            // {
            //     if (!ItemModel.Instance.IsNumMax((int) ResourceId.Energy) &&
            //         ItemModel.Instance.GetCountdown((int) ResourceId.Energy) <= 0) UIItemEnergy.Open();
            //     return;
            // }

            // switch ((ResourceId) data.Id)
            // {
            //     case ResourceId.Coin:
            //         DragonPlus.GameBIManager.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventIconClick, "Store", "Main");
            //         UIStore.Open(new UIStoreData()
            //         {
            //             Page = eStorePage.Main,
            //             itemType = eStoreItemType.Gold,
            //             source = Module.Source.ClickCoin,
            //             sourceItemId = data.Id
            //         });
            //         return;
            //     case ResourceId.Gem:
            //         DragonPlus.GameBIManager.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventIconClick, "Store", "Main");
            //         UIStore.Open(new UIStoreData()
            //         {
            //             Page = eStorePage.Main,
            //             itemType = eStoreItemType.None,
            //             source = Module.Source.ClickGem,
            //             sourceItemId = data.Id
            //         });
            //         return;
            //     case ResourceId.ChristmasToken:
            //         if (Activity.Christmas.Model.Instance.IsOpening)
            //         {
            //             UIBuyCap.Open();
            //         }
            //
            //         return;
            //     case ResourceId.AvatarCoin:
            //         TipBox.UIMain.Open(LocalizationManager.Instance.GetLocalizedString("UI_Dress_tips_1"), transform,
            //             TipType.Top, 0, -100);
            //         return;
            //     case ResourceId.MergeEnergy:
            //         UIPopupBuyEnergyController.Open();
            //         return;
            // }
        }

        private void OnItemUpdate(BaseEvent e)
        {
            return;
            // ItemUpdateEvent evt = e as ItemUpdateEvent;
            // if (evt == null) return;
            // if (evt.Id != data.Id)
            // {
            //     if (data.Id != (int) ResourceId.Energy) return;
            //     if (evt.Id != (int) ResourceId.Energy_Infinity) return; //这里做体力无限的显示
            // }
            //
            // SetTNum(evt.Count);
        }

        private void OnItemMaxUpdate(BaseEvent e)
        {
            if (!(e is ItemMaxUpdateEvent evt) || evt.Id != data.Id) return;

            SetNum(ItemModel.Instance.GetNum(data.Id));
        }

        private void OnUnfulfilledPaymentHandleSuccess(BaseEvent e)
        {
            // RefreshNum(); // 补单事件暂不刷新UI道具数字，因为可能存在开宝箱触发数字变化的动画，提前刷新到最终数值，动画会导致数值显示不正确
        }

        private void SetTNum(int tNum)
        {
            this.tNum = tNum;
            tCurNum = num;
            tNumDelta = tNum - num;
            // if (!data.IsShowAddBtn) return;//这里表示物品达到最大值时，不显示加号
            // btnAdd.gameObject.SetActive(!ItemModel.Instance.IsNumMax(cfg.Id));
            RefreshAddBtn();
        }

        protected virtual void SetNum(int num)
        {
            this.num = num;
            ulong timer = ItemModel.Instance.GetCountdown(data.Id);
            if (timer > 0)
            {
                txtNum.SetText($"{CommonUtils.GetLimitTimeNumText((int) timer / 1000)}");
            }
            else
            {
                txtNum.SetText(isShowMax ? $"{num}/{ItemModel.Instance.GetItemMax(data.Id)}" : $"{num:N0}");
            }
        }

        private void OnItemFly(int delta, ulong version)
        {
            if (version < UIItemTop.Version)
                return;

            SetTNum(tNum + delta);
        }

        private void OnItemArrive()
        {
            if (imgIcon == null)
                return;

            _scaleSequence?.Kill();
            _scaleSequence = DOTween.Sequence();
            _scaleSequence.Append(imgIcon.transform.DOScale(1.2f, 0.05f));
            _scaleSequence.Append(imgIcon.transform.DOScale(1f, 0.05f));
            _scaleSequence.SetLoops(3);
        }

        private void Refresh()
        {
            if (rectBg == null) return;

            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            rectBg.sizeDelta = new Vector2(isShort ? 145f : 185f, rectBg.rect.height);
            // btnAdd.gameObject.SetActive((data.IsShowAddBtn || data.OnAddBtnClick != null) && !ItemModel.Instance.IsNumMax(data.Id) && ItemModel.Instance.GetCountdown(data.Id) <= 0);
            RefreshAddBtn();
            num = data.DefaultValue == -1 ? ItemModel.Instance.GetNum(data.Id) : data.DefaultValue;
            data.DefaultValue = -1;
            tNum = num;
            SetNum(num);
            imgIcon.sprite = ResUtil.GetIcon((ResourceId) data.Id);
            // if ((ResourceId) data.Id == ResourceId.Energy && ItemModel.Instance.GetCountdown(data.Id) > 0)
            //     imgIcon.sprite = ResUtil.GetIcon(ResourceId.Energy_Infinity);

            if (data.IsShowFlyAnim)
            {
                ItemFlyModel.Instance.RegisterFlyTo(data.Id, imgIcon.transform, OnItemFly, OnItemArrive);
                ItemFlyModel.Instance.RegisterFlyOut(data.Id, imgIcon.transform, OnItemFly);
                // if ((ResourceId) data.Id == ResourceId.Energy)
                // {
                //     ItemFlyModel.Instance.RegisterFlyTo((int) ResourceId.Energy_Infinity, imgIcon.transform, OnItemFly,
                //         OnItemArrive);
                //     ItemFlyModel.Instance.RegisterFlyOut((int) ResourceId.Energy_Infinity, imgIcon.transform,
                //         OnItemFly);
                // }
            }

            EventDispatcher.Instance.AddEventListener(EventEnum.ItemUpdate, OnItemUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.ItemMaxUpdate, OnItemMaxUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.UnfulfilledPaymentHandleSuccess,
                OnUnfulfilledPaymentHandleSuccess);
        }

        private void RefreshAddBtn()
        {
            if (btnAdd == null)
                return;

            if (!data.IsShowAddBtn && data.OnAddBtnClick == null)
            {
                btnAdd.gameObject.SetActive(false);
                return;
            }

            // if (data.Id == (int) ResourceId.MergeEnergy)
            // {
            //     btnAdd.gameObject.SetActive(true);
            //     return;
            // }

            btnAdd.gameObject.SetActive(!ItemModel.Instance.IsNumMax(data.Id) &&
                                        ItemModel.Instance.GetCountdown(data.Id) <= 0);
        }

        public void Clear()
        {
            _scaleSequence?.Kill();

            if (data == null) return;

            if (data.IsShowFlyAnim)
            {
                ItemFlyModel.Instance.UnregisterFlyTo(data.Id, imgIcon.transform, OnItemFly);
                ItemFlyModel.Instance.UnregisterFlyOut(data.Id, imgIcon.transform, OnItemFly);
                // if ((ResourceId) data.Id == ResourceId.Energy)
                // {
                //     ItemFlyModel.Instance.UnregisterFlyTo((int) ResourceId.Energy_Infinity, imgIcon.transform,
                //         OnItemFly);
                //     ItemFlyModel.Instance.UnregisterFlyOut((int) ResourceId.Energy_Infinity, imgIcon.transform,
                //         OnItemFly);
                // }
            }

            EventDispatcher.Instance.RemoveEventListener(EventEnum.ItemUpdate, OnItemUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.ItemMaxUpdate, OnItemMaxUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.UnfulfilledPaymentHandleSuccess,
                OnUnfulfilledPaymentHandleSuccess);
        }

        public void SetData(Data data)
        {
            Clear();
            this.data = data;
            if (data != null) cfg = ItemModel.Instance.GetConfigById(data.Id);
            Refresh();
        }

        public void RefreshNum()
        {
            if (data == null)
                return;
            SetTNum(ItemModel.Instance.GetNum(data.Id));
        }

        public static ItemBar LoadItemBar(Transform parent, Data data = null)
        {
            GameObject asset = ResourcesManager.Instance.LoadResource<GameObject>("Module/Item/Prefabs/ItemBar");
            GameObject go = Instantiate(asset, parent);
            ItemBar itemBar = go.AddComponent<ItemBar>();
            itemBar.SetData(data);
            return itemBar;
        }
    }
}
