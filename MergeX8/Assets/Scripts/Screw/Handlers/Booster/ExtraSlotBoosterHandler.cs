using Cysharp.Threading.Tasks;
using DragonU3DSDK.Storage;
using Screw;
using Screw.UserData;

namespace Screw
{
    public class ExtraSlotBoosterHandler : BoosterHandler
    {
        private int MAX = 16;
        private int UseMAX = 2;
        private int useCount = 0;

        public ExtraSlotBoosterHandler(ScrewGameContext context)
            : base(context, BoosterType.ExtraSlot)
        {
            
        }

        public override bool GetShowBoosterUnLockPop()
        {
            // return false;
            //迁移报错注释
            return StorageManager.Instance.GetStorage<StorageScrew>().IsExtraSlotPopGuide;
        }

        public override void SetShowBoosterUnLockPop()
        {
            //迁移报错注释
            StorageManager.Instance.GetStorage<StorageScrew>().IsExtraSlotPopGuide = true;
        }

        public override int GetBoosterCount()
        {
            return UserData.UserData.Instance.GetRes(ResType.ExtraSlot);
        }

        public override int GetBoosterFreeCount()
        {
            return 0;
        }
        
        public override int GetPurchaseCoinCount()
        {
            //迁移报错注释
            return DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].ScrewHoleCoin;
        }
   
        public override int GetUnlockLevel()
        {
            return 3;
        }

        public override bool CanUse()
        {
            return Context.gameState == ScrewGameState.InProgress && useCount < UseMAX;
        }

        public bool CanUseInFailed()
        {
            return useCount < MAX;
        }

        public override async UniTask Use(bool playEff)
        {
            useCount++;
            Context.AddSlot(playEff);
        }

        public int GetUseCount()
        {
            return useCount;
        }
    }
}