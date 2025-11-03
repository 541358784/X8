using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gameplay;
using UnityEngine;

public partial class PathManager
{
#if UNITY_ANDROID
    static readonly string targetName = "android";
#elif UNITY_IPHONE
    static readonly string targetName = "iphone";
#elif UNITY_STANDALONE
    static readonly string targetName = "standalone";
#else
    static readonly string targetName = "mac";
#endif

    public static readonly string AdDataConfigPath = "Configs/ConfigAdReward/configadreward";

    public static readonly string
        assetBundleOutPath = Application.dataPath + "/AssetBundleOutNew/" + targetName; //新的资源打包模式输出路径

    public static readonly string dynSubPath = "DynRes";
    public static readonly string dynAssetBundlePath = assetBundleOutPath + "/" + dynSubPath;
    public static readonly string dynPathFile = "pathfile.json";
    public static readonly string staticPathFile = "pathfileother.json";
    public static readonly string dynPathFilePath = dynAssetBundlePath + "/" + dynPathFile;
    public static readonly string uiPrefabPathUnderExport = "Prefabs";

    public const string jsonConfigPath = /* export/ */ "configs/home";
    public static readonly string streamingPath_Platform = Application.streamingAssetsPath + "/" + targetName;
    public static readonly string TargetName = targetName;
    public static readonly string DummyMaterialBundleName = "dummyMaterial";
    public static readonly string CookingPrefabPrefix = uiPrefabPathUnderExport + "/";
    public static readonly string FriendHeadPath = CookingPrefabPrefix + "Common/FriendHead";

    public static readonly string SystemMailItemPath = CookingPrefabPrefix + "Common/FBMailBoxItem2";
    public static readonly string FriendMailItemPath = CookingPrefabPrefix + "Common/FBMailBoxItem";

    public static readonly string ActivityRoomItemPath = CookingPrefabPrefix + "ActivityRoom";
    public static readonly string EffectPath = CookingPrefabPrefix + "Effect";

    public static readonly string streamingAssetsPath_Platform = Application.streamingAssetsPath + "/" + targetName;
#if !UNITY_EDITOR && UNITY_ANDROID
    public static readonly string streamingAssetsPath_Platform_ForWWWLoad = streamingAssetsPath_Platform;
#elif UNITY_EDITOR
    public static readonly string streamingAssetsPath_Platform_ForWWWLoad = streamingAssetsPath_Platform;
#else
    public static readonly string streamingAssetsPath_Platform_ForWWWLoad = "file:///" + streamingAssetsPath_Platform;
#endif

    /// <summary>
    /// 整理代码的引用的path
    /// </summary>
    public static readonly string AnimationPrefix = "Animations/Character";

    public static readonly string ParticalSystemsCommon = "ParticalSystems/Common";
    public static readonly string MaterialsCommon = "Effects/Materials/UIMaterials";
    public static readonly string MaterialUIEffect = "Materials/UIEffect";
    public static readonly string MaterialCooking = "Materials/Cooking";
    public static readonly string Configs = "Configs";

    public static readonly string CommonPrefabPrefix = CookingPrefabPrefix + "Common";

    //没有自定义动画，添加通用动画
    public static readonly string CommonBuildingAnimator = "Video/BuildingAnimator";

    /// <summary>
    /// 房间配置文件
    /// </summary>
    public const string ROOM_NODE_JSON = "RoomRes/Configs/Room/Room{0}";

    /// <summary>
    /// 房间Prefab资源
    /// </summary>
    public const string ROOM_PREFAB_PATH = "RoomRes/Prefabs/Room/Room{0}";

    public const string ROOM_CONFIG_PATH = "RoomRes/Configs/Room/Room{0}";
    public const string ROOM_SOUNDS_PATH = "Audios/Sound{0}";
    public const string ROOM_ANIM_PATH = "RoomRes/Animations";
    public const string ROOM_EFFECT_PATH = "RoomRes/Prefabs/effect";
    public const string ROOM_MATERIAL_COMMON_PATH = "RoomRes/materials/common";
    public const string ROOM_MATERIAL_PATH = "RoomRes/Materials/Room{0}";
    public const string ROOM_ATLAS_HD = "SpriteAtlas/Remote/{0}/Hd";
    public const string ROOM_ATLAS_SD = "SpriteAtlas/Remote/{0}/Sd";

    public const string ROOM_ICONATLAS_HD = "SpriteAtlas/Remote/Icon{0}/Hd";
    public const string ROOM_ICONATLAS_SD = "SpriteAtlas/Remote/Icon{0}/Sd";

    public const string MAP_CONFIG_PATH = "Configs/Cooking/Map{0}Config";

    public static string AudioPath(string prefix, string audioName)
    {
        string[] audioArray = audioName.Split('/');
        if (audioArray.Length >= 2)
            return Path.Combine("Audios", audioArray[0], audioArray[1]);
        else
            return Path.Combine("Audios", prefix, audioName);
    }

    public static string MapPath(int mapId)
    {
        return Path.Combine(uiPrefabPathUnderExport, $"Map{mapId}");
    }

    public static string MapAudioPath(int mapId)
    {
        return AudioPath("BGM", $"bgm_map{mapId}");
    }

    public static string CommonPrefab(string prefabName)
    {
        return Path.Combine(CommonPrefabPrefix, prefabName);
    }

    public static string HomePrefabs(string prefabName)
    {
        return Path.Combine(uiPrefabPathUnderExport, "Home", prefabName);
    }
}