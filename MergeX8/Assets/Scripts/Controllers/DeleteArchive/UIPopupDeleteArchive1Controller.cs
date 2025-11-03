
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupDeleteArchive1Controller : UIWindowController
{
    private Button _buttonClose;
    private Button _cancelBtn;
    
    private Button _confirmButton;
    private GameObject _redBg;
    private GameObject _GaryBg;
    private LocalizeTextMeshProUGUI _redText;
    private LocalizeTextMeshProUGUI _garyText;
    private int CanClickTime=10;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnClose);
        _cancelBtn = GetItem<Button>("Root/CancellationButton");
        _cancelBtn.onClick.AddListener(OnClose);
        _confirmButton = GetItem<Button>("Root/ConfirmButton");
        _confirmButton.onClick.AddListener(OnConfirm);
        _redBg = GetItem("Root/ConfirmButton/RedBG");
        _GaryBg = GetItem("Root/ConfirmButton/GrayBG");
        _redText = GetItem<LocalizeTextMeshProUGUI>("Root/ConfirmButton/RedText");
        _garyText = GetItem<LocalizeTextMeshProUGUI>("Root/ConfirmButton/GrayText");
        InvokeRepeating("Repeat",1,1);
    }



    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Repeat();
    }
    public void Repeat()
    {
        CanClickTime--;
        if (CanClickTime > 0)
        {
            _redBg.SetActive(false);
            _GaryBg.SetActive(true);
            _garyText.gameObject.SetActive(true);
            _redText.gameObject.SetActive(false);
            _confirmButton.interactable = false;
            _garyText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_button_yes")+"("+CanClickTime+"s)");

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
        AnimCloseWindow(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupSet2);
        });
    }
    private void OnConfirm()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        AnimCloseWindow(()=>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupDeleteArchive2);
        });
    }
   
}