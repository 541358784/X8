using UnityEngine;

namespace Decoration
{
    public static class AtlasName
    {
        public const string UIStoryAtlas = "UIStoryAtlas";
        public const string MergeAtlas = "MergeAtlas";
    }
    
    
    public static class BuildingAnimationDefine
    {
        public static string NORMAL = "normal"; // 默认动画
        public static string ON_TAP_ANIMATION = "Tap";// 单击的反馈动画
        public static string PREVIEW = "Choice";// 建造预览的动画
        public static string BUILD_BUILDING_SUCCESS_ANIMATION = "confirm";// 建造成功的动画
        public static string FIRST_BUILD_BUILDING_SUCCESS_ANIMATION = "first_confirm";// 首次建造成功的动画
    
    
        public static string REMOVE = "appear"; // 清除动画
        public static string SHOW = "appear"; // 加载动画
    }
    
    public enum DecoUIStatus
    {
        Hide,        //隐藏所有
        Normal,      //显示全部
        Decoration,  //装修模式，N选1
    }
}