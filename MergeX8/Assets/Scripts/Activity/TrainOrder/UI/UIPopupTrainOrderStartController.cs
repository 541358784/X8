using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TrainOrder
{
    public class UIPopupTrainOrderStartController : UIWindowController
    {
        private Button _buttonClose;
        private Button _button;
        private LocalizeTextMeshProUGUI _textTime;

        public override void PrivateAwake()
        {
            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _button = GetItem<Button>("Root/Button");
            _buttonClose.onClick.AddListener(delegate { AnimCloseWindow(() =>
            {
                TrainOrderModel.Instance.TryOpenMain();
            }); });
            _button.onClick.AddListener(delegate { AnimCloseWindow(() =>
            {
                TrainOrderModel.Instance.TryOpenMain();
            }); });

            _textTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        }


        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            InvokeRepeating(nameof(UpdateTime), 0, 1);
        }

        protected override void OnCloseWindow(bool destroy = false)
        {
            base.OnCloseWindow(destroy);
            CancelInvoke(nameof(UpdateTime));
        }

        private void UpdateTime()
        {
            _textTime.SetText(TrainOrderModel.Instance.GetActivityLeftTimeString());
        }
        
        public static void Open()
        {
            UIManager.Instance.OpenWindow(UINameConst.UIPopupTrainOrderStart);
        }
        
    }
}