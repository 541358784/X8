using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;
using ABTest;
using DG.Tweening;
using DragonU3DSDK.Asset;
using Deco.World;
using Deco.Item;
using Deco.Area;
using Deco.Node;
using Deco.Stage;
using Decoration.DynamicMap;
using DragonU3DSDK.Config;
using DragonU3DSDK.Network.API.Protocol;
using Farm.Model;
using SRF;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Gameplay;
using Framework;
using Manager;
using MiniGame;

namespace Decoration
{
    public enum DecoOperationType
    {
        None,
        Buy,
        Preview,
        Install,
        MiniGame,
    }
    
    public class DecoManager : GlobalSystem<DecoManager>
    {
        private DecoWorld _currentWorld = null;
        public DecoWorld CurrentWorld
        {
            get { return _currentWorld; }
        }
        private DecoNode _previewNode = null;

        private bool isWorldReady = false;

        public bool IsWorldReady
        {
            get => isWorldReady;
            private set => isWorldReady = value;
        }
        public static float biRealtime = Time.realtimeSinceStartup;
        public static bool LoadSinleWorld => QualityMgr.Level < QualityMgr.QualityLevel.Medium;

        private Dictionary<int, Transform> _worldItemRootDic = new Dictionary<int, Transform>();
        public bool EnableUpdate = false;
        public delegate void EnterCall(bool isSuccess);
        private bool _isPauseMove;
        public bool IsPauseMove
        {
            get { return _isPauseMove; }
        }

        GameObjectPoolManager _poolManager;
        public GameObjectPoolManager PoolManager
        {
            get
            {
                return _poolManager;
            }
        }

        public int CurrentWorldId
        {
            get
            {
                int worldId = StorageManager.Instance.GetStorage<StorageHome>().CurrentWorldId;
                if (worldId == 0)
                    StorageManager.Instance.GetStorage<StorageHome>().CurrentWorldId = 1;

                return StorageManager.Instance.GetStorage<StorageHome>().CurrentWorldId;
            }
            set
            {
                StorageManager.Instance.GetStorage<StorageHome>().CurrentWorldId = value;
            }
        }
        
        
        public bool isUnLocking = false;
        public void PauseAll()
        {
            _isPauseMove = true;
        }

        public void ResumeAll()
        {
            _isPauseMove = false;
        }
        public void Update(float deltaTime)
        {
            if (EnableUpdate) _currentWorld?.Update(deltaTime);
        }
        public Camera GetCamera()
        {
            return DecoSceneRoot.Instance.mSceneCamera;
        }
        public DecoArea FindArea(int id)
        {
            DecoWorld.AreaLib.TryGetValue(id, out var value);
            return value;
        }

        public DecoNode FindNode(int id)
        {
            DecoWorld.NodeLib.TryGetValue(id, out var value);
            return value;
        }
        
        public DecoItem FindItem(int id)
        {
            DecoWorld.ItemLib.TryGetValue(id, out var value);
            return value;
        }
        
        public DecoNode FindNodeByItem(int id)
        {
            DecoWorld.ItemLib.TryGetValue(id, out var value);
            if (value != null)
                return value.Node;
            
            return null;
        }
        
        public DecoManager()
        {
            registerEvent();

        }

        private HashSet<DecoNode> _previewNodes = new HashSet<DecoNode>();
        
        ~DecoManager()
        {
            unRegisterEvent();
        }
        private void registerEvent()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.UNSELECT_NODE, OnUnSelectNode);
            EventDispatcher.Instance.AddEventListener(EventEnum.SELECT_ONE_ITEM, OnPreviewItem);
        }

        private void unRegisterEvent()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.UNSELECT_NODE, OnUnSelectNode);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.SELECT_ONE_ITEM, OnPreviewItem);
        }

        public void LoadWorlds()
        {
            CleanAll();
            DecoExtendAreaManager.Instance.Init();
            //var worldMap = MyMain.myGame.ModuleMgr.Get<UserDataMoudule>()._storageDeco.WorldMap;

            var storageWorld = Decoration.AssetCheckManager.Instance.GetStorage();
            var worldMap = storageWorld.WorldMap;
            var worldList = DecorationConfigManager.Instance.WorldConfigs;
            foreach (var worldConfig in worldList)
            {
                var worldId = worldConfig.id;
                var worldStorage = Decoration.AssetCheckManager.Instance.GetStorageWorld(worldMap, worldId);
                var world = new DecoWorld(worldConfig, worldStorage);
            }
        }
        public void EnterWorld(int worldId, Action<float> updateProcess, EnterCall enterCall = null)
        {
            DragonU3DSDK.DebugUtil.Log($"EnterWolrd-->worldId={worldId},_currentWorld={_currentWorld}");
            IsWorldReady = false;
            if (_poolManager == null)
            {
                _poolManager = new GameObjectPoolManager("DecoObjectPoolRoot");
            }

            DynamicMapConfigManager.Instance.InitConfig(worldId);
            
            CoroutineManager.Instance.StartCoroutine(Co_LoadWorldGraphic(worldId, updateProcess, enterCall));
        }
        public void ShowWorld(int worldId, bool fromLogin, bool focusToSuggestNode)
        {
            _currentWorld = null;

            foreach (var kv in DecoWorld.WorldLib)
            {
                if (kv.Key == worldId)
                {
                    _currentWorld = kv.Value;
                    _currentWorld.Show(fromLogin, focusToSuggestNode);
                    break;
                }
            }
            InteractLogicManager.Instance.LoadElement(worldId);
            PathMoveElementManager.Instance.LoadElement(worldId);
        }
        public IEnumerator Co_LoadWorldGraphic(int worldId, Action<float> updateProcess, EnterCall enterCall = null)
        {
            var useOneAtlas = worldId != 1; //如果当前用户未开放world2，动态图集用两张，如果已经开放world2，动态图集用一张 

            foreach (var kv in DecoWorld.WorldLib)
            {
                var world = kv.Value;
                if (world.GameObject != null) 
                    continue;

                if(worldId != world.Id)
                    continue;

                //加载World任务
                world.LoadGraphic(DecoSceneRoot.Instance.mRoot);
                //world.LoadGraphic(GameObject.Find("Main/DecoSceneRoot").gameObject);
                // RuntimeAtlasManager.Instance.Replace(useOneAtlas, world.Id, world?.GameObject, false);

                //加载区域Area
                foreach (var areaItem in world.AreaDict)
                {
                    var areaId = areaItem.Key;
                    var area = areaItem.Value;

                    var areaTransform =
                        area.World.GameObject.transform.Find("MapLayer/Viewport/Content/Areas/" + area.Id);
                    if (areaTransform == null)
                    {
                        continue;
                    }
                    area.LoadGraphic(areaTransform.gameObject);

                    // RuntimeAtlasManager.Instance.Replace(useOneAtlas, world.Id, areaTransform.gameObject, false);

                    foreach (var stage in area._stageDict.Values)
                    {
                        foreach (var node in stage.NodeDic.Values)
                        {
                            if (node._currentItem != null)
                            {
                                //node._currentItem.LoadGraphic(node.GameObject);
                                node.ChangeItemSilence(node._currentItem.Id);
                            }
                        }
                    }
                }

                if (world.Id != worldId)
                {
                    world.Hide();
                }

                // if (!useOneAtlas)
                // {
                //     RuntimeAtlasManager.Instance.GenerateLast(useOneAtlas, world.Id);
                //     RuntimeAtlasManager.Instance.ClearTemp(useOneAtlas, world.Id);
                // }
            }

            // if (useOneAtlas)
            // {
            //     RuntimeAtlasManager.Instance.GenerateLast(useOneAtlas, -1);
            //     RuntimeAtlasManager.Instance.ClearTemp(useOneAtlas, -1);
            // }
            DebugUtil.Log($"start init load world IsWorldReady");
            updateProcess?.Invoke(1f);
            yield return new WaitForSeconds(0.4f);

            IsWorldReady = true;
            if (enterCall != null)
                enterCall(true);
            //onProgressUpdate?.Invoke(1f);
        }

        public DecoWorld GetDecoWorld(int id)
        {
            if (!DecoWorld.WorldLib.ContainsKey(id))
                return null;

            return DecoWorld.WorldLib[id];
        }
        
        public void CleanCurrentWorld()
        {
            _currentWorld?.UnloadGraphic();
            _currentWorld = null;

            EnableUpdate = false;
            _isPauseMove = false;
            _worldItemRootDic?.Clear();
        }
        public void CleanAll()
        {
            CleanCurrentWorld();

            DecoWorld.WorldLib.Clear();
            DecoWorld.AreaLib.Clear();
            DecoWorld.StageLib.Clear();
            DecoWorld.NodeLib.Clear();
            DecoWorld.ItemLib.Clear();
            DecoWorld.SuggestNode.Clear();
            DecoWorld.ActivityItemLib.Clear();
        }

        private void OnSelectNode(BaseEvent e)
        {
            EndPreviewNode();
        }
        private void OnUnSelectNode(BaseEvent e)
        {
            try
            {
                OnNodeEndPreView();
                
                EndPreviewNode(false);

                var selectedNode = CurrentWorld.SelectedNode;
                if (selectedNode != null)
                {
                    DecoWorld.NodeLib.TryGetValue(selectedNode.Id, out var node);
                    if (node != null)
                    {
                    }
                    selectedNode = null;
                }

            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
            }
        }
        private void EndPreviewNode(bool playAnim = true)
        {
            if (_previewNode == null)
                return;
            
            _previewNode.EndPreview(playAnim);
            _previewNode = null;

            OnNodeEndPreView();
        }
        
        private void OnPreviewItem(BaseEvent baseEvent)
        {
            OnNodeEndPreView();
            
            if (baseEvent == null || baseEvent.datas == null || baseEvent.datas.Length < 3)
                return;
            var nodeId = (int)baseEvent.datas[0];
            var itemId = (int)baseEvent.datas[1];
            var fromFirstBuy = (bool)baseEvent.datas[2];
            
            if (!DecoWorld.ItemLib.ContainsKey(itemId))
                return;
            
            EndPreviewNode(false);

            if (DecoWorld.NodeLib.TryGetValue(nodeId, out var node))
            {
                node.PreviewItem(itemId);
                _previewNode = node;
            }
        }

        public void OnNodePreView(List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                if(!DecoWorld.ItemLib.ContainsKey(itemId))
                    continue;

                var node = DecoWorld.ItemLib[itemId].Node;
                _previewNodes.Add(node);

                node.PreviewItem(itemId, false);
                node.SetNodeDependsActive(true);
            }
        }

        public void OnNodeEndPreView()
        {
            if(_previewNodes.Count == 0)
                return;

            foreach (var previewNode in _previewNodes)
            {
                previewNode.EndPreview(false);
                previewNode.RestoreNodeNeeded();
                
                //激活依赖此挂点显示控制的其他挂点
                previewNode.ViewDependenceTest();
            }
            
            _previewNodes.Clear();
        }

        public void ChangeItemAsync(DecoNode node, DecoItem item, bool firstUnlock, Action onFinished = null)
        {
            CoroutineManager.Instance.StartCoroutine(ChangeItem(node, item, firstUnlock, onFinished));
        }

        private IEnumerator ChangeItem(DecoNode node, DecoItem item, bool firstUnlock, Action onFinished = null)
        {
            var itemId = item == null ? -1 : item.Id;
            if (firstUnlock)
            {
                yield return CoroutineManager.Instance.StartCoroutine( node.BuyFirstItem(itemId, onFinished));
            }
            else
            {
                yield return CoroutineManager.Instance.StartCoroutine(node.ReplaceItem(itemId,true, false, onFinished));
            }
        }

        public void BuildingPlayAnimation(int buildingPointId, int buildingId, string animationName)
        {
            if (DecoWorld.ItemLib.TryGetValue(buildingId, out var building))
            {
                building.PlayAnimation(animationName);
            }
            else
            {
                DebugUtil.LogError("BuildingPlayAnimation not found building: " + buildingId);
            }
        }

        public bool TriggerAreaUnlock(int curAreaId, int nextAreaId, Action onFinish)
        {
            if (_currentWorld == null)
                return false;
            
            if (!_currentWorld.AreaDict.ContainsKey(nextAreaId))
                return false;

            if (nextAreaId == 110)
            {
                if (ABTest.ABTestManager.Instance.IsLockMap())
                    return false;
            }
            
            isUnLocking = true;
            //触发区域解锁
            CurrentWorld.PlayAreaFinishCameraAnimation(nextAreaId, () =>
            {
                isUnLocking = false;
                onFinish?.Invoke();
            });
            
            return true;
        }

        public bool TriggerAreaUnlock(int storyId)
        {
            if (_currentWorld != null)
                return false;
            
            foreach (var area in _currentWorld.AreaDict.Values)
            {
                if (storyId != area.Config.storyId)
                    continue;
                
                //触发区域解锁
                var nextAreaId = area.Config.nextAreaId;
                var currentAreaId = area.Id;
                UIRoot.Instance.AddTouchBlock();
                CurrentWorld.PlayAreaFinishCameraAnimation(currentAreaId, () =>
                {
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        HasCancelButton = false,
                        HasCloseButton = false,
                        DescString = "Unlock View",
                        OKCallback = () =>
                        {
                            DecoManager.Instance.CurrentWorld.PlayAreaUnlockCameraAnimation(nextAreaId, () =>
                            {
                            });
                        }
                    });
                });
                return true;
            }

            return false;
        }
        
        public List<DecoNode> GetAreaNodes(DecoArea area)
        {
            if (area == null)
                return null;
            
            var nodes = new List<DecoNode>();
            foreach (var stage in area._stageDict)
            {
                nodes.AddRange(stage.Value._nodeDict.Values.ToList());
            }
            
            return nodes;
        }
        
        public float GetAreaDecorationRate(int areaId, ref int decNum)
        {
            if (_currentWorld == null) 
                return 0;
            
            Deco.Area.DecoArea decoArea = null;

            _currentWorld.AreaDict.TryGetValue(areaId, out decoArea);
            if (!IsAreaUnLock(decoArea))
                return 0;

            var storageArea = decoArea.Storage;

            int totalCount = 0;
            decNum = 0;
            foreach (var sStage in storageArea.StagesData)
            {
                totalCount += sStage.Value.NodesData.Count;
                foreach (var sNode in sStage.Value.NodesData)
                {
                    TableNodes nodes = DecorationConfigManager.Instance.GetNodeConfig(sNode.Value.Id);
                    if(nodes != null)
                    {
                        if (nodes.costId != (int)UserData.ResourceId.Seal &&
                            nodes.costId != (int)UserData.ResourceId.RareDecoCoin &&
                            nodes.costId != (int)UserData.ResourceId.Dolphin &&
                            nodes.costId != (int)UserData.ResourceId.Capybara)
                        {
                            if (sNode.Value.Status >= (int)DecoNode.Status.Owned)
                                decNum++;
                        }
                        else
                        {
                            totalCount--;
                        }
                    }
                }
            }

            return 1.0f * decNum / totalCount;
        }
        
        public bool IsAreaUnLock(int areaId)
        {
            var storageWorlds = Decoration.AssetCheckManager.Instance.GetStorage();

            var worldMaps = storageWorlds.WorldMap;
            var worldList = DecorationConfigManager.Instance.WorldConfigs;
            foreach (var worldConfig in worldList)
            {
                var worldId = worldConfig.id;
                var worldStorage = Decoration.AssetCheckManager.Instance.GetStorageWorld(worldMaps, worldId);
                if (worldStorage.AreasData.ContainsKey(areaId))
                {
                    var storageArea = worldStorage.AreasData[areaId];
                    return storageArea.State != (int)DecoArea.Status.Lock;
                }
            }

            return false;
        }

        public bool IsAreaUnLock(DecoArea area)
        {
            if (area == null) 
                return false;
            return area.Storage.State != (int)DecoArea.Status.Lock;
        }

        public int GetNodeAreaID(DecoNode node)
        {
            var areaId = Decoration.DecorationConfigManager.Instance.GetNodeBelongAreaID(node.Id);
            return areaId;
        }
        public DecoArea GetNodeArea(DecoNode node)
        {
            var areaId = Decoration.DecorationConfigManager.Instance.GetNodeBelongAreaID(node.Id);
            if (areaId < 0) return null;
            Deco.Area.DecoArea decoArea = null;
            if (_currentWorld.AreaDict.ContainsKey(areaId))
            {
                decoArea = _currentWorld.AreaDict[areaId];
            }
            return decoArea;
        }

        public bool IsAreaUnLock(DecoNode node)
        {
            if (node == null || node._stage == null || node._stage.Area == null)
                return true;

            return node._stage.Area.IsUnlock;
        }

        public bool IsFirstNode(DecoNode node)
        {
            if (node == null || node._data == null)
                return false;

            if (node._stage == null || node._stage._data == null)
                return false;
                
            return node._stage._data.IsFirstNode(node.Id);
        }

        public DecoNode GetFirstNode(int areaId)
        {
            return GetFirstNode(FindArea(areaId));
        }
        
        public DecoNode GetFirstNode(DecoArea area)
        {
            int id = GetFirstNodeId(area);
            if (id < 0)
                return null;

            DecoWorld.NodeLib.TryGetValue(id, out var node);
            
            return node;
        }

        private int GetFirstNodeId(DecoArea area)
        {  
            if (area == null)
                return -1;

            if (area.Config == null)
                return -1;

            if (area.Config.stages == null || area.Config.stages.Length == 0)
                return -1;

            var stage = DecorationConfigManager.Instance.GetStageConfig(area.Config.stages[0]);
            if (stage == null)
                return -1;

            if (stage.nodes == null || stage.nodes.Length == 0)
                return -1;
            
            return stage.nodes[0];
            
        }
        public void UnlockDecoBuilding(int buildingId,bool unlockNode = true)
        {
            if (DecoWorld.ItemLib.ContainsKey(buildingId))
            {
                var decoItem = DecoWorld.ItemLib[buildingId];
                if (unlockNode || decoItem._node._stage.Area.Config.hideAreaInDeco)
                    decoItem.Node.Buy();
                decoItem.Buy();
            }
        }
        public void LockDecoBuilding(int buildingId)
        {
            if (DecoWorld.ItemLib.ContainsKey(buildingId))
            {
                var decoItem = DecoWorld.ItemLib[buildingId];
                decoItem.Lock();
            }
        }

        public bool IsOwnedDecoBuilding(int itemId)
        {
            if (!DecoWorld.ItemLib.ContainsKey(itemId))
                return false;
            
            var decoItem = DecoWorld.ItemLib[itemId];
            return decoItem.IsOwned;
        }

        public bool IsOwnedNode(int nodeId)
        {
            DecoNode decoNode = FindNode(nodeId);
            if(decoNode == null)
                return false;

            return decoNode.IsOwned;
        }

        public void SelectNode(DecoNode node)
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TouchBubble);
                
            PlayerManager.Instance.UpdatePlayersState(node, 0.1f);
            
            CurrentWorld.SelectedNode = node;
            CurrentWorld.LookAtSuggestNodeBySpeed(CurrentWorld.SelectedNode, true, CurrentWorld.PinchMap.CurrentCameraScale, 10,
                () =>
                {
                    if (node.Id == UIPopupTaskController.DecoNodeId)
                    {
                        if (node.IsOwned)
                        {
                            WishingInstall(node);
                            return;
                        }
                    }
                    
                    if (FarmModel.Instance.IsFarmModel())
                    {
                        EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_SHOW_NODE_BUY, node);
                    }
                    else
                    {
                        if (node.IsOwned)
                        {
                            if (node.Config.costId == (int)UserData.ResourceId.Mermaid || node.Config.costId == (int)UserData.ResourceId.HappyGo)
                            {
                                EventDispatcher.Instance.DispatchEvent(EventEnum.SHOW_NODE_BUY);
                                return;
                            }
                            bool firstBuy = false;
                            if (!node.IsFinish)
                                firstBuy = true;
                
                            node.Stage.Area.World.Select(node, fromFirstBuy: firstBuy);
                        }
                        else
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.SHOW_NODE_BUY);
                        }
                    }
                });
            
        }

        private void WishingInstall(DecoNode node)
        {
            DecoChoseNodeView.AddDecorationReward(node.Config);
            
            List<ResData> resDatas = new List<ResData>();
            for (int i = 0; i < node.Config.rewardId.Length; i++)
            {
                resDatas.Add(new ResData(node.Config.rewardId[i], node.Config.rewardNumber[i]));
            }
            
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, false);
            
            node.PlaySpineAnimation("Normal2", () =>
            {
                CommonRewardManager.Instance.PopCommonReward(resDatas,
                    CurrencyGroupManager.Instance.currencyController, false,
                    new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.BindFacebook),
                    () =>
                    {
                        AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, true);
                        AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, true);
                    });
            });
        }
        
        public void InstallItem(List<int> itemIds, Action action = null)
        {
            if (itemIds == null || itemIds.Count == 0)
            {
                action?.Invoke();
                return;
            }

            DecoNode node = null;
            foreach (var itemId in itemIds)
            {
                if(!DecoWorld.ItemLib.ContainsKey(itemId))
                    continue;

                if(node == null)
                    node = DecoWorld.ItemLib[itemId].Node;
                
                if(DecoExtendAreaManager.Instance.ReceiveExtendArea(DecoWorld.ItemLib[itemId].Node))
                    DynamicMapManager.Instance.ForceLoadCurrentChunk();
            }
            
            if(node == null)
                return;

            UIRoot.Instance.EnableEventSystem = false;
            CurrentWorld.LookAtSuggestNodeBySpeed(node, true, CurrentWorld.PinchMap.CurrentCameraScale, 10,
                () =>
                {
                    CoroutineManager.Instance.StartCoroutine(InstallItemAsync(itemIds, action));
                });
        }

        private IEnumerator InstallAsync()
        {
            yield return new WaitForSeconds(0.1f);
            
        }
        public void ApplyItem(List<int> itemIds)
        {
            if(itemIds == null || itemIds.Count == 0)
                return;
            
            for (int i = 0; i < itemIds.Count; i++)
            {
                if(!DecoWorld.ItemLib.ContainsKey(itemIds[i]))
                    continue;

                var decoItem = DecoWorld.ItemLib[itemIds[i]];
                if(!decoItem.Node.IsOwned)
                    continue;
                
                CoroutineManager.Instance.StartCoroutine(decoItem.Node.ReplaceItem(itemIds[i],false, false));
            }
        }
        
        private IEnumerator InstallItemAsync(List<int> itemIds, Action action = null)
        {
            AudioManager.Instance.PlaySound(200);
        
            DynamicMapManager.Instance.ForceLoadCurrentChunk();
            yield return new WaitForSeconds(0.1f);
            
            UIHomeMainController.HideUI(true);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
        
            UIRoot.Instance.EnableEventSystem = false;
            AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, false);

            int itemCount = 0;

            DecoNode node = null;
            for (int i = 0; i < itemIds.Count; i++)
            {
                if(!DecoWorld.ItemLib.ContainsKey(itemIds[i]))
                    continue;

                var decoItem = DecoWorld.ItemLib[itemIds[i]];

                if (decoItem.Node.Config.nodeDepends > 0)
                {
                    if(!decoItem.Node.IsOwned)
                        continue;
                }

                bool waitStory = false;
                if (StorySubSystem.Instance.Trigger(StoryTrigger.GetDecoItem, itemIds[i].ToString(),
                        isFinish => { waitStory = false;
                            UIRoot.Instance.EnableEventSystem = false;
                        }))
                {
                    
                    UIRoot.Instance.EnableEventSystem = true;   
                    waitStory = true;
                }
                

                while (waitStory)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (decoItem.Node.Config.nodeDepends > 0)
                {
                    var nodeConfig = DecorationConfigManager.Instance.nodeConfigs.Find(a => a.nodeDepends == decoItem.Node.Id);
                    if (nodeConfig != null)
                    {
                        DecoNode decoNode = DecoManager.Instance.FindNode(nodeConfig.id);
                        if(decoNode.IsOwned)
                            continue;
                    }
                }

                node = decoItem.Node;
                
                // if (node != null && node.Stage != null && node.Stage.Area._data._config.showArea > 0)
                // { 
                //     PlayerManager.Instance.PlayAnimation(node, true);
                // }
                
                itemCount++;
                Decoration.DecoManager.Instance.ChangeItemAsync(decoItem.Node, decoItem, false, () =>
                {
                    if (itemIds.Count == 1)
                    {
                        if (StorySubSystem.Instance.Trigger(StoryTrigger.DecoItemSuccess, decoItem.Id.ToString(),
                                isFinish => { itemCount--; }))
                        {
                            UIRoot.Instance.EnableEventSystem = true;   
                        }
                        else
                        {
                            itemCount--;
                        }
                            
                    }
                    else
                    {
                        itemCount--;
                    }
                });
            }

            while (itemCount > 0)
            {
                yield return new WaitForEndOfFrame();
            }
            
            UIRoot.Instance.EnableEventSystem = true;   
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY);
            
            // if (node != null && node.Stage != null && node.Stage.Area._data._config.showArea > 0)
            // {
            //     PlayerManager.Instance.PlayAnimation(node);
            // }
            action?.Invoke();
        }

        public bool CanBuyOrGet()
        {
            if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.DecoAndMerge)
            {
                return CanBuyOrGet_Deco();
            }
            else
            {
                if (CanBuyOrGet_Mini())
                    return true;

                if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.MiniGame_Deo))
                    return false;
                
                return CanBuyOrGet_Deco();
            }
        }
        
        public bool CanBuyOrGet_Mini()
        {
            var isFinishAllLevel = MiniGameModel.Instance.IsMiniGameAllFinish();
            if(isFinishAllLevel)
                return false;

            int chapterId = MiniGameModel.Instance.GetMinUnFinishedChapterId();
            int levelId = MiniGameModel.Instance.GetMinUnfinishLevelInChapter(chapterId);

            var config = MiniGameModel.Instance.GetLevelConfig(levelId);
            if (config == null)
                return false;

            return UserData.Instance.CanAford(config.CostId, config.CostCount);
        }
        
        public bool CanBuyOrGet_Deco()
        {
            if (Instance.CurrentWorld == null)
                return false;
            
            if (DecoWorld.SuggestNode != null && DecoWorld.SuggestNode.Count > 0)
            { 
                foreach (var node in DecoWorld.SuggestNode)
                {
                    if (!UserData.Instance.IsResource(node._data._config.costId))
                        continue;
                
                    if(node._data._config.costId==(int)UserData.ResourceId.Mermaid || node._data._config.costId==(int)UserData.ResourceId.HappyGo)
                        continue;
                    int resValue = UserData.Instance.GetRes((UserData.ResourceId)node._data._config.costId);
                    if(resValue < node._data._config.price)
                        continue;

                    return true;
                }
            }

            Dictionary<int, List<DecoNode>> decoNodes = DecoManager.Instance.CurrentWorld.GetUnlockAndNotOwnedNodes();
            if (decoNodes == null)
                return false;
            
            foreach (var nodes in decoNodes)
            {
                DecoArea area = DecoManager.Instance.FindArea(nodes.Key);
                if(area == null)
                    continue;
                
                int decNum = 0;
                float rote = DecoManager.Instance.GetAreaDecorationRate(nodes.Key, ref decNum);
                if (rote >= 1)
                    continue;
                
                foreach (var node in nodes.Value)
                {
                    if (!UserData.Instance.IsResource(node._data._config.costId))
                        continue;
                
                    int resValue = UserData.Instance.GetRes((UserData.ResourceId)node._data._config.costId);
                    if(resValue < node._data._config.price)
                        continue;
                    
                    return true;
                }
            }
            
            return false;
        }

        public int GetOwnedNodeNum()
        {
            int num = 0;
            foreach (var area in _currentWorld._areaList)
            {
                if (!area.Unlocked)
                    continue;

                var storageArea = area.Storage;
                foreach (var stage in storageArea.StagesData)
                {
                    foreach (var node in stage.Value.NodesData)
                    {
                        var config = DecorationConfigManager.Instance.GetNodeConfig(node.Value.Id);
                        if (config != null && config.costId == (int)UserData.ResourceId.Coin)
                        {
                            if (node.Value.Status >= (int)DecoNode.Status.Owned)
                                num++;
                        }
                    }
                }
            }

            return num;
        }

        public DecoArea GetLockArea()
        {
            if (_currentWorld == null)
                return null;
            
            if (DecoWorld.AreaLib == null || DecoWorld.AreaLib.Count == 0)
                return null;

            foreach (var table in DecorationConfigManager.Instance.AreaConfigList)
            {
               if(!DecoWorld.AreaLib.TryGetValue(table.id, out var area))
                   continue;

               if(area._world.Id != _currentWorld.Id)
                   continue;
               
               if (table.id == 110)
               {
                   if(ABTest.ABTestManager.Instance.IsLockMap())
                       continue;
               }
               
               if (!area.IsUnlock)
                   return area;
            }

            return null;
        }

        public static int LastNodeId
        {
            get
            {
                if (ABTestManager.Instance.IsLockMap())
                    return 109035;
                // Debug.LogError("LastNodeId=" + DecorationConfigManager.Instance.LastNodeId);
                return DecorationConfigManager.Instance.LastNodeId;
            }
        }
        public bool IsFinishLastNode()
        {
            var storageWorld = AssetCheckManager.Instance.GetStorage();

            foreach (var world in storageWorld.WorldMap)
            {
                foreach (var area in world.Value.AreasData)
                {
                    foreach (var stage in area.Value.StagesData)
                    {
                        foreach (var node in stage.Value.NodesData)
                        {
                            if(node.Key != LastNodeId)
                                continue;

                            return node.Value.Status >= (int)DecoNode.Status.Owned;
                        }
                    }
                }
            }

            return false;
        }
    }
}