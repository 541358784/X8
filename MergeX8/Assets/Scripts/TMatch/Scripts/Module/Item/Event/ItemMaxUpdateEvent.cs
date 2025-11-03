/// <summary>
/// 物品上限更新
/// </summary>
namespace TMatch
{
    public class ItemMaxUpdateEvent : BaseEvent
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 最大持有数
        /// </summary>
        public int Max { get; }

        public ItemMaxUpdateEvent(int id, int max) : base(EventEnum.ItemMaxUpdate)
        {
            Id = id;
            Max = max;
        }
    }
}