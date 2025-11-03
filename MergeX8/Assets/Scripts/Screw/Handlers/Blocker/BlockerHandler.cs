
namespace Screw
{
    public abstract class BlockerHandler
    {
        protected ScrewGameContext context;

        public BlockerHandler(ScrewGameContext inContext)
        {
            context = inContext;
        }

        public virtual bool HandleScrewDown(ScrewModel screwModel, ScrewView view)
        {
            return false;
        }
        public virtual bool HandleScrewUp(ScrewModel screwModel, ScrewView view)
        {
            return false;
        }

        public virtual bool IsScrewDownValid(ScrewModel screwModel)
        {
            return true;
        }
        
        public virtual bool OnPreMove(MoveAction moveAction)
        {
            return false;
        }

        public virtual bool OnPostMove(MoveAction moveAction)
        {
            return false;
        }

        public virtual bool OnPrePairMove(PairMoveAction moveAction)
        {
            return false;
        }

        public virtual bool OnPostPairMove(PairMoveAction moveAction)
        {
            return false;
        }
        
        public virtual void CheckLevelFailed()
        {

        }
        public abstract bool CompleteBlockerInRevive(ScrewModel cellModel);
    }
}