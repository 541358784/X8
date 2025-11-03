using System.Collections.Generic;
namespace Screw
{
    public class IceBlockerHandler : BlockerHandler
    {
        public IceBlockerHandler(ScrewGameContext inContext) : base(inContext)
        {
        }
        
        public override bool IsScrewDownValid(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.IceBlocker, out BaseBlockerModel model))
            {
                if (model is IceBlockerModel iceBlockerModel)
                {
                    // 检测是否可以点击
                    if (iceBlockerModel.IsComplete())
                        return true;

                    return false;
                }
            }

            return true;
        }
        
        public override bool CompleteBlockerInRevive(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.IceBlocker, out BaseBlockerModel model))
            {
                if (model is IceBlockerModel iceBlockerModel)
                {
                    if (!iceBlockerModel.IsComplete())
                    {
                        iceBlockerModel.Complete();
                        var iceBlockerView = context.GetBlockerView<IceBlockerView>(ScrewBlocker.IceBlocker, iceBlockerModel.ScrewModel);
                        if (iceBlockerView != null)
                            iceBlockerView.BreakIceStage(iceBlockerModel);
                        return true;
                    }
                }
            }

            return false;
        }
 
        public override bool OnPrePairMove(PairMoveAction moveAction)
        {
            ProcessIceBreak();
            return false;
        }

        public override bool OnPreMove(MoveAction moveAction)
        {
            ProcessIceBreak();
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
            var iceBlockerModels = new List<IceBlockerModel>();
            var list = new List<ScrewModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.IceBlocker))
                    {
                        var iceBlockerModel = screwModel.ScrewBlockers[ScrewBlocker.IceBlocker] as IceBlockerModel;
                        if (iceBlockerModel.IsComplete())
                            list.Add(screwModel);
                        else
                            iceBlockerModels.Add(iceBlockerModel);
                    }
                    else
                    {
                        list.Add(screwModel);
                    }
                }
            }

            if (iceBlockerModels.Count <= 0)
                return;

            var residueCount = 0;
            foreach (var screwModel in list)
            {
                var screwView = context.GetScrewView(screwModel.ScrewId);
                if (!screwView.IsClicked())
                {
                    residueCount++;
                }
            }

            foreach (var iceBlockerModel in iceBlockerModels)
            {
                if (!iceBlockerModel.IsComplete() && iceBlockerModel.IsBlocking() && residueCount == 0)
                {
                    context.failReason = LevelFailReason.IceFailed;
                    context.gameState = ScrewGameState.Fail;
                    context.AddFailBlocker(LevelFailReason.IceFailed, iceBlockerModel.ScrewModel);
                }
            }
        }

        private void ProcessIceBreak()
        {
            var list = new List<IceBlockerModel>();
            foreach (var layerModel in context.LevelModel.LayerModels.Values)
            {
                foreach (var screwModel in layerModel.ScrewModels.Values)
                {
                    if (screwModel.HasBlocker && screwModel.ScrewBlockers.ContainsKey(ScrewBlocker.IceBlocker))
                    {
                        list.Add(screwModel.ScrewBlockers[ScrewBlocker.IceBlocker] as IceBlockerModel);
                    }
                }
            }

            if (list.Count <= 0)
                return;

            bool hasBreak = false;
            foreach (var iceBlockerModel in list)
            {
                if (!iceBlockerModel.IsComplete() && !context.GetScrewView(iceBlockerModel.ScrewId).IsOverlapping())
                {
                    iceBlockerModel.NextStage();

                    hasBreak = true;
                    var iceBlockerView = context.GetBlockerView<IceBlockerView>(ScrewBlocker.IceBlocker, iceBlockerModel.ScrewModel);

                    if (iceBlockerView != null)
                        iceBlockerView.BreakIceStage(iceBlockerModel);
                }
            }

            if (hasBreak)
            {
                SoundModule.PlaySfx("sfx_obstacle_ice");
            }
        }
    }
}