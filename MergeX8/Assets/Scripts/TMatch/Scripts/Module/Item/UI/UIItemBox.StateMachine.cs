    using System;
    using System.Collections;
    // using CustomStateMachine;
    using DG.Tweening;
    using DragonPlus;
    using DragonU3DSDK;
    using UnityEngine;
    using UnityEngine.UI;

    namespace TMatch
    {


        public partial class UIItemBox
        {
            private class UIItemBox_InitState : CustomFSMState<UIItemBox>
            {
                public UIItemBox_InitState(UIItemBox owner, HierarchicalStateMachine hsm) : base(owner, hsm)
                {
                }

                private bool isComplete;

                public override void OnEnter()
                {
                    base.OnEnter();
                    isComplete = false;
                    owner.InitComponent();
                }

                public override void Update()
                {
                    base.Update();
                    if (isComplete) return;
                    isComplete = true;
                    if (owner.data == null)
                    {
                        DebugUtil.LogError($"ItemBox paramdata is null");
                        return;
                    }

                    for (int i = 0; i < owner.boxGroup.childCount; i++)
                    {
                        owner.boxGroup.GetChild(i).gameObject.SetActive(i == (int) owner.data.boxType);
                        owner.boxTempGroup.GetChild(i).gameObject.SetActive(i == (int) owner.data.boxType);
                    }
                    // switch (owner.data.boxType)
                    // {
                    //     case BoxType.Blue:
                    //     case BoxType.Purple:
                    //         owner.box.gameObject.SetActive(true);
                    //         owner.boxRed.gameObject.SetActive(false);
                    //         owner.tempBox.gameObject.SetActive(true);
                    //         owner.tempRedBox.gameObject.SetActive(false);
                    //         owner.box.Skeleton.SetSkin($"box{(int) owner.data.boxType + 1}");
                    //         owner.tempBox.Skeleton.SetSkin($"box{(int) owner.data.boxType + 1}");
                    //         break;
                    //     case BoxType.Red:
                    //         owner.box.gameObject.SetActive(false);
                    //         owner.boxRed.gameObject.SetActive(true);
                    //         owner.tempBox.gameObject.SetActive(false);
                    //         owner.tempRedBox.gameObject.SetActive(true);
                    //         break;
                    //     default:
                    //         throw new ArgumentOutOfRangeException();
                    // }

                    hsm.ChangeState(owner.data.boxFlyOrgin == null
                        ? nameof(UIItemBox_PreOpenState)
                        : nameof(UIItemBox_FlyState));
                }
            }

            private class UIItemBox_IdleState : CustomFSMState<UIItemBox>
            {
                public UIItemBox_IdleState(UIItemBox owner, HierarchicalStateMachine hsm) : base(owner, hsm)
                {
                }
            }

            private class UIItemBox_FlyState : CustomFSMState<UIItemBox>
            {
                public UIItemBox_FlyState(UIItemBox owner, HierarchicalStateMachine hsm) : base(owner, hsm)
                {
                }

                private float timer = 0;

                private Transform _box;
                private Transform _boxTemp;

                public override void OnEnter()
                {
                    base.OnEnter();
                    timer = 0.7f;
                    for (int i = 0; i < owner.boxGroup.childCount; i++)
                    {
                        if ((int) owner.data.boxType != i) continue;
                        _box = owner.boxGroup.GetChild(i);
                        _boxTemp = owner.boxTempGroup.GetChild(i);
                    }

                    // switch (owner.data.boxType)
                    // {
                    //     case BoxType.Blue:
                    //     case BoxType.Purple:
                    //         _box = owner.box.transform;
                    //         _boxTemp = owner.tempBox.transform;
                    //         break;
                    //     case BoxType.Red:
                    //         _box = owner.boxRed.transform;
                    //         _boxTemp = owner.tempRedBox.transform;
                    //         break;
                    //     default:
                    //         throw new ArgumentOutOfRangeException();
                    // }
                    _box.gameObject.SetActive(false);
                    _boxTemp.transform.position = owner.data.boxFlyOrgin.Value;
                    _boxTemp.transform.localScale = Vector3.one * 0.1f;
                    Vector2 controlPos = new Vector2(_boxTemp.transform.position.x + 4.5f,
                        _boxTemp.transform.position.y + 2f);
                    Sequence sq = DOTween.Sequence();
                    sq.Append(CommonUtils.FlyObj(_boxTemp.transform, _box.transform.position, timer, controlPos));
                    sq.Join(_boxTemp.transform.DOScale(1, timer).SetEase(Ease.InCirc));
                    sq.Join(_boxTemp.transform.DOScale(_box.transform.localScale, 0.1f).SetEase(Ease.OutBack));
                    sq.OnComplete(() =>
                    {
                        _box.gameObject.SetActive(true);
                        _boxTemp.gameObject.SetActive(false);
                        hsm.ChangeState(nameof(UIItemBox_PreOpenState));
                    });
                }
            }

            private class UIItemBox_PreOpenState : CustomFSMState<UIItemBox>
            {
                public UIItemBox_PreOpenState(UIItemBox owner, HierarchicalStateMachine hsm) : base(owner, hsm)
                {
                }

                public override void OnEnter()
                {
                    base.OnEnter();
                    if (owner.data.ItemIds == null || owner.data.ItemNums == null || owner.data.ItemIds.Count <= 0 ||
                        owner.data.ItemNums.Count <= 0)
                    {
                        DebugUtil.LogError($"itembox itemdata is null");
                        return;
                    }

                    AudioManager.Instance.PlaySound(SfxNameConst.UIChest1);
                    owner.animator.Play($"appear_03");
                    owner.StartCoroutine(CommonUtils.DelayCall(1,
                        () => { hsm.ChangeState(nameof(UIItemBox_OpenState)); }));
                }
            }

            private class UIItemBox_OpenState : CustomFSMState<UIItemBox>
            {
                public UIItemBox_OpenState(UIItemBox owner, HierarchicalStateMachine hsm) : base(owner, hsm)
                {
                }

                public override void OnEnter()
                {
                    base.OnEnter();
                    // float width = owner.data.ItemIds.Count * owner.layout.cellSize.x + owner.layout.spacing.x *
                    //     (owner.data.ItemIds.Count > 0 ? owner.data.ItemIds.Count - 1 : 0);
                    // float rate = owner.rect.width / width;
                    // if (rate > 1) rate = 1;
                    // owner.layout.cellSize *= rate;
                    // owner.layout.spacing *= rate;
                    for (int i = 0; i < owner.layout.transform.childCount; i++)
                    {
                        owner.layout.transform.GetChild(i).gameObject.SetActive(false);
                    }

                    owner.StartCoroutine(DelayCreate());
                }

                private IEnumerator DelayCreate()
                {
                    yield return new WaitForEndOfFrame();
                    for (int i = 0; i < owner.data.ItemIds.Count; i++)
                    {
                        var item = owner.layout.transform.childCount <= i
                            ? UnityEngine.Object.Instantiate(owner.itemTemp, owner.layout.transform, false)
                            : owner.layout.transform.GetChild(i).gameObject;
                        Image icon = item.FindChild("Icon").GetComponent<Image>();
                        if (ItemModel.Instance.GetConfigById(owner.data.ItemIds[i]) != null)
                        {
                            icon.sprite = ItemModel.Instance.GetItemSprite(owner.data.ItemIds[i], false);
                            DragonPlus.Config.TMatchShop.ItemConfig itemCfg = ItemModel.Instance.GetConfigById(owner.data.ItemIds[i]);
                            icon.transform.Find("Limit").gameObject.SetActive((ItemType)itemCfg.type == ItemType.TMEnergyInfinity);
                            icon.transform.Find("LimitGroup").gameObject.SetActive(
                                (ItemType)itemCfg.type == ItemType.TMLightingInfinity ||
                                (ItemType)itemCfg.type == ItemType.TMClockInfinity);
                        }
                        // else
                        //     // 这里有可能有MergeItem
                        //     icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(MergeConfigManager.Instance
                        //         .GetItemConfig(owner.data.ItemIds[i]).Image);

                        LocalizeTextMeshProUGUI text = item.FindChild("txtNum").GetComponent<LocalizeTextMeshProUGUI>();
                        text.SetText($"{CommonUtils.GetItemText(owner.data.ItemIds[i], owner.data.ItemNums[i])}");
                        item?.SetActive(true);
                        yield return new WaitForSeconds(0.3f);
                    }

                    if (!owner.touchButton.interactable) owner.touchButton.interactable = true;
                }
            }

            private class UIItemBox_CloseState : CustomFSMState<UIItemBox>
            {
                public UIItemBox_CloseState(UIItemBox owner, HierarchicalStateMachine hsm) : base(owner, hsm)
                {
                }

                public override void OnEnter()
                {
                    base.OnEnter();
                    owner.OnCollectReward();
                }
            }
        }
    }