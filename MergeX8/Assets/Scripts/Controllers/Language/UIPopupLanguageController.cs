using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLanguageController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonOK;

    private Transform _contextTrans;
    private RectTransform _rebuildTrans;
    private GameObject _itemTemplate;

    private List<UIPopupLanguageItem> _items = new List<UIPopupLanguageItem>();

    private string _curentLocale;
    private UIPopupLanguageItem _curentSelect;

    private Animator _animator;
    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        
        _buttonClose = GetItem<Button>("Root/UI/Middle/ButtonClose");
        _buttonClose?.onClick.AddListener(OnCloseClick);

        _buttonClose = GetItem<Button>("Root/UI/Middle/OKButton");
        _buttonClose?.onClick.AddListener(OnOKClick);

        _rebuildTrans = GetItem<RectTransform>("Root/UI/Middle");
        _contextTrans = GetItem<Transform>("Root/UI/Middle/Scroll View/Viewport/Content");
        _itemTemplate = GetItem("Root/UI/Middle/Scroll View/Viewport/Content/Button");
        _itemTemplate?.SetActive(false);
    }

    private IEnumerator Start()
    {
        RefreshLanguageList();
        //yield return new WaitForSeconds(0.03f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rebuildTrans);
        yield return null;
    }

    private void RefreshLanguageList()
    {
        _items.ForEach(item => { Destroy(item.gameObject); });
        _items.Clear();

        foreach (var s in LocalizationManager.Instance.supportedLocale)
        {
            GameObject languageItem = Instantiate(_itemTemplate, _contextTrans);
            languageItem.SetActive(true);
            UIPopupLanguageItem item = languageItem.AddComponent<UIPopupLanguageItem>();
            item.Init(OnChooseLocale, s);
            _items.Add(item);
            if (s == LocalizationManager.Instance.GetCurrentLocale())
            {
                _curentLocale = s;
                _curentSelect = item;
            }
        }

        _items.ForEach(item => { });
    }

    private void OnCloseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            UIManager.Instance.OpenUI(UINameConst.UIPopupSet2, false);
        }));
    }

    private void OnOKClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            if (_curentLocale == LocalizationManager.Instance.GetCurrentLocale())
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupSet2, false);
                return;
            }

            LocalizationManager.Instance.SetCurrentLocale(_curentLocale);
            SceneFsm.mInstance.BackToLogin();
        }));
    }

    private void OnChooseLocale(string l, UIPopupLanguageItem itemSelect)
    {
        if (l == _curentLocale)
            return;

        _curentSelect.SetSelect(false);

        _curentLocale = l;
        _curentSelect = itemSelect;
        itemSelect.SetSelect(true);
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;
        
        OnCloseClick();
    }
    
}