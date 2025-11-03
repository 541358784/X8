using DragonPlus.Config.ExtraEnergy;
using UnityEngine;

namespace ExtraEnergy
{
    public class ExtraEnergyModel : ActivityEntityBase
    {
        public override string Guid => "OPS_EVENT_TYPE_EXTRA_ENERGY";

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.ExtraEnergy);
        }

        private static ExtraEnergyModel _instance;
        public static ExtraEnergyModel Instance => _instance ?? (_instance = new ExtraEnergyModel());

    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitAuto()
        {
            Instance.Init();
        }

        public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
            ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
        {
            base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
                activitySubType);
            ExtraEnergyConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }
        protected override void InitServerDataFinish()
        {
            base.InitServerDataFinish();
           
        }
        
        public ExtraEnergyActivityConfig GetExtraEnergyActivityConfigByCount(int buyCount)
        {
            int index = ExtraEnergyConfigManager.Instance.ExtraEnergyActivityConfigList.Count - 1;
            if(buyCount<ExtraEnergyConfigManager.Instance.ExtraEnergyActivityConfigList.Count-1)
                index=buyCount;
            return ExtraEnergyConfigManager.Instance.ExtraEnergyActivityConfigList[index];
        }

        public override bool IsOpened(bool hasLog = false)
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ExtraEnergy))
                return false;
            bool isOpen = base.IsOpened();
            if (!isOpen)
                return false;

            return base.IsOpened(hasLog);
        }
        
        private static string coolTimeKey = "ExtraEnergy";
        public static bool CanShowUI()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ExtraEnergy))
                return false;

            if (!Instance.IsOpened())
                return false;

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupExtraEnergyStart);
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                return true;
            }

            return false;
        }

    }
}
