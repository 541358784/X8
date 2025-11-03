using Framework;

namespace Psychology.System
{
    public class PsychologySystem
    {
        private SubSystemManager _subSystemManager = new SubSystemManager();
    
        public void Enter()
        {
            _subSystemManager.Init();
            if (_subSystemManager.AddGroup("Framework"))
            {
                _subSystemManager.AddSubSystem<CameraSystem>();
            }
        }

        public void Exit()
        {
            _subSystemManager.Release();
        }
        
        public void Update(float deltaTime)
        {
            _subSystemManager.Update(deltaTime);
        }
    }
}