using Decoration;
using Framework;


namespace TMatch
{
    public class TMatchSystem
    {
        private SubSystemManager _subSystemManager = new SubSystemManager();

        public void Enter(FsmParamTMatch enterParam)
        {
            _subSystemManager.Init();
            if (_subSystemManager.AddGroup("Framework"))
            {
                _subSystemManager.AddSubSystem<TMatchEnvSystem>();
                _subSystemManager.AddSubSystem<TMatchItemSystem>();
                _subSystemManager.AddSubSystem<TMatchCollectorSystem>();
                _subSystemManager.AddSubSystem<TMatchStateSystem>();
                _subSystemManager.AddSubSystem<TMatchBoostSystem>();
                InitLevelController(enterParam);
            }

            if (_subSystemManager.AddGroup("Gameplay"))
            {
                _subSystemManager.AddSubSystem<TMatchLevelBoostSystem>();
                _subSystemManager.AddSubSystem<TMatchGoldenHatterSystem>();
                _subSystemManager.AddSubSystem<TMatchPruchaseBoostSystem>();

            }

            LevelController.Build(enterParam);

            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_ENTER);
        }

        public void Exit()
        {
            //引导补丁：保证level-1的点消引导能重复进行
            // if (LevelController.LevelData.level == 1) GuideSubSystem.Instance.ClearCacheFinishedGuide();

            _subSystemManager.Release();

            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_EXIT);
        }

        public void Update(float deltaTime)
        {
            _subSystemManager?.Update(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            _subSystemManager?.LateUpdate(deltaTime);
        }

        private void InitLevelController(FsmParamTMatch enterParam)
        {
            if (enterParam.GameType == TMGameType.Normal)
            {
                LevelController = _subSystemManager.AddSubSystem<TMatchNormalLevelController>();
            }
            else if(enterParam.GameType == TMGameType.Kapibala)
            {
                LevelController = _subSystemManager.AddSubSystem<TMatchKapibalaLevelController>();
            }
            //根据玩法来选择添加controller
        }

        public static TMatchLevelController LevelController;
    }
}