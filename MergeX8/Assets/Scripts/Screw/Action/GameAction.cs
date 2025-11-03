using Cysharp.Threading.Tasks;

namespace Screw
{
    public class GameAction
    {
        protected ScrewGameContext context;

        protected GameAction(ScrewGameContext inContext)
        {
            context = inContext;
        }

        public virtual async UniTask<bool> ExecuteAction()
        {
            return false;
        }
    }
}