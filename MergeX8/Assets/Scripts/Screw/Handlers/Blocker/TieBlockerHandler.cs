using System.Collections.Generic;

namespace Screw
{
    public class TieBlockerHandler : BlockerHandler
    {
        public TieBlockerHandler(ScrewGameContext inContext) : base(inContext)
        {
        }

        public override bool IsScrewDownValid(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.TieBlocker, out BaseBlockerModel model))
            {
                if (model is TieBlockerModel tieBlockerModel)
                {
                    if (tieBlockerModel.IsComplete())
                        return true;
                    
                    if (tieBlockerModel.IsBlocking())
                        return false;
                }
            }

            return true;
        }

        public override bool CompleteBlockerInRevive(ScrewModel model)
        {
            return true;
        }
        
        
        public override bool OnPrePairMove(PairMoveAction moveAction)
        {
            ChangeTieStatus(moveAction.Models);
            return false;
        }

        public override bool OnPreMove(MoveAction moveAction)
        {
            ChangeTieStatus(new List<ScrewModel>() {moveAction.Model});
            return false;
        }

        public override bool OnPostMove(MoveAction moveAction)
        {
            return false;
        }

        public override bool OnPostPairMove(PairMoveAction moveAction)
        {
            return false;
        }

        private void ChangeTieStatus(List<ScrewModel> models)
        {
            var list = new List<TieBlockerModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.TieBlocker))
                    {
                        list.Add(screwModel.ScrewBlockers[ScrewBlocker.TieBlocker] as TieBlockerModel);
                    }
                }
            }

            if (list.Count <= 0)
                return;

            foreach (var tieBlockerModel in list)
            {
                if (tieBlockerModel.IsComplete())
                    continue;
                foreach (var screwModel in models)
                {
                    if (tieBlockerModel.TieScrewIds.Contains(screwModel.ScrewId))
                    {
                        tieBlockerModel.RemoveTieId(screwModel.ScrewId);
                        
                        // 断开对应id链条
                        var lockBlockerView = context.GetBlockerView<TieBlockerView>(ScrewBlocker.TieBlocker,
                            tieBlockerModel.ScrewModel);
                        
                        if (lockBlockerView != null)
                            lockBlockerView.UnlockTie(screwModel);
                    }
                }
            }
        }
    }
}