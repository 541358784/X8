using System.Collections.Generic;
using System.Linq;
using Activity.GardenTreasure.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace DragonPlus.Config.GardenTreasure
{
    public partial class GardenTreasureConfigManager
    {
        public class ShapeData
        {
            public int _index;
            public string _shapeSize = "";
            public List<Vector2Int> _shapeGrids = new List<Vector2Int>();
            public Vector2Int _size = Vector2Int.zero;
        }
        public class BoardData
        {
            public string _boardSize = "";
            public List<ShapeData> _shapeDatas = new List<ShapeData>();
        }

        private Dictionary<int, BoardData> _board = new Dictionary<int, BoardData>();

        public List<GardenTreasureLevelConfig> _normalLevelConfigs
        {
            get
            {
                var configs = new List<GardenTreasureLevelConfig>();
                foreach (var config in GardenTreasureLevelConfigListByPayLevel)
                {
                    if (config.IsRandom)
                    {
                        // _randomLevelConfig = config;   
                    }
                    else
                        configs.Add(config);
                }
                return configs;
            }
        }

        public GardenTreasureLevelConfig _randomLevelConfig
        {
            get
            {
                foreach (var config in GardenTreasureLevelConfigListByPayLevel)
                {
                    if (config.IsRandom)
                    { 
                        return config;   
                    }
                }
                return null;
            }
        }

        private Dictionary<int, List<GardenTreasureSetingConfig>> _configsMap = new Dictionary<int, List<GardenTreasureSetingConfig>>();

        public List<GardenTreasureLevelConfig> GardenTreasureLevelConfigListByPayLevel
        {
            get
            {
                var payLevelGroup = GardenTreasureModel.Instance.PayLevelGroup();
                var configs = new List<GardenTreasureLevelConfig>();
                var allConfigs = GardenTreasureLevelConfigList;
                foreach (var config in allConfigs)
                {
                    if (config.PayLevelGroup == payLevelGroup)
                        configs.Add(config);
                }
                if (configs.Count == 0)
                {
                    foreach (var config in allConfigs)
                    {
                        if (config.PayLevelGroup == 0)
                            configs.Add(config);
                    }
                }
                return configs;
            }
        }
        
        public void Trim()
        {

            foreach (var config in GardenTreasureBoardConfigList)
            {
                Trim(_board, config.Id, config.LevelConfig);
            }
            
            _configsMap.Clear();
            foreach (var config in GardenTreasureSetingConfigList)
            {
                if (!_configsMap.ContainsKey(config.PayLevelGroup))
                    _configsMap[config.PayLevelGroup] = new List<GardenTreasureSetingConfig>();
                
                _configsMap[config.PayLevelGroup].Add(config);
            }
        }

        private void Trim(Dictionary<int, BoardData> board, int id, List<string> levelConfig)
        {
            BoardData boardData = new BoardData();

            boardData._boardSize = levelConfig[0];
            for (int i = 1; i < levelConfig.Count; i++)
            {
                var config = levelConfig[i];
                var splitStrings = config.Split(';');
                
                ShapeData shapeData = new ShapeData();
                shapeData._shapeSize = splitStrings[0];
                shapeData._index = i-1;

                int size = 0;
                int.TryParse(shapeData._shapeSize, out size);
                shapeData._size.x = size / 10;
                shapeData._size.y = size % 10;
                
                for (int j = 1; j < splitStrings.Length; j++)
                {
                    var shapeString = splitStrings[j].Split(':');
                    
                    Vector2Int gridxy = new Vector2Int(int.Parse(shapeString[0]), int.Parse(shapeString[1]));
                    shapeData._shapeGrids.Add(gridxy);
                }
                
                boardData._shapeDatas.Add(shapeData);
            }
            
            board.Add(id, boardData);
        }

        public BoardData GetBoardData(int id)
        {
            if (_board.ContainsKey(id))
                return _board[id];

            return null;
        }

        public List<GardenTreasureSetingConfig> GetSettingConfig()
        {
            int group = GardenTreasureModel.Instance.PayLevelGroup();

            if (_configsMap.ContainsKey(group))
                return _configsMap[group];

            return _configsMap[_configsMap.Keys.First()];
        }
    }
}