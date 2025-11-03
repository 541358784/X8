using System.Collections.Generic;
namespace Screw
{
    public class ShutterBlockerHandler : BlockerHandler
    {
        public ShutterBlockerHandler(ScrewGameContext inContext) : base(inContext)
        {
        }
        
        public override bool IsScrewDownValid(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.ShutterBlocker, out BaseBlockerModel model))
            {
                if (model is ShutterBlockerModel shutterBlockerModel)
                {
                    // 检测是否可以点击
                    if (shutterBlockerModel.IsComplete())
                        return true;

                    if (!shutterBlockerModel.IsBlocking())
                        return true;

                    return false;
                }
            }

            return true;
        }
        
        public override bool CompleteBlockerInRevive(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.ShutterBlocker, out BaseBlockerModel model))
            {
                if (model is ShutterBlockerModel shutterBlockerModel)
                {
                    if (!shutterBlockerModel.IsComplete())
                    {
                        shutterBlockerModel.Complete();
                        var shutterBlockerView = context.GetBlockerView<ShutterBlockerView>(ScrewBlocker.ShutterBlocker, screwModel);
                        if (shutterBlockerView != null)
                            shutterBlockerView.Destroy();
                        return true;
                    }
                }
            }

            return false;
        }
 
        public override bool OnPrePairMove(PairMoveAction moveAction)
        {
            ChangeShutterStatus(moveAction.Models);
            return false;
        }

        public override bool OnPreMove(MoveAction moveAction)
        {
            ChangeShutterStatus(new List<ScrewModel>() {moveAction.Model});
            return false;
        }

        public override bool OnPostMove(MoveAction moveAction)
        {
            CheckLevelFailed();
            return false;
        }

        public override bool OnPostPairMove(PairMoveAction moveAction)
        {
            CheckLevelFailed();
          
            return false;
        }

        public override void CheckLevelFailed()
        {
            if (context.gameState == ScrewGameState.Fail || context.gameState == ScrewGameState.InUseBooster)
                return;
            var shutterBlockers = new List<ShutterBlockerModel>();
            var list = new List<ScrewModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    list.Add(screwModel);
                    
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.ShutterBlocker))
                    {
                        shutterBlockers.Add(screwModel.ScrewBlockers[ScrewBlocker.ShutterBlocker] as ShutterBlockerModel);
                    }
                }
            }

            if (shutterBlockers.Count <= 0)
                return;

            var residueCount = 0;
            foreach (var screwModel in list)
            {
                var screwView = context.GetScrewView(screwModel.ScrewId);
                if (!screwView.IsClicked() && context.membersHandler.IsScrewDownValid(screwModel))
                {
                    residueCount++;
                }
            }

            var hasPanelMoving = context.HasPanelMoving();
            foreach (var shutterBlockerModel in shutterBlockers)
            {
                if (!shutterBlockerModel.IsComplete() && shutterBlockerModel.IsBlocking() && residueCount == 0 && !hasPanelMoving)
                {
                    context.failReason = LevelFailReason.ShutterFailed;
                    context.gameState = ScrewGameState.Fail;
                    context.AddFailBlocker(LevelFailReason.ShutterFailed, shutterBlockerModel.ScrewModel);
                }
            }
        }

        private void ChangeShutterStatus(List<ScrewModel> models)
        {
            var list = new List<ShutterBlockerModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.ShutterBlocker) && !models.Contains(screwModel))
                    {
                        list.Add(screwModel.ScrewBlockers[ScrewBlocker.ShutterBlocker] as ShutterBlockerModel);
                    }
                }
            }

            // TODO 移除并设置Completed
            foreach (var screwModel in models)
            {
                if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.ShutterBlocker))
                {
                    var shutterBlockerModel = screwModel.ScrewBlockers[ScrewBlocker.ShutterBlocker] as ShutterBlockerModel;
                    shutterBlockerModel?.Complete();
                    var shutterBlockerView = context.GetBlockerView<ShutterBlockerView>(ScrewBlocker.ShutterBlocker, screwModel);
                    if (shutterBlockerView != null)
                        shutterBlockerView.Destroy();
                }
            }

            if (list.Count <= 0)
                return;

            bool hasShutter = false;
            foreach (var shutterBlockerModel in list)
            {
                if (!shutterBlockerModel.IsComplete() && !context.GetScrewView(shutterBlockerModel.ScrewId).IsOverlapping())
                {
                    shutterBlockerModel.ChangeStatus();
                    hasShutter = true;
                    var shutterBlockerView = context.GetBlockerView<ShutterBlockerView>(ScrewBlocker.ShutterBlocker, shutterBlockerModel.ScrewModel);
                    
                    if (shutterBlockerView != null)
                        shutterBlockerView.ChangeShutterStatus(shutterBlockerModel);
                }
            }

            if (hasShutter)
            {
                SoundModule.PlaySfx("sfx_obstacle_switch");
            }
        }
    }
}