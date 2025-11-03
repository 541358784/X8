
namespace OutsideGuide
{
    public enum GuideTouchMaskEnum
    {
        Within = 0, // 点击在圈内
        Outside = 1, // 点击在圈外
    }


    public enum GameGuideEventParamType
    {
        None = 0, 
        Level,//关卡Id
        Equipment,//设备Id
        Patient,//病人id
    }
    public enum GuideMaskType
    {
        Circle,
        Rectangle,
        ChangeOrder,
    }
    
    public enum NpcPos
    {
        Left,
        Right
    }
    public enum DialogType
    {
        None, //没有尖
        Left, //左边有尖
        Right, //右边有尖
    }
    public enum GuideArrowType
    {
        None = 0,
        Down,
        Up,
        FingerRightDown,
        FingerRightUp,
        FingerLeftDown,
        FingerLeftUp,
    }
}