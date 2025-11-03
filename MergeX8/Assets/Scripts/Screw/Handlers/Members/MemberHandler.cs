
namespace Screw
{
    public abstract class MemberHandler
    {
        protected ScrewGameContext context;

        protected MemberHandler(ScrewGameContext inContext)
        {
            context = inContext;
        }
        
        public virtual void HandleScrewDown(ScrewModel model, ScrewView view)
        {
        }
        public virtual void HandleScrewUp(ScrewModel model, ScrewView view)
        {
        }

        public abstract bool IsScrewDownValid(ScrewModel model);
        public abstract bool IsScrewUpValid(ScrewModel model);
        
        public virtual void OnPreMove(MoveAction moveAction)
        {
        }

        public virtual void OnPostMove(MoveAction moveAction)
        {
        }

        public virtual void OnPrePairMove(PairMoveAction moveAction)
        {
        }

        public virtual void OnPostPairMove(PairMoveAction moveAction)
        {
        }
        public virtual void CheckLevelFailed()
        {
        }
    }
}