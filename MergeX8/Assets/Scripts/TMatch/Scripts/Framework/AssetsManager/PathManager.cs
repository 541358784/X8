using System.Collections.Generic;
using DragonPlus.Config;

namespace TMatch
{


    public partial class PathManager
    {
        public static readonly string dynSubPath = "DynRes";

        // public static readonly string uiPrefabPathUnderExport = "Prefabs/UI/";
        public static readonly string CookingPrefabPrefix = "Prefabs/UI/Cooking/";

        public static readonly List<string> UIPrefabPath = new List<string>
            {"Prefabs/"};

        // public static string GetItem2DResAB(int roomId)
        // {
        //     var path = GameConfigModel.Instance.GetGlobalStringConfig(GlobalStringConfigKey.item_icon_path);
        //     return string.Format(path, roomId);
        // }
        //
        // public static string GetItem3DResAB(int roomId)
        // {
        //     var path = GameConfigModel.Instance.GetGlobalStringConfig(GlobalStringConfigKey.item_path);
        //     return string.Format(path, roomId);
        // }
        //
        // public static string GetRoom3DResAB(int roomId)
        // {
        //     var path = GameConfigModel.Instance.GetGlobalStringConfig(GlobalStringConfigKey.room_path);
        //     return string.Format(path, roomId);
        // }
        //
        // public static string GetRoomBGMResAB(int roomId)
        // {
        //     var path = GameConfigModel.Instance.GetGlobalStringConfig(GlobalStringConfigKey.room_bgm_path);
        //     return string.Format(path, roomId);
        // }
        //
        // public static string GetRoomAnimationResAB(int roomId)
        // {
        //     var path = GameConfigModel.Instance.GetGlobalStringConfig(GlobalStringConfigKey.room_animator_path);
        //     return string.Format(path, roomId);
        // }

    }
}
