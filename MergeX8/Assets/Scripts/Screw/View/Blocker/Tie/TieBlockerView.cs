using System.Collections.Generic;
using Screw.UI;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/TieBody")]
    public class TieBlockerView : BaseBlockerView
    {
        private TieHeadView _tieHeadView;
        private Dictionary<int, TieLineView> _tieLineViews;

        public void SetUpView(ScrewModel screwModel, TieBlockerModel tieBlockerModel, ScrewView screwView, Dictionary<int, ScrewView> tieJamViews)
        {
            root.position = screwModel.Position;

            _tieLineViews = new Dictionary<int, TieLineView>();

            _tieHeadView = LoadEntity<TieHeadView>(root, context);
            _tieHeadView.SetUpView(tieBlockerModel.TieScrewIds.Count, screwModel.LayerId);

            for (int i = 0; i < tieBlockerModel.TieScrewIds.Count; i++)
            {
                var tieLineView = LoadEntity<TieLineView>(root, context);
                
                var connectScrewViews = new List<ScrewView>();
                connectScrewViews.Add(tieJamViews[tieBlockerModel.TieScrewIds[i]]);
                connectScrewViews.Add(screwView);

                tieLineView.SetConnectScrewViews(connectScrewViews, screwModel);
                _tieLineViews.Add(tieBlockerModel.TieScrewIds[i], tieLineView);
            }
        }

        public void UnlockTie(ScrewModel model)
        {
            _tieHeadView.PlayAni();

            if (_tieLineViews.ContainsKey(model.ScrewId))
            {
                _tieLineViews[model.ScrewId].DisConnect();
            }
        }

        public override void Destroy()
        {
            _tieHeadView.Destroy();
            foreach (var tieLineView in _tieLineViews.Values)
                tieLineView.Destroy();
            base.Destroy();
        }
    }
}