using Screw;
using Screw.Module;
using UnityEngine;

namespace Screw
{
    public class GameTimerHandler : ILogicEventHandler
    {
        public float elapsedTime;

        public float timeOutTime;
        private float addExtraTime;

        private ScrewGameContext _context;
        private bool timerEnabled = false;

        public GameTimerHandler(ScrewGameContext context, float timeOut)
        {
            _context = context;
            timeOutTime = timeOut;
        }

        public int GetExecuteOrder()
        {
            return ExecuteOrder.GameTimerHandler;
        }

        public void OnLevelStart()
        {
            elapsedTime = 0;
            timerEnabled = true;
            UpdateScheduler.Instance.HookUpdate(Tick);
        }
        
        public void OnLevelEnd()
        {
            UpdateScheduler.Instance.UnhookUpdate(Tick);
        }

        public float GetLeftTime()
        {
           return timeOutTime - elapsedTime;
        }

        public float GetProgress()
        {
            var progress = 1.0f - (elapsedTime - addExtraTime) / (timeOutTime - addExtraTime);
            if (progress < 0)
            {
                return 0.0f;
            }
            return progress;
        }

        public void EnableTimer(bool enable)
        {
            timerEnabled = enable;
        }

        public void Tick()
        {
            if ((_context.gameState == ScrewGameState.InProgress || _context.gameState == ScrewGameState.InUseBooster) && timerEnabled)
            {
                elapsedTime += Time.deltaTime;

                _context.headerView.UpdateTimerText();
                if (IsTimeOut() && !_context.actionController.HasActionInExecute() && !_context.IsMovingTask())
                {
                    _context.gameState = ScrewGameState.Fail;
                    _context.failReason = LevelFailReason.Timer;
                    _context.hookContext.OnLogicEvent(LogicEvent.TimeOut, null);
                    UIModule.Instance.ShowUI(typeof(UITimesUpFail), _context);
                }
            }
        }

        public bool IsTimeOut()
        {
            return elapsedTime > timeOutTime;
        }

        public void AddExtraTime(float extraTime)
        {
            timeOutTime += extraTime;
            addExtraTime += extraTime;
        }

        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams)
        {
            switch (logicEvent)
            {
                case LogicEvent.EnterLevel:
                    UIModule.Instance.ShowUI(typeof(UITimeLevelOpen), _context);
                    break;
                case LogicEvent.ExitLevel:
                    OnLevelEnd();
                     break;
                case LogicEvent.ActionFinish:
                    Tick();
                    break;
            }
        }
    }
}