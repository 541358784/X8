using System.Collections.Generic;
using Deco.Node;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;

namespace Decoration
{
    public class DecoExtendAreaManager : Singleton<DecoExtendAreaManager>
    {
        private Dictionary<int, List<TableAreas>> _decoArea = new Dictionary<int, List<TableAreas>>();
        private Dictionary<int, int> _extendArea = new Dictionary<int, int>();

        private StorageDecoration storage
        {
            get{return StorageManager.Instance.GetStorage<StorageDecoration>();}
        }
        
        public void Init()
        {
            _decoArea.Clear();
            _extendArea.Clear();
            
            foreach (var tableAreas in DecorationConfigManager.Instance.AreaConfigList)
            {
                if(tableAreas.showArea == 0)
                    continue;

                if (!_decoArea.ContainsKey(tableAreas.showArea))
                    _decoArea[tableAreas.showArea] = new List<TableAreas>();
                
                _decoArea[tableAreas.showArea].Add(tableAreas);
                _extendArea.Add(tableAreas.id, tableAreas.id);
            }
            
            
            foreach (var kv in _decoArea)
            {
                if(storage.ExtendArea.ContainsKey(kv.Key))
                    continue;

                var defaultConfig = kv.Value.Find(a => a.isDefaultShow);
                if (defaultConfig == null)
                    defaultConfig = kv.Value[0];
                
                storage.ExtendArea.Add(kv.Key, defaultConfig.id);
            }
        }

        public bool ReceiveExtendArea(DecoNode node)
        {
            if(node == null || node.Stage == null || node.Stage.Area == null || node.Stage.Area.Config == null)
                return false;

            int areaId = node.Stage.Area.Config.id;
            if(!IsExtendArea(areaId))
                return false;

            int oldAreaId = GetCurrentExtendArea(areaId);
            if(oldAreaId == areaId)
                return false;

            var oldArea = DecoManager.Instance.FindArea(oldAreaId);
            if(oldArea != null && oldArea.Graphic != null)
                oldArea.Graphic.SetActive(false);
            
            var newArea = DecoManager.Instance.FindArea(areaId);
            if(newArea != null && newArea.Graphic != null)
                newArea.Graphic.SetActive(true);
            
            UpdateAreaShow(node.Stage.Area.Config.showArea, areaId);

            return true;
        }

        public void UpdateAreaShow(int area, int areaId)
        {
            if(!storage.ExtendArea.ContainsKey(area))
                storage.ExtendArea.Add(area, areaId);

            storage.ExtendArea[area] = areaId;
        }
        
        public bool CanShowArea(int areaId)
        {
            if (!IsExtendArea(areaId))
                return true;

            return GetCurrentExtendArea(areaId) == areaId;
        }

        public bool IsExtendArea(int areaId)
        {
            if (_extendArea.ContainsKey(areaId))
                return true;

            return false;
        }

        public int GetCurrentAreaByStorage(int area)
        {
            if (storage.ExtendArea.ContainsKey(area))
                return storage.ExtendArea[area];

            return -1;
        }
        public int GetCurrentExtendArea(int areaId)
        {
            var config = DecorationConfigManager.Instance.AreaConfigList.Find(a => a.id == areaId);
            if (config == null)
                return -1;

            return GetCurrentAreaByStorage(config.showArea);
        }

        public List<TableAreas> GetExtendAreas(int area)
        {
            if (!_decoArea.ContainsKey(area))
                return null;

            return _decoArea[area];
        }
    }
}