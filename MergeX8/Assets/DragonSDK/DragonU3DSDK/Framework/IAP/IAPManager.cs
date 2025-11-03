using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Threading;
using DragonU3DSDK.Network.API.Protocol;
using System.Text;
using System.Reflection;
using DragonU3DSDK.Network.BI;
using DragonU3DSDK.SDKEvents;

namespace DragonU3DSDK
{
    using PurchaseCallback = Action<bool, string, Product, PurchaseFailureReason>;
    public enum PaymentStatus
    {
        PREPARED = 0,
        BEFORE_VERIFIED = 1,
        VERIFIED = 2,
        VERIFYFAILED = 3,
        FULFILLED = 4,
    }

    public enum EProductType
    {
        UNKNOWN = -1,
        CONSUMABLE = 0,
        NONCONSUMABLE = 1,
        SUBSCRIPTION = 2
    }
    public class UnfulfilledPayment
    {
        public string PaymentId = "";
        public string ProductId = "";
        public string TransactionId = "";
        public PaymentStatus Status;
        public string Receipt = "";
        public string UserData = "";
        public bool pending = false;
    }

    public class Receipt
    {
        public string Store = "";
        public string TransactionID = "";
        public string Payload = "";
    }

    /*
     * iapmanager 用法
     * 1 创建iapmanager实例， 这个在调用 SDK.GetInstance().Initialize() 时已经执行了
     * 2 在游戏的配置加载后，调用Init传入商品id
     * 3 购买时，调用PurchaseProduct
     * 4 订单会有处理失败的情况，在合适的时机，调用 RequestUnfulfilledPaymentsAndTryVerify，把未完成的订单完成
     */

    public class IAPManager : IStoreListener
    {
        private IStoreController m_Controller;
        private IExtensionProvider m_Extensions;
        private IAppleExtensions m_AppleExtensions;
        private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
        private ITransactionHistoryExtensions m_TransactionHistoryExtensions;
        private bool isPurchaseInProgress = false;
        private string purchasingProductId = null;

        private PurchaseCallback callback = null;
        private PurchaseFailureReason failureReason = PurchaseFailureReason.Unknown;

        private ReaderWriterLockSlim callbackLock = new ReaderWriterLockSlim();
        private Queue<Action> callbackActionQueue = new Queue<Action>();

        private List<string> _checkProductIds = new List<string>();
        private const string UNFULFILLED_PAYMENTS_KEY = "unfulfilled_payments";
        private Dictionary<string, UnfulfilledPayment> unfulfilledPayments = new Dictionary<string, UnfulfilledPayment>();
        private List<string> consumableProductIds = new List<string>();
        private List<string> subProductIds = new List<string>();
        private List<string> nonconsumableProductIds = new List<string>();

        private Dictionary<string, bool> consumableProductInfo = new Dictionary<string, bool>();
        private Dictionary<string, bool> nonconsumableProductInfo = new Dictionary<string, bool>();
        private Dictionary<string, bool> subscriptionProductInfo = new Dictionary<string, bool>();

        private Action<List<string>> _restoreCompleted;
        private bool isInitializing = false;
        private bool isCheckRequestLocked = false;
        private bool _didCheckSubscribeState;
        private string attribution = null;
        private float checkPendingPurchaseClock = 0f;
        private const float checkPendingPurchaseInterval = 60f;
        public bool DidCheckSubscribeState
        {
            get => _didCheckSubscribeState;
        }

        public IAPManager()
        {
            TimerManager.Instance.AddDelegate(Update);
            TimerManager.Instance.StartCoroutine(TryInit());
        }

        private IEnumerator TryInit()
        {
            while (true)
            {
                yield return new WaitForSeconds(5f);
                if (!IsInitialized())
                {
                    if ((Application.internetReachability != NetworkReachability.NotReachable) && !isInitializing)
                    {
#if !UNITY_EDITOR
                        Log("IAPManager: Re Init in IAPManager Coroutine");
#endif
                        Init(consumableProductIds, nonconsumableProductIds, subProductIds);
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public void Update(float delta)
        {
            if (callbackLock.IsWriteLockHeld || callbackLock.TryEnterWriteLock(200))
            {
                try
                {
                    while (callbackActionQueue.Count > 0)
                    {
                        Action _callback = callbackActionQueue.Dequeue();
                        if (_callback != null)
                        {
                            _callback();
                        }
                    }
                }
                finally
                {
                    try
                    {
                        callbackLock.ExitWriteLock();
                    }
                    catch (SynchronizationLockException e)
                    {
                        LogError($"IAPManager: SynchronizationLockException : {e.Message}");
                    }
                }
            }
            else
            {
                LogError("lock_failed_on_update");
            }

            checkPendingPurchaseClock += delta;
            if (checkPendingPurchaseClock >= checkPendingPurchaseInterval)
            {
                checkPendingPurchaseClock = 0f;
                var pendingPurchase = CheckPendingPurchase();
                if (pendingPurchase != null)
                {
                    EventManager.Instance.Trigger<SDKEvents.UnfulfilledPaymentPending>().Data(pendingPurchase).Trigger();
                }
            }
        }

        /// <summary>
        /// 由于IAPManager需要使用各游戏内productId,故该方法需在游戏相关配置加载后调用
        /// </summary>
        /// <param name="consumableProductIds">consumableProductId identifiers.</param>
        /// <param name="nonconsumableProductIds">nonconsumableProductId identifiers.</param>
        /// <param name="subProductIds">subscriptionProductId identifiers.</param>
        public void Init(List<string> consumableProductIds, List<string> nonconsumableProductIds = null, List<string> subProductIds = null)
        {
            if (!IsInitialized())
            {
#if !UNITY_EDITOR
                isInitializing = true;
                if (consumableProductIds != null)
                    this.consumableProductIds = consumableProductIds;
                if (nonconsumableProductIds != null)
                    this.nonconsumableProductIds = nonconsumableProductIds;
                if (subProductIds != null)
                    this.subProductIds = subProductIds;
                LoadUnfulfilledPayments();
                InitUnityPurchase(this.consumableProductIds, this.nonconsumableProductIds, this.subProductIds);
#endif
            }
        }

        void LoadUnfulfilledPayments()
        {
            if (PlayerPrefs.HasKey(UNFULFILLED_PAYMENTS_KEY))
            {
                string json = Utils.ReadFromLocal(UNFULFILLED_PAYMENTS_KEY);
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        Log($"IAPManager: LoadUnfulfilledPayments : {json}");
                        JsonConvert.PopulateObject(json, unfulfilledPayments);
                    }
                    catch (Exception e)
                    {
                        LogError(e.ToString());
                    }
                }
            }
        }

        void SaveUnfulfilledPayments()
        {
            string json = JsonConvert.SerializeObject(unfulfilledPayments);
            Utils.SaveToLocal(UNFULFILLED_PAYMENTS_KEY, json);
            Log($"IAPManager: SaveUnfulfilledPayments : {json}");
        }

        // 初始化IAP;
        public void InitUnityPurchase(List<string> consumableProductIds, List<string> nonconsumableProductIds = null, List<string> subProductIds = null)
        {
            if (IsInitialized())
                return;

#if UNITY_ANDROID && !UNITY_EDITOR
            //var ownedItemsInfo = DragonNativeBridge.getPurchases();
            //Log($"Owned items : {ownedItemsInfo}");
            //var items = JArray.Parse(ownedItemsInfo);
            //bool unfulfilledPaymentsChanged = false;
            //if (items != null)
            //{
            //    foreach (var item in items)
            //    {
            //        var json = JToken.Parse(item.ToString());
            //        var jsonData = JToken.Parse(json["json"].ToString());
            //        var orderId = jsonData["orderId"].ToString();
            //        var productId = jsonData["productId"].ToString();
            //        if (consumableProductIds.Contains(productId) && !unfulfilledPayments.ContainsKey(productId))
            //        {
            //            unfulfilledPayments.Add(productId, new UnfulfilledPayment
            //            {
            //                Status = PaymentStatus.PREPARED,
            //                ProductId = productId
            //            });
            //        }

            //        if (unfulfilledPayments.ContainsKey(productId))
            //        {
            //            unfulfilledPayments[productId].TransactionId = orderId;
            //            var jReceipt = new JObject();
            //            var receipt = new Receipt
            //            {
            //                Store = "GooglePlay",
            //                TransactionID = orderId,
            //                Payload = item.ToString()
            //            };
            //            unfulfilledPayments[productId].Receipt = JsonConvert.SerializeObject(receipt);
            //            Log($"Construct receipt : {unfulfilledPayments[productId].Receipt}");
            //            unfulfilledPaymentsChanged = true;
            //        }
            //    }
            //}

            //if (unfulfilledPaymentsChanged)
            //{
            //    SaveUnfulfilledPayments();
            //}
#endif

            StandardPurchasingModule module = StandardPurchasingModule.Instance();

            // 配置模式;
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
            if (consumableProductIds != null && consumableProductIds.Count > 0)
            {
                // Consumable
                foreach (string productId in consumableProductIds)
                {
                    builder.AddProduct(productId, ProductType.Consumable);
                }
                // NonConsumable
                if (nonconsumableProductIds != null && nonconsumableProductIds.Count > 0)
                {
                    foreach (string productId in nonconsumableProductIds)
                    {
                        builder.AddProduct(productId, ProductType.NonConsumable);
                    }
                }
                // Subscription
                if (subProductIds != null && subProductIds.Count > 0)
                {
                    foreach (string productId in subProductIds)
                    {
                        builder.AddProduct(productId, ProductType.Subscription);
                    }
                }

                //初始化;
                UnityPurchasing.Initialize(this, builder);
            }
            BIManager.Instance.SendCommonEvent(new BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.Initialize,
                Data = "consumableProductIdsCount = " + consumableProductIds.Count.ToString() + " nonconsumableProductIdsCount = " + nonconsumableProductIds.Count.ToString()
            });
        }

        public bool IsInitialized()
        {
            return m_Controller != null && m_Extensions != null;
        }

        /// <summary>
        /// 判断是否拥有该商品
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool HasOwnedProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return false;
            }

            Dictionary<string, EProductType> result = GetOwnedProducts();
            if (result == null)
            {
                return false;
            }

            return result.ContainsKey(productId);
        }


        public EProductType GetProductTypeByProductId(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return EProductType.UNKNOWN;
            }

            if (consumableProductIds.Contains(productId))
            {
                return EProductType.CONSUMABLE;
            }

            if (nonconsumableProductIds.Contains(productId))
            {
                return EProductType.NONCONSUMABLE;
            }

            if (subProductIds.Contains(productId))
            {
                return EProductType.SUBSCRIPTION;
            }

            return EProductType.UNKNOWN;
        }


        /// <summary>
        /// 获取已拥有商品列表，带商品类型（可消耗，不可消耗，订阅）
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, EProductType> GetOwnedProducts()
        {
            if (!IsInitialized())
            {
                return null;
            }

            Dictionary<string, EProductType> result = new Dictionary<string, EProductType>();

            foreach (var kv in consumableProductInfo)
            {
                if (kv.Value)
                {
                    result[kv.Key] = EProductType.CONSUMABLE;
                }
            }

            foreach (var kv in nonconsumableProductInfo)
            {
                if (kv.Value)
                {
                    result[kv.Key] = EProductType.NONCONSUMABLE;
                }
            }

            foreach (var kv in subscriptionProductInfo)
            {
                if (kv.Value)
                {
                    result[kv.Key] = EProductType.SUBSCRIPTION;
                }
            }

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetOwnedProductsFormatString()
        {
            Dictionary<string, EProductType> result = GetOwnedProducts();

            if (result == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (var kv in result)
            {
                sb.AppendFormat("{0} : {1}\n", kv.Key, kv.Value);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Called when Unity IAP is ready to make purchases.
        /// </summary>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            isInitializing = false;
            Log("IAPManager: On Initialized Successfully !");

            m_Controller = controller;
            m_Extensions = extensions;
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
            m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();


            //获取商品具体信息
            ProductCollection products = m_Controller.products;
            Product[] all = products.all;

            for (int i = 0; i < all.Length; i++)
            {

                if (all[i].metadata == null) continue;

                if (this.nonconsumableProductIds.Contains(all[i].definition.id))
                {
                    this.nonconsumableProductInfo[all[i].definition.id] = all[i].hasReceipt;
                    Log($"IAPManager: 可用商品 {i} : {all[i].metadata.localizedTitle}|{all[i].definition.storeSpecificId}|{all[i].definition.id}|是否已购买:{all[i].hasReceipt}|{all[i].metadata.localizedPriceString}|{all[i].metadata.localizedDescription}|{all[i].metadata.isoCurrencyCode}");

                }

                if (this.consumableProductIds.Contains(all[i].definition.id))
                {
                    this.consumableProductInfo[all[i].definition.id] = all[i].hasReceipt;
                    Log($"IAPManager: 可用商品 {i} : {all[i].metadata.localizedTitle}|{all[i].definition.storeSpecificId}|{all[i].definition.id}|是否已购买:{all[i].hasReceipt}|{all[i].metadata.localizedPriceString}|{all[i].metadata.localizedDescription}|{all[i].metadata.isoCurrencyCode}");

                }

                if (this.subProductIds.Contains(all[i].definition.id))
                {
                    this.subscriptionProductInfo[all[i].definition.id] = all[i].hasReceipt;
                    Log($"IAPManager: 可用商品 {i} : {all[i].metadata.localizedTitle}|{all[i].definition.storeSpecificId}|{all[i].definition.id}|是否已购买:{all[i].hasReceipt}|{all[i].metadata.localizedPriceString}|{all[i].metadata.localizedDescription}|{all[i].metadata.isoCurrencyCode}");

                }

                if (!IAPUtils.Has_Inited)
                {
                    try
                    {
                        IAPUtils.DeviceCurrencyCode = all[i].metadata.isoCurrencyCode;
                        IAPUtils.DeviceCurrencySymbol = all[i].metadata.localizedPriceString.Replace(all[i].metadata.localizedPrice.ToString("#0.00"), "").Trim();
                        IAPUtils.Has_Inited = true;
                    }
                    catch (Exception e)
                    {
                        LogError($"IAPManager: Error {e.ToString()}");
                        continue;
                    }
                }
                Log($"IAPManager: IAPUtils.DeviceCurrencySymbol = {IAPUtils.DeviceCurrencySymbol}");
#if UNITY_IOS
                DebugUtil.Log("IAPManager: before confirm");
                m_Controller.ConfirmPendingPurchase(all[i]);
                DebugUtil.Log("IAPManager: after confirm");

#endif
            }

#if UNITY_IOS
            m_Extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(OnDeferred);
#endif
            EventManager.Instance.Trigger<SDKEvents.IAPInitialized>().Trigger();

            BIManager.Instance.SendCommonEvent(new BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.InitializeSuccess,
            });
            //BIManager.Instance.SendCommonGameEvent(BiEventCommon.Types.CommonGameEventType.IapInitialized);
        }

        /// <summary>
        /// Gets all product info.
        /// </summary>
        /// <returns>The all product info.</returns>
        public Product[] GetAllProductInfo()
        {
            if (!IsInitialized())
            {
                return null;
            }
            ProductCollection products = m_Controller.products;
            Product[] all = products.all;
            foreach (var p in all)
            {
                if (p.hasReceipt)
                {
                    //Log($"IAPManager: Have product in unfulfilled state productId : {p.definition.id} receipt : {p.receipt}");
                }
            }
            return all;
        }

        /// <summary>
        /// Called when Unity IAP encounters an unrecoverable initialization error.
        ///
        /// Note that this will not be called if Internet is unavailable; Unity IAP
        /// will attempt initialization until it becomes available.
        /// </summary>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            isInitializing = false;
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    LogError("IAPManager: Is your App correctly uploaded on the relevant publisher console?");
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    LogError("IAPManager: Billing disabled! Ask the user if billing is disabled in device settings.");
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    LogError("IAPManager: No products available for purchase! Developer configuration error; check product metadata!");
                    break;
            }
            BIManager.Instance.SendCommonEvent(new BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.InitializeFailure,
                Data = error.ToString(),
            });
            //BIManager.Instance.SendCommonGameEvent(BiEventCommon.Types.CommonGameEventType.IapInitializeFailure, error.ToString());
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            isInitializing = false;
            
            LogError($"IAPManager: OnInitializeFailed. Reason: {error.ToString()}. Message: {message}");
            
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    LogError("IAPManager: Is your App correctly uploaded on the relevant publisher console?");
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    LogError("IAPManager: Billing disabled! Ask the user if billing is disabled in device settings.");
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    LogError("IAPManager: No products available for purchase! Developer configuration error; check product metadata!");
                    break;
            }
            BIManager.Instance.SendCommonEvent(new BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.InitializeFailure,
                Data = error.ToString() + " " +message,
            });
        }

        // 设置未完成订单的订单id，主要是用于设置从服务器拉取下来的数据
        public void SetUnfulfilledPaymentId(string productId, string paymentId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(paymentId))
            {
                LogError($"IAPManager: SetUnfulfilledPaymentId error productId : {productId} paymentId: {paymentId}");
                return;
            }

            if (unfulfilledPayments.ContainsKey(productId) && unfulfilledPayments[productId].PaymentId != paymentId)
            {
                unfulfilledPayments[productId].PaymentId = paymentId;
                SaveUnfulfilledPayments();
            }
        }

        void PreparePayment(Product product, Action<bool> cb, string userData = "")
        {
            var productId = product.definition.id;
            var cPreparePayment = new CPreparePayment { ProductId = productId };
            Network.API.APIManager.Instance.Send(cPreparePayment, (SPreparePayment sPreparePayment) =>
            {
                Log($"prepare purchase with productId = {productId} paymentId = {sPreparePayment.Payment.PaymentId}");
                unfulfilledPayments[productId] = new UnfulfilledPayment
                {
                    PaymentId = sPreparePayment.Payment.PaymentId,
                    ProductId = productId,
                    Status = PaymentStatus.PREPARED,
                    UserData = userData
                };
                SaveUnfulfilledPayments();
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Prepare,
                    ProductId = productId,
                    PaymentId = sPreparePayment.Payment.PaymentId,
                    Data = "success",
                });
                cb(true);
            }, (errno, errmsg, resp) =>
            {
                LogError($"IAPManager: prepare purchase failed with errno = {errno} errmsg = {errmsg}");
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Prepare,
                    ProductId = productId,
                    Data = errmsg
                });
                cb(false);
            });
        }

        void VerifyPayment(string productId, string transactionID, string receipt, Action<bool> cb, int leftRetryCount = 0)
        {
            Product product = m_Controller.products.WithID(productId);
            if (product == null)
            {
                LogError($"IAPManager: verify purchase failed with product = null, productId = {productId}");
                cb?.Invoke(false);
                return;
            }

            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(transactionID) || string.IsNullOrEmpty(receipt))
            {
                LogError($"IAPManager: verify purchase productId {productId} failed ! check arguments error transactionID:{transactionID} receipt:{receipt}!");
                cb?.Invoke(false);
                return;
            }

            if (!unfulfilledPayments.ContainsKey(productId))
            {
                LogError($"IAPManager: verify purchase productId {productId} not found in unfulfilledPayments");
                cb?.Invoke(false);
                return;
            }

            var unfulfilledPayment = unfulfilledPayments[productId];
            if (unfulfilledPayment.Status != PaymentStatus.PREPARED && unfulfilledPayment.Status != PaymentStatus.BEFORE_VERIFIED)
            {
                LogError($"IAPManager: verify purchase productId {productId} wrong status {unfulfilledPayment.Status} unfulfilledPayments");
                cb?.Invoke(false);
                return;
            }

            unfulfilledPayment.TransactionId = transactionID;
            unfulfilledPayment.Receipt = receipt;
            unfulfilledPayment.Status = PaymentStatus.BEFORE_VERIFIED;
            SaveUnfulfilledPayments();

            var cVerifyPayment = new CVerifyPayment
            {
                PaymentId = unfulfilledPayment.PaymentId,
                TransactionId = unfulfilledPayment.TransactionId,
                Receipt = unfulfilledPayment.Receipt,
                IsoCurrencyCode = product.metadata.isoCurrencyCode,
                LocalizedPrice = product.metadata.localizedPrice.ToString(),
            };

            if (!string.IsNullOrEmpty(this.attribution))
            {
                cVerifyPayment.Attribution = this.attribution;
            }

            Network.API.APIManager.Instance.Send(cVerifyPayment, (SVerifyPayment sVerifyPayment) =>
            {
                if (sVerifyPayment.Payment.IsDuplicate)
                {
                    LogError($"IAPManager: verify purchase duplicate productId = {unfulfilledPayment.PaymentId} paymentId = {productId}");
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                    {
                        IapStep = BiEventCommon.Types.IapStep.Verify,
                        ProductId = productId,
                        PaymentId = unfulfilledPayment.PaymentId,
                        TransactionId = transactionID,
                        Data = "duplicate"
                    });
                }
                else
                {
                    Log($"IAPManager: verify purchase success productId = {unfulfilledPayment.PaymentId} paymentId = {productId}");
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                    {
                        IapStep = BiEventCommon.Types.IapStep.Verify,
                        ProductId = productId,
                        PaymentId = unfulfilledPayment.PaymentId,
                        TransactionId = transactionID,
                        Data = "success"
                    });
                }

                unfulfilledPayment.Status = PaymentStatus.VERIFIED;
                SaveUnfulfilledPayments();

                var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<Storage.StorageCommon>();
                if (storageCommon != null)
                {
                    PropertyInfo property = typeof(Storage.StorageCommon).GetProperty("RevenueUSDCents");
                    property.DeclaringType.GetProperty("RevenueUSDCents");
                    property.GetSetMethod(true).Invoke(storageCommon, new object[] { storageCommon.RevenueUSDCents + (ulong)Mathf.RoundToInt(float.Parse(sVerifyPayment.Payment.PriceUS) * 100.0f) });

                    storageCommon.ActiveData.ActiveRevenueUSDCents += (ulong)Mathf.RoundToInt(float.Parse(sVerifyPayment.Payment.PriceUS) * 100.0f);

                    property = typeof(Storage.StorageCommon).GetProperty("LastRevenueTime");
                    property.DeclaringType.GetProperty("LastRevenueTime");
                    property.GetSetMethod(true).Invoke(storageCommon, new object[] { DeviceHelper.CurrentTimeMillis() });

                    property = typeof(Storage.StorageCommon).GetProperty("RevenueCount");
                    property.DeclaringType.GetProperty("RevenueCount");
                    property.GetSetMethod(true).Invoke(storageCommon, new object[] { storageCommon.RevenueCount + 1 });

                }

                cb?.Invoke(true);
                JObject obj = new JObject();
                obj["price"] = product.metadata.localizedPrice;
                obj["currency"] = product.metadata.isoCurrencyCode;
                obj["transactionId"] = sVerifyPayment.Payment.TransactionId;
                obj["isTest"] = sVerifyPayment.Payment.IsTest;
                obj["productId"] = productId;

                var adjust = Dlugin.SDK.GetInstance().adjustPlugin;
                if (adjust != null)
                {
                    adjust.TrackEvent("purchase", 0, obj.ToString());
                }

                var firebase = Dlugin.SDK.GetInstance().firebasePlugin;
                if (firebase != null)
                {
                    firebase.TrackEvent("purchase", 0, obj.ToString());
                }

                if (!sVerifyPayment.Payment.IsTest)
                {
                    //Dlugin.SDK.GetInstance().loginService.LogPurchaseEvent(product.metadata.localizedPrice, product.metadata.isoCurrencyCode, productId);
                    if (Facebook.Unity.FB.IsInitialized && !sVerifyPayment.Payment.IsTest)
                    {
                        Facebook.Unity.FB.LogPurchase(product.metadata.localizedPrice, product.metadata.isoCurrencyCode, new Dictionary<string, object>
                        {
                            {
                                "transactionId", sVerifyPayment.Payment.TransactionId
                            },
                            {
                                "productId", productId
                            }
                        });
                    }
                }
                
                BIManager.Instance.onThirdPartyTracking(productId);
            },
            (errno, errmsg, resp) =>
            {
                LogError($"IAPManager: verify purchase productId {productId} failed with errno = {errno} errmsg = {errmsg}");
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Verify,
                    ProductId = productId,
                    PaymentId = unfulfilledPayment.PaymentId,
                    TransactionId = transactionID,
                    Data = errmsg
                });

                if (errno == ErrorCode.PaymentAlreadyFulfilledError ||
                    errno == ErrorCode.PaymentNotExistsError ||
                    errno == ErrorCode.PaymentVerifyError)
                {
                    failureReason = PurchaseFailureReason.PaymentDeclined;
                    unfulfilledPayment.Status = PaymentStatus.VERIFYFAILED;
                    SaveUnfulfilledPayments();
                }
                else if (errno == ErrorCode.PaymentVerifyPurchaseCanceled)
                {
                    failureReason = PurchaseFailureReason.UserCancelled;
                    unfulfilledPayment.Status = PaymentStatus.VERIFYFAILED;
                    SaveUnfulfilledPayments();
                }
                else if (errno == ErrorCode.PaymentVerifyPurchasePending)
                {
                    failureReason = PurchaseFailureReason.ExistingPurchasePending;
                    unfulfilledPayment.pending = false;
                }

                if (leftRetryCount > 0 && unfulfilledPayment.Status != PaymentStatus.VERIFYFAILED)
                {
                    VerifyPayment(productId, transactionID, receipt, cb, leftRetryCount - 1);
                }
                else
                {
                    cb?.Invoke(false);
                }
            });
        }

        void FulfillPayment(Product product, Action<bool> cb)
        {
            var productId = product.definition.id;
            if (!unfulfilledPayments.ContainsKey(productId))
            {
                LogError($"IAPManager: fulfill purchase productId {productId} not found in unfulfilledPayments");
                cb?.Invoke(false);
                return;
            }

            var unfulfilledPayment = unfulfilledPayments[productId];
            if (unfulfilledPayment.Status != PaymentStatus.VERIFIED && unfulfilledPayment.Status != PaymentStatus.VERIFYFAILED)
            {
                LogError($"IAPManager: fulfill purchase productId {productId} wrong status {unfulfilledPayment.Status} unfulfilledPayments");
                cb?.Invoke(false);
                return;
            }

            var cFulfillPayment = new CFulfillPayment
            {
                PaymentId = unfulfilledPayment.PaymentId,
            };

            Network.API.APIManager.Instance.Send(cFulfillPayment, (SFulfillPayment sFulfillPayment) =>
            {
                Log($"IAPManager: fulfill purchase success productId = {productId} paymentId = {unfulfilledPayment.PaymentId}");
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Fulfill,
                    ProductId = productId,
                    PaymentId = unfulfilledPayment.PaymentId,
                    TransactionId = (string.IsNullOrEmpty(unfulfilledPayment.TransactionId) ? "null" : unfulfilledPayment.TransactionId),
                    Data = "success"
                });
                unfulfilledPayments.Remove(productId);
                SaveUnfulfilledPayments();
                cb?.Invoke(true);
            },
            (errno, errmsg, resp) =>
            {
                LogError($"IAPManager: fulfill purchase productId {productId} failed with errno = {errno} errmsg = {errmsg}");
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Fulfill,
                    ProductId = productId,
                    PaymentId = unfulfilledPayment.PaymentId,
                    TransactionId = (string.IsNullOrEmpty(unfulfilledPayment.TransactionId) ? "null" : unfulfilledPayment.TransactionId),
                    Data = errmsg
                });
                if (unfulfilledPayment.Status == PaymentStatus.VERIFYFAILED)
                {
                    unfulfilledPayments.Remove(productId);
                    SaveUnfulfilledPayments();
                    cb?.Invoke(true);
                }
                else
                {
                    cb?.Invoke(false);
                }
            });
        }

        /*
         *
         *   bool: success; string: pruduct id;
         * 
         */

        public void PurchaseProduct(string productId, PurchaseCallback cb, string attribution = "", string userData = "")
        {
            //m_Controller.InitiatePurchase(productId);

            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.Start,
                ProductId = productId,
            });

            if (IsInitialized())
            {
                if (isPurchaseInProgress)
                {
                    cb?.Invoke(false, productId, null, PurchaseFailureReason.DuplicateTransaction);
                    return;
                }

                Product product = m_Controller.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    Log($"IAPManager: Purchasing product asychronously: '{product.definition.id}'");
                    PreparePayment(product, (result) =>
                    {
                        if (result)
                        {
                            if (isPurchaseInProgress)
                            {
                                cb?.Invoke(false, productId, null, PurchaseFailureReason.DuplicateTransaction);
                                return;
                            }
                            isPurchaseInProgress = true;

                            m_Controller.InitiatePurchase(product);
                            purchasingProductId = productId;
                            if (cb != null)
                            {
                                this.callback = cb;
                                this.failureReason = PurchaseFailureReason.Unknown;
                            }
                            if (!string.IsNullOrEmpty(attribution))
                            {
                                this.attribution = attribution;
                            }
                            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                            {
                                IapStep = BiEventCommon.Types.IapStep.Initiate,
                                ProductId = productId,
                            });
                        }
                        else
                        {
                            LogError($"IAPManager: prepare product {productId} failed");
                            cb?.Invoke(false, productId, product, PurchaseFailureReason.ProductUnavailable);
                        }
                    }, userData);
                }
                else
                {
                    LogError($"IAPManager: PurchaseProduct: FAILED. Not purchasing product, either is not found or is not available for purchase productId = {productId}");
                    cb?.Invoke(false, productId, null, PurchaseFailureReason.ProductUnavailable);
                }
            }
            else
            {
                LogError($"IAPManager: PurchaseProduct {productId} failed. Not initialized.");
                cb?.Invoke(false, productId, null, PurchaseFailureReason.PurchasingUnavailable);
            }
        }

        /// <summary>
        /// Called when a purchase completes.
        ///
        /// May be called at any time after OnInitialized().
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            var productId = e.purchasedProduct.definition.id;
            Log($"IAPManager: Purchase OK: {productId}");
            Log($"IAPManager: Receipt: {e.purchasedProduct.receipt}");

            EProductType type = GetProductTypeByProductId(productId);
            switch (type)
            {
                case EProductType.CONSUMABLE:
                    consumableProductInfo[productId] = true;
                    break;
                case EProductType.NONCONSUMABLE:
                    nonconsumableProductInfo[productId] = true;
                    break;
                case EProductType.SUBSCRIPTION:
                    subscriptionProductInfo[productId] = true;
                    break;
            }


            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.Callback,
                ProductId = productId,
                TransactionId = e.purchasedProduct.transactionID,
                Data = e.purchasedProduct.receipt,
            });

            if (unfulfilledPayments.ContainsKey(productId) && string.IsNullOrEmpty(unfulfilledPayments[productId].Receipt))
            {
                unfulfilledPayments[productId].TransactionId = e.purchasedProduct.transactionID;
                unfulfilledPayments[productId].Receipt = e.purchasedProduct.receipt;
                Log($"IAPManager: 新补单 {productId}");
            }

            if (!isPurchaseInProgress)
            {
                if (unfulfilledPayments.ContainsKey(productId))
                {
                    var data = unfulfilledPayments[productId];
                    data.pending = true;
                    data.Receipt = e.purchasedProduct.receipt;
                    data.TransactionId = e.purchasedProduct.transactionID;
                    SaveUnfulfilledPayments();
                    return PurchaseProcessingResult.Pending;
                }
                else
                {
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                    {
                        IapStep = BiEventCommon.Types.IapStep.Callback,
                        ProductId = productId,
                        Data = "purchase_not_in_progress"
                    });
                    return PurchaseProcessingResult.Complete;
                }
            }

            if (purchasingProductId != productId)
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Callback,
                    ProductId = productId,
                    TransactionId = e.purchasedProduct.transactionID,
                    Data = "purchasingProductId:" + purchasingProductId + " not equal productId:" + productId,
                });
                return PurchaseProcessingResult.Complete;
            }



            VerifyPayment(e.purchasedProduct.definition.id, e.purchasedProduct.transactionID, e.purchasedProduct.receipt, (result) =>
            {
                addTask(() =>
                {
                    string pruchasedProductId = e.purchasedProduct.definition.id;
                    if (result)
                    {
                        DoConfirmPendingPurchaseByID(pruchasedProductId);
                    }
                    else
                    {
                        DoConfirmPurchaseFailedByID(pruchasedProductId);
                    }
                });
            }, 3);
            return PurchaseProcessingResult.Pending;
        }

        // 获取所有订阅和不可消耗类型的商品
        private List<Product> GetAvailableSubscriptionAndNonConsumableProducts()
        {
            var products = new List<Product>();
            if (!IsInitialized())
            {
                return products;
            }
            
            Product[] all = m_Controller.products.all;
            for (int i = 0; i < all.Length; i++)
            {
                var product = all[i];
                if (product.receipt != null)
                {
                    if (product.definition.type == ProductType.Subscription)
                    {
                        if (CheckIfProductIsAvailableForSubscriptionManager(product.receipt))
                        {
                            GoogleProductMetadata metadata = product.metadata.GetGoogleProductMetadata();
                            SubscriptionManager p = new SubscriptionManager(product, metadata != null ? metadata.originalJson : null);
                            SubscriptionInfo info = p.getSubscriptionInfo();
                            Log($"IAPManager: isSubscribed = {info.isSubscribed()}, isExpired = {info.isExpired()}, productId = {product.definition.storeSpecificId}");
                            if (info.isSubscribed() == Result.True && info.isExpired() == Result.False)
                            {
                                products.Add(product);
                            }
                        }
                    }
                    else if (product.definition.type == ProductType.NonConsumable)
                    {
                        products.Add(product);
                    }
                }
            }
            return products;
        }

        // 获取SubscriptionInfo
        public SubscriptionInfo GetAvailableSubscriptionInfo(string productId)
        {
            if (string.IsNullOrEmpty(productId)) return null;
            var products = GetAvailableSubscriptionAndNonConsumableProducts();
            Product product = null;
            if (products.Count > 0)
            {
                foreach (var productItem in products)
                {
                    if (productItem.definition.id.Equals(productId))
                    {
                        product = productItem;
                        break;
                    }
                }
            }
            if (product != null && product.receipt != null)
            {
                if (CheckIfProductIsAvailableForSubscriptionManager(product.receipt))
                {
                    GoogleProductMetadata metadata = product.metadata.GetGoogleProductMetadata();
                    SubscriptionManager p = new SubscriptionManager(product, metadata != null ? metadata.originalJson : null);
                    SubscriptionInfo info = p.getSubscriptionInfo();
                    if (info.isSubscribed() == Result.True && info.isExpired() == Result.False)
                    {
                        DebugUtil.Log($"IAPManager: ExpireDate = {info.getExpireDate()}");
                        DebugUtil.Log($"IAPManager: FreeTrialPerio = {info.getFreeTrialPeriod()}");
                        DebugUtil.Log($"IAPManager: SubscriptionPeriod = {info.getSubscriptionPeriod()}");
                        return info;
                    }
                }
            }
            return null;
        }


        // 获取订阅剩余时间(ms)
        public double GetRemainingTime(string productId)
        {
            var info = GetAvailableSubscriptionInfo(productId);
            if (info != null)
            {
                return info.getRemainingTime().TotalMilliseconds;
            }
            return 0;
        }

        // 获取订阅状态，结合服务器返回的状态一起使用
        public bool IsSubscribed(string productId)
        {
            var info = GetAvailableSubscriptionInfo(productId);
            if (info != null)
            {
                return info.isSubscribed() == Result.True && info.isExpired() == Result.False;
            }
            return false;
        }

        public void CheckSubscriptionAndNonConsumableState(bool ignoreReceipt, Action<List<string>> checkCompleted = null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || isCheckRequestLocked)
            {
                Log($"IAPManager: CheckSubscriptionState或者OnRestoreClick调用时机触发了, 无网 = {Application.internetReachability == NetworkReachability.NotReachable}, isCheckRequestLocked = {isCheckRequestLocked}");
                checkCompleted?.Invoke(_checkProductIds);
                return;
            }
            if (ignoreReceipt)
            {
                isCheckRequestLocked = true;
                var cCheckSubPayments = new CCheckSubPayments();
                Log("IAPManager: CheckSubscriptionState调用时机触发了, 开始普通check请求...");
                Network.API.APIManager.Instance.Send(cCheckSubPayments, (SCheckSubPayments sCheckSubPayments) =>
                {
                    _checkProductIds.Clear();
                    _didCheckSubscribeState = true;
                    isCheckRequestLocked = false;
                    var subscriptions = sCheckSubPayments.Subscriptions;
                    Log($"IAPManager: CheckSubscriptionState调用时机触发了, 结束普通check，subscriptions = {subscriptions.Count}");
                    if (subscriptions != null && subscriptions.Count > 0)
                    {
                        foreach (var subscription in subscriptions)
                        {
                            var productId = subscription.ProductId;
                            if (!string.IsNullOrEmpty(productId) && subscription.RemainingTime > 0)
                            {
                                Log($"IAPManager: CheckSubscriptionState调用时机触发了, 结束普通check，productId = {productId}, RemainingTime = {subscription.RemainingTime}");
                                if (!_checkProductIds.Contains(productId)) _checkProductIds.Add(productId);
                            }
                        }
                    }
                    checkCompleted?.Invoke(_checkProductIds);
                },
                (errno, errmsg, resp) =>
                {
                    Log($"IAPManager: CheckSubscriptionState调用时机触发了, check errno = {errno}, errmsg = {errmsg}, ignoreReceipt = {ignoreReceipt}");
                    _didCheckSubscribeState = false;
                    isCheckRequestLocked = false;
                    checkCompleted?.Invoke(_checkProductIds);
                });
            }
            else
            {
                var products = GetAvailableSubscriptionAndNonConsumableProducts();
                Log($"IAPManager: OnRestoreClick调用时机触发了--1，products = {products.Count}");
                if (products.Count > 0)
                {
                    var checkCount = products.Count;
                    var productIds = new List<string>();
                    foreach (var product in products)
                    {
                        var productId = product.definition.id;
                        Log($"IAPManager: OnRestoreClick调用时机触发了--2，productId = {productId}");
                        var productType = CCheckSubPayments.Types.PurchaseType.Subscription;
                        if (product.definition.type == ProductType.NonConsumable)
                        {
                            productType = CCheckSubPayments.Types.PurchaseType.Nonconsumable;
                        }
                        var cCheckSubPayments = new CCheckSubPayments { ProductId = productId, Receipt = product.receipt, PurchaseType = productType };
                        Network.API.APIManager.Instance.Send(cCheckSubPayments, (SCheckSubPayments sCheckSubPayments) =>
                        {
                            var subscriptions = sCheckSubPayments.Subscriptions;
                            if (subscriptions != null && subscriptions.Count > 0)
                            {
                                Log($"IAPManager: OnRestoreClick调用时机触发了--3，subscriptions = {subscriptions.Count}");
                                foreach (var subscription in subscriptions)
                                {
                                    if (!string.IsNullOrEmpty(subscription.ProductId) && subscription.RemainingTime > 0)
                                    {
                                        Log($"IAPManager: OnRestoreClick调用时机触发了--4，productId = {productId}, RemainingTime = {subscription.RemainingTime}");
                                        if (!productIds.Contains(productId)) productIds.Add(productId);
                                    }
                                }
                            }
                            checkCount--;
                            if (checkCount <= 0)
                            {
                                _didCheckSubscribeState = true;
                                _checkProductIds = productIds;
                                Log($"IAPManager: OnRestoreClick调用时机触发了--5，_checkProductIds = {_checkProductIds.Count}");
                                checkCompleted?.Invoke(_checkProductIds);
                            }
                        },
                        (errno, errmsg, resp) =>
                        {
                            Log($"IAPManager: OnRestoreClick调用时机触发了--6，check errno = {errno}, errmsg = {errmsg}, ignoreReceipt = {ignoreReceipt}");
                            checkCount--;
                            if (checkCount <= 0)
                            {
                                _didCheckSubscribeState = false;
                                checkCompleted?.Invoke(_checkProductIds);
                            }
                        });
                    }
                }
                else
                {
                    _didCheckSubscribeState = true;
                    Log($"IAPManager: OnRestoreClick调用时机触发了--7，_checkProductIds = {_checkProductIds.Count}");
                    checkCompleted?.Invoke(_checkProductIds);
                }
            }
        }

        /// <summary>
        /// This will be called after a call to IAppleExtensions.RestoreTransactions().
        /// </summary>
        private void OnTransactionsRestored(bool success)
        {
            addTask(() =>
            {
                if (!success)
                {
                    _restoreCompleted.Invoke(new List<string>());
                    Log("IAPManager: OnRestoreClick调用时机触发了--1，失败了...");
                    return;
                }
                CheckSubscriptionAndNonConsumableState(false, (productIds) =>
                {
                    _restoreCompleted?.Invoke(productIds);
                });
            });
        }

        public void RestorePurchases(Action<List<string>> restoreCompleted)
        {
            if (restoreCompleted != null)
            {
                _restoreCompleted = restoreCompleted;
            }
            try
            {
#if UNITY_ANDROID
                //m_GooglePlayStoreExtensions.RestoreTransactions(OnTransactionsRestored);
                OnTransactionsRestored(true);
#elif UNITY_IOS
                m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
#endif
            }
            catch (Exception e)
            {
                LogError(e.ToString());
                Log("IAPManager: OnRestoreClick调用时机触发了--1，异常了...");
                restoreCompleted.Invoke(new List<string>());
            }
        }

        private bool CheckIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];
            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null ||
                                !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null ||
                                !developerPayload_wrapper.ContainsKey("is_free_trial") ||
                                !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                        {
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            return false;
        }

        // 判断该商品是否付费完但是没有消耗
        public bool IsProductAlreadyOwned(string productId)
        {
            if (unfulfilledPayments.ContainsKey(productId) &&
                (unfulfilledPayments[productId].Status == PaymentStatus.PREPARED ||
                unfulfilledPayments[productId].Status == PaymentStatus.BEFORE_VERIFIED
                ) &&
                !string.IsNullOrEmpty(unfulfilledPayments[productId].Receipt))
            {
                return true;
            }

            return false;
        }

        public bool GetPaymentId(string productId, out string paymentId)
        {
            if (unfulfilledPayments.ContainsKey(productId))
            {
                paymentId = unfulfilledPayments[productId].PaymentId;
                return true;
            }
            paymentId = string.Empty;
            return false;
        }
        
        public string GetUserData(string productId)
        {
            return !unfulfilledPayments.ContainsKey(productId) ? "" : unfulfilledPayments[productId].UserData;
        }

        // 用于补单操作
        public void VerifyUnfulfilledPayment(PurchaseCallback cb, string productId = "")
        {
            var products = GetAllProductInfo();
            if (products == null || products.Length <= 0 || unfulfilledPayments.Count <= 0)
            {
                cb?.Invoke(false, productId, null, PurchaseFailureReason.SignatureInvalid);
                return;
            }

            if (isPurchaseInProgress)
            {
                cb?.Invoke(false, productId, null, PurchaseFailureReason.DuplicateTransaction);
                return;
            }

            foreach (var kvp in unfulfilledPayments)
            {
                if (IsProductAlreadyOwned(kvp.Value.ProductId) && (string.IsNullOrEmpty(productId) || kvp.Value.ProductId == productId))
                {
                    if (cb != null)
                    {
                        callback = cb;
                        failureReason = PurchaseFailureReason.Unknown;
                    }
                    Log($"尝试重新验证Product : {kvp.Value.ProductId}");
                    isPurchaseInProgress = true;
                    VerifyPayment(kvp.Value.ProductId, kvp.Value.TransactionId, kvp.Value.Receipt, (result) =>
                    {
                        // TODO: while loop
                        addTask(() =>
                        {
                            string pruchasedProductId = kvp.Value.ProductId;
                            if (result)
                            {
                                DoConfirmPendingPurchaseByID(pruchasedProductId);
                            }
                            else
                            {
                                DoConfirmPurchaseFailedByID(pruchasedProductId);
                            }
                        });
                    }, 3);
                    return;
                }
            }

            cb?.Invoke(false, productId, null, PurchaseFailureReason.Unknown);
        }

        // 确认购买产品成功;
        public void DoConfirmPendingPurchaseByID(string productId)
        {
            Product product = m_Controller.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                if (isPurchaseInProgress)
                {
                    FulfillPayment(product, (result) =>
                    {
                        isPurchaseInProgress = false;
                        purchasingProductId = null;
                    });
                }
                else
                {
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                    {
                        IapStep = BiEventCommon.Types.IapStep.Failure,
                        ProductId = productId,
                        Data = "purchase_not_in_progress"
                    });
                }
                m_Controller.ConfirmPendingPurchase(product);

                EProductType type = GetProductTypeByProductId(productId);
                if (type == EProductType.CONSUMABLE)
                {
                    consumableProductInfo[productId] = false;
                }

            }
            else
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Failure,
                    ProductId = productId,
                    Data = "not_available_to_purchase"
                });
            }

            if (callback != null)
            {
                callback(true, productId, product, failureReason);
                callback = null;
                failureReason = PurchaseFailureReason.Unknown;
            }
        }
        // 确认购买产品失败，消耗掉订单
        public void DoConfirmPurchaseFailedByID(string productId)
        {
            Product product = m_Controller.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                if (isPurchaseInProgress)
                {
                    FulfillPayment(product, (result) =>
                    {
                        isPurchaseInProgress = false;
                        purchasingProductId = null;
                        if (result)
                        {
                            m_Controller.ConfirmPendingPurchase(product);
                        }
                    });
                }
                else
                {
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                    {
                        IapStep = BiEventCommon.Types.IapStep.Failure,
                        ProductId = productId,
                        Data = "purchase_not_in_progress"
                    });
                }
                //m_Controller.ConfirmPendingPurchase(product);
            }
            else
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                {
                    IapStep = BiEventCommon.Types.IapStep.Failure,
                    ProductId = productId,
                    Data = "not_available_to_purchase"
                });
            }

            if (callback != null)
            {
                callback(false, productId, product, failureReason);
                callback = null;
                failureReason = PurchaseFailureReason.Unknown;
            }
        }

        /// <summary>
        /// Called when a purchase fails.
        /// </summary>
        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            string errmsg = $"OnPurchaseFailed product = {i} reason = {p.ToString()} error_code = {m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode()}";
            if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
            {
                errmsg += $" description = {m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message}";
            }
            LogError(errmsg);

            isPurchaseInProgress = false;
            purchasingProductId = null;
            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.Failure,
                ProductId = i.definition.id,
                Data = (p == PurchaseFailureReason.Unknown) ? (p.ToString() + errmsg) : p.ToString(),
            });

            if (p == PurchaseFailureReason.DuplicateTransaction && !IsProductAlreadyOwned(i.definition.id))
            {
                // 这里表示这个商品已经拥有，但是收据丢了，直接把这个商品消耗掉，好让用户可以继续购买
                addTask(() =>
                {
                    Product product = m_Controller.products.WithID(i.definition.id);
                    if (product != null)
                    {
                        m_Controller.ConfirmPendingPurchase(product);
                        DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
                        {
                            IapStep = BiEventCommon.Types.IapStep.Failure,
                            ProductId = i.definition.id,
                            Data = "duplicate_transaction_not_owned_confirm"
                        });
                    }
                });
            }

            if (callback != null)
            {
                callback(false, i.definition.id, i, p);
                callback = null;
                failureReason = PurchaseFailureReason.Unknown;
            }
        }

        public void RequestUnfulfilledPaymentsAndTryVerify(PurchaseCallback cb, string checkProductId = "")
        {
            Network.API.APIManager.Instance.Send(new CListUnfulfilledPayments(),
            (Google.Protobuf.IMessage obj) =>
            {
                Log("CListUnfulfilledPayments success !");
                var payments = (obj as SListUnfulfilledPayments);
                foreach (var payment in payments.Payments)
                {
                    Dlugin.SDK.GetInstance().iapManager.SetUnfulfilledPaymentId(payment.ProductId, payment.PaymentId);
                }
                Dlugin.SDK.GetInstance().iapManager.VerifyUnfulfilledPayment(cb, checkProductId);
            },
            (errno, errmsg, resp) =>
            {
                LogError($"CListUnfulfilledPayments error : {errno.ToString()} {errmsg}");
                
                //在网络请求失败后依然需要回调，保证流程完成
                cb?.Invoke(false, null, null, PurchaseFailureReason.Unknown);
            });
        }

        //iOS延时处理，暂未实现
        private void OnDeferred(Product item)
        {
            LogError($"Purchase deferred: {item.definition.id}");
            DragonU3DSDK.Network.BI.BIManager.Instance.SendException(new Exception("iOS延时处理，暂未实现"));
        }

        void Log(string msg)
        {
            DebugUtil.Log(msg);
#if DEBUG || DEVELOPMENT_BUILD
            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.Debug,
                Data = msg,
            });
#endif
        }
        void LogError(string msg)
        {
            DebugUtil.LogError(msg);
            if (string.IsNullOrEmpty(msg)) msg = "";

#if DEBUG || DEVELOPMENT_BUILD
            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonEvent(new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.IapDetail
            {
                IapStep = BiEventCommon.Types.IapStep.Failure,
                Data = msg,
            });
#endif
        }

        bool addTask(Action task)
        {
            if (callbackLock.IsWriteLockHeld || callbackLock.TryEnterWriteLock(200))
            {
                try
                {
                    callbackActionQueue.Enqueue(task);
                }
                finally
                {
                    try
                    {
                        callbackLock.ExitWriteLock();
                    }
                    catch (SynchronizationLockException ex)
                    {
                        LogError($"SynchronizationLockException : {ex.ToString()}");
                    }
                }
                return true;
            }
            else
            {
                LogError("add Task failed");
                return false;
            }
        }

        public UnfulfilledPayment CheckPendingPurchase()
        {
            if (unfulfilledPayments != null)
            {
                foreach (var kv in unfulfilledPayments)
                {
                    if (kv.Value != null && kv.Value.pending)
                    {
                        return kv.Value;
                    }
                }
                return null;
            }
            return null;
        }
    }
}
