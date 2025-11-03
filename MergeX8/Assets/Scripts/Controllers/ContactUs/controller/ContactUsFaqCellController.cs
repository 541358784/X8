using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class ContactUsFaqCellController : MonoBehaviour
{
    private Text Text { get; set; }

    private ContactUsFaqConfig config;
    private string localeKey;

    void Awake()
    {
        Text = transform.Find("Text").GetComponent<Text>();
        this.GetComponent<Button>().onClick.AddListener(OnClickQuestion);

        localeKey = LocalizationManager.Instance.GetCurrentLocale();
    }

    public void InitData(ContactUsFaqConfig data)
    {
        config = data;

        ContactUsI18nConfig i18nConfig;
        if (ContactUsConfigManager.Instance.I18nDic.TryGetValue(config.Question, out i18nConfig))
        {
            string strValue = i18nConfig.I18nDic[localeKey];
            Text.text = strValue;
        }
    }

    public void OnClickQuestion()
    {
        EventDispatcher.Instance.DispatchEvent(new BaseEvent(EventEnum.FAQ_SELECT_QUESTION, config.Id));
        CAutoSendUserComplainMessage cMessage = new CAutoSendUserComplainMessage
        {
            Id = (uint) config.Id,
            Locale = localeKey
        };

        APIManager.Instance.Send(cMessage, (SAutoSendUserComplainMessage sGetConfig) =>
            {
                uint succId = sGetConfig.Id;
                ContactUsFaqConfig succConfig = ContactUsConfigManager.Instance.FaqDic[(int) succId];
                if (succConfig.Successor != 0)
                {
                    EventDispatcher.Instance.DispatchEvent(new BaseEvent(EventEnum.FAQ_QUESTION_SERVER_BACK,
                        succConfig.Successor));
                }
            },
            (errno, errmsg, resp) =>
            {
                DebugUtil.LogError("CAutoSendUserComplainMessage erro code = {0} message = {1}", errno, errmsg);
            });
    }
}