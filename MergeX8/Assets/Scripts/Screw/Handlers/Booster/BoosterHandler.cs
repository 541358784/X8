using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Screw.GameLogic;
using Screw.Module;
using Screw.UserData;
using UnityEngine;

namespace Screw
{
    public enum BoosterType : byte
    {
        None,
        ExtraSlot,
        BreakBody,
        TwoTask,
    }

    public abstract class BoosterHandler
    {
        public ScrewGameContext Context { get; }
        public BoosterType BoosterType { get; }

        public BoosterHandler(ScrewGameContext context, BoosterType boosterType)
        {
            Context = context;
            BoosterType = boosterType;
        }

        public abstract bool CanUse();

        public abstract UniTask Use(bool playEff);

        public abstract bool GetShowBoosterUnLockPop();
        public abstract void SetShowBoosterUnLockPop();
        public abstract int GetBoosterCount();
        public abstract int GetBoosterFreeCount();

        public abstract int GetPurchaseCoinCount();

        public virtual void PurchaseBooster()
        {
            var coinCount = GetPurchaseCoinCount();
            
            //TODO BI 
            //迁移报错注释
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BuyItemScrew,
                data1 = ScrewGameLogic.Instance.context.levelIndex.ToString(),
            };
            UserData.UserData.Instance.ConsumeRes(ResType.Coin, coinCount, reason);
            UserData.UserData.Instance.AddRes(BoosterTypeToItemType(BoosterType),3,reason);
            FlyBoosterItemToTarget(BoosterType, 3);
            Context.boostersView.OnPurchaseBooster(BoosterType,3);
        }

        public void FlyBoosterItemToTarget(BoosterType boosterType, int count)
        {
            var target = BoostersView.GetBoosterRoot(boosterType);
            //迁移报错注释
            // UIModule.Instance.EnableEventSystem = false;
            //
            // var dummyItem = SMItemUtility.GenerateDummyItem(BoosterTypeToItemType(boosterType), count);
            // GameApp.Get<FlySys>().FlyItem(dummyItem.ItemId, dummyItem.Amount, Vector2.zero, target.position,
            //     () => { UIModule.Instance.EnableEventSystem = true; },true, 0.3f);
            FlyModule.Instance.Fly(UserData.UserData.GetFirstItemId(BoosterTypeToItemType(boosterType)),count,Vector3.zero);
        }

        public void InitBoostCount()
        {
            if (GetBoosterCount() == 0)
            {
                //迁移报错注释
                //GameApp.Get<UserProfileSys>().InitBoostItem(BoosterTypeToItemType(BoosterType), 1);
            }
        }
        
        public virtual void ConsumeBooster()
        {
            //迁移报错注释
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ScrewPlayLevel,
                data1 = ScrewGameLogic.Instance.context.levelIndex.ToString(),
            };
            UserData.UserData.Instance.ConsumeRes(BoosterTypeToItemType(BoosterType), 1, reason);
            KapiScrewModel.Instance.CostProp((int)BoosterTypeToItemType(BoosterType),1);
        }

        public static ResType BoosterTypeToItemType(BoosterType boosterType)
        {
            switch (boosterType)
            {
                case BoosterType.ExtraSlot:
                    return ResType.ExtraSlot;
                case BoosterType.BreakBody:
                    return ResType.BreakBody;
                case BoosterType.TwoTask:
                    return ResType.TwoTask;
            }
            return ResType.None;
        }

        public abstract int GetUnlockLevel();

        public bool IsBoosterLocked()
        {
            if (Context is ScrewGameKapiScrewContext)
                return false;
            return Context.levelIndex < GetUnlockLevel();
        }

        public virtual void OnPreMove(MoveAction moveAction)
        {
            
        }
    }
}