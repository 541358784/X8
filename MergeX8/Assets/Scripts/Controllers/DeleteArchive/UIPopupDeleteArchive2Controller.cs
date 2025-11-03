
using System;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIPopupDeleteArchive2Controller : UIWindowController
{
    private Button _buttonClose;
    private Button _cancelBtn;
    
    private Button _confirmButton;
    private GameObject _redBg;
    private GameObject _GaryBg;
    private LocalizeTextMeshProUGUI _redText;
    private LocalizeTextMeshProUGUI _garyText;
    private int random;
    private InputField _inputField;
    private LocalizeTextMeshProUGUI _text;
    private Text _inputFieldDef;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnClose);
        _cancelBtn = GetItem<Button>("Root/CancellationButton");
        _cancelBtn.onClick.AddListener(OnClose);
        _confirmButton = GetItem<Button>("Root/ConfirmButton");
        _confirmButton.onClick.AddListener(OnConfirm);
        _redBg = GetItem("Root/ConfirmButton/RedBG");
        _GaryBg = GetItem("Root/ConfirmButton/GreyBG");
        _redText = GetItem<LocalizeTextMeshProUGUI>("Root/ConfirmButton/RedText");
        _garyText = GetItem<LocalizeTextMeshProUGUI>("Root/ConfirmButton/GreyText");
        _text = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        _inputField = GetItem<InputField>("Root/InputField");
        _inputFieldDef = GetItem<Text>("Root/InputField/Placeholder");
        _inputField.onValueChanged.AddListener((param) => { OnChangeInputField(); });
        _inputFieldDef.text = LocalizationManager.Instance.GetLocalizedString("ui_delete_account_desc3");
    }

    private void OnChangeInputField()
    {
        UpdateStatus();
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        random = Random.Range(1000, 9999);
        _text.SetText(String.Format(LocalizationManager.Instance.GetLocalizedString("ui_delete_account_desc2"),random));
        UpdateStatus();
    }
    public void UpdateStatus()
    {
        if (_inputField.text!=random.ToString())
        {
            _redBg.SetActive(false);
            _GaryBg.SetActive(true);
            _garyText.gameObject.SetActive(true);
            _redText.gameObject.SetActive(false);
            _confirmButton.interactable = false;
            _garyText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_button_yes"));

        }
        else
        {
            _redBg.SetActive(true);
            _GaryBg.SetActive(false);
            _garyText.gameObject.SetActive(false);
            _redText.gameObject.SetActive(true);
            _confirmButton.interactable = true;
            _redText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_button_yes"));
        }
    }
    private void OnClose()
    {
        AnimCloseWindow();
    }
    private void OnConfirm()
    {
        var cDeleteAccount = new CDeleteAccount
        {
            Confirm = true
        };
        APIManager.Instance.Send(cDeleteAccount, (SDeleteAccount SDeleteAccount) =>
        {
            DebugCmdExecute.ClearStorage();
            PlayerPrefs.DeleteKey("RunOnce");
            AccountManager.Instance.Clear();
            
            Application.Quit();
        }, null);
 
    }
   
}