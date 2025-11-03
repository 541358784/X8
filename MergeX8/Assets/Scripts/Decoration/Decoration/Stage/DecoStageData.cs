using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine.Tilemaps;
using Framework;
using Deco.World;
using Decoration;
namespace Deco.Stage
{
    using StorageDecoration=DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld=DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea=DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode=DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage=DragonU3DSDK.Storage.Decoration.StorageStage;
    public class DecoStageData
    {
        private DecoStage _stage;

        internal TableStages _config = null;
        internal StorageStage _storage = null;
        internal bool isFinish => _storage.State == (int)DecoStage.Status.Finished;

        public DecoStageData(DecoStage area, TableStages config, StorageStage storage)
        {
            _stage = area;
            _config = config;
            _storage = storage;
        }

        public bool IsFirstNode(int nodeId)
        {
            if (_config == null || _config.nodes == null || _config.nodes.Length == 0)
                return false;

            return _config.nodes[0] == nodeId;
        }
        
        public bool TryFinish()
        {
            if (isFinish) return true;

            var stageFinish = true;

            foreach (var node in _stage._nodeDict.Values)
            {
                if (!node.DirtyTest() && !node.CanPassTest())
                {
                    stageFinish = false;
                    break;
                }
            }

            if (stageFinish)
            {
                //DragonPlus.GameBIManager.SendDecoEvent_FinishStage(_config.id);

                _storage.State = (int)DecoStage.Status.Finished;
                if (_config.nextStageId > 0)
                {
                    unlockNextStage();
                }
            }

            return stageFinish;
        }

        public bool unlockNextStage()
        {
            if (_config.nextStageId > 0)
            {
                var nextStage = World.DecoWorld.StageLib[_config.nextStageId];
                nextStage.Unlock();
                return true;
            }

            return false;
        }

        internal void unlock()
        {
            if (_storage.State == (int)DecoStage.Status.Lock)
            {
                _storage.State = (int)DecoStage.Status.Unlock;
            }
        }
    }
}