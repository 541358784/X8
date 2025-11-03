public partial class UIPopupKeepPetTaskController
{
    public void CheckKeepPetDailyTaskInfo1Guide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetDailyTaskInfo1))
        {
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetDailyTaskInfo1, ""))
            {

            }
        }
    }
    public void CheckKeepPetDailyTaskInfo2Guide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetDailyTaskInfo2))
        {
            var curLevel = KeepPetModel.Instance.Storage.Exp.KeepPetGetCurLevelConfig();
            if (curLevel.Id >= KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel)
            {
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetDailyTaskInfo2, ""))
                {
            
                }    
            }
        }
    }
}