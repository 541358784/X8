using System.Collections.Generic;
using DragonU3DSDK.Config;

namespace Stimulate.Configs
{
    public class StimulateConfigManager : Singleton<StimulateConfigManager>
    {
        public List<TableStimulateBoard> _stimulateBoard;
        public List<TableStimulateNodes> _stimulateNodes;
        public List<TableStimulateMerge> _stimulateMerge;
        public List<TableStimulateSetting> _stimulateSetting;
        public List<TableStimulateSpine> _stimulateSpine;

        public Dictionary<int, List<TableStimulateNodes>> _dicStimulateNodes = new Dictionary<int, List<TableStimulateNodes>>();
        public Dictionary<int, List<TableStimulateBoard>> _dicStimulateBoard = new Dictionary<int, List<TableStimulateBoard>>();
        
        public void InitConfig()
        {
            TableManager.Instance.InitLocation("configs/stimulate");

            _stimulateBoard = TableManager.Instance.GetTable<TableStimulateBoard>();
            _stimulateNodes = TableManager.Instance.GetTable<TableStimulateNodes>();
            _stimulateMerge = TableManager.Instance.GetTable<TableStimulateMerge>();
            _stimulateSetting = TableManager.Instance.GetTable<TableStimulateSetting>();
            _stimulateSpine = TableManager.Instance.GetTable<TableStimulateSpine>();
            
            _dicStimulateNodes.Clear();
            _dicStimulateBoard.Clear();
            
            foreach (var config in _stimulateNodes)
            {
                if (!_dicStimulateNodes.ContainsKey(config.levelId))
                    _dicStimulateNodes[config.levelId] = new List<TableStimulateNodes>();
                
                _dicStimulateNodes[config.levelId].Add(config);
            }
            
            foreach (var config in _stimulateBoard)
            {
                if (!_dicStimulateBoard.ContainsKey(config.boardId))
                    _dicStimulateBoard[config.boardId] = new List<TableStimulateBoard>();
                
                _dicStimulateBoard[config.boardId].Add(config);
            }
        }

        public List<TableStimulateNodes> GetNodes(int levelId)
        {
            if (_dicStimulateNodes.ContainsKey(levelId))
                return _dicStimulateNodes[levelId];

            return null;
        }
        
        public List<TableStimulateBoard> GetBoards(int boardId)
        {
            if (_dicStimulateBoard.ContainsKey(boardId))
                return _dicStimulateBoard[boardId];

            return null;
        }
    }
}