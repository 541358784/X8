using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace DragonPlus
{
    [Serializable]
    public class BuildingPointData
    {
        public int buildingPointId;
        public int buildingId;
    }

    [Serializable]
    [CreateAssetMenu(menuName = "MapEditorCache")]
    public class MapEditorCache : ScriptableObject
    {
        public List<BuildingPointData> buildingPointCacheList;
        public void Add (int buildingPointId, int buildingId)
        {
            var data = new BuildingPointData();
            data.buildingPointId = buildingPointId;
            data.buildingId = buildingId;
            buildingPointCacheList.Add(data);
        }
    }
}