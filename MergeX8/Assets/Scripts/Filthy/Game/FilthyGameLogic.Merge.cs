using System;
using DragonPlus.Config.Filthy;
using Filthy.Model;

namespace Filthy.Game
{
    public partial class FilthyGameLogic
    {
        private void LoadMergeLevel(FilthyNodes config)
        {
            ExitMergeLevel(true);
            
            UIManager.Instance.OpenUI(UINameConst.UIFilthyMergeMain, config);
        }
        
        private bool ExitMergeLevel(bool isInit = false)
        {
            var state = FilthyModel.Instance.GetNodeState(_nodeConfig.LevelId, _nodeConfig.Id);
            
            _spineConfig = null;
            _spineGameLogic = null;
            UIManager.Instance.CloseUI(UINameConst.UIFilthyMergeMain, true);

            if (!isInit && _nodeState != state)
                return true;

            return false;
        }
    }
}