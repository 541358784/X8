using DragonPlus.Config.MiniGame;

namespace fsm_new
{
    public class AsmrStep
    {
        public bool Finish;

        public AsmrStepConfig Config;

        public AsmrStep(AsmrStepConfig config)
        {
            Config = config;
        }
    }
}