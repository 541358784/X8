using ABTest;
using DragonU3DSDK.Storage;

namespace Filthy.Model
{
    public partial class FilthyModel
    {
        public const string AbKey = "MiniGameFilthyMergeGroup";
        
        public int GetSettingGroup()
        {
            return 1;
            // if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AbKey))
            // {
            //     StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] = ABTestManager.Instance.IsOpenMiniGameFilthy() ? "1" : "0";
            // }
            //
            // bool isOpen = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] == "1";
            //
            // if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AbKey))
            // {
            //     StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AbKey] = isOpen ? "1" : "0";
            // }
            //
            // return isOpen ? 1 : 2;
        }

        public bool IsProcedureB()
        {
            if (!IsOpenFilthyAndMerge())
                return false;

            return ABTestManager.Instance.GetFilthyAndMergeType() == 0;
        }
        
        public bool IsProcedureA()
        {
            if (!IsOpenFilthyAndMerge())
                return false;

            return ABTestManager.Instance.GetFilthyAndMergeType() == 1;
        }
        

        public bool IsOpenFilthyAndMerge()
        {
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AbKey))
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] = ABTestManager.Instance.IsOpenFilthyAndMerge() ? "1" : "0";
            }

            bool isOpen = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] == "1";
            
            if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AbKey))
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AbKey] = isOpen ? "1" : "0";
            }

            return isOpen;
        }
    }
}