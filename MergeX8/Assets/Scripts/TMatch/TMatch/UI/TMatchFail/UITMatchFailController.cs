using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.TMatch;
using Framework;
using UnityEngine.UI;
using DragonU3DSDK.Storage;
using TMPro;
using System;
using UnityEngine;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Network.API.Protocol;

namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UISettlementFail")]
    public class UITMatchFailController : UIPopup
    {
        [ComponentBinder("FailGroup")] private Transform normalLiveFailGroup;
        [ComponentBinder("InfiniteGroup")] private Transform infiniteFailGroup;

        [ComponentBinder("Root/InsideGroup/PropsGruop/PropItem1/TagImage/NumberText")]
        private LocalizeTextMeshProUGUI infiniteTimeText1;

        [ComponentBinder("Root/InsideGroup/PropsGruop/PropItem2/TagImage/NumberText")]
        private LocalizeTextMeshProUGUI infiniteTimeText2;

        private bool item1IsInfinite = false;
        private bool item2IsInfinite = false;

        public override Action EmptyCloseAction => null;

        public override void OnViewOpen(UIViewParam param)
        {
            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Failed);
            base.OnViewOpen(param);
            TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchSystem.LevelController.LevelData.level);
            CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);

            transform.Find($"Root/PlayButton").GetComponent<Button>().onClick.AddListener(TryAgainOnClickWithAd);
            transform.Find($"Root/CloseButton").GetComponent<Button>().onClick.AddListener(CloseOnClickWithAd);

            DragonPlus.Config.TMatchShop.ItemConfig lightingItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostLightingItemId);
            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterLightningUnlock)
            {
                transform.Find($"Root/InsideGroup/PropsGruop/PropItem1").GetComponent<Button>().onClick.AddListener(() =>
                {
                    AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
                    BoosetItemOnClick(lightingItem);
                });
                RefreshBoostItem(lightingItem);
            }
            else
            {
                LockBoostItem(lightingItem);
            }

            DragonPlus.Config.TMatchShop.ItemConfig clockItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostClockItemId);
            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterClockUnlock)
            {
                transform.Find($"Root/InsideGroup/PropsGruop/PropItem2").GetComponent<Button>().onClick.AddListener(() =>
                {
                    AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
                    BoosetItemOnClick(clockItem);
                });
                RefreshBoostItem(clockItem);
            }
            else
            {
                LockBoostItem(clockItem);
            }

            var isUnlimited = EnergyModel.Instance.GetEnterGameIsUnlimitedState();
            normalLiveFailGroup.gameObject.SetActive(!isUnlimited);
            infiniteFailGroup.gameObject.SetActive(isUnlimited);


            EventDispatcher.Instance.AddEventListener(EventEnum.BuyEnergyInOutLives, OnBuyEnergyInOutLivesEvt);
        }

        public override async Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BuyEnergyInOutLives, OnBuyEnergyInOutLivesEvt);
            await base.OnViewClose();
        }

        private void RefreshBoostItem(DragonPlus.Config.TMatchShop.ItemConfig item)
        {
            string subRoot = "";
            bool select = false;
            bool isInfinite = false;
            if (item.GetItemType() == ItemType.TMLighting)
            {
                subRoot = "PropItem1";
                select = StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseLighting;
                isInfinite = UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMLightingInfinity);
                item1IsInfinite = isInfinite;
            }
            else if (item.GetItemType() == ItemType.TMClock)
            {
                subRoot = "PropItem2";
                select = StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseClock;
                isInfinite = UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMClockInfinity);
                item2IsInfinite = isInfinite;
            }

            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/NumberTag/NbText").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemModel.Instance.GetNum(item.id).ToString());
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/CheckTag").gameObject.SetActive(select && ItemModel.Instance.GetNum(item.id) > 0 && !isInfinite);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/BuyTag").gameObject.SetActive(ItemModel.Instance.GetNum(item.id) == 0 && !isInfinite);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/NumberTag").gameObject.SetActive(!isInfinite);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/Icon").gameObject.SetActive(true);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/Lock").gameObject.SetActive(false);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/InfiniteTag").gameObject.SetActive(false);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/TagImage").gameObject.SetActive(isInfinite);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/Countless").gameObject.SetActive(isInfinite);
        }

        private void LockBoostItem(DragonPlus.Config.TMatchShop.ItemConfig item)
        {
            string subRoot = "";
            int unlockLevel = 1;
            if (item.GetItemType() == ItemType.TMLighting)
            {
                subRoot = "PropItem1";
                unlockLevel = TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterLightningUnlock;
            }
            else if (item.GetItemType() == ItemType.TMClock)
            {
                subRoot = "PropItem2";
                unlockLevel = TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterClockUnlock;
            }

            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/CheckTag").gameObject.SetActive(false);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/NumberTag").gameObject.SetActive(false);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/Icon").gameObject.SetActive(false);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/BuyTag").gameObject.SetActive(false);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/Lock").gameObject.SetActive(true);
            transform.Find($"Root/InsideGroup/PropsGruop/{subRoot}/Lock/LockText").GetComponent<TextMeshProUGUI>().SetText($"Lv.{unlockLevel}");
        }

        private void BoosetItemOnClick(DragonPlus.Config.TMatchShop.ItemConfig item)
        {
            if (item.GetItemType() == ItemType.TMLighting && item1IsInfinite) return;
            if (item.GetItemType() == ItemType.TMClock && item2IsInfinite) return;
            if (ItemModel.Instance.GetNum(item.id) <= 0)
            {
                UIViewSystem.Instance.Open<UITMatchBuyProps>(new UITMatchBuyPropsData()
                {
                    inItem = item, closeAction = () =>
                    {
                        if (gameObject != null) RefreshBoostItem(item);
                    }
                });
                return;
            }

            if (item.GetItemType() == ItemType.TMLighting)
            {
                StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseLighting =
                    !StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseLighting;
            }
            else if (item.GetItemType() == ItemType.TMClock)
            {
                StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseClock =
                    !StorageManager.Instance.GetStorage<StorageTMatch>().LevelBoost.UseClock;
            }

            RefreshBoostItem(item);
        }

        void TryAgainOnClickWithAd()
        {
            if (RemoveAdModel.Instance.IsUnlock() && (!StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd.RemoveAd) && AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.TM_FINISH_FAIL_GAME))
            {
                AdSubSystem.Instance.PlayInterstital(ADConstDefine.TM_FINISH_FAIL_GAME, b =>
                {
                    if (RemoveAdModel.Instance.TryToAutoOpen())
                    {
                        TryAgainOnClick();
                    }
                    else
                    {
                        TryAgainOnClick();
                    }
                });
            }
            else
            {
                TryAgainOnClick();
            }
        }
        private void TryAgainOnClick()
        {
            if (!ItemModel.Instance.IsEnough((int)ResourceId.TMEnergy) && !EnergyModel.Instance.IsEnergyUnlimited())
            {
                DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                    .GameEventTmEnergyPop,data1:"1");
                UIViewSystem.Instance.Open<UiOutLivesController>();
                return;
            }

            UIViewSystem.Instance.Close<UITMatchFailController>();
            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_TRY_AGAIN);
        }

        void CloseOnClickWithAd()
        {
            if (RemoveAdModel.Instance.IsUnlock() && (!StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd.RemoveAd) && AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.TM_FINISH_FAIL_GAME))
            {
                AdSubSystem.Instance.PlayInterstital(ADConstDefine.TM_FINISH_FAIL_GAME, b =>
                {
                    if (RemoveAdModel.Instance.TryToAutoOpen())
                    {
                        CloseOnClick();
                    }
                    else
                    {
                        CloseOnClick();
                    }
                });
            }
            else
            {
                CloseOnClick();
            }
        }
        private void CloseOnClick()
        {
            SceneFsm.mInstance.ChangeState(StatusType.TripleMatchEntry, new FsmParamTMatchEntry(StatusType.TripleMatch)
            {
                lastMatchLevelData = TMatchSystem.LevelController.LevelData
            });
            // Main.Game.Fsm.ChangeState(FsmStateType.Decoration, new FsmParamDecoration(FsmStateType.TripleMatch)
            // {
            //     lastMatchLevelData = TMatchSystem.LevelController.LevelData
            // });
        }

        private void OnBuyEnergyInOutLivesEvt(BaseEvent evt)
        {
            TryAgainOnClick();
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            if (item1IsInfinite)
            {
                var time1 = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMLightingInfinity);
                if (time1 > 0)
                {
                    infiniteTimeText1.SetText(CommonUtils.FormatPropItemTime(time1));
                }
                else
                {
                    DragonPlus.Config.TMatchShop.ItemConfig lightingItem =
                        TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostLightingItemId);
                    RefreshBoostItem(lightingItem);
                }
            }

            if (item2IsInfinite)
            {
                var time2 = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMClockInfinity);
                if (time2 > 0)
                {
                    infiniteTimeText2.SetText(CommonUtils.FormatPropItemTime(time2));
                }
                else
                {
                    DragonPlus.Config.TMatchShop.ItemConfig clockItem =
                        TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostClockItemId);
                    RefreshBoostItem(clockItem);
                }
            }
        }
    }
}