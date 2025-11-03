using System;
using System.Collections.Generic;
using Activity.BalloonRacing.Dynamic;
using Dynamic;
using Activity.BiuBiu.UI;
using Activity.CatchFish.UI;
using Activity.CommonResourceLeaderBoard.View;
using Activity.FishCulture.View;
using Activity.FlowerField.UI;
using Activity.GardenTreasure.View;
using Activity.GiftBagProgress.View;
using Activity.JumpGrid.Controller;
using Activity.JungleAdventure.Controller;
using Activity.KapiScrew.View;
using Activity.KapiTile.View;
using Activity.LuckyGoldenEgg.Controller;
using Activity.MixMaster.View;
using Activity.Monopoly.View;
using Activity.Parrot.UI;
using Activity.PhotoAlbum.View;
using Activity.PillowWheel.UI;
using Activity.RabbitRacing.Dynamic;
using Activity.SlotMachine.View;
using Activity.StarrySkyCompass.View;
using Activity.SummerWatermelonBread.UI;
using Activity.Turntable.Controller;
using Activity.TurtlePang.View;
using Activity.Zuma.View;
using ActivityLocal.BlindBox.View;
using ActivityLocal.CardCollection.UI;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Easter2024LeaderBoard;
using Framework;
using Game;
using Gameplay.UI.Room.Auxiliary.AusItem;
using SnakeLadderLeaderBoard;
using ThemeDecorationLeaderBoard;
using TMatch;
using UnityEngine;
using UnityEngine.UI;


public class MainAux_LeftController : SuperMainController
{
    private List<Aux_ItemBase> _auxItems = new List<Aux_ItemBase>();
    private Dictionary<Type, List<Transform>> _siblingTransforms = new Dictionary<Type, List<Transform>>();
    private GameObject _seatPointObj;

    private Type[] _dynamicEntryKey = new Type[]
    {
        typeof(DynamicEntry_Home_BindBoxTheme),
        typeof(DynamicEntry_Home_CardCollectionTheme),
        typeof(DynamicEntry_Home_SnakeLadder),
        typeof(DynamicEntry_Home_Easter2024),
        typeof(DynamicEntry_Home_GiftBagProgress),
        typeof(DynamicEntry_Home_PhotoAlbum),
        typeof(DynamicEntry_Home_ThemeDecoration),
        typeof(DynamicEntry_Home_Zuma),
        typeof(DynamicEntry_Home_Monopoly),
        typeof(DynamicEntry_Home_SlotMachine),
        typeof(DynamicEntry_Home_MixMaster),
        typeof(DynamicEntry_Home_StarrySkyCompass),
        typeof(DynamicEntry_SummerWatermelonBread),
        typeof(DynamicEntry_Home_Turntable),
        typeof(DynamicEntry_Home_TurtlePang),
        typeof(DynamicEntry_Home_Parrot),
        typeof(DynamicEntry_Home_PillowWheel),
        typeof(DynamicEntry_Home_CatchFish),
        typeof(DynamicEntry_Home_FlowerField),
        typeof(DynamicEntry_Home_JungleAdventure),
        typeof(DynamicEntry_Home_FishCulture),
        typeof(DynamicEntry_Home_LuckyGoldenEgg),
        typeof(DynamicEntry_Home_BiuBiu),
        typeof(DynamicEntry_Home_KapiScrew),
        typeof(DynamicEntry_Home_KapiTile),
        typeof(DynamicEntry_Home_Kapibala),
        typeof(DynamicEntry_Home_GardenTreasure),
        typeof(DynamicEntry_Home_CommonResourceLeaderBoard),
        typeof(DynamicEntry_Home_ThemeDecorationLeaderBoard),
        typeof(DynamicEntry_Home_JumpGrid),
        typeof(DynamicEntry_Home_BalloonRacing),
        typeof(DynamicEntry_Home_RabbitRacing),
    };
    
    private Type[] _siblingSort = new Type[]
    {
        typeof(Aux_NoAdsGiftBag),
        
        typeof(Aux_BindEmail),
        typeof(Aux_BlindBoxTheme),
        typeof(Aux_CardCollectionTheme),
        
        typeof(Aux_SnakeLadder),
        typeof(Aux_Easter2024),
        typeof(Aux_Monopoly),
        typeof(Aux_ThemeDecoration),
        typeof(Aux_PhotoAlbum),
        
        typeof(Aux_GiftBagProgress),
        typeof(Aux_PigBank),
        typeof(Aux_PillowWheel),
        typeof(Aux_CatchFish),
        typeof(Aux_MixMaster),
        typeof(Aux_StarrySkyCompass),
        typeof(Aux_TurtlePang),
        
        typeof(Aux_ClimbTree),
        typeof(Aux_DogHope),
        typeof(Aux_Parrot),
        typeof(Aux_FlowerField),
        typeof(Aux_JungleAdventure),
        typeof(Aux_FishCulture),
        
        typeof(Aux_GardenTreasure),
        typeof(Aux_TreasureHunt),
        typeof(Aux_LuckyGoldenEgg),
        typeof(Aux_CoinCompetition),
        typeof(Aux_JumpGrid),
        typeof(Aux_SeaRacing),
        
        typeof(Aux_ButterflyWorkShop),
        typeof(Aux_BiuBiu),
        typeof(Home_BalloonRacing),
        typeof(Home_RabbitRacing),
        
        typeof(Aux_KapiScrew),
        typeof(Aux_KapiTile),
        typeof(Aux_Kapibala),
        
        typeof(Aux_Zuma),
        typeof(Aux_GarageCleanup),
        
        typeof(Aux_CoinLeaderBoard),
        typeof(Aux_RecoverCoin),
        typeof(Aux_ThemeDecorationLeaderBoard),
        
        typeof(Aux_Turntable),
        typeof(Aux_SlotMachine),
        
        typeof(Aux_WeekCard),
        
        typeof(Aux_IceBreakPack),
        typeof(Aux_IceBreakPackLow),
        
        typeof(Aux_HappyGo),
        typeof(Aux_DailyRank),
        typeof(Aux_Mermaid),
        typeof(Aux_SummerWatermelon),
        typeof(Aux_SummerWatermelonBread),
        
        
    };
        
    
    private Transform _groupTransform;
     
    private void Awake()
    {
        _groupTransform = transform.Find("Root/Views/MainGroup/LeftGroup/Content");
        
        InvokeRepeating("RefreshUI", 0, 2);
    }

    private void OnDestroy()
    {
    }

    private void Start()
    {
        _seatPointObj = CommonUtils.FindObject(transform, "Root/Views/MainGroup/LeftGroup/Content/SeatPoint");
        AddAuxItem<Aux_BindEmail>("Root/Views/MainGroup/LeftGroup/Content/BindEmail");
        AddAuxItem<Aux_IceBreakPack>("Root/Views/MainGroup/LeftGroup/Content/ButtonIcebreakingPack");
        AddAuxItem<Aux_IceBreakPackLow>("Root/Views/MainGroup/LeftGroup/Content/ButtonIcebreakingPackLow");
        AddAuxItem<Aux_PigBank>("Root/Views/MainGroup/LeftGroup/Content/ButtonPigBox");
        AddAuxItem<Aux_DailyRank>("Root/Views/MainGroup/LeftGroup/Content/ButtonLevelRanking");
        AddAuxItem<Aux_DogHope>("Root/Views/MainGroup/LeftGroup/Content/ButtonDog");
        AddAuxItem<Aux_ClimbTree>("Root/Views/MainGroup/LeftGroup/Content/ClimbTree");
        AddAuxItem<Aux_RecoverCoin>("Root/Views/MainGroup/LeftGroup/Content/RecoverCoin");
        AddAuxItem<Aux_CoinLeaderBoard>("Root/Views/MainGroup/LeftGroup/Content/CoinLeaderBoard");
        AddAuxItem<Aux_SeaRacing>("Root/Views/MainGroup/LeftGroup/Content/SeaRace");
        AddAuxItem<Aux_SummerWatermelon>("Root/Views/MainGroup/LeftGroup/Content/SummerWatermelon");
        AddAuxItem<Aux_GarageCleanup>("Root/Views/MainGroup/LeftGroup/Content/GarageCleanup");
        AddAuxItem<Aux_Easter>("Root/Views/MainGroup/LeftGroup/Content/Easter");
        AddAuxItem<Aux_EasterPack>("Root/Views/MainGroup/LeftGroup/Content/EasterPack");
        AddAuxItem<Aux_Mermaid>("Root/Views/MainGroup/LeftGroup/Content/Mermaid");
        AddAuxItem<Aux_CoinCompetition>("Root/Views/MainGroup/LeftGroup/Content/GoldCoinCompetition");
        AddAuxItem<Aux_HappyGo>("Root/Views/MainGroup/LeftGroup/Content/HappyGo");
        AddAuxItem<Aux_WeekCard>("Root/Views/MainGroup/LeftGroup/Content/Aux_WeeklyCard");
        AddAuxItem<Aux_TreasureHunt>("Root/Views/MainGroup/LeftGroup/Content/TreasureHunt");
        AddAuxItem<Aux_ButterflyWorkShop>("Root/Views/MainGroup/LeftGroup/Content/ButterflyWorkShop");
        AddAuxItem<Aux_NoAdsGiftBag>("Root/Views/MainGroup/LeftGroup/Content/NoADS");
    }

    public void RefreshUI()
    {
        foreach (var type in _dynamicEntryKey)
        {
            var monoBehaviours = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviours<Aux_ItemBase>(type);
            if(monoBehaviours == null)
                continue;
            
            foreach (var auxItemBase in monoBehaviours)
            {
                if(_auxItems.Contains(auxItemBase))
                    continue;

                AddAuxItem(auxItemBase);
                UpdateSibling();
            }
        }
        _auxItems.ForEach(item => { item.UpdateUI(); });
        UpdateSibling();
    }

    public override void Show()
    {
        // _auxItems.ForEach(item => { item.UpdateUI(); });
        RefreshUI();
        UpdateSibling();
    }

    public override void Hide()
    {
    }

    private void AddAuxItem<T>(string path) where T : Aux_ItemBase
    {
        AddAuxItem(CommonUtils.FindOrCreate<T>(transform, path));
    }
    
    private void AddAuxItem(Aux_ItemBase itemBase)
    {
        if(itemBase == null)
            return;
        
        if(_auxItems.Contains(itemBase))
            return;
        
        _auxItems.Add(itemBase);

        if (!_siblingTransforms.ContainsKey(itemBase.GetType()))
            _siblingTransforms[itemBase.GetType()] = new List<Transform>();
        
        _siblingTransforms[itemBase.GetType()].Add(itemBase.transform);
    }

    private void UpdateSibling()
    {
        int index = 1;
        foreach (var type in _siblingSort)
        {
            if(!_siblingTransforms.ContainsKey(type))
                continue;
            
            foreach (var trans in _siblingTransforms[type])
            {
                trans.SetSiblingIndex(index);
                index++;
            }
        }
    }
    
    public override Transform GetTransform()
    {
        return _groupTransform;
    }
}