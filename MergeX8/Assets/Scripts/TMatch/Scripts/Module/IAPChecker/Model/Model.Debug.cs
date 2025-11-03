using System.Collections.Generic;
// using DragonPlus.Config.Game;
namespace IAPChecker
{
    public partial class Model
    {
        public static List<DebugCfg> GetDebugCfg()
        {
            return new List<DebugCfg>
            {
                new DebugCfg
                {
                    TitleStr = "补单",
                    ClickCallBack = (param1, param2) =>
                    {
                        var id = param1.ToInt();
                        var config = DragonPlus.Config.TMatchShop.TMatchShopConfigManager.Instance.ShopList.Find(item => item.id == id);
                        if (config == null)
                            return;
                        //TMatch.StoreModel.Instance.ForRewards(config, true);
                        // TMatch.UIManager.Instance.CloseUI<DebugUiController>();
                    }
                },
            };
        }
    }
}

