using System.Collections.Generic;

namespace DragonPlus.Ad.UA
{
    public interface IUaSdkWebViewEventCallback
    {
        void OnEvent(string eventName, Dictionary<string, string> parameters);
        void OnPageLoaded();
    }
}