using System;
using System.Collections.Generic;
using System.Linq;
using Activity.BalloonRacing.Dynamic;
using Activity.BattlePass;
using Activity.BattlePass_2;
using Activity.BiuBiu.UI;
using Activity.CatchFish.UI;
using Activity.CoinCompetition;
using Activity.CoinRush;
using Activity.FishCulture.View;
using Activity.FlowerField.UI;
using Activity.GardenTreasure.View;
using Activity.GiftBagProgress.View;
using Activity.JumpGrid;
using Activity.JumpGrid.Controller;
using Activity.JungleAdventure.Controller;
using Activity.KapiScrew.View;
using Activity.KapiTile.View;
using Activity.LuckyGoldenEgg;
using Activity.LuckyGoldenEgg.Controller;
using Activity.Matreshkas.View;
using Activity.MixMaster.View;
using Activity.Monopoly.View;
using Activity.Parrot.UI;
using Activity.PillowWheel.UI;
using Activity.PhotoAlbum.View;
using Activity.PigBank.Controller;
using Activity.RabbitRacing.Dynamic;
using Activity.SaveTheWhales;
using Activity.SeaRacing.UI;
using Activity.SlotMachine.View;
using Activity.StarrySkyCompass.View;
using Activity.SummerWatermelonBread.UI;
using Activity.Turntable.Controller;
using Activity.TurtlePang.View;
using Activity.Zuma.View;
using ActivityLocal.BlindBox.View;
using ActivityLocal.BuyDiamondTicket.View;
using ActivityLocal.CardCollection.UI;
using ActivityLocal.ExtraOrderRewardCoupon.UI;
using ActivityLocal.KeepPet.UI;
using ClimbTreeLeaderBoard;
using DogHopeLeaderBoard;
using Dynamic;
using Easter2024LeaderBoard;
using SnakeLadderLeaderBoard;
using ThemeDecorationLeaderBoard;
using TMatch;
using UnityEngine;

public partial class MergeTaskTipsController
{
    private Type[] _dynamicEntryKey = new Type[]
    {
        typeof(DynamicEntry_Game_SummerWatermelonBread),
        typeof(DynamicEntry_Game_PigBank),
        typeof(DynamicEntry_Game_CoinCompetition),
        typeof(DynamicEntry_Game_ClimbTree),
        typeof(DynamicEntry_Game_SeaRacing),
        typeof(DynamicEntry_Game_DogHope),
        typeof(DynamicEntry_Game_CardCollection),
        typeof(DynamicEntry_Game_CardPackage),
        typeof(DynamicEntry_Game_Easter2024),
        typeof(DynamicEntry_Game_SaveTheWhales),
        typeof(DynamicEntry_Game_SnakeLadder),
        typeof(DynamicEntry_Game_ExtraOrderRewardCoupon),
        typeof(DynamicEntry_Game_Matreshkas),
        typeof(DynamicEntry_Game_Turntable),
        typeof(DynamicEntry_Game_GardenTreasure),
        typeof(DynamicEntry_Game_BindBox),
        typeof(DynamicEntry_Game_BuyDiamondTicket),
        typeof(DynamicEntry_Game_BattlePass),
        typeof(DynamicEntry_Game_BattlePass2),
        typeof(DynamicEntry_Game_JungleAdventure),
        typeof(DynamicEntry_Game_LuckyGoldenEgg),
        typeof(DynamicEntry_Game_BiuBiu),
        typeof(DynamicEntry_Game_Parrot),
        typeof(DynamicEntry_Game_PillowWheel),
        typeof(DynamicEntry_Game_CatchFish),
        typeof(DynamicEntry_Game_FlowerField),
        typeof(DynamicEntry_Game_JumpGrid),
        typeof(DynamicEntry_Game_ThemeDecoration),
        typeof(DynamicEntry_Game_ThemeDecorationLeaderBoard),
        typeof(DynamicEntry_Game_SlotMachine),
        typeof(DynamicEntry_Game_Monopoly),
        typeof(DynamicEntry_Game_KeepPet),
        typeof(DynamicEntry_Game_CoinRush),
        typeof(DynamicEntry_Game_MixMaster),
        typeof(DynamicEntry_Game_TurtlePang),
        typeof(DynamicEntry_Game_StarrySkyCompass),
        typeof(DynamicEntry_Game_Zuma),
        typeof(DynamicEntry_Game_Kapibala),
        typeof(DynamicEntry_Game_KapiScrew),
        typeof(DynamicEntry_Game_FishCulture),
        typeof(DynamicEntry_Game_KapiTile),
        typeof(DynamicEntry_Game_PhotoAlbum),
        typeof(DynamicEntry_Game_GiftBagProgress),
        typeof(DynamicEntry_Game_BalloonRacing),
        typeof(DynamicEntry_Game_RabbitRacing),
        typeof(Activity.TrainOrder.DynamicEntry_MergeTrainOrder),
        
    };
    
    private Dictionary<Type, List<MonoBehaviour>> _dynamicEntryMap = new Dictionary<Type, List<MonoBehaviour>>();
    
    public Transform DynamicParent => content;


    #region Funciton

    public MergeCardPackage MergeCardPackage=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeCardPackage, DynamicEntry_Game_CardPackage>();
    public MergeCardCollection MergeCardCollection=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeCardCollection, DynamicEntry_Game_CardCollection>();
    public MergePigBoxInTaskController _MergePigBoxInTask=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergePigBoxInTaskController, DynamicEntry_Game_PigBank>();
    public MergeCoinCompetition mergeCoinCompetition=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeCoinCompetition, DynamicEntry_Game_CoinCompetition>();
    public MergeSeaRacing MergeSeaRacing=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSeaRacing, DynamicEntry_Game_SeaRacing>();
    public MergeDogHope _mergeDogHope=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeDogHope, DynamicEntry_Game_DogHope>();
    public MergeZuma MergeZumaEntry=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeZuma, DynamicEntry_Game_Zuma>();
    public MergeSummerWatermelonBread _mergeSummerWatermelonBread=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSummerWatermelonBread, DynamicEntry_Game_SummerWatermelonBread>();
    public MergeClimbTree _mergeClimbTree=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeClimbTree, DynamicEntry_Game_ClimbTree>();
    public MergeEaster2024 MergeEaster2024=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeEaster2024, DynamicEntry_Game_Easter2024>();
    public MergeBiuBiu MergeBiuBiu=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeBiuBiu, DynamicEntry_Game_BiuBiu>();
    public MergeParrot MergeParrot=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeParrot, DynamicEntry_Game_Parrot>();
    public MergePillowWheel MergePillowWheel=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergePillowWheel, DynamicEntry_Game_PillowWheel>();
    public MergeCatchFish MergeCatchFish=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeCatchFish, DynamicEntry_Game_CatchFish>();
    public MergeFlowerField MergeFlowerField=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeFlowerField, DynamicEntry_Game_FlowerField>();
    public MergeBattlePass _MergeBattlePass=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeBattlePass, DynamicEntry_Game_BattlePass>();
    public MergeBattlePass2 _MergeBattlePass2=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeBattlePass2, DynamicEntry_Game_BattlePass2>();
    public MergeJungleAdventure _MergeJungleAdventure=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeJungleAdventure, DynamicEntry_Game_JungleAdventure>();
    public MergeTaskItemJumpGrid _MergeJumpGrid=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeTaskItemJumpGrid, DynamicEntry_Game_JumpGrid>();
    public MergeLuckyGoldenEgg _MergeLuckyGoldenEgg=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeLuckyGoldenEgg, DynamicEntry_Game_LuckyGoldenEgg>();
    public MergeBlindBox MergeBlindBox=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeBlindBox, DynamicEntry_Game_BindBox>();
    public MergeBuyDiamondTicket MergeBuyDiamondTicket=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeBuyDiamondTicket, DynamicEntry_Game_BuyDiamondTicket>();
    public MergeMatreshkasEntry MergeMatreshkasEntry=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeMatreshkasEntry, DynamicEntry_Game_Matreshkas>();
    public MergeTurntableEntry MergeTurntableEntry=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeTurntableEntry, DynamicEntry_Game_Turntable>();
    public MergeGardenTreasureEntry MergeGardenTreasureEntry=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeGardenTreasureEntry, DynamicEntry_Game_GardenTreasure>();
    public MergeSnakeLadder MergeSnakeLadder=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSnakeLadder, DynamicEntry_Game_SnakeLadder>();
    public MergeExtraOrderRewardCoupon MergeExtraOrderRewardCoupon=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeExtraOrderRewardCoupon, DynamicEntry_Game_ExtraOrderRewardCoupon>();
    public MergeSaveTheWhales _MergeSaveTheWhales=>DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSaveTheWhales, DynamicEntry_Game_SaveTheWhales>();
    
    #endregion
    
    
    
    
    
    
    
    
    
    private void RefreshDynamicEntry()
    {
        if(!gameObject.activeInHierarchy)
            return;
        
        foreach (var type in _dynamicEntryKey)
        {
            if (!_dynamicEntryMap.ContainsKey(type))
                _dynamicEntryMap[type] = new List<MonoBehaviour>();
            
            var monos = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviours<MonoBehaviour>(type);
            if (monos == null)
                continue;

            foreach (var mono in monos)
            {
                if(mono == null)
                    continue;
                
                if(_dynamicEntryMap[type].Contains(mono))
                    continue;
                
                _dynamicEntryMap[type].Add(mono);
                AddSibling(mono.GetType().ToString(), mono.transform);
            }
        }
    }
}