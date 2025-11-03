using System;
using System.Collections.Generic;
using DragonU3DSDK;

namespace IAPChecker
{
    public partial class Model : Manager<Model>
    {
        /// <summary>
        /// 未完成的订单
        /// </summary>
        private readonly List<string> _unfinishedOrders = new List<string>();

        /// <summary>
        /// 当前未完成的订单
        /// </summary>
        private string UnfinishedOrderId => HasUnfinishedOrder ? _unfinishedOrders[0] : null;

        /// <summary>
        /// 是否有未完成的订单
        /// </summary>
        public bool HasUnfinishedOrder => _unfinishedOrders.Count > 0;

        /// <summary>
        /// 是否订单处理中
        /// </summary>
        public bool isOrderProcessing;

        private void Update()
        {
            if (isOrderProcessing)
                return;
            // var unfulfilledPayment = Dlugin.SDK.GetInstance().iapManager.CheckPendingPurchase();
            // if (unfulfilledPayment != null)
            //     AddUnfinishedOrder(unfulfilledPayment.ProductId);
        }

        // /// <summary>
        // /// 尝试补单
        // /// </summary>
        // private void TryToFinishOrder()
        // {
        //     if (UnfinishedOrderId == null)
        //         return;
        //
        //     isOrderProcessing = true;
        //     Dlugin.SDK.GetInstance().iapManager.RequestUnfulfilledPaymentsAndTryVerify(OnPurchased, UnfinishedOrderId);
        // }

        /// <summary>
        /// 补单回执
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="productId">商品id</param>
        /// <param name="product">商品</param>
        /// <param name="failureReason">失败原因</param>
        private void OnPurchased(bool success, string productId, UnityEngine.Purchasing.Product product,
            UnityEngine.Purchasing.PurchaseFailureReason failureReason)
        {
            //TMatch.StoreModel.Instance.OnPurchasedDispatch(success, productId, product, failureReason, true);

            if (success)
            {
                DebugUtil.Log($"IAPChecker: Finished order {productId} success.");
                TMatch.CommonEvent<string>.DispatchEvent(TMatch.EventEnum.UnfulfilledPaymentHandleSuccess, productId);
            }
            else
            {
                DebugUtil.Log($"IAPChecker: Finished order {productId} failure, Reason {failureReason}.");
            }

            isOrderProcessing = false;
            // 这里不管成功与否都先移除掉，后续根据底层提供的订单重新补单
            RemoveUnfinishedOrder(productId);
        }

        /// <summary>
        /// 移除未完成订单
        /// </summary>
        /// <param name="id"></param>
        private void RemoveUnfinishedOrder(string id)
        {
            _unfinishedOrders.Remove(id);
        }

        /// <summary>
        /// 添加未完成的订单
        /// </summary>
        /// <param name="id"></param>
        // public void AddUnfinishedOrder(string id)
        // {
        //     if (_unfinishedOrders.Contains(id) || MyMain.myGame.Fsm.CurrentState.Type != FsmStateType.Decoration)
        //         return;
        //
        //     _unfinishedOrders.Add(id);
        //     TaskSystem.Model.Instance.InsertTaskAtLast(EntityName);
        //     DebugUtil.Log($"IAPChecker: Add unfinished order {id}.");
        // }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            // TaskSystem.Model.Instance.ChangeTaskNodeEvent(this);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            isOrderProcessing = false;
            _unfinishedOrders.Clear();
        }
    }
}