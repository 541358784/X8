using System.Collections.Generic;
using Decoration;
using DragonPlus.ConfigHub.Ad;
using Farm.Model;
using Merge.Order;
using UnityEngine;

public static class UnlockManager
{
    public static int debug_OrderTaskOpenLevel = -1;

    private static Dictionary<MergeUnlockType, string> _unlockKeys = new Dictionary<MergeUnlockType, string>()
    {
        {MergeUnlockType.GameStore, "shop_unlock"},
        {MergeUnlockType.BubbleOpen, "Bubble_unlock"},
        {MergeUnlockType.Bag, "package_unlock"},
        {MergeUnlockType.CustomerGift, "customer_gift_unlock"},
        {MergeUnlockType.Balloon, "balloon_unlock"},
        {MergeUnlockType.ItemSell, "item_sell_unlock"},
        {MergeUnlockType.DailyTask, "daily_task_unlock"},
        {MergeUnlockType.LikeUs, "LikeUsUnlockLevel"},
        {MergeUnlockType.RateUs, "RateUsUnlockLevel"},
        {MergeUnlockType.GiftBagLink, "gift_bag_link_unlock"},
        {MergeUnlockType.MasterCard, "masterCard_unlock"},
        {MergeUnlockType.Bundle, "sotre_bundle_unlock"},
        {MergeUnlockType.TaskAssist, "store_task_assist"},
        {MergeUnlockType.IceBreak, "icebreak_pack_unlock"},
        {MergeUnlockType.PigBank, "piggybank_unlock"},
        {MergeUnlockType.SuperLoginGift, "superlogingift_unlock"},
        {MergeUnlockType.DailyBonus, "daily_login"},
        {MergeUnlockType.DailyRank, "daily_rank_unlock"},
        {MergeUnlockType.DogHope, "dogHope_unlock"},
        {MergeUnlockType.ClimbTree, "climbTree_unlock"},
        {MergeUnlockType.DailyPack, "daily_pack_unlock"},
        {MergeUnlockType.EnergyPack, "energy_pack_unlock"},
        {MergeUnlockType.DescPurchase, "item_desc_purchase_unlock"},
        {MergeUnlockType.DailyRv, "tv_reward"},
        {MergeUnlockType.FlashSale, "flash_sale_unlock"},
        {MergeUnlockType.TaskCenter, "taskcenter_unlock"},
        {MergeUnlockType.PayRebate, "payrebate_unlock"},
        {MergeUnlockType.PayRebateLocal, "payrebatelocal_unlock"},
        {MergeUnlockType.GarageCleanup, "garagecleanup_unlock"},
        {MergeUnlockType.Easter, "easter_unlock"},
        {MergeUnlockType.BattlePass, "battle_pass_unlock"},
        {MergeUnlockType.EnergyTorrent, "energy_frenzy_level"},
        {MergeUnlockType.Mermaid, "mermaid_unlock"},
        {MergeUnlockType.CoinRush, "coin_rush_unlock"},
        {MergeUnlockType.CoinCompetition, "coin_competition_unlock"},
        {MergeUnlockType.SummerWatermelon, "summer_watermelon_unlock"},
        {MergeUnlockType.SummerWatermelonBread, "summer_watermelon_bread_unlock"},
        {MergeUnlockType.CoinLeaderBoard, "coin_leader_board_unlock"},
        {MergeUnlockType.SeaRacing, "sea_racing_unlock"},
        {MergeUnlockType.HappyGo, "happy_go_unlock"},
        {MergeUnlockType.NewDailyPack, "new_daily_pack_unlock"},
        {MergeUnlockType.CardCollection, "card_collection_unlock"},
        {MergeUnlockType.ThreeGift, "three_gift_unlock"},
        {MergeUnlockType.Easter2024, "easter_2024_unlock"},
        {MergeUnlockType.TimeOrder, "time_order_unlock"},
        {MergeUnlockType.GiftBagBuyBetter, "gift_bag_buy_better_unlock"},
        {MergeUnlockType.GiftBagSendOne, "gift_bag_send_one_unlock"},
        {MergeUnlockType.LimitOrderLine, "limit_order_unlock"},
        {MergeUnlockType.SnakeLadder, "snake_ladder_unlock"},
        {MergeUnlockType.ShopExtraReward, "shop_extra_reward"},
        {MergeUnlockType.ThemeDecoration, "theme_decoration"},
        {MergeUnlockType.SlotMachine, "slot_machine"},
        {MergeUnlockType.WeeklyCard, "weekly_card"},
        {MergeUnlockType.Monopoly, "monopoly"},
        {MergeUnlockType.CrazeOrder, "crazeorder"},
        {MergeUnlockType.TreasureMap, "treasuremap"},
        {MergeUnlockType.KeepPetDog,"keep_pet_dog"},
        {MergeUnlockType.TreasureHunt,"treasurehunt"},
        {MergeUnlockType.ExtraEnergy,"extraenergy"},
        {MergeUnlockType.OptionalGift,"optionalgift"},
        {MergeUnlockType.GiftBagProgress,"giftbagprogress"},
        {MergeUnlockType.Matreshkas,"matreshkas"},
        {MergeUnlockType.ButterflyWorkShop,"butterflyworkshop"},
        {MergeUnlockType.Turntable,"turntable_unlock"},
        {MergeUnlockType.DiamondReward,"diamondreward_unlock"},
        {MergeUnlockType.GiftBagDouble,"giftbagdoubl_unlock"},
        {MergeUnlockType.GardenTreasure,"gardentreasure_unlock"},
		{MergeUnlockType.MixMaster,"mix_master_unlock"},
        {MergeUnlockType.SaveTheWhales,"savethewhalse_unlock"},
        {MergeUnlockType.TurtlePang,"turtle_pang_unlock"},
        {MergeUnlockType.StarrySkyCompass,"starry_sky_compass_unlock"},
        {MergeUnlockType.BlindBox,"blind_box_unlock"},
        {MergeUnlockType.TotalRecharge,"total_recharge_unlock"},
        {MergeUnlockType.TotalRechargeNew,"new_recharge_unlock"},
        {MergeUnlockType.NewDailyPackageExtraReward,"new_daily_package_extra_reward_unlock"},
        {MergeUnlockType.Zuma,"zuma_unlock"},
        {MergeUnlockType.Kapibala,"kapibala_unlock"},
        {MergeUnlockType.Farm,"farm_unlock"},
        {MergeUnlockType.KapiScrew,"kapi_screw_unlock"},
        {MergeUnlockType.DogPlayUnlockLevel,"dog_play_unlock_level"},
        {MergeUnlockType.DogPlayUnlockKeepPetLevel,"dog_play_unlock_dog_evel"},
        {MergeUnlockType.FishCulture,"fish_culture_unlock_level"},
        {MergeUnlockType.DogPlayExtraReward,"dog_play_extra_reward_unlock_level"},
        {MergeUnlockType.Farm_TiemOrder,"farm_time_order_unlock_level"},
        {MergeUnlockType.KapiTile,"kapi_tile_unlock"},
        {MergeUnlockType.JungleAdventure,"jungleadventure_unlock_level"},
        {MergeUnlockType.PhotoAlbum,"photo_album_unlock_level"},
        {MergeUnlockType.LuckyGoldenEgg,"luckygoldenegg_unlock_level"},
        {MergeUnlockType.BiuBiu,"biubiu_unlock_level"},
        {MergeUnlockType.Parrot,"parrot_unlock_level"},
        {MergeUnlockType.JumpGrid,"jumpgrid_unlock_level"},
        {MergeUnlockType.BalloonRacing,"BalloonRacing_unlock_level"},
        {MergeUnlockType.NewNewIceBreakPack,"NewNewIceBreakPack_unlock_level"},
        {MergeUnlockType.FlowerField,"FlowerField_unlock_level"},
        {MergeUnlockType.ClimbTower,"ClimbTower_unlock_level"},
        {MergeUnlockType.GiftBagSendTwo,"GiftBagSendTwo_unlock_level"},
        {MergeUnlockType.RabbitRacing,"RabbitRacing_unlock_level"},
        {MergeUnlockType.GiftBagSendThree,"GiftBagSendThree_unlock_level"},
        {MergeUnlockType.CollectStone,"CollectStone_unlock_level"},
        {MergeUnlockType.TipReward,"tipReward_unlock_level"},
        {MergeUnlockType.Team,"team_unlock_level"},
        {MergeUnlockType.GiftBagSend4,"GiftBagSend4_unlock_level"},
        {MergeUnlockType.GiftBagSend6,"GiftBagSend6_unlock_level"},
        {MergeUnlockType.MiniGame_Deo,"miniGameStatus_decoUnlock"},
        { MergeUnlockType.PillowWheel,"pillow_wheel_unlock"},
        { MergeUnlockType.TrainOrder,"train_order_unlock"},
        { MergeUnlockType.EndlessEnergyGiftBag,"endless_energy_gift_bag_unlock"},
        { MergeUnlockType.CatchFish,"catch_fish_unlock"},
        
    };

    private static Dictionary<MergeUnlockType, int> _unlockParams = new Dictionary<MergeUnlockType, int>();
    public static bool IsOpen(MergeUnlockType type)
    {
        bool result = false;
        int level = ExperenceModel.Instance.GetLevel();
    
        int unlockParam = GetUnlockParam(type);
        switch (type)
        {
            case MergeUnlockType.DailyTask:
            case MergeUnlockType.OrderTask:
            case MergeUnlockType.MasterCard:
            case MergeUnlockType.TaskAssist:
            case MergeUnlockType.PigBank:
            case MergeUnlockType.SuperLoginGift:
            case MergeUnlockType.DailyBonus:
            case MergeUnlockType.DailyRank:
            case MergeUnlockType.LikeUs:
            case MergeUnlockType.RateUs:
            case MergeUnlockType.GameStore:
            case MergeUnlockType.ItemSell:
            case MergeUnlockType.DescPurchase:
            case MergeUnlockType.DailyRv:
            case MergeUnlockType.BubbleOpen:
            case MergeUnlockType.Balloon:
            case MergeUnlockType.CustomerGift:
            case MergeUnlockType.TaskCenter:
            case MergeUnlockType.PayRebate:
            case MergeUnlockType.PayRebateLocal:
            case MergeUnlockType.GarageCleanup:
            case MergeUnlockType.Easter:
            case MergeUnlockType.TMatch:
            {
                return unlockParam - MainOrderManager.Instance.CompleteTaskNum <= 0;
            }
            case MergeUnlockType.Mermaid:
                return DecoManager.Instance.IsOwnedNode(unlockParam);
            case MergeUnlockType.DogPlayUnlockKeepPetLevel:
                return KeepPetModel.Instance.GetLevel() >= unlockParam;
            case MergeUnlockType.Farm_TiemOrder:
            {
                level = FarmModel.Instance.GetLevel();
                break;
            }
        }
        result = level >= unlockParam;
        return result;
    }

    public static int GetUnlockParam(MergeUnlockType type)
    {
        int unlockParam = 0;
        if (_unlockParams.ContainsKey(type))
            return _unlockParams[type];

        if (_unlockKeys.ContainsKey(type))
        {
            if (GlobalConfigManager.Instance.tableGlobal_Config_Number.Count > 0)
            {
                unlockParam =  GlobalConfigManager.Instance.GetNumValue(_unlockKeys[type]);   
                _unlockParams.Add(type, unlockParam);   
            }

            return unlockParam;
        }
        
        switch (type)
        {                  
            case MergeUnlockType.TMatch:
                var adConfigCommon = AdConfigHandle.Instance.GetCommon();
                if (adConfigCommon != null)
                {
                    unlockParam =  adConfigCommon.TMatchUnlock;   
                }
                else
                {
                    unlockParam = 999999;
                }
                break; 
        }

        return unlockParam;
    }

    public static bool IsUnlockSoon(MergeUnlockType type)
    {
        int unLockParam = GetUnlockParam(type);
        
        int level = ExperenceModel.Instance.GetLevel();

        switch (type)
        {
            case MergeUnlockType.DailyTask:
            case MergeUnlockType.OrderTask:
            case MergeUnlockType.MasterCard:
            case MergeUnlockType.TaskAssist:
            case MergeUnlockType.PigBank:
            case MergeUnlockType.SuperLoginGift:
            case MergeUnlockType.DailyBonus:
            case MergeUnlockType.DailyRank:
            case MergeUnlockType.LikeUs:
            case MergeUnlockType.RateUs:
            case MergeUnlockType.GameStore:
            case MergeUnlockType.ItemSell:
            case MergeUnlockType.DescPurchase:
            case MergeUnlockType.DailyRv:
            case MergeUnlockType.BubbleOpen:
            case MergeUnlockType.Balloon:
            case MergeUnlockType.CustomerGift:
            case MergeUnlockType.TaskCenter:
            case MergeUnlockType.PayRebate:
            case MergeUnlockType.PayRebateLocal:
            case MergeUnlockType.GarageCleanup:
            case MergeUnlockType.Easter:
            case MergeUnlockType.TMatch:
            case MergeUnlockType.HappyGo:
            case MergeUnlockType.ThreeGift:
            {
                return MainOrderManager.Instance.CompleteTaskNum >= unLockParam - 10;
            }
            case MergeUnlockType.Mermaid:
                return true;
            case MergeUnlockType.Bag:
                return true;
            case MergeUnlockType.Farm_TiemOrder:
            {
                level = FarmModel.Instance.GetLevel();
                break;
            }
        }

        return  level >= (unLockParam-2);
    }
    
    public enum MergeUnlockType
    {
        // dailyTask,//每日任务功能开放的挂点数
        GameStore, //商城系统开放的挂点数
        BubbleOpen, //同时也决定气泡不会在这之前出现
        Bag, //仓库系统开放的挂点数
        CustomerGift, //神秘礼物
        Balloon, //气球广告
        ItemSell, //出售功能
        DailyTask, //每日任务
        LikeUs,
        RateUs,
        OrderTask,
        GiftBagLink,
        MasterCard,
        Bundle,
        TaskAssist,
        IceBreak, //破冰
        PigBank,
        SuperLoginGift,
        DailyBonus,
        DailyRank,
        DogHope,
        ClimbTree,
        DailyPack,
        EnergyPack,
        DescPurchase,//物品详情购买
        DailyRv,
        FlashSale,
        TaskCenter, //任务中心
        PayRebate, //充值返利
        PayRebateLocal, //充值返利本地
        GarageCleanup, //仓库整理
        Easter, //复活节活动
        BattlePass,
        EnergyTorrent,
        Mermaid,
        CoinRush,
        TMatch,
        CoinCompetition,
        SummerWatermelon,
        SummerWatermelonBread,
        CoinLeaderBoard,
        SeaRacing,
        HappyGo,
        NewDailyPack,
        CardCollection,
        ThreeGift,
        Easter2024,
        TimeOrder,
        GiftBagBuyBetter,
        GiftBagSendOne,
        LimitOrderLine,
        SnakeLadder,
        ShopExtraReward,
        ThemeDecoration,
        SlotMachine,
        WeeklyCard,
        Monopoly,
        CrazeOrder,
        TreasureMap,
        TreasureHunt,
        LuckyGoldenEgg,
        KeepPetDog,
        ExtraEnergy,
        OptionalGift,
        GiftBagProgress,
        Matreshkas,
        ButterflyWorkShop,
        Turntable,
        DiamondReward,
        GiftBagDouble,
        GardenTreasure,
		MixMaster,
        SaveTheWhales,
        TurtlePang,
        StarrySkyCompass,
        BlindBox,
        TotalRecharge,
        TotalRechargeNew,
        NewDailyPackageExtraReward,
        Zuma,
        Kapibala,
        Farm,
        KapiScrew,
        DogPlayUnlockLevel,
        DogPlayUnlockKeepPetLevel,
        FishCulture,
        DogPlayExtraReward,
        KapiTile,
        Farm_TiemOrder,
        JungleAdventure,
        PhotoAlbum,
        BiuBiu,
        Parrot,
        JumpGrid,
        BalloonRacing,
        NewNewIceBreakPack,
        FlowerField,
        ClimbTower,
        GiftBagSendTwo,
        RabbitRacing,
        GiftBagSendThree,
        CollectStone,
        TipReward,
        Team,
        GiftBagSend4,
        GiftBagSend6,
        MiniGame_Deo,
        PillowWheel,
        TrainOrder,
        EndlessEnergyGiftBag,
        CatchFish,
    }
}
