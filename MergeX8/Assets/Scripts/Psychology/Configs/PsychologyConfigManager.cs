using System.Collections.Generic;
using System.IO;
using Decoration.DynamicMap;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using UnityEditor;
using UnityEngine;

namespace Psychology
{
    public class PsychologyConfigManager : Manager<PsychologyConfigManager>
    {
        public List<TablePsychology> _configs;
        public List<TablePsychologyLevel> _configLevels;
        public List<TablePsychologyExplain> _configExplains;
        public List<TableBlueBlockLevel> BlueBlockLevelConfigs;
        public List<TableBlueBlockShape> BlueBlockShapeConfigs;
        public void InitTableConfigs()
        {
            TableManager.Instance.InitLocation("configs/Psychology");
        
            _configs = TableManager.Instance.GetTable<TablePsychology>();
            _configLevels = TableManager.Instance.GetTable<TablePsychologyLevel>();
            _configExplains = TableManager.Instance.GetTable<TablePsychologyExplain>();
            BlueBlockLevelConfigs = TableManager.Instance.GetTable<TableBlueBlockLevel>();
            BlueBlockShapeConfigs = TableManager.Instance.GetTable<TableBlueBlockShape>();
        }
    }
}