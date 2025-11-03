using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.BindEmail
{
    public class UIPopupBindEmail_Eighteen : MonoBehaviour
    {
        private Button _yes;
        private Button _no;

        private Button _close;
        private UIPopupBindEmailController _rootMono;
        
        private void Awake()
        {
            _yes = transform.Find("ButtonYes").GetComponent<Button>();
            _yes.onClick.AddListener(() =>
            {
                _rootMono.NextStage(2);
            });
            
            _no = transform.Find("ButtonNo").GetComponent<Button>();
            _no.onClick.AddListener(() =>
            {
                _rootMono.AnimCloseWindow();
            });
            
            _close = transform.Find("ButtonClose").GetComponent<Button>();
            _close.onClick.AddListener(() =>
            {
                _rootMono.AnimCloseWindow();
            });
        }
        
        public void Init(UIPopupBindEmailController mono)
        {
            _rootMono = mono;
        }
    }
}