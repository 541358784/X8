using System;
using System.Collections;
using System.Collections.Generic;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.OutsideGuide;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OutsideGuide
{
    public abstract class GuideStepExcutor
    {
        protected readonly GuideStep StepInfo;
        public GuideStepExcutor(DecoGuideManager mgr,GuideStep step)
        {
            StepInfo = step;
            GuideMgr = mgr;
        }
        protected float StartTimestamp;
        protected List<GuideStepData> datas;
        public virtual bool IsStepFinished { get; set; }

        protected DecoGuideManager GuideMgr { get; set; }
        public abstract bool IsFinished();

        /// <summary>
        /// 在每一步开始之前，处理好遮挡、对白、箭头、增加AI之类的动作
        /// </summary>
        public virtual void OnStepBegan()
        {
            if (StepInfo == null) return;//容错
            SendBiEventOnBegin();
            StartTimestamp = Time.time;
            GuideMgr.HideArrow();
            GuideMgr.GuideMaskComponent.Hide();
            if (!IsGuideCanContinue()) return;

            string voice = StepInfo.voice;
            if (!string.IsNullOrEmpty(voice))
            {
                AudioManager.Instance.PlaySound(voice);
            }
            
            ExecutorAction();
            
            //获得中心坐标后
            HospitalGuideStepData sd = new HospitalGuideStepData();
            sd.step = StepInfo;
            datas = FindTarget(StepInfo.targetType.ToList(),StepInfo.targetParam.ToList());
            sd.uiCenterPos = new List<Transform>();
            for (int i = 0; i < datas.Count; i++)
            {
                datas[i].targetButtonAction?.Invoke();
                Transform trans = datas[i].target;
                if (trans == null)
                {
                    trans = datas[i].eventTarget?.Invoke();
                    if (trans == null) continue;
                }

                if (StepInfo.holeType != null && i < StepInfo.holeType.Length && (GuideMaskType) StepInfo.holeType[i] == GuideMaskType.ChangeOrder)
                {
                    trans.gameObject.GetOrCreateComponent<GuideTarget>();
                }

                sd.uiCenterPos.Add(trans);
            }

            GuideMgr.StartCoroutine(ExecutorGuide(sd));
        }

        protected virtual IEnumerator ExecutorGuide(HospitalGuideStepData sd)
        {
            yield return new WaitForEndOfFrame();
            GuideMgr.InitMask(sd);
            GuideMgr.InitArrow(sd);
            // 是否有对白框
            
            if (!string.IsNullOrEmpty(StepInfo.dialogKey))
            {
                float x = 0;
                float y = 0;
                if (null != StepInfo.dialogPos)
                {
                    List<int> pos = StepInfo.dialogPos.ToList();
                    if (pos.Count > 0) x = pos[0];
                    if (pos.Count > 1) y = pos[1];
                    if (pos.Count != 2)
                    {
                        DebugUtil.LogError("没有配置对话坐标");
                    }
                }
                var offset = StepInfo.characteroffset == null ? new List<int>() {0, 0} : StepInfo.characteroffset.ToList();
                GuideMgr.GuideNpcSayComponent.NpcSay(StepInfo.dialogKey, new Vector2(x, y), StepInfo.character,(DialogType) StepInfo.dialogType, (NpcPos) StepInfo.characterType,new Vector2(offset[0],offset[1]));
            }
            else
            {
                GuideMgr.GuideNpcSayComponent.Hide();
            }
        }
        /// <summary>
        /// 是否满足引导继续进行
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsGuideCanContinue()
        {
            return true;
        }

        protected abstract void ExecutorAction();

        public virtual void OnStepEnded()
        {
            GuideMgr.GuideMaskComponent?.Hide();
            GuideMgr.HideArrow();
            GuideMgr.GuideNpcSayComponent?.Hide();
            GuideMgr.GuideSkipComponent?.Hide();
            GuideMgr.GuideTipComponent?.Hide();
            SendBiEventOnEnd();
        }
        protected abstract void SendBiEventOnBegin();
        protected abstract void SendBiEventOnEnd();

        public abstract void OnTouchMaskEvent(GuideTouchMaskEnum result, EventTriggerType mode, BaseEventData data,Transform target);
        public abstract void OnGameEvent(string eType, int paramId);

        /// <summary>
        /// 是否过了不能点击的时间
        /// </summary>
        /// <param name="finishEvent"></param>
        /// <returns></returns>
        protected virtual bool IfPassDuration(string finishEvent,float duration)
        {
            if (duration <= 0) return true;
            if (!finishEvent.Equals("TouchAny") && !finishEvent.Equals("TouchHole")) return true;
            float passedTime = Time.time - StartTimestamp;
            return passedTime >= duration;
        }
        /// <summary>
        /// 获取事件执行状态
        /// </summary>
        /// <param name="eEvent"></param>
        /// <param name="paramId"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        protected virtual bool GetEventState(int finishType, string finishParam, int paramId)
        {
            bool isTrigger = false;
            if (finishType == 0) return true;
            if (string.IsNullOrEmpty(finishParam)) return isTrigger;
            if (!int.TryParse(finishParam, out int p)) return isTrigger;
            if (paramId != p) return isTrigger;
            isTrigger = true;
            return isTrigger;
        }

        public abstract List<GuideStepData> FindTarget(List<int> type, List<string> param);
    }

    /// <summary>
    /// 引导数据
    /// </summary>
    public class GuideStepData
    {
        public Transform target;
        public System.Action targetButtonAction; 
        public Func<Transform> eventTarget;
        public System.Action eventAction;
        public Action clickAnyWhereAction;
    }

    public class HospitalGuideStepData
    {
        public GuideStep step;
        public List<Transform> uiCenterPos;
    }
}