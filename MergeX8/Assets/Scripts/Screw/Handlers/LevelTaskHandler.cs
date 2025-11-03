using Cysharp.Threading.Tasks;

namespace Screw
{
    public class LevelTaskHandler : ILogicEventHandler
    {
        private ScrewGameContext _context;

        public LevelTaskHandler(ScrewGameContext context)
        {
            _context = context;
        }

        public int GetExecuteOrder()
        {
            return ExecuteOrder.LevelTaskHandler;
        }

        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams)
        {
            switch (logicEvent)
            {
                case LogicEvent.CheckTask:
                    _context.CheckTask().Forget();
                    break;
                case LogicEvent.RefreshTaskStatus:
                    _context.RefreshTaskStatus(((MoveEndParams) eventParams).ScrewModel);
                    break;
            }
        }
    }
}