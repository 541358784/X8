using ABTest;
using DragonU3DSDK.Storage;

namespace Difference
{
    public class DifferenceManager : Singleton<DifferenceManager>
    {
        public bool IsOpenDifference { get; set; }
        public bool IsOpenSecondGuide { get; set; }
        public bool Is_Plan_C { get; set; }

        public const string OpenDiffKey = "OpenDiffKey";
        private bool _isInit = false;

        public const string AB_SecondGuideKey = "AB_SecondGuideKey";
        public const string AB_GuidePlan_C = "AB_GuidePlan_C";
        
        public void Reset()
        {
            _isInit = false;
        }
        
        public void Init()
        {
            if(_isInit)
                return;
            
            bool isFirstInit = false;
            IsOpenDifference = IsDiffPlan_A(ref isFirstInit);

            if (IsOpenDifference)
            {
                GlobalConfigManager.Instance.GuideDataRecombine();

                if (isFirstInit)
                    MergeManager.Instance.AdaptBoard();
            }
            
            _isInit = true;
        }

        public bool IsDiffPlan_New()
        {
            return IsDiffPlan_B() || IsDiffPlan_C();
        }
        
        public bool IsDiffPlan_A(ref bool isFirst)
        {
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(OpenDiffKey))
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[OpenDiffKey] = ABTestManager.Instance.IsOpenGuideTest() ? "1" : "0";
                isFirst = true;
            }

            IsOpenDifference = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[OpenDiffKey] == "1";
            
            if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(OpenDiffKey))
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests[OpenDiffKey] = IsOpenDifference ? "1" : "0";
            }

            return IsOpenDifference;
        }
        
        public bool IsDiffPlan_B()
        {
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AB_SecondGuideKey))
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AB_SecondGuideKey] = ABTestManager.Instance.IsOpenSecondGuide() ? "1" : "0";
            }

            IsOpenSecondGuide = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AB_SecondGuideKey] == "1";
            
            if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AB_SecondGuideKey))
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AB_SecondGuideKey] = IsOpenSecondGuide ? "1" : "0";
            }

            return IsOpenSecondGuide;
        }

        public bool IsDiffPlan_C()
        {
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AB_GuidePlan_C))
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AB_GuidePlan_C] = ABTestManager.Instance.IsOpenGuidePlan_C() ? "1" : "0";
            }

            Is_Plan_C = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AB_GuidePlan_C] == "1";
            
            if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AB_GuidePlan_C))
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AB_GuidePlan_C] = Is_Plan_C ? "1" : "0";
            }

            return Is_Plan_C;
        }

    }
}