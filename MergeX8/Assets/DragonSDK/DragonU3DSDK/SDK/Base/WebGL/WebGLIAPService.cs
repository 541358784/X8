using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Dlugin.PluginStructs;

#if UNITY_WEBGL
namespace Dlugin
{
	public class WebGLIAPService : WebGLServiceProvider, IPayChannel
	{
		[DllImport("__Internal")]
		private static extern void Plugin_pay (string pluginId, string payment, string context);
		[DllImport("__Internal")]
        private static extern void Plugin_restore (string pluginId, string context);
		[DllImport("__Internal")]
        private static extern void Plugin_consume (string pluginId, string payment, string context);
		[DllImport("__Internal")]
        private static extern void Plugin_requestUnfinished (string pluginId, string context);
        [DllImport("__Internal")]
        private static extern void Plugin_requestProductList (string pluginId, string products, string context);

		public WebGLIAPService(PluginDefine[] allDefine) : base (allDefine)
		{
		}

        public void Pay (PARAMPay payment, string context)			//tag means a context
		{
            string paymentString = JsonUtility.ToJson(payment);
			var iter = GetAllPluginFiltedIterator (PluginFilter);
			while (iter.MoveNext ()) {
				PluginDefine pd = iter.Current as PluginDefine;
                SDK.FormatDebug ("WebGLIAPServiceProvider.Pay ----> pay {0} with plugin {1}, paymentString {2}", payment.productId, pd.pluginId, paymentString);
                Plugin_pay (pd.pluginId, paymentString, context);
			}
        }

        public void Restore (string context)
		{
			var iter = GetAllPluginFiltedIterator (PluginFilter);
			while (iter.MoveNext ()) {
				PluginDefine pd = iter.Current as PluginDefine;
                SDK.FormatDebug ("WebGLIAPServiceProvider.Restore ----> restore with plugin {0}", pd.pluginId);
                Plugin_restore (pd.pluginId, context);
			}
        }

        public void Consume(PaymentInfo info, string context)
        {
            string paymentString = JsonUtility.ToJson(info);
			var iter = GetAllPluginFiltedIterator (PluginFilter);
			while (iter.MoveNext ()) {
				PluginDefine pd = iter.Current as PluginDefine;
                SDK.FormatDebug ("WebGLIAPServiceProvider.Consume ----> consume {0} with plugin {1}, paymentString {2}", info.productId, pd.pluginId, paymentString);
                Plugin_consume (pd.pluginId, paymentString, context);
			}
		}

        public void RequestUnfinished(string context)
		{
			var iter = GetAllPluginFiltedIterator (PluginFilter);
			while (iter.MoveNext ()) {
				PluginDefine pd = iter.Current as PluginDefine;
				SDK.FormatDebug ("WebGLIAPServiceProvider.RequestUnfinished ----> get unfinished with plugin {0}", pd.pluginId);
				Plugin_requestUnfinished (pd.pluginId, context);
			}
        }

        public void RequestProductList(List<ProductInfo> products, string context)
        {
            PARAMProductArray array = new PARAMProductArray() { infos = products.ToArray() };
            string productsStr = JsonUtility.ToJson(array);
            var iter = GetAllPluginFiltedIterator (PluginFilter);
            while (iter.MoveNext ()) {
                PluginDefine pd = iter.Current as PluginDefine;
                SDK.FormatDebug ("WebGLIAPServiceProvider.RequestProductList ----> get unfinished with plugin {0}", pd.pluginId);

                Plugin_requestProductList (pd.pluginId,productsStr, context);
            }
        }
	}
}
#endif