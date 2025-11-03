using ABTest;
using DragonU3DSDK.Storage;
using UnityEngine.Serialization;

namespace Ditch.Model
{
    public partial class DitchModel
    {
        public const string AB_Ios_DitchPlan_D = "AB_Ditch_Plan_D";

        public bool Is_Ios_Plan_D = false;
        
        public bool Ios_Ditch_Plan_D()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AB_Ios_DitchPlan_D))
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AB_Ios_DitchPlan_D] = "0";
#endif
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AB_Ios_DitchPlan_D))
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AB_Ios_DitchPlan_D] = ABTestManager.Instance.IsOpenIosDitchPlan_D() ? "1" : "0";
            }

            Is_Ios_Plan_D = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AB_Ios_DitchPlan_D] == "1";
            
            if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AB_Ios_DitchPlan_D))
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AB_Ios_DitchPlan_D] = Is_Ios_Plan_D ? "1" : "0";
            }

            return Is_Ios_Plan_D;
        }
    }
}