using Decoration;
using Decoration.DaysManager;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace Makeover
{
    public class MakeoverModel : Singleton<MakeoverModel>
    {
        public StorageMakeOver MakeOver
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().MakeOver; }
        }

        public bool IsUnLock(TableMoLevel config)
        {
            if (config == null)
                return false;
            
            if (config.unlockNodeNum <= 0)
                return true;

            if (IsPlayLevel(config))
                return true;

            return DecoManager.Instance.GetOwnedNodeNum() >= config.unlockNodeNum;
        }

        public int GetBodyStep(TableMoLevel config)
        {
            if (config == null)
                return 0;

            if (MakeOver.BodyPartStep.ContainsKey(config.stepLevelId))
                return MakeOver.BodyPartStep[config.stepLevelId];

            return 0;
        }

        public void AddBodyStep(TableMoLevel config, int index)
        {
            if (config == null)
                return;
            
            if (!MakeOver.BodyPartStep.ContainsKey(config.stepLevelId))
                MakeOver.BodyPartStep.Add(config.stepLevelId, 0);

            MakeOver.BodyPartStep[config.stepLevelId] = index;
        }

        public bool IsPlayLevel(TableMoLevel config)
        {
            if (config == null)
                return false;

            if (IsFinish(config))
                return true;

            if (MakeOver.BodyPartStep.ContainsKey(config.stepLevelId) && MakeOver.BodyPartStep[config.stepLevelId] > 0)
                return true;

            return false;
        }
        
        public bool IsFinish(TableMoLevel config)
        {
            if (config == null)
                return false;

            return MakeOver.FinishInfo.ContainsKey(config.id);
        }

        public void FinishLevel(TableMoLevel config)
        {
            if(config == null)
                return;
            
            if(MakeOver.FinishInfo.ContainsKey(config.id))
                return;
            
            MakeOver.FinishInfo.Add(config.id, true);
        }

        public bool HaveNoFinishLevel()
        {
            foreach (var tableMoLevel in MakeoverConfigManager.Instance.levelList)
            {
                if (IsUnLock(tableMoLevel) && !IsFinish(tableMoLevel))
                    return true;
            }

            return false;
        }
        
        public bool IsShowHand()
        {
            for(int i = 0; i <  MakeoverConfigManager.Instance.levelList.Count; i++)
            {
                if (i == 3 && i == 4)
                {
                    if (IsUnLock(MakeoverConfigManager.Instance.levelList[i]) && !IsFinish(MakeoverConfigManager.Instance.levelList[i]))
                        return true;
                }
            }

            return false;
        }
    }
}