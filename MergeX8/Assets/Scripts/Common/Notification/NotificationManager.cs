using System;
using System.Collections.Generic;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using Facebook.Unity;
using UnityEngine;

namespace DragonPlus
{
    public class NotificationManager
    {
        private const string APP_NAME_KEY = "&key.NAME";
        private static NotificationManager _instance = null;
        private static readonly object _syslock = new object();
        public bool UseDebug = false;
        public static NotificationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syslock)
                    {
                        if (_instance == null)
                        {
                            _instance = new NotificationManager();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 
        /// Pushs the local notifications.
        /// 推送前会清除之前设置的本地推送
        /// 
        /// </summary>
        public void RegistLocalNotifications()
        {
            ClearNotifications();

            if(GlobalConfigManager.Instance == null)
                return;
            
            var configs = GlobalConfigManager.Instance.GetNotificationConfigs();
            if (configs != null && configs.Count >= 4)
            {
                DebugUtil.Log("Sending notifications");
                
                // 获取当前日期
                DateTime currentDate = DateTime.UtcNow;
                // 设置时间为中午12点
                DateTime noonUtcTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 0, 0);
                for (int i = 0; i < 30; i++)
                {
                   
                    var delayMs = (noonUtcTime.TotalSeconds() - currentDate.TotalSeconds() + i * 24 * 60 * 60)*1000;
                    if (delayMs > 0)
                    {
                        MobileNotificationManager.SendNotification(10+i, delayMs,
                            LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                            LocalizationManager.Instance.GetLocalizedString("ui_new_notification2"),
                            new Color32(0xff, 0x44, 0x44, 255));
                    }
                }
                
                MobileNotificationManager.SendNotification(1, configs[0].timeInSecond * 1000,
                    LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                    LocalizationManager.Instance.GetLocalizedString(configs[0].text),
                    new Color32(0xff, 0x44, 0x44, 255));
                MobileNotificationManager.SendNotification(2, configs[1].timeInSecond * 1000,
                    LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                    LocalizationManager.Instance.GetLocalizedString(configs[1].text),
                    new Color32(0xff, 0x44, 0x44, 255));
                MobileNotificationManager.SendNotification(3, configs[2].timeInSecond * 1000,
                    LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                    LocalizationManager.Instance.GetLocalizedString(configs[2].text),
                    new Color32(0xff, 0x44, 0x44, 255));
                MobileNotificationManager.SendNotification(4, configs[3].timeInSecond * 1000,
                    LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                    LocalizationManager.Instance.GetLocalizedString(configs[3].text),
                    new Color32(0xff, 0x44, 0x44, 255));
                if (EnergyModel.Instance.EnergyNumber() < EnergyModel.Instance.MaxEnergyNum)
                {
                    MobileNotificationManager.SendNotification(5, EnergyModel.Instance.MilisecondsBeforeEnergyFull(),
                        LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                        LocalizationManager.Instance.GetLocalizedString("UI_push_hp_max"),
                        new Color32(0xff, 0x44, 0x44, 255));
                }

                if (GameConfigManager.Instance.InitFlag)
                {
                    var sealCd = MergeManager.Instance.GetProductCdByLine(21101,MergeBoardEnum.Main);
                    if (sealCd>0)
                    {
                        MobileNotificationManager.SendNotification(6, sealCd*1000,
                            LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                            LocalizationManager.Instance.GetLocalizedString("Notification_text_5"),
                            new Color32(0xff, 0x44, 0x44, 255));
                    }
                    var cd2 = MergeManager.Instance.GetProductCdByLine(23101,MergeBoardEnum.Main);
                    if (cd2>0)
                    {
                        MobileNotificationManager.SendNotification(7, cd2*1000,
                            LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                            LocalizationManager.Instance.GetLocalizedString("Notification_text_6"),
                            new Color32(0xff, 0x44, 0x44, 255));
                    }          
                    var cd3 = MergeManager.Instance.GetTimeProductCdByLine(20501,MergeBoardEnum.Main);
                    if (cd3>0)
                    {
                        MobileNotificationManager.SendNotification(8, cd3*1000,
                            LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                            LocalizationManager.Instance.GetLocalizedString("ui_new_notification1"),
                            new Color32(0xff, 0x44, 0x44, 255));
                    }   
                }

                if (KeepPetModel.Instance != null && KeepPetModel.Instance.CurState != null && KeepPetModel.Instance.IsOpen() && KeepPetModel.Instance.CurState.Enum == KeepPetStateEnum.Searching)
                {
                    MobileNotificationManager.SendNotification(9, KeepPetModel.Instance.Storage.SearchEndTime - (long)APIManager.Instance.GetServerTime(),
                        LocalizationManager.Instance.GetLocalizedString(APP_NAME_KEY),
                        LocalizationManager.Instance.GetLocalizedString("ui_new_notification3"),
                        new Color32(0xff, 0x44, 0x44, 255));
                }
            }
        }

        public void ClearNotifications()
        {
            if (UseDebug)
                return;
            DebugUtil.Log("Clearing Local Notifications");
            MobileNotificationManager.ClearScheduledNotifications();
            MobileNotificationManager.ClearDisplayedNotifications();
        }
    }
}