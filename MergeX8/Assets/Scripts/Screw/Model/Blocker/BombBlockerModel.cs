
using Screw;
using Screw.Configs;

namespace Screw
{
    public class BombBlockerModel : BaseBlockerModel
    {
        private ScrewModel _screwModel;

        public ScrewModel ScrewModel => _screwModel;
        public int ScrewId => _screwModel.ScrewId;

        private int _stageCount;
        public int StageCount => _stageCount;

        private bool _isCompleted = false;

        private ScrewGameContext context;

        public BombBlockerModel() : base(ScrewBlocker.BombBlocker)
        {
            
        }
        
        public override void Complete()
        {
            _isCompleted = true;
        }

        public override bool IsComplete()
        {
            return _isCompleted;
        }

        public override bool IsBlocking()
        {
            return false;
        }

        public void StageCounter()
        {
            _stageCount--;
        }

        public override void Init(ScrewModel screwModel, LevelScrewBlock screwBlockerUnion, ScrewGameContext inContext)
        {
            _screwModel = screwModel;
            _isCompleted = false;
            _stageCount = screwBlockerUnion.stageCount;
            context = inContext;
        }
    }
}