using UnityEngine.UI;
public class UIPopupCoinLeaderBoardNewDecoAreaController:UIWindowController
    {
        private Button _buttonPlay;
        public override void PrivateAwake()
        {
            _buttonPlay = GetItem<Button>("Root/Button");
            _buttonPlay.onClick.AddListener(OnPlayBtn);
        }

        public void OnPlayBtn()
        {
            AnimCloseWindow();
        }
    }