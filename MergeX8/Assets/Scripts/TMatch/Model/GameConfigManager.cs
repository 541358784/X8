using DragonPlus.Config.TMatch;
using TMatch;
namespace DragonPlus.Config.TMatchShop
{
    public partial class TMatchShopConfigManager
    {
        public int InnerBoostMagnetItemId;        //局内道具-磁铁的ItemId
        public int InnerBoostBroomItemId;         //局内道具-扫帚的ItemId
        public int InnerBoostWindmillItemId;      //局内道具-吹风的ItemId
        public int InnerBoostFrozenItemId;        //局内道具-冰封的ItemId

        public int OuterBoostLightingItemId;          //局外道具-闪电
        public int OuterBoostClockItemId;             //局外道具-时钟
        
        public int OuterBoostInfinityLightingItemId;  //局外道具-无限闪电
        public int OuterBoostInfinityClockItemId;     //局外道具-无限时钟

        public void InitBoost()
        {
            for (int i = 0; i < ItemConfigList.Count; i++)
            {
                var item = ItemConfigList[i];
                if (item.GetResouceId() == ResourceId.TMMagnet) InnerBoostMagnetItemId = item.id;
                else if (item.GetResouceId() == ResourceId.TMBroom) InnerBoostBroomItemId = item.id;
                else if (item.GetResouceId() == ResourceId.TMWindmill) InnerBoostWindmillItemId = item.id;
                else if (item.GetResouceId() == ResourceId.TMFrozen) InnerBoostFrozenItemId = item.id;
                else if (item.GetResouceId() == ResourceId.TMLighting) OuterBoostLightingItemId = item.id;
                else if (item.GetResouceId() == ResourceId.TMClock) OuterBoostClockItemId = item.id;
            }
            
            for (int i = 0; i < ItemConfigList.Count; i++)
            {
                var item = ItemConfigList[i];
                if (item.GetItemType() == ItemType.TMLightingInfinity) OuterBoostInfinityLightingItemId = item.id;
                else if (item.GetItemType() == ItemType.TMClockInfinity) OuterBoostInfinityClockItemId = item.id;
            }
        }

        public ItemConfig GetItem(int id)
        {
            return ItemConfigList.Find(x => x.id == id);
        }
    }
}