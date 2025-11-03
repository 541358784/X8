using System;
using Decoration;
using Screw;
using Screw.Module;

namespace Screw
{
    public class ScrewGameKapiScrewContext : ScrewGameContext
    {
        public enum ScrewGameCallbackType
        {
            Win,
            FullFail,
            BlockerFail,
        }
        public Action<object> _action { get; set; }
        
        public void OnWin()
        {
            _action?.Invoke(ScrewGameCallbackType.Win);
        }
        public void OnBlockerFail()
        {
            _action?.Invoke(ScrewGameCallbackType.BlockerFail);
        }
        public void OnFullFail()
        {
            _action?.Invoke(ScrewGameCallbackType.FullFail);
        }
        public override async void OnGiveUp()
        {
            SendLevelFailBi();

            if (afterFailLevelHandlers != null && afterFailLevelHandlers.Count > 0)
            {
                for (int i = 0; i < afterFailLevelHandlers.Count; i++)
                {
                    await afterFailLevelHandlers[i].Invoke(this);
                }
            }
            UIModule.Instance.ShowUI(typeof(UIKapiScrewTryAgainPopup), this, true);
        }
    
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