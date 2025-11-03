using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.BindEmail
{
    public class UIPopupBindEmail_ErrorTips : MonoBehaviour
    {
        private Button _yes;
        private Button _no;

        private LocalizeTextMeshProUGUI _text;
        private UIPopupBindEmailController _rootMono;

        private void Awake()
        {
            _text = transform.Find("BG/Text1").GetComponent<LocalizeTextMeshProUGUI>();
            
            _yes = transform.Find("ButtonYes").GetComponent<Button>();
            _yes.onClick.AddListener(() =>
            {
                if (StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Stage == 3)
                    StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Stage = 0;
                
                _rootMono.AnimCloseWindow();
            });
            
            _no = transform.Find("ButtonNo").GetComponent<Button>();
            _no.onClick.AddListener(() =>
            {
                transform.gameObject.SetActive(false);
            });
        }

        public void Init(UIPopupBindEmailController mono)
        {
            _rootMono = mono;
        }
        
        public void ShowErrorTips(string key)
        {
            //_text.SetTerm(key);
        }
    }
}