
namespace Screw
{
    public class PhillipsHandler : MemberHandler
    {
        public PhillipsHandler(ScrewGameContext inContext) : base(inContext)
        {
        }

        public override bool IsScrewDownValid(ScrewModel model)
        {
            if (context.gameState != ScrewGameState.InProgress)
            {
                return false;
            }
            if (!context.blockersHandler.IsScrewDownValid(model))
            {
                return false;
            }

            if (context.GetScrewView(model).IsOverlapping())
            {
                return false;
            }

            return true;
        }

        public override bool IsScrewUpValid(ScrewModel model)
        {
            return true;
        }

        public override void HandleScrewDown(ScrewModel model, ScrewView view)
        {
            var result = context.blockersHandler.HandleScrewDown(model, view);

            if (result == false)
            {
                context.actionController.AddMoveAction(model, view).Forget();
            }
        }

        public override void HandleScrewUp(ScrewModel model, ScrewView view)
        {
            context.blockersHandler.HandleScrewUp(model, view);
        }
        
        public override void OnPreMove(MoveAction moveAction)
        {
            context.blockersHandler.OnPreMove(moveAction);
        }

        public override void OnPrePairMove(PairMoveAction moveAction)
        {
            context.blockersHandler.OnPrePairMove(moveAction);
        }

        public override void OnPostMove(MoveAction moveAction)
        {
            context.blockersHandler.OnPostMove(moveAction);
        }

        public override void OnPostPairMove(PairMoveAction moveAction)
        {
            context.blockersHandler.OnPostPairMove(moveAction);
        }

        public override void CheckLevelFailed()
        {
            context.blockersHandler.CheckLevelFailed();
        }
    }
}