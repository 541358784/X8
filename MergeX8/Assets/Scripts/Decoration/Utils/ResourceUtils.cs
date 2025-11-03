using UnityEditor;
using UnityEngine;
using DragonU3DSDK.Asset;

namespace Decoration
{
    public static class Utils
    {
        private const string ResPathRoot = "Decoration";
        private const string ResPathWorld = ResPathRoot + "/Worlds/World";

        public static string PathTextureIcon(int worldId, int areaId) {
            return $"{ResPathWorld}{worldId}/Textures/Icons/{areaId}";
        }

        public static string PathTextureFog(int worldId,  string imageName) {
            return $"{ResPathWorld}{worldId}/Texture/Fog/{imageName}";
        }
        
        public static string PathSpriteAtlasIcon(int worldId)
        {
            return $"{ResPathWorld}{worldId}/SpriteAtlas/Icon/Icon";
        }
        
        public static string PathSpriteAtlas()
        {
            return $"{ResPathRoot}/SpriteAtlas";
        }
        public static string PathPrefabMap(int worldId)
        {
            return $"{ResPathWorld}{worldId}/Prefabs/Map";
        }
        public static string PathPrefabItem(int worldId)
        {
            return $"{ResPathWorld}{worldId}/Prefabs/Item";
        }
        public static string PathPrefabArea(int worldId, int areaId)
        {
            return areaId < 0
                ? $"{ResPathWorld}{worldId}/Prefabs/Area"
                : $"{ResPathWorld}{worldId}/Prefabs/Area/{areaId}";
        }
        public static string PathPrefabBuilding(int worldId, int areaId, int buildId)
        {
            return $"{ResPathWorld}{worldId}/Prefabs/Building/{areaId}/{buildId}";
        }
        public static string PathPrefabBuilding(string activityType) {
            return $"{ResPathRoot}/Activity/Prefabs/Building/{activityType}";
        }
        public static string PathPrefabSpines(string filePath) {
            return $"{ResPathRoot}/Spines{filePath}";
        }
        
        public static T LoadResource<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return AssetDatabase.LoadAssetAtPath<T>($"Assets/Export/{path}.json");
#endif
            return ResourcesManager.Instance.LoadResource<T>(path);
        }
    }
}