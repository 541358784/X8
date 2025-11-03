using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLanguageItem : MonoBehaviour
{
    private Text _languageText;
    private Button _buttonSelect;
    private Image _bgImg;
    private GameObject _choseIcon;

    private Action<string, UIPopupLanguageItem> _clickCallback;
    private string _locale;

    private void Awake()
    {
        _buttonSelect = GetComponent<Button>();
        _buttonSelect.onClick.AddListener(OnClicked);

        _bgImg = CommonUtils.Find<Image>(transform, "BG");
        _languageText = CommonUtils.Find<Text>(transform, "Text");

        _choseIcon = transform.Find("Icon").gameObject;
        _choseIcon.gameObject.SetActive(false);
    }

    public void Init(Action<string, UIPopupLanguageItem> cb, string locale)
    {
        _locale = locale;
        _clickCallback = cb;
        _languageText.text = LanguageModel.Instance.GetCfg()[locale].DisStr;
        SetSelect(_locale == LocalizationManager.Instance.GetCurrentLocale());
    }

    public void SetSelect(bool select)
    {
        _choseIcon.gameObject.SetActive(select);
    }

    private void OnClicked()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        _clickCallback(_locale, this);
    }
}