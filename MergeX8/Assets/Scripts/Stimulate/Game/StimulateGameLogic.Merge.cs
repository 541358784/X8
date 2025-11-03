using System;
using DG.Tweening;
using DragonU3DSDK.Asset;
using Framework;
using Stimulate.Configs;
using Stimulate.Event;
using Stimulate.Model.Spine;
using Stimulate.System;
using UnityEngine;

namespace Stimulate.Model
{
    public partial class StimulateGameLogic : Manager<StimulateGameLogic>
    {
        private void LoadMergeLevel(TableStimulateNodes config)
        {
            ExitMergeLevel(true);
            
            UIManager.Instance.OpenUI(UINameConst.UIStimulateMergeMain, config);
        }
        
        private bool ExitMergeLevel(bool isInit = false)
        {
            var state = StimulateModel.Instance.GetNodeState(_nodeConfig.levelId, _nodeConfig.id);
            
            _spineConfig = null;
            _spineGameLogic = null;
            UIManager.Instance.CloseUI(UINameConst.UIStimulateMergeMain, true);

            if (!isInit && _nodeState != state)
                return true;

            return false;
        }
    }
}