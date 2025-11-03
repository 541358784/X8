using UnityEngine.UI;
public class UIPopupRecoverCoinNewDecoAreaController:UIWindowController
    {
        private Button _buttonPlay;
        private Button CloseBtn;
        public override void PrivateAwake()
        {
            _buttonPlay = GetItem<Button>("Root/Button");
            _buttonPlay.onClick.AddListener(OnPlayBtn);
            CloseBtn = GetItem<Button>("Root/ButtonClose");
            CloseBtn.onClick.AddListener(OnClickCloseBtn);
        }

        public void OnPlayBtn()
        {
            AnimCloseWindow();
        }

        public void OnClickCloseBtn()
        {
            AnimCloseWindow();
        }
    }