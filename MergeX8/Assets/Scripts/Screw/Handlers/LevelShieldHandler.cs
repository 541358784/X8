namespace Screw
{
    public class LevelShieldHandler : ILogicEventHandler
    {      
        private ScrewGameContext _context;

        public LevelShieldHandler(ScrewGameContext context)
        {
            _context = context;
        }
        
        public int GetExecuteOrder()
        {
            return ExecuteOrder.ShieldHandler;
        }

        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams)
        {
            switch (logicEvent)
            {
                case LogicEvent.RefreshShield:
                {
                    _context.RefreshShield();
                    break;
                }
            }
        }
    }
}