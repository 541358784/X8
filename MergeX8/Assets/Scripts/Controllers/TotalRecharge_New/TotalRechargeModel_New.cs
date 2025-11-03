using System;
using System.Collections.Generic;
using ActivityLocal.DecoBuildReward;
using Decoration;
using Decoration.Bubble;
using DragonPlus;
using DragonPlus.Config.TotalRecharge;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

namespace TotalRecharge_New
{
    public class TotalRechargeModel_New : Manager<TotalRechargeModel_New>
    {
        public TotalRechargeModel_New()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.PURCHASE_SUCCESS_REWARD,OnPaySuccess);
        }
        
        private static TotalRechargeModel_New _instance;
        public static TotalRechargeModel_New Instance => _instance ?? (_instance = new TotalRechargeModel_New());
        private const int Day=7;
        public StorageTotalRechargeNew StorageTotalRechargeNew
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges_New;
            }
        }
   
        public List<TableTotalRechargeNew> TotalRechargeRewards()
        {
            //return GlobalConfigManager.Instance.TableTotalRechargeNewList.FindAll(a=>a.group==StorageTotalRechargeNew.GruopId);
            return GlobalConfigManager.Instance.GatTotalRechargeConfig();
        }

        public TableTotalRechargeNew GetTotalRechargeReward(int id)
        {
            return TotalRechargeRewards().Find(a=>a.id==id);
        }
        // public void OnPaySuccess(TableShop shopCfg)
        // {
        //     if (!IsOpen())
        //         return;
        //     StorageTotalRechargeNew.TotalRecharge += (int)(Mathf.Round(shopCfg.price * 100));
        //     GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTotalRechargeNewScore,StorageTotalRechargeNew.TotalRecharge.ToString(),StorageTotalRechargeNew.GruopId.ToString());
        // }
        
        public void OnPaySuccess(BaseEvent baseEvent)
        {
            if (!IsOpen())
                return;
            
            if (baseEvent.datas == null || baseEvent.datas.Length <= 0)
                return;
            List<ResData> resDatas =(List<ResData>) baseEvent.datas[0];
            foreach (var resData in resDatas)
            {
                if (resData.id ==(int) UserData.ResourceId.Diamond)
                {
                    StorageTotalRechargeNew.TotalRecharge += resData.count;
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTotalRechargeNewScore,StorageTotalRechargeNew.TotalRecharge.ToString(),StorageTotalRechargeNew.GruopId.ToString());
                }
            }
        }
        
        public bool IsClaimed(int id)
        {
            return StorageTotalRechargeNew.CollectGroups.Contains(id);
        }

        public bool IsCanClaim(int id)
        {
            var config = GetTotalRechargeReward(id);
            if (config == null)
                return false;
            return StorageTotalRechargeNew.TotalRecharge >= config.score && !IsClaimed(id);
        }

        public void Claim(int id)
        {
            var config = GetTotalRechargeReward(id);
            if (config == null)
                return ;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTotalRechargeNewReward,config.id.ToString(),StorageTotalRechargeNew.GruopId.ToString());
            
            StorageTotalRechargeNew.CollectGroups.Add(id);
            var resDatas = CommonUtils.FormatReward(config.rewardId, config.rewardNum);
            if (config.decoRewardId != null)
            {
                foreach (var decoId in config.decoRewardId)
                {
                    resDatas.Add(new ResData(decoId, 1, true));
                    DecoBuildRewardManager.Instance.InitDecoBuild(decoId.ToString());

                }
            }
            PopReward(resDatas, config.decoRewardId);
        }

        public bool IsHaveCanClaim()
        {
            var configs = TotalRechargeRewards();
            foreach (var cfg in configs)
            {
                if (IsCanClaim(cfg.id))
                    return true;
            }
            return false;
        }
        
        public void PopReward(List<ResData> listResData, int[] decoReward)
        {
            if (listResData == null || listResData.Count <= 0)
                return;

            if (decoReward != null)
            {
                foreach (var id in decoReward)
                {
                    DecoManager.Instance.UnlockDecoBuilding(id);
                }
            }
            
            int count = listResData.Count > 8 ? 8 : listResData.Count;
            var list = listResData.GetRange(0, count);
            listResData.RemoveRange(0, count);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TotalRechargeRewardGet);
            CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, reasonArgs, animEndCall:
                () =>
                {
                    PopReward(listResData, decoReward);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.BUTTERFLY_WORKSHOP_PURCHASE);

                    if (listResData.Count == 0 && decoReward != null)
                    {
                        UIManager.Instance.CloseUI(UINameConst.UIPopupTotalRecharge_New, true);
                        
                        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                        {
                            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Install,
                                new List<int>(decoReward), null);
                        }
                        else
                        {
                            NodeBubbleManager.Instance.SetBubbleActive(false);
                            
                            DecoManager.Instance.InstallItem(new List<int>(decoReward), () =>
                            {
                                foreach (var id in decoReward)
                                {
                                   var decoItem =  DecoManager.Instance.FindItem(id);
                                   if(decoItem != null)
                                       NodeBubbleManager.Instance.OnLoadBubble(decoItem.Node);
                                }
                            });
                        }
                    }
                });
        }
        public bool IsOpen()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TotalRechargeNew))
            {
                if (StorageTotalRechargeNew.TotalRecharge > 0)
                    return true;

                return false;
            }
            
            if (GetLeftTime() <= 0)
                return false;
            
            return true;
        }

        public long GetLeftTime()
        {
            if (StorageTotalRechargeNew.JoinTime == 0)
                return 0;
            long time=Day * 24 * 60 * 60 * 1000;
            long leftTime=time - ((long)APIManager.Instance.GetServerTime() - StorageTotalRechargeNew.JoinTime);
            return leftTime;
        }
        public string GetLeftTimeString()
        {
            return CommonUtils.FormatLongToTimeStr(GetLeftTime());
        }
         static bool IsActivityStorageEnd()
         {
             return Instance.StorageTotalRechargeNew.JoinTime>0 && Instance.GetLeftTime() <= 0;
         }
   
        public static List<ResData> GetAllUnCollectRewards()
        {
            var unCollectRewardsList = new List<ResData>();
            var configs = Instance.TotalRechargeRewards();
            var storage = StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges_New;
            if (configs!=null&&!storage.IsFinish &&IsActivityStorageEnd())
            {
                for (int j = 0; j < configs.Count; j++)
                {
                    var config = configs[j];
                    if (storage.TotalRecharge >= config.score && !storage.CollectGroups.Contains(config.id))
                    {
                        for (int k = 0; k < config.rewardId.Length; k++)
                        {
                            unCollectRewardsList.Add(new ResData(config.rewardId[k],config.rewardNum[k]));
                        }
                        
                        if (config.decoRewardId != null)
                        {
                            foreach (var decoId in config.decoRewardId)
                            {
                                unCollectRewardsList.Add(new ResData(decoId, 1,true));
                                DecoManager.Instance.UnlockDecoBuilding(decoId);
                                DecoBuildRewardManager.Instance.InitDecoBuild(decoId.ToString());
                            }
                        }
                    }
                   
                }
                storage.IsFinish = true;
            }

            return unCollectRewardsList;
        }

        public static bool CanShowUnCollectRewardsUI()
        {
            var allUnCollectRewards = GetAllUnCollectRewards();
            if (allUnCollectRewards.Count > 0)
            {
                var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TotalRechargeRewardGet};
                var unCollectRewards = allUnCollectRewards;
                
                CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, null, () =>
                {
                    foreach (var reward in unCollectRewards)
                    {
                        if (!UserData.Instance.IsResource(reward.id))
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs()
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonSummerGet,
                                itemAId = reward.id,
                                isChange = true,
                            });
                        }
                    }
                });
                return true;
            }
            return false;
        }
        private static string constPlaceId = "totalRecharge_new";
        public static bool CanShowUI()
        {
            if (CanShowUnCollectRewardsUI())
                return true;
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TotalRechargeNew))
                return false;
            if (Instance.StorageTotalRechargeNew.JoinTime > 0)
                return false;
            Instance.StorageTotalRechargeNew.JoinTime = (long)APIManager.Instance.GetServerTime();
            Instance.StorageTotalRechargeNew.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().TotalRechargeNewGroupId;
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
                return false;
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenUI(UINameConst.UIPopupTotalRecharge_New);
            return true;
        }
        
    }
}