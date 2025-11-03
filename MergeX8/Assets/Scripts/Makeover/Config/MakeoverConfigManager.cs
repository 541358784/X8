using System.Collections.Generic;
using DragonU3DSDK.Config;
using Framework;

namespace DragonPlus.Config.Makeover
{
    public class MakeoverConfigManager: Singleton<MakeoverConfigManager>
    {   
        public List<TableAction> actionList;
        public List<TableCustomerTimeLine> timeLineList;
        public List<TableMoLevel> levelList;
        public List<TableStep> stepList;
        public List<TableAsmrStepNew> stepNewList;
        public List<TableAsmrGroup> groupList;
        
        public void InitConfig()
        {
            TableManager.Instance.InitLocation("configs/makeover");

            actionList = TableManager.Instance.GetTable<TableAction>();
            timeLineList = TableManager.Instance.GetTable<TableCustomerTimeLine>();
            levelList = TableManager.Instance.GetTable<TableMoLevel>();
            stepList = TableManager.Instance.GetTable<TableStep>();
            stepNewList = TableManager.Instance.GetTable<TableAsmrStepNew>();
            groupList = TableManager.Instance.GetTable<TableAsmrGroup>();
        }
        
        public List<TableStep> GetStepsByLevelId(int levelId)
        {
            return stepList.FindAll(s=>s.levelId==levelId);
        }
        
        public List<TableMoLevel> GetLevels()
        {
            return levelList;
        }
    }
}