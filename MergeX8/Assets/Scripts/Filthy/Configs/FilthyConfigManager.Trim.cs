using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Filthy.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.Filthy
{
    public partial class FilthyConfigManager
    {
        private Dictionary<int, List<FilthySetting>> _settingMap = new Dictionary<int, List<FilthySetting>>();
        private Dictionary<int, List<FilthyProcedure>> _procedureAMap = new Dictionary<int, List<FilthyProcedure>>();
        private Dictionary<int, List<FilthyProcedure>> _procedureBMap = new Dictionary<int, List<FilthyProcedure>>();
        
        public List<FilthyProcedure> FilthyBProcedureConfig;
        
        protected override void Trim()
        {
            _settingMap.Clear();
            
            _settingMap.Add(1, new List<FilthySetting>());
            foreach (var config in FilthyASettingList)
            {
                _settingMap[1].Add(new FilthySetting(config));
            }
            
            _settingMap.Add(2, new List<FilthySetting>());
            foreach (var config in FilthyBSettingList)
            {
                _settingMap[2].Add(new FilthySetting(config));
            }
            
            _procedureAMap.Clear();
            foreach (var filthyProcedure in FilthyAProcedureList)
            {
                if (!_procedureAMap.ContainsKey(filthyProcedure.LevelId))
                    _procedureAMap[filthyProcedure.LevelId] = new List<FilthyProcedure>();
                
                _procedureAMap[filthyProcedure.LevelId].Add(new FilthyProcedure(filthyProcedure));
            }
            
            _procedureBMap.Clear();
            foreach (var filthyProcedure in FilthyBProcedureList)
            {
                if (!_procedureBMap.ContainsKey(filthyProcedure.LevelId))
                    _procedureBMap[filthyProcedure.LevelId] = new List<FilthyProcedure>();
                
                _procedureBMap[filthyProcedure.LevelId].Add(new FilthyProcedure(filthyProcedure));
            }
        }

        public List<FilthySetting> GetSettingConfig(int group)
        {
            if (_settingMap.ContainsKey(group))
                return _settingMap[group];

            return _settingMap[group];
        }

        public List<FilthySetting> GetSettingConfig()
        {
            return GetSettingConfig(FilthyModel.Instance.GetSettingGroup());
        }

        public List<FilthyProcedure> GetFilthyProcedures(int levelId)
        {
            Dictionary<int, List<FilthyProcedure>> _data = _procedureAMap;
            if (FilthyModel.Instance.IsProcedureB())
                _data = _procedureBMap;
            
            if (!_data.ContainsKey(levelId))
                return null;
            
            return _data[levelId];
        }
    }
}