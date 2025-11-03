using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DragonU3DSDK
{
    public static class IAPUtils
    {

        /// <summary>
        /// 玩家货币代码，默认为美元，USD
        /// </summary>
        public static string DeviceCurrencyCode = "USD";

        public static string DeviceCurrencySymbol = "US$";

        public static bool Has_Inited = false;


        private static readonly Dictionary<string, double> ExchangeRateTable = new Dictionary<string, double>
        {
            {"USD",1},
            {"EUR",0.749625187},
            {"GBP",0.639617691},
            {"CAD",1.032308846},
            {"AUD",1.089955022},
            {"JPY",97.473763118},
            {"NZD",1.236956522},
            {"CHF",0.926086957},
            {"ZAR",10.002848576},
            {"BRL",2.347001499},
            {"INR",61.713643178},
            {"MXN",12.849475262},
            {"CYP",0.438736132},
            {"CZK",19.340329835},
            {"DKK",5.590329835},
            {"EEK",11.729085457},
            {"HUF",225.142428786},
            {"LTL",2.588305847},
            {"LVL",0.526761619},
            {"MTL",0.321814093},
            {"PLN",3.169490255},
            {"SEK",6.526986507},
            {"SIT",179.64017991},
            {"SKK",22.583208396},
            {"NOK",5.923163418},
            {"BGN",1.466116942},
            {"HRK",5.652773613},
            {"RON",3.332833583},
            {"RUB",32.898050975},
            {"TRY",1.937331334},
            {"CNY",6.114992504},
            {"HKD",7.754122939},
            {"IDR",10505.397301349},
            {"KRW",1113.290854573},
            {"MYR",3.277211394},
            {"PHP",43.712893553},
            {"SGD",1.270914543},
            {"THB",31.28035982},
            {"DZD",119.70},
            {"AED",3.67},
            {"EGP",16.07},
            {"PKR",154.60},
            {"PYG",6456.06},
            {"BOB",6.93},
            {"COP",3332.50},
            {"CRC",568.98},
            {"GEL",2.86},
            {"KZT",383.98},
            {"GHS",5.72},
            {"QAR",3.64},
            {"KES",100.60},
            {"LBP",1514.87},
            {"BDT",85.06},
            {"PEN",3.34},
            {"MMK",1501.18},
            {"MAD",9.64},
            {"NGN",362.50},
            {"RSD",106.03},
            {"SAR",3.75},
            {"LKR",181.46},
            {"TWD",30.13},
            {"TZS",2299.50},
            {"UAH",23.36},
            {"IQD",1190.4994},
            {"ILS",3.47},
            {"JOD",0.71},
            {"VND",23172.50},
            {"CLP",759.20},
            {"MOP",8.05}
        };


        /// <summary>
        /// 由美元数值，计算当前货币价格
        /// </summary>
        /// <param name="priceInUSD">美元价格，如5.99</param>
        /// <returns>当地货币价格，double类型，保留全精度，如138.193821</returns>
        public static double GetLocalizedPrice(double priceInUSD)
        {
            if (!ExchangeRateTable.ContainsKey(DeviceCurrencyCode))
            {
                DebugUtil.LogWarning("当前货币不存在于兑换表: currency : {0}", DeviceCurrencyCode);
                return priceInUSD;
            }

            double rate = ExchangeRateTable[DeviceCurrencyCode];

            return priceInUSD * rate;
        }


        /// <summary>
        /// 由美元数值，获取当前货币价格字符串，如 US$4.99
        /// </summary>
        /// <param name="priceInUSD">美元价格数值</param>
        /// <returns>当地字符串价格，CNY648.00,自动保留两位小数</returns>
        public static string GetLocalizedPriceString(double priceInUSD)
        {

            double price = GetLocalizedPrice(priceInUSD);


            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", DeviceCurrencySymbol, string.Format("{0:N2}", price));

            DebugUtil.Log("货币转换: 当前国家" + DeviceCurrencyCode);
            DebugUtil.Log("货币转换: 转换前价格 " + price);
            DebugUtil.Log("货币转换: 转换后价格" + sb);

            return sb.ToString();
        }
    }
}