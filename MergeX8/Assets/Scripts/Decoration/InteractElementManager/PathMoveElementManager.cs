using System.Collections.Generic;
using UnityEngine;

namespace Decoration
{
    public class PathMoveElementManager : Singleton<PathMoveElementManager>
    {
        private List<GameObject> _pathElements = new List<GameObject>();
        
        public void LoadElement(int worldId)
        {
            _pathElements.Clear();
            
            var config = DecorationConfigManager.Instance.TablePathMoveItemList.Find(a => a.id == worldId);
            if(config == null)
                return;
            
            foreach (var path in config.movePath)
            {
                Transform item = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find(path);
                if(item == null)
                    continue;
                
                _pathElements.Add(item.gameObject);
            }
        }

        public void SetActive(bool isActive)
        {
            _pathElements.ForEach(a=>a.gameObject.SetActive(isActive));
        }
    }
}