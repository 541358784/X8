using System.Collections.Generic;
using DragonPlus.Config.MiniGame;
using DragonU3DSDK.Storage;
using AsmrGroupConfig = DragonPlus.Config.MiniGame.AsmrGroupConfig;
using AsmrLevelConfig = DragonPlus.Config.MiniGame.AsmrLevelConfig;
using AsmrStepConfig = DragonPlus.Config.MiniGame.AsmrStepConfig;

namespace ASMR
{
    public partial class ASMRModel : Manager<ASMRModel>
    {
        public struct AttachData
        {
            public int chapterId;
            public int subLevelId;
            public int levelId;


            public AttachData(int chapterId, int subLevelId, int levelId)
            {
                this.chapterId = chapterId;
                this.subLevelId = subLevelId;
                this.levelId = levelId;
            }
        }

        private AsmrLevelConfig _levelConfig;
        public AttachData AttData;

        public List<AsmrLevelConfig> AsmrLevelConfigs;
        public List<AsmrGroupConfig> AsmrGroupConfigs;
        public List<AsmrStepConfig> AsmrStepConfigs;

        private ASMRModel()
        {
        }

        public void Init()
        {
            AsmrLevelConfigs = MiniGameConfigManager.Instance.AsmrLevelConfigList;
            AsmrGroupConfigs = MiniGameConfigManager.Instance.AsmrGroupConfigList;
            AsmrStepConfigs = MiniGameConfigManager.Instance.AsmrStepConfigList;
        }

        //获取某一个关存档信息
        public StorageASMRLevel GetLevelStorageByLevelId(int levelId)
        {
            var storageMain = StorageManager.Instance.GetStorage<StorageASMR>();
            var levelInfo = storageMain.LevelInfos.Find(c => c.Id == levelId);
            if (levelInfo == null)
            {
                levelInfo = new StorageASMRLevel() { Id = levelId };
                storageMain.LevelInfos.Add(levelInfo);
            }

            return levelInfo;
        }

        public void FastFinishCurrentLevel()
        {
            if(_asmrLevel == null)
                return;
            _asmrLevel.FastFinishCurrentLevel();
        }
    }
}