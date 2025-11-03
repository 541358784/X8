namespace TMatch
{


    public partial class EventEnum
    {
        public const string GUIDE_EVENT = "GUIDE_EVENT";
        public const string GAMEGUIDE_START = "GAMEGUIDE_START"; //局内引导开始
        public const string GAMEGUIDE_END = "GAMEGUIDE_END"; //局内引导结束
        public const string GUIDESTEPCOMPLETE = "GUIDESTEPCOMPLETE"; //单步引导结束
    }

    namespace GameGuide
    {
        public enum GuideTouchMaskEnum
        {
            Within = 0, // 点击在圈内
            Outside = 1, // 点击在圈外
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
}