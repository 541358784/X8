using DragonPlus.Config.TMatch;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using TMatch;
namespace DragonPlus
{
    public partial class GameBIManager
    {
        public BiEventAdventureIslandMerge.Types.LevelInfoTm LevelInfo = new BiEventAdventureIslandMerge.Types.LevelInfoTm();
        
        public void Init()
        {
            TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.TMATCH_GAME_ENTER, OnEnterEvt); 
        }

        public void Release()
        {
            TMatch.EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.TMATCH_GAME_ENTER, OnEnterEvt);  
        }
        
        private void OnEnterEvt(TMatch.BaseEvent evt)
        {
            var mainLevel = TMatchSystem.LevelController.LevelData.level;
            var layouCfg = TMatchSystem.LevelController.LevelData.layoutCfg;
            var useLighting = (StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseClock ||
                               UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMClockInfinity));
            var useClock = (StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseLighting ||
                            UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMLightingInfinity));

            EnterLevel(mainLevel, layouCfg, useClock, useLighting);
        }

        public void EnterLevel(int mainLevel, Layout layoutCfg, bool useLighting, bool useClock)
        {
            var buffWinTimes = StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt;
            var totalTaskCnt = 0;
            foreach (var cnt in layoutCfg.targetItemCnt)
            {
                totalTaskCnt += cnt;
            }

            var totalThingCnt = totalTaskCnt;
            if (layoutCfg.normalItemCnt != null)
            {
                foreach (var cnt in layoutCfg.normalItemCnt)
                {
                    totalThingCnt += cnt;
                }
            }
            
            LevelInfo = new BiEventAdventureIslandMerge.Types.LevelInfoTm()
            {
                LevelCount = (uint)mainLevel,
                LevelId = (uint)layoutCfg.id,
                LevelDifficulty = (uint)layoutCfg.difficultyMark,
                Lightning = useLighting,
                Clock = useClock,
                LevelResult = "enter",
                BuffWin = buffWinTimes > 3 ? 3 : (uint)buffWinTimes,
                LeftStepCount = (uint)totalTaskCnt,
                LeftGoalCount = (uint)totalTaskCnt,
                TotalTime = (uint)layoutCfg.levelTimes,
                EnterTime = (uint)StorageManager.Instance.GetStorage<StorageTMatch>().MainLevelFailCnt + 1,
                EnergyInfinite = EnergyModel.Instance.IsEnergyUnlimited(),
                LightningUse = UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMLightingInfinity),
                TimingUse = UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMClockInfinity),
                InitialThingCount = (uint)totalThingCnt,
                CollectionCount = 0,
            };

            SendLevelInfoEvent();
        }

        public void SendLevelInfoEvent()
        {
            DragonPlus.GameBIManager.Instance.SendEvent(LevelInfo);
        }
    }
}