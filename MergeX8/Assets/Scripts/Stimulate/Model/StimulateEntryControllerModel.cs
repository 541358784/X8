using Decoration;
using Decoration.DaysManager;
using DragonPlus;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Stimulate;
using Stimulate.Configs;
using Stimulate.Model;
using UnityEngine;

namespace StimulateSpace
{
    public class StimulateEntryControllerModel : Singleton<StimulateEntryControllerModel>
    {
        public bool IsUnLock(TableStimulateSetting config)
        {
            if (config == null)
                return false;
            
            if (config.unlockNodeNum <= 0)
                return true;

            if (IsPlayLevel(config))
                return true;

            return DecoManager.Instance.GetOwnedNodeNum() >= config.unlockNodeNum;
        }

        public bool IsPlayLevel(TableStimulateSetting config)
        {
            if (config == null)
                return false;

            if (IsFinish(config))
                return true;

            return false;
        }
        
        public bool IsFinish(TableStimulateSetting config)
        {
            if (config == null)
                return false;

            return StimulateModel.Instance.IsFinish(config);
        }
    }
}