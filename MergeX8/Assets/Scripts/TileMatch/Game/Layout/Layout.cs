using System.Collections.Generic;
using UnityEngine;

namespace TileMatch.Game.Layout
{
    public class Layout : ILoadView, IDestroy
    {
        public LayoutModel _layoutModel;
        public LayoutView _layoutView;
        public List<Layer.Layer> _layers = new List<Layer.Layer>();
        
        public Layout(LayoutData.Layout layout, int index)
        {
            _layoutModel = new LayoutModel(this, layout, index);
            _layoutView = new LayoutView(this);
        }
        
        public void LoadView(Transform parent)
        {
            _layoutView.LoadView(parent);

            InitLayer();
        }

        private void InitLayer()
        {
            for (int i = 0; i < _layoutModel._layoutData.layers.Count; i++)
            {
                Layer.Layer layer = new Layer.Layer(_layoutModel._layoutData, _layoutModel._layoutData.layers[i], i);

                layer.LoadView(_layoutView._root.transform);
                _layers.Add(layer);
            }
        }

        public void Destroy()
        {
            if (_layers != null)
            {
                _layers.ForEach(a=>a.Destroy());
                _layers.Clear();
            }
            _layers = null;
            
            _layoutModel.Destroy();
            _layoutModel = null;
            
            _layoutView.Destroy();
            _layoutView = null;
        }
    }
}