
using System;

namespace Screw.UI
{
    public class ViewAssetAttribute : Attribute
    {
        /// <summary>
        /// 资源定位地址。
        /// </summary>
        public string AssetName;

        public ViewAssetAttribute(string assetName = "")
        {
            AssetName = assetName;
        }
    }
}