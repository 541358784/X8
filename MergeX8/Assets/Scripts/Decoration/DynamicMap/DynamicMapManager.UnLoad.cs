using UnityEngine;

namespace Decoration.DynamicMap
{
    public partial class DynamicMapManager
    {
        public void UnLoad()
        {
            _dynamicObj = null;
            
            _unLoadData.Clear();
            _loadData.Clear();
            _loadItems.Clear();        
            
            if(_loadCoroutine != null)
                StopCoroutine(_loadCoroutine);
            
            CancelInvoke("UnLoadInvoke");
            
            _currentChunkAreas.Clear();
            
            foreach (var kv in _areaAtlas)
            {
                OpUtils.UnloadSpriteAtlas(AssetCheckManager.GetAreaBuildAtlasName(kv.Key));
            }
            _areaAtlas.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}