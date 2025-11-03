using Cysharp.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Screw;
using Screw.GameLogic;
using Screw.UserData;
using UnityEngine;

namespace Screw
{
    public class BreakBodyBoosterHandler : BoosterHandler
    {
        public BreakBodyBoosterHandler(ScrewGameContext context)
            : base(context, BoosterType.BreakBody)
        {
            
        }

        public override bool GetShowBoosterUnLockPop()
        {
            // return false;
            //迁移报错注释
            return StorageManager.Instance.GetStorage<StorageScrew>().IsBreakBodyPopGuide;
        }

        public override void SetShowBoosterUnLockPop()
        {
            //迁移报错注释
            StorageManager.Instance.GetStorage<StorageScrew>().IsBreakBodyPopGuide = true;
        }

        public override int GetBoosterCount()
        {
            return UserData.UserData.Instance.GetRes(ResType.BreakBody);
        }

        public override int GetBoosterFreeCount()
        {
            return 0;
        }
        
        public override int GetPurchaseCoinCount()
        {
            return DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].HammerCoin;
        }
   
        public override int GetUnlockLevel()
        {
            return 5;
        }

        public override void ConsumeBooster()
        {
            
        }

        public void ConsumeHammerBooster()
        {
            //迁移报错注释
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ScrewPlayLevel,
                data1 = ScrewGameLogic.Instance.context.levelIndex.ToString()
            };
            UserData.UserData.Instance.ConsumeRes(BoosterTypeToItemType(BoosterType), 1, reason);
        }

        public override bool CanUse()
        {
            return Context.gameState == ScrewGameState.InProgress;
        }

        public override async UniTask Use(bool playEff)
        {
            Context.gameState = ScrewGameState.InUseBooster;
            Context.hookContext.OnLogicEvent(LogicEvent.EnterBreakPanel, null);
            await Context.boostersView.PlayAni("Open");
        }
    }
}