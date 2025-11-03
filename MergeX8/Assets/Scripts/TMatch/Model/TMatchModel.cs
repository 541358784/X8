using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABTest;
using Decoration;
using Dlugin;
using DragonPlus;
// using DragonPlus.Config.Game;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Framework;
// using IAPChecker;
using OutsideGuide;
using SRF;
using UnityEngine;
using FrameworkUIManager = UIManager;

namespace TMatch
{
    public static class ChangeShopStateUtils
    {
        public static Shop ChangeTableShopToTMatchShop(this TableShop tableShop)
        {
            return TMatchShopConfigManager.Instance.ShopList.Find((a) => a.id == tableShop.id);
        }
        public static TableShop ChangeTMatchShopToTableShop(this Shop shop)
        {
            return GlobalConfigManager.Instance.GetTableShopByID(shop.id);
        }
    }

    public partial class TMatchModel : GlobalSystem<TMatchModel>, IInitable
    {
        public static GameObjectPoolManager ObjectPoolMgr { get; private set; }
        public StorageTMatch storageTMatch;

        private IAPModel _iap;

        private float _stayTime;

        public IAPModel IAP => _iap ??= new IAPModel();

        public bool IsUnlocked => IsUnlock();
        
        public void Init()
        {
            storageTMatch = StorageManager.Instance.GetStorage<StorageTMatch>();
            ObjectPoolMgr = new GameObjectPoolManager("TMatchObjectPoolRoot");
            // global::EventDispatcher.Instance.AddEventListener(global::EventEnum.BackHomeStep, OnBackHome);
        }

        // public void OnBackHome(global::BaseEvent evt)
        // {
        //     TaskSystem.Model.Instance.RestartTask();
        // }

        public void Release()
        {
        }
        public bool IsUnlock()
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey("CloseTM"))
                return false;
            
            if (!ABTestManager.Instance.IsOpenTMatch())
                return false;
            
            if (TMatchModel.Instance.storageTMatch.IsUnlock)
                return true;
            if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TMatch))
            {
                TMatchModel.Instance.storageTMatch.IsUnlock = true;
                return true;
            }
            return false;
        }

        private void InitItems()
        {
            foreach (var globalReward in TMatchConfigManager.Instance.GlobalRewardList)
            {
                if (globalReward.rewardType == "BeginReward")
                {
                    for (var i = 0; i < globalReward.rewardID.Length; i++)
                    {
                        var cur = ItemModel.Instance.GetNum(globalReward.rewardID[i]);
                        var add = globalReward.rewardCnt[i] - cur;
                        ItemModel.Instance.Add(globalReward.rewardID[i], add, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CreateProfileTm
                        });
                    }
                }
            }
        }

        public int GetMainLevel()
        {
            int level = storageTMatch.MainLevel;
            if (level <= 0)
            {
                level = 1;
                storageTMatch.MainLevel = 1;
                InitItems();
                // if (Launching.LaunchingInitFileFinish)
                // {
                //     storageTMatch.MainLevel = 1;
                //     InitItems();
                // }
            }

            return level;
        }

        public void MainLevelFinish()
        {
            TMatchConfigManager.Instance.DeleteDynamicCfgCache(storageTMatch.MainLevel);
            storageTMatch.MainLevel++;
            storageTMatch.MainLevelFailCnt = 0;
        }

        public void Enter()
        {
            if (!IsUnlock())
                return;
            global::UIManager.Instance.extraSiblingIndex = 550;
            TMBPModel.Instance.ClearValue();
            // GameObject.Find("FpsButton").AddComponent<DebugButtonController>();
            TMatch.UILoadingEnter.Open(() =>
            {
                // ResourcesManager.Instance.LoadSpriteAtlas(HospitalConst.TMatchSpriteAtlas);
                // UIHomeMainController.mainController.AnimShowMainUI(false, true);
                UIHomeMainController.HideUI();
                FarmModel.Instance.AnimShow(false);
                PlayerManager.Instance.HidePlayer();
                DecoManager.Instance.CurrentWorld.HideByPosition();
                global::UIRoot.Instance.EnableEventSystem = true;
                //UIRoot.Instance.ToPortrait();
                SceneFsm.mInstance.ChangeState(StatusType.TripleMatchEntry,new FsmParamTMatchEntry(SceneFsm.mInstance.GetCurrSceneType()));
            });

            // 记录进入时间
            _stayTime = Time.time;
            DragonPlus.GameBIManager.SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmMainui);
        }

        public void Exit()
        {
            global::UIRoot.Instance.EnableEventSystem = false;
            global::UIManager.Instance.extraSiblingIndex = 0;
            global::UIManager.Instance.UpdateUIOrder();
            DragonPlus.AudioManager.Instance.PlayMusic(1, true);
            // GameObject.Find("FpsButton").RemoveComponentIfExists<DebugButtonController>();
            UIViewSystem.Instance.Close<UILobbyView>();
            TMatch.UILoadingExit.Open(() =>
            {
                //UIRoot.Instance.ToLandscape();
                UIManager.Instance.ClearAllWindows();
                // ResourcesManager.Instance.UnloadSpriteAtlasImmediate(HospitalConst.TMatchSpriteAtlas);
                SceneFsm.mInstance.ChangeState(StatusType.BackHome);
                
                // UIHomeMainController.mainController.AnimShowMainUI(true, true);
                FarmModel.Instance.AnimShow(true);
                UIHomeMainController.ShowUI();
                PlayerManager.Instance.RecoverPlayer();
                DecoManager.Instance.CurrentWorld.ShowByPosition();
                global::UIRoot.Instance.EnableEventSystem = true;
            });

            DragonPlus.GameBIManager.SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmExit, Time.time - _stayTime);
        }

        public void Purchase(int id, Transform sender)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                FrameWorkUINotice.Open(new UINoticeData
                {
                    DescString =
                        LocalizationManager.Instance.GetLocalizedString("&key.UI_store_common_offlineerr_text"),
                    HasCloseButton = false
                });
                return;
            }

            StoreModel.Instance.Purchase(id, param1:sender);
        }
        
        public void OnPurchase(TableShop tableShop, Transform sender, bool isUnfulfilled)
        {
            var shop = tableShop.ChangeTableShopToTMatchShop();
            var shopConfig = TMatchConfigManager.Instance.ShopConfigList.Find(item => item.id == shop.id);
            if (shopConfig == null)
            {
                if (isUnfulfilled)
                    IAPChecker.UIMain.Open(new IAPChecker.UIData {UIType = IAPChecker.UIType.ContactUs});
                return;
            }

            IAP.AddPurchasedTimes(shop.id, 1);

            if (isUnfulfilled)
            {
                IAPChecker.UIMain.Open(new IAPChecker.UIData
                {
                    UIType = IAPChecker.UIType.Item,
                    ItemIds = shopConfig.itemId.ToList(),
                    ItemCounts = shopConfig.itemCnt.ToList(),
                });
            }

            ItemModel.Instance.Add(shopConfig.itemId.ToList(), shopConfig.itemCnt.ToList(), new DragonPlus.GameBIManager.ItemChangeReasonArgs
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap
            }, true,addType:1);

            var removeAdPackShopId1 = RemoveAdModel.Instance.GetRemoveAdPackShopId(0);
            var removeAdPackShopId2 = RemoveAdModel.Instance.GetRemoveAdPackShopId(1);
            if (shop.id == removeAdPackShopId1 || 
                shop.id == removeAdPackShopId2)
            {
                RemoveAdModel.Instance.SetRemoveAd();                   
            }

            var view = sender != null ? sender.GetComponent<UIView>() : null;
            EventDispatcher.Instance.DispatchEvent(new IAPSuccessEvent(shop, view));
        }

        public static List<DebugCfg> GetDebugCfg()
        {
            var list = new List<DebugCfg>
            {
                new DebugCfg
                {
                    TitleStr = "清理",
                    ClickCallBack = (param1, param2) =>
                    {
                        foreach (var itemConfig in DragonPlus.Config.TMatchShop.TMatchShopConfigManager.Instance.ItemConfigList)
                        {
                            if (itemConfig.id > 100000 && itemConfig.id <= 101000)
                                ItemModel.Instance.Clear(itemConfig.id,
                                    new DragonPlus.GameBIManager.ItemChangeReasonArgs
                                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug});
                        }

                        for (var i = 10112; i <= 10125; i++)
                        {
                            DecoGuideManager.Instance.ClearGameGuide(i);
                        }

                        StorageManager.Instance.GetStorage<StorageTMatch>().Clear();
                        StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel = 1;
                        StorageManager.Instance.GetStorage<StorageCurrencyTMatch>().Clear();
                    }
                },
                new DebugCfg
                {
                    TitleStr = "设置关卡进度",
                    ClickCallBack = (param1, param2) =>
                    {
                        Instance.storageTMatch.MainLevel = param1.ToInt();
                        StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel = param1.ToInt() % 10;
                        StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.CurIndex = param1.ToInt() / 10;
                    }
                },
                new DebugCfg
                {
                    TitleStr = "横屏",
                },
                new DebugCfg
                {
                    TitleStr = "竖屏",
                },
            };
            return list;
        }

        private const int TMatchIdScale = 1000;
        public bool IsTMatchResId(int resourceId)
        {
            return resourceId > (100000 * TMatchIdScale) && resourceId < (200000 * TMatchIdScale);
        }
        public int ChangeToTMatchId(int resourceId)
        {
            return resourceId / TMatchIdScale;
        }
        public TMatchReviveSystem GetReviveBottonShowType()
        {
            TMatchReviveSystem showType = TMatchReviveSystem.NoSystem;
            TMatchReviveSystem lastShowType = (TMatchReviveSystem)StorageManager.Instance.GetStorage<StorageTMatch>().ReviveShowTag;
            switch (lastShowType)
            {
                case TMatchReviveSystem.NoSystem:
                case TMatchReviveSystem.GoldenPass:
                    //上次展示的bp或者没有展示 优先展示复活礼包
                    if (ReviveGiftPackController.Instance.model.CanShow())
                    {
                        showType = TMatchReviveSystem.ReviveGiftPack;
                    }
                    else
                    {
                        showType = TMBPModel.Instance.ShowBPBuy() ? TMatchReviveSystem.GoldenPass : TMatchReviveSystem.NoSystem;
                    }

                    break;
                case TMatchReviveSystem.ReviveGiftPack:
                    //上次展示的复活礼包 本轮优先判断bp
                    if (TMBPModel.Instance.ShowBPBuy())
                    {
                        showType = TMatchReviveSystem.GoldenPass;
                    }
                    else if (ReviveGiftPackController.Instance.model.CanShow())
                    {
                        showType = TMatchReviveSystem.ReviveGiftPack;
                    }
                    break;
            }

            return showType;
        }
    }
}
