using System;
using System.Collections.Generic;
// using Collect;
using Deco.World;
using DragonPlus.Config.OutsideGuide;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using TMatch;
namespace OutsideGuide
{
    public enum DecoGuideActionType
    {
        None,
        OpenUI,
        CloseUI,
        MoveToNode,
        SetMainMask,
    }

    public enum DecoGuideTargetType
    {
        None,
        Unlock,
        Change,
        Main,
        ItemBar,
        Collection,
        UI,
    }
    
    public class DecoGuideStepExecutor : GuideStepExcutor
    {
        private float touchTiming;
        
        // private void OpenUI(string param)
        // {
        //     switch (param)
        //     {
        //         case "UIDecoStar":
        //         {
        //             UIDecoStar.Open();
        //             break;
        //         }
        //         case "UIDecoUnlock":
        //         {
        //             UIDecoUnlock.Open(MyMain.myGame.DecorationMgr.CurrentWorld.GetSuggestNode());
        //             UIDecoUnlock.Get().HideRole();
        //             break;
        //         }
        //         case "UIMain":
        //         {
        //             UIMain.Open();
        //             break;
        //         }
        //     }
        // }
        //
        // private void CloseUI(string param)
        // {
        //     switch (param)
        //     {
        //         case "UIDecoStar":
        //         {
        //             UIDecoStar.Close();
        //             break;
        //         }
        //         case "UIMain":
        //         {
        //             UIMain.Close();
        //             break;
        //         }
        //     }
        // }
        
        private void OnActionExecute(int type, string param = null)
        {
            return;
            // switch ((DecoGuideActionType)type)
            // {
            //     case DecoGuideActionType.None:
            //         break;
            //     case DecoGuideActionType.OpenUI:
            //         OpenUI(param);
            //         break;
            //     case DecoGuideActionType.CloseUI:
            //         CloseUI(param);
            //         break;
            //     case DecoGuideActionType.MoveToNode:
            //         MyMain.myGame.DecorationMgr.CurrentWorld.FocusNode(int.Parse(param));
            //         break;
            //     case DecoGuideActionType.SetMainMask:
            //         UIMain.Get()?.SetMaskActive(param == "true");
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(type), type, null);
            // }
        }

        private GuideStepData OnGenGuideStepData(int type, string param = null)
        {
            GuideStepData data = new GuideStepData();
            switch ((DecoGuideTargetType)type)
            {
                case DecoGuideTargetType.None:
                    break;
                // case DecoGuideTargetType.Unlock:
                // {
                //     var ui = UIDecoUnlock.Get();
                //     switch (param)
                //     {
                //         case "Unlock":
                //         {
                //             data.target = ui.GuideUnlock;
                //             data.targetButtonAction = () =>
                //             {
                //                 GuideButton.Create(ui.GuideBuy.gameObject);
                //             };
                //             break;
                //         }
                //         case "Check":
                //         {
                //             data.target = ui.GuideClose;
                //             data.eventAction = ui.OnGuideClose;
                //             break;
                //         }
                //         case "Buy":
                //         {
                //             data.target = ui.GuideBuy;
                //             data.eventAction = ui.OnGuideUnlock;
                //             break;
                //         }
                //     }
                // }
                //     break;
                // case DecoGuideTargetType.Change:
                // {
                //     var ui = UIDecoChange.Get();
                //     switch (param)
                //     {
                //         case "Center":
                //             data.target = ui.GuideCenter;
                //             break;
                //         case "UnlockGroup":
                //             data.target = ui.UnlockGroup;
                //             break;
                //         case "Confirm":
                //             data.target = ui.transform.Find("Root/Right/btnUnlock");
                //             // data.eventAction = ui.OnGuideEvent;
                //             data.targetButtonAction = () =>
                //             {
                //                 GuideButton.Create(ui.transform.Find("Root/Right/btnUnlock").gameObject);
                //             };
                //             break;
                //     }
                // }
                //     break;
                // case DecoGuideTargetType.Main:
                // {
                //     var ui = UIMain.Get();
                //     switch (param)
                //     {
                //         // case "Unlock":
                //         //     data.target = ui.GuideUnlock;
                //         //     data.eventAction = () =>
                //         //     {
                //         //         UIDecoUnlock.Open(MyMain.myGame.DecorationMgr.CurrentWorld.GetSuggestNode());
                //         //         UIDecoUnlock.Get().HideRole();
                //         //     };
                //         //     break;
                //         case "Enter":
                //             data.target = ui.GuideStart;
                //             data.eventAction = ui.GuideEnterMap;
                //             break;
                //         // case "Smile":
                //         //     data.target = ui.GuideCenter;
                //         //     data.eventAction = SmileCollectModel.Instance.Collect;
                //         //     break;
                //         case "SmileCollect":
                //             data.target = ui.GuideSmileCollect;
                //             break;
                //         case "Collection":
                //             data.target = ui.GuideMem;
                //             data.eventAction = ui.GuideOpenMem;
                //             break;
                //         case "Map":
                //             data.target = ui.GuideMap;
                //             data.eventAction = ui.GuideOpenMap;
                //             break;
                //         case "Asmr":
                //             data.target = ui.GuideAsmr;
                //             data.eventAction = ui.GuideOpenAsmr;
                //             break;
                //     }
                // }
                //     break;
                // case DecoGuideTargetType.ItemBar:
                // {
                //     var ui = UIItemTop.Get();
                //     switch (param)
                //     {
                //         case "Right":
                //             data.target = ui.GuideBar;
                //             break;
                //         case "Icon":
                //             data.target = ui.GuideIcon;
                //             break;
                //     }
                // }
                //     break;
                // case DecoGuideTargetType.Collection:
                // {
                //     var ui = TMatch.UIManager.Instance.GetOpenedWindow<CollectionController>();
                //     switch (param)
                //     {
                //         case "Mem":
                //             data.target = ui.GuideMem;
                //             data.eventAction = ui.GuideClickMem;
                //             break;
                //         case "MemItem":
                //             data.target = ui.GuideMemItem;
                //             data.eventAction = Memory.Model.Instance.RecieveAllReward;
                //             break;
                //         case "MemNewItem":
                //             data.target = ui.GuideMemNewItem;
                //             break;
                //         case "Patient":
                //             data.target = ui.GuidePatient;
                //             data.eventAction = ui.GuideClickPat;
                //             break;
                //         case "PatItem":
                //             data.target = ui.GuidePatItem;
                //             break;
                //         case "PatReward":
                //             data.target = ui.GuidePatReward;
                //             data.eventAction = ui.GuideClickPatReward;
                //             break;
                //     }
                // }
                //     break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return data;
        }
        
        public DecoGuideStepExecutor(DecoGuideManager mgr, GuideStep step) : base(mgr, step)
        {
        }

        public override bool IsFinished()
        {
            if (IsStepFinished)
            {
                switch (StepInfo.stepId)
                {
                    // case 1000103:
                    // case 1001001:
                    //     return !TMatch.UIManager.Instance.IsWindowOpened<UIDecoUnlock>();
                    // case 1000104:
                    //     return !TMatch.UIManager.Instance.IsWindowOpened<UIDecoChange>();
                    // case 1001102:
                    //     // 手动处理当前引导
                    //     UIDecoChange.Open(DecoWorld.NodeLib[101138]);
                    //     UIMain.Get()?.SetMaskActive(false);
                    //     return true;
                    default:
                        return true;
                }
            }

            return false;
        }

        protected override void ExecutorAction()
        {
            if (StepInfo?.actionType == null)
            {
                return;
            }
            
            for (int i = 0; i < StepInfo.actionType.Length; i++)
            {
                if (StepInfo.actionParm != null && StepInfo.actionParm.Length > i)
                {
                    OnActionExecute(StepInfo.actionType[i], StepInfo.actionParm[i]);
                }
                else
                {
                    OnActionExecute(StepInfo.actionType[i]);
                }
            }
        }

        protected override void SendBiEventOnBegin()
        {
            
        }

        protected override void SendBiEventOnEnd()
        {
            switch (StepInfo.stepId)
            {
                // case 1000101:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideOpeninginstructionsTap);
                //     break;
                // case 1000102:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideStarTap);
                //     break;
                // case 1000103:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideDecorationTap);
                //     break;
                // case 1000104:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideDecorationselectionConfirm);
                //     //bi:完成第一次装修
                //     DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFristDecoration);
                //     break;
                // case 1000201:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideDecoNostarEntranceTap);
                //     break;
                // case 1000202:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideDecoNostarTap);
                //     break;
                // case 1000203:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideDecoNostarExit);
                //     break;
                // case 1000204:
                // case 1010901:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuidePlayTap);
                //     break;
                // case 1000401:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryEntranceTap);
                //     break;
                // case 1000402:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryBookmarkTap);
                //     break;
                // case 1000403:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryContentTap);
                //     break;
                // case 1000501:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideSmileyTap);
                //     break;
                // case 1000502:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideSmileyBarTap);
                //     break;
                // case 1000801:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuidePatientcollectEntranceTap);
                //     break;
                // case 1000802:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuidePatientcollectBookmarkTap);
                //     break;
                // case 1000803:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuidePatientcollectInfoTap);
                //     break;
                // case 1000804:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuidePatientcollecttContentTap);
                //     break;
                // case 1000901:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryrewardsEntranceTap);
                //     break;
                // case 1000902:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryrewardsBookmarkTap);
                //     break;
                // case 1000903:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryrewardsContentTap);
                //     break;
                // case 1001001:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideNewareaUsestarTap);
                //     break;
                // case 1001002:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideNewareaEntranceTap);
                //     break;
                // case 1001003:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideNewareaContentTap);
                //     break;
                // case 1001101:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideSwitchdecorationsTap);
                //     break;
                // case 1001102:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideSwitchdecorationsPress);
                //     break;
            }
        }

        public override void OnTouchMaskEvent(GuideTouchMaskEnum result, EventTriggerType mode, BaseEventData data, Transform target)
        {
            if (Time.time - touchTiming < 0.1f)
            {
                return;
            }
            
            switch (StepInfo.finishType)
            {
                case 0:
                    datas?.ForEach(p=>p.clickAnyWhereAction?.Invoke());
                    IsStepFinished = true;
                    break;
                case 1:
                    if (result == GuideTouchMaskEnum.Within)
                    {
                        if (target == null) return;
                        if (datas == null) return;
                        var t = datas.Find(p => p.target == target || (p.eventTarget?.Invoke() == target));
                        if (t == null) return;
                        if ((t.target as RectTransform != null || t.eventTarget?.Invoke() as RectTransform != null) && mode != EventTriggerType.PointerClick) return;
                        t.eventAction?.Invoke();
                        IsStepFinished = true;
                    }
                    break;
            }
            touchTiming = Time.time;
        }

        public override void OnGameEvent(string eType, int paramId)
        {
            
        }

        public override List<GuideStepData> FindTarget(List<int> type, List<string> param)
        {
            List<GuideStepData> list = new List<GuideStepData>();
            if (type != null && type.Count > 0)
            {
                for (int i = 0; i < type.Count; i++)
                {
                    DecoGuideTargetType targetType = (DecoGuideTargetType) type[i];
                    if (targetType == DecoGuideTargetType.UI)
                    {
                        int.TryParse(param[i], out var stepId);
                        list.AddRange(GuideMgr.FindUITarget(stepId));
                        continue;
                    }
                    if (param != null && param.Count > i)
                    {
                        list.Add(OnGenGuideStepData(type[i], param[i]));
                    }
                    else
                    {
                        list.Add(OnGenGuideStepData(type[i]));
                    }
                }
            }
            return list;
        }
    }
}
