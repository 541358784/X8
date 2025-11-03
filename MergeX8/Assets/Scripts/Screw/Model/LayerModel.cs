using System.Collections.Generic;

namespace Screw
{
    public class LayerModel
    {
        public int LayerId { get; }
        public Dictionary<int, PanelBodyModel> PanelBodyModels { get; }

        public Dictionary<int, ScrewModel> ScrewModels { get; }

        public Dictionary<int, ShieldModel> ShieldModels { get; }
        public LayerModel(Dictionary<int, PanelBodyModel> panelBodyModels, Dictionary<int, ScrewModel> screwModels, Dictionary<int, ShieldModel> shieldModels, int layerId)
        {
            LayerId = layerId;

            PanelBodyModels = panelBodyModels;

            ScrewModels = screwModels;

            ShieldModels = shieldModels;
        }
    }
}