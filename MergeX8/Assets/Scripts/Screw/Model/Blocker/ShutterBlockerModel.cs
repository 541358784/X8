
using Screw;
using Screw.Configs;

namespace Screw
{
    public class ShutterBlockerModel : BaseBlockerModel
    {
        private ScrewModel _screwModel;

        public ScrewModel ScrewModel => _screwModel;
        public int ScrewId => _screwModel.ScrewId;

        private bool _isOpen = false;

        private bool _isCompleted = false;

        private ScrewGameContext context;

        public ShutterBlockerModel() : base(ScrewBlocker.ShutterBlocker)
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
            return !_isOpen;
        }

        public void ChangeStatus()
        {
            _isOpen = !_isOpen;
        }

        public override void Init(ScrewModel screwModel, LevelScrewBlock screwBlockerUnion, ScrewGameContext inContext)
        {
            _screwModel = screwModel;
            _isCompleted = false;
            _isOpen = screwBlockerUnion.isOpen;
            context = inContext;
        }
    }
}