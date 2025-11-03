using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Dlugin
{
    namespace PluginStructs
    {
        [System.Serializable]
        public class RETPluginArray
        {
            public PluginDefine[] allPlugins;
        }

        [System.Serializable]
        public class PARAMLogin
        {
            public int[] permissions;
            public string context;
        }

        [System.Serializable]
        public class PARAMProductArray
        {
            public ProductInfo[] infos;
        }

        [System.Serializable]
        public class PARAMPay
        {
            public string productId;
            public string desc;
            public string payload;
            public int count;
        }

        [StructLayout(LayoutKind.Sequential)]
        [System.Serializable]
        public class PaymentInfo
        {
            public string productId;
            public string payload;
            public string desc;
            public string paymentId;
            public int count;
            public int productType;
            public string purchaseReceipt;
            public string purchaseSignature;
            public string purchaseInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        [System.Serializable]
        public class ProductInfo
        {
            public string productId;
            public string description;
            public string title;
            public string localePrice;
            public int productType = Constants.kProductTypeConsume;
            public int price;
            public override string ToString()
            {
                return string.Format("[ProductInfo id:{0} title:{1} price:{2} localePrice:{3} type:{4} description:{5}]", productId, title, price, localePrice, productType, description);
            }
            public static ProductInfo Decode(string[] str, int startIndex) 
            {
                if (str == null || str.Length < startIndex + 5)
                    return null;
                ProductInfo ui = new ProductInfo ();
                if(str.Length > startIndex + 0)
                    ui.productId = str[startIndex + 0];
                if(str.Length > startIndex + 1)
                    ui.productType = int.Parse(str[startIndex + 1]);
                if(str.Length > startIndex + 2)
                    ui.price = int.Parse(str[startIndex + 2]);
                if(str.Length > startIndex + 3)
                    ui.description = str[startIndex + 3];
                if(str.Length > startIndex + 4)
                    ui.title = str[startIndex + 4];
                return ui;
            }

            public static List<ProductInfo> DecodeMany(string[] str, int startIndex) 
            {
                List<ProductInfo> ret = new List<ProductInfo>();
                if (str == null || str.Length < startIndex + 5)
                    return ret;
                while (true)
                {
                    ProductInfo pi = Decode(str, startIndex);
                    if (pi != null)
                        ret.Add(pi);
                    else
                        break;
                    startIndex += 5;
                }
                return ret;
            }

            public string[] Encode() 
            {
                return new string[]
                {
                    productId==null?"":productId,
                    productType.ToString(),
                    price.ToString(),
                    description==null?"":productId,
                    title==null?"":productId
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        [System.Serializable]
        public class UserInfo
        {
            public string userId;
            public string userName;
            public string userToken;
            public string email;
            public int status = Constants.kLoginStatusNotLogin;
            public int[] userPermissions;
            public string appleIdentityToken;
            public string appleAuthorizationCode;
            public int facebookMode;
            public static UserInfo Decode(string[] str, int startIndex)
            {
                if (str == null || str.Length == 0)
                    return null;
                UserInfo ui = new UserInfo ();
                if(str.Length > startIndex + 0)
                    ui.userId = str[startIndex + 0];
                if(str.Length > startIndex + 1)
                    ui.userName = str[startIndex + 1];
                if(str.Length > startIndex + 2)
                    ui.userToken = str[startIndex + 2];
                if(str.Length > startIndex + 3)
                    ui.email = str[startIndex + 3];
                if(str.Length > startIndex + 4)
                    ui.status = int.Parse(str[startIndex + 4]);
                if (str.Length > startIndex + 5) {
                    var permissions = str [startIndex + 5].Split (',');
                    var permissionList = new List<int> ();
                    for (int i = 0; i < permissions.Length; i++) {
                        try
                        {
                            permissionList.Add(int.Parse(permissions[i]));
                        }
                        catch(System.Exception ex) {
                            SDK.FormatError ("UserInfo.Create ----> {0} is not a valid permission number:{1}", permissions [i], ex.ToString());
                        }
                    }
                    ui.userPermissions = permissionList.ToArray ();
                }
                return ui;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        [System.Serializable]
        public class SDKError
        {
            public SDKError()
            {
                err = 0;
            }
            public SDKError (int errorNo, int channelErrorNo, string msg)
            {
                err = errorNo;
                channelErrno = channelErrorNo;
                errmsg = msg;
            }
            public int err;
            public int channelErrno;
            public string errmsg;
            public override string ToString()
            {
                return string.Format("[SDKPluginError err({0}), channelErrno({1}), errmsg({2})]", err, channelErrno, errmsg);
            }
        } 

        [System.Serializable]
        public struct SDKPluginError
        {
            public string pluginId;
            public SDKError error;
        }

        [System.Serializable]
        public class PluginDefine
        {
            public PluginDefine()
            {
                m_PluginParam = "";
                m_PluginName = "";
                m_PluginVersion = "";
            }
            public PluginDefine(string id, string type,string version)
            {
                m_PluginParam = id;
                m_PluginName = type;
                m_PluginVersion = version;
            }
            public PluginDefine(string id, string type,  string version, IEnumerable<int> services)
            {
                m_PluginParam = id;
                m_PluginName = type;
                m_PluginVersion = version;
            }
            public string m_PluginParam;
            public string m_PluginName;
            public string m_PluginVersion;
            public override string ToString()
            {
                return string.Format("[PluginDefine pluginId({0}), pluginType({1}), pluginVersion({2}))]", m_PluginParam, m_PluginName, m_PluginVersion);
            }
        }
    }

    public class SDKJsonConfig
    {
        [System.Serializable]
        public class PluginJsonConfig
        {
            public string id;
            public string className;
            public int[] services;
            public string appId;
        }
        public PluginJsonConfig[] managed;
        public PluginJsonConfig[] native;
    }
}
