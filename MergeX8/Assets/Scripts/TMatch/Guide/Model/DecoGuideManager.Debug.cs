using System.Collections.Generic;
using DragonU3DSDK.Storage;

namespace OutsideGuide
{
    public partial class DecoGuideManager
    {
        public const string DebugTitle = "局外引导";

        public static List<DebugCfg> GetDebugCfg()
        {
            List<DebugCfg> debugCfgs = new List<DebugCfg>();

            debugCfgs.Add(new DebugCfg()
            {
                TitleStr = "关闭引导",
                ClickCallBack = (arg1, arg2) =>
                {
                    Instance.OpenGuide = false;
                    Instance.Reset();
                }
            });
            debugCfgs.Add(new DebugCfg()
            {
                TitleStr = "打开引导",
                ClickCallBack = (arg1, arg2) =>
                {
                    Instance.OpenGuide = true;
                    Instance.Reset();
                }
            });
            debugCfgs.Add(new DebugCfg()
            {
                TitleStr = "清除指定引导",
                ClickCallBack = (arg1, arg2) =>
                {
                    int.TryParse(arg1, out int currentGuideId);
                    StorageDecorationGuide storage = StorageManager.Instance.GetStorage<StorageDecorationGuide>();
                    if (storage.GuideData.ContainsKey(currentGuideId))
                    {
                        storage.GuideData.Remove(currentGuideId);
                    }
                    // UIManager.Instance.CloseUI<DebugUiController>();
                }
            });

            debugCfgs.Add(new DebugCfg()
            {
                TitleStr = "完成引导",
                ClickCallBack = (arg1, arg2) =>
                {
                    int.TryParse(arg1, out int currentGuideId);
                    Instance.CurrentGuideId = currentGuideId;
                    Instance.SaveGuide();
                    Instance.Reset();
                    // UIManager.Instance.CloseUI<DebugUiController>();
                }
            });
            debugCfgs.Add(new DebugCfg()
            {
                TitleStr = "清除所有引导",
                ClickCallBack = (arg1, arg2) =>
                {
                    StorageDecorationGuide storage = StorageManager.Instance.GetStorage<StorageDecorationGuide>();
                    storage.GuideData.Clear();
                    // UIManager.Instance.CloseUI<DebugUiController>();
                }
            });

            return debugCfgs;
        }
    }
}