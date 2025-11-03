/// <summary>
/// 物品持续时间更新
/// </summary>
namespace TMatch
{


    public class ItemDurationUpdateEvent : BaseEvent
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 到期时间
        /// </summary>
        public ulong Timestamp { get; }

        public ItemDurationUpdateEvent(int id, ulong timestamp) : base(EventEnum.ItemDurationUpdate)
        {
            Id = id;
            Timestamp = timestamp;
        }
    }
}
