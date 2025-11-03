using System.Collections.Generic;
using Deco.Node;
using Difference;
using DragonPlus;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Farm.Model;
using Gameplay;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Decoration.DaysManager
{
    public class DaysManager : Singleton<DaysManager>
    {
        public StorageDays StorageDays
        {
            get { return StorageManager.Instance.GetStorage<StorageDecoration>().Days; }
        }

        public List<TableDays> DaysConfigs
        {
            get { return DecorationConfigManager.Instance.DaysConfigs; }
        }

        public int DayNum
        {
            get { return StorageDays.DayNum; }
            private set { StorageDays.DayNum = value; }
        }

        public int DayStep
        {
            get { return StorageDays.DayStep; }
            private set { StorageDays.DayStep = value; }
        }
        
        public int TotalNodeNum
        {
            get { return StorageDays.TotalNodeNum; }
            private set{ StorageDays.TotalNodeNum = value; }
        }

        public bool InitDays
        {
            get { return StorageDays.InitDays; }
        }

        public bool CompleteFixData
        {
            get;
            set;
        }
        
        public bool CanShowRetrieveReward()
        {
            var resDatas = HaveRetrieveReward();
            if (resDatas == null)
                return false;

            return true;
        }

        public void FixDays()
        {
            if(StorageDays.IsFixDays)
                return;

            StorageDays.IsFixDays = true;
            CalculationTotalNode();
            CalculationDays();
            NewDay();
        }
        
        private List<ResData> HaveRetrieveReward()
        {
            if (StorageDays.InitDays)
            {
                FixDays();
                return null;
            }

            StorageDays.InitDays = true;
            
            CalculationTotalNode();
            CalculationDays();
            NewDay();
            
            var rewards = CalculationRetrieveReward();
            if (rewards == null)
                return null;
            
            foreach (var res in rewards)
            {
                UserData.Instance.AddRes(res, new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.DecoDay), false);

                if (!UserData.Instance.IsResource(res.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDecoDay,
                        itemAId = res.id,
                        isChange = true,
                    });
                }
            }

            if (rewards != null && rewards.Count > 0)
                UIPopupRewardItemController.Show(rewards);
            
            return rewards;
        }

        public void OwnNode(DecoNode node)
        {
            if(FarmModel.Instance.IsFarmModel())
                return;
            
            if(node == null)
                return;
            
            if(node.Config == null)
                return;
            
            if(node.Config.costId != (int)UserData.ResourceId.Coin)
                return;

            TotalNodeNum++;
            DayStep++;

            var config = GetDayConfig();
            if(config == null)
                return;
            
            if(DayStep <= config.nodeNumber-1)
                return;

            DayNum++;
            DayStep = -1;
        }

        public bool NewDay()
        {
            if (FarmModel.Instance.IsFarmModel())
                return false;
            
            var config = GetDayConfig();
            if(config == null)
                return false;

            if (DayStep >= config.nodeNumber - 1)
            {
                DayNum++;
                DayStep = -1;
                return true;
            }

            return false;
        }
        
        private void CalculationTotalNode()
        {
            StorageDays.TotalNodeNum = 0;
            
            DecoManager.Instance.CurrentWorld._areaList.ForEach(a =>
            {
                foreach (var stage in a.Storage.StagesData)
                {
                    foreach (var node in stage.Value.NodesData)
                    {
                        if(node.Value.Status < (int)DecoNode.Status.Owned)
                            continue;

                        DecoNode dcoNode = DecoManager.Instance.FindNode(node.Value.Id);
                        if(dcoNode == null)
                            continue;
                        
                        if(dcoNode._stage.Area.Config.hideAreaInDeco)
                            continue;
                        
                        if(dcoNode.Config.costId != (int)UserData.ResourceId.Coin)
                            continue;

                        StorageDays.TotalNodeNum++;
                    }
                }
            });
        }

        private void CalculationDays()
        {
            StorageDays.DayNum = 0;
            StorageDays.DayStep = -1;

            if(TotalNodeNum == 0)
                return;
            
            int totalNode = 0;
            for (int i = 0; i < DaysConfigs.Count; i++)
            {
                totalNode += DaysConfigs[i].nodeNumber;
                
                if(totalNode < TotalNodeNum)
                    continue;

                StorageDays.DayNum = i;
                break;
            }

            StorageDays.DayStep = DaysConfigs[DayNum].nodeNumber - (totalNode - TotalNodeNum) - 1;
        }

        private List<ResData> CalculationRetrieveReward()
        {
            if (DayNum == 0 && DayStep < 0)
                return null;

            if (DayNum < 0 || DayNum >= DaysConfigs.Count)
                return null;

            List<ResData> resDatas = new List<ResData>();
            for (int i = 0; i <= DayNum; i++)
            {
                TableDays config = DaysConfigs[i];
                if(config.rewardIndex == null || config.rewardIndex.Length == 0)
                    continue;

                if(config.retrieveRewardId == null || config.retrieveRewardId.Length == 0)
                    continue;
                
                if (i < DayNum)
                {
                    for(int j = 0; j < config.retrieveRewardId.Length; j++)
                        resDatas.Add(new ResData(config.retrieveRewardId[j], config.retrieveRewardNum[j]));
                }
                else
                {
                    for (int j = 0; j < config.rewardIndex.Length; j++)
                    {
                        if(DayStep < 0)
                            break;
                    
                        if(config.rewardIndex[j]-1 > DayStep)
                            break;

                        if (config.rewardIndex[j] - 1 == DayStep)
                        {
                            if(j < config.retrieveRewardId.Length)
                                resDatas.Add(new ResData(config.retrieveRewardId[j], config.retrieveRewardNum[j]));
                        }
                    }
                }
            }

            return resDatas;
        }

        public TableDays GetDayConfig()
        {
            if (DayNum < 0 || DayNum >= DaysConfigs.Count)
                return null;

            return DaysConfigs[DayNum];
        }

        public List<ResData> GetDayStepReward(DecoNode node, bool autoAdd = false)
        {
            if(node == null)
                return null;
            
            if(node.Config == null)
                return null;
            
            if(node.Config.costId != (int)UserData.ResourceId.Coin)
                return null;
            
            TableDays config = GetDayConfig();
            if (config == null)
                return null;

            if (config.rewardIndex == null || config.rewardIndex.Length == 0)
                return null;

            List<ResData> resDatas = GetDayStepRewardByIndex(config, DayStep);
            if (resDatas == null)
                return null;
            
            if (resDatas.Count > 0 && autoAdd)
            {
                string key = $"{DayNum}_{DayStep}";
                //if (!StorageDays.GetRewardState.ContainsKey(key))
                {
                    //StorageDays.GetRewardState.Add(key, true);

                    bool inFirst = false;
                    if (DifferenceManager.Instance.IsDiffPlan_New())
                    {
                        if (DayNum == 0 && DayStep == 6)
                            inFirst = true;
                    }
                    
                    foreach (var res in resDatas)
                    {
                        UserData.Instance.AddRes(res, new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.DecoDay), false, inFirst:inFirst);

                        if (!UserData.Instance.IsResource(res.id))
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDecoDay,
                                itemAId = res.id,
                                isChange = true,
                                data1 = key,
                            });
                        }
                    }
                }
            }
            return resDatas;
        }
    
        public List<ResData> GetDayStepRewardByIndex(TableDays config, int index)
        {
            if (config == null)
                return null;

            if (config.rewardIndex == null || config.rewardIndex.Length == 0)
                return null;

            if (index < 0)
                return null;
        
            List<ResData> resDatas = new List<ResData>();
            for (int i = 0; i < config.rewardIndex.Length; i++)
            {
                if(config.rewardIndex[i]-1 != index)
                    continue;

                if(config.rewardIndex[i]-1 > index)
                    break;

                string[] rewardId = config.rewardId;
                if (DifferenceManager.Instance.IsDiffPlan_New())
                {
                    rewardId = config.planb_rewardId == null ? config.rewardId : config.planb_rewardId;
                }
                
                if(i >= rewardId.Length)
                    break;
                
                string[] rws = rewardId[i].Split(';');
                string[] num = config.rewardNum[i].Split(';');

                for (int j = 0; j < rws.Length; j++)
                {
                    resDatas.Add(new ResData(int.Parse(rws[j]), int.Parse(num[j])));
                }
            }

            return resDatas;
        }
    
        public int GetDayTotalNodes()
        {
            int totalNode = 0;
            for (int i = 0; i < DaysConfigs.Count; i++)
            {
                if(DayNum <= i)
                    break;
                
                totalNode += DaysConfigs[i].nodeNumber;
            }

            return totalNode;
        }
    }
}