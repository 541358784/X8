using System.Collections.Generic;

namespace Screw
{
    public class BombBlockerHandler : BlockerHandler
    {
        public BombBlockerHandler(ScrewGameContext inContext) : base(inContext)
        {
        }
        
        public override bool IsScrewDownValid(ScrewModel screwModel)
        {
            return true;
        }
        
        public override bool CompleteBlockerInRevive(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.BombBlocker, out BaseBlockerModel model))
            {
                if (model is BombBlockerModel bombBlockerModel)
                {
                    if (!bombBlockerModel.IsComplete())
                    {
                        bombBlockerModel.Complete();
                        var bomBlockView = context.GetBlockerView<BombBlockerView>(ScrewBlocker.BombBlocker, screwModel);
                        if (bomBlockView != null)
                            bomBlockView.Destroy();
                        return true;
                    }
                }
            }

            return false;
        }
 
        public override bool OnPrePairMove(PairMoveAction moveAction)
        {
            ChangeStageCount(moveAction.Models);
            return false;
        }

        public override bool OnPreMove(MoveAction moveAction)
        {
            ChangeStageCount(new List<ScrewModel>() {moveAction.Model});
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
            if (context.gameState == ScrewGameState.Fail)
                return;

            var list = new List<BombBlockerModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.BombBlocker))
                    {
                        list.Add(screwModel.ScrewBlockers[ScrewBlocker.BombBlocker] as BombBlockerModel);
                    }
                }
            }

            if (list.Count <= 0)
                return;

            foreach (var bombBlockerModel in list)
            {
                if (!bombBlockerModel.IsComplete() && bombBlockerModel.StageCount == 0)
                {
                    context.failReason = LevelFailReason.BombFailed;
                    context.gameState = ScrewGameState.Fail;
                    context.AddFailBlocker(LevelFailReason.BombFailed, bombBlockerModel.ScrewModel);
                    break;
                }
            }
        }

        private void ChangeStageCount(List<ScrewModel> models)
        {
            var list = new List<BombBlockerModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.BombBlocker))
                    {
                        list.Add(screwModel.ScrewBlockers[ScrewBlocker.BombBlocker] as BombBlockerModel);
                    }
                }
            }

            // TODO 移除并设置Completed
            foreach (var screwModel in models)
            {
                if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.BombBlocker))
                {
                    var bombBlockerModel = screwModel.ScrewBlockers[ScrewBlocker.BombBlocker] as BombBlockerModel;
                    bombBlockerModel?.Complete();
                    var bomBlockView = context.GetBlockerView<BombBlockerView>(ScrewBlocker.BombBlocker, screwModel);
                    if (bomBlockView != null)
                        bomBlockView.Destroy();
                }
            }

            if (list.Count <= 0)
                return;

            var hasAdd = false;
            var hasBomb = false;
            foreach (var bombBlockerModel in list)
            {
                if (!bombBlockerModel.IsComplete() && !context.GetScrewView(bombBlockerModel.ScrewId).IsOverlapping())
                {
                    bombBlockerModel.StageCounter();

                    var bomBlockView = context.GetBlockerView<BombBlockerView>(ScrewBlocker.BombBlocker, bombBlockerModel.ScrewModel);
                    if (bomBlockView != null)
                    {
                        bool bomb = bomBlockView.ChangeStageCounter(bombBlockerModel);
                        if (bomb)
                            hasBomb = true;
                    }
                    
                    if (bombBlockerModel.StageCount <= 0)
                    {
                        // 这里设置为失败，是为了不让玩家继续点击螺丝
                        context.gameState = ScrewGameState.Fail;
                        context.failReason = LevelFailReason.BombFailed;
                        if (!hasAdd)
                        {
                            context.AddFailBlocker(LevelFailReason.BombFailed, bombBlockerModel.ScrewModel);
                            hasAdd = true;
                        }
                    }
                }
            }

            if (hasBomb)
            {
                SoundModule.PlaySfx("sfx_obstacle_bomb");
            }
        }
    }
}