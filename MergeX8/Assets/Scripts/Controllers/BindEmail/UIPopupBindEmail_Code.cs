using System;
using System.Globalization;
using System.Net.Mail;
using DragonPlus;
using DragonU3DSDK.Account;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.BindEmail
{
    public class UIPopupBindEmail_Code : MonoBehaviour
    {
        private Button _next;
        private Button _pre;
        
        private Button _protocol;
        private Transform _agree;
        private LocalizeTextMeshProUGUI _errorTips;
        
        private Button _close;
        private UIPopupBindEmailController _rootMono;

        private InputField _inputField;
        private Text _tipsText;

        private bool _isAgree = true;

        private LocalizeTextMeshProUGUI _emailText;
        
        private void Awake()
        {
            _emailText = transform.Find("BG/Text1").GetComponent<LocalizeTextMeshProUGUI>();
            
            _close = transform.Find("ButtonClose").GetComponent<Button>();
            _close.onClick.AddListener(() =>
            {
                _rootMono.ShowErrorTips("是否退出 code");
            });

            _next = transform.Find("ButtonNext").GetComponent<Button>();
            _next.onClick.AddListener(OnClickNext);
            _next.interactable = _isAgree;

            _pre = transform.Find("ButtonPrevious").GetComponent<Button>();
            _pre.onClick.AddListener(() =>
            {
                _rootMono.NextStage(2);
            });
            
            _protocol = transform.Find("ButtonProtocol").GetComponent<Button>();
            _protocol.onClick.AddListener(OnClickProtocol);
            
            _agree = transform.Find("ButtonProtocol/Agree");
            _agree.gameObject.SetActive(_isAgree);
            
            _errorTips = transform.Find("Error/Error1").GetComponent<LocalizeTextMeshProUGUI>();
            _errorTips.SetText("");
            
            _inputField = transform.Find("InputField").GetComponent<InputField>();      
            _inputField.onValueChanged.AddListener((param) => { OnChangeInputField(); });
            _inputField.characterLimit = 30;
            _tipsText = transform.Find("InputField/Text").GetComponent<Text>();
        }

        private void OnChangeInputField()
        {
            _tipsText.text = _inputField.text;
        }

        private void Start()
        {
            CommonUtils.SetShieldButUnEnable(_next.gameObject);
        }

        private void OnEnable()
        {
            _inputField.text = "";
            _tipsText.text = "";
            
            _isAgree = true;
            var email = StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Email;
            _emailText.SetText(email);
            _errorTips.SetText("");
        }

        public void Init(UIPopupBindEmailController mono)
        {
            _rootMono = mono;
        }

        private void OnClickProtocol()
        {
            _isAgree = !_isAgree;
            _agree.gameObject.SetActive(_isAgree);
            _next.interactable = _isAgree;
        }

        private void OnClickNext()
        {
            if(!_isAgree)
                return;
            
            if (_inputField.text.IsEmptyString())
            {
                _errorTips.SetTerm("ui_bind_email_verifyError");
                return;
            }

            var email = StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Email;
            WaitingManager.Instance.OpenWindow();
            AccountManager.Instance.EmailAccountBindEmailVerify(email,_inputField.text, (b, c) =>
            {
                WaitingManager.Instance.CloseWindow();
                if (b)
                {
                    _rootMono.NextStage(4);
                }
                else
                {
                    _errorTips.SetTerm(_rootMono.GetErrorKeyByCode(c));
                }
            });
        }
    }
}