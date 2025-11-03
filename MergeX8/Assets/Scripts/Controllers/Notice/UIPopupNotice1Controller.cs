
using System.Collections;
using DragonPlus;
using Mosframe;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupNotice1Controller : UIWindow
{
    public static bool IsLockSystemBack = false;

    private Button CloseButton { get; set; }

    LocalizeTextMeshProUGUI DescText { get; set; }
    LocalizeTextMeshProUGUI TitleText { get; set; }
    Button CancelButton { get; set; }
    LocalizeTextMeshProUGUI CancelButtonText { get; set; }
    Image CancelButtonImage { get; set; }
    Button OKButton { get; set; }
    LocalizeTextMeshProUGUI OKButtonText { get; set; }
    Image OKButtonImage { get; set; }

    NoticeUIData Data { get; set; }

    //private ContentSizeFitter sizefitter;
    // 唤醒界面时调用(创建的时候加载一次)
    public override void PrivateAwake()
    {
        CloseButton = transform.Find("Root/BgPopupBoand/ButtonClose").GetComponent<Button>();
        CloseButton.onClick.AddListener(OnClickCloseButton);

        CancelButton = transform.Find("Root/ButtonGroup/ButtonReturn").GetComponent<Button>();
        CancelButton.onClick.AddListener(OnClickCancelButton);
        CancelButtonText = transform.Find("Root/ButtonGroup/ButtonReturn/Text").GetComponent<LocalizeTextMeshProUGUI>();
        CancelButtonImage = transform.Find("Root/ButtonGroup/ButtonReturn/UIBg").GetComponent<Image>();

        OKButton = transform.Find("Root/ButtonGroup/ButtonNew").GetComponent<Button>();
        OKButton.onClick.AddListener(OnClickOKButton);
        OKButtonText = transform.Find("Root/ButtonGroup/ButtonNew/Text").GetComponent<LocalizeTextMeshProUGUI>();
        OKButtonImage = transform.Find("Root/ButtonGroup/ButtonNew/UIBg").GetComponent<Image>();

        DescText = transform.Find("Root/MiddleGroup/TextHint").GetComponent<LocalizeTextMeshProUGUI>();
        TitleText = transform.Find("Root/BgPopupBoand/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        //sizefitter = transform.Find("notice/Text").GetComponent<ContentSizeFitter>();
    }

    void OnClickCancelButton()
    {
        if (Data.CancelCallback != null)
            Data.CancelCallback();

        OnClickCloseButton();
    }

    void OnClickOKButton()
    {
        if (Data.NoTweenClose)
        {
            CloseWindowWithinUIMgr(true);
            
            if (Data.OKCallback != null)
                Data.OKCallback();
        }
        else
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, "disappear", null, ()=>
            {
                CloseWindowWithinUIMgr(true);
            
                if (Data.OKCallback != null)
                    Data.OKCallback();
            }));
        }
    }

    public void SetData(NoticeUIData data)
    {
        Data = data;
        if (Data.CullingMaskLayer != gameObject.layer)
        {
            Debug.LogError("切换Layer层级 "+Data.CullingMaskLayer+"=>"+gameObject.layer);
            gameObject.setLayer(Data.CullingMaskLayer,true);
        }
        ReloadUi();

        if (!Data.IsHideMainGroup)
            return;
    }

    void ReloadUi()
    {
        // if(Data.IsHighSortingOrder)
        // transform.GetComponent<Canvas>().sortingOrder = 9999;
        IsLockSystemBack = Data.IsLockSystemBack;

        DescText.SetText(Data.DescString);
        if (!string.IsNullOrEmpty(Data.TitleString))
            TitleText.SetText(Data.TitleString);
        CloseButton.gameObject.SetActive(Data.HasCloseButton);
        CancelButton.gameObject.SetActive(Data.HasCancelButton);

        if (Data.OKButtonText != null)
            OKButtonText.SetText(Data.OKButtonText);
        else
            OKButtonText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok"));
        if (Data.OkButtonSprite != null)
            OKButtonImage.sprite = Data.OkButtonSprite;

        if (Data.CancelButtonText != null)
            CancelButtonText.SetText(Data.CancelButtonText);
        else
            CancelButtonText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_button_no"));
        if (Data.CancelButtonSprite != null)
            CancelButtonImage.sprite = Data.CancelButtonSprite;

        if (Data.sortingOrder > 0)
            canvas.sortingOrder = Data.sortingOrder + UIManager.Instance.extraSiblingIndex;

        //StartCoroutine(RefreshText());
    }


    private IEnumerator RefreshText()
    {
        yield return new WaitForEndOfFrame();
        //sizefitter.enabled = false;
        yield return new WaitForEndOfFrame();
        //sizefitter.enabled = true;
        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    // 打开界面时调用(每次打开都调用)
    protected override void OnOpenWindow(params object[] objs)
    {
        //CommonUtils.TweenOpen(transform.Find("Root"));

        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NOTICE_POPUP);
    }


    private void OnClickCloseButton()
    {
        if (Data.NoTweenClose)
        {
            CloseWindowWithinUIMgr(true);
            
            if (Data.CancelCallback != null)
                Data.CancelCallback();
        }
        else
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, "disappear", null, ()=>
            {
                CloseWindowWithinUIMgr(true);
            
                if (Data.CancelCallback != null)
                    Data.CancelCallback();
            }));
        }


        if (!Data.IsHideMainGroup)
            return;
    }


    private void OnDestroy()
    {
        IsLockSystemBack = false;
    }
}