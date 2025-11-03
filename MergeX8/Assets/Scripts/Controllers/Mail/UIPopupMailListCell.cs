using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using Framework;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMailListCell : MonoBehaviour
{
    private Transform _hint;
    private LocalizeTextMeshProUGUI _titleText;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _button;
    private LocalizeTextMeshProUGUI _buttonText;
    private SystemMail _mail;

    private Button _deleteButton;
    private void Awake()
    {
        _hint = transform.Find("IconGroup/HintIcon");
        _titleText = transform.Find("MidGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find("MidGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _button = transform.Find("Button").GetComponent<Button>();
        _deleteButton = transform.Find("DeleteButton").GetComponent<Button>();
        _deleteButton.gameObject.SetActive(false);
        _button.onClick.AddListener(OnButtonRead);
        _deleteButton.onClick.AddListener(OnButtonDelete);
        _buttonText = transform.Find("Button/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private void OnButtonDelete()
    {
        
    }

    public void RefreshUI(SystemMail mail)
    {
        _mail = mail;
        _hint.gameObject.SetActive(mail.Mail.Rewards.Count > 0);
        _titleText.SetText(MailDataModel.Instance.GetSystemMailTitle(mail));
        _buttonText.SetTerm("UI_button_read");
    }

    private void OnButtonRead()
    {
        UIPopupMailController.Open(_mail);
    }
}