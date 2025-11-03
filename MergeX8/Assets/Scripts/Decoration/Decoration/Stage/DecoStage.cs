using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine.Tilemaps;
using Framework;
using Deco.Area;
using Deco.Graphic;
using Deco.Item;
using Deco.World;
using Deco.Node;
using Decoration;
namespace Deco.Stage
{
    using StorageDecoration=DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld=DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea=DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode=DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage=DragonU3DSDK.Storage.Decoration.StorageStage;
    public class DecoStage : DecoGraphicHost<DecoStageGraphic>
    {
        public enum Status
        {
            Lock = 0, //锁定中
            Unlock, //已解锁，可购买
            Finished, //已完成
        }

        internal DecoStageData _data;
        private Area.DecoArea _area = null;

        internal Dictionary<int, Node.DecoNode> _nodeDict = new Dictionary<int, Node.DecoNode>();

        public Dictionary<int, Node.DecoNode> NodeDic
        {
            get { return _nodeDict; }
        }

        public bool Locked
        {
            get => _data._storage.State == (int)Status.Lock;
        }

        public bool IsFinish => _data.isFinish;

        internal Area.DecoArea Area
        {
            get => _area;
            set => _area = value;
        }

        public DecoStage(TableStages config, StorageStage storage, Area.DecoArea area)
        {
            _data = new DecoStageData(this, config, storage);
            Graphic = new DecoStageGraphic(this);
            _area = area;

            World.DecoWorld.StageLib[config.id] = this;

            initNodes();

            Action action = NodeDependenceTest;
            if (RecoverCoinModel.Instance.IsCurWeekExistByStorage())
            { 
                RecoverCoinModel.Instance.PushActionAfterWeekEnd(action);
                return;
            }
            else
            {
                action.Invoke();
            }
        }


        private void initNodes()
        {
            if (_data._config == null) return;
            if (_data._config.nodes == null || _data._config.nodes.Length == 0) return;

            var configManager = DecorationConfigManager.Instance;

            foreach (var nodeId in _data._config.nodes)
            {
                if (_nodeDict.ContainsKey(nodeId)) continue;

                var nodeConfig = configManager.GetNodeConfig(nodeId);
                if (nodeConfig != null)
                {
                    // var nodeStorage = UserDataMoudule.GetStorageNode(_data._storage, nodeConfig.id);
                    var nodeStorage = AssetCheckManager.Instance.GetStorageNode(_data._storage, nodeConfig.id);
                    var node = new Node.DecoNode(nodeConfig, nodeStorage, this);
                    _nodeDict[nodeId] = node;
                }
                else
                {
                    DebugUtil.LogError(string.Format("## BuildingPointConfig [{0}] not exist in Area [{1}] ##", nodeId, _data._config.id));
                }
            }
        }

        public void NodeDependenceTest()
        {
            foreach (var node in _nodeDict.Values)
            {
                if (node._stage._area._data._config.id == 110)
                {
                    if(ABTest.ABTestManager.Instance.IsLockMap())
                        continue;
                }
                    
                node.DependenceTest();
            }
        }

        public void NodeNoDependenceTest()
        {
            foreach (var node in _nodeDict.Values)
            {
                node.NoDependenceTest();
            }
        }

        public void Show(bool fromLogin)
        {
            foreach (var node in _nodeDict.Values)
            {
                node.Show(fromLogin);
            }
        }

        public void UpdateSuggestNode()
        {
            foreach (var node in _nodeDict.Values)
            {
                node.UpdateSuggestNode();
            }
        }

        public bool TryFinish()
        {
            return _data.TryFinish();
        }

        public void Unlock()
        {
            _data.unlock();
            NodeNoDependenceTest();
        }

        public DecoItemTouchResult TouchTest(Vector2 screenPos, out Node.DecoNode touchedNode)
        {
            var touchResult = new DecoItemTouchResult();
            touchedNode = null;
            var nodes = _nodeDict.Values.ToList();
            for(int i = nodes.Count-1; i >= 0; i--)
            {
                var node = nodes[i];
                if (node.Locked) continue;

                var temp = node.TouchTest(screenPos);
                if (temp.result && World.DecoWorld.FrontTest(temp.z, touchResult.z))
                {
                    touchResult = temp;
                    touchedNode = node;
                }
            }

            return touchResult;
        }

        public override void UnloadGraphic()
        {
            Graphic.Unload();
        }

        public override void LoadGraphic(GameObject parentObj)
        {
            Graphic.Load(parentObj.transform);
        }
        
        public override void AsyncLoadGraphic(GameObject parentObj, bool isPreview, Action onFinished)
        {
        }


        public bool HaveOwnedNode()
        {
            if (NodeDic == null || NodeDic.Count == 0)
                return false;

            foreach (var keyValuePair in NodeDic)
            {
                if (keyValuePair.Value.IsOwned)
                    return true;
            }

            return false;
        }
    }
}