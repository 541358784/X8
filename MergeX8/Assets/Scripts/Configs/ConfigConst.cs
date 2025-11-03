using System;

namespace Game.Config
{
    public static class ConfigConst
    {
        public static String C_CommonTableName = "config";

        //game.json
        public static String C_GlobalTableName = "global";
        public static String C_RewardMapTableName = "rewards";
        public static String C_RecommendLevelTableName = "recommendedlevel";
        public static String C_DailyBonusTableName = "dailybonus";
        public static String C_HourlyBonusTableName = "hourlybonus";
        public static String C_BoostTableName = "boosts";
        public static String C_AchievementTableName = "achievement";
        public static String C_RoundTableName = "rounds";
        public static String C_MapTableName = "mapconfig";
        public static String C_TutorialTableName = "tutorial";
        public static String C_HelpTableName = "help";
        public static String C_ADTableName = "configad";
        public static String C_ADRewardTableName = "configrewardad";
        public static String C_BonusTableName = "bonus";
        public static String C_AdCountryTableName = "country";
        public static String C_AdRegTimeTableName = "registrationtime";
        public static String C_ADSettingTableName = "configsettingads";
        public static String C_DynamicDifficultyTableName = "dynamicdifficulty";
        public static String C_PigBankTableName = "pigbank";
        public static String C_InterstitialProtectTableName = "interstitialprotect";
        public static String C_ComboTableName = "comboconfig";
        public static String C_BonusLimitTableName = "bonuslimit";
        public static String C_CustomerTableName = "customer";
        public static String C_SpecialOfferTableName = "specialofferconfig";
        public static String C_NewWorldBonusTableName = "newworldbonus";
        public static String C_NotificationTableName = "pushnotification";
        public static String C_TaskBonusTableName = "taskbonus";
        public static String C_BundleTableName = "bundle";
        public static String C_SpecialBundleTableName = "specialofferbundle";
        public static String C_InGameBoostBundleTableName = "ingameboostbundle";
        public static String C_ResourceTableName = "resource";
        public static String C_PlayerTierTableName = "playertier";
        public static String C_DynamicTimelineTableName = "dynamictimeline";
        public static String C_FragementTableName = "fragement";
        public static String C_Day7BonusTableName = "day7bonus";
        public static String C_Day7BonusRewardTableName = "day7bonusreward";
        public static String C_LevelPassGuideTableName = "levelpassguide";
        public static String C_LevelPassGuideRewardTableName = "levelpassguidereward";
        public static String C_ShopTableName = "shop";
        public static String C_ShopDailySaleGroupTableName = "shopdailysalegroup";
        public static String C_ShopDailySaleTableName = "shopdailysale";
        public static String C_ShopDailyBundleeGroupTableName = "shopdailybundlegroup";
        public static String C_ShopDailyBundleTableName = "shopdailybundle";

        //mapjson
        public static String C_LevelTableName = "level";
        public static String C_FoodTableName = "food";
        public static String C_FoodGroupTableName = "foodgroup";
        public static String C_DecorationTableName = "decorationbindconfigs";
        public static String C_TimelineTableName = "timeline";
        public static String C_TrayRegionTableName = "trayareaconfigs";
        public static String C_DishRegionTableName = "dishareaconfigs";
        public static String C_CookRegionTableName = "cookareaconfigs";
        public static String C_UnlockRegionTableName = "regionunlock";
        public static String C_FormulaTableName = "formula";
        public static String C_UpgradeTableName = "upgrade";
        public static String C_ActivityUpgradeTableName = "activityupgrade";

        //activity 
        public static String C_ChallengeActivityTableName = "activityconfig";
        public static String C_ChallengeFoodTableName = "food";
        public static String C_ChallengeLevelTableName = "level";
        public static String C_ChallengeTimeLineTableName = "timeline";
        public static String C_ChallengeActivityUpgradeTableName = "activityupgrade";

        public static String C_ChallengeActivityLanternFestival = "lanternfestival";

        //christmasCollect
        public static String C_ChristmasCollectTableName = "christmascollect";
        public static String C_NewYearBundleTableNmae = "activityspinbundle";

        public static String C_ActivityTableName = "activityconfig";

        // sale
        public static String C_SaleTableName = "config";
        public static String C_SaleUITableName = "uiconfig";

        public static String C_I18nTableName = "i18n";

        // piggyBank
        public static String C_PiggyBankTableName = "activityconfig";
        public static String C_UserGroupTableName = "usergroup";

        public static String C_PiggyListTableName = "piggylist";

        // easterChanllenge
        public static String C_EasterCollectTableName = "eastercollect";

        public static String C_WinStreakTableName = "activityconfig";

        //decorationjson
        public static String C_BuildingTableName = "building";
        public static String C_BuildingPointsTableName = "buildingpoints";
        public static String C_WorldTableName = "worlds";
        public static String C_AreaTableName = "areas";
        public static String C_DigPointsTableName = "visitdigpoints";
        public static String C_HomeLevelTableName = "homelevel";
        public static String C_CandyBuildingLevelTableName = "candybuildinglevel";
        public static String C_FriendTreeLevelTableName = "friendtreelevel";
        public static String C_StoryTableName = "story";
        public static String C_StoryNpcTableName = "npc";

        // task 
        public static String C_TaskLineTableName = "taskline";
        public static String C_TaskTableName = "task";
        public static String C_DailyTaskTableName = "dailytask";
        public static String C_DailyTaskBonusTableName = "dailytaskbonus";

        //spin
        public static String C_SpinTableName = "spin";
        public static String C_SpinMissionTargetTableName = "spinmissiontarget";

        // 存档兼容
        public static String C_ConvertFragmentTableName = "fragmentconvert";
        public static String C_ConvertBuildingTableName = "buildingconvert";
        public static String C_CheckPointTableName = "checkpoint";

        // skin
        public static string C_Macht3SkinScene = "scene";
        public static string C_Match3SkinTheme = "theme";

        // decorate
        public static string C_Macht3MapDecorate = "decorate";

        // Flow
        public static string C_Flow = "flows";
        public static string C_FlowLobby = "lobbyflow";
        public static string C_FlowGame = "gameflow";

        public static string C_Match3MapTablePath = "Configs/M3GameConfig/m3mapconfig";
        public static string C_Match3LevelTablePath = "Configs/M3GameConfig/m3levelconfig";
        public static string C_Match3SkinTablePath = "Configs/M3GameConfig/m3skinconfig";
        public static string C_Match3FlowTablePath = "Configs/M3GameConfig/m3lobbyflowconfig";
        public static string C_Match3LayoutTablePath = "Configs/LevelLayout/{0}";
        public static string C_RewardGiftTablePath = "Configs/CommonConfig/rewardgift";
    }
}