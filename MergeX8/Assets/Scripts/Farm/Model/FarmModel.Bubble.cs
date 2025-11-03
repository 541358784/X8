using System.Collections.Generic;
using System.Linq;
using Deco.Node;
using Farm.Logic;
using UnityEngine;

namespace Farm.Model
{
    public partial class FarmModel
    {
        private Dictionary<DecoNode, BubbleLogic> _bubbleLogics = new Dictionary<DecoNode, BubbleLogic>();

        public BubbleLogic SpawnBubble()
        {
            var prefab = _poolManager.SpawnGameObject("Farm/Prefabs/UI/Bubble");
            if (prefab == null)
                return null;

            return prefab.GetOrCreateComponent<BubbleLogic>();
        }

        public void RecycleBubble(GameObject obj)
        {
            _poolManager.RecycleGameObject(obj);
        }  
        
        private void LoadBubble(DecoNode node, FarmType type)
        {
            if (!_bubbleLogics.ContainsKey(node))
            {
                _bubbleLogics.Add(node, SpawnBubble());
            }

            var logic = _bubbleLogics[node];
            logic.Init(node.GameObject.transform, node, type);

#if UNITY_EDITOR
            logic.gameObject.name = node.Id.ToString();
#endif
        }
        
        private void UnLoadBubble(DecoNode node)
        {       
            if(!_bubbleLogics.ContainsKey(node))
                return;
            
            RecycleBubble(_bubbleLogics[node].gameObject);
            _bubbleLogics.Remove(node);
        }
        public void UpdateBubble(DecoNode node)
        {   
            if(!_bubbleLogics.ContainsKey(node))
                return;
            
            _bubbleLogics[node].UpdateStatus();
        }

        private DecoNode FindBubble(int id)
        {
            return _bubbleLogics.Keys.ToList().Find(a=>a.Id == id);
        }

        public void UpdateAllBubbleStatus()
        {
            foreach (var logic in _bubbleLogics.Values)
            {
                logic.UpdateStatus();
            }
        }
    }
}