using System.Collections.Generic;
namespace Screw
{
    public class LockBlockerHandler : BlockerHandler
    {
        public LockBlockerHandler(ScrewGameContext inContext) : base(inContext)
        {
        }
        
        public override bool IsScrewDownValid(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.LockBlocker, out BaseBlockerModel model))
            {
                if (model is LockBlockerModel lockBlockerModel)
                {
                    // 检测是否可以点击
                    if (lockBlockerModel.IsComplete())
                        return true;

                    return false;
                }
            }

            return true;
        }
        
        public override bool CompleteBlockerInRevive(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.LockBlocker, out BaseBlockerModel model))
            {
                if (model is LockBlockerModel lockBlockerModel)
                {
                    if (!lockBlockerModel.IsComplete())
                    {
                        lockBlockerModel.Complete();
                        return true;
                    }
                }
            }

            return false;
        }
 
        public override bool OnPrePairMove(PairMoveAction moveAction)
        {
            CheckKeyJam(moveAction.Models);
            return false;
        }

        public override bool OnPreMove(MoveAction moveAction)
        {
            CheckKeyJam(new List<ScrewModel>(){moveAction.Model});
            return false;
        }

        public override bool OnPostMove(MoveAction moveAction)
        {
            // CheckLevelFailed();
            return false;
        }

        public override bool OnPostPairMove(PairMoveAction moveAction)
        {
            // CheckLevelFailed();
            return false;
        }

        /// <summary>
        /// 锁 不可能输，除非关卡配置错误
        /// </summary>
        public override void CheckLevelFailed()
        {
            if (context.gameState == ScrewGameState.Fail)
                return;
            var lockBlockerModels = new List<LockBlockerModel>();
            var list = new List<ScrewModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    list.Add(screwModel);
                    
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.LockBlocker))
                    {
                        lockBlockerModels.Add(screwModel.ScrewBlockers[ScrewBlocker.LockBlocker] as LockBlockerModel);
                    }
                }
            }

            if (lockBlockerModels.Count <= 0)
                return;

            var count = 0;
            foreach (var screwModel in list)
            {
                var screwView = context.GetScrewView(screwModel.ScrewId);
                if (!screwView.IsClicked() && !screwView.IsOverlapping())
                {
                    count++;
                }
            }

            if (count > 0)
                return;

            foreach (var blockerModel in lockBlockerModels)
            {
                var keyJamView = context.GetScrewView(blockerModel.KeyScrewId);

                if (!blockerModel.IsComplete() && blockerModel.IsBlocking() && !keyJamView.IsClicked() && keyJamView.IsOverlapping())
                {
                    context.failReason = LevelFailReason.LockFailed;
                    context.gameState = ScrewGameState.Fail;
                    break;
                }
            }
        }

        private void CheckKeyJam(List<ScrewModel> screwModels)
        {
            var list = new List<LockBlockerModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.LockBlocker))
                    {
                        list.Add(screwModel.ScrewBlockers[ScrewBlocker.LockBlocker] as LockBlockerModel);
                    }
                }
            }

            if (list.Count <= 0)
                return;

            foreach (var lockBlockerModel in list)
            {
                if (!lockBlockerModel.IsComplete() && CheckHasKey(screwModels, lockBlockerModel.KeyScrewId))
                {
                    lockBlockerModel.Complete();

                    var lockBlockerView = context.GetBlockerView<LockBlockerView>(ScrewBlocker.LockBlocker, lockBlockerModel.ScrewModel);
                    
                    if (lockBlockerView != null)
                        lockBlockerView.UnLock();
                }
            }
        }

        private bool CheckHasKey(List<ScrewModel> screwModels, int keyScrewId)
        {
            foreach (var screwModel in screwModels)
            {
                if (screwModel.ScrewId == keyScrewId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}