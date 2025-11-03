
using Framework.Utils;

namespace fsm_new
{
    public enum AsmrTypes
    {
        Click = 1, //1.点击
        Drag, //2.拖拽
        Swipe_Single, //3.单手滑动
        Swipe_Double, //4.双手滑动
        LongPress_Double, //5.双手长按
        Erase, //6.擦除
        Paint, //7.擦除
    }

    public struct EventAsmrGroupChange : IEvent
    {
        public int current;
        public int total;

        public EventAsmrGroupChange(int current, int total)
        {
            this.current = current;
            this.total = total;
        }
    }

    public struct EventAsmrStepChange : IEvent
    {
        public int groupIndex;
        public int stepCount;
        public int total;
        public int groupCount;

        public EventAsmrStepChange(int groupIndex, int stepCount, int total, int groupCount)
        {
            this.groupIndex = groupIndex;
            this.stepCount = stepCount;
            this.total = total;
            this.groupCount = groupCount;
        }
    }
}