﻿using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using TMPro;

 namespace TMatch
 {
     public class UiTipMainBox : UIWindow, ICanvasRaycastFilter
     {
         public enum TipBoxItemDir
         {
             UpLeft,
             UpMiddle,
             UpRight,
             DownLeft,
             DownMiddle,
             DownRight,
             Left,
             Right,
         }

         public class TipBoxItemInfo
         {
             public Sprite ItemSprite { get; set; }
             public int ItemId { get; set; }
             public int ItemCount { get; set; }
         }

         public class TipBoxItemInfoConvert : IFeedbackItemConverter<TipBoxItemInfo>
         {
             public static TipBoxItemInfoConvert Instance { get; } = new TipBoxItemInfoConvert();

             public bool IsMapping(int itemId, TipBoxItemInfo feedBackItem)
             {
                 return feedBackItem.ItemId == itemId;
             }

             public TipBoxItemInfo ConvertCreate(int itemId, int itemCount)
             {
                 return new TipBoxItemInfo()
                 {
                     ItemId = itemId,
                     ItemCount = itemCount,
                     ItemSprite = null
                 };
             }

             public void ConvertAddTo(TipBoxItemInfo feedBackItem, int itemCount)
             {
                 feedBackItem.ItemCount += itemCount;
             }
         }

         public static void ShowTip(string title, List<int> itemsId, List<int> itemsCount, Transform node, TipBoxItemDir dir, Vector2 offset,
             bool isAttachToArrow = false, bool needChangeWidth = false, float autoCloseTime = 0f, bool autoAdapt = true)
         {
             // var items = new List<TipBoxItemInfo>();
             // for (var i = 0; i < itemsId.Count; i++)
             // {
             //     items.Add(new TipBoxItemInfo()
             //     {
             //         ItemId = itemsId[i],
             //         ItemCount = itemsCount[i]
             //     });
             // }
             var items = ItemModel.Instance.PreviewItemsWithExchangeable(itemsId, itemsCount,
                 TipBoxItemInfoConvert.Instance);
             ShowTip(title, items, node, dir, offset, isAttachToArrow, needChangeWidth, autoCloseTime, autoAdapt);
         }

         public static void ShowTip(string title, List<TipBoxItemInfo> items, Transform node, TipBoxItemDir dir, Vector2 offset,
             bool isAttachToArrow = false, bool needChangeWidth = false, float autoCloseTime = 0f, bool autoAdapt = true)
         {
             var tip = UIManager.Instance.OpenWindow<UiTipMainBox>("TMatch/Prefabs/TipBoxItems");
             tip.Init(title, items, node, dir, offset, isAttachToArrow, needChangeWidth, autoCloseTime);
             if (autoAdapt)
             {
                 tip.autoAdapt();
             }
         }

         RectTransform transRoot;
         RectTransform transRootImage;
         RectTransform transItemRoot;

         RectTransform transImageArrowUpLeft;
         RectTransform transImageArrowUpMiddle;
         RectTransform transImageArrowUpRight;
         RectTransform transImageArrowDownLeft;
         RectTransform transImageArrowDownMiddle;
         RectTransform transImageArrowDownRight;
         RectTransform transImageArrowLeft;
         RectTransform transImageArrowRight;

         private const float originHeight = 160;
         private const float originUpY = 85;

         LocalizeTextMeshProUGUI TextTitle;
         ScrollRect scrollView;
         LayoutElement layoutElementScrollView;

         private GameObject _tsCloneItem;

         bool StartRemove;

         private bool _isAttachToArrow = false;
         private Vector2 _offsetFinal;
         public override bool EffectUIAnimation { get; set; } = false;

         public override void PrivateAwake()
         {
             transRoot = transform.Find("Root").GetComponent<RectTransform>();
             transRootImage = transform.Find("Root/Image").GetComponent<RectTransform>();
             transItemRoot = transform.Find("Root/Image/Scroll View/Viewport/Content").GetComponent<RectTransform>();

             transImageArrowUpLeft = transform.Find("Root/Image/ImageArrowUpLeft").GetComponent<RectTransform>();
             transImageArrowUpMiddle = transform.Find("Root/Image/ImageArrowUpMiddle").GetComponent<RectTransform>();
             transImageArrowUpRight = transform.Find("Root/Image/ImageArrowUpRight").GetComponent<RectTransform>();
             transImageArrowDownLeft = transform.Find("Root/Image/ImageArrowDownLeft").GetComponent<RectTransform>();
             transImageArrowDownMiddle = transform.Find("Root/Image/ImageArrowDownMiddle").GetComponent<RectTransform>();
             transImageArrowDownRight = transform.Find("Root/Image/ImageArrowDownRight").GetComponent<RectTransform>();
             transImageArrowLeft = transform.Find("Root/Image/ImageArrowLeft").GetComponent<RectTransform>();
             transImageArrowRight = transform.Find("Root/Image/ImageArrowRight").GetComponent<RectTransform>();

             TextTitle = transform.Find("Root/Image/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
             scrollView = transform.Find("Root/Image/Scroll View").GetComponent<ScrollRect>();
             layoutElementScrollView = transform.Find("Root/Image/Scroll View").GetComponent<LayoutElement>();

             _tsCloneItem = ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Prefabs/TipBoxItem");
         }

         public void Init(string title, List<TipBoxItemInfo> items, Transform node, TipBoxItemDir dir, Vector2 offset, bool isAttachToArrow, bool needChangeWidth, float autoCloseTime = 0f)
         {
             StartRemove = false;

             transImageArrowUpLeft.gameObject.SetActive(dir == TipBoxItemDir.UpLeft);
             transImageArrowUpMiddle.gameObject.SetActive(dir == TipBoxItemDir.UpMiddle);
             transImageArrowUpRight.gameObject.SetActive(dir == TipBoxItemDir.UpRight);
             transImageArrowDownLeft.gameObject.SetActive(dir == TipBoxItemDir.DownLeft);
             transImageArrowDownMiddle.gameObject.SetActive(dir == TipBoxItemDir.DownMiddle);
             transImageArrowDownRight.gameObject.SetActive(dir == TipBoxItemDir.DownRight);
             transImageArrowLeft.gameObject.SetActive(dir == TipBoxItemDir.Left);
             transImageArrowRight.gameObject.SetActive(dir == TipBoxItemDir.Right);

             switch (dir)
             {
                 case TipBoxItemDir.UpLeft:
                 case TipBoxItemDir.UpMiddle:
                 case TipBoxItemDir.UpRight:
                     transRootImage.pivot = new Vector2(0.5f, 1);
                     break;
                 case TipBoxItemDir.DownLeft:
                 case TipBoxItemDir.DownMiddle:
                 case TipBoxItemDir.DownRight:
                     transRootImage.pivot = new Vector2(0.5f, 0);
                     break;
                 case TipBoxItemDir.Left:
                     transRootImage.pivot = new Vector2(0f, 0.5f);
                     break;
                 case TipBoxItemDir.Right:
                     transRootImage.pivot = new Vector2(1f, 0.5f);
                     break;
                 default:
                     throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
             }
             TextTitle.SetText(title);
             // TextTitle.SetTerm(title);

             transRoot.localScale = Vector3.zero;
             // transRoot.SetParent(node);
             Transform parentTr = transRoot.parent;
             transRoot.SetParent(node);
             transRoot.localPosition = offset;
             transRoot.localScale = Vector3.one;
             transRoot.SetParent(parentTr);
             transRoot.localScale = Vector3.one;

             var heightBase = items.Count > 9 ? 80 : 90;
             var heightItem = items.Count > 9 ? 76 : 80;
             var height = 0;
             if (items.Count > 9)
                 height = heightBase + heightItem * (3 - 1);
             else
                 height = heightBase + (items.Count == 0 ? 0 : ((items.Count - 1) / 3) * heightItem);
             layoutElementScrollView.minHeight = height;
             layoutElementScrollView.preferredHeight = height;

             if (needChangeWidth)
             {
                 transRootImage.GetComponent<RectTransform>().SetSizeWidth(items.Count < 3 ? 100 * items.Count * 1.5f : 316);
                 layoutElementScrollView.GetComponent<RectTransform>()
                     .SetSizeWidth(items.Count < 3 ? 100 * items.Count * 1.5f : 316);
             }

             scrollView.vertical = items.Count > 9;

             foreach (var item in items)
             {
                 var tsItem = Instantiate(_tsCloneItem.transform, transItemRoot);
                 tsItem.localPosition = Vector3.zero;
                 tsItem.localScale = Vector3.one;
                 tsItem.localRotation = Quaternion.identity;
                 Image img = tsItem.GetComponent<Image>();
                 img.sprite = item.ItemSprite != null ? item.ItemSprite : ItemModel.Instance.GetItemSprite(item.ItemId);
                 LocalizeTextMeshProUGUI tmp = tsItem.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                 // tmp.SetText($"x{item.ItemCount}");
                 var txt = CommonUtils.GetItemText(item.ItemId, item.ItemCount);
                 // DebugUtil.LogError($"txt:{txt}");
                 tmp.SetText(txt);
                 
                 DragonPlus.Config.TMatchShop.ItemConfig itemCfg = ItemModel.Instance.GetConfigById(item.ItemId);
                 tsItem.Find("Limit").gameObject.SetActive((ItemType)itemCfg.type == ItemType.TMEnergyInfinity);
                 tsItem.Find("LimitGroup").gameObject.SetActive(
                     (ItemType)itemCfg.type == ItemType.TMLightingInfinity ||
                     (ItemType)itemCfg.type == ItemType.TMClockInfinity);
                 

                 tsItem.gameObject.SetActive(true);
             }

             LayoutRebuilder.ForceRebuildLayoutImmediate(transItemRoot);
             transItemRoot.anchoredPosition = new Vector2(transItemRoot.anchoredPosition.x, -transItemRoot.rect.height / 2);

             _isAttachToArrow = isAttachToArrow;
             _offsetFinal = offset;

             if (autoCloseTime > 0f)
             {
                 StartCoroutine(CommonUtils.DelayWork(autoCloseTime, () =>
                 {
                     StartRemove = true;
                     StartCoroutine(DeleyTimeAction());
                 }));
             }
         }
         private float GetCanvasScale()
         {
             CanvasScaler mScaler = UIRoot.Instance.mRootCanvas.GetComponent<CanvasScaler>();
             return Math.Abs(mScaler.matchWidthOrHeight - 1) < 0.001f ? mScaler.referenceResolution.y / Screen.height : mScaler.referenceResolution.x / Screen.width;
         }

         private void autoAdapt()
         {
             UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(transRootImage);
             var screenScale = GetCanvasScale();
             var pos = RectTransformUtility.WorldToScreenPoint(UIRoot.Instance.mUICamera, transRootImage.transform.position) * screenScale;
             var rect = transRootImage.rect;

             var screenW = Screen.width * screenScale;
             float halfW = rect.size.x / 2;
             var left = pos.x + rect.center.x - halfW;
             var right = pos.x + rect.center.x + halfW;
             float _offsetX = 0;
             if (left < 0)
                 _offsetX = -left;
             if (right > screenW)
                 _offsetX = screenW - right;

             var screenH = Screen.height * screenScale;
             float halfH = rect.size.y / 2;
             var bottom = pos.y + rect.center.y - halfH;
             var top = pos.y + rect.center.y + halfH;
             float _offsetY = 0;
             if (bottom < 0)
                 _offsetY = -bottom;
             if (top > screenH)
                 _offsetY = screenH - top;

             transRootImage.anchoredPosition = new Vector2(transRootImage.anchoredPosition.x + _offsetX, transRootImage.anchoredPosition.y);
             transRoot.anchoredPosition = new Vector2(transRoot.anchoredPosition.x, transRoot.anchoredPosition.y + _offsetY);

             // transImageArrowUpLeft.anchoredPosition = new Vector2(transImageArrowUpLeft.anchoredPosition.x,
             //     transRootImage.rect.height - originHeight + originUpY);
             // transImageArrowUpMiddle.anchoredPosition = new Vector2(transImageArrowUpMiddle.anchoredPosition.x,
             //     transRootImage.rect.height - originHeight + originUpY);
             // transImageArrowUpRight.anchoredPosition = new Vector2(transImageArrowUpRight.anchoredPosition.x,
             //     transRootImage.rect.height - originHeight + originUpY);

             if (_isAttachToArrow)
             {
                 transRoot.anchoredPosition = new Vector2(transRoot.anchoredPosition.x, transRoot.anchoredPosition.y) + _offsetFinal;
             }
         }

         public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
         {
             var localpos = CommonUtils.ScreenToCanvasPos(transRootImage, sp);
             bool isClickImage = transRootImage.rect.Contains(localpos);
             if (Input.GetMouseButtonDown(0))
                 if (!isClickImage && !StartRemove)
                 {
                     StartRemove = true;
                     StartCoroutine(DeleyTimeAction());
                 }

             return isClickImage;
         }

         IEnumerator DeleyTimeAction()
         {
             yield return new WaitForEndOfFrame();

             CloseWindowWithinUIMgr(true);
         }
     }
 }