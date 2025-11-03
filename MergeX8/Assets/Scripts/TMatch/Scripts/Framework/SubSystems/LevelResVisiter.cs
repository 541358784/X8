using System.Collections.Generic;
using System.IO;
using DragonU3DSDK.Asset;
using Framework;

namespace TMatch
{
    

public partial class PathManager
{
    /// <summary>
    /// 房间Prefab资源
    /// </summary>
    public const string ROOM_PREFAB_PATH = "RoomRes/Prefabs/Room/Room{0}";
    public const string ROOM_CONFIG_PATH = "RoomRes/Configs/Room/Room{0}";
    public const string ROOM_BLUEPRINT_PATH = "RoomRes/Textures/Blueprint/Room{0}";
    public const string ROOM_MATERIAL_PATH = "RoomRes/Materials/Room{0}";
    public const string MAP_CONFIG_PATH = "Configs/Map{0}Config";
    public const string ROOM_EFFECT_PATH = "RoomRes/Prefabs/effect";
    public const string ROOM_ANIM_PATH = "RoomRes/Animations";
    public const string ROOM_COMMON_MATERIAL_PATH = "RoomRes/Materials/Common";

    public const string COLOR_ORIGIN_PATH = "Color/ColorOrigin/{0}";
    public const string COLOR_RES_PATH = "Color/ColorRes/{0}";
    public const string COLOR_THUMBNAILS_PATH = "Color/ColorThumbnails/{0}";
}


    public class LevelResVisiter : GlobalSystem<LevelResVisiter>
    {
        public static string[] MAP_RES_TEMPLATES = new string[]
        {
            "ui/hotel3maps/map{0}.ab",
            "spriteatlas/hotelmap{0}atlas.ab",
            "audios/bgm/bg_hotel{0}.ab",
        };

        public static string[] WORLD_RES_TEMPLATES = new string[]
        {
            "hospital/maps/map{0}/config.ab",
            "hospital/maps/map{0}/nurseItem.ab",
            "hospital/maps/map{0}/prefabs.ab",
            "Hospital/maps/map{0}/spriteatlas/h1map{0}.ab",
            "Hospital/maps/map{0}/spriteatlas/h1memory{0}.ab",
            "Hospital/maps/map{0}/spriteatlas/h1role{0}.ab",
            "Hospital/maps/map{0}/upgrade.ab",
        };
        
        public static string[] HOSPITAL_RES_TEMPLATES = new string[]
        {
            "hospital/maps/map{0}/audio/music.ab",
            "hospital/maps/map{0}/config.ab",
            "hospital/maps/map{0}/nurseitem.ab",
            "hospital/maps/map{0}/prefabs.ab",
            "hospital/maps/map{0}/spriteatlas/h1map{0}.ab",
            "hospital/maps/map{0}/spriteatlas/h1memory{0}.ab",
            "hospital/maps/map{0}/spriteatlas/h1role{0}.ab",
            "hospital/maps/map{0}/upgrade.ab",
            "hospital/maps/map{0}/config_b.ab",
        };
        
        public class FileInfo
        {
            public string group;
            public string key;
            public string md5;
        }

        public static List<FileInfo> CheckNeedUpdateFile(Dictionary<string, string> needCheckAbRes, bool checkHash)
        {
            List<FileInfo> files = new List<FileInfo>();

            foreach (var item in needCheckAbRes)
            {
                var abName = Path.ChangeExtension(item.Key, "ab").ToLower();
                var groupName = item.Value;
                //DebugUtil.Log("检测需要下载的ABNAME:"+abName+",groupName:"+groupName);
                var updateDic = VersionManager.Instance.GetUpdateFilesDict(groupName, abName);
                if (updateDic != null && updateDic.Count > 0)
                {
                    foreach (KeyValuePair<string, string> keyValuePair in updateDic)
                    {
                        if (item.Key != keyValuePair.Key) continue;
                        var assetName = keyValuePair.Key;
                        var remoteMd5 = keyValuePair.Value;
                        //DebugUtil.Log("确定需要下载的ABNAME:"+assetName+",groupName:"+groupName);
                        files.Add(new FileInfo() {key = assetName, md5 = remoteMd5, group = item.Value});
                    }
                }
            }

            return files;
        }
    }
}