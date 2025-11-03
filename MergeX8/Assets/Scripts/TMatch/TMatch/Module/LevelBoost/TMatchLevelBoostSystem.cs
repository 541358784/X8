using System.Threading.Tasks;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Storage;
using Framework;


namespace TMatch
{
    public class TMatchLevelBoostSystem : GlobalSystem<TMatchLevelBoostSystem>, IInitable
    {
        public void Init()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_START, OnGameStartEvt);
        }

        public void Release()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_START, OnGameStartEvt);
        }

        private async void OnGameStartEvt(BaseEvent evt)
        {
            UseLevelBoostClock();
            UseLevelBoostLighting();
        }

        private async void UseLevelBoostClock()
        {
            if (StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseClock ||
                UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMClockInfinity))
            {
                await Task.Delay(1000);

                EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(
                    UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMClockInfinity) ? TMatchShopConfigManager.Instance.OuterBoostInfinityClockItemId : TMatchShopConfigManager.Instance.OuterBoostClockItemId));
            }
        }

        private async void UseLevelBoostLighting()
        {
            if (StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseLighting ||
                UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMLightingInfinity))
            {
                await Task.Delay(1000);

                EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(
                    UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMLightingInfinity) ? TMatchShopConfigManager.Instance.OuterBoostInfinityLightingItemId : TMatchShopConfigManager.Instance.OuterBoostLightingItemId));
            }
        }
    }
}