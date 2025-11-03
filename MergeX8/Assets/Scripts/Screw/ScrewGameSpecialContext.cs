using Decoration;
using Screw;
using Screw.Module;

namespace Screw
{
    public class ScrewGameSpecialContext : ScrewGameContext
    {
        public override void OnEnterLevel(int inLevelIndex, int inLevelId)
        {
            _screwAtlas = AssetModule.Instance.LoadSpriteAtlas(ConstName.ScrewAtlas);
            levelIndex = inLevelIndex;
            levelId = inLevelId;

            LoadLevelSetting();
            LoadLevelModel(true);
            LoadLevelView();

            gameState = ScrewGameState.InProgress;
            
            Record = new RecordAction(this);
            levelView.OnEnterLevel();
            
            hookContext.OnLogicEvent(LogicEvent.EnterLevel, null);
            
            PoolModule.Instance.CreatePool(ConstName.HammerEffect, 2);

            InitBiLevelInfo(false);
            UpdateScheduler.Instance.HookUpdate(Tick);
        }
    }
}