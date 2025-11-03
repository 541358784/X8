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
using Deco.Item;
using Deco.Stage;
using Decoration;
namespace Deco.Area
{
    using StorageDecoration=DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld=DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea=DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode=DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage=DragonU3DSDK.Storage.Decoration.StorageStage;
    public class DecoAreaData
    {
        private DecoArea _area;

        internal TableAreas _config = null;
        internal StorageArea _storage = null;

        internal bool unlocked => _storage.State != (int)DecoArea.Status.Lock;
        public bool IsFinish => _storage.State == (int)DecoArea.Status.Complete;

        public DecoAreaData(DecoArea area, TableAreas config, StorageArea storage)
        {
            _area = area;
            _config = config;
            _storage = storage;

            checkDefaultOpen();

            //修正迷雾状态，防止上次中途退出，未驱散迷雾
            RemoveMask();
        }

        public void RemoveMask()
        {
            if (_storage.State == (int)DecoArea.Status.Unlock) _storage.State = (int)DecoArea.Status.MaskRemove;
        }

        internal void unlock(bool canUnlcokNewMap)
        {
            if (_storage.State == (int)DecoArea.Status.Lock)
            {
                _storage.State = (int)DecoArea.Status.Unlock;
                _area.RefreshMask(false);

                //解锁本Area内第一个Stage
                unlockFirstStage();
            }
        }

        #region 默认区域开启逻辑有点怪

        private void checkDefaultOpen()
        {
            //检测是否默认开启
            if (_storage.State == (int)DecoArea.Status.Lock)
            {
                if (_config.isDefaultUnlock > 0)
                {
                    unlock(true);
                }
            }
        }
        //第一个默认开启的Area内的第一个stage开启未通过Stage实例，此时还未创建Stage实例
        private void unlockFirstStage()
        {
            var stageId = _config.stages[0];

            if (_area._stageDict.Values.Count > 0)
            {
                foreach (var stage in _area._stageDict.Values)
                {
                    if (stage._data._config.id == stageId)
                    {
                        stage.Unlock();
                        break;
                    }
                }
            }
            else
            {
                //var stageStorage = UserDataMoudule.GetStorageStage(_storage, stageId);
                var stageStorage =Decoration.AssetCheckManager.Instance.GetStorageStage(_storage, stageId);
                if (stageStorage.State == (int)Stage.DecoStage.Status.Lock)
                {
                    stageStorage.State = (int)Stage.DecoStage.Status.Unlock;
                }
                stageStorage.State = (int)Stage.DecoStage.Status.Unlock;
            }

            //区域内无依赖挂点解锁测试
            if (_area._stageDict != null)
            {
                foreach (var stage in _area._stageDict.Values)
                {
                    stage.NodeNoDependenceTest();
                }
            }
        }

        #endregion

        public bool TryFinish()
        {
            if (IsFinish) return true;

            var areaFinish = true;
            foreach (var stage in _area._stageDict.Values)
            {
                if (!stage.IsFinish)
                {
                    areaFinish = false;
                    break;
                }
            }

            //如果Area内stage全部完成
            if (areaFinish)
            {
                _storage.State = (int)DecoArea.Status.Complete;
                unlockNextArea();
                _area.RefreshMask(false);
            }

            return areaFinish;
        }

        private void unlockNextArea()
        {
            if (_config.nextAreaId == 110)
            {
                if(ABTest.ABTestManager.Instance.IsLockMap())
                    return;
            }
            
            if (_config.nextAreaId > 0)
            {
                // 判断当前区域对应的最后一个map是否完成，如果未完成则不能解锁新的map，防止debug直接加key造成异常
                var nextArea = _area._world.AreaDict[_config.nextAreaId];
                if (!nextArea.IsUnlock)
                {
                    ////lianyi TODO:判断区域解锁的地方。当前版本不需要等级限制 2022-04-11
                    //CommonUtils.LogMerge("判断区域解锁的地方");
                    //if (nextArea.Config.unlockLevel > ExpModel.Instance.Level)
                    //{
                    //    //显示区域上锁Bubble
                    //}
                    //else
                    //{
                        //这里解锁区域
                        nextArea.Unlock(true);
                        //EventDispatcher.Instance.DispatchEvent(new ShowBubbleEvent(true));
                    //}
                  
                }
            }
        }
    }
}