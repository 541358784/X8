using System.Collections.Generic;

namespace Screw
{
    public class LogicEventHookContext
    {
        private List<ILogicEventHandler> _logicEventHandler;

        public LogicEventHookContext()
        {
            _logicEventHandler = new List<ILogicEventHandler>();
        }

        public void AddEventHandler(ILogicEventHandler handler)
        {
            _logicEventHandler.Insert(0, handler);

            _logicEventHandler.Sort((a, b) => { return a.GetExecuteOrder() - b.GetExecuteOrder(); });
        }

        public void RemoveHandler(ILogicEventHandler handler)
        {
            if (!_logicEventHandler.Contains(handler))
                return;
            
            _logicEventHandler.Remove(handler);
        }

        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams)
        {
            _logicEventHandler.ForEach(a=>a.OnLogicEvent(logicEvent, eventParams));
        }
    }
}