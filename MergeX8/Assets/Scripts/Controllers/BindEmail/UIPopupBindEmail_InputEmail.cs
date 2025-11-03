using System;
using System.Globalization;
using System.Net.Mail;
using DragonPlus;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.BindEmail
{
    public class UIPopupBindEmail_InputEmail : MonoBehaviour
    {
        private Button _next;
        private Button _protocol;
        private Transform _agree;
        private LocalizeTextMeshProUGUI _errorTips;
        
        private Button _close;
        private UIPopupBindEmailController _rootMono;

        private InputField _inputField;
        private Text _tipsText;

        private bool _isAgree = false;
        
        private void Awake()
        {
            _close = transform.Find("ButtonClose").GetComponent<Button>();
            _close.onClick.AddListener(() =>
            {
                _rootMono.ShowErrorTips("是否退出");
            });

            _next = transform.Find("ButtonNext").GetComponent<Button>();
            _next.onClick.AddListener(OnClickNext);
            _next.interactable = _isAgree;
            
            _protocol = transform.Find("ButtonProtocol").GetComponent<Button>();
            _protocol.onClick.AddListener(OnClickProtocol);
            
            _agree = transform.Find("ButtonProtocol/Agree");
            _agree.gameObject.SetActive(_isAgree);
            
            _errorTips = transform.Find("Error/Error1").GetComponent<LocalizeTextMeshProUGUI>();
            _errorTips.SetText("");
            
            _inputField = transform.Find("InputField").GetComponent<InputField>();      
            _inputField.onValueChanged.AddListener((param) => { OnChangeInputField(); });
            _inputField.characterLimit = 500;
            _tipsText = transform.Find("InputField/Text").GetComponent<Text>();
        }

        private void Start()
        {
            CommonUtils.SetShieldButUnEnable(_next.gameObject);
        }

        private void OnChangeInputField()
        {
            _tipsText.text = _inputField.text;

            StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Email = _inputField.text;
        }

        private void OnEnable()
        {
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
            
            if (!IsValidEmail(_inputField.text))
            {
                _errorTips.SetTerm("ui_bind_email_formatError");
                return;
            }
            WaitingManager.Instance.OpenWindow();
            AccountManager.Instance.EmailAccountC2SSendAddress(_inputField.text, (b, c) =>
            {
                WaitingManager.Instance.CloseWindow();
                if (b)
                {
                    _rootMono.NextStage(3);
                }
                else
                {
                    _errorTips.SetTerm(_rootMono.GetErrorKeyByCode(c));
                }
            });
        }
        
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}