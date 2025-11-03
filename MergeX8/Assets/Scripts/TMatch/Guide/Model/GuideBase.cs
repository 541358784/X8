using System.Collections.Generic;
using Dlugin;
using DragonPlus.Config.OutsideGuide;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.EventSystems;
using TMatch;
namespace OutsideGuide
{
    public abstract class GuideBase<T> : Manager<T> where T : GuideBase<T>
    {
        private Transform root;
        protected List<int> _guideGroups = new List<int>();
        public int CurrentGuideId { get; set; }
        public int CurrentStepId { get; set; }
        public bool IsRunning { get; set; }
        public GuideMask GuideMaskComponent { get; set; }
        public GuideNpcSay GuideNpcSayComponent { get; set; }
        public GuideSkip GuideSkipComponent { get; set; }
        public GuideTip GuideTipComponent { get; set; }
        protected GuideStepExcutor GuideStepExecutor;
        public GuideEvent TriggerEventData;
        public bool OpenGuide { get; set; } = true;
        public bool MakePause = false; //引导导致暂停
        protected List<GuideStep> CurrentGuideSteps = new List<GuideStep>();

        protected override void InitImmediately()
        {
            base.InitImmediately();
        }

        public virtual void Init()
        {
            TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.GUIDE_EVENT, OnGameEvent);
        }

        public virtual void Release()
        {
            TMatch.EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.GUIDE_EVENT, OnGameEvent);
            _guideGroups.Clear();
            CurrentGuideSteps.Clear();
            Reset();
        }

        protected virtual void Awake()
        {
            root = TMatch.UIRoot.Instance.mGuideRoot.transform;

            GuideMaskComponent = FindCanUseItem<GuideMask>();
            GuideMaskComponent.OnTouchMaskEvent = OnTouchMaskEvent;
            GuideMaskComponent.Hide();

            GuideNpcSayComponent = FindCanUseItem<GuideNpcSay>();
            GuideNpcSayComponent.Hide();
            
            HideArrow();

            GuideTipComponent = FindCanUseItem<GuideTip>();
            GuideTipComponent.Hide(true);
        }

        protected virtual T FindCanUseItem<T>(bool isNew = true) where T : GuideGraphicBase
        {
            var prefabName = typeof(T).Name;
            string path = $"TMatch/Guide/Prefabs/{prefabName}";
            T t = default(T);
            if (!isNew)
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i);
                    t = child.GetComponent<T>();
                    if (t == null || t.IsShow()) continue;
                    t.gameObject.SetActive(true);
                    return t;
                }
            }

            GameObject go = ResourcesManager.Instance.LoadResource<GameObject>(path);
            if (go == null)
            {
                DebugUtil.LogError($"{prefabName} prefab is null,cannot Instantiate " + path);
            }

            GameObject ui = Instantiate(go, root);
            ui.name = $"{prefabName}";
            t = ui.GetComponent<T>();
            if (t == null) t = ui.AddComponent<T>();
            t.transform.SetParent(root, false);
            return t;
        }

        protected virtual void Update()
        {
            if (!IsRunning) return;
            if (!CheckStepCanContinue()) return;
            GuideStepExecutor.OnStepEnded();
            CurrentStepId++;
            ActionStep();
        }

        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 检测引导分步是否可以继续
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckStepCanContinue()
        {
            if (GuideStepExecutor == null) return false;
            if (!GuideStepExecutor.IsFinished()) return false;
            return true;
        }

        /// <summary>
        /// 检测当前引导中有没有符合的
        /// </summary>
        protected virtual void CheckCanDoGuide()
        {
            foreach (var guideItem in _guideGroups)
            {
                if (GetGuideState(guideItem)) continue;
                TMatch.EventDispatcher.Instance.DispatchEventImmediately(new TMatch.BaseEvent(TMatch.EventEnum.GAMEGUIDE_START, guideItem));
                break;
            }
        }

        protected virtual void OnGameEvent(TMatch.BaseEvent obj)
        {
            if (obj == null) return;
            GuideEvent ggEvent = obj as GuideEvent;
            if (ggEvent == null) return;
            GuideStepExecutor?.OnGameEvent(ggEvent.type, ggEvent.ParamId);
        }

        protected virtual void OnGuideBegan(bool isShowTip = false)
        {
            MakePause = false;
            IsRunning = true;
            CurrentStepId = 0;
            GuideSkipComponent?.Show();
            if(isShowTip) GuideTipComponent?.Show();
            ActionStep();
            CommonEvent<int>.DispatchEvent(TMatch.EventEnum.GAMEGUIDE_START, CurrentGuideId);
        }

        protected virtual void OnGuideEnd()
        {
            TriggerEventData?.EndAction?.Invoke();
            TMatch.EventDispatcher.Instance.DispatchEvent(new GuideEvent($"GuideFinished", CurrentGuideId));
            CommonEvent<int>.DispatchEvent(TMatch.EventEnum.GAMEGUIDE_END, CurrentGuideId);
            SaveGuide();
            Reset();
        }

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            IsRunning = false;

            GuideStepExecutor = null;
            HideArrow();
            GuideMaskComponent?.Hide();
            GuideNpcSayComponent?.Hide();
            GuideSkipComponent?.Hide();
            GuideTipComponent?.Hide();
            TriggerEventData = null;
            CurrentGuideId = 0;
            CurrentStepId = 0;
        }

        public void HideArrow()
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                GuideArrow t = child.GetComponent<GuideArrow>();
                if (t == null) continue;
                t.Hide();
            }
        }

        /// <summary>
        /// 注册引导
        /// </summary>
        public virtual void ExecutorGuide(int guideGroupId)
        {
            RemoveGuide(guideGroupId);
            AddGuideGroup(guideGroupId);
        }

        /// <summary>
        /// 注册引导
        /// </summary>
        public virtual void ExecutorGuide(string guidekey)
        {
            RemoveGuide(guidekey);
            AddGuideGroup(guidekey);
        }

        /// <summary>
        /// 添加引导组进入缓存
        /// </summary>
        /// <param name="guideGroupId"></param>
        protected virtual void AddGuideGroup(int guideGroupId)
        {
            if (_guideGroups.Contains(guideGroupId)) return;
            _guideGroups.Add(guideGroupId);
        }

        /// <summary>
        /// 添加引导组key进入缓存
        /// </summary>
        /// <param name="guideKey"></param>
        protected virtual void AddGuideGroup(string guideKey)
        {
            var id = ConvertGuideKeyToID(guideKey);
            if (_guideGroups.Contains(id)) return;
            _guideGroups.Add(id);
        }

        protected virtual int ConvertGuideKeyToID(string guideKey)
        {
            return 0;
        }

        /// <summary>
        /// 注销引导
        /// </summary>
        public virtual void RemoveGuide(int guideGroupId)
        {
            if (_guideGroups.Contains(guideGroupId)) return;
            _guideGroups.Remove(guideGroupId);
        }

        /// <summary>
        /// 注销引导
        /// </summary>
        public virtual void RemoveGuide(string guideKey)
        {
            var id = ConvertGuideKeyToID(guideKey);
            if (_guideGroups.Contains(id)) return;
            _guideGroups.Remove(id);
        }

        /// <summary>
        /// 开始新的引导
        /// </summary>
        /// <param name="guideId">引导组id</param>
        protected virtual void StartNewGuide(int guideId, List<GuideStep> steps, bool isShowTip = false)
        {
            if (!OpenGuide) return;
            CurrentGuideId = guideId;
            if (!AddGuideCount(CurrentGuideId)) return;
            CurrentGuideSteps = steps;
            OnGuideBegan(isShowTip);
        }

        protected virtual void ActionStep()
        {
            if (CurrentStepId >= CurrentGuideSteps.Count)
            {
                OnGuideEnd();
                return;
            }

            InitExecutor();
        }

        protected abstract void InitExecutor();

        public virtual void OnTouchMaskEvent(GuideTouchMaskEnum result,EventTriggerType mode,BaseEventData data,Transform target)
        {
            GuideStepExecutor?.OnTouchMaskEvent(result, mode, data,target);
        }

        /// <summary>
        /// 保存引导
        /// </summary>
        public abstract void SaveGuide(int guideId = 0);
        public abstract bool AddGuideCount(int guideId);

        public virtual void InitMask(HospitalGuideStepData data)
        {
            if (!data.step.hasMask)
            {
                GuideMaskComponent.Hide();
                return;
            }

            GuideMaskComponent.ChangeTarget(data.uiCenterPos, data.step.radius.ToList(), data.step.holeType.ToList(), data.step.holeOffset.ToList(), data.step.isTransparentMask, data.step.isNeedTrace.ToList());
            // GuideMaskComponent.SetCircleVisible(data.step.HasCircle);
        }

        public virtual void InitArrow(HospitalGuideStepData data)
        {
            GuideStep step = data.step;
            if (step.arrowType == null)
            {
                HideArrow();
                return;
            }
            if (step.isDrag)
            {
                var arrow = FindCanUseItem<GuideArrow>(false);
                arrow.PointToTarget(data.uiCenterPos[0], data.uiCenterPos[1]);
                return;
            }

            var pos1 = step.fingerPos;
            for (int i = 0; i < data.uiCenterPos.Count; i++)
            {
                if (step.arrowType.Length <= i || step.arrowType[i] == 0) continue;
                var arrow = FindCanUseItem<GuideArrow>(false);
                var pos = pos1 == null || pos1.Length <= i ? "0|0" : pos1[i];
                var str = pos.Split('|');
                int.TryParse(str[0], out int x);
                int.TryParse(str[1], out int y);
                if (step.arrowParam != null && step.arrowParam.Length != 0)
                {
                    arrow.PointTo(data.uiCenterPos[i], new Vector3(x, y), (GuideArrowType) step.arrowType[i], step.arrowParam[i]);
                }
                else
                {
                    arrow.PointTo(data.uiCenterPos[i], new Vector3(x, y), (GuideArrowType) step.arrowType[i]);
                }
            }
        }

        /// <summary>
        /// 是否有遮罩
        /// </summary>
        /// <returns></returns>
        public virtual bool HasMask()
        {
            return GuideMaskComponent.gameObject.activeSelf;
        }

        /// <summary>
        /// 获取引导状态
        /// </summary>
        /// <param name="guideGroupId"></param>
        /// <returns></returns>
        public abstract bool GetGuideState(int guideGroupId);

        /// <summary>
        /// 检测事件是否满足
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="param"></param>
        public abstract bool CheckEventConditionState(List<int> eventType, List<string> param = null);

        /// <summary>
        /// 清理指定引导
        /// </summary>
        /// <param name="id">为0清空所有</param>
        public abstract void ClearGameGuide(int id = 0);
        
        public void CompleteStep(bool immediate)
        {
            if (GuideStepExecutor == null) return;
            GuideStepExecutor.IsStepFinished = true;
            if (immediate)
                Update();
        }
        
        public void CompleteStep()
        {
            CompleteStep(false);
        }
    }
}