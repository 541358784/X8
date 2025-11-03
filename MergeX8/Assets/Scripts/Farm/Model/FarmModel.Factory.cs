using System.Collections.Generic;
using Deco.Node;
using Farm.Logic;
using UnityEngine;

namespace Farm.Model
{
    public partial class FarmModel
    {
        private Dictionary<FarmType, Dictionary<DecoNode, ILogic>> _logicMap = new Dictionary<FarmType, Dictionary<DecoNode, ILogic>>();
        
        public void CreateLogic(Transform root, DecoNode node, FarmType type)
        {
            ILogic logic = GetLogic(node, type);
            if (logic == null)
            {
                switch (type)
                {
                    case FarmType.Ground:
                        logic = root.gameObject.AddComponent<GroundLogic>();
                        break;
                    case FarmType.Tree:
                        logic = root.gameObject.AddComponent<TreeLogic>();
                        break;
                    case FarmType.Animal:
                        logic = root.gameObject.AddComponent<AnimalLogic>();
                        break;
                    case FarmType.Machine:
                        logic = root.gameObject.AddComponent<MachineLogic>();
                        break;
                }
            }
            
            if(logic == null)
                return;
            
            logic.Init(root, node, type);
            if (!_logicMap.ContainsKey(type))
                _logicMap[type] = new Dictionary<DecoNode, ILogic>();
            
            _logicMap[type][node] = logic;
        }

        public ILogic GetLogic(DecoNode node, FarmType type)
        {
            if (!_logicMap.ContainsKey(type))
                return null;

            if (!_logicMap[type].ContainsKey(node))
                return null;
            
            return _logicMap[type][node];
        }

        public bool RemoveLogic(DecoNode node, FarmType type)
        {
            var logic = GetLogic(node, type);
            if (logic == null)
                return false;

            _logicMap[type].Remove(node);
            
            GameObject.Destroy((MonoBehaviour)logic);

            return true;
        }

        public bool UpdateStatus(DecoNode node, FarmType type)
        {
            UpdateBubble(node);
            
            var logic = GetLogic(node, type);
            if (logic == null)
                return false;
            
            logic.UpdateStatus();

            return true;
        }
        
        public void RemoveAllLogic()
        {
            foreach (var kv in _logicMap)
            {
                foreach (var pair in kv.Value)
                {
                    GameObject.Destroy((MonoBehaviour)pair.Value);
                }
            }
            
            _logicMap.Clear();
        }
    }
}