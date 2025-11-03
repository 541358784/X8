using Framework.UI;

namespace Scripts.UI
{
    public class UIMiniGameUnlockTip : UIPopupView
    {
        public static void Open()
        {
            Framework.UI.UIManager.Instance.Open<UIMiniGameUnlockTip>("UI/UIMiniGame/Prefab/UIPopupUnlock");
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            BindButtonEvent("Root/BottomGroup/ButtonGo", OnPlayBtnClick);
        }

        private bool _clickPaly;

        private void OnPlayBtnClick()
        {
            _clickPaly = true;
            Close();
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();


            if (_clickPaly)
            {
                var ui = Framework.UI.UIManager.Instance.GetView<UIChapter>();
                ui.GameObject.SetActive(false);

                Framework.UI.UIManager.Instance.Close<UIChapter>();

                //先注释
                //App.Instance.ChangeState<StateMainHome>(new StateMainHome.Data(true));
            }
        }
    }
}