using System.Collections.Generic;
using System.Threading.Tasks;
// using DragonPlus.Config.Game;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Framework;
using UnityEngine;


namespace TMatch
{
    public class TMatchGoldenHatterSystem : GlobalSystem<TMatchGoldenHatterSystem>, IInitable
    {
        public const int LightingItemId = 22;
        public const int ClockItemId = 23;

        public int GoldenHatterMarkValue;

        public void Init()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_START, OnGameStartEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_WIN, OnGameWinEvt);
        }

        public void Release()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_START, OnGameStartEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_WIN, OnGameWinEvt);
        }

        private async void OnGameStartEvt(BaseEvent evt)
        {
            await Task.Delay(1000);

            //引导-闪电
            if (TMatchModel.Instance.GetMainLevel() == TMatchConfigManager.Instance.GlobalList[0].MatchLevelGoldenHatterUnlock - 2)
            {
                // if (!GuideSubSystem.Instance.IsFinished("GUIDE_121"))
                // {
                //     List<TMatchBaseItem> baseItems = new List<TMatchBaseItem>();
                //     DragonPlus.Config.Game.Item item = GameConfigManager.Instance.GetItem(LightingItemId);
                //     for (int i = 0; i < 3; i++)
                //     {
                //         TMatchBaseItem baseItem = TMatchItemSystem.Instance.Create(item.MatchItemId);
                //         Vector3 min = TMatchEnvSystem.Instance.SceneRandomPosMin + new Vector3(1.0f, 0.0f, 1.0f);
                //         Vector3 max = TMatchEnvSystem.Instance.SceneRandomPosMax + new Vector3(-1.0f, 0.0f, -1.0f);
                //         baseItem.RandomPos(min, max);
                //         TMatchItemSystem.Instance.Explode(baseItem.GameObject.transform.position);
                //         baseItems.Add(baseItem);
                //     }
                //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchItemGlodenHatter, baseItems[0].GameObject.transform, LightingItemId.ToString());
                // }
            }
            //引导-时钟
            else if (TMatchModel.Instance.GetMainLevel() == TMatchConfigManager.Instance.GlobalList[0].MatchLevelGoldenHatterUnlock - 1)
            {
                // if (!GuideSubSystem.Instance.IsFinished("GUIDE_122"))
                // {
                //     List<TMatchBaseItem> baseItems = new List<TMatchBaseItem>();
                //     DragonPlus.Config.Game.Item item = GameConfigManager.Instance.GetItem(ClockItemId);
                //     for (int i = 0; i < 3; i++)
                //     {
                //         TMatchBaseItem baseItem = TMatchItemSystem.Instance.Create(item.MatchItemId);
                //         Vector3 min = TMatchEnvSystem.Instance.SceneRandomPosMin + new Vector3(1.0f, 0.0f, 1.0f);
                //         Vector3 max = TMatchEnvSystem.Instance.SceneRandomPosMax + new Vector3(-1.0f, 0.0f, -1.0f);
                //         baseItem.RandomPos(min, max);
                //         TMatchItemSystem.Instance.Explode(baseItem.GameObject.transform.position);
                //         baseItems.Add(baseItem);
                //     }
                //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchItemGlodenHatter, baseItems[0].GameObject.transform, ClockItemId.ToString());
                // }
            }

            GoldenHatterMarkValue = 0;
            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelGoldenHatterUnlock)
            {
                GoldenHatterMarkValue = StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt;
                StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt = 0;

                if (GoldenHatterMarkValue > 0)
                {
                    GlodenHatter glodenHatter = TMatchConfigManager.Instance.GetGlodenHatterByTimes(GoldenHatterMarkValue);
                    if (glodenHatter != null)
                    {
                        for (int i = 0; i < glodenHatter.rewardID.Length; i++)
                        {
                            DragonPlus.Config.TMatchShop.ItemConfig item = TMatchShopConfigManager.Instance.GetItem(glodenHatter.rewardID[i]);
                            for (int j = 0; j < glodenHatter.rewardCnt[i]; j++)
                            {
                                TMatchBaseItem baseItem = TMatchItemSystem.Instance.Create(item.subId);
                                TMatchItemSystem.Instance.Explode(baseItem.GameObject.transform.position);
                            }
                        }
                    }
                }
            }
        }

        private void OnGameWinEvt(BaseEvent evt)
        {
            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelGoldenHatterUnlock)
            {
                StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt = GoldenHatterMarkValue + 1;
            }
        }
    }
}