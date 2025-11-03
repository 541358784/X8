using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus.Config.ClimbTower;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace ActivityLocal.ClimbTower.Model
{
    public class ClimbTowerModel : Manager<ClimbTowerModel>
    {
        public enum GameState
        {
            None=-1,
            Free,
            Fail,
            Finish,
            Cout,
        }
        
        public enum OpenState
        {
            None = -1,
            Close,
            Open
        }
        
        public StorageClimbTower ClimbTower
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().ClimbTower; }
        }

        private GameObject _shopItem = null;
        protected override void InitImmediately()
        {
            InvokeRepeating("InvokeUpdate", 0, 1);
            
            _shopItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/ActivityLocal/ClimbTower/ShopItem");
        }

        private void InvokeUpdate()
        {
            if(CommonUtils.IsSameDay((ulong)ClimbTower.RefreshTime, APIManager.Instance.GetServerTime()))
                return;

            if (ClimbTower.LevelId == 0)
            {
                RestClimbTower();
            }
            if (ClimbTower.State == (int)GameState.Finish)
            {
                RestClimbTower();
            }
            ClimbTower.RefreshTime = (long)APIManager.Instance.GetServerTime();
            ClimbTower.FreeTimes = 1;
            ClimbTower.PayTimes = 1;
        }

        public int LevelId(bool isPay)
        {
            if (ClimbTower.LevelId == 0)
                ClimbTower.LevelId = ClimbTowerConfigManager.Instance.GetRewardConfig(isPay).First().Id;

            return ClimbTower.LevelId;
        }
        
        public void RestClimbTower()
        {
            long refresh = ClimbTower.RefreshTime;
            int freeTimes = ClimbTower.FreeTimes;
            int payTimes = ClimbTower.PayTimes;
            bool isPay = ClimbTower.IsPay;
            bool isPayLevel = ClimbTower.IsPayLevel;
            ClimbTower.Clear();
            ClimbTower.RefreshTime = refresh;
            ClimbTower.PayLevel = PayLevelModel.Instance.GetCurPayLevelConfig().ClimbTowerGroupId;
            ClimbTower.Stage = 0;
            ClimbTower.IsPay = isPay;
            ClimbTower.FreeTimes = freeTimes;
            ClimbTower.PayTimes = payTimes;
            ClimbTower.IsPayLevel = isPayLevel;
            ClimbTower.State = (int)GameState.Free;
            ClimbTower.LevelId = ClimbTowerConfigManager.Instance.GetRewardConfig(isPay).First().Id;
        }
        
        public int PlayLevel()
        {
            return ClimbTower.PayLevel;
        }

        public bool IsOpen()
        {
            return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ClimbTower);
        }

        public bool IsCanEnter()
        {
            if (!IsOpen())
                return false;

            if (!ClimbTower.IsPayLevel && ClimbTower.FreeTimes <= 0 && ClimbTower.State == (int)GameState.Finish)
                return false;

            if (ClimbTower.IsPayLevel && ClimbTower.PayTimes <= 0 && ClimbTower.State == (int)GameState.Finish)
                return false;
            
            return true;
        }

        public bool IsCanPay()
        {
            if (ClimbTower.IsPayLevel)
                return false;
            
            if (ClimbTower.PayTimes <= 0 && ClimbTower.State == (int)GameState.Finish)
                return false;
            
            if (ClimbTower.FreeTimes <= 0 && ClimbTower.State != (int)GameState.Finish)
                return false;

            if (ClimbTower.FreeTimes > 0)
                return false;
            
            return true;
        }
        
        public void InitShopEntry(Transform parent)
        {
            if(_shopItem == null)
                return;

            var cloneItem = GameObject.Instantiate(_shopItem);
            
            cloneItem.transform.SetParent(parent);
            cloneItem.transform.localScale = Vector3.one;
            cloneItem.transform.localPosition = Vector3.zero;

            cloneItem.AddComponent<ClimbTowerShopItem>();
        }

        public void GetRewardCache(bool isPay, out List<int> rewardIds, out List<int> rewardNums, out List<int> openState)
        {
            int levelIdCache = ClimbTower.LevelIdCache;
            int levelId = LevelId(isPay);

            if (levelId > ClimbTowerConfigManager.Instance.GetRewardConfig(isPay).Last().Id)
            {
                rewardIds = ClimbTower.RewardIdCache;
                rewardNums = ClimbTower.RewardNumCache;
                openState = ClimbTower.OpenStateCache;
                return;
            }
            
            if (levelId != levelIdCache)
            {
                CreateRewardCache(isPay);
            }

            rewardIds = ClimbTower.RewardIdCache;
            rewardNums = ClimbTower.RewardNumCache;
            openState = ClimbTower.OpenStateCache;
        }

        public void CreateRewardCache(bool isPay)
        {
            var config = ClimbTowerConfigManager.Instance.GetRewardConfig(isPay).Find(a => a.Id == LevelId(isPay));
            if (config == null)
                config = ClimbTowerConfigManager.Instance.GetRewardConfig(isPay).First();

            ClimbTower.LevelIdCache = LevelId(isPay);
            
            ClimbTower.RewardIdCache.Clear();
            ClimbTower.RewardNumCache.Clear();
            ClimbTower.OpenStateCache.Clear();
            ClimbTower.RewardIndexCache.Clear();
            
            for (int i = 0; i < config.Weight.Count; i++)
            {
                ClimbTower.RewardIdCache.Add(-10);
                ClimbTower.RewardNumCache.Add(0);
                ClimbTower.OpenStateCache.Add((int)OpenState.Close);
                ClimbTower.RewardIndexCache.Add(-1);
            }
        }

        public void FillRewardCache(int index, bool isPay, int levelId, out int rewardId, out int rewardNum)
        {
            rewardId = 102;
            rewardNum = 1;
            
            if(index < 0 || index >= ClimbTower.RewardIdCache.Count)
                return;

            if (ClimbTower.RewardIdCache[index] == -10)
            {
                var config = ClimbTowerConfigManager.Instance.GetRewardConfig(isPay).Find(a => a.Id == levelId);
                if (config == null)
                    config = ClimbTowerConfigManager.Instance.GetRewardConfig(isPay).First();
                
                List<int> weights = new List<int>();
                List<int> rewardIds = new List<int>();
                List<int> rewardNums = new List<int>();
                List<int> rewardIndex = new List<int>();

                for (var i = 0; i < config.Weight.Count; i++)
                {
                    var indexCache = ClimbTower.RewardIndexCache.FindIndex(a => a == i);
                    if(indexCache >= 0)
                        continue;
                    
                    weights.Add(config.Weight[i]);
                    rewardIds.Add(config.RewardItem[i]);
                    rewardNums.Add(config.RewardNum[i]);
                    rewardIndex.Add(i);
                }
                
                
                var randomIndex = CommonUtils.RandomIndexByWeight(weights);

                ClimbTower.RewardIdCache[index] = rewardIds[randomIndex];
                ClimbTower.RewardNumCache[index] = rewardNums[randomIndex];
                ClimbTower.RewardIndexCache[index] = rewardIndex[randomIndex];
            }
            
            rewardId = ClimbTower.RewardIdCache[index];
            rewardNum = ClimbTower.RewardNumCache[index];
        }

        public void RecordOpenState(int index, OpenState state)
        {
            if(index < 0 || index >= ClimbTower.OpenStateCache.Count)
                return;

            ClimbTower.OpenStateCache[index] = (int)state;
        }

        public void SetGameState(GameState state)
        {
            ClimbTower.State = (int)state;
        }

        public void AddClimReward(int id, int num)
        {
            var resData = ClimbTower.Rewards.Find(a => a.Id == id);
            if (resData != null)
            {
                resData.Count += num;
            }
            else
            {
                ClimbTower.Rewards.Add(new StorageResData());
                ClimbTower.Rewards.Last().Id = id;
                ClimbTower.Rewards.Last().Count = num;
            }
        }

        public StorageResData GatClimReward(int id)
        {
            return ClimbTower.Rewards.Find(a => a.Id == id);
        }

        public void PurchaseSuccess(TableShop shopConfig)
        {
            RestClimbTower();
            
            ClimbTower.IsPay = true;
            ClimbTower.IsPayLevel = true;

            ClimbTower.PayTimes--;

            UIManager.Instance.CloseUI(UINameConst.UIPopupClimbTowerPay, true);
            UIManager.Instance.CloseUI(UINameConst.UIClimbTowerMain, true);
            UIManager.Instance.OpenWindow(UINameConst.UIClimbTowerMainPay);
        }
    }
}