using System;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using GameConfigManager = TMatch.GameConfigManager;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UILevelPreparation")]
    public class UITMatchLevelPrepareView : UIPopup
    {
        private static Transform lightingTopView;
        private static Transform clockTopView;

        public static Transform GetLightingTopView()
        {
            return lightingTopView;
        }

        public static Transform GetClockTopView()
        {
            return clockTopView;
        }

        public override Action EmptyCloseAction => null;

        [ComponentBinder("HelpButton")] private Button HelpButton;
        [ComponentBinder("PlayButton")] private Button PlayButton;
        [ComponentBinder("CloseButton")] private Button CloseButton;

        [ComponentBinder("Root/Root1/InsideGroup/PropsGruop/PropItem1/TagImage/NumberText")]
        private LocalizeTextMeshProUGUI infiniteTimeText1;

        [ComponentBinder("Root/Root1/InsideGroup/PropsGruop/PropItem2/TagImage/NumberText")]
        private LocalizeTextMeshProUGUI infiniteTimeText2;

        [ComponentBinder("TagIcon")] private Image tagIcon;

        [ComponentBinder("Root/Root1/PlayButton/TagIcon/Text")]
        private LocalizeTextMeshProUGUI tagDifficultText;

        private bool item1IsInfinite = false;
        private bool item2IsInfinite = false;

        public void GuideClickLighting()
        {
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem1").GetComponent<Button>().onClick.Invoke();
        }

        public void GuideClickClock()
        {
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem2").GetComponent<Button>().onClick.Invoke();
        }

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            HelpButton.onClick.AddListener(GlodenHatterHelpOnClick);
            lightingTopView = transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem1");
            clockTopView = transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem2");

            TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchModel.Instance.GetMainLevel());
            tagIcon.gameObject.SetActive(difficulty != TMatchDifficulty.Normal);
            if (difficulty != TMatchDifficulty.Normal)
            {
                var key = difficulty == TMatchDifficulty.Hard ? "UI_levelstart_level_empty1" : "UI_levelstart_level_empty2";
                tagDifficultText.SetTerm(key);
                if (difficulty == TMatchDifficulty.Demon)
                {
                    tagIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, "ui_common_level_tag_2");
                }
            }

            CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);

            var levelTextStr = LocalizationManager.Instance.GetLocalizedStringWithFormats($"&key.UI_return_reward_help_target",
                TMatchModel.Instance.GetMainLevel().ToString());
            transform.Find($"Root/Root1/TitleGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>().SetText(levelTextStr);

            DragonPlus.Config.TMatchShop.ItemConfig lightingItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostLightingItemId);
            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterLightningUnlock)
            {
                transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem1").GetComponent<Button>().onClick.AddListener(() =>
                {
                    AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
                    BoosetItemOnClick(lightingItem);

                    // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.LevelPrepareBoost);
                });
                RefreshBoostItem(lightingItem);

                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.LevelPrepareBoost, transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem1"), 
                //     GameConfigManager.Instance.OuterBoostLightingItemId.ToString());
            }
            else
            {
                LockBoostItem(lightingItem);
            }

            DragonPlus.Config.TMatchShop.ItemConfig clockItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostClockItemId);
            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterClockUnlock)
            {
                transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem2").GetComponent<Button>().onClick.AddListener(() =>
                {
                    AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
                    BoosetItemOnClick(clockItem);

                    // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.LevelPrepareBoost);
                });
                RefreshBoostItem(clockItem);

                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.LevelPrepareBoost, transform.Find($"Root/Root1/InsideGroup/PropsGruop/PropItem2"), 
                //     TMatchShopConfigManager.Instance.OuterBoostClockItemId.ToString());
            }
            else
            {
                LockBoostItem(clockItem);
            }


            PlayButton.onClick.AddListener(PlayOnClick);
            CloseButton.onClick.AddListener(CloseOnClick);

            AddChildView<UITMatchLevelPrepareGlodenHatterView>(transform.Find("Root/Root2").gameObject);
            AddChildView<UITMatchLevelPrepareWeeklyChallengeView>(transform.Find("Root/UIEntrance1").gameObject);
            AddChildView<UITMatchLevelPrepareCollectActivityView>(transform.Find("Root/UIEntrance2").gameObject);
            AddChildView<UITMatchLevelPrepareCollectGuildView>(transform.Find("Root/UIEntrance3").gameObject);

            // GuideSubSystem.Instance.Trigger(GuideTrigger.LevelPrepareOpen, TMatchModel.Instance.GetMainLevel().ToString());

            StartGuide();
        }

        private void StartGuide()
        {
            if (TMatchModel.Instance.TryGuideLighting())
                return;

            if (TMatchModel.Instance.TryGuideClock())
                return;
        }

        public override void OnViewDestroy()
        {
            lightingTopView = null;
            clockTopView = null;
            LobbyTaskSystem.Instance.FinishCurrentTask();
            base.OnViewDestroy();
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

            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/NumberTag/NbText").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemModel.Instance.GetNum(item.id).ToString());
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/CheckTag").gameObject.SetActive(select && ItemModel.Instance.GetNum(item.id) > 0 && !isInfinite);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/BuyTag").gameObject.SetActive(ItemModel.Instance.GetNum(item.id) == 0 && !isInfinite);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/NumberTag").gameObject.SetActive(!isInfinite);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/Icon").gameObject.SetActive(true);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/Lock").gameObject.SetActive(false);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/InfiniteTag").gameObject.SetActive(false);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/TagImage").gameObject.SetActive(isInfinite);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/Countless").gameObject.SetActive(isInfinite);
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

            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/CheckTag").gameObject.SetActive(false);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/NumberTag").gameObject.SetActive(false);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/Icon").gameObject.SetActive(false);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/BuyTag").gameObject.SetActive(false);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/Lock").gameObject.SetActive(true);
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/Lock/LockText").GetComponent<TextMeshProUGUI>().SetText($"Lv.{unlockLevel}");
        }

        private void BoosetItemOnClick(DragonPlus.Config.TMatchShop.ItemConfig item)
        {
            if (item.GetItemType() == ItemType.TMLighting && item1IsInfinite) return;
            if (item.GetItemType() == ItemType.TMClock && item2IsInfinite) return;
            if (ItemModel.Instance.GetNum(item.id) <= 0)
                // if (CurrencyModel.Instance.GetRes(item.GetResouceId()) <= 0)
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

        private void PlayOnClick()
        {
            if (!ItemModel.Instance.IsEnough((int)ResourceId.TMEnergy) && !EnergyModel.Instance.IsEnergyUnlimited())
            {
                DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                    .GameEventTmEnergyPop,data1:"1");
                UIViewSystem.Instance.Open<UiOutLivesController>();
                return;
            }

            Level levelCfg = TMatchConfigManager.Instance.GetLevel(TMatchModel.Instance.GetMainLevel());
            int layoutId = StorageManager.Instance.GetStorage<StorageTMatch>().MainLevelFailCnt >= levelCfg.reduceGradeThreshold ? levelCfg.easierLayoutId : levelCfg.layoutId;
            SceneFsm.mInstance.ChangeState(StatusType.TripleMatch, new FsmParamTMatch(levelCfg.levelId,
                TMatchConfigManager.Instance.GetLayout(levelCfg.levelId, layoutId),TMGameType.Normal));
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<UITMatchLevelPrepareView>();
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
                    DragonPlus.Config.TMatchShop.ItemConfig lightingItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostLightingItemId);
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
                    DragonPlus.Config.TMatchShop.ItemConfig clockItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostClockItemId);
                    RefreshBoostItem(clockItem);
                }
            }
        }

        private void GlodenHatterHelpOnClick()
        {
            UIViewSystem.Instance.Open<UITMatchGoldenHatterHelp>();
        }

        // 其它系统跳转弹窗，存在小游戏能玩时，不弹出
        public static void TryToOpen()
        {
            if (TMatchModel.Instance.GetMainLevel() > UILobbyMainViewLevelButton.MAXLevel) return;
            UIViewSystem.Instance.Open<UITMatchLevelPrepareView>();
        }
    }
}