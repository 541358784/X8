using UnityEngine;

namespace TileMatch.Game.Layout
{
    public class LayoutView : ILoadView, IDestroy
    {
        public Layout _layout;
        public Transform _parent;
        public GameObject _root;
        
        public LayoutView(Layout layout)
        {
            _layout = layout;
        }

        public void LoadView(Transform parent)
        {
            _parent = parent;
            _root = new GameObject("Layout " + _layout._layoutModel._index);
            CommonUtils.AddChild(parent, _root.transform);

            _root.transform.localPosition = Vector3.zero;
        }

        public void Destroy()
        { 
            if(_root != null)
                GameObject.Destroy(_root);

            _root = null;
            _layout = null;
        }
    }
}