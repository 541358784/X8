using System;
using System.Collections;
using System.Collections.Generic;
using Deco.World;
using DragonPlus.Config.OutsideGuide;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
// using Hospital;
using TMatch;
namespace OutsideGuide
{
    /// <summary>
    /// 装修引导管理器
    /// </summary>
    public partial class DecoGuideManager : GuideBase<DecoGuideManager>
    {
        public enum Type
        {
            Default,
            Merge,
        }

        private Dictionary<int, StorageDecoGuide> data = StorageManager.Instance.GetStorage<StorageDecorationGuide>()
            .GuideData;

        private Dictionary<int, List<GuideStepData>> uidata = new Dictionary<int, List<GuideStepData>>();
        private bool isInit;

        public const string FristDecoGuide = "FristDecoGuide";
        public const string DecoNextGuide = "DecoNextGuide";
        public const string SecondLevelEnterGuide = "SecondLevelEnterGuide";
        public const string ThirdLevelEnterGuide = "ThirdLevelEnterGuide";
        public const string FourthLevelEnterGuide = "FourthLevelEnterGuide";
        public const string NewMemoryGuide = "NewMemoryGuide";
        public const string SmileGuide = "SmileGuide";
        public const string ParientsGuide = "ParientsGuide";
        public const string MemoryRewardGuide = "MemoryRewardGuide";
        public const string ChangeDecoGuide = "ChangeDecoGuide";
        
        public const string AsmrFirstGuide = "AsmrFirstGuide";
        public const string AsmrNextGuide = "AsmrNextGuide";

        public override void Init()
        {
            if (isInit) return;
            isInit = true;
            base.Init();
            uidata.Clear();
            // TaskSystem.Model.Instance.ChangeTaskNodeEvent(this);
        }

        public override void Release()
        {
            if (!isInit) return;
            isInit = false;
            base.Release();
            UnRegisterUIGuide();
        }

        private void Save(int id, bool isFinish)
        {
            if (!data.ContainsKey(id))
            {
                data.Add(id, new StorageDecoGuide() {Id = id, IsFinish = isFinish});
                return;
            }

            data[id].IsFinish = true;
        }

        protected override void InitExecutor()
        {
            if (!OpenGuide) return;
            GuideStepExecutor = new DecoGuideStepExecutor(this, CurrentGuideSteps[CurrentStepId]);
            GuideStepExecutor.OnStepBegan();
        }

        public void HideGuide()
        {
            Reset();
        }

        protected override void OnGuideBegan(bool isShowTip = false)
        {
            switch (CurrentGuideId)
            {
                // case 10001:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideOpeninginstructionsShow);
                //     break;
                // case 10004:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryEntranceShow);
                //     break;
                // case 10005:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideSmileyShow);
                //     break;
                // case 10009:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideMemoryrewardsEntranceShow);
                //     break;
                // case 10010:
                //     DragonPlus.GameBIManager.SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFteGuideNewareaUsestarShow);
                //     break;
            }
            
            base.OnGuideBegan(isShowTip);
        }

        // protected override void OnGuideEnd()
        // {
        //     bool isStopCheck = CurrentGuideId == 10002 || CurrentGuideId == 10004 || CurrentGuideId == 10008 || CurrentGuideId == 10009;
        //     
        //     base.OnGuideEnd();
        //     
        //     if (isStopCheck) return;
        //     WorldCheckModel.Instance.ContinueCheck();
        // }

        public override void SaveGuide(int guideId = 0)
        {
            int id = guideId == 0 ? CurrentGuideId : guideId;
            Save(id, true);
        }

        /// <summary>
        /// 增加引导出现次数
        /// </summary>
        public override bool AddGuideCount(int guideId)
        {
#if UNITY_EDITOR
            return true;
#endif
            if (!data.ContainsKey(guideId))
            {
                StorageDecoGuide guide = new StorageDecoGuide();
                guide.Id = guideId;
                data.Add(guideId, guide);
            }

            data[guideId].GuideShowCount++;
            if (data[guideId].GuideShowCount >= 3)
            {
                SaveGuide();
                return false;
            }
            return true;
        }

        public override bool GetGuideState(int guideGroupId)
        {
            return data.ContainsKey(guideGroupId) && data[guideGroupId].IsFinish;
        }

        public override bool CheckEventConditionState(List<int> eventType, List<string> param = null)
        {
            return true;
        }

        public override void ClearGameGuide(int id = 0)
        {
            StorageDecorationGuide storage = StorageManager.Instance.GetStorage<StorageDecorationGuide>();
            if (!storage.GuideData.ContainsKey(id)) return;
            storage.GuideData.Remove(id);
        }

        public void StartGuide(int id)
        {
            GuideGroup group = OutsideGuideConfigManager.Instance.GuideGroupList.Find(p => p.id == id);
            if (group == null) return;
            if (group.triggerType == 0)
            {
                int.TryParse(group.triggerValue1, out int levelId);
                if (levelId > ClientMgr.Instance.MainMaxLevel) return;//关卡不满足条件
            }
            List<GuideStep> steps = new List<GuideStep>();
            foreach (var step in OutsideGuideConfigManager.Instance.GuideStepList)
            {
                if (step.groupId == id)
                {
                    steps.Add(step);
                }
            }

            StartNewGuide(id, steps);
        }

        // public IEnumerator StartDecoGuide()
        // {
        //     if (!GetGuideState(10001) && !DecoWorld.NodeLib[101115].IsOwned && ItemModel.Instance.GetNum((int) ResourceId.Star) > 0)
        //     {
        //         StartGuide(10001);
        //         yield break;
        //     }
        //
        //     // WorldCheckModel.Instance.ContinueCheck();
        // }

        // public IEnumerator StartDecoNextGuide()
        // {
        //     //v1.0.38版本，去除装修入口按钮引导（新增了个人排行榜入口）
        //     
        //     if (!GetGuideState(10109) && !DecoWorld.NodeLib[101118].IsOwned && ItemModel.Instance.GetNum(501) == 0)
        //     {
        //         // StartGuide(10002);
        //         StartGuide(10109);
        //         yield break;
        //     }
        //
        //     // WorldCheckModel.Instance.ContinueCheck();
        // }

        // public IEnumerator StartDecoLastGuide()
        // {
        //     if (!GetGuideState(10004) && Memory.Model.Instance.HaveNew())
        //     {
        //         StartGuide(10004);
        //         yield break;
        //     }
        //     
        //     if (!GetGuideState(10005) && DecoWorld.NodeLib[101146].IsOwned)
        //     {
        //         StartGuide(10005);
        //         yield break;
        //     }
        //     
        //     // if (!GetGuideState(10008) && Patients.Model.Instance.HaveRedPoint())
        //     // {
        //     //     StartGuide(10008);
        //     //     yield break;
        //     // }
        //     
        //     if (!GetGuideState(10009) && Memory.Model.Instance.HaveRedPoint())
        //     {
        //         StartGuide(10009);
        //         yield break;
        //     }
        //
        //     if (!GetGuideState(10011) && DecoWorld.NodeLib[101138].IsOwned)
        //     {
        //         StartGuide(10011);
        //         yield break;
        //     }
        //     // 开启装修进度显示
        //     yield return MyMain.myGame.DecorationMgr.TryShowUnlockLevelTip();
        // }
        
        
        public void RegisterUIGuide(int guideStepId, List<GuideStepData> data,bool isClear = false)
        {
            if (!uidata.ContainsKey(guideStepId)) uidata.Add(guideStepId, new List<GuideStepData>());
            if(isClear) uidata[guideStepId].Clear();
            uidata[guideStepId].AddRange(data);
        }
        public void UnRegisterUIGuide(int guideStepId)
        {
            if (uidata.ContainsKey(guideStepId)) uidata.Remove(guideStepId);
        }

        public void UnRegisterUIGuide()
        {
            uidata.Clear();
        }

        public List<GuideStepData> FindUITarget(int guideStepId)
        {
            uidata.TryGetValue(guideStepId, out var steps);
            if (steps == null) steps = new List<GuideStepData>();
            return steps;
        }

        public GuideGroup GetGuideGroup(int id)
        {
            return OutsideGuideConfigManager.Instance.GuideGroupList.Find(item => item.id == id);
        }

        public GuideGroup GetCurrentGuideGroup()
        {
            return GetGuideGroup(CurrentGuideId);
        }
        
        public GuideStep GetGuideStep(int id)
        {
            return OutsideGuideConfigManager.Instance.GuideStepList.Find(item => item.stepId == id);
        }

        public GuideStep GetCurrentStep()
        {
            return CurrentGuideSteps[CurrentStepId];
        }
        public Dictionary<int, StorageDecoGuide> GetDataDict()
        {
            return data;
        }
    }
}

