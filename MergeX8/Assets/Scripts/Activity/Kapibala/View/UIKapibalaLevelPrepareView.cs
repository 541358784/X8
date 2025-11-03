using System;
using Decoration;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using TMatch;
using TMPro;
using UnityEngine.UI;
// using GameConfigManager = TMatch.GameConfigManager;

public class UIKapibalaLevelPrepareParam : UIViewParam
{
    public int BigLevel;
    public int SmallLevel;
    public int SortingOrder;
}
namespace TMatch
{
    [AssetAddress("Prefabs/Activity/Kapibala/UIKapibalaLevelPreparation")]
    public class UIKapibalaLevelPrepareView : UIPopup
    {
        private UIKapibalaLevelPrepareParam _param;

        public override Action EmptyCloseAction => null;

        // [ComponentBinder("HelpButton")] private Button HelpButton;
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

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            _param = param as UIKapibalaLevelPrepareParam;
            // HelpButton.onClick.AddListener(GlodenHatterHelpOnClick);

            // TMatchDifficulty difficulty = TMatchDifficulty.Normal;
            tagIcon.gameObject.SetActive(false);

            // CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);

            var levelTextStr = LocalizationManager.Instance.GetLocalizedStringWithFormats($"&key.UI_return_reward_help_target",
                (_param.SmallLevel+1).ToString());
            transform.Find($"Root/Root1/TitleGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>().SetText(levelTextStr);

            DragonPlus.Config.TMatchShop.ItemConfig lightingItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostLightingItemId);
            if (true)
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
            if (true)
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
            Canvas.sortingOrder = _param.SortingOrder;
        }

        public void RefreshAllBoostItem()
        { 
            ItemConfig clockItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostClockItemId);
            ItemConfig lightingItem = TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.OuterBoostLightingItemId);
            RefreshBoostItem(clockItem);
            RefreshBoostItem(lightingItem);
        }

        public override void OnViewDestroy()
        {
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
            transform.Find($"Root/Root1/InsideGroup/PropsGruop/{subRoot}/BGYes").gameObject.SetActive(select && ItemModel.Instance.GetNum(item.id) > 0 && !isInfinite);
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
                UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage, () =>
                {
                    if (this)
                        RefreshAllBoostItem();
                });
                XUtility.WaitFrames(1).AddCallBack(() =>
                {
                    if (UIKapibalaMainController.Instance)
                    {
                        Canvas.sortingOrder = UIKapibalaMainController.Instance.canvas.sortingOrder + 5;
                    }  
                }).WrapErrors();
                // UIViewSystem.Instance.Open<UITMatchBuyProps>(new UITMatchBuyPropsData()
                // {
                //     inItem = item, closeAction = () =>
                //     {
                //         if (gameObject != null) RefreshBoostItem(item);
                //     }
                // });
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

        private bool IsWaiting = false;
        private async void PlayOnClick()
        {
            if (KapibalaModel.Instance.Storage.Life <= 0)
            {
                UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage);
                XUtility.WaitFrames(1).AddCallBack(() =>
                {
                    if (UIKapibalaMainController.Instance)
                    {
                        Canvas.sortingOrder = UIKapibalaMainController.Instance.canvas.sortingOrder + 5;
                    }  
                }).WrapErrors();
                return;
            }
            if (IsWaiting)
                return;
            IsWaiting = true;
            KapibalaModel.Instance.DealStartGame();
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapibalaLevelEnter,data1:_param.BigLevel.ToString(),data2:_param.SmallLevel.ToString());
            int layoutId = KapibalaModel.Instance.LevelConfig[_param.BigLevel].SmallLevels[_param.SmallLevel];
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
                await XUtility.WaitSeconds(1f);
            }
            SceneFsm.mInstance.ChangeState(StatusType.TripleMatch, new FsmParamTMatch(0,
                TMatchConfigManager.Instance.GetLayout(0, layoutId),TMGameType.Kapibala));
            await XUtility.WaitFrames(1);
            UIViewSystem.Instance.Close<UIKapibalaLevelPrepareView>();
            UIKapibalaMainController.Hide();
            global::UIManager.Instance.extraSiblingIndex = 550;
            DecoManager.Instance.CurrentWorld.HideByPosition();
            // global::UIRoot.Instance.EnableEventSystem = true;
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<UIKapibalaLevelPrepareView>();
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
    }
}