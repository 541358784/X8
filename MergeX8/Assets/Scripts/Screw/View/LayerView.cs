using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/Layer")]
    public class LayerView : Entity
    {
        [UIBinder("Screws")] private Transform screwsContainer;
        [UIBinder("Panels")] private Transform panelsContainer;
        [UIBinder("Shields")] private Transform shieldsContainer;

        private LayerModel _layerModel;

        private Dictionary<int, PanelView> _panelBodyViews;
        private Dictionary<int, ScrewView> _screwViews;
        private Dictionary<int, ShieldView> _shieldViews;

        private Dictionary<ScrewBlocker, Dictionary<int, BaseBlockerView>> _blockerViews;

        public PanelView GetPanelView(int id)
        {
            if (_panelBodyViews == null || !_panelBodyViews.ContainsKey(id))
                return null;

            return _panelBodyViews[id];
        }
        
        public void SetUpLayer(LayerModel layerModel)
        {
            _layerModel = layerModel;

            _panelBodyViews = new Dictionary<int, PanelView>();
            foreach (var panelBodyModel in _layerModel.PanelBodyModels.Values)
            {
                var panelBodyView = LoadEntity<PanelView>(panelsContainer, context);
                panelBodyView.SetUpBody(panelBodyModel);
                _panelBodyViews.Add(panelBodyModel.PanelId, panelBodyView);
            }

            _screwViews = new Dictionary<int, ScrewView>();
            foreach (var screwModel in _layerModel.ScrewModels.Values)
            {
                var screwView = LoadEntity<ScrewView>(screwsContainer, context);
                screwView.SetUpScrew(screwModel);
                _screwViews.Add(screwModel.ScrewId, screwView);
            }

            _shieldViews = new Dictionary<int, ShieldView>();
            foreach (var shieldModel in _layerModel.ShieldModels.Values)
            {
                var shieldView = LoadEntity<ShieldView>(shieldsContainer, context);
                shieldView.SetUpBody(shieldModel);
                _shieldViews.Add(shieldModel.ShieldId, shieldView);
            }

            _blockerViews = new Dictionary<ScrewBlocker, Dictionary<int, BaseBlockerView>>();
            foreach (var screwModel in _layerModel.ScrewModels.Values)
            {
                var screwView = GetScrewView(screwModel);
                if (!screwModel.HasBlocker)
                    continue;
                foreach (var keyValue in screwModel.ScrewBlockers)
                {
                    switch (keyValue.Key)
                    {
                        case ScrewBlocker.ConnectBlocker:
                            var connectBlockerModel = (ConnectBlockerModel)keyValue.Value;

                            bool hasConnect = false;
                            if (_blockerViews.ContainsKey(ScrewBlocker.ConnectBlocker))
                            {
                                for (int i = 0; i < connectBlockerModel.ConnectScrew.Count; i++)
                                {
                                    if (_blockerViews[ScrewBlocker.ConnectBlocker].ContainsKey(connectBlockerModel.ConnectScrew[i]))
                                        hasConnect = true;
                                }
                            }
                            else
                            {
                                _blockerViews.Add(ScrewBlocker.ConnectBlocker, new Dictionary<int, BaseBlockerView>());
                            }

                            if (!hasConnect)
                            {
                                var connectScrewViews = new List<ScrewView>();
                                for (int i = 0; i < connectBlockerModel.ConnectScrew.Count; i++)
                                    connectScrewViews.Add(GetScrewView(connectBlockerModel.ConnectScrew[i]));

                                connectScrewViews.Add(screwView);
                                var connectBlockerView = LoadEntity<ConnectBlockerView>(root, context);
                                connectBlockerView.SetConnectScrewViews(connectScrewViews, screwModel);

                                for (int i = 0; i < connectBlockerModel.ConnectScrew.Count; i++)
                                    _blockerViews[ScrewBlocker.ConnectBlocker].Add(connectBlockerModel.ConnectScrew[i], connectBlockerView);

                                _blockerViews[ScrewBlocker.ConnectBlocker].Add(screwModel.ScrewId, connectBlockerView);
                            }

                            break;
                        case ScrewBlocker.IceBlocker:
                            if (!_blockerViews.ContainsKey(ScrewBlocker.IceBlocker))
                                _blockerViews.Add(ScrewBlocker.IceBlocker, new Dictionary<int, BaseBlockerView>());

                            var iceBlockerView = LoadEntity<IceBlockerView>(root, context);
                            iceBlockerView.SetUpView(screwModel);
                            _blockerViews[ScrewBlocker.IceBlocker].Add(screwModel.ScrewId, iceBlockerView);
                            break;
                        case ScrewBlocker.ShutterBlocker:
                            if (!_blockerViews.ContainsKey(ScrewBlocker.ShutterBlocker))
                                _blockerViews.Add(ScrewBlocker.ShutterBlocker, new Dictionary<int, BaseBlockerView>());

                            var shutterBlockerModel = (ShutterBlockerModel)keyValue.Value;

                            var shutterBlockerView = LoadEntity<ShutterBlockerView>(root, context);
                            shutterBlockerView.SetUpView(screwModel, shutterBlockerModel);
                            _blockerViews[ScrewBlocker.ShutterBlocker].Add(screwModel.ScrewId, shutterBlockerView);
                            break;
                        case ScrewBlocker.BombBlocker:
                            if (!_blockerViews.ContainsKey(ScrewBlocker.BombBlocker))
                                _blockerViews.Add(ScrewBlocker.BombBlocker, new Dictionary<int, BaseBlockerView>());

                            var bombBlockerModel = (BombBlockerModel)keyValue.Value;

                            var bombBlockerView = LoadEntity<BombBlockerView>(root, context);
                            bombBlockerView.SetUpView(screwModel, bombBlockerModel, GetScrewView(screwModel.ScrewId));
                            _blockerViews[ScrewBlocker.BombBlocker].Add(screwModel.ScrewId, bombBlockerView);
                            break;
                        case ScrewBlocker.LockBlocker:
                            if (!_blockerViews.ContainsKey(ScrewBlocker.LockBlocker))
                                _blockerViews.Add(ScrewBlocker.LockBlocker, new Dictionary<int, BaseBlockerView>());

                            var lockBlockerModel = (LockBlockerModel)keyValue.Value;

                            var lockBlockerView = LoadEntity<LockBlockerView>(root, context);
                            lockBlockerView.SetUpView(screwModel, lockBlockerModel, lockBlockerModel.KeyScrewId);
                            _blockerViews[ScrewBlocker.LockBlocker].Add(screwModel.ScrewId, lockBlockerView);
                            break;
                        case ScrewBlocker.TieBlocker:
                            if (!_blockerViews.ContainsKey(ScrewBlocker.TieBlocker))
                                _blockerViews.Add(ScrewBlocker.TieBlocker, new Dictionary<int, BaseBlockerView>());
                            var tieBlockerModel = (TieBlockerModel)keyValue.Value;
                            var tieBlockerView = LoadEntity<TieBlockerView>(root, context);
                            Dictionary<int, ScrewView> tieJamViews = new Dictionary<int, ScrewView>();
                            for (int i = 0; i < tieBlockerModel.TieScrewIds.Count; i++)
                            {
                                tieJamViews.Add(tieBlockerModel.TieScrewIds[i], GetScrewView(tieBlockerModel.TieScrewIds[i]));
                            }

                            tieBlockerView.SetUpView(screwModel, tieBlockerModel, GetScrewView(screwModel.ScrewId), tieJamViews);
                            _blockerViews[ScrewBlocker.TieBlocker].Add(screwModel.ScrewId, tieBlockerView);
                            break;
                    }
                }
            }

            root.position = Vector3.back * layerModel.LayerId;
        }

        public T GetBlockerView<T>(ScrewBlocker type, ScrewModel screwModel) where T : BaseBlockerView
        {
            if (_blockerViews.ContainsKey(type) && _blockerViews[type].ContainsKey(screwModel.ScrewId))
                return _blockerViews[type][screwModel.ScrewId] as T;
            return null;
        }

        public List<BaseBlockerView> GetAllBlockerView(ScrewBlocker type)
        {
            if (_blockerViews.ContainsKey(type))
            {
                return _blockerViews[type].Values.ToList();
            }

            return null;
        }

        public ScrewView GetScrewView(ScrewModel screwModel)
        {
            return _screwViews[screwModel.ScrewId];
        }

        public ScrewView GetScrewView(int screwId)
        {
            ScrewView view;
            _screwViews.TryGetValue(screwId, out view);
            return view;
        }

        public bool CheckHasScrewMoving()
        {
            foreach (var screwView in _screwViews.Values)
            {
                if (screwView.IsMoving())
                {
                    return true;
                }
            }

            return false;
        }

        public override void Destroy()
        {
            foreach (var keyValue in _panelBodyViews)
                keyValue.Value.Destroy();

            foreach (var keyValue in _screwViews)
                keyValue.Value.Destroy();

            foreach (var dic in _blockerViews.Values)
            {
                foreach (var blockView in dic.Values)
                    blockView.Destroy();
            }

            base.Destroy();
        }

        public void EnterBreakPanel()
        {
            foreach (var keyValue in _screwViews)
                keyValue.Value.EnterBreakPanel();
        }

        public void ExitBreakPanel()
        {
            foreach (var keyValue in _screwViews)
                keyValue.Value.ExitBreakPanel();
        }

        public int GetCollectJamCount()
        {
            var count = 0;
            foreach (var keyValue in _screwViews)
            {
                if (keyValue.Value.IsClicked())
                    count++;
            }

            return count;
        }

        public bool HasPanelMoving()
        {
            foreach (var keyValue in _panelBodyViews)
            {
                var isMoving = keyValue.Value.IsMoving();
                if (isMoving)
                    return true;
            }

            return false;
        }

        public bool HasShield()
        {
            return _shieldViews != null && _shieldViews.Count > 0;
        }

        public bool HaveActiveShield()
        {
            if (!HasShield())
                return false;

            foreach (var kv in _shieldViews)
            {
                if (kv.Value.ShieldModel.IsActive)
                    return true;
            }

            return false;
        }

        public async UniTask RefreshShield(List<LayerView> layerViews)
        {
            if (!HasShield())
                return;

            foreach (var kv in _shieldViews)
            {
                if (!kv.Value.ShieldModel.IsActive)
                    continue;

                bool isFree = true;
                foreach (var panelId in kv.Value.ShieldModel.CoverPanelIds)
                {
                    var panelView = GetPanelView(panelId, layerViews);
                    if (panelView == null)
                    {
                        isFree = false;
                        continue;
                    }

                    if (panelView.PanelBodyModel.IsActive)
                    {
                        isFree = false;
                        break;
                    }
                }

                if (isFree)
                {
                    kv.Value.ShieldModel.IsActive = false;
                    await kv.Value.ActiveFalseAnim();
                }
            }
        }

        private PanelView GetPanelView(int id, List<LayerView> layerViews)
        {
            foreach (var layerView in layerViews)
            {
                var panelView = layerView.GetPanelView(id);
                if(panelView == null)
                    continue;

                return panelView;
            }

            return null;
        }
    }
}