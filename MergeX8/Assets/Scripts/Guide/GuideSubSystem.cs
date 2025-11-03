using System;
using DragonU3DSDK;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using DragonPlus;
using UnityEngine;
using DG.Tweening;
using DragonU3DSDK.Config;
using System.Collections;
using System.Linq;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;

public class GuideSubSystem : GlobalSystem<GuideSubSystem>
{
    public Dictionary<int, int> CacheGuideFinished => _cacheGuideFinished;
    private Dictionary<int, int> _cacheGuideFinished = new Dictionary<int, int>();
    private Dictionary<GuideTargetType, Dictionary<string, RectTransform>> _targetCacheDic = new Dictionary<GuideTargetType, Dictionary<string, RectTransform>>();
    private Dictionary<GuideTargetType, List<RectTransform>> _moveTargetCacheDic = new Dictionary<GuideTargetType, List<RectTransform>>();
    private Dictionary<GuideTargetType, Dictionary<string, List<Transform>>> _topLayerDic =
        new Dictionary<GuideTargetType, Dictionary<string, List<Transform>>>();

    private List<int> _filterGuides = new List<int>();
    
    private TableGuide _currentConfig;
    public TableGuide CurrentConfig
    {
        get { return _currentConfig; }
    }

    private TableGuide _finishConfig;
    
    public const string TARGET_DEFAULT_KEY = "Empty";
    private bool isHideGuide = false;

    private bool canBackHome = true;
    private bool canProduct = true;
    private bool canEnterGame = true;

    private bool isLockGuide = false;

    private float _finishGuideTime = 0;
    
    private Coroutine _coroutine;

    private StorageHome _storageHome = null;
    private StorageHome storageHome
    {
        get
        {
            if (_storageHome == null)
                _storageHome = StorageManager.Instance.GetStorage<StorageHome>();
            return _storageHome;
        }
    }

    public bool Trigger(GuideTriggerPosition position, string param, string triggerParam = null)
    {
        if (_currentConfig != null && _currentConfig.triggerPosition == (int) position &&
            (string.IsNullOrEmpty(param) || _currentConfig.triggerParam == param))
        {
            if (isHideGuide)
            {
                if(position == GuideTriggerPosition.EatFood)
                    Trigger(_currentConfig, triggerParam);
                else
                {
                    TriggerCurrent();
                }
            }

            return true;
        }

        TableGuide guideConfig = findMatchConfig(position, param);
        if (guideConfig == null)
            return false;

        if (_currentConfig != null && guideConfig.priority > _currentConfig.priority)
            return false;

        if (_finishConfig != null && _finishConfig.group > 0 && guideConfig.group != _finishConfig.group)
        {
            if (!IsGroupAllFinish(_finishConfig))
                return false;
        }
        
        return Trigger(guideConfig, triggerParam);
    }

    private TableGuide findMatchConfig(GuideTriggerPosition position, string param)
    {
        var matchList = GetTableGuides(position, param);
        if (matchList == null)
            return null;

        for (int i = 0; i < matchList.Count; i++)
        {
            if (!matchList[i].triggerParam.IsEmptyString() && !matchList[i].triggerParam.Equals(param))
                continue;

            if (isFinished(matchList[i].id))
                continue;

            if (matchList[i].targetType == (int)GuideTargetType.Asmr) 
            {
                if(!Makeover.Utils.IsOpen)
                    continue;
            }
            
            if (matchList[i].preFinish)
            {
                //找寻前置任务 是否完成
                int index = GlobalConfigManager.Instance.GetGuides().FindIndex(a=>a == matchList[i]);
                index = index - 1;
                if (index > 0)
                {
                    TableGuide preConfig = GlobalConfigManager.Instance.GetGuides()[index];
                    if (preConfig != null)
                    {
                        if (!isFinished(preConfig.id))
                            continue;
                    }
                }
            }

            if (matchList[i].preGuideId > 0)
            {
                if (!isFinished(matchList[i].preGuideId))
                    continue;
            }

            if (!isFinished(matchList[i].id))
            {
                return matchList[i];
            }
        }

        return null;
    }
    
    public bool TriggerCurrent()
    {
        if (_currentConfig == null)
            return false;

        return Trigger(_currentConfig, null);
    }

    public bool isFinished(GuideTriggerPosition position, string param = null)
    {
        var matchList = GetTableGuides(position, param);
        if (matchList == null)
            return true;

        for (int i = 0; i < matchList.Count; i++)
        {
            if (!isFinished(matchList[i].id))
                return false;
        }

        return true;
    }

    private bool Trigger(TableGuide config, string triggerParam)
    {
        if (config == null)
            return false;
        if (config.id == 510 && !MermaidModel.Instance.IsOpened())
        {
            return false;
        }
        _currentConfig = config;
        DebugUtil.Log($"Begin GuideId:{_currentConfig.id}");

        if (_coroutine != null)
            CoroutineManager.Instance.StopCoroutine(_coroutine);
        _coroutine = CoroutineManager.Instance.StartCoroutine(showGuide(triggerParam));
        sendTriggerBI();

        isLockGuide = _currentConfig.lockTime > 0;

        //ASMR 兼容
        if (config.targetType == (int)GuideTargetType.EnterASMR)
        {
            if (isFinished(101))
            {
                FinishCurrent();
            }
            else
            {
                //FinishCurrent();
                if (Makeover.Utils.IsOpen)
                {
                    //SceneFsm.mInstance.ChangeState(StatusType.Makeover, MakeoverConfigManager.Instance.levelList[0]);
                }
                else
                {
                    Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
                }
            }
        }
        return true;
    }

    private void sendTriggerBI()
    {
        if(_currentConfig == null)
            return;
        
        if(_currentConfig.tiggerBi.IsEmptyString())
            return;
        
        BiEventAdventureIslandMerge.Types.GameEventType biEvent;
        GameBIManager.TryParseGameEventType(_currentConfig.tiggerBi, out biEvent);
        GameBIManager.Instance.SendGameEvent(biEvent);
    }

    private void sendFinishBI(TableGuide config = null)
    {
        config = config == null ? _currentConfig : config;
        if (config == null)
            return;

        if (string.IsNullOrEmpty(config.finishBi))
            return;
        
        BiEventAdventureIslandMerge.Types.GameEventType biEvent;
        GameBIManager.TryParseGameEventType(config.finishBi, out biEvent);
        GameBIManager.Instance.SendGameEvent(biEvent);
    }

    IEnumerator showGuide(string triggerParam)
    {
        if (_currentConfig == null)
            yield break;

        RectTransform target = null;
        var targetType = _currentConfig.targetType;
        var targetParam = triggerParam;

        //UIRoot.Instance.EnableEventSystem = false;
        float time = 0;
        bool isReset = false;
        while (targetType > 0 && ( target == null || (target != null && !target.gameObject.activeSelf)))
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Dictionary<string, RectTransform> typeDic = null;
            time += Time.deltaTime;
            if (_targetCacheDic.TryGetValue((GuideTargetType) targetType, out typeDic))
            {
                targetParam = string.IsNullOrEmpty(targetParam) ? TARGET_DEFAULT_KEY : targetParam;
                typeDic.TryGetValue(targetParam, out target);
            }
            else if (time >= 0.5f && !isReset)
            {
                if (MergeMainController.Instance != null)
                    MergeMainController.Instance.SetGuideMaskActive(false);
                isReset = true;
            }
        }

        isHideGuide = false;
        UIRoot.Instance.EnableEventSystem = false;
        // 引导界面，为了引导准确，等待打开动画
        if (_currentConfig != null && !_currentConfig.isImmedShow)
        {
            float waitTime = _currentConfig.waitTime;
            waitTime = waitTime == 0f ? 0.3f : waitTime;
            yield return new WaitForSeconds(waitTime);
        }

        if (_currentConfig != null)
        {
            canBackHome = _currentConfig.canBackHome;
            canProduct = _currentConfig.canProduct;
            canEnterGame = !_currentConfig.maskEnterGame;
            
            UIManager.Instance.OpenUI(UINameConst.UIGuidePortrait, _currentConfig, targetParam);

            if (_currentConfig.targetType != (int)GuideTargetType.InfoDes &&  _currentConfig.targetType != (int)GuideTargetType.CloseItemInfo)
            {
                UIManager.Instance.CloseUI(UINameConst.UIPopupMergeInformation, true);
                UIManager.Instance.CloseUI(UINameConst.UIPopupMergeInformationExplain, true);
            }
            if(_currentConfig.targetType!=771 && _currentConfig.targetType!=772)
                UIManager.Instance.CloseUI(UINameConst.UIPopupMergePackage, true);
            UIManager.Instance.CloseUI(UINameConst.UIPopupBattlePass2Refresh, true);
            UIManager.Instance.CloseUI(UINameConst.UIPopupBattlePassRefresh, true);
            
            // if (_currentConfig.targetType != (int)GuideTargetType.GetFreeReward)
            // {
            //     UIManager.Instance.CloseUI(UINameConst.UIStore, true);
            //     UIManager.Instance.CloseUI(UINameConst.UIStoreGame, true);
            // }

            if (_currentConfig.triggerPosition == (int)GuideTriggerPosition.EnterHome)
            {
                UIManager.Instance.CloseUI(UINameConst.UIPopupGameTabulation, true);
            }
            
            storageHome.IsBackGame = _currentConfig.backGame;
        }

        _coroutine = null;
        UIRoot.Instance.EnableEventSystem = true;

        if (MergeMainController.Instance.MergeBoard != null)
            MergeMainController.Instance.MergeBoard.RestoreInput();
    }

    public void RegisterTarget(GuideTargetType targetType, Transform targetTransform, List<RectTransform> moveToTargets = null, string targetParam = null, bool isReplace = true, Transform topLayer = null)
    {
        List<Transform> topLayerList = null;
        if (topLayer != null)
        {
            topLayerList = new List<Transform>();
            topLayerList.Add(topLayer);
        }

        RegisterTarget(targetType, targetTransform as RectTransform, moveToTargets, targetParam, isReplace, topLayerList);
    }
    
    public void RegisterTarget(GuideTargetType targetType, RectTransform targetTransform,
        RectTransform moveToTarget = null, string targetParam = null, bool isReplace = true, List<Transform> topLayer = null)
    {
        List<RectTransform> moveToTargets = null;
        if (moveToTarget != null)
        {
            moveToTargets = new List<RectTransform>();
            moveToTargets.Add(moveToTarget);
        }

        RegisterTarget(targetType, targetTransform, moveToTargets, targetParam, isReplace, topLayer);
    }

    public void ClearTarget(GuideTargetType targetType)
    {
        _targetCacheDic.Remove(targetType);
        _moveTargetCacheDic.Remove(targetType);
        _topLayerDic.Remove(targetType);
    }

    public void RegisterTarget(GuideTargetType targetType, RectTransform targetTransform,
        List<RectTransform> moveToTargets, string targetParam = null, bool isReplace = true, List<Transform> topLayer = null)
    {
        if (string.IsNullOrEmpty(targetParam))
        {
            targetParam = TARGET_DEFAULT_KEY;
        }

        if (_targetCacheDic.ContainsKey(targetType))
        {
            if (_targetCacheDic[targetType].ContainsKey(targetParam))
            {
                if(isReplace)
                    _targetCacheDic[targetType][targetParam] = targetTransform;
            }
            else
            {
                _targetCacheDic[targetType].Add(targetParam, targetTransform);
            }
        }
        else
        {
            var temp = new Dictionary<string, RectTransform>();
            temp.Add(targetParam, targetTransform);
            _targetCacheDic.Add(targetType, temp);
        }

        if (_moveTargetCacheDic.ContainsKey(targetType))
        {
            _moveTargetCacheDic[targetType] = moveToTargets;
        }
        else
        {
            _moveTargetCacheDic.Add(targetType, moveToTargets);
        }

        if (_topLayerDic.ContainsKey(targetType))
        {
            if (_topLayerDic[targetType].ContainsKey(targetParam))
            {
                if(isReplace)
                    _topLayerDic[targetType][targetParam] = topLayer;
            }
            else
            {
                _topLayerDic[targetType].Add(targetParam, topLayer);
            }
        }
        else
        {
            _topLayerDic[targetType] = new Dictionary<string, List<Transform>>();
            _topLayerDic[targetType].Add(targetParam, topLayer);
        }
    }

    public void HideCurrent()
    {
        isHideGuide = true;
        storageHome.IsBackGame = false;
        UIManager.Instance.CloseUI(UINameConst.UIGuidePortrait, false); // guide 频繁出现，缓存
        if (_coroutine != null)
        {
            CoroutineManager.Instance.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void CloseCurrent(bool isInit = false)
    {
        if (isInit)
        {
            canBackHome = true;
            canProduct = true;
            canEnterGame = true;
        }

        isHideGuide = false;
        isLockGuide = false;
        _currentConfig = null;
        storageHome.IsBackGame = false;
        UIManager.Instance.CloseUI(UINameConst.UIGuidePortrait, false); // guide 频繁出现，缓存
        if (_coroutine != null)
        {
            CoroutineManager.Instance.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void FinishCurrent(bool isSkip = false)
    {
        if (_currentConfig == null)
            return;

        int id = _currentConfig.id;
        FinishCurrent((GuideTargetType) _currentConfig.targetType, null, isSkip);

        if (id != 703 && SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            MergeGuideLogic.Instance.CheckMergeGuide();
    }

    public bool IsActionParams(string param)
    {
        if (_currentConfig == null)
            return false;

        if (_currentConfig.finishParam.IsEmptyString())
            return false;

        return _currentConfig.finishParam.Equals(param);
    }
    
    public int GetIntActionParams()
    {
        if (_currentConfig == null)
            return -1;

        if (_currentConfig.finishParam.IsEmptyString())
            return -1;

        if (!int.TryParse(_currentConfig.finishParam, out var result))
            return -1;

        return result;
    }

    public bool IsTargetParams(string param)
    {
        if (_currentConfig == null)
            return false;

        return _currentConfig.triggerParam.Equals(param);
    }

    public string GetActionParams()
    {
        if (_currentConfig == null)
            return null;

        if (_currentConfig.finishParam == null)
            return null;

        return _currentConfig.finishParam;
    }

    public void FinishCurrent(GuideTargetType sourceTarget, string finishParam = null, bool isSkip = false)
    {
        if (_currentConfig == null)
            return;

        if (isLockGuide)
        {
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(_currentConfig.lockTime, () =>
            {
                isLockGuide = false;
            }));

            return;
        }
        
        if (!finishParam.IsEmptyString() && !_currentConfig.finishParam.IsEmptyString())
        {
            if (!_currentConfig.finishParam.Equals(finishParam))
                return;
        }

        //检测目标是否为当前target
        if (sourceTarget != (GuideTargetType) _currentConfig.targetType)
            return;

        UIManager.Instance.CloseUI(UINameConst.UIGuidePortrait, false); // guide 频繁出现，缓存
        
        sendFinishBI();

        var tempCfg = _currentConfig;
        _currentConfig = null;
        
        // 有可能开启下一个guide，所以 之前就要把 当前guide清空
        setFinished(tempCfg);

        if (isSkip)
        {
            if (tempCfg.skipGuides != null)
            {
                foreach (var guideId in tempCfg.skipGuides)
                {
                    List<TableGuide> skipGuides = GlobalConfigManager.Instance.GetGuides().FindAll(a => a.id == guideId);
                    if (skipGuides != null)
                    {
                        foreach (var config in skipGuides)
                        {
                            sendFinishBI(config);
                            setFinished(config);

                            if (_currentConfig == config)
                                CloseCurrent();
                        }
                    }
                }
            }

            canBackHome = true;
            canProduct = true;
            canEnterGame = true;
        }

        if (MergeManager.Instance != null)
            MergeMainController.Instance.SetGuideMaskActive(false);
        storageHome.IsBackGame = false;

        _finishGuideTime = Time.realtimeSinceStartup;
    }

    public void ForceFinished(int id)
    {
        var guides = GlobalConfigManager.Instance.GetGuides();
        var guide = guides.Find(a => a.id == id);
        if (guide == _currentConfig)
        {
            UIManager.Instance.CloseUI(UINameConst.UIGuidePortrait, false);
        }
        setFinished(guide);
    }
    private void setFinished(TableGuide config)
    {
        if (config == null)
            return;

        var guideId = config.id;
        DebugUtil.Log($"{GetType()} finish guide id = {guideId}.");

        if (!_cacheGuideFinished.ContainsKey(guideId))
        {
            _cacheGuideFinished.Add(guideId, guideId);
        }
        //真正保存进度
        if (config.saveFlag)
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
            if (storage != null)
            {
                if (!storage.ContainsKey(guideId))
                {
                    storage.Add(guideId, guideId);
                }
            }

            IterationSave(config);
        }

        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.GuideFinish,config);
        //教程结束触发新教程
        Trigger(GuideTriggerPosition.GuideEnd, config.id.ToString());
        if (config.nextGuideid > 0)
        {
            TableGuide autoConfig = GlobalConfigManager.Instance.GetGuides().Find(c => c.id == config.nextGuideid);
            if (autoConfig != null)
            {
                if (autoConfig.checkEmptyGrid)
                {
                    if (MergeManager.Instance != null)
                    {
                        int emptyGrid = MergeManager.Instance.FindEmptyGrid(1,MergeBoardEnum.Main);
                        if(emptyGrid < 0)
                            return;
                    }
                }
                Trigger(autoConfig, null);
            }
        }
        
        _finishConfig = config;
    }

    public void SaveStorage(GuideTriggerPosition trigger)
    {
        TableGuide config = findMatchConfig(trigger, null);
        if (config == null)
            return;

        var storage = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
        if (storage != null)
        {
            if (!storage.ContainsKey(config.id))
            {
                storage.Add(config.id, config.id);
            }
        }

        IterationSave(config);
    }

    public void SaveCurrentStorage()
    {
        if (_currentConfig == null)
            return;

        var storage = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
        if (storage != null)
        {
            if (!storage.ContainsKey(_currentConfig.id))
            {
                storage.Add(_currentConfig.id, _currentConfig.id);
            }
        }

        IterationSave(_currentConfig);
    }

    private void IterationSave(TableGuide config)
    {
        if (config == null)
            return;

        if (config.saveGuideIds == null || config.saveGuideIds.Length == 0)
            return;

        for (int i = 0; i < config.saveGuideIds.Length; i++)
        {
            TableGuide findConfig = GlobalConfigManager.Instance.GetGuides().Find(c => c.id == config.saveGuideIds[i]);
            if (findConfig == null)
                continue;

            var storage = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
            if (storage == null)
                continue;

            if (storage.ContainsKey(findConfig.id))
                continue;

            storage.Add(findConfig.id, findConfig.id);
            IterationSave(findConfig);
        }
    }

    public bool isFinished(int guideId)
    {
        if (DebugCmdExecute.isFBVersion)
            return true;
        
        if (_cacheGuideFinished.ContainsKey(guideId)) 
            return true;
        
        var storage = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
        return storage != null && storage.ContainsKey(guideId);
    }

    public RectTransform GetTarget(GuideTargetType targetType, string param)
    {
        if (!_targetCacheDic.ContainsKey(targetType))
            return null;
        
        if (string.IsNullOrEmpty(param))
            return _targetCacheDic[targetType][TARGET_DEFAULT_KEY];
        
        return _targetCacheDic[targetType][param];
    }

    public List<Transform> GetTopLayers(GuideTargetType targetType, string param)
    {
        if (!_topLayerDic.ContainsKey(targetType))
            return null;
        
        if (string.IsNullOrEmpty(param))
            return _topLayerDic[targetType][TARGET_DEFAULT_KEY];
        
        return _topLayerDic[targetType][param];
    }
        
    public List<RectTransform> GetMoveTarget(GuideTargetType targetType)
    {
        if (!_moveTargetCacheDic.ContainsKey(targetType))
            return null;
        
        return _moveTargetCacheDic[targetType];
    }

    public bool IsShowingGuide(bool printLog = false)
    {
        if (printLog)
        {
            var guideId = _currentConfig != null ? _currentConfig.id.ToString() : "空";
            Debug.LogWarning($"是否处于引导链中: [{InGuideChain}] 当前引导ID: [{guideId}]");
        }
        return InGuideChain || _currentConfig != null && !_currentConfig.isWeak;
    }

    public bool IsGuideFrozen()
    {
        if (IsShowingGuide())
            return true;
        
        return Time.realtimeSinceStartup - _finishGuideTime < 5f;
    }
    
    public bool InGuideChain //是否处于引导链中
    {
        get;
        set;
    }
    // public bool InNewPlayerGuideChain //是否处于新手引导链中
    // {
    //     get;
    //     set;
    // }

    private List<TableGuide> GetTableGuides(GuideTriggerPosition position, string param)
    {
        List<TableGuide> configs = GlobalConfigManager.Instance.GetGuidesByPosition((int)position);

        if (configs == null || configs.Count == 0)
            return null;
        
        int filterType = -1;
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Transition)
        {
            if(((SceneFsmTransition)SceneFsm.mInstance.GetCurrentState()).nextSceneType == StatusType.HappyGoGame)
                filterType = 2;
            else if(((SceneFsmTransition)SceneFsm.mInstance.GetCurrentState()).nextSceneType == StatusType.Game)
                filterType = 0;
            else if(((SceneFsmTransition)SceneFsm.mInstance.GetCurrentState()).nextSceneType == StatusType.EnterStimulate)
                filterType = 3;
        }
        else if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.HappyGoGame)
        {
            filterType = 2;
        }
        else if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        {
            filterType = 0;
        }
        else if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.EnterStimulate)
        {
            filterType = 3;
        }
        else if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.EnterFilthy)
        {
            filterType = 5;
        }
        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIButterflyWorkShopMain) != null)
        {
            filterType = 4;
        }

        List<TableGuide> filterTables = new List<TableGuide>(configs);
        if (filterType >= 0)
        {
            for (int i = 0; i < filterTables.Count; i++)
            {
                if (filterTables[i].guideType == filterType)
                   continue;
                
                filterTables.Remove(filterTables[i]);
                i--;
            }
        }

        if (_filterGuides.Count > 0)
        {
            for (int i = 0; i < filterTables.Count; i++)
            {
                if(_filterGuides.Contains(filterTables[i].id))
                    continue;
                
                filterTables.Remove(filterTables[i]);
                i--;
            }
        }
        
        
        if (filterTables == null || filterTables.Count == 0)
            return null;
        
        if (param.IsEmptyString())
            return filterTables;

        var matchList = filterTables.FindAll(a => a.triggerParam == param);

        return matchList;
    }

    public bool ShieldDecoClose()
    {
        if (!IsShowingGuide())
            return false;

        return _currentConfig.shieldDecoClose;
    }

    public bool IsTargetTypeGuide(GuideTargetType type)
    {
        if (_currentConfig == null)
            return false;

        return _currentConfig.targetType == (int) type;
    }

    public bool CanProduct()
    {
        return canProduct;
    }

    public bool CanBackHome()
    {
        return canBackHome;
    }

    public bool CanEnterGame()
    {
        return canEnterGame;
    }

    public bool AutoChoose()
    {
        if (!IsShowingGuide())
            return false;

        return _currentConfig.autoChose;
    }

    public int MergeStartIndex()
    {
        if (CurrentConfig == null)
            return -1;

        return CurrentConfig.mergeStartIndex;
    }
    
    public int MergeEndIndex()
    {
        if (CurrentConfig == null)
            return -1;

        return CurrentConfig.mergeEndIndex;
    }
    
    public List<string> GetActionParams(GuideTriggerPosition position)
    {
        List<TableGuide> tableGuides = GetTableGuides(position, "");
        if (tableGuides == null || tableGuides.Count == 0)
            return null;

        List<string> actionParams = new List<string>();
        foreach (var config in tableGuides)
        {
            if (config.finishParam.IsEmptyString())
                continue;

            if (isFinished(config.id))
                continue;

            if (config.preGuideId > 0)
            {
                if (!isFinished(config.preGuideId))
                    continue;
            }

            if (config.preFinish)
            {
                if(!IsPreFinish(config))
                    continue;
            }

            actionParams.Add(config.finishParam);
        }

        return actionParams;
    }

    private bool IsPreFinish(TableGuide guide)
    {
        int index = GlobalConfigManager.Instance.GetGuides().FindIndex(a=>a == guide);
        index = index - 1;
        if (index > 0)
        {
            TableGuide preConfig = GlobalConfigManager.Instance.GetGuides()[index];
            if (preConfig != null)
            {
                if (!isFinished(preConfig.id))
                    return false;
            }
        }

        return true;
    }

    private bool IsGroupAllFinish(TableGuide guide)
    {
        if (guide.group <= 0)
            return true;

        var guides = GlobalConfigManager.Instance.GetGuides().FindAll(a => a.group == guide.group);
        if (guides == null || guides.Count == 0)
            return true;
        
        foreach (var tableGuide in guides)
        {
            if (!isFinished(tableGuide.id))
                return false;
        }

        return true;
    }

    public void SetFilterGuide(List<int> guides)
    {
        _filterGuides.Clear();
        
        if(guides == null)
            return;
        _filterGuides = new List<int>(guides);
    }

    public void CleanFilterGuide()
    {
        _filterGuides.Clear();
    }

    public void CleanGuide()
    {
        _cacheGuideFinished.Clear();
        _targetCacheDic.Clear();
        _moveTargetCacheDic.Clear();
        _topLayerDic.Clear();
        _filterGuides.Clear();
    }
}