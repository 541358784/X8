using UnityEngine.UI;

namespace Activity.LuckyGoldenEgg
{
    public class UILuckyGoldenEggHelpController : UIWindowController
    {
        private Button _closeBtn;

        public override void PrivateAwake()
        {
            _closeBtn = GetItem<Button>("Root/CloseButton");
            _closeBtn.onClick.AddListener(OnBtnCLose);
        }

        private void OnBtnCLose()
        {
            AnimCloseWindow();
        }
    }
}