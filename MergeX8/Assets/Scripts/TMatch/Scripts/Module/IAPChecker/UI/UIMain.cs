using System;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IAPChecker
{
    /// <summary>
    /// UI类型
    /// </summary>
    public enum UIType
    {
        /// <summary>
        /// 物品
        /// </summary>
        Item,

        /// <summary>
        /// 特权
        /// </summary>
        Vip,

        /// <summary>
        /// 联系我们
        /// </summary>
        ContactUs,
    }

    /// <summary>
    /// Vip类型
    /// </summary>
    public enum UIVipType
    {
        /// <summary>
        /// 战令黄金券
        /// </summary>
        BpGoldenTicket,

        /// <summary>
        /// 战令黄金钥匙
        /// </summary>
        BpGoldenKey,

        /// <summary>
        /// RV商城会员卡
        /// </summary>
        RvShopVip,
        
        /// <summary>
        /// 战令循环特惠礼包购买后等级增加
        /// </summary>
        BpLevelAdd,
        /// <summary>
        /// TM战令黄金券
        /// </summary>
        TM_BpGoldenTicket
    }

    /// <summary>
    /// UI数据
    /// </summary>
    public class UIData : TMatch.UIWindowData
    {
        public UIType UIType;
        public UIVipType UIVipType;
        public List<int> ItemIds;
        public List<int> ItemCounts;
        public Action OnCallback;
    }

    public class UIMain : TMatch.UIWindowControllerEx
    {
        private TMatch.UIList _uiList;
        private Image _imageVip;
        private GameObject _goItem;
        private GameObject _goVip;
        private GameObject _goContactUs;
        private UIData _data;

        public override void PrivateAwake()
        {
            _uiList = new TMatch.UIList(widgets.transforms[0], widgets.gameObjects[3], OnItemRefresh);
            _imageVip = widgets.images[0];
            _goItem = widgets.gameObjects[0];
            _goVip = widgets.gameObjects[1];
            _goContactUs = widgets.gameObjects[2];

            SetClickListener(0, OnItemClaim);
            SetClickListener(1, InternalClose);
            SetClickListener(2, OnContactUsClick);
        }

        private void OnItemClaim()
        {
            // 这里为了显示效果所以这么写
            TMatch.UIMask.Enable(true);
            TMatch.ItemFlyModel.Instance.PlayItemAdd(_data.ItemIds, _data.ItemCounts,
                transform, () => { TMatch.UIMask.Enable(false); });
            StartCoroutine(TMatch.CommonUtils.DelayCall(0.5f, InternalClose));
        }

        private void OnContactUsClick()
        {
            // ContactUsController.Open();
            InternalClose();
        }

        private void OnItemRefresh(GameObject item, int index)
        {
            var id = _data.ItemIds[index];
            item.FindChild("TextNum").GetComponent<TextMeshProUGUI>().text =
                TMatch.CommonUtils.GetItemText(id, _data.ItemCounts[index]);
            var imageIcon = item.FindChild("ImageIcon").GetComponent<Image>();
            if (TMatch.ItemModel.Instance.GetConfigById(id) != null)
            {
                imageIcon.sprite = TMatch.ItemModel.Instance.GetItemSprite(id);
                return;
            }

            // var mergeItem = MergeConfigManager.Instance.GetItemConfig(id);
            // if (mergeItem != null)
            // {
            //     imageIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.Image);
            // }
        }

        private void RefreshGroupItem()
        {
            _goItem.SetActive(_data.UIType == UIType.Item);
            if (_data.UIType != UIType.Item)
                return;

            _uiList.Show(_data.ItemIds.Count);
        }

        private void RefreshGroupVip()
        {
            _goVip.SetActive(_data.UIType == UIType.Vip);
            if (_data.UIType != UIType.Vip)
                return;
        }

        private void RefreshGroupContactUs()
        {
            _goContactUs.SetActive(_data.UIType == UIType.ContactUs);
            if (_data.UIType != UIType.ContactUs)
                return;
        }

        private void GetVipIamge(UIData data)
        {
            switch (data.UIVipType)
            {
                case UIVipType.RvShopVip:
                    _imageVip.sprite = TMatch.CommonUtils.GetCommonSprite("RVstroe_Icon_Vip");
                    _imageVip.SetNativeSize();
                    break;
                case UIVipType.BpGoldenKey:
                    _imageVip.sprite = TMatch.CommonUtils.GetCommonSprite("BpGoldenKey");
                    _imageVip.SetNativeSize();
                    break;
                case UIVipType.BpGoldenTicket:
                    _imageVip.sprite = TMatch.CommonUtils.GetCommonSprite("BpGoldenTicket");
                    _imageVip.SetNativeSize();
                    break;
                case UIVipType.BpLevelAdd:
                    _imageVip.sprite = TMatch.CommonUtils.GetCommonSprite("BpAddLevel");
                    _imageVip.SetNativeSize();
                    break;
                default:
                    break;
            }
        }

        protected override void OnOpenWindow(TMatch.UIWindowData data)
        {
            base.OnOpenWindow(data);

            _data = (UIData) data;
            GetVipIamge((UIData) data);
            RefreshGroupVip();
            RefreshGroupItem();
            RefreshGroupContactUs();
            ShowItemBar(new TMatch.ItemBar.Data(201, false), new TMatch.ItemBar.Data(101, false), new TMatch.ItemBar.Data(301, false), TMatch.ItemBarModel.ItemBarStyle.LeftPlaceHold);
        }

        protected override void OnCloseWindow(bool destroy = true)
        {
            base.OnCloseWindow(destroy);

            _data.OnCallback?.Invoke();
        }

        public static void Open(UIData data)
        {
            TMatch.UIManager.Instance.OpenWindow<UIMain>("IAPChecker/Prefabs/UIMain", data);
        }
    }
}