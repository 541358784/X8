/// <summary>
/// 物品上限更新
/// </summary>
namespace TMatch
{


    public class ItemUpdateEvent : BaseEvent
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 增量
        /// </summary>
        public int Delta { get; }

        /// <summary>
        /// 总量
        /// </summary>
        public int Count { get; }

        public ItemUpdateEvent(int id, int delta, int count) : base(EventEnum.ItemUpdate)
        {
            Id = id;
            Delta = delta;
            Count = count;
        }
    }

    /// <summary>
    /// 物品上限更新
    /// </summary>
    public class ItemChangeEvent : BaseEvent
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 增量
        /// </summary>
        public int Delta { get; }

        /// <summary>
        /// 总量
        /// </summary>
        public int Count { get; }

        public ItemChangeEvent(int id, int delta, int count) : base(EventEnum.ItemChange)
        {
            Id = id;
            Delta = delta;
            Count = count;
        }
    }
}