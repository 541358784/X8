using System.Collections.Generic;
using Screw;
using Screw.Configs;

namespace Screw
{
    public class ConnectBlockerModel : BaseBlockerModel
    {
        private ScrewGameContext context;

        private List<int> connectScrew;

        private bool isComplete;
        public List<int> ConnectScrew => connectScrew;

        public ConnectBlockerModel() : base(ScrewBlocker.ConnectBlocker)
        {
            
        }

        public override void Complete()
        {
            isComplete = true;
        }

        public override bool IsComplete()
        {
            return isComplete;
        }

        public override bool IsBlocking()
        {
            for (int i = 0; i < connectScrew.Count; i++)
            {
                var jamView = context.GetScrewView(connectScrew[i]);
                if (jamView != null)
                {
                    return jamView.IsOverlapping();
                }
            }
            return false;
        }

        public override void Init(ScrewModel screwModel, LevelScrewBlock screwBlockerUnion, ScrewGameContext inContext)
        {
            connectScrew = new List<int>();
            connectScrew.AddRange(screwBlockerUnion.connetIds);
            context = inContext;
        }
    }
}