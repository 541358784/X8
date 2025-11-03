using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Gameplay.UI.UpdateRewardManager
{
    public class UpdateRewardManager : Singleton<UpdateRewardManager>
    {
        private readonly string _Key = "UpdateRd_1.0.10";
        
        public StorageHome StorageHome
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>();
            }
        }
        
        public bool IsGetUpdateReward()
        {
            return StorageHome.AbTestConfig.ContainsKey(_Key);
        }

        public void GetUpdateReward()
        {
            if(StorageHome.AbTestConfig.ContainsKey(_Key))
                return;
            
            StorageHome.AbTestConfig.Add(_Key, _Key);
        }
        
        public void Init()
        {
            if (!StorageHome.IsFirstLogin)
            {
                GetUpdateReward();
            }
        }
        
        public static bool CanShow()
        {
            if (Instance.IsGetUpdateReward())
                return false;

            Instance.GetUpdateReward();
            
            int id = GlobalConfigManager.Instance.GetNumValue("1_0_10_reward");
            ResData resData = new ResData(id, 1);
            
            UserData.Instance.AddRes(resData, new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.DecoDay), false);
            if (!UserData.Instance.IsResource(id))
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonUpgrade,
                    itemAId = id,
                    isChange = true,
                });
            }

            List<ResData> rewards = new List<ResData>();
            rewards.Add(resData);
            UIPopupRewardItemController.Show(rewards);
            
            return true;
        }
    }
}