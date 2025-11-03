using System;
using DragonPlus.Config.Filthy;
using Filthy.Model;
using Screw;

namespace Filthy.Game
{
    public partial class FilthyGameLogic
    {
        private SceneFsmScrewGame _subScrewGame;
        
        private void LoadScrewLevel(FilthyNodes config)
        {
            ExitMergeLevel(true);

            Action<object> action = (b) =>
            {
                ScrewAction(config, (bool)b);
            };

            _subScrewGame = new SceneFsmScrewGame();
            _subScrewGame.Enter(config.Param, action);
        }
        
        private bool ExitScrewLevel(bool isInit = false)
        {
            _subScrewGame?.Exit();
            _subScrewGame = null;
            
            var state = FilthyModel.Instance.GetNodeState(_nodeConfig.LevelId, _nodeConfig.Id);
            
            _spineConfig = null;
            _spineGameLogic = null;
            //UIManager.Instance.CloseUI(UINameConst.UIStimulateMergeMain, true);

            if (!isInit && _nodeState != state)
                return true;

            return false;
        }

        private void ScrewAction(FilthyNodes config, bool isSuccess)
        {
            _subScrewGame?.Exit();
            _subScrewGame = null;
            
            if(isSuccess)
                FilthyModel.Instance.OwnedNode(config.LevelId, config.Id);

            ExitGame(config);
        }
    }
}