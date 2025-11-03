using UnityEngine;

namespace TileMatch.Game.Layer
{
    public class LayerView : ILoadView, IDestroy
    {
        public Layer _layer;

        public Transform _parent;
        public GameObject _root;
        public LayerView(Layer layer)
        {
            _layer = layer;
        }

        public void LoadView(Transform parent)
        {
            _parent = parent;

            _root = new GameObject("Layer " + _layer._layerModel._index);
            CommonUtils.AddChild(parent, _root.transform);

            _root.transform.localPosition = Vector3.zero;
        }

        public void Destroy()
        {
            if(_root != null)
                GameObject.Destroy(_root);

            _root = null;
            _layer = null;
        }
    }
}