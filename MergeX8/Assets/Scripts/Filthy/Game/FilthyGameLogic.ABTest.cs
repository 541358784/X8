using ABTest;
using DragonU3DSDK.Storage;
using Filthy.Model;
using UnityEngine;

namespace Filthy.Game
{
    public partial class FilthyGameLogic
    {
        public const string AbKey = "OpenFilthyGamefKey";
        
        public bool IsOpenFilthy()
        {
            if (!Makeover.Utils.IsOpen || !Makeover.Utils.IsOpenScrew)
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AbKey] = "0";
                return false;
            }

#if UNITY_ANDROID || UNITY_EDITOR
            if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.69"))
            {
                return false;
            }  
#endif
            
            if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.65"))
            {
                return FilthyModel.Instance.IsOpenFilthyAndMerge();
            }
            
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AbKey))
            {
                if (StorageManager.Instance.GetStorage<StorageHome>().IsFirstLogin && !StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.63"))
                {
                    StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] = "0";
                }
                else
                {
                    StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] = ABTestManager.Instance.IsOpenScrewFilthyGame() ? "1" : "0";
                }
            }

            bool isOpen = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] == "1";
            
            if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AbKey))
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AbKey] = isOpen ? "1" : "0";
            }

            return isOpen;
        }

        public bool IsSelectGame()
        {
            return StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey("FilthyGameSelect");
        }

        public void PopSelectGame()
        {
            if(IsSelectGame())
                return;

            UIManager.Instance.OpenUI(UINameConst.UIPopupScrewGameSelect);
        }
    }
}