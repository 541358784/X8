using Cysharp.Threading.Tasks;

namespace Screw
{
    public class MoveAction : GameAction
    {
        public ScrewModel Model;
        public ScrewView View;

        public MoveAction(ScrewGameContext inContext) : base(inContext)
        {
        }

        public override async UniTask<bool> ExecuteAction()
        {
            var collectTarget = context.StorageScrew(Model);
            
            if(collectTarget == null)
                return false;

            context.hookContext.OnLogicEvent(LogicEvent.PreMove, new MoveParams(this));
            
            // TODO 对View操作
            await View.DoMove(collectTarget);
            //
            // SoundModule.PlaySfx("tile_tap");
            //
            // await context.collectAreaController.MoveMember(CellModel, context.collectableAreaModel.GetModelIndex(CellModel));
            //
            // if (collectResult.blastInfos.Count > 0)
            // {
            //     await context.collectAreaController.PerformBlastAnimation(collectResult.blastInfos, context.collectableAreaModel);
            // }
            //
            context.hookContext.OnLogicEvent(LogicEvent.PostMove, new MoveParams(this));
            return true;
        }
    }
}