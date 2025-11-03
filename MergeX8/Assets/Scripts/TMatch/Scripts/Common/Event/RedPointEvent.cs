/*
 * 红点事件 - RedPointEvent
 * @author lu
 */

namespace TMatch
{
    public class RedPointEvent : BaseEvent
    {
        // 值
        public int Value { get; set; }

        // 索引
        public int Index { get; set; }

        public RedPointEvent(int index, int value) : base(EventEnum.REDPOINT)
        {
            Index = index;
            Value = value;
        }
    }
}
