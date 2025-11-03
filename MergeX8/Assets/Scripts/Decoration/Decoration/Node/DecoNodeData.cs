using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using Spine.Unity;
using System;
using DG.Tweening;
using Framework;
using System.Linq;
using Deco.Area;
using Deco.World;
using Decoration;
using Decoration.DaysManager;
using Gameplay.UI.Capybara;

namespace Deco.Node
{
    using StorageDecoration=DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld=DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea=DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode=DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage=DragonU3DSDK.Storage.Decoration.StorageStage;
    public class DecoNodeData
    {
        private DecoNode _node;

        internal TableNodes _config = null;
        internal StorageNode _storage = null;

        /// <summary>
        /// 是否免费
        /// </summary>
        private bool IsFree
        {
            get
            {
                if (_config == null) 
                    return true;

                return _config.price < 0;
            }
        }

        public DecoNodeData(DecoNode node, TableNodes config, StorageNode storage)
        {
            _node = node;
            _config = config;
            _storage = storage;

            noDependenceTest();

            fixWrongState();
        }

        //修复清扫挂点因bug状态未完成bug
        private void fixWrongState()
        {
            if (_config.itemList != null && _config.itemList.Length == 1)   //直接做动画
            {
                if (_storage.Status == (int)DecoNode.Status.Owned)
                {
                    _storage.CurrentItemId = _config.itemList[0];
                    markNodeFinish();
                }
            }
        }

        internal void markNodeFinish()
        {
            _storage.Status = (int)DecoNode.Status.Finish;
            // if (CapybaraManager.Instance.IsOpenCapybara())
            // {
            //     if (_node.Id == 101098)
            //     {
            //         if(_node._stage._nodeDict.TryGetValue(101099, out DecoNode linkNode))
            //             linkNode.MarkNodeFinish();
            //     }
            // }
            if (CapybaraManager.Instance.IsOpenCapybara())
            {
                if (_node != null && _node._data != null && _node._data._config != null && _node.Id == 101098)
                {
                    if(_node._stage != null && _node._stage._nodeDict != null && _node._stage._nodeDict.TryGetValue(101099, out DecoNode linkNode))
                        linkNode.MarkNodeFinish();
                }
            }
        }

        internal void noDependenceTest()
        {
            if (!_node.Stage.Locked)
            {
                if ((int)DecoNode.Status.Lock == _storage.Status)
                {
                    if (_config.dependNodeIds == null || _config.dependNodeIds.Length == 0)
                    {
                        unlock();
                    }
                }

                if (_config.defaultOwned && _storage.Status < (int)DecoNode.Status.Owned)
                {
                    unlock();
                    markNodeFinish();
                    if (_config.itemList != null && _config.itemList.Length > 0)
                    {
                        changeItem(_config.itemList[0]);
                    }
                }
            }
        }

        internal bool dependenceTest()
        {
            var unlockNode = false;
            if ((int)DecoNode.Status.Lock == _storage.Status)
            {
                if (_config.dependNodeIds != null && _config.dependNodeIds.Length > 0)
                {
                    var allDependHasBuildings = true;
                    foreach (var nodeId in _config.dependNodeIds)
                    {
                        var changeNodeId = nodeId;
                        
                        if (_node._stage._nodeDict.TryGetValue(changeNodeId, out DecoNode node))
                        {
                            if (!node.DirtyTest())
                            {
                                allDependHasBuildings = false;
                            }
                        }
                        else
                        {
                            DecoNode findNode =  DecoManager.Instance.FindNode(changeNodeId);
                            if (findNode != null && !findNode.DirtyTest())
                                allDependHasBuildings = false;
                        }
                    }

                    if (allDependHasBuildings)
                    {
                        unlock();
                        unlockNode = true;
                    }
                }
            }

            return unlockNode;
        }

        internal void unlock()
        {
            if ((int)DecoNode.Status.Lock == _storage.Status)
            {
                _storage.Status = (int)DecoNode.Status.Unlock;
                if (IsFree) 
                    own();

                fixDaysNode();
            }
        }

        //改成days 模式 老用户兼容 剧情挂点全部解锁
        internal void fixDaysNode()
        {
            if(DaysManager.Instance.CompleteFixData)
                return;
            
            if (DaysManager.Instance.InitDays)
                return;

            if (_config.defaultItem == 0 &&
                (_config.itemList == null || _config.itemList.Length == 0 || _config.itemList[0] == 0))
            {
                //Debug.LogWarning("--------fix day node " + _config.id);
                markNodeFinish();
            }
        }
        
        internal void own()
        {
            if (_storage.Status < (int)DecoNode.Status.Owned)
            {
                _storage.Status = (int)DecoNode.Status.Owned;
                EventDispatcher.Instance.DispatchEvent(EventEnum.OwnDecoNode, this);
            }

            // if (_config.itemList!= null && _config.itemList.Length == 1)
            // {
            //     markNodeFinish();
            //     _storage.CurrentItemId = _config.itemList[0];
            //
            //     // if (!StorySubSystem.Instance.IsShowing && !MyMain.myGame.WorldGuideMgr.IsRunning)
            //     // {
            //     //     ExpModel.Instance.CheckLevelup();
            //     // }
            // }
        }

        // 建筑点上是否建有建筑且已解锁(不包含初始建筑)
        internal bool dirtyTest()
        {
            return _storage.Status == (int)DecoNode.Status.Finish;
        }

        //推荐挂点测试
        internal bool suggestTest()
        {
            if (_node.Stage.Area.World._data._storage.State != (int)DecoWorld.Status.Unlock) return false;

            //未解锁
            var nodeLock = _storage.Status == (int)DecoNode.Status.Lock;
            if (nodeLock)
            {
                // DebugUtil.LogError("suggestTest fail nodeLock:" + _config.id);
                return false;
            }

            //已安装
            if (dirtyTest())
            {
                // DebugUtil.LogError("suggestTest fail dirtyTest:" + _config.id);
                return false;
            }

            return true;
        }

        internal void changeItem(int itemId)
        {
            _storage.CurrentItemId = itemId;
        }
    }
}