using System.Collections.Generic;
using Screw;
using Screw.Configs;

namespace Screw
{
    public class TieBlockerModel : BaseBlockerModel
    {
        private ScrewGameContext context;

        private ScrewModel _screwModel;
        public ScrewModel ScrewModel => _screwModel;

        public int ScrewId => _screwModel.ScrewId;

        private List<int> tieScrewIds;
        public List<int> TieScrewIds => tieScrewIds;

        public TieBlockerModel() : base(ScrewBlocker.TieBlocker)
        {
            
        }

        public override void Complete()
        {
            
        }

        public override bool IsComplete()
        {
            return tieScrewIds.Count == 0;
        }

        public override bool IsBlocking()
        {
            return tieScrewIds.Count > 0;
        }

        public override void Init(ScrewModel inScrewModel, LevelScrewBlock screwBlockerUnion, ScrewGameContext inContext)
        {
            _screwModel = inScrewModel;
            tieScrewIds = new List<int>();
            tieScrewIds.AddRange(screwBlockerUnion.tieIds);
            context = inContext;
        }

        public void RemoveTieId(int screwId)
        {
            tieScrewIds.Remove(screwId);
        }
    }
}