using System;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;

namespace ASMR
{
    public partial class Model : Manager<Model>
    {
        public List<TableMiniGameItem> GetMiniGameItem(UIPopupGameTabulationController.MiniGameTypeTab typeTab)
        {
            var configs = StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.46")?
                GlobalConfigManager.Instance.MiniGameGroupDictionary:
                GlobalConfigManager.Instance.MiniGameNewGroupDictionary;

            if (!configs.ContainsKey((int)typeTab))
                return null;

            return configs[(int)typeTab];
        }
    }
}