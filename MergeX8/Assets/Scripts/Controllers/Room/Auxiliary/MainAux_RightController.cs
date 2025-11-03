using System;
using System.Collections.Generic;
using System.Linq;
using Activity.BattlePass;
using Activity.BattlePass_2;
using Activity.ChristmasBlindBox.View;
using Activity.CollectStone.View;
using Activity.DiamondReward.View;
using Activity.GiftBagBuyBetter.Controller;
using Activity.GiftBagDouble.View;
using Activity.GiftBagLink.Controller;
using Activity.GiftBagSend4.Controller;
using Activity.GiftBagSend6.Controller;
using Activity.GiftBagSendOne.Controller;
using Activity.GiftBagSendThree.Controller;
using Activity.GiftBagSendTwo.Controller;
using Activity.JumpGrid.Controller;
using Activity.JungleAdventure.Controller;
using Activity.LuckyGoldenEgg.Controller;
using Activity.NewDailyPackageExtraReward.View;
using Activity.ShopExtraReward.View;
using Activity.SummerWatermelon.UI;
using Activity.SummerWatermelonBread.UI;
using ActivityLocal.BuyDiamondTicket.View;
using ActivityLocal.LevelUpPackage.UI;
using ActivityLocal.NewIceBreakGiftBag.View;
using DragonPlus;
using DragonU3DSDK.Storage;
using Dynamic;
using Game;
using Gameplay.UI.Room.Auxiliary.AusItem;
using UnityEngine;
using UnityEngine.UI;

public class MainAux_RightController : SuperMainController
{
    private GameObject _seatPointObj;

    private List<Aux_ItemBase> _auxItems = new List<Aux_ItemBase>();
    private Dictionary<Type, List<Transform>> _siblingTransforms = new Dictionary<Type, List<Transform>>();

    private Transform _groupTransform;

    private Type[] _dynamicEntryKey = new Type[]
    {
        typeof(DynamicEntry_Home_BattlePass),
        typeof(DynamicEntry_Home_BattlePass2),
        typeof(DynamicEntry_Home_BuyDiamondTicket),
        typeof(DynamicEntry_Home_GiftBagSendOne),
        typeof(DynamicEntry_Home_GiftBagSendTwo),
        typeof(DynamicEntry_Home_GiftBagSendThree),
        typeof(DynamicEntry_Home_GiftBagSend4),
        typeof(DynamicEntry_Home_GiftBagSend6),
        typeof(DynamicEntry_Home_GiftBagBuyBetter),
        typeof(DynamicEntry_Home_GiftBagDouble),
        typeof(DynamicEntry_Home_DiamondReward),
        typeof(DynamicEntry_Home_ShopExtraReward),
        typeof(DynamicEntry_Home_NewDailyPackageExtraReward),
        typeof(DynamicEntry_Home_LevelUp),
        typeof(DynamicEntry_Home_SummerWatermelonGift),
        typeof(DynamicEntry_Home_SummerWatermelonBreadGift),
        typeof(DynamicEntry_Home_ChristmasBlindBox),
        typeof(DynamicEntry_Home_GiftBagLink),
        typeof(DynamicEntry_Home_NewIceBreakGiftBag),
        typeof(DynamicEntry_Home_CollectStone),
        
    };

    private Type[] _siblingSort = new Type[]
    {
        typeof(Aux_BattlePass),
        typeof(Aux_BattlePass2),
        typeof(Aux_GiftBagLink),
        typeof(Aux_GiftBagBuyBetter),
        typeof(Aux_OptionalGift),
        typeof(Aux_GiftBagSendOne),
        typeof(Aux_GiftBagSendTwo),
        typeof(Aux_GiftBagSendThree),
        typeof(Aux_GiftBagSend4),
        typeof(Aux_GiftBagSend6),
        typeof(Aux_ThreeGift),
        typeof(Aux_MultipleGift),
        typeof(Aux_GiftBagDouble),
        typeof(Aux_NewDailyPack),
        typeof(Aux_DiamondReward),
        typeof(Aux_ShopExtraReward),
        typeof(Aux_ExtraEnergy),
        typeof(Aux_BuyDiamondTicket),
        typeof(Aux_NewDailyPackageExtraReward),
        typeof(Aux_NewIceBreakGiftBag),
        typeof(Aux_NewNewIceBreakPack),
        typeof(Aux_TotalRecharge),
        typeof(Aux_TotalRecharge_New),
        typeof(Aux_ChristmasBlindBox),
        typeof(Aux_EnergyPack),
        typeof(Aux_LevelUpPackage),
        typeof(Aux_DailyPack),
        typeof(Aux_DailyRv),
        typeof(Aux_PayRebateLocal),
        typeof(Aux_PayRebate),
        typeof(Aux_SummerWatermelonGift),
        typeof(Aux_SummerWatermelonBreadGift),
        typeof(Aux_CollectStone),
    };
    
    private void Awake()
    {
        _groupTransform = transform.Find("Root/Views/MainGroup/RightGroup/Content");
        
        _seatPointObj = CommonUtils.FindObject(transform, "Root/Views/MainGroup/RightGroup/Content/SeatPoint");
        
        InvokeRepeating("RefreshUI", 0, 2);
    }

    private void Start()
    {
        AddAuxItem<Aux_DailyRv>("Root/Views/MainGroup/RightGroup/Content/ButtonDailyRv");
        AddAuxItem<Aux_DailyPack>("Root/Views/MainGroup/RightGroup/Content/ButtonMaterialPack");
        AddAuxItem<Aux_PayRebate>("Root/Views/MainGroup/RightGroup/Content/PayRebate");
        AddAuxItem<Aux_PayRebateLocal>("Root/Views/MainGroup/RightGroup/Content/PayRebateLocal");
        AddAuxItem<Aux_EnergyPack>("Root/Views/MainGroup/RightGroup/Content/EnergyGift");
        AddAuxItem<Aux_NewDailyPack>("Root/Views/MainGroup/RightGroup/Content/DailyGift");
        AddAuxItem<Aux_ThreeGift>("Root/Views/MainGroup/RightGroup/Content/ThreeGift");
        AddAuxItem<Aux_MultipleGift>("Root/Views/MainGroup/RightGroup/Content/MultipleGift");
        AddAuxItem<Aux_ExtraEnergy>("Root/Views/MainGroup/RightGroup/Content/ExtraEnergy");
        AddAuxItem<Aux_OptionalGift>("Root/Views/MainGroup/RightGroup/Content/OptionalGift");
        AddAuxItem<Aux_TotalRecharge>("Root/Views/MainGroup/RightGroup/Content/ButtonCumulativeRecharge");
        AddAuxItem<Aux_TotalRecharge_New>("Root/Views/MainGroup/RightGroup/Content/ButtonCumulativeRecharge_New");
        AddAuxItem<Aux_NewNewIceBreakPack>("Root/Views/MainGroup/RightGroup/Content/NewNewIceBreakPack");
        LevelUpPackageModel.Instance.InitAux();
    }

    public enum AuxItemGroup
    {
        Left,
        Right,
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
    
    public override Transform GetTransform()
    {
        return _groupTransform;
    }
}