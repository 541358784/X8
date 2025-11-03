using Framework.UI;
using Framework.Utils;
using MiniGame;

namespace Scripts.UI
{
    public class UIMiniGameFail : UIPopupView
    {
        protected override bool AutoBgClose => false;
        protected override bool CommonCloseBtn => false;

        public static void Open()
        {
            Framework.UI.UIManager.Instance.Open<UIMiniGameFail>("NewMiniGame/UIMiniGame/Prefab/UIFail");
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            BindButtonEvent("Root/ButtonRetry", OnRetryClicked);
        }

        private void OnRetryClicked()
        {
            EventBus.Send<EventMiniGameLevelFailRetryClicked>();
            Close();
        }
    }
}