using System;
using UnityEngine.UI;

namespace ActivityLocal.ClimbTower.Model
{
    public class UIPopupClimbTowerQuitController : UIWindowController
    {
        private Button _close;
        private Button _yes;
        private Button _no;

        private Action _yesCall;
        
        public override void PrivateAwake()
        {
            _close = transform.Find("Root/CloseButton").GetComponent<Button>();
            _close.onClick.AddListener(() => AnimCloseWindow());
                
            _yes = transform.Find("Root/ButtonYse").GetComponent<Button>();
            _yes.onClick.AddListener(() =>
            {
                _yesCall?.Invoke();
                AnimCloseWindow();
            });
            
            _no = transform.Find("Root/ButtonNo").GetComponent<Button>();
            _no.onClick.AddListener(() => AnimCloseWindow());
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _yesCall = (Action)objs[0];
        }
    }
}