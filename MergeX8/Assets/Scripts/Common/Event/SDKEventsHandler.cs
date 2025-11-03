using DragonPlus.Config;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.SDKEvents;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.API;
using Google.Protobuf;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using Framework;
using Gameplay;

namespace DragonPlus
{
    public class SDKEventsHandler : MonoBehaviour,
        IEventHandler<ProfileConflictEvent>,
        IEventHandler<AccountLoginOtherDeviceEvent>,
        IEventHandler<ProfileReplacedEvent>,
        IEventHandler<LoginEvent>,
        IEventHandler<ProfileFetchedEvent>,
        IEventHandler<ProfileCreatedEvent>,
        IEventHandler<ProfileResolvedEvent>,
        IEventHandler<CheckUpdateEvent>,
        IEventHandler<AdjustIdUpdatedEvent>,
        IEventHandler<DownloadFileEvent>,
        IEventHandler<DiskFullEvent>,
        IEventHandler<DeepLinkEvent>,
        IEventHandler<AssetCheckClearEvent>,
        IEventHandler<ConfirmWindowEvent>,
        IEventHandler<AdsOnCurrencySpend>,
        IEventHandler<ConfigHubUpdatedEvent>,
        IEventHandler<UnfulfilledPaymentPending>
        
    {       

        DelayAction DelayAction = new DelayAction();

        // Use this for initialization
        void Awake()
        {
            EventManager.Instance.Subscribe<ProfileConflictEvent>(this);
            EventManager.Instance.Subscribe<AccountLoginOtherDeviceEvent>(this);
            EventManager.Instance.Subscribe<ProfileReplacedEvent>(this);
            EventManager.Instance.Subscribe<LoginEvent>(this);
            EventManager.Instance.Subscribe<ProfileFetchedEvent>(this);
            EventManager.Instance.Subscribe<ProfileCreatedEvent>(this);
            EventManager.Instance.Subscribe<ProfileResolvedEvent>(this);
            EventManager.Instance.Subscribe<CheckUpdateEvent>(this);
            EventManager.Instance.Subscribe<AdjustIdUpdatedEvent>(this);
            EventManager.Instance.Subscribe<DownloadFileEvent>(this);
            EventManager.Instance.Subscribe<DeepLinkEvent>(this);
            EventManager.Instance.Subscribe<DiskFullEvent>(this);
            EventManager.Instance.Subscribe<AssetCheckClearEvent>(this);
            EventManager.Instance.Subscribe<ConfirmWindowEvent>(this);
            EventManager.Instance.Subscribe<AdsOnCurrencySpend>(this);
            EventManager.Instance.Subscribe<ConfigHubUpdatedEvent>(this);
            EventManager.Instance.Subscribe<UnfulfilledPaymentPending>(this);
        }

        void OnDestroy()
        {
            EventManager.Instance.Unsubscribe<ProfileConflictEvent>(this);
            EventManager.Instance.Unsubscribe<AccountLoginOtherDeviceEvent>(this);
            EventManager.Instance.Unsubscribe<ProfileReplacedEvent>(this);
            EventManager.Instance.Unsubscribe<LoginEvent>(this);
            EventManager.Instance.Unsubscribe<ProfileFetchedEvent>(this);
            EventManager.Instance.Unsubscribe<ProfileCreatedEvent>(this);
            EventManager.Instance.Unsubscribe<ProfileResolvedEvent>(this);
            EventManager.Instance.Unsubscribe<CheckUpdateEvent>(this);
            EventManager.Instance.Unsubscribe<AdjustIdUpdatedEvent>(this);
            EventManager.Instance.Unsubscribe<DownloadFileEvent>(this);
            EventManager.Instance.Unsubscribe<DeepLinkEvent>(this);
            EventManager.Instance.Unsubscribe<DiskFullEvent>(this);
            EventManager.Instance.Unsubscribe<AssetCheckClearEvent>(this);
            EventManager.Instance.Unsubscribe<ConfirmWindowEvent>(this);
            EventManager.Instance.Unsubscribe<AdsOnCurrencySpend>(this);
            EventManager.Instance.Unsubscribe<ConfigHubUpdatedEvent>(this);
            EventManager.Instance.Unsubscribe<UnfulfilledPaymentPending>(this);
        }

        public void OnNotify(ProfileConflictEvent profileConflictEvent)
        {
            WaitingManager.Instance.CloseWindow(); // 一般在绑定后， 没有走回调而是走了这里，所以这里要关闭转菊花
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChooseProfilePop);
            var profile = profileConflictEvent.ServerProfile;
            // var storageHome =
            //     DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHome>();
            // if (storageHome.CurRoomId <= 0)
            // {
            //     DragonU3DSDK.Storage.StorageManager.Instance.ResolveProfileConfict(profile, true);
            //     return;
            // }

            UIManager.Instance.OpenUI(UINameConst.UIPopupChooseProgress)
                .GetComponent<UIPopupChooseProgressController>().SetData(profile);
        }

        public void OnNotify(AccountLoginOtherDeviceEvent accountLoginOtherDeviceEvent)
        {
            UIPopupChooseProgressController.IsOpenWindow = true;

            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_errmsg_duplicate_login_native"),
                OKCallback = () =>
                {
                    SceneFsm.mInstance.ClientInited = false;
                    SceneFsm.mInstance.BackToLogin();
                    UIPopupChooseProgressController.IsOpenWindow = false;
                },
                HasCloseButton = false
            });
        }

        public void OnNotify(ProfileReplacedEvent profileReplacedEvent)
        {
            DebugUtil.Log("profile replaced by server");

            // 刷新一次成就的数据,防止存档重置后成就监听出现的问题

            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Login)
            {
                if (profileReplacedEvent.clear)
                {
                    // 覆盖存档,重新初始化
                    SceneFsm.mInstance.ClientInited = false;
                    SceneFsm.mInstance.BackToLogin();
                }
                else
                {
                    //GameBIManager.Instance.SendGameEvent( BiEventAdventureIslandMerge.Types.GameEventType.GameEventChooseProfileForce);
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString =
                            LocalizationManager.Instance.GetLocalizedString(
                                "&key.UI_profile_force_use_server_profile_desc"),
                        OKCallback = () =>
                        {
                            // 覆盖存档,重新初始化
                            // SceneFsm.mInstance.ClientInited = false;
                            // LocalizationManager.Instance.MatchLanguage();
                            // SceneFsm.mInstance.BackToLogin();
                            
                            DebugCmdExecute.QuitApp();
                        },
                        HasCloseButton = false,
                        IsLockSystemBack = true,
                        IsHighSortingOrder = true
                    });
                }
            }
            else
            {
                // 覆盖存档,重新初始化
                SceneFsm.mInstance.ClientInited = false;
            }
        }

        public void OnNotify(LoginEvent loginEvent)
        {
        }

        public void OnNotify(ProfileFetchedEvent profileFetchedEvent)
        {
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventGetProfileFinish);
        }

        public void OnNotify(ProfileCreatedEvent profileCreatedEvent)
        {
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteCreateProfileSuccess);
        }

        public void OnNotify(ProfileResolvedEvent profileResolvedEvent)
        {
            var server = profileResolvedEvent.server;
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventResolveProfile, server.ToString());
        }

        public void OnNotify(CheckUpdateEvent checkUpdateEvent)
        {
            //switch (checkUpdateEvent.checkUpdateEventType)
            //{
            //    case CheckUpdateEvent.CheckUpdateEventType.CHECK_UPDATE_EVENT_TYPE_START:
            //        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventCheckUpdateStart);
            //        break;
            //    case CheckUpdateEvent.CheckUpdateEventType.CHECK_UPDATE_EVENT_TYPE_FINISH:
            //        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventCheckUpdateFinish, checkUpdateEvent.updateResult.ToString());
            //        break;
            //    case CheckUpdateEvent.CheckUpdateEventType.CHECK_UPDATE_EVENT_TYPE_FAILURE:
            //        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventCheckUpdateFailure, checkUpdateEvent.errno);
            //        break;
            //}
        }

        public void OnNotify(AdjustIdUpdatedEvent adjustIdUpdatedEvent)
        {
            DebugUtil.Log("adjust id updated");
        }

        public void OnNotify(DownloadFileEvent downloadFileEvent)
        {
            switch (downloadFileEvent.stage)
            {
                case "DiskFullException":
                {
                    UIManager.Instance.OpenUI(UINameConst.UILocalNotice)?
                        .GetComponent<UIPopupNoticeController>()?
                        .SetData(new NoticeUIData
                        {
                            DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_storagefull_notice_text"),
                            HasCloseButton =  false,
                            HasCancelButton =  false,
                        });
                    break;
                }
                break;
            }
            
            /*
            switch (downloadFileEvent.stage)
            {
                case "start":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadStart, downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), downloadFileEvent.data);
                    break;
                case "finish":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFinish, downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), downloadFileEvent.data);
                    DebugUtil.Log("GameEventDownloadFinish: " + downloadFileEvent.name);
                    DelayAction.Debounce(
                        "DownloadFinish",
                        1000,
                        () =>
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.DOWNLOAD_FINISH);
                        }
                    );
                    break;
                case "failure":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFailure, downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), downloadFileEvent.data);
                    break;
                case "timeout":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFailure, downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), "timeout");
                    break;
                case "abort":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFailure, "head" + downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), "abort");
                    break;
                case "head_start":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadStart, "head" + downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), downloadFileEvent.data);
                    break;
                case "head_finish":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFinish, "head" + downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), downloadFileEvent.data);
                    break;
                case "head_failure":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFailure, "head" + downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), downloadFileEvent.data);
                    break;
                case "head_timeout":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFailure, "head" + downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), "timeout");
                    break;
                case "head_abort":
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDownloadFailure, "head" + downloadFileEvent.name, downloadFileEvent.bytesDownloaded.ToString(), "abort");
                    break;
                case "DiskFullException":
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_storagefull_notice_text")
                    });
                    break;
            }
            */
        }

        public void OnNotify(DeepLinkEvent deepLinkEvent)
        {
            DebugUtil.Log("DeepLinkEvent : path = {0} title = {1} content = {2}", deepLinkEvent.route,
                deepLinkEvent.title, deepLinkEvent.content);

            DeepLinkModel.Instance.OnWorking(deepLinkEvent);
        }

        public void OnNotify(DiskFullEvent evt)
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_storagefull_notice_text")
            });
        }

        public void OnNotify(AssetCheckClearEvent evt)
        {
            SDKManagerPack.Instance.Release();
        }

        public void OnNotify(ConfirmWindowEvent evt)
        {
            UIManager.Instance.OpenUI(UINameConst.UILocalNotice)?
                .GetComponent<UIPopupNoticeController>()?
                .SetData(new NoticeUIData
                {
                    DescString = evt.UiData?.DescString,
                    HasCloseButton =  false,
                    HasCancelButton =  false,
                });
        }

        public void OnNotify(AdsOnCurrencySpend message)
        {
            //AdLogicManager.Instance.OnNotify(message);
        }

        public void OnNotify(ConfigHubUpdatedEvent evt)
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.OnConfigHubUpdated);
        }     
        public void OnNotify(UnfulfilledPaymentPending evt)
        {
            DebugUtil.Log("SDK 补单通知======>UnfulfilledPaymentPending evt"+evt.data.ProductId);
            if (SceneFsm.mInstance.ClientLogin)
            {
                Dlugin.SDK.GetInstance().iapManager.RequestUnfulfilledPaymentsAndTryVerify(StoreModel.Instance.OnPurchased,evt.data.ProductId);
            }
        }
    }
}