using System;
using System.Collections.Generic;
using ABTest;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Logic;
using Gameplay;
using UnityEngine;

namespace Farm.Model
{
    public enum FarmType
    {
        None=-1,
        Ground,
        Tree,
        Machine,
        Animal,
    }

    public enum FarmProductStatus
    {
        None,
        Free, //空闲
        Producing, //生产中
        Finish //完成 可领取
    }
    
    public enum SeedType
    {
        Veggie=1,
        Fruit
    }
    
    public partial class FarmModel : Singleton<FarmModel>
    {
        public const string FarmIconAtlasName = "FarmIconAtlas";
        
        public StorageFarm storageFarm
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageFarm>();
            }
        }

        private GameObjectPoolManager _poolManager = new GameObjectPoolManager("FarmObjectPoolRoot");
        
        public bool Debug_OpenModule;
        public bool Debug_CompleteAllOrder = false;
        public bool Debug_OpenFram = false;
        
        public void Release()
        {
            _bubbleLogics.Clear();
            _groundSeeds.Clear();
            RemoveAllLogic();
            _poolManager.Release();
            _isAddListener = false;
            EventDispatcher.Instance.RemoveEventListener(EventEnum.STORY_MOVIE_FINISH, StoryMovieFinish);
        }

        public bool IsFarmModel()
        {
            return DecoManager.Instance.CurrentWorld != null && DecoManager.Instance.CurrentWorld.Id == 2;
        }
        
        public bool CanUnLockNode(DecoNode node)
        {
            if (!FarmConfigManager.Instance.IsLinkDecoNode(node.Id))
            {
                return node.SuggestTest();
            }

            return storageFarm.Level >= FarmConfigManager.Instance.GetLinkDecoNodeUnLockLevel(node.Id) && node.SuggestTest();
        }

        public bool IsUnLockNode(DecoNode node)
        {
            if (!FarmConfigManager.Instance.IsLinkDecoNode(node.Id))
                return true;

            return storageFarm.Level >= FarmConfigManager.Instance.GetLinkDecoNodeUnLockLevel(node.Id);
        }
        
        public int GetLevel()
        {
            if (storageFarm.Level <= 0)
                storageFarm.Level = 1;
            
            return storageFarm.Level;
        }

        public bool HavEnoughProduct(int id, int num)
        {
            if (!storageFarm.ProductItems.ContainsKey(id))
                return false;

            return storageFarm.ProductItems[id] >= num;
        }

        public void AddProductItem(int itemId, int itemNum)
        {
            if (!storageFarm.ProductItems.ContainsKey(itemId))
                storageFarm.ProductItems[itemId] = 0;

            storageFarm.ProductItems[itemId] += itemNum;
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_REFRESH_PRODUCT, itemId, storageFarm.ProductItems[itemId]);
        }

        public void ConsumeProductItem(int itemId, int itemNum)
        {
            if (!storageFarm.ProductItems.ContainsKey(itemId))
                return;

            int num = storageFarm.ProductItems[itemId] - itemNum;
            num = Math.Max(num, 0);
            
            storageFarm.ProductItems[itemId] = num;
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_REFRESH_PRODUCT, itemId, num);
        }
        
        public int GetProductItemNum(int itemId)
        {
            if (!storageFarm.ProductItems.ContainsKey(itemId))
                return 0;

            return storageFarm.ProductItems[itemId];
        }
        
        public Sprite GetFarmIcon(string name)
        {
            return  ResourcesManager.Instance.GetSpriteVariant(FarmIconAtlasName, name);
        }

        public void AddExp(int exp, bool isSendEvent =true)
        {
            storageFarm.Exp += exp;
            
            foreach (var config in FarmConfigManager.Instance.TableFarmLevelList)
            {
                if(config.Id < storageFarm.Level)
                    continue;
                
                if (storageFarm.Exp < config.LevelExp)
                    break;
                
                AddExpReward(config);
                storageFarm.Level++;
                storageFarm.Exp -= config.LevelExp;
                GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Farm_Exp, config.LevelExp, (ulong)storageFarm.Exp, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TaskRewardFarm,
                    data1 = "LevelUp",
                });
                SendBi_LevelUp();
            }
        }

        public bool IsMaxLevel()
        {
            return GetLevel() > FarmConfigManager.Instance.TableFarmLevelList[FarmConfigManager.Instance.TableFarmLevelList.Count - 1].Id;
        }

        public void AddExpReward(TableFarmLevel config)
        {
            for (int i = 0; i < config.RewardIds.Count; i++)
            {
                int id = config.RewardIds[i];
                int num = config.RewardNums[i];

                if (UserData.Instance.IsFarmProp(id) && FarmConfigManager.Instance.TableFarmProductList.Find(a => a.Id == id) != null)
                {
                   AddProductItem(id, num);
                   
                   GameBIManager.Instance.SendItemChangeEvent((UserData.ResourceId)id, num, (ulong)FarmModel.Instance.GetProductItemNum(id), new GameBIManager.ItemChangeReasonArgs()
                   {
                       reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.LevelUpFarm,
                       data1 = id.ToString(),
                   });
                }
                else
                {
                    UserData.Instance.AddRes(id, num, new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.LevelUpFarm,
                    });
                }
            }
        }

        public bool IsUnLock()
        {
            if (!storageFarm.IsEnter)
            {
                if (UserData.Instance.GetRes(UserData.ResourceId.Coin) < 50)
                    return false;
            }
            
            return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Farm) && (ABTestManager.Instance.IsOpenFarmGame() || Debug_OpenFram);
        }

        public bool HaveFinishProduct()
        {
            if (!IsUnLock())
                return false;

            if (HaveFinishMachine())
                return true;

            if (HaveFinishAnimal())
                return true;

            if (HaveFinishGround())
                return true;

            if (HaveFinishTree())
                return true;

            return false;
        }
    }
}