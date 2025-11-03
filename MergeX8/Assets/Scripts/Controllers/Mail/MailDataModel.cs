using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonPlus;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DragonU3DSDK;
using BiEvent = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using Game.Config;
using Gameplay;

namespace Game
{
    public enum MailType
    {
        System = 1,
        Friend = 2
    };

    public class SystemMail
    {
        public Mail Mail { get; set; }
        public int Order { get; set; }

        public bool Sort(SystemMail sMail)
        {
            return this.Order < sMail.Order;
        }
    }

    public class MailDataModel : Singleton<MailDataModel>
    {
        readonly string MAIL_READ_STATE = "MAIL_READ_STATE";
        private Dictionary<string, bool> m_MailStateDict;

        // 跟服务器同步邮件的CD时间,设定为60秒
        private const float REQUEST_MAIL_CD = 60.0f;

        Dictionary<string, SystemMail> m_MailDict = new Dictionary<string, SystemMail>();

        public Dictionary<string, SystemMail> Mails
        {
            get { return m_MailDict; }
        }

        #region 邮件状态本地存储、读取

        private void LoadLocalMailState()
        {
            if (m_MailStateDict == null)
            {
                m_MailStateDict = new Dictionary<string, bool>();
                if (PlayerPrefs.HasKey(MAIL_READ_STATE))
                {
                    var str = PlayerPrefs.GetString(MAIL_READ_STATE);
                    JsonConvert.PopulateObject(str, m_MailStateDict);
                }
            }
        }

        private void SaveLocalMailState()
        {
            if (m_MailStateDict != null)
            {
                var str2 = JsonConvert.SerializeObject(m_MailStateDict);
                PlayerPrefs.SetString(MAIL_READ_STATE, str2);
            }
        }

        public bool IsMailReaded(string id)
        {
            LoadLocalMailState();
            m_MailStateDict.TryGetValue(id, out bool value);
            return value;
        }

        public void ReadMail(string id, bool value = true)
        {
            LoadLocalMailState();
            if (m_MailStateDict.ContainsKey(id))
                m_MailStateDict[id] = value;
            else
                m_MailStateDict.Add(id, value);
            SaveLocalMailState();
            //EventManager.Instance.GlobalDispatcher.DispatchEvent(GlobalEvent.GLOBAL_MAIL_UPDATED);
        }

        #endregion

        public void RequestMailList()
        {
            DebugUtil.Log("Start RequestMailList ");
            APIManager.Instance.Send(new CListMail(), (IMessage obj) =>
                {
                    string popMailId = "";
                    Mails.Clear();
                    var sMail = obj as SListMail;
                    for (int i = 0; i < sMail.Mails.Count; ++i)
                    {
                        var mail = sMail.Mails[i];
                        var systemMail = new SystemMail
                        {
                            Mail = mail,
                            Order = i
                        };
                        Mails.Add(mail.MailId, systemMail);
                        if (mail.MailType == Mail.Types.MailType.Announcement ||
                            (mail.IsPop && !IsMailReaded(mail.MailId)))
                            popMailId = mail.MailId;
                    }

                    DebugUtil.Log("OnResponseMailList success , mails count : " + sMail.Mails.Count);

                    // 弹出系统公告
                    if (!string.IsNullOrEmpty(popMailId))
                    {
                        //EventDispatcher.Instance.DispatchEvent(new PopAnnouncementEvent(popMailId));
                    }

                    // 刷新邮件相关UI
                    EventDispatcher.Instance.DispatchEvent(EventEnum.GLOBAL_MAIL_UPDATED);
                },
                (arg1, arg2, arg3) => { DebugUtil.Log("CListMail Error " + arg1 + "  " + arg2 + "  " + arg3); });

            CommonUtils.DelayedCall(REQUEST_MAIL_CD, RequestMailList);
        }

        // 检查是否有未读的
        public bool HasNoReadMails()
        {
            bool bHave = false;
            foreach (string key in Mails.Keys)
            {
                var data = Mails[key];
                if (!IsMailReaded(data.Mail.MailId))
                {
                    bHave = true;
                    break;
                }
            }

            return bHave;
        }

        public SystemMail GetAnyNoReadMail()
        {
            foreach (string key in Mails.Keys)
            {
                var data = Mails[key];
                if (!IsMailReaded(data.Mail.MailId))
                {
                    return data;
                }
            }

            return null;
        }

        // 领取奖励或者标记邮件删除
        public void ClaimMail(string mailId, Action callback)
        {
            var cClaim = new CClaimMail()
            {
                MailId = mailId
            };
            CommonUtils.OpenWaitingWindow(2.0f, 30.0f);
            APIManager.Instance.Send(cClaim, (IMessage obj) =>
                {
                    var systemMailId = (obj as SClaimMail).MailId;
                    bool claimed = false;
                    foreach (var mail in Mails)
                    {
                        if (mail.Value.Mail.MailId == systemMailId)
                        {
                            if (mail.Value.Mail.Rewards.Count > 0)
                            {
                                var rewards = mail.Value.Mail.Rewards;
                                List<ResData> proceeds = new List<ResData>();
                                foreach (var reward in rewards)
                                {
                                    var proceed = new ResData((int) reward.RewardId, (int) reward.RewardValue);
                                    proceeds.Add(proceed);
                                    if (!UserData.Instance.IsResource((int) reward.RewardId))
                                    {
                                        TableMergeItem mergeItemConfig =
                                            GameConfigManager.Instance.GetItemConfig((int) reward.RewardId);
                                        if (mergeItemConfig != null)
                                        {
                                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                                            {
                                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType
                                                    .MergeChangeReasonEmailRead,
                                                itemAId = mergeItemConfig.id,
                                                ItemALevel = mergeItemConfig.level,
                                                isChange = true,
                                                extras = new Dictionary<string, string>
                                                {
                                                    {"count", reward.RewardValue.ToString()},
                                                }
                                            });
                                        }
                                    }
                                }

                                var BI = new GameBIManager.ItemChangeReasonArgs
                                    {reason = BiEvent.Types.ItemChangeReason.EmailRead, data1 = mail.Value.Mail.MailId};
                                CommonRewardManager.Instance.PopCommonReward(proceeds,
                                    CurrencyGroupManager.Instance.currencyController, true, BI);
                            }

                            claimed = true;
                            break;
                        }
                    }

                    if (claimed && Mails.ContainsKey(systemMailId))
                        Mails.Remove(systemMailId);

                    CommonUtils.CloseWaitingWindow();
                    callback?.Invoke();
                    // 刷新邮件相关UI
                    EventDispatcher.Instance.DispatchEvent(EventEnum.GLOBAL_MAIL_UPDATED);
                },
                (arg1, arg2, arg3) =>
                {
                    CommonUtils.CloseWaitingWindow();
                    callback?.Invoke();
                    DebugUtil.Log("OnClaimError Error " + arg1 + "  " + arg2 + "  " + arg3);
                });
            DebugUtil.Log("Claim mail Id : " + mailId);
        }

        public string GetSystemMailTitle(SystemMail data)
        {
            var locale = LocalizationManager.Instance.GetCurrentLocale();
            Mail.Types.MailInfo locale_content;
            if (data.Mail.LocaleMailInfos != null)
            {
                data.Mail.LocaleMailInfos.TryGetValue(locale, out locale_content);
                if (locale_content == null)
                    data.Mail.LocaleMailInfos.TryGetValue(Locale.ENGLISH, out locale_content);

                if (locale_content == null)
                    locale_content = data.Mail.MailInfo;
            }
            else
                locale_content = data.Mail.MailInfo;

            return locale_content.Title;
        }

        public string GetSystemMailMessage(SystemMail data)
        {
            var locale = LocalizationManager.Instance.GetCurrentLocale();
            Mail.Types.MailInfo locale_content;
            if (data.Mail.LocaleMailInfos != null)
            {
                data.Mail.LocaleMailInfos.TryGetValue(locale, out locale_content);
                if (locale_content == null)
                    data.Mail.LocaleMailInfos.TryGetValue(Locale.ENGLISH, out locale_content);

                if (locale_content == null)
                    locale_content = data.Mail.MailInfo;
            }
            else
                locale_content = data.Mail.MailInfo;

            return locale_content.Message;
        }
    }
}