using System;
using Deco.Node;
using Decoration;
using Ditch.Merge;
using DragonPlus.Config.Ditch;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using UnityEngine;

namespace Ditch.Model
{
    public partial class DitchModel : Manager<DitchModel>
    {
        public StorageDitch Ditch
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().Ditch;
            }
        }

        public bool IsFinishLevel(int levelId)
        {
            return Ditch.FinishDitchLevel.ContainsKey(levelId);
        }

        public bool IsUnLockLevel(TableDitchLevel config)
        {
            if (config == null)
                return false;

            if (config.UnlockNodeId < 0)
                return true;
            
            DecoNode node = DecoManager.Instance.FindNode(config.UnlockNodeId);
            if (node == null)
                return false;

            return node.IsOwned;
        }

        public void FinishLevel(int levelId)
        {
            Ditch.FinishDitchLevel.TryAdd(levelId, levelId);
        }

        public bool HaveNoFinish()
        {
            foreach (var config in DitchConfigManager.Instance.TableDitchLevelList)
            {
                if(config.IsComingSoon)
                    continue;
                
                if (IsUnLockLevel(config) && !IsFinishLevel(config.Id))
                    return true;
            }

            return false;
        }
        
        public void EnterLevel(TableDitchLevel config)
        {
            UIDigTrenchNewMainController.Open(config);
        }

        public void EnterMergeGame(int levelId, Action<bool> action)
        {
            DitchMergeGameLogic.Instance.EnterGame(levelId, action);
        }
    }
}