using Deco.Node;
using Farm.Model;
using UnityEngine;

namespace Farm.Logic
{
    public interface ILogic
    {
        public DecoNode _node { get; set; }
        public FarmType _type { get; set; }
        public bool _isInit { get; set; }
        public GameObject _root{ get; set; }
        
        public void Init(Transform root, DecoNode node, FarmType type);

        public void UpdateStatus();
        public void Select();
        public void UnSelect();
    }
}