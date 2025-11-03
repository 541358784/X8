using LayoutData;

namespace TileMatch.Game.Layer
{
    public class LayerModel : IDestroy
    {
        public Layer _layer;
        public LayoutData.Layer _layerData;
        public LayoutData.Layout _layoutData;
        public int _index;

        public int _orderZ;
        public LayerModel(Layer layer, LayoutData.Layout layout, LayoutData.Layer layerData, int index)
        {
            _layer = layer;
            _layerData = layerData;
            _layoutData = layout;
            _index = index;

            _orderZ = index;
        }

        public void Destroy()
        {
        }
    }
}