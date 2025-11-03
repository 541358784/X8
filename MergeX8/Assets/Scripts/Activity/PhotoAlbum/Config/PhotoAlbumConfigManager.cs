/************************************************
 * PhotoAlbum Config Manager class : PhotoAlbumConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.PhotoAlbum
{
    public partial class PhotoAlbumConfigManager : Manager<PhotoAlbumConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<PhotoAlbumGlobalConfig> PhotoAlbumGlobalConfigList;
        public List<PhotoAlbumStoreLevelConfig> PhotoAlbumStoreLevelConfigList;
        public List<PhotoAlbumStoreItemConfig> PhotoAlbumStoreItemConfigList;
        public List<PhotoAlbumTaskRewardConfig> PhotoAlbumTaskRewardConfigList;
        public List<PhotoAlbumPhotoConfig> PhotoAlbumPhotoConfigList;
        public List<PhotoAlbumPhotoPieceConfig> PhotoAlbumPhotoPieceConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(PhotoAlbumGlobalConfig)] = "PhotoAlbumGlobalConfig",
            [typeof(PhotoAlbumStoreLevelConfig)] = "PhotoAlbumStoreLevelConfig",
            [typeof(PhotoAlbumStoreItemConfig)] = "PhotoAlbumStoreItemConfig",
            [typeof(PhotoAlbumTaskRewardConfig)] = "PhotoAlbumTaskRewardConfig",
            [typeof(PhotoAlbumPhotoConfig)] = "PhotoAlbumPhotoConfig",
            [typeof(PhotoAlbumPhotoPieceConfig)] = "PhotoAlbumPhotoPieceConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("photoalbumglobalconfig")) return false;
            if (!table.ContainsKey("photoalbumstorelevelconfig")) return false;
            if (!table.ContainsKey("photoalbumstoreitemconfig")) return false;
            if (!table.ContainsKey("photoalbumtaskrewardconfig")) return false;
            if (!table.ContainsKey("photoalbumphotoconfig")) return false;
            if (!table.ContainsKey("photoalbumphotopiececonfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "PhotoAlbumGlobalConfig": cfg = PhotoAlbumGlobalConfigList as List<T>; break;
                case "PhotoAlbumStoreLevelConfig": cfg = PhotoAlbumStoreLevelConfigList as List<T>; break;
                case "PhotoAlbumStoreItemConfig": cfg = PhotoAlbumStoreItemConfigList as List<T>; break;
                case "PhotoAlbumTaskRewardConfig": cfg = PhotoAlbumTaskRewardConfigList as List<T>; break;
                case "PhotoAlbumPhotoConfig": cfg = PhotoAlbumPhotoConfigList as List<T>; break;
                case "PhotoAlbumPhotoPieceConfig": cfg = PhotoAlbumPhotoPieceConfigList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            return cfg;
        }
        public void InitConfig(String configJson = null)
        {
            ConfigFromRemote = true;
            Hashtable table = null;
            if (!string.IsNullOrEmpty(configJson))
                table = JsonConvert.DeserializeObject<Hashtable>(configJson);

            if (table == null || !CheckTable(table))
            {
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/PhotoAlbum/photoalbum");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/PhotoAlbum/photoalbum error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            PhotoAlbumGlobalConfigList = JsonConvert.DeserializeObject<List<PhotoAlbumGlobalConfig>>(JsonConvert.SerializeObject(table["photoalbumglobalconfig"]));
            PhotoAlbumStoreLevelConfigList = JsonConvert.DeserializeObject<List<PhotoAlbumStoreLevelConfig>>(JsonConvert.SerializeObject(table["photoalbumstorelevelconfig"]));
            PhotoAlbumStoreItemConfigList = JsonConvert.DeserializeObject<List<PhotoAlbumStoreItemConfig>>(JsonConvert.SerializeObject(table["photoalbumstoreitemconfig"]));
            PhotoAlbumTaskRewardConfigList = JsonConvert.DeserializeObject<List<PhotoAlbumTaskRewardConfig>>(JsonConvert.SerializeObject(table["photoalbumtaskrewardconfig"]));
            PhotoAlbumPhotoConfigList = JsonConvert.DeserializeObject<List<PhotoAlbumPhotoConfig>>(JsonConvert.SerializeObject(table["photoalbumphotoconfig"]));
            PhotoAlbumPhotoPieceConfigList = JsonConvert.DeserializeObject<List<PhotoAlbumPhotoPieceConfig>>(JsonConvert.SerializeObject(table["photoalbumphotopiececonfig"]));
            
        }
    }
}