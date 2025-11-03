using System;
using DragonU3DSDK.Storage;

namespace Farm.Order.Creator
{
    public class Creator
    {
        public Creator(Func<OrderSlot, bool> canCreate, Func<OrderSlot, StorageFarmOrderItem, StorageFarmOrderItem> tryCreate)
        {
            _canCreate = canCreate;
            _tryCreate = tryCreate;
        }
        public Func<OrderSlot, bool> _canCreate;
        public Func<OrderSlot, StorageFarmOrderItem, StorageFarmOrderItem> _tryCreate;
    }
}