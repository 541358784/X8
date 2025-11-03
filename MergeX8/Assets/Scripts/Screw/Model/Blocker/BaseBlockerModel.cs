using Screw;
using Screw.Configs;

namespace Screw
{
    public abstract class BaseBlockerModel
    {
        public readonly ScrewBlocker BlockerType;

        public abstract void Complete();
        public abstract bool IsComplete();
        
        public abstract bool IsBlocking();
        
        protected BaseBlockerModel(ScrewBlocker blockerType)
        {
            BlockerType = blockerType;
        }

        public abstract void Init(ScrewModel screwModel, LevelScrewBlock screwBlockerUnion, ScrewGameContext inContext);
    }
}