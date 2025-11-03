using System;
using System.Collections.Generic;
using System.Net;
using Activity.CollectStone.Model;
using Activity.JumpGrid;
using Activity.LuckyGoldenEgg;
using Activity.SaveTheWhales;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Framework;
using Game;
using Screw;
using Scripts.UI;
using TMatch;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
    public sealed class UserData : GlobalSystem<UserData>
    {
        // same with global_item config
        public enum ResourceId
        {
            BlueBlock = -105,
            ConnectLine = -104,
            OnePath = -103,
            FishEatFish = -102,
            DigTrench = -101,
            Asmr = -100,
            None = -1,
            Diamond = 101,
            Coin = 102, //装修币
            RareDecoCoin = 103, //特殊装修币
            Exp = 104, //exp
            BagToken = 105, //背包解锁卷
            Energy = 201, //体力
            Infinity_Energy = 202, //无限体力
            HappyGo_Energy = 203, //happyGo体力
            HappyGo_Infinity_Energy = 204, //happyGo无限体力
            NoAds = 301, //去广告
            Fishpond_token = 501, //鱼塘卷
            Hammer=601,// 挖宝锤子
            GoldenEgg=602,// 金蛋
            Seal = 900, //海豹
            Dolphin = 901, //海豚
            Mermaid = 902, //美人鱼
            Capybara = 990, //水豚
            Stone = 800,//石头
            RecoverCoinStar = 903,//回收金币星星
            Easter2024Egg = 904,//复活节蛋
            SnakeLadderTurntable = 905,//蛇梯子转盘
            ThemeDecorationScore = 906,//主题装修积分
            SlotMachineScore = 907,//老虎机Spin次数
            MonopolyDice = 908,//大富翁骰子
            Turntable = 909,//积分轮盘
            TurtlePangPackage = 910,//乌龟对对碰盲盒
            StarrySkyCompassRocket = 926,//星空罗盘火箭
            ZumaBall = 927,//祖玛球
            ZumaBomb = 928,//祖玛炸弹
            ZumaLine = 929,//祖玛万能
            BuyDiamondTicket1 = 932,//一次性钻石券
            KapibalaReborn = 938,//卡皮巴拉死者苏生
            KapibalaLife=939,//卡皮巴拉生命值
            KapiScrewReborn = 940,//卡皮巴拉死者苏生
            KapiScrewLife=941,//卡皮巴拉生命值
            FishCultureScore = 942,//养鱼积分
            KapiTileLife = 943,//卡皮tile生命值
            KapiTileReborn=944,//卡皮tile死者苏生
            PhotoAlbumScore = 945,//相册积分
            JungleAdventure = 946,//丛林探险
            TeamLife=947,//公会生命值
            TeamCoin=948,//公会筹
            PillowWheel = 949,//枕头
            CatchFish = 951,//抓鱼积分
            KeepPetDogFrisbee = 151,//养狗飞盘
            KeepPetDogSteak = 152,//养狗牛排
            KeepPetDogDrumstick = 153,//养狗鸡腿
            KeepPetDogHead = 154,//养狗狗头
            HappyGo = 888, //HappyGo
            Ship = 889, //大船活动
            Theme =90001,//桃乐丝
            CardPackageFreeLevel1=911,//等级1卡包
            CardPackageFreeLevel2=912,//等级2卡包
            CardPackageFreeLevel3=913,//等级3卡包
            CardPackageFreeLevel4=914,//等级4卡包
            CardPackageFreeLevel5=915,//等级5卡包
            CardPackagePayLevel1=916,//等级1卡包
            CardPackagePayLevel2=917,//等级2卡包
            CardPackagePayLevel3=918,//等级3卡包
            CardPackagePayLevel4=919,//等级4卡包
            CardPackagePayLevel5=920,//等级5卡包
            WildCard3=923,//等级3wild卡
            WildCard4=924,//等级4wild卡
            WildCard5=925,//等级5wild卡
            CardPackagePangLevel1=933,//等级1大卡包
            CardPackagePangLevel2=934,//等级2大卡包
            CardPackagePangLevel3=935,//等级3大卡包
            CardPackagePangLevel4=936,//等级4大卡包
            CardPackagePangLevel5=937,//等级5大卡包
            AvatarEaster2024=701,//复活节2024头像
            AvatarSnakeLadder=702,//蛇梯子头像
            AvatarMonopoly=703,//大富翁头像
            AvatarDonut=704,//甜甜圈头像
            AvatarCamping=705,//蛇梯子2(露营主题)头像
            AvatarMonopoly2=706,//大富翁2(露营主题)头像
            AvatarEaster2024_2=707,//复活节2024_2头像
            AvatarHalloween = 708,//万圣节(蛇梯子2)头像
            AvatarSpaceDog = 709,//太空狗(大富翁2)头像
            AvatarDonut2 = 710,//圣诞节(甜甜圈2(复活节3))头像
            AvatarMonopoly3 = 711,//圣诞节(甜甜圈2(复活节3))头像
            AvatarSnakeLadder3 = 712,//圣诞节(甜甜圈2(复活节3))头像
            AvatarEaster2025=713,//复活节2025头像
            AvatarNvZhu=714,//vip动态头像
            AvatarMonopoly4=715,//大富翁4
            AvatarSnakeLadder4=716,//蛇梯棋4
            AvatarEaster2024_4=717,//蛇梯棋4
            AvatarMonopoly5=718,//大富翁4
            ExtraOrderRewardCouponCoin1_5Free_30	=851	,//金币*1.5(免费30)30分钟
            ExtraOrderRewardCouponCoin1_5Free_60	=852	,//金币*1.5(免费60)60分钟
            ExtraOrderRewardCouponCoin1_5Pay_30	=853	,//金币*1.5(付费30)30分钟
            ExtraOrderRewardCouponCoin1_5Pay_60	=854	,//金币*1.5(付费60)60分钟
            ExtraOrderRewardCouponCoin2Free_30	=855	,//金币*2(免费30)30分钟
            ExtraOrderRewardCouponCoin2Free_60	=856	,//金币*2(免费60)60分钟
            ExtraOrderRewardCouponCoin2Pay_30	=857	,//金币*2(付费30)30分钟
            ExtraOrderRewardCouponCoin2Pay_60	=858	,//金币*2(付费60)60分钟
            ExtraOrderRewardCouponBanana2Free_30	=861	,//香蕉*2(免费30)30分钟
            ExtraOrderRewardCouponBanana2Free_60	=862	,//香蕉*2(免费60)60分钟
            ExtraOrderRewardCouponBanana2Pay_30	=863	,//香蕉*2(付费30)30分钟
            ExtraOrderRewardCouponBanana2Pay_60	=864	,//香蕉*2(付费60)60分钟
            ExtraOrderRewardCouponDog2Free_30	=871	,//狗粮*2(免费30)30分钟
            ExtraOrderRewardCouponDog2Free_60	=872	,//狗粮*2(免费60)60分钟
            ExtraOrderRewardCouponDog2Pay_30	=873	,//狗粮*2(付费30)30分钟
            ExtraOrderRewardCouponDog2Pay_60	=874	,//狗粮*2(付费60)60分钟
            ExtraOrderRewardCouponTurntable2Free_30	=881	,//转盘*2(免费30)30分钟
            ExtraOrderRewardCouponTurntable2Free_60	=882	,//转盘*2(免费60)60分钟
            ExtraOrderRewardCouponTurntable2Pay_30	=883	,//转盘*2(付费30)30分钟
            ExtraOrderRewardCouponTurntable2Pay_60	=884	,//转盘*2(付费60)60分钟
            ExtraOrderRewardCouponThemeDecoration2Free_30	=891	,//主题装修*2(免费30)30分钟
            ExtraOrderRewardCouponThemeDecoration2Free_60	=892	,//主题装修*2(免费60)60分钟
            ExtraOrderRewardCouponThemeDecoration2Pay_30	=893	,//主题装修*2(付费30)30分钟
            ExtraOrderRewardCouponThemeDecoration2Pay_60	=894	,//主题装修*2(付费60)60分钟
            
            KeepPetClue_1 = 161,//宠物线索1
            KeepPetClue_2 = 162,//宠物线索2
            KeepPetClue_3 = 163,//宠物线索3
            KeepPetClue_4 = 164,//宠物线索4
            KeepPetClue_5 = 165,//宠物线索5
            KeepPetClue_6 = 166,//宠物线索6
            
            GardenShovel = 930,//花园铲子
            GardenBomb = 931,//花园炸弹
            
            Water = 950,//水滴
            
            MixMasterCoffee = 960,//调制咖啡
            MixMasterTea = 961,//调制茶
            MixMasterMilk = 962,//调制牛奶
            MixMasterLemonJuice = 963,//调制柠檬汁
            MixMasterIceCream = 964,//调制冰激凌
            MixMasterCream = 965,//调制奶油
            MixMasterPearl = 966,//调制珍珠
            MixMasterSugar = 967,//调制糖
            MixMasterIce = 968,//调制冰
            MixMasterExtra1 = 969,//调制冰
            
            BlindBox1 = 751,//盲盒主题1
            BlindBox2 = 752,//盲盒主题2
            BlindBox3 = 753,//盲盒主题3
            BlindBox4 = 754,//盲盒主题4
            BlindBox5 = 755,//盲盒主题5
            BlindBox6 = 756,//盲盒主题6
            BlindBox7 = 757,//盲盒主题7
            BlindBox8 = 758,//盲盒主题8
            BlindBox49 = 799,//盲盒主题49
            
            Farm_Exp = 30, //农场经验
            Farm_Wood = 9991,// 木头-仓库
            Farm_Brush= 9992,// 刷子-仓库
            Farm_Brick= 9993,// 砖头-仓库
            Farm_Nail= 9994,// 钉子-仓库
            Farm_NFertilizer = 9911, //化肥-地块
            Farm_SFertilizer = 9912, //超级化肥-地块
            Farm_NKettle = 9921, //水壶-果树
            Farm_SKettle = 9922, //超级水壶-果树
            Farm_Gear = 9931, //齿轮-机器
            Farm_Clock = 9941, //怀表-动物
            
            Prop_Back = 211, //道具撤回
            Prop_SuperBack = 212, //道具超级撤回
            Prop_Magic = 213, //道具魔法棒
            Prop_Shuffle = 214, //道具洗牌
            Prop_Extend = 215, //道具格子+1
            
        }

        private StorageHome _storageHome;

        private StorageHome storageHome
        {
            get
            {
                if (_storageHome == null)
                {
                    _storageHome = StorageManager.Instance.GetStorage<StorageHome>();
                }

                return _storageHome;
            }
        }

        public StorageDictionary<int, int> Bag
        {
            get { return storageHome.Bag; }
        }

        public bool CanAford(ResData cost)
        {
            return (cost == null) || CanAford((UserData.ResourceId) cost.id, cost.count);
        }

        public bool CanAford(List<ResData> costList)
        {
            foreach (var cost in costList)
            {
                if (!CanAford(cost)) return false;
            }

            return true;
        }
        public bool CanAford(int id, int cout)
        {
            return GetRes((ResourceId)id) >= cout;
        }
        
        public bool CanAford(ResourceId id, int cout)
        {
            return GetRes(id) >= cout;
        }
        public bool CanAford(ResourceId id, int cout,out int needCount)
        {
            var hasCount = GetRes(id);
            if (hasCount >= cout)
            {
                needCount = 0;
            }
            else
            {
                needCount = cout - hasCount;
            }
            return hasCount >= cout;
        }

        public void ConsumeRes(ResourceId resourceId, int count, GameBIManager.ItemChangeReasonArgs reason)
        {
            var currentCount = Instance.GetRes(resourceId);
            var costCount = count;
            if (resourceId == ResourceId.Infinity_Energy || resourceId == ResourceId.Energy)
            {
                EnergyModel.Instance.CostEnergy(count, reason);
            }
            else if (resourceId == ResourceId.HappyGo_Infinity_Energy || resourceId == ResourceId.HappyGo_Energy)
            {
                HappyGoEnergyModel.Instance.CostEnergy(count, reason);
            }
            else
            {
                SetRes(resourceId, currentCount - costCount, reason);
                EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.Consume, (int)resourceId, count);
                EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.Consume, (int)resourceId, count);
            }
            EventDispatcher.Instance.SendEventImmediately(new EventUserDataConsumeRes(resourceId, count));
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.UserDataUpdate, resourceId);
        }

        public void ConsumeRes(ResData cost, GameBIManager.ItemChangeReasonArgs reason)
        {
            ConsumeRes((UserData.ResourceId) cost.id, cost.count, reason);
        }

        public void ConsumeRes(List<ResData> costList, GameBIManager.ItemChangeReasonArgs reason)
        {
            foreach (var cost in costList)
            {
                ConsumeRes(cost, reason);
            }
        }

        public bool IsFarmRes(int id)
        {
            return id >= (int)ResourceId.Farm_Wood && id <= (int)ResourceId.Farm_Nail || id >= (int)ResourceId.Farm_NFertilizer && id <= (int)ResourceId.Farm_Clock;
        }

        public bool IsFarmExp(int id)
        {
            return id == (int)ResourceId.Farm_Exp;
        }

        public bool IsFarmProp(int id)
        {
            return id >= (int)ResourceId.Farm_NFertilizer && id <= (int)ResourceId.Farm_Clock;
        }
        public void AddRes(int id, int count, GameBIManager.ItemChangeReasonArgs reason, bool isEvent = true,
            ShowRewardType rewardType = ShowRewardType.Common,bool isIgnore=false, bool inFirst = false)
        {
            if (IsFarmRes(id))
            {
                FarmModel.Instance.AddProductItem(id, count);
            }
            else if (TMatchModel.Instance.IsTMatchResId(id))
            {
                ItemModel.Instance.Add(TMatchModel.Instance.ChangeToTMatchId(id), count, reason, isEvent);
            }
            else if (ScrewGameModel.Instance.IsScrewResId(id))
            {
                Screw.UserData.UserData.Instance.AddRes(ScrewGameModel.Instance.ChangeToScrewId(id), count, reason, isEvent);
            }
            else if (IsResource(id))
            {
                AddRes((UserData.ResourceId) id, count, reason, isEvent);
                var resourceId = (ResourceId) id;
                if (resourceId != ResourceId.Energy && 
                    resourceId != ResourceId.Infinity_Energy && 
                    resourceId != ResourceId.HappyGo_Energy && 
                    resourceId != ResourceId.HappyGo_Infinity_Energy && 
                    resourceId != ResourceId.Coin && 
                    resourceId != ResourceId.RecoverCoinStar)
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.GetRes, id, count);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.GetRes, id, count);
                }
                if (id == (int) ResourceId.Fishpond_token)
                {
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.AddFishpondToken, count, reason);   
                }
                if ((id == (int) ResourceId.Coin || id == (int) ResourceId.RecoverCoinStar) && !isIgnore)
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.GetRes, (int) ResourceId.Coin, count);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.GetRes, (int) ResourceId.Coin, count);
                    if (id == (int) ResourceId.Coin)
                    {
                        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.AddCoin, count, reason); 
                    }
                    if (id == (int) ResourceId.RecoverCoinStar)
                    {
                        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.AddRecoverCoinStar, count, reason);   
                    } 
                
                    if (CoinCompetitionModel.Instance.IsOpened() && CoinCompetitionModel.Instance.IsShowStart())
                    {
                        XUtility.WaitSeconds(1.2f,()=>
                        {
                            CoinCompetitionModel.Instance.AddScore(count);
                        });
                    }
                    
                    if (JumpGridModel.Instance.IsOpened() && JumpGridModel.Instance.IsShowStart())
                    {
                        XUtility.WaitSeconds(1.2f,()=>
                        {
                            JumpGridModel.Instance.AddScore(count);
                        });
                    }
                }
                EventDispatcher.Instance.SendEventImmediately(new EventUserDataAddRes((UserData.ResourceId) id, count));
                CommonResourceLeaderBoardModel.Instance.OnAddRes((UserData.ResourceId) id,count);
            }
            else
            {
                // if (id == (int) ResourceId.CardPackageFreeLevel1 ||
                //     id == (int) ResourceId.CardPackageFreeLevel2 ||
                //     id == (int) ResourceId.CardPackageFreeLevel3 ||
                //     id == (int) ResourceId.CardPackageFreeLevel4 ||
                //     id == (int) ResourceId.CardPackageFreeLevel5 ||
                //     id == (int) ResourceId.CardPackagePayLevel1 ||
                //     id == (int) ResourceId.CardPackagePayLevel2 ||
                //     id == (int) ResourceId.CardPackagePayLevel3 ||
                //     id == (int) ResourceId.CardPackagePayLevel4 ||
                //     id == (int) ResourceId.CardPackagePayLevel5)
                // {
                //     var packageConfig = CardCollectionModel.Instance.ThemeInUse.GetCardPackageConfig((ResourceId)id);
                //     if (packageConfig != null)
                //     {
                //         CardCollectionModel.Instance.AddCardPackage(packageConfig.id,count);
                //     }
                //     else
                //     {
                //         var exchangeResources = CardCollectionModel.Instance.ExchangeCardPackageResource((ResourceId)id,count);
                //         AddRes(exchangeResources,reason);
                //     }
                // }
                // else if (id == (int) ResourceId.WildCard3 ||
                //          id == (int) ResourceId.WildCard4 ||
                //          id == (int) ResourceId.WildCard5)
                // {
                //     CardCollectionModel.Instance.AddWildCardResources((ResourceId)id, count);
                // }
                // else 
                 if (rewardType == ShowRewardType.HappyGo)
                {
                    if (DecoWorld.ItemLib.ContainsKey(id))
                    {
                        DecoManager.Instance.UnlockDecoBuilding(id);
                    }
                    else
                    {
                        var mergeItem = MergeManager.Instance.GetEmptyItem();
                        mergeItem.Id = id;
                        mergeItem.State = 1;
                        MergeManager.Instance.AddRewardItem(mergeItem, MergeBoardEnum.HappyGo,count);
                    }
                    if (isEvent)
                        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
                }
                else
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = id;
                    mergeItem.State = 1;
                    MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main, count, inFirst);
                    if (isEvent)
                        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                }
            }
        }

        private void AddRes(ResourceId resourceId, int count, GameBIManager.ItemChangeReasonArgs reason,
            bool isEvent = true )
        {
            switch (resourceId)
            {
                case ResourceId.Infinity_Energy:
                {
                    EnergyModel.Instance.AddEnergyUnlimitedTime((int) count * 1000, reason);
                    break;
                }
                case ResourceId.Energy:
                {
                    EnergyModel.Instance.AddEnergy(count, reason, false, isEvent);
                    break;
                }
                case ResourceId.HappyGo_Infinity_Energy:
                {
                    HappyGoEnergyModel.Instance.AddEnergyUnlimitedTime((int) count * 1000, reason);
                    break;
                }
                case ResourceId.HappyGo_Energy:
                {
                    HappyGoEnergyModel.Instance.AddEnergy(count, reason, false, isEvent);
                    break;
                }
                case ResourceId.CardPackageFreeLevel1:
                case ResourceId.CardPackageFreeLevel2:
                case ResourceId.CardPackageFreeLevel3:
                case ResourceId.CardPackageFreeLevel4:
                case ResourceId.CardPackageFreeLevel5:
                case ResourceId.CardPackagePayLevel1:
                case ResourceId.CardPackagePayLevel2:
                case ResourceId.CardPackagePayLevel3:
                case ResourceId.CardPackagePayLevel4:
                case ResourceId.CardPackagePayLevel5:
                case ResourceId.CardPackagePangLevel1:
                case ResourceId.CardPackagePangLevel2:
                case ResourceId.CardPackagePangLevel3:
                case ResourceId.CardPackagePangLevel4:
                case ResourceId.CardPackagePangLevel5:
                {
                    if (CardCollectionActivityModel.Instance.IsInitFromServer())
                    {
                        var themeState = CardCollectionModel.Instance.GetCardThemeState(CardCollectionActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme();
                        var packageConfig = themeState.GetCardPackageConfig(resourceId);
                        CardCollectionModel.Instance.AddCardPackage(packageConfig.Id,count,reason.reason.ToString());
                    }
                    else
                    {
                        CardCollectionModel.Instance.SaveAbstractCardPackage((int)resourceId,count,reason.reason.ToString());
                        // var exchangeResources = CardCollectionModel.Instance.ExchangeCardPackageResource(resourceId,count);
                        // if (exchangeResources != null)
                        //     AddRes(exchangeResources,reason);
                    }
                    // var packageConfig = CardCollectionModel.Instance.ThemeInUse.GetCardPackageConfig(resourceId);
                    // if (packageConfig != null)
                    // {
                    //     CardCollectionModel.Instance.AddCardPackage(packageConfig.Id,count,reason.reason.ToString());
                    // }
                    // else
                    // {
                    //     var exchangeResources = CardCollectionModel.Instance.ExchangeCardPackageResource(resourceId,count);
                    //     AddRes(exchangeResources,reason);
                    // }
                    break;
                }
                case ResourceId.WildCard3:
                case ResourceId.WildCard4:
                case ResourceId.WildCard5:
                {
                    CardCollectionModel.Instance.AddWildCardResources(resourceId, count,reason.reason.ToString());
                    break;
                }
                case ResourceId.ExtraOrderRewardCouponCoin1_5Free_30:
                case ResourceId.ExtraOrderRewardCouponCoin1_5Free_60:
                case ResourceId.ExtraOrderRewardCouponCoin1_5Pay_30:
                case ResourceId.ExtraOrderRewardCouponCoin1_5Pay_60:
                case ResourceId.ExtraOrderRewardCouponCoin2Free_30:
                case ResourceId.ExtraOrderRewardCouponCoin2Free_60:
                case ResourceId.ExtraOrderRewardCouponCoin2Pay_30:
                case ResourceId.ExtraOrderRewardCouponCoin2Pay_60:
                case ResourceId.ExtraOrderRewardCouponBanana2Free_30:
                case ResourceId.ExtraOrderRewardCouponBanana2Free_60:
                case ResourceId.ExtraOrderRewardCouponBanana2Pay_30:
                case ResourceId.ExtraOrderRewardCouponBanana2Pay_60:
                case ResourceId.ExtraOrderRewardCouponDog2Free_30:
                case ResourceId.ExtraOrderRewardCouponDog2Free_60:
                case ResourceId.ExtraOrderRewardCouponDog2Pay_30:
                case ResourceId.ExtraOrderRewardCouponDog2Pay_60:
                case ResourceId.ExtraOrderRewardCouponTurntable2Free_30:
                case ResourceId.ExtraOrderRewardCouponTurntable2Free_60:
                case ResourceId.ExtraOrderRewardCouponTurntable2Pay_30:
                case ResourceId.ExtraOrderRewardCouponTurntable2Pay_60:
                case ResourceId.ExtraOrderRewardCouponThemeDecoration2Free_30:
                case ResourceId.ExtraOrderRewardCouponThemeDecoration2Free_60:
                case ResourceId.ExtraOrderRewardCouponThemeDecoration2Pay_30:
                case ResourceId.ExtraOrderRewardCouponThemeDecoration2Pay_60:
                {
                    ExtraOrderRewardCouponModel.Instance.AddCoupon(resourceId,count,reason.reason.ToString());
                    break;
                }
                case ResourceId.Stone:
                {
                    CollectStoneModel.Instance.AddStone(count);
                    break;
                }
                case ResourceId.AvatarEaster2024:
                {
                    HeadIconUtils.GetAvatar(6);
                    break;
                }
                case ResourceId.AvatarSnakeLadder:
                {
                    HeadIconUtils.GetAvatar(7);
                    break;
                }
                case ResourceId.AvatarMonopoly:
                {
                    HeadIconUtils.GetAvatar(8);
                    break;
                }  
                case ResourceId.AvatarDonut:
                {
                    HeadIconUtils.GetAvatar(9);
                    break;
                }
                case ResourceId.AvatarCamping:
                {
                    HeadIconUtils.GetAvatar(10);
                    break;
                }
                case ResourceId.AvatarMonopoly2:
                {
                    HeadIconUtils.GetAvatar(11);
                    break;
                }  
                case ResourceId.AvatarEaster2024_2:
                {
                    HeadIconUtils.GetAvatar(12);
                    break;
                }
                case ResourceId.AvatarHalloween:
                {
                    HeadIconUtils.GetAvatar(13);
                    break;
                }
                case ResourceId.AvatarSpaceDog:
                {
                    HeadIconUtils.GetAvatar(14);
                    break;
                }
                case ResourceId.AvatarDonut2:
                {
                    HeadIconUtils.GetAvatar(15);
                    break;
                }
                case ResourceId.AvatarMonopoly3:
                {
                    HeadIconUtils.GetAvatar(16);
                    break;
                }  
                case ResourceId.AvatarSnakeLadder3:
                {
                    HeadIconUtils.GetAvatar(17);
                    break;
                }  
                case ResourceId.AvatarEaster2025:
                {
                    HeadIconUtils.GetAvatar(18);
                    break;
                }
                case ResourceId.AvatarNvZhu:
                {
                    HeadIconUtils.GetAvatar(19);
                    break;
                }
                case ResourceId.AvatarMonopoly4:
                {
                    HeadIconUtils.GetAvatar(20);
                    break;
                }
                case ResourceId.AvatarSnakeLadder4:
                {
                    HeadIconUtils.GetAvatar(21);
                    break;
                }
                case ResourceId.AvatarEaster2024_4:
                {
                    HeadIconUtils.GetAvatar(22);
                    break;
                }
                case ResourceId.AvatarMonopoly5:
                {
                    HeadIconUtils.GetAvatar(23);
                    break;
                }
                case ResourceId.Hammer:
                {
                    TreasureHuntModel.Instance.AddHammer(count);
                    break;
                }
                case ResourceId.GoldenEgg:
                {
                    LuckyGoldenEggModel.Instance.AddGoldenEgg(count);
                    break;
                }
                case ResourceId.KeepPetClue_1:
                case ResourceId.KeepPetClue_2:
                case ResourceId.KeepPetClue_3:
                case ResourceId.KeepPetClue_4:
                case ResourceId.KeepPetClue_5:
                case ResourceId.KeepPetClue_6:
                {
                    KeepPetModel.Instance.GetClue((int)resourceId);
                    break;
                }
                default:
                {
                    SetRes(resourceId, GetRes(resourceId) + count, reason);
                    break;
                }
            }
        }

        public void AddRes(ResData cost, GameBIManager.ItemChangeReasonArgs reason, bool isEvent = true,
            ShowRewardType rewardType = ShowRewardType.Common, bool inFirst = false)
        {
            AddRes(cost.id, cost.count, reason, isEvent, rewardType, inFirst:inFirst);
        }

        public void AddRes(List<ResData> costList, GameBIManager.ItemChangeReasonArgs reason, bool isEvent = true)
        {
            foreach (var cost in costList)
            {
                AddRes(cost, reason);
            }
        }

        public int GetTotalDecoCoin()
        {
            return storageHome.TotalDecoCoin;
        }

        public int GetRes(ResourceId resourceId)
        {
            if (BlindBoxModel.Instance.IsBlindBoxId((int) resourceId))
            {
                return BlindBoxModel.Instance.GetBlindBoxCount((int)resourceId);
            }
            else
            {
                switch (resourceId)
                {
                    case ResourceId.RareDecoCoin:
                    case ResourceId.Diamond:
                    case ResourceId.Coin:
                    {
                        string key = GetCurrencyKey(resourceId);
                        if (storageHome.Currency.TryGetValue(key, out var number))
                        {
                            return number.GetValue();
                        }

                        break;
                    }
                    case ResourceId.Exp:
                    {
                        return storageHome.Exp;
                        break;
                    } 
                    case ResourceId.BagToken:
                    {
                        return storageHome.BagToken;
                        break;
                    }     
                    case ResourceId.Hammer:
                    {
                        return TreasureHuntModel.Instance.GetHammer();
                        break;
                    }  
                    case ResourceId.GoldenEgg:
                    {
                        return LuckyGoldenEggModel.Instance.GetGoldenEgg();
                        break;
                    }
                    case UserData.ResourceId.Mermaid:
                        return MermaidModel.Instance.GetScore();
                        break;
                    case UserData.ResourceId.Easter2024Egg:
                        return Easter2024Model.Instance.GetEgg();
                        break;   
                    case UserData.ResourceId.Water:
                        return SaveTheWhalesModel.Instance.GetWater();
                        break;
                    case UserData.ResourceId.SnakeLadderTurntable:
                        return SnakeLadderModel.Instance.GetTurntableCount();
                        break;
                    case UserData.ResourceId.ThemeDecorationScore:
                        return ThemeDecorationModel.Instance.GetScore();
                        break;
                    case UserData.ResourceId.SlotMachineScore:
                        return SlotMachineModel.Instance.GetScore();
                        break;
                    case UserData.ResourceId.Turntable:
                        return TurntableModel.Instance.GetCoin();
                        break;
                    case UserData.ResourceId.MonopolyDice:
                        return MonopolyModel.Instance.GetDiceCount();
                        break;
                    case UserData.ResourceId.TurtlePangPackage:
                        return TurtlePangModel.Instance.GetPackageCount();
                        break;
                    case UserData.ResourceId.StarrySkyCompassRocket:
                        return StarrySkyCompassModel.Instance.GetRocketCount();
                        break;
                    case UserData.ResourceId.ZumaBall:
                        return ZumaModel.Instance.GetBallCount();
                        break;
                    case UserData.ResourceId.ZumaBomb:
                        return ZumaModel.Instance.GetBombCount();
                        break;
                    case UserData.ResourceId.ZumaLine:
                        return ZumaModel.Instance.GetLineCount();
                        break;
                    case UserData.ResourceId.BuyDiamondTicket1:
                        return BuyDiamondTicketModel.Instance.GetTicketCount((int)resourceId);
                        break;
                    case UserData.ResourceId.KapibalaReborn:
                        return KapibalaModel.Instance.GetRebornCount();
                        break;
                    case UserData.ResourceId.KapibalaLife:
                        return KapibalaModel.Instance.GetLife();
                        break;
                    case UserData.ResourceId.KapiScrewReborn:
                        return KapiScrewModel.Instance.GetRebornCount();
                        break;
                    case UserData.ResourceId.KapiScrewLife:
                        return KapiScrewModel.Instance.GetLife();
                        break;
                    case UserData.ResourceId.KapiTileReborn:
                        return KapiTileModel.Instance.GetRebornCount();
                        break;
                    case UserData.ResourceId.KapiTileLife:
                        return KapiTileModel.Instance.GetLife();
                        break;
                    case UserData.ResourceId.FishCultureScore:
                        return FishCultureModel.Instance.GetScore();
                        break;
                    case UserData.ResourceId.PhotoAlbumScore:
                        return PhotoAlbumModel.Instance.GetScore();
                        break;
                    case UserData.ResourceId.KeepPetDogFrisbee:
                        return KeepPetModel.Instance.GetDogFrisbee();
                        break;
                    case UserData.ResourceId.KeepPetDogDrumstick:
                        return KeepPetModel.Instance.GetDogDrumstick();
                        break;
                    case UserData.ResourceId.KeepPetDogSteak:
                        return KeepPetModel.Instance.GetDogSteak();
                        break;
                    case UserData.ResourceId.KeepPetDogHead:
                        return KeepPetModel.Instance.GetDogHead();
                        break;
                    case UserData.ResourceId.RecoverCoinStar:
                        return RecoverCoinModel.Instance.GetStar();
                        break;
                    case UserData.ResourceId.Energy:
                        return EnergyModel.Instance.EnergyNumber();
                        break;
                    case UserData.ResourceId.Infinity_Energy:
                        return EnergyModel.Instance.EnergyUnlimitedLeftTime();
                        break; 
                    case UserData.ResourceId.HappyGo_Energy:
                        return HappyGoEnergyModel.Instance.EnergyNumber();
                        break;
                    case UserData.ResourceId.HappyGo_Infinity_Energy:
                        return HappyGoEnergyModel.Instance.EnergyUnlimitedLeftTime();
                        break;
                    case UserData.ResourceId.TeamLife:
                        return TeamManager.Instance.GetLife();
                        break;
                    case UserData.ResourceId.TeamCoin:
                        return TeamManager.Instance.GetCoin();
                        break;
                    case UserData.ResourceId.PillowWheel:
                        return PillowWheelModel.Instance.GetItem();
                        break;
                    case UserData.ResourceId.CatchFish:
                        return CatchFishModel.Instance.GetItem();
                        break;
                    case ResourceId.Stone:
                    {
                        return CollectStoneModel.Instance.GetStone();
                    }
                    case ResourceId.ExtraOrderRewardCouponCoin1_5Free_30:
                    case ResourceId.ExtraOrderRewardCouponCoin1_5Free_60:
                    case ResourceId.ExtraOrderRewardCouponCoin1_5Pay_30:
                    case ResourceId.ExtraOrderRewardCouponCoin1_5Pay_60:
                    case ResourceId.ExtraOrderRewardCouponCoin2Free_30:
                    case ResourceId.ExtraOrderRewardCouponCoin2Free_60:
                    case ResourceId.ExtraOrderRewardCouponCoin2Pay_30:
                    case ResourceId.ExtraOrderRewardCouponCoin2Pay_60:
                    case ResourceId.ExtraOrderRewardCouponBanana2Free_30:
                    case ResourceId.ExtraOrderRewardCouponBanana2Free_60:
                    case ResourceId.ExtraOrderRewardCouponBanana2Pay_30:
                    case ResourceId.ExtraOrderRewardCouponBanana2Pay_60:
                    case ResourceId.ExtraOrderRewardCouponDog2Free_30:
                    case ResourceId.ExtraOrderRewardCouponDog2Free_60:
                    case ResourceId.ExtraOrderRewardCouponDog2Pay_30:
                    case ResourceId.ExtraOrderRewardCouponDog2Pay_60:
                    case ResourceId.ExtraOrderRewardCouponTurntable2Free_30:
                    case ResourceId.ExtraOrderRewardCouponTurntable2Free_60:
                    case ResourceId.ExtraOrderRewardCouponTurntable2Pay_30:
                    case ResourceId.ExtraOrderRewardCouponTurntable2Pay_60:
                    case ResourceId.ExtraOrderRewardCouponThemeDecoration2Free_30:
                    case ResourceId.ExtraOrderRewardCouponThemeDecoration2Free_60:
                    case ResourceId.ExtraOrderRewardCouponThemeDecoration2Pay_30:
                    case ResourceId.ExtraOrderRewardCouponThemeDecoration2Pay_60:
                    {
                        return ExtraOrderRewardCouponModel.Instance.GetLeftCoupon(resourceId);
                    }
                    case UserData.ResourceId.MixMasterCoffee:
                    case UserData.ResourceId.MixMasterTea:
                    case UserData.ResourceId.MixMasterMilk:
                    case UserData.ResourceId.MixMasterLemonJuice:
                    case UserData.ResourceId.MixMasterIceCream:
                    case UserData.ResourceId.MixMasterCream:
                    case UserData.ResourceId.MixMasterPearl:
                    case UserData.ResourceId.MixMasterSugar:
                    case UserData.ResourceId.MixMasterIce:
                    case UserData.ResourceId.MixMasterExtra1:
                    {
                        MixMasterModel.Instance.GetMaterialCount((int)resourceId);
                        break;
                    }
                    default:
                    {
                        return Bag.ContainsKey((int) resourceId) ? Bag[(int) resourceId] : 0;
                    }
                }   
            }

            return 0;
        }

        public void SetRes(ResourceId resourceId, int count, GameBIManager.ItemChangeReasonArgs reason)
        {
            var current = GetRes(resourceId);
            bool ignore = false;
            if (BlindBoxModel.Instance.IsBlindBoxId((int) resourceId))
            {
                BlindBoxModel.Instance.AddBlindBox((int) resourceId, count - current);
            }
            else
            {
                switch (resourceId)
                {
                    case ResourceId.RareDecoCoin:
                    case ResourceId.Diamond:
                    case ResourceId.Coin:
                    {
                        string key = GetCurrencyKey(resourceId);
                        count = Mathf.Max(0, count);
                        if (storageHome.Currency.ContainsKey(key))
                        {
                            storageHome.Currency[key].SetValue(count);
                        }
                        else
                        {
                            StorageCurrency storageCoin = new StorageCurrency();
                            storageCoin.SetValue(count);
                            storageHome.Currency.Add(key, storageCoin);
                        }

                        break;
                    }
                    case UserData.ResourceId.Mermaid:
                        MermaidModel.Instance.AddScore(count-current);
                        break;
                    case UserData.ResourceId.Easter2024Egg:
                        Easter2024Model.Instance.AddEgg(count-current,reason.reason.ToString());
                        break;     
                    case UserData.ResourceId.Water: 
                        SaveTheWhalesModel.Instance.AddWater(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.SnakeLadderTurntable:
                        SnakeLadderModel.Instance.AddTurntable(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.ThemeDecorationScore:
                        ThemeDecorationModel.Instance.AddScore(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.SlotMachineScore:
                        SlotMachineModel.Instance.AddScore(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.Turntable:
                        TurntableModel.Instance.SetCoin(count-current, reason.reason.ToString());
                        break;
                    case UserData.ResourceId.MonopolyDice:
                        MonopolyModel.Instance.AddDice(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.TurtlePangPackage:
                        TurtlePangModel.Instance.AddPackage(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.StarrySkyCompassRocket:
                        StarrySkyCompassModel.Instance.AddRocket(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.ZumaBall:
                        ZumaModel.Instance.AddBall(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.ZumaBomb:
                        ZumaModel.Instance.AddBomb(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.ZumaLine:
                        ZumaModel.Instance.AddLine(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.BuyDiamondTicket1:
                        BuyDiamondTicketModel.Instance.OnCollectTicket((int)resourceId);
                        break;
                    case UserData.ResourceId.KapibalaReborn:
                        KapibalaModel.Instance.AddRebornItem(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KapibalaLife:
                        KapibalaModel.Instance.AddLife(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KapiScrewReborn:
                        KapiScrewModel.Instance.AddRebornItem(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KapiScrewLife:
                        KapiScrewModel.Instance.AddLife(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KapiTileReborn:
                        KapiTileModel.Instance.AddRebornItem(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KapiTileLife:
                        KapiTileModel.Instance.AddLife(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.Stone:
                        CollectStoneModel.Instance.AddStone(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.FishCultureScore:
                        FishCultureModel.Instance.AddScore(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.PhotoAlbumScore:
                        PhotoAlbumModel.Instance.AddScore(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.PillowWheel:
                        PillowWheelModel.Instance.AddItem(count-current,reason);
                        break;
                    case UserData.ResourceId.CatchFish:
                        CatchFishModel.Instance.AddItem(count-current,reason);
                        break;
                    case UserData.ResourceId.KeepPetDogFrisbee:
                        KeepPetModel.Instance.AddDogFrisbee(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KeepPetDogDrumstick:
                        KeepPetModel.Instance.AddDogDrumstick(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KeepPetDogSteak:
                        KeepPetModel.Instance.AddDogSteak(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.KeepPetDogHead:
                        KeepPetModel.Instance.AddDogHead(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.TeamLife:
                        TeamManager.Instance.AddLife(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.TeamCoin:
                        TeamManager.Instance.AddCoin(count-current,reason.reason.ToString());
                        break;
                    case UserData.ResourceId.RecoverCoinStar:
                        RecoverCoinModel.Instance.AddStar(count-current);
                        break;
                    case UserData.ResourceId.Energy:
                        if (count > current)
                            EnergyModel.Instance.AddEnergy((int) count-current, reason);
                        else if(count < current)
                            EnergyModel.Instance.CostEnergy((int)current-count, reason);
                        ignore = true;
                        break;  
                    case UserData.ResourceId.Infinity_Energy:
                        EnergyModel.Instance.AddEnergyUnlimitedTime((int) count * 1000, reason);
                        ignore = true;
                        break;
                    case UserData.ResourceId.HappyGo_Energy:
                        HappyGoEnergyModel.Instance.AddEnergy((int) count, reason);
                        ignore = true;
                        break;  
                    case UserData.ResourceId.HappyGo_Infinity_Energy:
                        HappyGoEnergyModel.Instance.AddEnergyUnlimitedTime((int) count * 1000, reason);
                        ignore = true;
                        break;
                    case UserData.ResourceId.MixMasterCoffee:
                    case UserData.ResourceId.MixMasterTea:
                    case UserData.ResourceId.MixMasterMilk:
                    case UserData.ResourceId.MixMasterLemonJuice:
                    case UserData.ResourceId.MixMasterIceCream:
                    case UserData.ResourceId.MixMasterCream:
                    case UserData.ResourceId.MixMasterPearl:
                    case UserData.ResourceId.MixMasterSugar:
                    case UserData.ResourceId.MixMasterIce:
                    case UserData.ResourceId.MixMasterExtra1:
                    {
                        MixMasterModel.Instance.AddMaterial((int)resourceId,count-current,true);
                        break;
                    }
                    case ResourceId.Exp:
                    {
                        count = Mathf.Max(0, count);
                        storageHome.Exp = count;
                        break;
                    }  
                    case ResourceId.BagToken:
                    {
                        count = Mathf.Max(0, count);
                        storageHome.BagToken = count;
                        break;
                    }
                    default:
                    {
                        int intId = (int) resourceId;
                        if (Bag.ContainsKey(intId))
                        {
                            Bag[intId] = Mathf.Max(0, count);
                        }
                        else
                        {
                            Bag.Add(intId, Mathf.Max(0, count));
                        }

                        count = Bag[intId];
                        // EventData data = new EventData();
                        // data.Integers = new []{intId};
                        // EventManager.Instance.GlobalDispatcher.DispatchEvent(GlobalEvent.GLOBAL_BAG_CHANGED,data);
                        // EventDispatcher.Instance.DispatchEventImmediately(EventEnum.UserDataUpdate, resourceId);
                        break;
                    }
                }   
            }

            if (ignore == false)
            {
                GameBIManager.Instance.SendItemChangeEvent(resourceId, (count - current), (ulong) GetRes(resourceId),
                    reason);
            }
        }


        public bool IsResource(int resourceId)
        {
            if (resourceId == (int)ResourceId.Asmr)
                return false;
            if (resourceId == (int)ResourceId.DigTrench)
                return false;
            if (resourceId == (int)ResourceId.FishEatFish)
                return false;
            if (resourceId == (int)ResourceId.OnePath)
                return false;
            if (resourceId == (int)ResourceId.ConnectLine)
                return false;
            if (resourceId == (int) ResourceId.BlueBlock)
                return false;
            // if (resourceId == (int) ResourceId.CardPackageFreeLevel1 ||
            //     resourceId == (int) ResourceId.CardPackageFreeLevel2 ||
            //     resourceId == (int) ResourceId.CardPackageFreeLevel3 ||
            //     resourceId == (int) ResourceId.CardPackageFreeLevel4 ||
            //     resourceId == (int) ResourceId.CardPackageFreeLevel5 ||
            //     resourceId == (int) ResourceId.CardPackagePayLevel1 ||
            //     resourceId == (int) ResourceId.CardPackagePayLevel2 ||
            //     resourceId == (int) ResourceId.CardPackagePayLevel3 ||
            //     resourceId == (int) ResourceId.CardPackagePayLevel4 ||
            //     resourceId == (int) ResourceId.CardPackagePayLevel5)
            //     return false;
            // if (resourceId == (int) ResourceId.WildCard3 ||
            //     resourceId == (int) ResourceId.WildCard4 ||
            //     resourceId == (int) ResourceId.WildCard5)
            //     return false;
            
            foreach (ResourceId item in Enum.GetValues(typeof(ResourceId)))
            {
                if ((int) item == resourceId)
                    return true;
            }

            return false;
        }

        public static string ParseRewardNumText(int resourceId, int num)
        {
            return ParseRewardNumText((ResourceId) resourceId, num);
        }

        public static string ParseRewardNumText(ResourceId resourceId, int num)
        {
            return $"{num}";
        }


        static public string GetResourceName(ResourceId resouceId)
        {
            return LocalizationManager.Instance.GetLocalizedString($"UI_boost_name_{(int) resouceId}");
        }

        static public string GetResourceDesc(ResourceId resouceId)
        {
            return LocalizationManager.Instance.GetLocalizedString($"UI_boost_desc_{(int) resouceId}");
        }

        public enum ResourceSubType
        {
            Small,
            Big,
            Reward
        }

        public static Sprite GetResourceIcon(int resourceId, ResourceSubType subType = ResourceSubType.Small)
        {
            if(UserData.Instance.IsResource(resourceId))
                return GetResourceIcon((ResourceId) resourceId, subType);
  
            
            TableMergeItem tableMerge = GameConfigManager.Instance.GetItemConfig(resourceId);
            if (tableMerge != null)
                return MergeConfigManager.Instance.mergeIcon.GetSprite(tableMerge.image);
            
            if (TMatchModel.Instance.IsTMatchResId(resourceId))
                return TMatch.ItemModel.Instance.GetItemSprite(TMatchModel.Instance.ChangeToTMatchId(resourceId), false);
            if (ScrewGameModel.Instance.IsScrewResId(resourceId))
                return Screw.UserData.UserData.GetResourceIcon(ScrewGameModel.Instance.ChangeToScrewId(resourceId));

            var farmConfig = FarmConfigManager.Instance.GetFarmProductConfig(resourceId);
            if (farmConfig != null)
                return FarmModel.Instance.GetFarmIcon(farmConfig.Image);
            
            return null;
        }

        public static string GetResourceIconName(int resouceId, ResourceSubType subType = ResourceSubType.Small)
        {
            var tableItem = GlobalConfigManager.Instance.GetTableItem((int) resouceId);
            if (tableItem == null)
                return null;

            string fileName;
            switch (subType)
            {
                case ResourceSubType.Big:
                    fileName = tableItem.imageBig;
                    break;
                case ResourceSubType.Small:
                    fileName = tableItem.image;
                    break;
                case ResourceSubType.Reward:
                    fileName = tableItem.imageReward;
                    break;
                default:
                    fileName = tableItem.image;
                    break;
            }

            if (string.IsNullOrEmpty(fileName))
                fileName = tableItem.image;

            return fileName;
        }

        public static Sprite GetResourceIcon(ResourceId resouceId, ResourceSubType subType = ResourceSubType.Small)
        {
            try
            {
                string fileName = GetResourceIconName((int) resouceId, subType);
                Sprite goodsIcon = GetResourceIcon(fileName);
                return goodsIcon;
            }
            catch (System.Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e.Message);
                DragonU3DSDK.DebugUtil.LogError($"ResourceId:" + resouceId);
            }

            return null;
        }

        public static Sprite GetResourceIcon(string fileName)
        {
            try
            {
                Sprite goodsIcon = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, fileName);
                return goodsIcon;
            }
            catch (System.Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e.Message);
                DragonU3DSDK.DebugUtil.LogError($"fileName:" + fileName);
            }

            return null;
        }

        public string GetCurrencyKey(ResourceId resId)
        {
            return "currency_" + (int) resId;
        }
    }
}
