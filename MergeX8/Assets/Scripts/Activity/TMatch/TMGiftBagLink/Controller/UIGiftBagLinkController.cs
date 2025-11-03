using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace TMatch
{
    public class UIGiftBagLinkController : UIWindowController
    {
        private Button closeButton;
        private List<Vector3> orgLoaclPosition = new List<Vector3>();
        private List<UIGiftBagLinkItem> _giftBagLinkItems = new List<UIGiftBagLinkItem>();
        private LocalizeTextMeshProUGUI countDown;
        private List<TMGiftBagLinkResourceConfig> _linkResources = null;
        private UIGiftBagLinkItem _curGiftBagData;
        private Coroutine unLockNextCoroutine = null;
        private bool isClose = false;

        private static string coolTimeKey = "TMGiftBagLink";

        public override void PrivateAwake()
        {
            TMGiftBagLinkModel.Instance.UpdateActivityState();

            countDown = transform.Find("Root/TopGroup/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();

            closeButton = transform.Find("Root/TopGroup/CloseButton").GetComponent<Button>();
            closeButton.onClick.AddListener(OnBtnClose);

            for (int i = 1; i < 8; i++)
            {
                Transform item = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content/RewardItem" + i);
                UIGiftBagLinkItem script = item.gameObject.AddComponent<UIGiftBagLinkItem>();
                item.gameObject.SetActive(false);

                _giftBagLinkItems.Add(script);
                orgLoaclPosition.Add(item.transform.localPosition);
            }

            EventDispatcher.Instance.AddEventListener(EventEnum.TM_GIFTBAGLINK_PURCHASE_REFRESH, PurchseRefresh);
            EventDispatcher.Instance.AddEventListener(EventEnum.TM_GIFTBAGLINK_GET_REWARD, GiftBagKinkGetReward);
        }

        protected override void OnOpenWindow(UIWindowData data)
        {
            _linkResources = TMGiftBagLinkModel.Instance.GetGiftBagLinkResources();
            if (_linkResources == null)
                return;

            int index = TMGiftBagLinkModel.Instance.GetCurIndex();
            for (int i = index; i < index + 7; i++)
            {
                if (i >= _linkResources.Count)
                    break;

                int rlIndex = i - index;
                _giftBagLinkItems[rlIndex].gameObject.SetActive(true);
                _giftBagLinkItems[rlIndex].UpdateData(_linkResources[i], i);
            }

            InvokeRepeating("UpdateCountDownTime", 0, 1);

            // CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
            // CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(false);
        }

        private void OnBtnClose()
        {
            isClose = true;
            StartCoroutine(CommonUtils.PlayAnimation(transform.GetComponent<Animator>(), UIAnimationConst.DisAppear, null,
                () => { CloseWindowWithinUIMgr(true); }));
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_GIFTBAGLINK_PURCHASE_REFRESH, PurchseRefresh);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_GIFTBAGLINK_GET_REWARD, GiftBagKinkGetReward);
            CancelInvoke("UpdateCountDownTime");
            CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(true);
        }

        private void RewardPopup(BaseEvent baseEvent)
        {
            //CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
        }

        private void UpdateCountDownTime()
        {
            countDown.SetText(TMGiftBagLinkModel.Instance.GetActivityLeftTimeString());
        }

        private void GiftBagKinkGetReward(BaseEvent e)
        {
            if (isClose)
                return;

            if (_linkResources == null)
                return;

            if (e == null || e.datas == null || e.datas.Length < 1)
                return;

            _curGiftBagData = (UIGiftBagLinkItem)e.datas[0];

            if (_curGiftBagData.index < TMGiftBagLinkModel.Instance.GetCurIndex())
                return;

            if (unLockNextCoroutine != null)
                return;

            switch (_curGiftBagData.ConsumeType)
            {
                case 1:
                {
                    int index = 0;
                    for (int i = 0; i < _curGiftBagData.giftBagLinkResource.RewardID.Count; i++)
                    {
                        int id = _curGiftBagData.giftBagLinkResource.RewardID[i];
                        int num = _curGiftBagData.giftBagLinkResource.Amount[i];

                        FlySystem.Instance.FlyItem(id,
                            num,
                            Vector2.zero, 
                            FlySystem.Instance.GetTargetTransform(id).position,
                            () =>
                            {
                                index ++;
                                if (index == _curGiftBagData.giftBagLinkResource.RewardID.Count)
                                {
                                    unLockNextCoroutine = StartCoroutine(UnLockNext(_curGiftBagData.index));
                                }
                            });
                        
                        ItemModel.Instance.Add(id, num, new DragonPlus.GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GiftBagLinkGetTm,
                        });
                    }

                    TMGiftBagLinkModel.Instance.AddCurIndex();
                    break;
                }
                case 2:
                {
                    StoreModel.Instance.Purchase(_curGiftBagData.giftBagLinkResource.ConsumeAmount);
                    break;
                }
            }
        }

        private void PurchseRefresh(BaseEvent e)
        {
            if (_curGiftBagData == null)
                return;

            if (e == null || e.datas == null || e.datas.Length < 1)
                return;

            int index = (int)e.datas[0];

            if (_curGiftBagData.index != index)
                return;
            if (unLockNextCoroutine != null)
                return;

            unLockNextCoroutine = StartCoroutine(UnLockNext(_curGiftBagData.index));
        }

        private IEnumerator UnLockNext(int index)
        {
            if ((_curGiftBagData.index + 1) >= _linkResources.Count)
            {
                CloseWindowWithinUIMgr(true);
                EventDispatcher.Instance.DispatchEvent(EventEnum.TM_GIFTBAGLINK_OPEN_REFRESH);
                yield break;
            }

            UIGiftBagLinkItem linkItem = _giftBagLinkItems[0];
            _giftBagLinkItems.Remove(linkItem);
            linkItem.CompleteAnim();
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < _giftBagLinkItems.Count; i++)
            {
                if (!_giftBagLinkItems[i].gameObject.activeSelf)
                    continue;

                int moveIndex = i;
                _giftBagLinkItems[i].transform.DOLocalMove(orgLoaclPosition[i], 0.5f).OnComplete(() =>
                {
                    if (moveIndex == 0)
                        _giftBagLinkItems[moveIndex].UnLockAnim();
                });

                if (i == 0)
                    _giftBagLinkItems[i].ChangeAnim();

                yield return new WaitForSeconds(0.2f);
            }

            _giftBagLinkItems[0].UpdateStatus(true);
            yield return new WaitForSeconds(0.5f);

            linkItem.gameObject.SetActive(false);
            linkItem.gameObject.transform.localPosition = orgLoaclPosition[orgLoaclPosition.Count - 1];
            _giftBagLinkItems.Add(linkItem);

            _curGiftBagData = null;

            int curIndex = TMGiftBagLinkModel.Instance.GetCurIndex();
            for (int i = curIndex; i < curIndex + 7; i++)
            {
                if (i >= _linkResources.Count)
                    break;

                int rlIndex = i - curIndex;
                if (rlIndex == 6)
                {
                    _giftBagLinkItems[rlIndex].gameObject.SetActive(true);
                    _giftBagLinkItems[rlIndex].UpdateData(_linkResources[i], i);
                }
                else
                {
                    _giftBagLinkItems[rlIndex].UpdateIndex(i);
                }
            }

            unLockNextCoroutine = null;
        }

        public static bool CanShowUI()
        {
            if (!TMGiftBagLinkModel.Instance.CanShowUI())
                return false;

            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
                return false;

            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
                CommonUtils.GetTimeStamp());

            GiftBagLinkEntranceView.Open();
            return true;
        }
    }
}