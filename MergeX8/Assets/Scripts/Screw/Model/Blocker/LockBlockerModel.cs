
using Screw;
using Screw.Configs;

namespace Screw
{
    public class LockBlockerModel : BaseBlockerModel
    {
        private int keyScrewId;

        public int KeyScrewId => keyScrewId;

        private ScrewModel _screwModel;

        public ScrewModel ScrewModel => _screwModel;
        public int ScrewId => _screwModel.ScrewId;

        private bool _isCompleted = false;

        public LockBlockerModel() : base(ScrewBlocker.LockBlocker)
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
            return !_isCompleted;
        }

        public override void Init(ScrewModel screwModel, LevelScrewBlock screwBlockerUnion, ScrewGameContext inContext)
        {
            _screwModel = screwModel;
            keyScrewId = screwBlockerUnion.keyId;
        }
    }
}