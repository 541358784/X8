using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Deco.Graphic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine.Tilemaps;
using Framework;
using Deco.World;
using Deco.Item;
using Deco.Stage;
using Deco.Node;
using SomeWhere;
using DragonU3DSDK.Config;
using DragonU3DSDK.Asset;
using Decoration;
using SomeWhere;
namespace Deco.Area
{
    using StorageDecoration=DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld=DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea=DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode=DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage=DragonU3DSDK.Storage.Decoration.StorageStage;

    public class DecoArea : DecoGraphicHost<DecoAreaGraphic>
    {
        public enum Status
        {
            Lock = 0, //锁定中
            Unlock, //已解锁
            MaskRemove, //迷雾已驱散
            Complete, //已完成
        }

        internal DecoAreaData _data;
        internal World.DecoWorld _world = null;
        internal Dictionary<int, Stage.DecoStage> _stageDict = new Dictionary<int, Stage.DecoStage>();


        public int Id => _data._config.id;
        public TableAreas Config => _data._config;
        public StorageArea Storage => _data._storage;
        public World.DecoWorld World => _world;
        public GameObject GameObject => Graphic.gameObject;
        public bool IsUnlock => _data._storage == null ? false : _data._storage.State != (int)Status.Lock;
        public bool IsDisplayUnlock => _data._storage.State >= (int)Status.Unlock;
        public bool Unlocked => _data.unlocked;
        public bool IsFinish => _data._storage.State == (int)Status.Complete;


        public DecoArea(TableAreas config, StorageArea storage, DecoWorld world)
        {
            _data = new DecoAreaData(this, config, storage);
            Graphic = new DecoAreaGraphic(this);
            _world = world;

            DecoWorld.AreaLib[config.id] = this;

            initStages();
        }

        private void initStages()
        {
            if (_data._config.stages == null || _data._config.stages.Length == 0) return;

            var configManager = DecorationConfigManager.Instance;

            foreach (var stageId in _data._config.stages)
            {
                var stageConfig = configManager.GetStageConfig(stageId);
                //var stageStorage = UserDataMoudule.GetStorageStage(_data._storage, stageId);
                var stageStorage = AssetCheckManager.Instance.GetStorageStage(_data._storage, stageId);
                var stage = new Stage.DecoStage(stageConfig, stageStorage, this);
                _stageDict[stageId] = stage;
            }
        }

        public override void UnloadGraphic()
        {
            Graphic.Unload();
        }

        public override void AsyncLoadGraphic(GameObject parentObj, bool isPreview, Action onFinished)
        {
        }

        public override void LoadGraphic(GameObject parentObj)
        {
            Graphic.Load(parentObj.transform);
        }

        public void Update(float deltaTime)
        {
            //updateNpc(deltaTime);
        }

        public void Show(bool fromLogin, bool refreshTilemapDirty = false)
        {
            Graphic.Show(fromLogin, refreshTilemapDirty);
        }

        public bool TryFinish()
        {
            return _data.TryFinish();
        }

        public void RefreshMask(bool anim)
        {
            Graphic?.RefreshMask(anim);
        }

        public void Unlock(bool canUnlcokNewMap)
        {
            _data.unlock(canUnlcokNewMap);

            if (Graphic._resReady)
            {
                StorySubSystem.Instance.Trigger(StoryTrigger.UnLockNewArea, Config.id.ToString(), null, null);
            }
        }

        public void UpdateSuggestNode()
        {
            foreach (var stage in _stageDict)
            {
                stage.Value.UpdateSuggestNode();
            }
        }

        public bool TouchTest(Vector2 screenPos)
        {
            return Graphic.touchTest(screenPos);
        }

        public Node.DecoNode NodeTouchTest(Vector2 screenPos)
        {
            Node.DecoNode touchedNode = null;
            var touchResult = new DecoItemTouchResult();
            foreach (var stage in _stageDict.Values)
            {
                var temp = stage.TouchTest(screenPos, out Node.DecoNode tempNode);
                if (temp.result && Deco.World.DecoWorld.FrontTest(temp.z, touchResult.z))
                {
                    touchResult = temp;
                    touchedNode = tempNode;
                }
            }

            return touchedNode;
        }

        public void SetResReady()
        {
            Graphic._resReady = true;
            RefreshMask(false);
        }

        public void ApplyItemList(List<int> itemIdList)
        {
            foreach (var itemID in itemIdList)
            {
                if (DecoWorld.ItemLib.TryGetValue(itemID, out var item))
                {
                    if (item.Node.IsOwned && item.IsOwned)
                    {
                        item.Node.ChangeItem(itemID);
                    }
                }
            }
        }

        public void PreviewStage(int themeId, int stageId)
        {
            preview(themeId, stageId, false);
        }

        private void preview(int themeId, int stageId, bool exit)
        {
            var itemList = new List<int>();

            void previewItem(DecoNode node, int itemId)
            {
                if (exit)
                {
                    node.EndPreview();
                }
                else
                {
                    node.PreviewItem(itemId, false);
                }
            }

            foreach (var stage in _stageDict.Values)
            {
                foreach (var node in stage._nodeDict.Values)
                {
                    var stageItemListContainsItem = false;
                    if (itemList.Contains(node.Config.defaultItem))
                    {
                        stageItemListContainsItem = true;
                        previewItem(node, node.Config.defaultItem);
                    }
                    else if (node.Config.itemList != null)
                    {
                        foreach (var itemid in node.Config.itemList)
                        {
                            if (itemList.Contains(itemid))
                            {
                                stageItemListContainsItem = true;
                                previewItem(node, itemid);
                                break;
                            }
                        }
                    }

                    if (!stageItemListContainsItem)
                    {
                        if (exit)
                        {
                            node.EndPreview();
                        }
                        else
                        {
                            node.PreviewItem(-1);
                        }
                    }
                }
            }
        }

        public bool HaveOwnedNode()
        {
            if (_stageDict == null || _stageDict.Count == 0)
                return false;

            foreach (var keyValuePair in _stageDict)
            {
                if (keyValuePair.Value.HaveOwnedNode())
                    return true;
            }

            return false;
        }
    }
}