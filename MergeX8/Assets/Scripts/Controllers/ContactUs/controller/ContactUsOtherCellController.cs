/*
 * @file ContactUsOtherCellController
 * 联系我们 - 别人说的话
 * @author lu
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DragonPlus.Config;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class ContactUsOtherCellController : MonoBehaviour
{
    UIPopupContactUsController Parent { get; set; }
    UserComplainMessage ItemData { get; set; }
    LinkImageText MessageText { get; set; }
    Text TimeText { get; set; }
    private RectTransform rectTransform;

    private Transform faqNode;

    // Use this for initialization
    void Start()
    {
        MessageText = transform.Find("Image/Image/Text").GetComponent<LinkImageText>();
        TimeText = transform.Find("Image/Image/Time").GetComponent<Text>();

        faqNode = transform.Find("Image");

        EventDispatcher.Instance.AddEventListener(EventEnum.FAQ_SELECT_QUESTION, ChangeSelectQuestion);

        rectTransform = transform as RectTransform;

        ReloadFromData();
    }

    public void SetData(UIPopupContactUsController parent, UserComplainMessage itemData)
    {
        Parent = parent;
        ItemData = itemData;
    }

    public void ReloadFromData()
    {
        string Message = ItemData.Message;

        Regex re = new Regex(@"(?<url>http(s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)");
        MatchCollection mc = re.Matches(Message);
        foreach (Match m in mc)
        {
            Message = Message.Replace(m.Result("${url}"), string.Format("<a href={0}>{0}</a>", m.Result("${url}")));
        }

        MessageText.text = Message;
        TimeText.text = DragonU3DSDK.Utils.GetTimeStampDateString(ItemData.CreatedAt);
        UpdateMessageView();
        Parent.GotoScrollviewEnd();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FAQ_SELECT_QUESTION, ChangeSelectQuestion);
    }

    public void AddIOSQuestions(int id)
    {
        faqNode = transform.Find("Image");

        List<ContactUsFaqConfig> qsList = ContactUsConfigManager.Instance.GetQuestions(id);
        for (int i = 0; i < qsList.Count; i++)
        {
            GameObject pre =
                ResourcesManager.Instance.LoadResource<GameObject>(PathManager.CookingPrefabPrefix +
                                                                   "Home/ContactUsCell");
            ContactUsFaqCellController faqItem = Instantiate(pre).AddComponent<ContactUsFaqCellController>();

            ContactUsFaqConfig config = qsList[i];
            faqItem.InitData(config);

            CommonUtils.AddChild(faqNode, faqItem.transform);
        }
    }

    private void UpdateMessageView()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(MessageText.transform as RectTransform);

        LayoutRebuilder.ForceRebuildLayoutImmediate(faqNode as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void ChangeSelectQuestion(BaseEvent baseEvent)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(faqNode as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        //Parent.GotoScrollviewEnd();
    }
}