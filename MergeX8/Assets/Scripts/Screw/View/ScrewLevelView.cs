using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using DragonPlus;
using Screw.Module;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Main/ScrewLevel")]
    public class ScrewLevelView : Entity
    {
        [UIBinder("Canvas")] 
        private Canvas _canvas;

        [UIBinder("Header")] 
        private Transform _header;
        
        [UIBinder("TasksView")] 
        private Transform _tasksViewContainer;
        [UIBinder("TaskBG")] 
        private Animator _taskAni;

        [UIBinder("LayersView")] 
        private Transform _layersViewContainer;

        [UIBinder("LevelBackground")] 
        private SpriteRenderer _levelBackground;

        [UIBinder("SlotArea")] 
        private Transform _slotArea;
        [UIBinder("TaskBG1")] 
        private SpriteRenderer _slotAreaBg;

        [UIBinder("LeftGroup")] 
        private Transform _leftGroup;

        private LevelModel levelModel;

        private List<LayerView> _layerViews;

        private List<OrderView> _taskViews;

        private int _currentTask;

        private OrderAreaView _orderAreaView;

        private float TASK_OFFSET = 7;

        private UniTaskCompletionSource _uniTask;
        private bool _moveScrewToTask;

        private List<OrderView> _currentTaskViews;
        private List<OrderModel> _currentTaskModels;
        private bool _useBoosterTwoTask = false;

        private GameObject _guideObj;
        //迁移报错注释
        //private EventBus.Listener _listener;

        public static ScrewLevelView LoadLevel(ScrewGameContext screwGameContext, LevelModel inLevelModel)
        {
            var viewAssetAttribute = typeof(ScrewLevelView).GetCustomAttribute<ViewAssetAttribute>();
            var assetObj = AssetModule.Instance.LoadAsset<GameObject>(viewAssetAttribute.AssetName);
            
            var levelView = new ScrewLevelView();
            levelView.Bind(assetObj.transform, screwGameContext);
            levelView.SetLevelModel(inLevelModel);
            levelView.SetUpLevel();
            if (screwGameContext is ScrewMiniGameContext)
            {
                levelView._levelBackground.gameObject.SetActive(false);
            }

            return levelView;
        }

        private void SetLevelModel(LevelModel inLevelModel)
        {
            levelModel = inLevelModel;
        }

        private void SetUpLevel()
        {
            _canvas.worldCamera = UIModule.Instance.UICamera;
            _canvas.planeDistance = 0;
            
            Vector3 up = _leftGroup.position;
            Vector3 upScreenPoint = UIModule.Instance.WorldCamera.WorldToScreenPoint(up);
            Vector3 upWorldPos = UIModule.Instance.WorldCamera.ScreenToWorldPoint(upScreenPoint);

            TASK_OFFSET = -upWorldPos.x;

            BindSubView();

            _taskViews = new List<OrderView>();
            for (int i = 0; i < levelModel.TaskModels.Count; i++)
            {
                var taskView = LoadEntity<OrderView>(_tasksViewContainer, context);
                taskView.SetUpTask(levelModel.TaskModels[i], _taskAni);
                if (i == 0 || i == 1)
                {
                    taskView.GetRoot().localPosition = Vector3.zero + new Vector3(-TASK_OFFSET * (i + 2), 0, 0);
                    taskView.DoLocalMove(Vector3.zero + new Vector3(-TASK_OFFSET * i, 0, 0), 0.3f);
                }
                else
                {
                    taskView.GetRoot().localPosition = Vector3.zero + new Vector3(-TASK_OFFSET * i, 0, 0);
                }
                _taskViews.Add(taskView);
            }

            _layerViews = new List<LayerView>();
            for (int i = 0; i < levelModel.LayerModels.Count; i++)
            {
                var layerView = LoadEntity<LayerView>(_layersViewContainer, context);
                layerView.SetUpLayer(levelModel.LayerModels[i]);
                _layerViews.Add(layerView);
            }

            _orderAreaView = new OrderAreaView();
            _orderAreaView.Bind(_slotArea.transform, context);
            _orderAreaView.SetUpArea();

            if (!string.IsNullOrEmpty(levelModel.GuideKey))
            {
                //迁移报错注释
                // _listener = EventBus.Subscribe<EventLanguageChange>((evt) =>
                // {
                //     RefreshGuide();
                // });
                _guideObj = AssetModule.Instance.LoadAsset<GameObject>("TutorialText", root);
                RefreshGuide();
            }
        }

        private async void RefreshGuide()
        {
            var pos = Vector3.zero;
            pos.x = levelModel.GuidePos.x;
            pos.y = levelModel.GuidePos.y;
            pos.z = 10;
            var rect = _guideObj.GetComponent<RectTransform>();
            rect.anchoredPosition3D = pos;
            _guideObj.transform.Find("Tip").GetComponent<LocalizeTextMeshProUGUI>().SetTerm(levelModel.GuideKey);
            await ScrewUtility.WaitNFrame(1);
            _guideObj.GetComponent<SpriteRenderer>().size = new Vector2(rect.sizeDelta.x + 1, rect.sizeDelta.y + 1);
        }

        private void BindSubView()
        {
            context.boostersView = new BoostersView(_canvas.transform, context);

            context.headerView = new ScrewHeaderView(_header, context);
        }

        public void SetSlotState(Transform target)
        {
            var taskViews = GetCurrentTaskViews();
            foreach (var taskView in taskViews)
            {
                taskView.SetSlotState(target);
            }
        }
        public void SetModelSlotState(Transform target)
        {
            var taskViews = GetCurrentTaskViews();
            foreach (var taskView in taskViews)
            {
                taskView.SetModelSlotState(target);
            }
        }

        private List<OrderView> GetCurrentTaskViews()
        {
            if (_currentTaskViews == null)
                _currentTaskViews = new List<OrderView>();
            else
                _currentTaskViews.Clear();
            
            _currentTaskViews.Add(_taskViews[_currentTask]);

            // 做一个保护
            if (_useBoosterTwoTask && (_currentTask + 1 < _taskViews.Count))
            {
                _currentTaskViews.Add(_taskViews[_currentTask + 1]);
            }

            return _currentTaskViews;
        }
        
        
        private List<OrderModel> GetCurrentTaskModels()
        {
            if (_currentTaskModels == null)
                _currentTaskModels = new List<OrderModel>();
            else
                _currentTaskModels.Clear();
            
            _currentTaskModels.Add(levelModel.TaskModels[_currentTask]);

            // 做一个保护
            if (_useBoosterTwoTask && (_currentTask + 1 < levelModel.TaskModels.Count))
            {
                _currentTaskModels.Add(levelModel.TaskModels[_currentTask + 1]);
            }

            return _currentTaskModels;
        }

        public Transform StorageScrew(ScrewModel screwModel)
        {
            var taskViews = GetCurrentTaskViews();
            foreach (var taskView in taskViews)
            {
                var targetTransform = taskView.StorageScrew(screwModel);
                if (targetTransform != null)
                    return targetTransform;
            }
            return _orderAreaView.StorageScrew(screwModel);
        }

        public async UniTask CheckMoveToNextTask()
        {
            bool isCompleteTask = true;
            bool isModelCompleteTask = true;
            
            bool isFirstCompleteTask = true;
            bool isFirstModelCompleteTask = true;
            var taskViews = GetCurrentTaskViews();
            var currentIndex = 0;
            foreach (var taskView in taskViews)
            {
                if (!taskView.IsModelComplete())
                {
                    isModelCompleteTask = false;
                    if (currentIndex == 0)
                        isFirstModelCompleteTask = false;
                }
                if (!taskView.IsComplete())
                {
                    isCompleteTask = false;
                    if (currentIndex == 0)
                        isFirstCompleteTask = false;
                }
                else
                {
                    context.SetCompleteTaskSafeAreaCount(_currentTask + currentIndex,
                        _orderAreaView.GetStorageJamCount());
                    taskView.PlayCompleted();
                }
                currentIndex++;
            }

            if (isCompleteTask && _uniTask == null)
            {
                _uniTask = new UniTaskCompletionSource();

                await ScrewUtility.WaitSeconds(0.5f, false);
                for (int i = 0; i < taskViews.Count; i++)
                {
                    // TODO move
                    var lastTask = taskViews[i];
                    var index = i;
                    SendBiEvent();
                    lastTask.DoLocalMove(Vector3.right * 20, 0.3f, () =>
                    {
                        if (index == taskViews.Count - 1)
                        {
                            if (_currentTask < _taskViews.Count - 1)
                            {
                                _currentTask += taskViews.Count;
                                
                                // 这里防止使用booster导致越界
                                if (_currentTask >= _taskViews.Count)
                                    _currentTask = _taskViews.Count - 1;

                                if (_useBoosterTwoTask)
                                {
                                    _useBoosterTwoTask = false;
                                    context.hookContext.OnLogicEvent(LogicEvent.TwoTaskEnd, null);
                                }
                                Debug.LogError("current task index : " + _currentTask);
                            }

                            if (_currentTask == _taskViews.Count - 1 && _taskViews[_currentTask].IsComplete())
                            {
                                if (context is not ScrewMiniGameContext)
                                    context.gameState = ScrewGameState.Win;
                            }
                            else if (_orderAreaView.CheckFail() && !_orderAreaView.CheckHasNextTaskJam(_taskViews[_currentTask].GetTaskModel()))
                            {
                                context.SetLevelFailReasonCollectableArea();
                            }

                            _uniTask.TrySetResult();
                        }
                    });
                }

                if (_currentTask + taskViews.Count < _taskViews.Count)
                {
                    var index = 0;
                    for (int i = _currentTask + taskViews.Count; i < _taskViews.Count; i++)
                    {
                        var currentTask = _taskViews[i];
                        currentTask.DoLocalMove(Vector3.zero + new Vector3(-TASK_OFFSET * index, 0, 0), 0.3f);
                        index++;
                    }
                }

                await _uniTask.Task;
                _uniTask = null;
            }
            else if (isFirstCompleteTask && _uniTask == null && taskViews.Count > 1)
            {
                _uniTask = new UniTaskCompletionSource();

                await ScrewUtility.WaitSeconds(0.5f, false);
                await UniTask.WaitUntil(() => !CheckHasScrewMoving());

                var completeCount = 1;
                if (taskViews[1].IsComplete())
                {
                    taskViews[1].DoLocalMove(Vector3.right * 20, 0.3f);
                    completeCount++;
                }

                SendBiEvent();
                taskViews[0].DoLocalMove(Vector3.right * 20, 0.3f, () =>
                {
                    if (_currentTask < _taskViews.Count - 1)
                    {
                        _currentTask += completeCount;
                        // 这里防止使用booster导致越界
                        if (_currentTask >= _taskViews.Count)
                            _currentTask = _taskViews.Count - 1;

                        if (_useBoosterTwoTask)
                        {
                            _useBoosterTwoTask = false;
                            context.hookContext.OnLogicEvent(LogicEvent.TwoTaskEnd, null);
                        }
                        Debug.LogError("current task index : " + _currentTask);
                    }

                    if (_currentTask == _taskViews.Count - 1 && _taskViews[_currentTask].IsComplete())
                        context.gameState = ScrewGameState.Win;
                    else if (_orderAreaView.CheckFail() && !_orderAreaView.CheckHasNextTaskJam(_taskViews[_currentTask].GetTaskModel()))
                    {
                        context.SetLevelFailReasonCollectableArea();
                    }

                    _uniTask.TrySetResult();
                });

                if (_currentTask + completeCount < _taskViews.Count)
                {
                    var index = 0;
                    for (int i = _currentTask + completeCount; i < _taskViews.Count; i++)
                    {
                        var currentTask = _taskViews[i];
                        currentTask.DoLocalMove(Vector3.zero + new Vector3(-TASK_OFFSET * index, 0, 0), 0.3f);
                        index++;
                    }
                }

                await _uniTask.Task;
                _uniTask = null;
            }
            else if(_orderAreaView.CheckFail())
            {
                if ((isCompleteTask || isFirstCompleteTask) && _uniTask != null)
                {
                    // 这里判断一下 当前安全区是否存在下个任务的螺丝
                    if (_currentTask < _taskViews.Count - 1 && _orderAreaView.CheckHasNextTaskJam(_taskViews[_currentTask + 1].GetTaskModel()))
                    {
                        
                    }
                    else
                    {
                        context.SetLevelFailReasonCollectableArea();
                    }
                }
                else
                {
                    if (isModelCompleteTask || isFirstModelCompleteTask)
                    {
                        if (_currentTask < _taskViews.Count - 1 && _orderAreaView.CheckHasNextTaskJam(_taskViews[_currentTask + 1].GetTaskModel()))
                        {
                        
                        }
                        else
                        {
                            context.SetLevelFailReasonCollectableArea();
                        }
                    }
                    else
                    {
                        context.SetLevelFailReasonCollectableArea();
                    }
                }
            }
        }

        private void SendBiEvent()
        {
            if (context.levelIndex == 1)
            {
                //迁移报错注释
                //BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventFunnelLevel1Delete);
            }
        }

        public void CheckCollectAreaToCurrentTask()
        {
            if (context.gameState != ScrewGameState.InProgress)
            {
                context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
                return;
            }

            if (_uniTask != null)
                return;

            bool isCompleteTask = true;
            var taskViews = GetCurrentTaskViews();
            foreach (var taskView in taskViews)
            {
                if (!taskView.IsComplete())
                {
                    isCompleteTask = false;
                    break;
                }
            }
            // bool isCompleteTask = _taskViews[_currentTask].IsComplete();
            if (!isCompleteTask)
            {
                var taskModels = GetCurrentTaskModels();
                bool isActionFinish = true;

                for (int i = 0; i < taskModels.Count; i++)
                {
                    var taskModel = taskModels[i];
                    var taskColor = taskModel.ColorType;

                    for (int j = 0; j < taskModel.Shapes.Count; j++)
                    {
                        if (taskModel.GetSlotState(j))
                            continue;

                        var shape = taskModel.Shapes[j];

                        if (!taskViews[i].CheckCouldStorage(taskColor, shape))
                            continue;

                        var screwModel = _orderAreaView.GetStorageScrewModel(taskColor, shape);
                        
                        if (screwModel != null)
                        {
                            // 必须在screwModel 不为空的情况下寻找，注意⚠️这两个if不能合并
                            var targetTransform = taskViews[i].StorageScrew(screwModel);
                            if (targetTransform != null)
                            {
                                MoveScrewToTask(screwModel, targetTransform, i == taskModels.Count - 1);
                                isActionFinish = false;
                            }
                            else
                            {
                                Debug.LogError("这里有问题，会使安全区的螺丝停在安全区，");
                            }
                        }
                    }
                }

                if (isActionFinish)
                    context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
            }
            else
                context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
        }

        private async void MoveScrewToTask(ScrewModel screwModel, Transform targetTransform, bool toNextLogic)
        {
            if (toNextLogic)
                _moveScrewToTask = true;

            var screwView = GetScrewView(screwModel);
            await UniTask.WaitUntil(() => !screwView.IsMoving());
            
            var taskViews = GetCurrentTaskViews();
            bool isComplete = false;

            if (targetTransform != null)
            {
                await screwView.DoMove(targetTransform);
            }
            else
            {
                Debug.LogError("这里有问题，会使安全区的螺丝停在安全区，");
            }
            
            for (int i = 0; i < taskViews.Count; i++)
            {
                var taskView = taskViews[i];
                if (taskView.IsComplete())
                    isComplete = true;
            }

            bool hasConnect = context.CheckConnectLineScrewJam();
            if (toNextLogic)
            {
                _moveScrewToTask = false;

                if (hasConnect)
                    return;
                
                // 当前任务都完成了
                if (isComplete)
                {
                    context.hookContext.OnLogicEvent(LogicEvent.CheckTask, null);
                }
                else
                {
                    context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
                }
            }
        }

        public ScrewView GetScrewView(ScrewModel screwModel)
        {
            return _layerViews[screwModel.LayerId].GetScrewView(screwModel);
        }

        public ScrewView GetScrewView(int screwId)
        {
            for (int i = 0; i < _layerViews.Count; i++)
            {
                var view = _layerViews[i].GetScrewView(screwId);
                if (view != null)
                {
                    return view;
                }
            }

            return null;
        }
        public bool CheckHasScrewMoving()
        {
            for (int i = 0; i < _layerViews.Count; i++)
            {
                var iScrewMoving = _layerViews[i].CheckHasScrewMoving();
                if (iScrewMoving)
                {
                    return true;
                }
            }

            return false;
        }
        
        public T GetBlockerView<T>(ScrewBlocker type, ScrewModel screwModel) where T : BaseBlockerView
        {
            return _layerViews[screwModel.LayerId].GetBlockerView<T>(type, screwModel);
        }
        
        public List<BaseBlockerView> GetAllBlockerView(ScrewBlocker type)
        {
            var list = new List<BaseBlockerView>();
            foreach (var layerView in _layerViews)
            {
                var blockerViews = layerView.GetAllBlockerView(type);
                if (blockerViews != null)
                    list.AddRange(blockerViews);
            }
            return list;
        }

        public void AddSlot(bool playEff)
        {
            _orderAreaView.AddSlot(playEff);
            var useCount = context.boostersHandler.GetHandler<ExtraSlotBoosterHandler>(BoosterType.ExtraSlot).GetUseCount();
            var count = useCount / 2;
            if (useCount % 2 == 1)
                count++;

            _slotAreaBg.size = new Vector2(8 + count * 2.4f, 1.27f);
        }

        public bool GetCouldUseTwoTaskBooster()
        {
            return !_useBoosterTwoTask && _currentTask < _taskViews.Count - 1;
        }

        public void UseBoosterTwoTask()
        {
            if (_currentTask >= _taskViews.Count - 1)
                return;

            if (context.gameState != ScrewGameState.InProgress)
                return;

            if (_uniTask != null)
                return;
            SoundModule.PlaySfx("sfx_useItem_box");
            _useBoosterTwoTask = true;
             UIModule.Instance.EnableEventSystem = false;
             //  移动任务位置
             _taskViews[_currentTask].DoLocalMove(Vector3.right * 2.5f, 0.2f);
             _taskViews[_currentTask+1].DoLocalMove(Vector3.left * 2.5f, 0.2f, () =>
             {
                 CheckCollectAreaToCurrentTask();
                 UIModule.Instance.EnableEventSystem = true;
             });
        }
        
        public bool SlotAreaFull()
        {
            return _orderAreaView.CheckFail();
        }

        public void OnEnterLevel()
        {
            context.boostersView.OnEnterLevel();
        }

        public void ExitLevel()
        {
            //迁移报错注释
            // if (_listener != null)
            //     EventBus.UnSubscribe(_listener);
            // _listener = null;

            context.boostersView.OnExitLevel();
            for (int i = 0; i < _layerViews.Count; i++)
                _layerViews[i].Destroy();
            Destroy();
        }

        public void EnterBreakPanel()
        {
            for (int i = 0; i < _layerViews.Count; i++)
                _layerViews[i].EnterBreakPanel();
        }

        public void ExitBreakPanel()
        {
            for (int i = 0; i < _layerViews.Count; i++)
                _layerViews[i].ExitBreakPanel();
        }

        public void RefreshTaskStatus(ScrewModel screwModel)
        {
            var taskViews = GetCurrentTaskViews();
            foreach (var taskView in taskViews)
            {
                taskView.RefreshTaskStatus(screwModel);
            }
        }

        public int GetCollectJamCount()
        {
            var count = 0;
            for (int i = 0; i < _layerViews.Count; i++)
                count += _layerViews[i].GetCollectJamCount();
            return count;
        }

        public uint GetSafeAreaStorageJamCount()
        {
            return _orderAreaView.GetStorageJamCount();
        }
        public uint GetAllAreaCount()
        {
            return _orderAreaView.GetAllAreaCount();
        }

        public bool IsMovingTask()
        {
            return _uniTask != null || _moveScrewToTask;
        }
        
        public bool HasPanelMoving()
        {
            foreach (var layerView in _layerViews)
            {
                var isMoving = layerView.HasPanelMoving();
                if (isMoving)
                    return true;
            }

            return false;
        }

        public bool HasShield()
        {
            foreach (var layerView in _layerViews)
            {
                var hasShield = layerView.HasShield();
                if (hasShield)
                    return true;
            }

            return false;
        }

        public bool HaveActiveShield()
        {
            foreach (var layerView in _layerViews)
            {
                var hasShield = layerView.HaveActiveShield();
                if (hasShield)
                    return true;
            }

            return false;
        }

        public async UniTask  RefreshShield()
        {
            if (!HasShield())
                return;
            
            if(!HaveActiveShield())
                return;
            
            foreach (var layerView in _layerViews)
            {
               await layerView.RefreshShield(_layerViews);
            }

            if (HaveActiveShield())
                return;
            
            if (context is not ScrewMiniGameContext)
                return;
            
            context.gameState = ScrewGameState.Win;
            context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
        }
    }
}