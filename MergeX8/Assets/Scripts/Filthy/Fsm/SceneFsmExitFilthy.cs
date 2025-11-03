namespace Filthy.Fsm
{
    public class SceneFsmExitFilthy : IFsmState
    {
        public StatusType Type => StatusType.ExitFilthy;
        
        public void Enter(params object[] objs)
        {
        }

        public void Update(float deltaTime)
        {
        }

        public void LateUpdate(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}