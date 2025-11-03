using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMatch
{
    public class UIViewSystem
    {
        private static UIViewSystem _instance;

        public static UIViewSystem Instance => _instance ??= new UIViewSystem();

        private string TryGetAssetAddressFromAttribute(Type type)
        {
            string address = null;

            var assetAddressAttributes = type.GetCustomAttributes(typeof(AssetAddressAttribute), false);

            if (assetAddressAttributes.Length > 0)
            {
                var assetAddressAttribute = assetAddressAttributes[0] as AssetAddressAttribute;

                if (assetAddressAttribute != null)
                {
                    address = assetAddressAttribute.assetAddress;

                    // if (CommonUtils.IsLE_16_10())
                    // {
                    //     if (!string.IsNullOrEmpty(assetAddressAttribute.assetAddressPad))
                    //     {
                    //         address = assetAddressAttribute.assetAddressPad;
                    //     }
                    // }
                }
            }

            return address;
        }

        public void Open<T>(UIViewParam param = null) where T : UIView
        {
            UIManager.Instance.OpenWindow<T>(TryGetAssetAddressFromAttribute(typeof(T)), param);
        }

        public void Close<T>() where T : UIView
        {
            UIManager.Instance.CloseWindow<T>();
        }

        public T Get<T>() where T : UIView
        {
            return UIManager.Instance.GetOpenedWindow<T>();
        }

        public void Close(Type type)
        {
            UIManager.Instance.CloseWindow(type);
        }

        public void CloseAll(params int[] excludeLayer)
        {
            // TODO: There's not right. fix later.
            UIManager.Instance.ClearAllWindows();
        }

        public bool HasPopup()
        {
            return UIManager.Instance.HasPopup();
        }
    }
}
