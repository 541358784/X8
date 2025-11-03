using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TrainOrder
{
    public class TrainOrderMergeItem : MonoBehaviour
    {
        public CommonRewardItem MergeItem => _mergeItem;
        public CommonRewardItem ExtraItem => _extraRewardItem;
        
        private TrainOrderItem _trainOrderItem;
        private TrainOrderOrder _order;
        private int _mergeItemId;
        private int _index;

        private Button _buttonServer;
        private Transform _tranFinish;
        private CommonRewardItem _mergeItem;

        private int _lastCount = 0;

        private Transform _tranExtra;
        private CanvasGroup _cgExtra;
        private CommonRewardItem _extraRewardItem;
        private LocalizeTextMeshProUGUI _textExtraTime;


        private void Awake()
        {
            _mergeItem = transform.GetComponentDefault<CommonRewardItem>();
            _buttonServer = transform.Find("Button").GetComponent<Button>();
            _buttonServer.onClick.AddListener(() =>
            {
                if (_order == null)
                    return;

                if (!TrainOrderModel.Instance.DebugComplete)
                {
                    Dictionary<int, int> mergeItemCounts =
                        MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.TrainOrder);

                    if (!mergeItemCounts.ContainsKey(_mergeItemId))
                        return;
                }
                
                TrainOrderModel.Instance.OrderItemComplete(_trainOrderItem, this, _order, _mergeItemId, _index);
            });
            _tranFinish = transform.Find("Finish");
            _tranExtra = transform.Find("TimeTask");
            _tranExtra.gameObject.SetActive(true);
            
            _cgExtra = _tranExtra.GetComponentDefault<CanvasGroup>();
            _cgExtra.alpha = 0;
            
            _extraRewardItem = _tranExtra.Find("Item").GetComponentDefault<CommonRewardItem>();
            _textExtraTime = _tranExtra.Find("TimeText").GetComponent<LocalizeTextMeshProUGUI>();

            EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_REFRESH, OnMergeBoardRefresh);


            InvokeRepeating(nameof(RefreshExtra), 0, 1);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_REFRESH, OnMergeBoardRefresh);
            CancelInvoke(nameof(RefreshExtra));
        }

        private void OnMergeBoardRefresh(BaseEvent e)
        {

            if (TrainOrderModel.Instance.DebugComplete)
            {
                RefreshView();
                return;
            }
            
            if (e.datas != null && e.datas.Length > 4)
            {
                MergeBoardEnum boardEnum = (MergeBoardEnum)e.datas[0];
                int mergeItemId = (int)e.datas[4];

                Dictionary<int, int> mergeItemCounts =
                    MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.TrainOrder);
                int tempCount = mergeItemCounts.ContainsKey(_mergeItemId) ? mergeItemCounts[_mergeItemId] : 0;

                if ((boardEnum == MergeBoardEnum.TrainOrder && _mergeItemId == mergeItemId) || tempCount < _lastCount)
                {
                    RefreshView();
                }
            }
        }


        public void Init(TrainOrderItem train, TrainOrderOrder order, int mergeItemId, int index)
        {
            _lastCount = 0;
            _trainOrderItem = train;
            _order = order;
            _mergeItemId = mergeItemId;
            _index = index;
            _mergeItem.Init(new ResData(_mergeItemId, 1),isShowClickTips:false);

            StorageTrainOrderItemState state = TrainOrderModel.Instance.GetOrderItemState(_order, _mergeItemId, _index);
            if (state != null && state.ExtraEndTime > 0)
                _extraRewardItem.Init(new ResData(_order.ExtraRewardId[_index],_order.ExtraRewardNum[_index]));

            RefreshView();
            RefreshExtra();
        }

        private void RefreshExtra()
        {
            _cgExtra.alpha = 0;
            if (_order == null)
                return;

            StorageTrainOrderItemState state = TrainOrderModel.Instance.GetOrderItemState(_order, _mergeItemId, _index);
            if (state == null || state.State == 1 || state.ExtraEndTime == 0)
                return;

            var left = (long)state.ExtraEndTime - (long)APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;

            _cgExtra.alpha = left > 0 ? 1 : 0;
            if (_cgExtra.alpha > 0)
            {
                _textExtraTime.SetText(CommonUtils.FormatLongToTimeStr(left));
            }
        }

        private void RefreshView()
        {
            if (_order == null)
                return;

            _buttonServer.gameObject.SetActive(false);
            _tranFinish.gameObject.SetActive(false);
            bool isOrderItemComplete = TrainOrderModel.Instance.IsOrderItemComplete(_order, _mergeItemId, _index);
            if (isOrderItemComplete)
            {
                _tranFinish.gameObject.SetActive(true);
            }
            else
            {
                Dictionary<int, int> mergeItemCounts =
                    MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.TrainOrder);

                if (mergeItemCounts.TryGetValue(_mergeItemId, out _lastCount)|| TrainOrderModel.Instance.DebugComplete)
                    _buttonServer.gameObject.SetActive(true);
                else
                    _buttonServer.gameObject.SetActive(false);
            }
        }
    }
}