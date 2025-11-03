
    using DragonPlus;
    using ExtraEnergy;
    using UnityEngine.UI;

    public class UIPopupExtraEnergyStartController : UIWindowController
    {
        private LocalizeTextMeshProUGUI _timeText;
        private Button _button;
        private Button _buttonClose;
        public override void PrivateAwake()
        {
            _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
            _button = GetItem<Button>("Root/Button");
            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _button.onClick.AddListener(OnBtnBuy);
            _buttonClose.onClick.AddListener(OnBtnClose);
            InvokeRepeating("RefreshTime",0,1);
        }

        private void OnBtnBuy()
        {
            AnimCloseWindow(() =>
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupBuyEnergy, "diamond_lack_energy");
            });
        }

        public void RefreshTime()
        {
            _timeText.SetText(ExtraEnergyModel.Instance.GetActivityLeftTimeString());
            if(ExtraEnergyModel.Instance.GetActivityLeftTime()<=0)
                AnimCloseWindow();
        }
        private void OnBtnClose()
        { 
            AnimCloseWindow();
        }

    }
