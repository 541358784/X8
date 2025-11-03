using Cysharp.Threading.Tasks;
using DragonU3DSDK.Storage;
using Screw;
using Screw.UserData;

namespace Screw
{
    public class TwoTaskBoosterHandler : BoosterHandler
    {
        public TwoTaskBoosterHandler(ScrewGameContext context)
            : base(context, BoosterType.TwoTask)
        {
            
        }

        public override bool GetShowBoosterUnLockPop()
        {
            // return false;
            //迁移报错注释
            return StorageManager.Instance.GetStorage<StorageScrew>().IsTwoTaskPopGuide;
        }

        public override void SetShowBoosterUnLockPop()
        {
            //迁移报错注释
            StorageManager.Instance.GetStorage<StorageScrew>().IsTwoTaskPopGuide = true;
        }

        public override int GetBoosterCount()
        {
            return UserData.UserData.Instance.GetRes(ResType.TwoTask);
        }

        public override int GetBoosterFreeCount()
        {
            return 0;
        }
        
        public override int GetPurchaseCoinCount()
        {
            //迁移报错注释
            return DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].ExtraBoxCoin;
        }
   
        public override int GetUnlockLevel()
        {
            return 10;
        }

        public override bool CanUse()
        {
            return Context.gameState == ScrewGameState.InProgress && Context.GetCouldUseTwoTaskBooster();
        }

        public override async UniTask Use(bool playEff)
        {
            if (Context.GetCouldUseTwoTaskBooster())
            {
                Context.UseBoosterTwoTask();
            }
        }
    }
}