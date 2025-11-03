using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TrainOrder
{
    public class TrainOrderItem : MonoBehaviour
    {
        public CommonRewardItem RewardItem => _orderRewardItem;

        private TrainOrderOrder _order;

        private Transform _root;
        private Transform _bgRoot;
        private Dictionary<int, Transform> _bgDic = new Dictionary<int, Transform>();

        private Transform _mergeItemRoot;
        private Transform _mergeItem;

        private CommonRewardItem _orderRewardItem;


        private List<TrainOrderMergeItem> _mergeItems = new List<TrainOrderMergeItem>();


        private RectTransform _rectSelf;

        private void Awake()
        {
            _rectSelf = transform.GetComponent<RectTransform>();
            _root = transform.Find("Viewport/Content");
            _bgRoot = _root.Find("BG");
            for (var i = 0; i < _bgRoot.childCount; i++)
            {
                Transform t = _bgRoot.GetChild(i);
                _bgDic.Add(int.Parse(t.gameObject.name), t);
                t.gameObject.SetActive(false);
            }

            _mergeItemRoot = _root.Find("ItemGroup");
            _mergeItem = _root.Find("ItemGroup/Item");
            _mergeItem.gameObject.SetActive(false);

            _orderRewardItem = _root.Find("Reward/Item").GetComponentDefault<CommonRewardItem>();
        }


        public void Init(TrainOrderOrder order, bool fromOutSide = false)
        {
            _order = order;
            _orderRewardItem.Init(new ResData(_order.OrderReward[0], _order.OrderReward[1]));

            if (_bgDic.TryGetValue(_order.MergeItemId.Count, out Transform bg))
                bg.gameObject.SetActive(true);

            InitMergeItems();

            //已完成
            if (TrainOrderModel.Instance.IsOrderComplete(_order))
                TrainMoveOut(0f);
            else
            {
                if (fromOutSide)
                {
                    _rectSelf.anchoredPosition = new Vector2(1000f, _rectSelf.anchoredPosition.y);
                    _rectSelf.DOAnchorPosX(0f, 1);
                }
                else
                {
                    _rectSelf.anchoredPosition = Vector2.zero;
                }
            }
        }

        private void InitMergeItems()
        {
            for (var i = 0; i < _order.MergeItemId.Count; i++)
            {
                int itemId = _order.MergeItemId[i];
                Transform tran = Instantiate(_mergeItem, _mergeItemRoot);
                tran.gameObject.SetActive(true);
                TrainOrderMergeItem item = tran.GetComponentDefault<TrainOrderMergeItem>();
                item.Init(this, _order, itemId, i);
                _mergeItems.Add(item);
            }
        }


        public async Task TrainMoveOut(float duration = 1f)
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();

            _rectSelf.DOAnchorPosX(-1000f, duration).OnComplete(() => { waitTask.SetResult(true); });

            await waitTask.Task;
        }
    }
}