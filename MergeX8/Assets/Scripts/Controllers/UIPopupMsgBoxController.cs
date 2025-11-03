using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMsgBoxController : UIWindowController
{
    //common
    public static string COMMON_TITLE_MSG = "&key.UI_POPUP_MESSAGE_Title";
    public static string COMMON_TITLE_FAILED = "&key.UI_POPUP_FAILED_Title";
    public static string COMMON_BUTTON_CONTINUE = "&key.UI_button_continue";
    public static string COMMON_BUTTON_RETRY = "&key.UI_BUTTON_Retry";
    public static string COMMON_BUTTON_CANCEL = "&key.UI_button_cancel";

    public static string COMMON_BUTTON_OK = "&key.UI_button_ok";

    //game
    public static string Game_Name = "&key.Game_Name";

    //network
    public static string UI_NTWORKXNET_Title = "&key.UI_NTWORKXNET_Title";
    public static string UI_NTWORKXNET_Content = "&key.UI_NTWORKXNET_Content";

    public static string UI_NTWORKOVERTIME_Content = "&key.UI_NTWORKOVERTIME_Content";

    //rateus
    public static string UI_POPUP_RATEUS_Title = "&key.UI_POPUP_RATEUS_Title";
    public static string UI_POPUP_RATEUS_Content = "&key.UI_POPUP_RATEUS_Content";
    public static string UI_POPUP_RATEUS_BUTTON_Rating = "&key.UI_POPUP_RATEUS_BUTTON_Rating";
    public static string UI_POPUP_RATEUS_BUTTON_Later = "&key.UI_POPUP_RATEUS_BUTTON_Later";
    public static string UI_POPUP_RATEUS_BUTTON_NoRating = "&key.UI_POPUP_RATEUS_BUTTON_NoRating";

    //exit
    public static string UI_POPUP_EXIT_Title = "&key.UI_POPUP_EXIT_Title";
    public static string UI_POPUP_EXIT_Content = "&key.UI_POPUP_EXIT_Content";
    public static string UI_POPUP_EXIT_BUTTON_Exit = "&key.UI_POPUP_EXIT_BUTTON_Exit";

    // save image
    public static string UI_POPUP_SAVE_Content = "&key.UI_POPUP_SAVE_Content";

    // 缺金币
    public static string UI_HOME_LACKCOINS_Title = "&key.UI_HOME_LACKCOINS_Title";
    public static string UI_HOME_LACKCOINS_Content = "&key.UI_HOME_LACKCOINS_Content";
    public static string UI_HOME_LACKCOINS_BUTTON_StartColoring = "&key.UI_HOME_LACKCOINS_BUTTON_StartColoring";

    public static string UI_POPUP_FB_FAILED_Content = "&key.UI_POPUP_FB_FAILED_Content";
    public static string UI_POPUP_RESERROR_Content = "&key.UI_POPUP_RESERROR_Content";

    //老结构 需要保存的
    public static string UI_NEEDUPDATE_Msg = "&key.UI_NEEDUPDATE_Msg"; //
    public static string UI_MULTIDEVICE_LOGIN_Msg = "&key.UI_MULTIDEVICE_LOGIN_Msg"; //
    public static string Noti_Text_ = "&key.Noti_Text_"; //
    public static string UI_POPUP_SERVERERROR_ = "&key.UI_POPUP_SERVERERROR_"; //
    public static string UI_POPUP_BUY_Fail_Content = "&key.UI_POPUP_BUY_Fail_Content";

    public static string UI_NODE_LOCKED_TITLE = "&key.UI_NODE_LOCKED_TITLE";
    public static string UI_NODE_LOCKED_CONTENT = "&key.UI_node_locked_content";
    public static string UI_ITEM_PENDING_TIP = "&key.UI_home_pending_tips";

    //老结构 暂时用不到的功能
    public static string UI_LOGOUT_POPUP_FAILED_Content = "&key.UI_LOGOUT_POPUP_FAILED_Content";

    //setting user id
    public static readonly string UI_SETTING_USER_ID = "&key.UI_SETTING_LABEL_Pid";


    public static readonly string UI_POPUP_NOADS_Title = "&key.UI_POPUP_NOADS_Title";
    public static readonly string UI_POPUP_NOADS_Content = "&key.UI_POPUP_NOADS_Content";

    public static readonly string UI_POPUP_ALL_LEVELS_COMPLETED_Title = "&key.UI_POPUP_ALLLEVELSCOMPLETED_Title";
    public static readonly string UI_POPUP_ALLLEVELSCOMPLETED_Content = "&key.UI_POPUP_ALLLEVELSCOMPLETED_Content";

    public static bool IsLockSystemBack = false; // 是否屏蔽安卓返回键
    protected GameObject _goClose;
    protected LocalizeTextMeshProUGUI _txtContent;
    protected LocalizeTextMeshProUGUI _txtOk;
    protected LocalizeTextMeshProUGUI _txtCancel;

    protected GameObject _goCancel;
    protected bool _canBackClose = true;

    protected Action _onButtonOk;
    protected Action _onButtonCancel;

    public class PopupInfo
    {
        public string title;
        public string content;
        public string txtOk;
        public string txtCancel;
        public bool showCancel = false;
        public bool showClose = true;

        public bool canBackClose = true;
        public Action onButtonOk;
        public Action onButtonCancel;

        ///
        public string atlasName;

        public string imgName;
        public string imgPath;
        public string txtCount;
    }

    public static void Popup(PopupInfo info)
    {
        UIPopupMsgBoxController uiMsgBoxController =
            UIManager.Instance.OpenUI(UINameConst.UIMsgBox) as UIPopupMsgBoxController;
        if (uiMsgBoxController != null)
        {
            uiMsgBoxController._txtContent.SetText(info.content);
            uiMsgBoxController._txtOk.SetText(info.txtOk);
            uiMsgBoxController._onButtonOk = info.onButtonOk;

            uiMsgBoxController._txtCancel.SetText(info.txtCancel);
            uiMsgBoxController._goCancel.SetActive(info.showCancel);
            uiMsgBoxController._onButtonCancel = info.onButtonCancel;

            uiMsgBoxController._goClose.SetActive(info.showClose);
            uiMsgBoxController._canBackClose = info.canBackClose;
            IsLockSystemBack = !info.canBackClose;
        }
    }

    public override void PrivateAwake()
    {
        _txtContent = GetItem<LocalizeTextMeshProUGUI>("GeneralPopupContent/Text");
        _txtOk = GetItem<LocalizeTextMeshProUGUI>("GeneralPopupContent/ButtonGroup/ButtonOK/Text");
        BindEvent("GeneralPopupContent/ButtonGroup/ButtonOK", this.gameObject, BtnSure_Click);

        _txtCancel = GetItem<LocalizeTextMeshProUGUI>("GeneralPopupContent/ButtonGroup/ButtonCancel/Text");
        _goCancel = BindEvent("GeneralPopupContent/ButtonGroup/ButtonCancel", this.gameObject, BtnClose_Click);

        _goClose = BindEvent("GeneralPopupContent/BgPopupBoand/ButtonClose", this.gameObject, BtnClose_Click);
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        if (destroy)
        {
            IsLockSystemBack = false;
        }
    }

    protected void BtnClose_Click(GameObject btnObj)
    {
        CloseWindowWithinUIMgr(true);
        _onButtonCancel?.Invoke();
    }

    protected void BtnSure_Click(GameObject btnObj)
    {
        CloseWindowWithinUIMgr(true);
        _onButtonOk?.Invoke();
    }

    public override bool OnBack()
    {
        if (_canBackClose)
        {
            return base.OnBack();
        }
        else
        {
            return true;
        }
    }
}