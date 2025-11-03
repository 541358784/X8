using DragonU3DSDK.Asset;
using Screw.Configs;
using UnityEngine;
using UnityEngine.U2D;

namespace Screw
{
    public class AssetModule : Singleton<AssetModule>
    {
        public T LoadAsset<T>(string assetName, Transform parent) where T : Object
        {
            var asset = ResourcesManager.Instance.LoadResource<T>(assetName, addToCache:false);
            if (typeof(T) == typeof(GameObject))
            {
                return GameObject.Instantiate(asset, parent);
            }

            return asset;
        }   
        
        public T LoadAsset<T>(string assetName) where T : Object
        {
            return LoadAsset<T>(assetName, null);
        }

        public TextAsset LoadLevelData(int levelId)
        {
            string folderName = "Group{0}";
            if (levelId < 10000)
            {
                folderName = string.Format(folderName, 0);
            }
            else
            {
                int group = int.Parse(levelId.ToString()[0].ToString());
                folderName = string.Format(folderName, group);
            }

            string folderPath = ConstConfig.FolderNameConfigPath(folderName);
            folderPath += $"/{levelId}";

            folderPath = folderPath.Replace("Assets/Export/", "");

            return LoadAsset<TextAsset>(folderPath);
        }
        
        public SpriteAtlas LoadSpriteAtlas(string atlasName)
        {
            return ResourcesManager.Instance.LoadSpriteAtlasVariant(atlasName);
        }

        public Sprite GetSprite(string atlasName, string spriteName)
        {
            var atlas = ResourcesManager.Instance.LoadSpriteAtlasVariant(atlasName);
            if (atlas == null)
                return null;

            return atlas.GetSprite(spriteName);
        }
    }
}