using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using DragonU3DSDK.Asset;
using Framework;
using Deco.Area;
using Deco.Graphic;
using Deco.Item;
using Deco.Node;
using DG.Tweening;
using SomeWhere;
using DragonU3DSDK;
using DragonU3DSDK.Config;
using UnityEngine.PlayerLoop;
using Framework;
//using Gameplay.SubSystems;
using Decoration;
using Decoration.Bubble;
using Decoration.DaysManager;
using Decoration.DynamicMap;
using Decoration.WorldFogManager;
using Farm.Model;
using Gameplay;
using Gameplay.UI.Capybara;

namespace Deco.World
{
    using StorageDecoration = DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld = DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea = DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem = DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode = DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage = DragonU3DSDK.Storage.Decoration.StorageStage;
    public enum WorldViewMode
    {
        Normal,
        Preview,
    }

    public partial class DecoWorld : DecoGraphicHost<DecoWorldGraphic>
    {
        public enum Status
        {
            Lock = 0, //未解锁
            Unlock = 1, //已解锁
        }

        public enum TouchStatus
        {
            None,
            Begin,
            ArrowTip,
            LongPress,
        }

        internal DecoWorldData _data;
        private WorldViewMode _worldViewMode;
        //private DecorationMainController _worldMainUI;

        private TouchStatus _touchStatus = TouchStatus.None;
        private float ARROW_TIP_SHOW_DELTA = 0.3f;
        private float LONG_PRESS_DELTA = 0.5f;
        private Vector2 _touchStartPosition;
        private float _touchStartTime;


        private bool _isTouchUi = false;
        
        private Node.DecoNode _selectedNode;
        private Area.DecoArea _selectedArea;
        internal Dictionary<int, Area.DecoArea> _areaDict;
        internal List<DecoArea> _areaList;

        private float _defaultCameraScaleForNode;
        private bool _isPad;

        public bool IsLocked => _data._storage.State == (int)Status.Lock;

        public Node.DecoNode SelectedNode
        {
            get => _selectedNode;
            set => _selectedNode = value;
        }

        public DecoArea SelectedArea => _selectedArea;

        public PinchMapComponent PinchMap => Graphic._pinchMap;
        public Dictionary<int, Area.DecoArea> AreaDict => _areaDict;
        public GameObject GameObject => Graphic.gameObject;
        public PathMap PathMap => Graphic._pathMap;
        public bool IsPreviewMode => _worldViewMode == WorldViewMode.Preview;
        public int Id => _data._config.id;

        private const float _focusTime = 1.5f;
        
        private static Vector3 showPosition = Vector3.zero;
        private static Vector3 hidePosition = new Vector3(10000, 10000, 0);
        
        public DecoWorld(TableWorlds config, StorageWorld worldStorage)
        {
            _data = new DecoWorldData(this, config, worldStorage);
            Graphic = new DecoWorldGraphic(this);

            DecoWorld.WorldLib[config.id] = this;

            initAreas();

            _isPad = CommonUtils.IsLE_16_10();
            _defaultCameraScaleForNode = Decoration.DecorationConfigManager.Instance.GetGlobalConfigNumber(_isPad
                ? GlobalNumberConfigKey.deco_camera_focus_default_pad
                : GlobalNumberConfigKey.deco_camera_focus_default);
        }

        private void initAreas()
        {
            var configManager = DecorationConfigManager.Instance;
            var worldConfig = _data._config;
            _areaDict = new Dictionary<int, Area.DecoArea>();
            _areaList = new List<DecoArea>();
            DaysManager.Instance.CompleteFixData = false;

            foreach (var areaId in worldConfig.areaIds)
            {
                var areaConfig = configManager.GetAreaConfig(areaId);
                if (areaConfig == null)
                {
                    DebugUtil.LogError("areaConfig is null:" + areaId);
                    continue;
                }

                //var areaStorage = UserDataMoudule.GetStorageArea(_data._storage, areaId);

                //var area = new Area.DecoArea(areaConfig, areaStorage, this);
                var areaStorage = AssetCheckManager.Instance.GetStorageArea(_data._storage, areaId);
                var area = new Area.DecoArea(areaConfig, areaStorage, this);
                _areaDict[areaId] = area;
                _areaList.Add(area);
            }

            DaysManager.Instance.CompleteFixData = true;

            Action action = () =>
            {
                //检查更新新区域解锁
                for (int i = 0; i < worldConfig.areaIds.Length; i++)
                {
                    var areaId = worldConfig.areaIds[i];
                    var area = _areaDict[areaId];
                    
                    if (area._data._config.id == 110)
                    {
                        if(ABTest.ABTestManager.Instance.IsLockMap())
                            continue;
                    }
                    
                    foreach (var kv in area._stageDict)
                    {
                        kv.Value.TryFinish();
                    }
                    
                    area.TryFinish();
                    if (i > 0)
                    {
                        var lastAreaId = worldConfig.areaIds[i - 1];
                        var lastArea = _areaDict[lastAreaId];
                        if (lastArea._data.IsFinish && !area.Unlocked)
                        {
                            area.Unlock(true);
                            area._data.RemoveMask();
                            WorldFogManager.Instance.Hide(area.Id);
                            DynamicMapManager.Instance.ForceLoadCurrentChunk();
                            break;
                        }
                    }
                }
            };
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

        public override void UnloadGraphic()
        {
            foreach (var area in _areaList)
            {
                foreach (var stage in area._stageDict.Values)
                {
                    foreach (var node in stage._nodeDict.Values)
                    {
                        foreach (var item in node._itemDict.Values)
                        {
                            item.UnloadGraphic();
                        }

                        node.UnloadGraphic();
                    }

                    stage.UnloadGraphic();
                }

                area.UnloadGraphic();
            }

            Graphic.Unload();
        }

        public override void LoadGraphic(GameObject parentObj)
        {
            Graphic.Load(parentObj.transform);
        }

        public void Hide()
        {
            //MyMain.myGame.DecorationMgr.EnableUpdate = false;
            DecoManager.Instance.EnableUpdate = false;
            Graphic.Hide();

            //UIRoot.Instance.mWorldUIRoot.gameObject.SetActive(false);
        }

        public void HideByPosition()
        {
            Graphic.gameObject.transform.localPosition = hidePosition;
            PathMoveElementManager.Instance.SetActive(false);
            DynamicMapManager.Instance.PauseLogic = true;
        }

        public void ShowByPosition()
        {
            Graphic.gameObject.transform.localPosition = showPosition;
            PathMoveElementManager.Instance.SetActive(true);
            DynamicMapManager.Instance.PauseLogic = false;
        }
        
        public void RefreshAreaStatus()
        {
            foreach (var area in _areaList)
            {
                area.Graphic.RefreshLoadingButton();
                area.RefreshMask(false);
            }
        }

        public void Show(bool fromLogin, bool focusToSuggestNode)
        {
            _worldViewMode = WorldViewMode.Normal;
            Graphic.Show(fromLogin);
            if (focusToSuggestNode)
            {
                LookAtSuggestNode(false);
            }
            else
            {
                Graphic._pinchMap.Reset();
            }

            DecoManager.Instance.EnableUpdate = true;
        }

        public void LookAtSuggestNode(bool animate, float focusTime = _focusTime)
        {
            Node.DecoNode targetNode = GetSuggestNode();

            if (animate)
            {
                if(targetNode == null)
                    return;
            }
            
            LookAtSuggestNode(targetNode, animate, focusTime);
        }


        public void LookAtSuggestNodeBySpeed(bool animate, int speed = 10, Action onFinished = null)
        {
            Node.DecoNode targetNode = GetSuggestNode();

            if(targetNode == null)
                return;
            
            
            LookAtSuggestNodeBySpeed(targetNode, animate, PinchMap.CurrentCameraScale, speed, onFinished);
        }
        

        public void LookAtSuggestNodeCurrentCameraSize(bool animate, float focusTime = _focusTime, DecoNode node = null)
        {
            Node.DecoNode targetNode = GetSuggestNode();

            if (animate)
            {
                if(targetNode == null)
                    return;
            }

            if (node != null && targetNode != null)
            {
                if(node._data._config.costId == 103 && node._stage.Area.Id != targetNode._stage.Area.Id)
                    return;
            }
            
            LookAtSuggestNode(targetNode, animate, Graphic._pinchMap.CurrentCameraScale, focusTime);
        }
        

        public void LookAtSuggestNode(DecoNode targetNode, bool animate, float focusTime = _focusTime)
        {
            if (targetNode != null)
            {
                lookAtNode(targetNode, animate, focusTime);
            }
            else
            {
                areaFocusWithoutSuggestNode(Graphic._pinchMap.FinishPosition.position, CAMERA_ACTIVITY_PREVIEW);
            }
        }
        
        public void LookAtSuggestNode(DecoNode targetNode, bool animate, float scale, float focusTime = _focusTime, Action onFinished = null)
        {
            if (targetNode == null)
            {
                onFinished?.Invoke();
                return;
            }
            
             lookAtNode(targetNode, animate, scale, focusTime, onFinished);
        }
        
        public void LookAtSuggestNodeBySpeed(DecoNode targetNode, bool animate, float scale, int speed = 10, Action onFinished = null)
        {
            if (targetNode == null)
            {
                onFinished?.Invoke();
                return;
            }
            
            lookAtNodeBySpeed(targetNode, animate, scale, speed, onFinished);
        }
            
        //获取推荐挂载点
        public Node.DecoNode GetSuggestNode()
        {
            Node.DecoNode targetNode = null;
            Node.DecoNode specialNode = null;
            foreach (var node in NodeLib.Values)
            {
                if (node.Stage.Area.World.Id != Id) continue;

                //特殊点推荐摄像机
                if (node.Config.costId == (int)UserData.ResourceId.RareDecoCoin)
                {
                    if (node.SuggestTest())
                        specialNode = node;
                    continue;
                }

                if (node.Config.costId == (int)UserData.ResourceId.Coin && node.SuggestTest())
                {
                    if (FarmModel.Instance.IsFarmModel())
                    {
                        if(FarmModel.Instance.CanUnLockNode(node))
                            targetNode = node;
                    }
                    else
                    {
                        if (targetNode == null || targetNode.Config.suggestLevel > node.Config.suggestLevel)
                        {
                            targetNode = node;
                        }
                    }
                }
            }

            if (FarmModel.Instance.IsFarmModel())
            {
                if (targetNode == null)
                    targetNode = DecoManager.Instance.FindNode(201001);
            }
            
            return targetNode == null ? specialNode : targetNode;
        }
        public void UpdateSuggestNode()
        {
            SuggestNode.Clear();
            
            foreach (var area in _areaList)
            {
                area.UpdateSuggestNode();
            }
        }

        private void initWorldMainUI()
        {
            //_worldMainUI = UIManager.Instance.GetOpenedWindow<DecorationMainController>();
        }

        private void updateTouch()
        {
            if(!UIRoot.Instance.EnableEventSystem)
                return;
            
            if (MainDecorationController.mainController == null)
                return;
            
            if(MainDecorationController.mainController.Status == DecoUIStatus.Decoration)
                return;
            
            if (CommonUtils.IsTouchUGUI()) 
                return;
            
            if (DecoManager.Instance.IsPauseMove)
                return;
            
            switch (_touchStatus)
            {
                case TouchStatus.None:
                {
                    if (FarmModel.Instance.IsFarmModel())
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            _isTouchUi = EventSystem.current.currentSelectedGameObject != null;
                            _touchStartPosition = Input.mousePosition;
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            if (_isTouchUi || !TouchValidTest())
                            {
                                _isTouchUi = false;
                                return;
                            }

                            Node.DecoNode tempSelectedNode = null;
                            foreach (var area in _areaList)
                            {
                                if (area.TouchTest(Input.mousePosition))
                                {
                                    tempSelectedNode = area.NodeTouchTest(Input.mousePosition);
                                    if (tempSelectedNode != null) 
                                        break;
                                }
                            }
                            
                            if(tempSelectedNode != null)
                                EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_TOUCH_NODE, tempSelectedNode);
                            else
                                EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_CANCEL_TOUCH_NODE, tempSelectedNode);

                        }
                        break;
                    }
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        _touchStatus = TouchStatus.Begin;
                        _touchStartPosition = Input.mousePosition;
                        _touchStartTime = Time.time;

                        Node.DecoNode tempSelectedNode = null;
                        _selectedArea = null;
                        foreach (var area in _areaList)
                        {
                            if (area.TouchTest(_touchStartPosition))
                            {
                                _selectedArea = area;

                                tempSelectedNode = area.NodeTouchTest(_touchStartPosition);
                                if (tempSelectedNode != null) break;
                            }
                        }
                        
                        if (tempSelectedNode != null && !tempSelectedNode.IsOwned) tempSelectedNode = null;

                        _selectedNode = tempSelectedNode;
                        

                        
                        if (_selectedNode != null)
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.HIDE_NODE_BUY);
                            //再次点击未点到任何建筑，取消之前的选择
                            if (tempSelectedNode == null)
                            {
                                EventDispatcher.Instance.DispatchEvent(EventEnum.UNSELECT_NODE);
                                _selectedNode = null;
                            }
                            else if (_selectedNode != tempSelectedNode) //点中不同的建筑
                            {
                                EventDispatcher.Instance.DispatchEvent(EventEnum.UNSELECT_NODE);
                            }
                        }
                    }
                    break;
                }
                case TouchStatus.Begin:
                    {
                        if (!TouchValidTest())
                        {
                            _touchStatus = TouchStatus.None;
                            break;
                        }

                        // if (_selectedNode != null && _selectedNode.Config.costId == (int)UserData.ResourceId.HappyGo)
                        // {
                        //     _touchStatus = TouchStatus.None;
                        //     break;
                        // }
                        
                        if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.EnterFarm)
                        {
                            //触发箭头提示,切换状态
                            var touchDeltaTime = Time.time - _touchStartTime;
                            if (touchDeltaTime >= ARROW_TIP_SHOW_DELTA && touchDeltaTime < LONG_PRESS_DELTA)
                            {
                                _touchStatus = TouchStatus.ArrowTip;
                                if (_selectedNode != null)
                                {
                                    EventDispatcher.Instance.DispatchEvent(EventEnum.SHOW_OR_HIDE_LONG_PRESS_ARROW, true, _touchStartPosition);
                                }

                                break;
                            }
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            if (_touchStatus != TouchStatus.LongPress)
                            {
                                _touchStatus = TouchStatus.None;
                                _selectedNode?.OnTap();
                            }
                        }
                    }
                    break;
                case TouchStatus.ArrowTip:
                    {
                        if (!TouchValidTest())
                        {
                            _touchStatus = TouchStatus.None;
                            break;
                        }
                        
                        //触发长按,切换状态
                        var touchDeltaTime = Time.time - _touchStartTime;
                        if (touchDeltaTime >= LONG_PRESS_DELTA)
                        {
                            _touchStatus = TouchStatus.LongPress;
                            break;
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            _touchStatus = TouchStatus.None;
                            //EventDispatcher.Instance.DispatchEvent(new LongPressArrowEvent(false, _touchStartPosition));
                            _selectedNode?.OnTap();
                        }
                    }
                    break;
                case TouchStatus.LongPress:
                    {
                        _touchStatus = TouchStatus.None;
                        Select(_selectedNode);
                    }
                    break;
            }
        }

        public void Select(Node.DecoNode node, int itemId = 0, bool fromFirstBuy = false)
        {
            _selectedNode = node;
            if (_selectedNode != null) 
                _selectedArea = _selectedNode.Stage.Area;

            if (_selectedArea == null)
            {
                //DebugUtil.LogError("未点中任何区域");
                CommonUtils.ShowTipAtTouchPosition("UI_explain1");
                return;
            }

            if (!_selectedArea.Unlocked)
            {
                //DebugUtil.LogError("区域未解锁");
                CommonUtils.ShowTipAtTouchPosition("UI_explain2");
                return;
            }
            
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SELECT_NODE, itemId, fromFirstBuy);
        }

        public void Update(float deltaTime)
        {
            if (GameObject == null || !GameObject.activeSelf) return;

            if (_worldViewMode == WorldViewMode.Normal)
            {
                updateTouch();

                foreach (var area in _areaList)
                {
                    area.Update(deltaTime);
                }
            }
        }

        private bool TouchValidTest()
        {
            return Vector2.Distance(_touchStartPosition, Input.mousePosition) < 20.0f;
        }

        public Vector3 FindAreaBaseAnchor(int areaId)
        {
            return Graphic.FindAreaBaseAnchor(areaId);
        }

        public bool TryWorldFinish()
        {
            foreach (var area in _areaList)
            {
                if (area.Id == 110)
                {
                    if(ABTest.ABTestManager.Instance.IsLockMap())
                        continue;
                }
                
                if (!area._data.IsFinish)
                    return false;
            }

            Graphic.ShowComingSoon();

            return true;
        }

        public bool IsFinish()
        {
            foreach (var area in _areaList)
            {
                if (area.Id == 110)
                {
                    if(ABTest.ABTestManager.Instance.IsLockMap())
                        continue;
                }
                
                if (area.Id < 888 && !area._data.IsFinish)
                    return false;
            }

            if (WorldLib.TryGetValue(_data._config.id + 1, out var nextWorld))
            {
                nextWorld.TryUnlock();
            }

            return true;
        }

        public int GetCurrentStageId()
        {
            var currentStage = 0;
            foreach (var area in DecoManager.Instance.CurrentWorld._areaList)
            {
                if (area.Unlocked && !area._data.IsFinish)
                {
                    foreach (var stage in area._stageDict.Values)
                    {
                        if (!stage.Locked && !stage.IsFinish)
                        {
                            currentStage = stage._data._config.id;
                            break;
                        }
                    }
                }
            }

            if (currentStage == 0)
            {
                var lastAreaId = _data._config.areaIds.Last();

                if (_areaDict.TryGetValue(lastAreaId, out var area))
                {
                    var lastStageId = area._data._config.stages.Last();
                    currentStage = lastStageId;
                }
            }
            return currentStage;
        }
        public int GetCurrentAreaId()
        {
            var currentAreaID = 0;
            foreach (var area in DecoManager.Instance.CurrentWorld._areaList)
            {
                if (area.Unlocked && !area._data.IsFinish)
                {
                    currentAreaID = area.Id;
                    break;
                }
            }
            return currentAreaID;
        }
        public bool TryWorldIntroduceCameraAnimation()
        {
            //var storage = StorageManager.Instance.GetStorage<StorageHome>().DecoGuide.WorldIntroFinished;
            //storage.TryGetValue(_data._config.id, out var worldIntroFinish);
            bool worldIntroFinish = false;
            int configId = _data._config.id;
            if (!worldIntroFinish && configId == 1)
            {
                //storage[configId] = true;
                UIRoot.Instance.EnableTouch(false);
                UIRoot.Instance.AddTouchBlock();
                var firstAreaID = _data._config.id == 1 ? 101 : _data._config.areaIds[0];

                focusAreaOut(firstAreaID, CAMERA_SCALE_AREA_FAR, false, 0f,
                    () =>
                    {
                        focusAreaOut(firstAreaID, CAMERA_SCALE_AREA_NEAR, true, 1.8f,
                            () =>
                            {
                                UIRoot.Instance.EnableTouch(true);
                                UIRoot.Instance.RemoveTouchBlock();
                            });
                    });
                return true;
            }
            return false;
        }

        private void areaFocusWithoutSuggestNode(Vector3 position, float scale)
        {
            focus(position, scale, false, 0f, null);
        }

        public void PlayAreaFinishCameraAnimation(int areaId, Action onFinish)
        {
            focusAreaOut(areaId, CAMERA_SCALE_AREA_FAR, true, _focusTime+0.5f, onFinish);
        }

        public void FocusOnFestivalView(int areaId, Transform focustTransform = null)
        {
            focusActivityArea(areaId, true, null, focustTransform);
        }

        public void ExistFestivalViewMode(List<int> itemList)
        {
            if (itemList == null || itemList.Count == 0) return;
            _worldViewMode = WorldViewMode.Normal;

            foreach (var itemId in itemList)
            {
                var item = DecoWorld.ItemLib[itemId];
                item.Node.EndPreview();
            }
        }

        public void PlayAreaUnlockCameraAnimation(int areaId, Action onFinished)
        {
            //todo:Arthur 109先特殊处理下
            if (areaId == 109)
            {
                onFinished?.Invoke();
                return;
            }
            focusAreaIn(areaId, CAMERA_SCALE_AREA_NEAR, true, _focusTime, () =>
            {
                if (_areaDict.TryGetValue(areaId, out var area))
                {
                    area._data.RemoveMask();
                    area.RefreshMask(true);

                    DragonPlus.AudioManager.Instance.PlaySound("sfx_ui_area_unlock");
                }

                onFinished?.Invoke();
            });
        }

        /// <summary>
        /// 已完成108区域的玩家，首次进入游戏后，触发109区域解锁剧情
        /// </summary>
        public bool CheckUnlock109AreaStory()
        {
            var area = DecoManager.Instance.FindArea(108);
            if (area != null)
                return false;

            return false;
        }

        public void TryUnlock()
        {
            if (IsLocked)
            {
                _data._storage.State = (int)Status.Unlock;
            }

            //解锁本世界内第一个区域
            var areaId = _data._config.areaIds[0];
            var area = DecoWorld.AreaLib[areaId];
            if (!area.Unlocked)
            {
                area.Unlock(false);
            }
        }

        private  Dictionary<int, List<DecoNode>> unlockAndNotOwnedNodes = new Dictionary<int, List<DecoNode>>();
        public Dictionary<int, List<DecoNode>> GetUnlockAndNotOwnedNodes()
        {
            unlockAndNotOwnedNodes.Clear();
            
            foreach (var area in _areaList)
            {
                if(!area.Unlocked)
                    continue;
                
                if(area.Config.hideAreaInDeco)
                    continue;

                if (area.Id == 110)
                {
                    if(ABTest.ABTestManager.Instance.IsLockMap())
                        continue;
                }
                // if(area.Storage.IsGetReward)
                //     continue;

                unlockAndNotOwnedNodes.Add(area.Id, new List<DecoNode>());
                
                var storageArea = area.Storage;
                foreach (var stage in storageArea.StagesData)
                {
                    foreach (var node in stage.Value.NodesData)
                    {
                        if(node.Value.Status != (int)DecoNode.Status.Unlock && node.Value.Status != (int)DecoNode.Status.Owned)
                            continue;

                        if (NodeLib.TryGetValue(node.Value.Id, out var decoNode))
                        {
                            var _node = decoNode;
                            if (_node.Id == 101099 || _node.Id == 101098)
                            {
                                if (CapybaraManager.Instance.IsOpenCapybara())
                                {
                                    if(_node.Id == 101099)
                                        continue;
                                }
                                else
                                {
                                    if(_node.Id == 101098)
                                        continue;
                                }
                            }
                            unlockAndNotOwnedNodes[area.Id].Add(decoNode);
                        }
                    }
                }
            }

            return unlockAndNotOwnedNodes;
        }
        
        public override void AsyncLoadGraphic(GameObject parentObj, bool isPreview, Action onFinished)
        {
        }
    }
}