/*
 * @file ContactUsController
 * 联系我们
 * @author lu
 */

using System;
using System.Globalization;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework.Motion;
using Google.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupContactUsController : UIWindowController
{
    Button CloseButton { get; set; }
    RepeatedField<UserComplainMessage> MessageList { get; set; }
    GameObject MyCellUi { get; set; }
    GameObject OtherCellUi { get; set; }
    Transform Content { get; set; }

    Button SendButton { get; set; }
    InputField InputField { get; set; }
    Text TipsText { get; set; }
    InputField EmailInputField { get; set; }
    Transform TipsRoot { get; set; }
    Text InputTextLengthText { get; set; }
    ScrollRect ScrollView { get; set; }

    //Text EmailPlaceholder { get; set; }
    //Text MessagePlaceholder { get; set; }

    // 需要等待执行的计数器
    int UpdateWaitAction { get; set; }

    // 输入最长文本
    readonly int MaxInputLength = 2000;

    private bool isFirstSend;
    private Animator _animator;

    private int sendMessageNumber = 1;

    private bool isCloseWithOpenSet = true;
    // 唤醒界面时调用(创建的时候加载一次)
    public override void PrivateAwake()
    {
        CloseButton = transform.Find("Root/Title/ButtonClose").GetComponent<Button>();
        CloseButton.onClick.AddListener(OnClickCloseButton);

        ScrollView = transform.Find("Root/MidImage/CoinsEnough/Scroll View").GetComponent<ScrollRect>();
        Content = transform.Find("Root/MidImage/CoinsEnough/Scroll View/Viewport/Content");

        SendButton = transform.Find("Root/Button/UpgradeButton").GetComponent<Button>();
        SendButton.onClick.AddListener(OnClickSendButton);

        InputField = transform.Find("Root/Button/InputField").GetComponent<InputField>();
        InputField.onValueChanged.AddListener((param) => { OnChangeInputField(); });

        InputField.onValidateInput += delegate(string input, int charIndex, char addedChar)
        {
            return MyValidate(addedChar);
        };

        EmailInputField = transform.Find("Root/GameObject/InputField").GetComponent<InputField>();
        TipsRoot = transform.Find("Root/Button/InputField/Input");
        TipsText = transform.Find("Root/Button/InputField/Input/Image/Text").GetComponent<Text>();
        //InputTextLengthText = transform.Find("Root/Button/InputField/Text").GetComponent<Text>();

        InputField.characterLimit = MaxInputLength;
        //InputTextLengthText.text = LocalizationManager.Instance.GetLocalizedStringWithFormats("UI_progress", 0, MaxInputLength.ToString());
        TipsRoot.gameObject.SetActive(false);
        _animator = transform.GetComponent<Animator>();
        //EmailPlaceholder = transform.Find("ContactUs/GameObject/InputField/Placeholder").GetComponent<Text>();
        //EmailPlaceholder.text = LocalizationManager.Instance.GetLocalizedString(EmailPlaceholder.text);
        //MessagePlaceholder = transform.Find("ContactUs/Button/InputField/Placeholder").GetComponent<Text>();
        //MessagePlaceholder.text = LocalizationManager.Instance.GetLocalizedString(MessagePlaceholder.text);

        sendMessageNumber = 1;
        EventDispatcher.Instance.AddEventListener(EventEnum.FAQ_SELECT_QUESTION, ChangeSelectQuestion);
        EventDispatcher.Instance.AddEventListener(EventEnum.FAQ_QUESTION_SERVER_BACK, ServerBackQuestion);
    }

    private char MyValidate(char charToValidate)
    {
        // Emoji表情
        if (char.GetUnicodeCategory(charToValidate) == UnicodeCategory.Surrogate)
        {
            return '\0';
        }

        return charToValidate;
    }

    void OnChangeInputField()
    {
        TipsText.text = InputField.text;
        //TipsRoot.gameObject.SetActive(!string.IsNullOrEmpty(TipsText.text));

        //var textLength = TipsText.text.Length.ToString();
        //InputTextLengthText.text = LocalizationManager.Instance.GetLocalizedStringWithFormats("UI_progress", textLength , MaxInputLength);
    }

    // 打开界面时调用(每次打开都调用)
    protected override void OnOpenWindow(params object[] objs)
    {
        //GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventContactUs, LevelGroupSystem.Instance.GetCurLevelCountIdx().ToString());
        if (objs != null && objs.Length > 0)
            isCloseWithOpenSet =(bool) objs[0];
        ReloadUi();
    }

    void ReloadUi()
    {
        WaitingManager.Instance.OpenWindow(10);
        ContactUsModel.Instance.GetMessageList(OnGetMessageListResult, OnGetMessageListError);

        var email = ContactUsModel.Instance.GetSubscribeEmailAddress();
        if (!string.IsNullOrEmpty(email))
            EmailInputField.text = email;
    }

    void OnGetMessageListResult()
    {
        UpdateWaitAction = 2;
        var MessagesList = ContactUsModel.Instance.MessageList.Messages;


        for (int index = 0; index < MessagesList.Count; index++)
        {
            UserComplainMessage itemData = MessagesList[index];

            AddCell(itemData);
        }

        //RedPointCenter.Instance.Set("menu.set.contact_us", 0);

        WaitingManager.Instance.CloseWindow();
    }

    void AddCell(UserComplainMessage itemData)
    {
        if (itemData.MessageType == UserComplainMessage.Types.MessageType.Complain)
        {
            // 自己的话

            if (MyCellUi == null)
            {
                MyCellUi = ResourcesManager.Instance.LoadResource<GameObject>(PathManager.CookingPrefabPrefix +
                                                                              "Home/ContactUsItem2");
            }

            GameObject ItemPrfIns = Instantiate(MyCellUi, null, false);
            var ItemController = ItemPrfIns.AddComponent<ContactUsMyCellController>();
            ItemController.SetData(this, itemData);
            CommonUtils.AddChild(Content, ItemPrfIns.transform);

            CheckIOSQuestions();
        }
        else if (itemData.MessageType == UserComplainMessage.Types.MessageType.Reply)
        {
            // 别人的话

            if (OtherCellUi == null)
            {
                OtherCellUi =
                    ResourcesManager.Instance.LoadResource<GameObject>(PathManager.CookingPrefabPrefix +
                                                                       "Home/ContactUsItem");
            }

            GameObject ItemPrfIns = Instantiate(OtherCellUi, null, false);
            var ItemController = ItemPrfIns.AddComponent<ContactUsOtherCellController>();
            ItemController.SetData(this, itemData);
            CommonUtils.AddChild(Content, ItemPrfIns.transform);
        }
    }

    void OnGetMessageListError()
    {
        WaitingManager.Instance.CloseWindow();
    }

    public void GotoScrollviewEnd()
    {
        ScrollView.verticalNormalizedPosition = 0f;
    }

    void OnClickSendButton()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        if (string.IsNullOrEmpty(InputField.text))
        {
            DebugUtil.Log("消息不能为空");

            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_contactus_panel_input_popup_text")
            });

            return;
        }

        if (!string.IsNullOrEmpty(EmailInputField.text) && !CommonUtils.IsEmail(EmailInputField.text))
        {
            DebugUtil.Log("错误的邮箱");

            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_contactus_panel_email_err_text")
            });

            return;
        }

        var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        if (string.IsNullOrEmpty(storageCommon.Email) && !string.IsNullOrEmpty(EmailInputField.text))
        {
            storageCommon.Email = EmailInputField.text;
        }

        WaitingManager.Instance.OpenWindow();

        string email = string.IsNullOrEmpty(EmailInputField.text) ? "" : EmailInputField.text;

        ContactUsModel.Instance.SenMyMessage(email, InputField.text, OnSendMessageResult, OnSendMessageError);
    }

    void OnSendMessageResult()
    {
        WaitingManager.Instance.CloseWindow();

        if (this == null)
            return;

        InputField.text = "";
        OnChangeInputField();
        UpdateWaitAction = 2;

        if (sendMessageNumber == 1)
            isFirstSend = true;

        sendMessageNumber++;

        AddCell(ContactUsModel.Instance.TempSendMessage);
    }

    void OnSendMessageError()
    {
        WaitingManager.Instance.CloseWindow();

        if (this == null)
            return;

        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
        {
            DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_contactus_panel_send_err_text")
        });
    }

    // 重新加载界面时调用
    protected override void OnReloadWindow()
    {
    }

    // 关闭界面时调用(onDestroy)
    protected override void OnCloseWindow(bool destroy = false)
    {
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FAQ_SELECT_QUESTION, ChangeSelectQuestion);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FAQ_QUESTION_SERVER_BACK, ServerBackQuestion);
    }

    void OnClickCloseButton()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        
        CloseButton.interactable = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () =>
            {
                CloseWindowWithinUIMgr(true);
                if(isCloseWithOpenSet)
                    UIManager.Instance.OpenUI(UINameConst.UIPopupSet1, false);
            }));
    }

    private void Update()
    {
        UpdateWaitAction--;
        if (UpdateWaitAction == 0)
        {
            GotoScrollviewEnd();
        }
    }

    void CheckIOSQuestions()
    {
        if (isFirstSend)
        {
            isFirstSend = false;
            CreateOtherMessage(0);
        }
    }

    void CreateOtherMessage(int id)
    {
        UserComplainMessage message = new UserComplainMessage();
        message.Message = ContactUsConfigManager.Instance.GetAnswer(id);
        message.CreatedAt = (ulong) Utils.TotalMilliseconds();

        if (OtherCellUi == null)
        {
            OtherCellUi =
                ResourcesManager.Instance.LoadResource<GameObject>(PathManager.CookingPrefabPrefix +
                                                                   "Home/ContactUsItem");
        }

        GameObject ItemPrfIns = Instantiate(OtherCellUi, null, false);
        var ItemController = ItemPrfIns.AddComponent<ContactUsOtherCellController>();
        ItemController.SetData(this, message);
        ItemController.AddIOSQuestions(id);

        CommonUtils.AddChild(Content, ItemPrfIns.transform);
    }

    public void ChangeSelectQuestion(BaseEvent baseEvent)
    {
        int id = (int) baseEvent.datas[0];

        CreateMyMessage(id);
        CreateOtherMessage(id);

        UpdateWaitAction = 2;
    }

    public void ServerBackQuestion(BaseEvent baseEvent)
    {
        int id = (int) baseEvent.datas[0];
        CreateOtherMessage(id);
    }

    void CreateMyMessage(int id)
    {
        UserComplainMessage message = new UserComplainMessage();
        message.Message = ContactUsConfigManager.Instance.GetQuestion(id);
        message.CreatedAt = (ulong) Utils.TotalMilliseconds();

        if (MyCellUi == null)
        {
            MyCellUi = ResourcesManager.Instance.LoadResource<GameObject>(PathManager.CookingPrefabPrefix +
                                                                          "Home/ContactUsItem2");
        }

        GameObject ItemPrfIns = Instantiate(MyCellUi, null, false);
        ContactUsMyCellController ItemController = ItemPrfIns.AddComponent<ContactUsMyCellController>();
        ItemController.SetData(this, message);
        CommonUtils.AddChild(Content, ItemPrfIns.transform);
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;
        
        OnClickCloseButton();
    }
}