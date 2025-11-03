using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using System.Collections.Generic;
using DragonPlus.Config;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Game;
using Gameplay;

public class UIPopupMailListController : UIWindowController
{
    private GameObject content;
    private GameObject bottombBG;
    private GameObject bg;
    private GameObject m_MailistCell;
    private List<UIPopupMailListCell> _cells;
    private Button closeButton;
    private LocalizeTextMeshProUGUI _nullText;
    public override void PrivateAwake()
    {
        content = transform.Find("Root/MiddleGroup/Viewport/Content").gameObject;
        m_MailistCell = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Home/UIPopupMailListCell");
        _cells = new List<UIPopupMailListCell>();
        EventDispatcher.Instance.AddEventListener(EventEnum.GLOBAL_MAIL_UPDATED, OnMailUpdate);
        closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() => { AnimCloseWindow(); });
        _nullText = transform.Find("Root/NullText").GetComponent<LocalizeTextMeshProUGUI>();
        bottombBG = transform.Find("Root/BottomBG").gameObject;
        bg = transform.Find("Root/BGGroup/BG").gameObject;
    }

    private void OnMailUpdate(BaseEvent obj)
    {
        if (MailDataModel.Instance.HasNoReadMails())
        {
            RefreshUI();
        }
        else
        {
            CloseWindowWithinUIMgr(true);
        }
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        CommonUtils.DestroyAllChildren(content.transform);
        _nullText.gameObject.SetActive(MailDataModel.Instance.Mails.Count<=0);
        bg.gameObject.SetActive(MailDataModel.Instance.Mails.Count<=0);
        bottombBG.gameObject.SetActive(MailDataModel.Instance.Mails.Count<=0);
        foreach (var mail in MailDataModel.Instance.Mails)
        {
            var view = Instantiate(m_MailistCell, content.transform, false);
            var cell = view.gameObject.AddComponent<UIPopupMailListCell>();
            cell.RefreshUI(mail.Value);
            _cells.Add(cell);
        }
    }

    public static void Open()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupMailList);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GLOBAL_MAIL_UPDATED, OnMailUpdate);
    }
}