using System;

namespace Screw.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WindowAttribute : Attribute
    {
        public UIWindowType WindowType;
        
        public UIWindowLayer WindowLayer;
        
        public string AssetPath;

        public bool AddUIMask;

        public WindowAttribute(UIWindowLayer windowLayer,string assetPath = "", bool addUIMask = false, UIWindowType windowType = UIWindowType.Normal)
        {
            WindowType = windowType;
            WindowLayer = windowLayer;
            AssetPath = assetPath;
            AddUIMask = addUIMask;
        }
    }
}