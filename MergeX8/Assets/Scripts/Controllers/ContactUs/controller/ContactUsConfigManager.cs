using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DragonPlus.Config
{
    public class ContactUsConfigManager
    {
        private static ContactUsConfigManager instance = null;
        private static readonly object syslock = new object();

        public static ContactUsConfigManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new ContactUsConfigManager();
                        }
                    }
                }

                return instance;
            }
        }

        private ContactUsConfigManager()
        {
        }

        public List<ContactUsFaqConfig> FaqList;
        public Dictionary<int, ContactUsFaqConfig> FaqDic;

        public List<ContactUsI18nConfig> I18nList;
        public Dictionary<string, ContactUsI18nConfig> I18nDic;

        public void InitConfigs()
        {
            TextAsset ta = ResourcesManager.Instance.LoadResource<TextAsset>(PathManager.Configs + "/Faq/faq");
            string jsStr = ta.text;

            if (String.IsNullOrEmpty(jsStr))
            {
                DebugUtil.LogError("Load contactus config error!");
                return;
            }

            InitConfigForString(jsStr);

            GetServerConfig();
        }

        public void InitConfigForString(string jsStr)
        {
            Hashtable table = JsonConvert.DeserializeObject<Hashtable>(jsStr);

            object tempObj = table["faq"];
            String tempObjStr = JsonConvert.SerializeObject(tempObj);
            FaqList = JsonConvert.DeserializeObject<List<ContactUsFaqConfig>>(tempObjStr);
            FaqDic = new Dictionary<int, ContactUsFaqConfig>();

            for (int i = 0; i < FaqList.Count; i++)
            {
                ContactUsFaqConfig config = FaqList[i];

                FaqDic.Add(config.Id, config);
            }

            tempObj = table["i18n"];
            tempObjStr = JsonConvert.SerializeObject(tempObj);
            I18nList = JsonConvert.DeserializeObject<List<ContactUsI18nConfig>>(tempObjStr);
            I18nDic = new Dictionary<string, ContactUsI18nConfig>();

            for (int i = 0; i < I18nList.Count; i++)
            {
                ContactUsI18nConfig config = I18nList[i];
                config.InitI18n();

                I18nDic.Add(config.Key, config);
            }
        }

        public void LoadRemoteConfig(string jsStr)
        {
            if (String.IsNullOrEmpty(jsStr))
            {
                DebugUtil.LogError("Load remote contactus config error!");
                return;
            }

            InitConfigForString(jsStr);
        }

        // public List<ContactUsFaqConfig> GetQuestionsForTag(int tag)
        // {
        //     List<ContactUsFaqConfig> questions = new List<ContactUsFaqConfig>();
        //     for (int i = 0; i < FaqList.Count; i++)
        //     {
        //         ContactUsFaqConfig config = FaqList[i];
        //         if (config.DefaultQuestion == tag)
        //         {
        //             questions.Add(config);
        //         }
        //     }
        //     return questions;
        // }

        public void GetServerConfig()
        {
            CGetConfig cGetConfig = new CGetConfig
            {
                Route = "config_faq",
            };

            APIManager.Instance.Send(cGetConfig, (SGetConfig sGetConfig) =>
            {
                if (string.IsNullOrEmpty(sGetConfig.Config.Json))
                {
                    DebugUtil.LogWarning("config_faq 服务器配置为空！");
                    return;
                }

                try
                {
                    JObject obj = JObject.Parse(sGetConfig.Config.Json);
                    LoadRemoteConfig(sGetConfig.Config.Json);
                }
                catch (Exception e)
                {
                }
            }, (errno, errmsg, resp) => { });
        }


        public string GetAnswer(int id)
        {
            ContactUsFaqConfig config;
            if (FaqDic.TryGetValue(id, out config))
            {
                ContactUsI18nConfig i18nConfig;
                if (I18nDic.TryGetValue(config.Answer, out i18nConfig))
                {
                    string locale = LocalizationManager.Instance.GetCurrentLocale();
                    return i18nConfig.I18nDic[locale];
                }
            }

            return "";
        }

        public string GetQuestion(int id)
        {
            ContactUsFaqConfig config;
            if (FaqDic.TryGetValue(id, out config))
            {
                ContactUsI18nConfig i18nConfig;
                if (I18nDic.TryGetValue(config.Question, out i18nConfig))
                {
                    string locale = LocalizationManager.Instance.GetCurrentLocale();
                    return i18nConfig.I18nDic[locale];
                }
            }

            return "";
        }

        public List<ContactUsFaqConfig> GetQuestions(int id)
        {
            List<ContactUsFaqConfig> questions = new List<ContactUsFaqConfig>();
            ContactUsFaqConfig config;
            if (FaqDic.TryGetValue(id, out config))
            {
#if UNITY_ANDROID
                if (config.AndroidQuestionList != null)
                {
                    for (int i = 0; i < config.AndroidQuestionList.Count; i++)
                    {
                        int key = config.AndroidQuestionList[i];
                        ContactUsFaqConfig qc = FaqDic[key];
                        questions.Add(qc);
                    }
                }
#elif UNITY_IOS
                if (config.IosQuestionList != null)
                {
                    for (int i = 0; i < config.IosQuestionList.Count; i++)
                    {
                        int key = config.IosQuestionList[i];
                        ContactUsFaqConfig qc = FaqDic[key];
                        questions.Add(qc);
                    }
                }
#endif
            }

            return questions;
        }
    }
}