using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public enum BiWorkType
        {
            Generate,
            GainProduct
        }
        
        public void SendBI_FarmWork(BiWorkType type, int id, int productId)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFarmWork, id.ToString(), type.ToString(), productId.ToString());
        }   
        public void SendBI_CreateOrder(StorageFarmOrderItem order)
        {
            Dictionary<string, string> extras = new Dictionary<string, string>();
            extras.Add("slot", order.Slot.ToString());
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFarmAppearTask, order.Id, GetOrderRewardBiInfo(order), GetOrderNeedBiInfo(order),extras);
        }

        public void SendBI_FinishOrder(StorageFarmOrderItem order)
        {
            Dictionary<string, string> extras = new Dictionary<string, string>();
            extras.Add("slot", order.Slot.ToString());
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFarmFinishTask, order.Id, GetOrderRewardBiInfo(order), GetOrderNeedBiInfo(order), extras);
        }
        
        public void SendBi_LevelUp()
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFarmLevelUp, GetLevel().ToString());
        }
        private string GetOrderRewardBiInfo(StorageFarmOrderItem order)
        {
            string biInfo = "";
            for (var i = 0; i < order.RewardIds.Count; i++)
            {
                int id = order.RewardIds[i];
                int num = order.RewardNums[i];

                biInfo += id + "," + num;

                if (i <= order.RewardIds.Count - 2)
                    biInfo += ";";
            }

            return biInfo;
        }
        
        private string GetOrderNeedBiInfo(StorageFarmOrderItem order)
        {
            string biInfo = "";
            for (var i = 0; i < order.NeedItemIds.Count; i++)
            {
                int id = order.NeedItemIds[i];
                int num = order.NeedItemNums[i];

                biInfo += id + "," + num;

                if (i <= order.NeedItemIds.Count - 2)
                    biInfo += ";";
            }

            return biInfo;
        }
    }
}