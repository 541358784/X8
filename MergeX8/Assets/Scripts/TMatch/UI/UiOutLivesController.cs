using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIOutLives")]
    public class UiOutLivesController : UIPopup
    {
        private float _updateTime = 1.0f;

        private TextMeshProUGUI numberText;
        private LocalizeTextMeshProUGUI countDownText;

        [ComponentBinder("Root/ButtonGruop/RefillButton")]
        private Button refillButton;

        private TextMeshProUGUI _textCost;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            _textCost = GetItem<TextMeshProUGUI>("Root/ButtonGruop/RefillButton/Coin/NumberText");
            numberText = this.GetItem<TextMeshProUGUI>("Root/MiddleGruop/NomberText");
            countDownText = this.GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGruop/TimeGruop/TimeText");
            RefreshNumber();
            this.BindEvent("Root/ButtonGruop/RefillButton", OnRefillButtonClicked);
            this.BindEvent("Root/ButtonGruop/SettingButton", OnSettingButtonClick);
            transform.Find("Root/ButtonGruop/SettingButton").gameObject.SetActive(false);
            this.BindEvent("Root/CloseButton", OnCloseButtonClicked);
            // if (MyMain.myGame.IsInMatch())
            // {
            //     transform.Find("Root/ButtonGruop/SettingButton").gameObject.SetActive(LivesBankModel.Instance.GetStorageHelpDict().Count > 0);
            //     DelayShowText();
            // }

            EventDispatcher.Instance.AddEventListener(EventEnum.ENERGY_CHANGED, OnEnergyChangeEvent);
            
            InvokeRepeating("Refresh", 0, 1);
        }

        // public async void DelayShowText()
        // {
        //     await Task.Delay(10);
        //     var useText = transform.Find("Root/ButtonGruop/SettingButton/PlayText")
        //         .GetComponent<LocalizeTextMeshProUGUI>();
        //     var canUseMax = LivesBankModel.Instance.GetStorageHelpDict().Count;
        //     canUseMax = canUseMax > 5 ? 5 : canUseMax;
        //     var textStr = LocalizationManager.Instance.GetLocalizedStringWithFormats($"&key.UI_common_notice9", 
        //         canUseMax.ToString());
        //     useText.SetText(textStr);
        // }

        public override async Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.ENERGY_CHANGED, OnEnergyChangeEvent);
            await base.OnViewClose();
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);

            _updateTime += deltaTime;
            if (_updateTime < 1f)
            {
                return;
            }

            _updateTime = 0f;
        }

        public void Refresh()
        {
            var cd = (int)(ItemAutoClaimModel.Instance.GetNextClaimCD((int)ResourceId.TMEnergy) * 0.001);
            var newText = DragonU3DSDK.Utils.GetTimeString("%mm:%ss", cd);
            if (countDownText.GetText() != newText) countDownText.SetText(newText);
            RefreshNumber();
        }

        private void RefreshNumber()
        {
            numberText.SetText(ItemModel.Instance.GetNum((int)ResourceId.TMEnergy).ToString());

            var id = (int)ResourceId.TMEnergy;
            var max = ItemModel.Instance.GetItemMax(id);
            var num = max - ItemModel.Instance.GetNum(id);
            _textCost.SetText((num * ItemModel.Instance.GetItemPrice(id)).ToString());
        }

        private void OnEnergyChangeEvent(BaseEvent evt)
        {
            RefreshNumber();
            if (ItemModel.Instance.IsNumMax((int)ResourceId.TMEnergy))
            {
                OnCloseButtonClicked();
            }
        }

        private async void OnRefillButtonClicked()
        {
            refillButton.interactable = false;

            var id = (int)ResourceId.TMEnergy;
            var max = ItemModel.Instance.GetItemMax(id);
            var num = max - ItemModel.Instance.GetNum(id);
            if (num <= 0)
                return;

            if (ItemModel.Instance.Buy(id, num, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug
                }))
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.BuyEnergyInOutLives);
                EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(ResourceId.TMCoin));
            }
            else
            {
                UIViewSystem.Instance.Open<ShopPartPopup>();
            }

            UIViewSystem.Instance.Close<UiOutLivesController>();
        }

        private void OnSettingButtonClick()
        {
            // if (MyMain.myGame.IsInMatch())
            // {
            //     LivesBankModel.Instance.TryToUseLivesInMatch();
            // }
            // else
            // {
            //     EventDispatcher.Instance.DispatchEvent(new JumpToLobbyNavigationTypeEvent(UILobbyNavigationBarView.UIType.Energy));
            // }
            // UIViewSystem.Instance.Close<UITMatchLevelPrepareView>();
            // UIViewSystem.Instance.Close<UiOutLivesController>();

        }

        private void OnCloseButtonClicked()
        {
            UIViewSystem.Instance.Close<UiOutLivesController>();
        }
    }
}