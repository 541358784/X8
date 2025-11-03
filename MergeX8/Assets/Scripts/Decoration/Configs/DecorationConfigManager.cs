using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DragonU3DSDK.Asset;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Config;

namespace Decoration
{
    public class DecorationConfigManager : Singleton<DecorationConfigManager>
    {
        public List<TableItem> ItemConfigList;
        public List<TableStages> StageList;
        public List<TableNodes> nodeConfigs;
        public List<TableWorlds> WorldConfigs;
        public List<TableAreas> AreaConfigList;
        public List<TableDays> DaysConfigs;
        
        public List<TableNpcConfig> NpcConfigList;
        public List<TableNpcInfo> NpcInfoList;
        
        private Dictionary<int, List<TableStages>> _areaStagesDic;
        private Dictionary<int, List<TableNodes>> _areaNodesDic;
        private Dictionary<int, int> _areaNodesMaping;


        public List<TableInteractElement> InteractElementList;
        public List<TableInteractLogic> InteractLogicList;
        public List<TablePathMoveItem> TablePathMoveItemList;
        
        public List<TableGlobalConfigNumber> GlobalConfigNumberList;
        private Dictionary<string, TableGlobalConfigNumber> globalConfigNumberDic;

        bool isInit = false;
        public int LastNodeId;
        public void InitConfigs(bool checkInit = true)
        {
            if (checkInit && isInit) 
                return;

            globalConfigNumberDic = null;
          
            ItemConfigList = TableManager.DeSerialize<TableItem>(LoadJson("item"));
            nodeConfigs = TableManager.DeSerialize<TableNodes>(LoadJson("nodes"));
            WorldConfigs = TableManager.DeSerialize<TableWorlds>(LoadJson("worlds"));
            AreaConfigList = TableManager.DeSerialize<TableAreas>(LoadJson("areas"));
            StageList = TableManager.DeSerialize<TableStages>(LoadJson("stages"));
            DaysConfigs = TableManager.DeSerialize<TableDays>(LoadJson("days"));
            InteractElementList= TableManager.DeSerialize<TableInteractElement>(LoadJson("interactelement"));
            InteractLogicList= TableManager.DeSerialize<TableInteractLogic>(LoadJson("interactlogic"));
            TablePathMoveItemList= TableManager.DeSerialize<TablePathMoveItem>(LoadJson("pathmoveitem"));
            
            GlobalConfigNumberList = TableManager.DeSerialize<TableGlobalConfigNumber>(LoadJson("globalconfignumber"));
            
            NpcConfigList = TableManager.DeSerialize<TableNpcConfig>(LoadJson("NpcConfig"));
            NpcInfoList = TableManager.DeSerialize<TableNpcInfo>(LoadJson("NpcInfo"));
            
            _areaStagesDic = new Dictionary<int, List<TableStages>>();
            _areaNodesDic = new Dictionary<int, List<TableNodes>>();
            _areaNodesMaping = new Dictionary<int, int>();
            
            foreach (var area in AreaConfigList)
            {
                foreach (var stageId in area.stages)
                {
                    var stageCfg = StageList.Find(a => a.id == stageId);
                    if (stageCfg != null)
                    {
                        //stage
                        if (!_areaStagesDic.ContainsKey(area.id))
                        {
                            var tempStageList = new List<TableStages>();
                            tempStageList.Add(stageCfg);
                            _areaStagesDic.Add(area.id, tempStageList);
                        }
                        else
                        {
                            _areaStagesDic[area.id].Add(stageCfg);
                        }
                        //node
                        if (stageCfg.nodes != null)
                        {
                            foreach (var nodeId in stageCfg.nodes)
                            {
                                var nodeCfg = nodeConfigs.Find(a => a.id == nodeId);
                                if(nodeCfg == null)
                                    Debug.LogError("nodeCfg is null " + nodeId);
                                
                                if (!_areaNodesDic.ContainsKey(area.id))
                                {
                                    var tempNodeList = new List<TableNodes>();
                                    tempNodeList.Add(nodeCfg);
                                    _areaNodesDic.Add(area.id, tempNodeList);
                                }
                                else
                                {
                                    _areaNodesDic[area.id].Add(nodeCfg);
                                }
                                
                                _areaNodesMaping.Add(nodeCfg.id, area.id);
                            }
                        }
                    }
                }
            }

            isInit = true;
            for (var i = StageList.Count - 1; i >= 0; i--)
            {
                if (StageList[i].id / 10000 == 1)
                {
                    var areaData = AreaConfigList.Find(a => a.stages.Contains(StageList[i].id));
                    if (areaData != null && !areaData.hideAreaInDeco)
                    {
                        LastNodeId = StageList[i].nodes.Last();
                        break;
                    }
                }
            }
        }

        private string LoadJson(string tableName)
        {
            var path = $"Configs/Decoration/{tableName}";
            var ta = Utils.LoadResource<TextAsset>(path);
            
            if (string.IsNullOrEmpty(ta.text))
            {
                DebugUtil.LogError($"Load {path} error!");
                return "";
            }
            return ta.text;
        }
        
        public TableAreas GetAreaConfig(int areaId)
        {
            return AreaConfigList.Find(a => a.id == areaId);
        }

        public TableWorlds GetWorldConfig(int worldId)
        {
            return WorldConfigs.Find(c => c.id == worldId);
        }

        public TableNodes GetNodeConfig(int nodeID)
        {
            return nodeConfigs.Find(a => a.id == nodeID);
        }
        
        public TableItem GetItemConfig(int itemId)
        {
            return ItemConfigList.Find(a => a.id == itemId);
        }

        public int GetNodeBelongAreaID(int nodeID)
        {
            if (_areaNodesMaping.ContainsKey(nodeID))
                return _areaNodesMaping[nodeID];
            
            foreach (var kvpNodes in _areaNodesDic)
            {
                if (kvpNodes.Value.Find(a => a.id == nodeID) != null)
                {
                    return kvpNodes.Key;
                }
            }
            return -1;
        }

        public List<TableNodes> GetNodesByAreaID(int areaId)
        {
            if (_areaNodesDic.ContainsKey(areaId))
                return _areaNodesDic[areaId];

            return null;
        }

        public TableStages GetStageConfig(int stageId)
        {
            return StageList.Find(a => a.id == stageId);
        }

        public float GetGlobalConfigNumber(string key, float defalut = 0)
        {
            TableGlobalConfigNumber cfg = null;
            if (globalConfigNumberDic == null)
            {
                globalConfigNumberDic = new Dictionary<string, TableGlobalConfigNumber>(1024);
                var e = GlobalConfigNumberList.GetEnumerator();
                while (e.MoveNext())
                {
                    globalConfigNumberDic.Add(e.Current.key, e.Current);
                }
            }
            globalConfigNumberDic.TryGetValue(key, out cfg);
            if (cfg == null)
            {
                return defalut;
            }
            return cfg.value;
        }
    }
}