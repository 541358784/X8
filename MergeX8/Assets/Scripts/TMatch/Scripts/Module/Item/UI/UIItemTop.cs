using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
// using Hospital.Game;
// using Hospital.UI;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TMatch
{


    /// <summary>
    /// 物品顶层
    /// 用来播放动画，放置ItemBar等等
    /// </summary>
    public class UIItemTop : UIWindowController
    {
        public static ulong Version;

        private class ItemInfo
        {
            private int count = 5;
            private Image icon;
            private float speed = 6f;
            private Sequence sequence;

            public bool Using;
            public GameObject Item;
            private GameObject effect;
            public int Id;

            public void SetId(int id, bool useEffect = true)
            {
                Id = id;
                if (icon == null) icon = Item.GetComponent<Image>();
                icon.sprite = ItemModel.Instance.GetItemSprite(id);
                icon.color = Color.white;
                string effectName = $"vfx_item_trail_0";
                Vector2 size = new Vector2(70, 70);
                // switch ((ResourceId) id)
                // {
                //     case ResourceId.Gem:
                //         effectName = $"vfx_diamond_trail_0";
                //         size = new Vector2(90, 90);
                //         break;
                //     case ResourceId.Coin:
                //         effectName = $"vfx_coin_trail_0";
                //         break;
                //     case ResourceId.Booster1:
                //     case ResourceId.Booster2:
                //     case ResourceId.Booster3:
                //         size = new Vector2(90, 90);
                //         break;
                // }

                icon.GetComponent<RectTransform>().sizeDelta = size;

                if (useEffect)
                {
                    if (effect != null)
                    {
                        effect.SetActive(true);
                        return;
                    }

                    effect = ResourcesManager.Instance.LoadResource<GameObject>($"Module/Item/Prefabs/{effectName}");
                    if (effect != null)
                    {
                        effect = Instantiate(effect, Item.transform, false);
                        effect.transform.localPosition = Vector3.zero;
                        effect.transform.localScale = Vector3.one;
                    }
                }
                else
                {
                    effect?.SetActive(false);
                }
            }

            public void SetItem(GameObject item)
            {
                Item = Instantiate(item);
            }

            public void SetParent(Transform parent)
            {
                Item.transform.SetParent(parent, false);
            }

            public void FlyObjOrgin(Transform from, Transform to, int index, Action onComplete, Action onArrive = null)
            {
                icon.sprite = from.GetComponent<Image>().sprite;
                Vector2 size = from.GetComponent<RectTransform>().sizeDelta;
                RectTransform rect = Item.GetComponent<RectTransform>();
                rect.sizeDelta = size;
                Vector3 startPos = from.position;
                Transform parent = Item.transform.parent;
                Item.transform.SetParent(from);
                Item.transform.localPosition = Vector3.zero;
                Item.transform.localRotation = Quaternion.identity;
                Item.transform.localScale = Vector3.one;
                Vector3 scale = Item.transform.localScale;
                Vector3 endPos = to.position;
                float duration = Math.Abs(endPos.y - startPos.y);
                Vector3 disPos = (endPos + startPos) / 2;
                Vector3 controlPos = disPos;
                float dx = Random.Range(3f, 8f) * duration / 10;
                float dy = Random.Range(3f, 8f) * duration / 10;
                if (index % 2 == 0)
                {
                    dx = -dx;
                    dy = -dy;
                }

                dx = -dx;
                dy = -dy;
                controlPos.x += dx;
                controlPos.y += dy;
                Vector3[] path = CommonUtils.Bezier2Path(startPos, controlPos, endPos, count);
                duration = 0f;
                float tempSpeed = speed;
                for (int i = 1; i < path.Length; i++)
                {
                    duration += Vector3.Distance(path[i - 1], path[i]) / tempSpeed;
                }

                if (duration > 1.5f) duration *= 0.8f;
                sequence = DOTween.Sequence();
                Sequence sq1 = DOTween.Sequence();
                sq1.Append(Item.transform.DOPath(path, duration, PathType.CatmullRom).SetDelay(index * 0.08f + 0.5f));
                Sequence sq2 = DOTween.Sequence();
                sq2.Append(rect.DOScale(0.8f, duration * 0.5f).SetDelay(index * 0.05f + 0.5f).OnComplete(() =>
                {
                    Item.transform.SetParent(parent);
                }));
                sq1.SetEase(Ease.Linear);
                sq2.SetEase(Ease.Linear);
                sequence.Append(sq1);
                sequence.Join(sq2);
                sequence.onComplete = () =>
                {
                    sequence = null;
                    onArrive?.Invoke();
                    onComplete?.Invoke();
                };
            }

            public void FlyObj(Transform from, Transform to, int index, Action onComplete, Action onArrive = null)
            {
                Vector3 startPos = from.position;
                startPos.x += Random.Range(-1.5f, 1.5f);
                startPos.y += Random.Range(-0.5f, 0.5f);
                Item.transform.position = startPos;
                Vector3 endPos = to.position;
                float duration = Math.Abs(endPos.y - startPos.y);
                Vector3 disPos = (endPos + startPos) / 2;
                Vector3 controlPos = disPos;
                float dx = Random.Range(3f, 8f) * duration / 10;
                float dy = Random.Range(3f, 8f) * duration / 10;
                // if (index % 2 == 0)
                // {
                //     dx = -dx;
                //     dy = -dy;
                // }
                dx = -dx;
                dy = -dy;

                controlPos.x += dx;
                controlPos.y += dy;
                Vector3[] path = CommonUtils.Bezier2Path(startPos, controlPos, endPos, count);
                duration = 0f;
                float tempSpeed = speed * 2;
                for (int i = 1; i < path.Length; i++)
                {
                    duration += Vector3.Distance(path[i - 1], path[i]) / tempSpeed;
                }

                if (duration > 1.5f) duration *= 0.8f;
                Item.transform.localScale = new Vector3(0f, 0f, 1f);
                sequence = DOTween.Sequence();
                var rect = Item.GetComponent<RectTransform>();
                Vector2 pos = rect.anchoredPosition;
                Sequence sq1 = DOTween.Sequence();
                sq1.Append(rect.DOAnchorPosY(pos.y + 60, 0.16f).SetDelay(index * 0.05f).SetEase(Ease.Linear));
                sq1.Append(rect.DOAnchorPosY(pos.y + 10, 0.42f).SetEase(Ease.Linear));
                sq1.Append(rect.DOAnchorPosY(pos.y, 0.5f).SetEase(Ease.Linear));
                Sequence sq2 = DOTween.Sequence();
                sq2.Append(rect.DOScale(Vector3.one * 1.15f, 0.16f).SetDelay(index * 0.05f).SetEase(Ease.Linear));
                sq2.Append(rect.DOScale(Vector3.one, 0.42f).SetEase(Ease.Linear));
                sq1.SetEase(Ease.Linear);
                sq2.SetEase(Ease.Linear);
                sequence.Append(sq1);
                sequence.Join(sq2);
                sequence.AppendInterval(0.5f).SetEase(Ease.Linear);

                sequence.Append(Item.transform.DOPath(path, duration, PathType.CatmullRom).OnComplete(() =>
                {
                    icon.color = new Color(1, 1, 1, 0);
                    onArrive?.Invoke();
                }));
                sequence.Join(Item.transform.DOScale(0.8f, duration));
                sequence.AppendInterval(0.5f);
                sequence.onComplete = () =>
                {
                    sequence = null;
                    onComplete?.Invoke();
                };
            }

            public void Fly(Transform from, Transform to, int index, Action onComplete, Action onArrive = null)
            {
                Vector3 startPos = from.position;
                startPos.x += Random.Range(-0.5f, 0.5f);
                startPos.y += Random.Range(-0.5f, 0.5f);
                Item.transform.position = startPos;
                Vector3 endPos = to.position;
                float duration = Math.Abs(endPos.y - startPos.y);
                Vector3 disPos = (endPos + startPos) / 2;
                Vector3 controlPos = disPos;
                float dx = Random.Range(3f, 5f) * duration / 10;
                float dy = Random.Range(3f, 5f) * duration / 10;
                if (index % 2 == 0)
                {
                    dx = -dx;
                    dy = -dy;
                }

                controlPos.x += dx;
                controlPos.y += dy;
                Vector3[] path = CommonUtils.Bezier2Path(startPos, controlPos, endPos, count);
                duration = 0f;
                for (int i = 1; i < path.Length; i++)
                {
                    duration += Vector3.Distance(path[i - 1], path[i]) / speed;
                }

                if (duration > 1.5f) duration *= 0.8f;
                Item.transform.localScale = new Vector3(0f, 0f, 1f);
                sequence = DOTween.Sequence();
                sequence.Append(Item.transform.DOScale(new Vector3(1.15f, 1.15f, 1f), 0.3f).SetDelay(index * 0.05f));
                sequence.Append(Item.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
                sequence.Append(Item.transform.DOPath(path, duration, PathType.CatmullRom));
                sequence.onComplete = () =>
                {
                    sequence = null;
                    onArrive?.Invoke();
                    onComplete?.Invoke();
                };
            }

            public void Clear()
            {
                if (sequence != null)
                {
                    sequence.Kill();
                    sequence = null;
                }
            }
        }

        private bool isFirst;
        private ItemBar left1;
        private ItemBar left2;
        private ItemBar right;
        private Transform root;
        private Transform hide;
        private GameObject item;
        private GameObject mask;
        private GameObject placeHold;
        private GameObject placeHoldLeft;
        private RectTransform top;
        private Tweener tweener;
        private Tweener itemTween;
        private readonly List<ItemInfo> items = new List<ItemInfo>();
        private RectTransform transBooster;
        private bool isShowBoosterFly;
        private float preCloseBoosterFlyTimer;
        public override bool IsUsedInTaskChokedEvent { get; } = false;

        public override UIWindowLayer WindowLayer => UIWindowLayer.NormalTop;
        public override UIWindowType WindowType => UIWindowType.Fixed;

        private void Update()
        {
            CloseItemFly();
        }

        private void CloseItemFly()
        {
            if (!isShowBoosterFly) return;
            if (preCloseBoosterFlyTimer == -1) return;
            preCloseBoosterFlyTimer -= Time.deltaTime;
            if (preCloseBoosterFlyTimer > 0) return;
            PlayBoosterAnimation(false);
        }

        private ItemInfo GetOrAddItemInfo(int id)
        {
            ItemInfo itemInfo;
            for (int i = 0; i < items.Count; i++)
            {
                itemInfo = items[i];
                if (!itemInfo.Using && itemInfo.Id == id)
                {
                    return itemInfo;
                }
            }

            itemInfo = new ItemInfo();
            itemInfo.SetItem(item);
            items.Add(itemInfo);
            return itemInfo;
        }

        private void ReleaseItemInfo(ItemInfo itemInfo)
        {
            itemInfo.Using = false;
            itemInfo.SetParent(hide);
        }

        private void OnOpenUIWindow(BaseEvent evt)
        {
            UIWindow ui = evt.datas[0] as UIWindow;
            if (ui == null || !ui.EffectUIAnimation) return;
            PlayAnimation();
        }

        private void OnCloseUIWindow(BaseEvent evt)
        {
            UIWindow ui = evt.datas[0] as UIWindow;
            if (ui == null || !ui.EffectUIAnimation) return;
            PlayAnimation();
        }

        public override void PrivateAwake()
        {
            EffectUIAnimation = false;

            top = GetItem<RectTransform>("Root/Top");
            item = GetItem("Root/Hide/ItemFly");
            mask = GetItem("Root/Mask");
            root = GetItem<Transform>("Root");
            hide = GetItem<Transform>("Root/Hide");
            left1 = ItemBar.LoadItemBar(GetItem<Transform>("Root/Top/LayoutLeft/Left1"));
            left2 = ItemBar.LoadItemBar(GetItem<Transform>("Root/Top/LayoutLeft/Left2"));
            right = ItemBar.LoadItemBar(GetItem<Transform>("Root/Top/Layout/Right"));
            placeHold = GetItem("Root/Top/Layout/PlaceHold");
            placeHoldLeft = GetItem("Root/Top/LayoutLeft/PlaceHold");
            items.Add(new ItemInfo() {Item = item});

            GuideBar = GetItem<Transform>("Root/Top/Layout/Right");
            GuideIcon = GetItem<Transform>("Root/Top/Layout/Right/GuideIcon");
            transBooster = transform.Find($"Root/RB/Boosters").GetComponent<RectTransform>();
            // foreach (var cfgBooster in Hospital.Config.MapCommon.ConfigManager.Instance.BoosterList)
            // {
            //     var entityBooster = new EntityBooster(cfgBooster);
            //     HospitalItem.UIItemBooster.Create(transBooster, entityBooster);
            // }

            isPlayDefaultOpenAudio = false;
            isPlayDefaultCloseAudio = false;

            var sgs = GetComponentsInChildren<SkeletonGraphic>();
            if (sgs != null)
            {
                foreach (var sg in sgs)
                {
                    sg.updateWhenInvisible = UpdateMode.Nothing;
                }
            }
        }

        protected override void OnOpenWindow(UIWindowData data)
        {
            base.OnOpenWindow(data);

            isFirst = true;

            ItemFlyModel.Instance.UIItemTop = this;
            ItemBarModel.Instance.UIItemTop = this;

            EventDispatcher.Instance.AddEventListener(EventEnum.OpenUIWindow, OnOpenUIWindow);
            EventDispatcher.Instance.AddEventListener(EventEnum.CloseUIWindow, OnCloseUIWindow);
        }

        protected override void OnCloseWindow(bool destroy = true)
        {
            base.OnCloseWindow(destroy);

            left1.Clear();
            left2.Clear();
            right.Clear();
            ItemFlyModel.Instance.UIItemTop = null;
            ItemBarModel.Instance.UIItemTop = null;
            if (tweener != null)
            {
                tweener.Kill();
                tweener = null;
            }

            itemTween?.Kill();

            foreach (var itemInfo in items)
            {
                itemInfo.Clear();
            }

            EventDispatcher.Instance.RemoveEventListener(EventEnum.OpenUIWindow, OnOpenUIWindow);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.CloseUIWindow, OnCloseUIWindow);
        }

        public void PlayItemFlyAnimation(int id, int count, Transform from, Transform to, Action onArrive,
            Action onComplete,
            ItemFlyModel.ItemFlyType flyType = ItemFlyModel.ItemFlyType.Default, bool useEffect = true,
            bool useAudio = true)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (i > 5) break;

                    ItemInfo itemInfo = GetOrAddItemInfo(id);
                    itemInfo.Using = true;
                    itemInfo.SetId(id, useEffect);
                    itemInfo.SetParent(root);
                    bool isLast = i == count - 1 || i == 5;
                    bool isFirst = i == 0;
                    Action<Transform, Transform, int, Action, Action> action = itemInfo.Fly;
                    switch (flyType)
                    {
                        case ItemFlyModel.ItemFlyType.Default:
                            break;
                        case ItemFlyModel.ItemFlyType.PiggyBank:
                            action = itemInfo.FlyObj;
                            break;
                        case ItemFlyModel.ItemFlyType.PlayOrginImage:
                            action = itemInfo.FlyObjOrgin;
                            break;
                    }

                    action?.Invoke(from, to, i, () =>
                    {
                        ReleaseItemInfo(itemInfo);
                        if (useAudio)
                        {
                            if (isFirst)
                            {
                                // switch ((ResourceId) id)
                                // {
                                //     case ResourceId.Coin:
                                //         AudioManager.Instance.PlaySound(SfxNameConst.UICoin);
                                //         break;
                                //     case ResourceId.Gem:
                                //         AudioManager.Instance.PlaySound(SfxNameConst.UIDiamond);
                                //         break;
                                // }
                            }
                        }

                        if (isLast)
                        {
                            onComplete?.Invoke();
                            if (isShowBoosterFly)
                            {
                                // switch ((ResourceId) id)
                                // {
                                //     case ResourceId.Booster1:
                                //     case ResourceId.Booster2:
                                //     case ResourceId.Booster3:
                                //         preCloseBoosterFlyTimer = 0.2f;
                                //         break;
                                // }
                            }
                        }
                    }, onArrive);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.Message + e.StackTrace);
                onComplete?.Invoke();
                if (isShowBoosterFly)
                {
                    // switch ((ResourceId) id)
                    // {
                    //     case ResourceId.Booster1:
                    //     case ResourceId.Booster2:
                    //     case ResourceId.Booster3:
                    //         preCloseBoosterFlyTimer = 0.2f;
                    //         break;
                    // }
                }

                mask.SetActive(false);
            }

            if (useAudio) AudioManager.Instance.PlaySound(SfxNameConst.UIFly);
        }

        public void RefreshItemBar(ItemBar.Data left1, ItemBar.Data left2, ItemBar.Data right,
            ItemBarModel.ItemBarStyle style)
        {
            if (isFirst) PlayAnimation();
            this.left1.SetData(left1);
            this.left2.SetData(left2);
            this.right.SetData(right);

            placeHold.SetActive(style == ItemBarModel.ItemBarStyle.RightPlaceHold ||
                                style == ItemBarModel.ItemBarStyle.PlaceHold);
            placeHoldLeft.SetActive(style == ItemBarModel.ItemBarStyle.LeftPlaceHold ||
                                    style == ItemBarModel.ItemBarStyle.PlaceHold);

            Version++;
        }

        public void PlayAnimation()
        {
            if (tweener != null) return;
            top.anchoredPosition = new Vector2(0, 220);
            tweener = top.DOAnchorPosY(0, 0.2f);
            tweener.onComplete = () => { tweener = null; };
            isFirst = false;
        }

        /// <summary>
        /// 物品目标点动画
        /// </summary>
        /// <param name="isEnter"></param>
        public void PlayBoosterAnimation(bool isEnter, Action onComplete = null)
        {
            itemTween?.Kill();
            if (transBooster == null)
            {
                isShowBoosterFly = isEnter;
                preCloseBoosterFlyTimer = -1;
                itemTween = null;
                onComplete?.Invoke();
                return;
            }

            itemTween = transBooster.DOAnchorPosY(isEnter ? 0 : -250, isEnter ? 0.2f : 0.5f);
            StartCoroutine(CommonUtils.DelayCall(0.2f, () =>
            {
                isShowBoosterFly = isEnter;
                preCloseBoosterFlyTimer = -1;
                itemTween = null;
                onComplete?.Invoke();
            }));
        }

        /// <summary>
        /// 获取ItemBar
        /// </summary>
        /// <param name="id">资源id</param>
        /// <returns></returns>
        public Transform GetItemBar(int id)
        {
            if (left1.Id == id)
            {
                return left1.transform;
            }

            if (left2.Id == id)
            {
                return left2.transform;
            }

            return right.Id == id ? right.transform : null;
        }

        /// <summary>
        /// 获取右边ItemBar
        /// </summary>
        /// <returns></returns>
        public Transform GetRightBar()
        {
            return right.transform;
        }

        /// <summary>
        /// 获取左边ItemBar
        /// </summary>
        /// <returns></returns>
        public Transform GetLeftBar()
        {
            return left1.transform;
        }

        #region 引导相关

        public Transform GuideBar;
        public Transform GuideIcon;

        #endregion

        public static void Open()
        {
            UIManager.Instance.OpenWindow<UIItemTop>(GlobalPrefabPath.UIItemTop);
        }

        public static UIItemTop Get()
        {
            return UIManager.Instance.GetOpenedWindow<UIItemTop>();
        }
    }
}