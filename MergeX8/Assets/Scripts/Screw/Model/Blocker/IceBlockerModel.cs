
using Screw;
using Screw.Configs;

namespace Screw
{
    public class IceBlockerModel : BaseBlockerModel
    {
        private ScrewModel _screwModel;

        public ScrewModel ScrewModel => _screwModel;
        public int ScrewId => _screwModel.ScrewId;

        private const int STAGECOUNT = 3;

        private int stageCounter = 3;

        public int StageCounter => stageCounter;

        private ScrewGameContext context;

        public IceBlockerModel() : base(ScrewBlocker.IceBlocker)
        {
            
        }

        public override void Complete()
        {
            stageCounter = 0;
        }

        public void NextStage()
        {
            if (stageCounter > 0)
            {
                stageCounter--;
            }
        }

        public override bool IsComplete()
        {
            return stageCounter <= 0;
        }

        public override bool IsBlocking()
        {
            return stageCounter > 0;
        }

        public override void Init(ScrewModel screwModel, LevelScrewBlock screwBlockerUnion, ScrewGameContext inContext)
        {
            _screwModel = screwModel;
            stageCounter = STAGECOUNT;
            context = inContext;
        }
    }
}