using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Activity.CrazeOrder.Model;
using Activity.LimitTimeOrder;
using Activity.Matreshkas.Model;
using Activity.SaveTheWhales;
using Activity.TimeOrder;
using ConnectLine;
using Decoration;
using Decoration.Bubble;
using Decoration.DynamicMap;
using Decoration.WorldFogManager;
using DragonPlus;
using DragonPlus.Config;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.Config.CardCollect;
using DragonPlus.Config.ClimbTower;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.Ditch;
using DragonPlus.Config.Farm;
using DragonPlus.Config.Filthy;
using DragonPlus.Config.FishEatFish;
using DragonPlus.Config.FishEatFishInner;
using DragonPlus.Config.FishEatFishInnerTwo;
using DragonPlus.Config.Makeover;
using DragonPlus.Config.MiniGame;
using DragonPlus.Config.Team;
using DragonPlus.Config.TileMatch;
using DragonPlus.Config.TipReward;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.ABTest;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Dynamic;
using Framework;
using Game;
using Game.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Gameplay;
using Gameplay.UI.Capybara;
using Gameplay.UI.Store.Vip.Model;
using Gameplay.UI.UpdateRewardManager;
using Manager;
using Merge.Order;
using MiniGame;
using OnePath;
using Psychology;
using Psychology.Model;
using Scripts.UI;
using SomeWhere;
using Stimulate.Configs;
using Stimulate.Model;
using Debug = UnityEngine.Debug;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using Utils = Makeover.Utils;

public class LoadingController : MonoBehaviour
{
    public LocalizeTextMeshProUGUI loadingText;
    public LocalizeTextMeshProUGUI loadingTips;

    private float lastUpdate = 0f;
    private int numElipses = 1;

    public Slider slider;
    private const int CHECK_VERSION_FILE_TRY_TIME = 3;

    //------------------------- 进度控制
    private float displayProgress = 0f;

    private float toProgress = 0f;
    // 初始包里的资源copy到下载目录结束，进度条走5

    //-------------------------- end
    private Stopwatch stopWatch = new Stopwatch();
    private bool goOnLoading = true;

    private Func<float, float, IEnumerator>[] coroutines;
    private float[] progress;
    private bool loadResEnd;

    bool ClickedBackButton { get; set; }

    private static LoadingController loadingController = null;

    private void OnEnable()
    {
        displayProgress = 0;
        toProgress = 0;
        EventDispatcher.Instance.AddEventListener(EventEnum.DOWNLOAD_ERROR, OnDownloadError);
        EventDispatcher.Instance.AddEventListener(EventEnum.GDPR_ACCEPTED, OnPrivacyAccepted);
        EventDispatcher.Instance.AddEventListener(EventEnum.GDPR_REFUSED, OnPrivacyRefused);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.DOWNLOAD_ERROR, OnDownloadError);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GDPR_ACCEPTED, OnPrivacyAccepted);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GDPR_REFUSED, OnPrivacyRefused);
        DragonU3DSDK.Device.Instance.RemoveBackButtonCallback(BackButtonCallback);
    }

    public void OnDownloadError(BaseEvent e)
    {
        TipNetworkError();
    }

    public void OnPrivacyAccepted(BaseEvent gdprAcceptedEvent)
    {
#if !UNITY_EDITOR
        PrivacyModel.Instance.SetPrivacyAgreed(true);
#endif
        StartCoroutine(MainLoading());
    }

    public void OnPrivacyRefused(BaseEvent gdprRefusedEvent)
    {
        PrivacyModel.Instance.TryToShowPrivacy();
    }

    void BackButtonCallback()
    {
        //选档界面屏蔽返回键
        if (UIPopupChooseProgressController.IsOpenWindow)
            return;

        if (ClickedBackButton)
            return;
        ClickedBackButton = true;
    }

    private void Awake()
    {
        coroutines = new[]
        {
            (Func<float, float, IEnumerator>) FirstRunAppCheck,
            AppVersionCheck,
            CheckNextSceneResourceVersion,
            LoadConfig,
            PreloadResources,
        };
        progress = new[]
        {
            5f,
            20f,
            80f,
            85f,
            99f,
        };

        AudioManager.Instance.StopAllMusic();
        transform.Find("BlackForCG").gameObject.SetActive(false);
        slider = transform.Find("Slider").GetComponent<Slider>();
        slider.value = 0f;
        loadingText = slider.transform.Find("progressinfo").GetComponent<LocalizeTextMeshProUGUI>();
        loadingTips = slider.transform.Find("loadingtips").GetComponent<LocalizeTextMeshProUGUI>();

        if (Application.internetReachability == NetworkReachability.NotReachable)
            VersionManager.Instance.StartOfflineMode();

        DebugUtil.Log($"{GetType()}: is offline mode = {VersionManager.Instance.OfflineMode}");
        loadingText?.gameObject.SetActive(false);
        LocaleConfigManager.Instance.LoadBakLocale();
        loadingTips?.SetText(GameTextUtils.GetRandomTip());
        //屏蔽loading Tips
        loadingTips?.gameObject.SetActive(false);
        ClickedBackButton = false;
        DragonU3DSDK.Device.Instance.AddBackButtonCallback(BackButtonCallback);

        //UIManager.Instance.OpenCookingWindow("Common/PrivacyPolicy");

        if (DragonU3DSDK.DeviceHelper.IsFullScreenIOS())
        {
            var tipTransform = transform.Find("Slider/loadingtips");
            tipTransform.localPosition = new Vector3(tipTransform.localPosition.x, 45, tipTransform.localPosition.z);
        }

        //关闭资源栏
        UIManager.Instance.CloseUI(UINameConst.UIMainGroup, true);

        if (transform.Find("Loading_2/1") && transform.Find("Loading_2/2"))
        {
            var oldBG = transform.Find("Loading_2/1");
            var newBG = transform.Find("Loading_2/2");
            var oldLogo = transform.Find("logoGroup/1");
            var newLogo = transform.Find("logoGroup/2");
            var curTime = APIManager.Instance.GetServerTime();
            ulong changeTime = 1761235200000;
            oldBG.gameObject.SetActive(curTime < changeTime);
            newBG.gameObject.SetActive(curTime >= changeTime);
            oldLogo.gameObject.SetActive(curTime < changeTime);
            newLogo.gameObject.SetActive(curTime >= changeTime);
        }
    }

    private IEnumerator MainLoading()
    {
        if (coroutines != null)
        {
            for (int i = 0; i < coroutines.Length; i++)
            {
                yield return StartCoroutine(coroutines[i](i == 0 ? 0 : progress[i - 1], progress[i]));
                toProgress = progress[i];
                if (!goOnLoading)
                {
                    while (!goOnLoading)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        LocaleConfigManager.Instance.InitConfigs();
        toProgress = 100f;
        while (displayProgress < toProgress)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneFsm.mInstance.ClientInited = true;

        MyMain.Game.InitManager(false);

        EventDispatcher.Instance.DispatchEvent(EventEnum.LOADING_FINISH);
    }

    public void StartLoading()
    {
        displayProgress = 0;
        toProgress = 0;

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtueLaunchApp);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLoading);

        if (PrivacyModel.Instance.ShouldShowPrivacy())
        {
            PrivacyModel.Instance.TryToShowPrivacy();
            StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.UpdatePrivacyPolicy = true;
        }
        else if (!StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.UpdatePrivacyPolicy)
        {
            StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.UpdatePrivacyPolicy = true;
            UIManager.Instance.OpenUI("PrivacyPolicy", windowType:UIWindowType.Normal, windowLayer:UIWindowLayer.Max, type:typeof(PrivacyPolicyController), false);
        }
        else
        {
            StartCoroutine(MainLoading());
        }
    }

    void Update()
    {
        LoadingText();
        if (displayProgress < toProgress)
        {
            float diffValue = toProgress - displayProgress;
            float deltaValue = 0.02f;

            if (loadResEnd) //资源文件下载完后，进度条最小走1
                deltaValue = Mathf.Max(1.0f, diffValue * 0.1f);
            else if (diffValue < 0.5f) //资源下载过程中，进度条精确到小数点后2位
                deltaValue = Mathf.Max(0.02f, diffValue * 0.1f);
            else
                deltaValue = Mathf.Max(0.1f, diffValue * 0.1f);

            displayProgress += deltaValue;
            displayProgress = Mathf.Min(100f, displayProgress);
            SetLoadingPrecentage(displayProgress);

            //if (displayProgress >= 100f)
            //{
            //    EventDispatcher.Instance.DispatchEvent(EventEnum.SPEED_UP_LOADING_ANIMATION, 400); //加速400倍,为了让动画瞬间播完

            //    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLoadingFinish);
            //}
            //else if ((int) displayProgress % 10 == 0)
            //{
            //    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLoadingProgress, ((int) displayProgress).ToString());
            //}
        }
    }


    // 是否是安装后第一次运行
    private IEnumerator FirstRunAppCheck(float begin, float end)
    {
        yield return new WaitForSeconds(0.2f);

        bool needPatchAB = false;
        if (!SceneFsm.mInstance.ClientInited) // 每次启动游戏，只做一次判定，避免卡0%
        {
#if UNITY_EDITOR
            if (AssetConfigController.Instance.UseAssetBundle)
                needPatchAB = VersionManager.Instance.IsNeedPatchVersionAfterOverLayInstall();
#else
            needPatchAB = VersionManager.Instance.IsNeedPatchVersionAfterOverLayInstall();
#endif
        }

        if (needPatchAB)
        {
            yield return StartCoroutine(PatchVersionAfterOverLayInstall());
        }
        else
        {
#if UNITY_EDITOR
            yield return StartCoroutine(VersionManager.Instance.CheckInInitialPacketExport(false));
            yield return null;
#else
            yield return StartCoroutine(VersionManager.Instance.CheckInInitialPacketExport(false));
#endif
        }
    }

    // 拷贝初始包里的AB包到下载目录
    private IEnumerator PatchVersionAfterOverLayInstall()
    {
        this.stopWatch.Restart();

        //如果要调整StartProtect的调用逻辑，则需要注意PatchVersionAfterOverLayInstall
        // if (VersionManager.Instance.IsFirstInstall())
        // {
        //     VersionManager.Instance.StartProtect();
        // }

        yield return StartCoroutine(VersionManager.Instance.PatchVersionAfterOverLayInstall());
        this.stopWatch.Stop();
        //DebugUtil.LogError("qushuang =====> CopyFilesToDownloadPath =====> " + this.stopWatch.ElapsedMilliseconds);
    }

    // 强更检查
    private IEnumerator AppVersionCheck(float begin, float end)
    {
        LocaleConfigManager.Instance.InitConfigs();
        GlobalConfigManager.Instance.InitTableConfigs();

        this.stopWatch.Restart();
        var hasResult = false;
        var result = AppUpdateResult.Newest;
        if (VersionManager.Instance.OfflineMode)
        {
            hasResult = true;
        }
        else
        {
            float waitTime = 3;
            StorageHome home = StorageManager.Instance.GetStorage<StorageHome>();
            if (home.IsFirstLoading)
            {
                PlayerPrefs.SetFloat("CheckAppVersionWaitTime", waitTime);
            }

            home.IsFirstLoading = true;

            VersionManager.Instance.CheckAppVersion((AppUpdateResult r) =>
            {
                this.stopWatch.Stop();
                hasResult = true;
                result = r;
            }, waitTime);
        }

        while (!hasResult)
        {
            yield return null;
        }

        OnVersionCheckOver(result);
    }

    private void OnVersionCheckOver(AppUpdateResult result)
    {
        if (result != AppUpdateResult.Newest)
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_update_app_desc_text"),
                HasCloseButton = result == AppUpdateResult.Suggested,
                sortingOrder = 200,
                OKCallback = () =>
                {
                    Global.OpenAppStore();
                    //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventCheckUpdateOpenStore);
                    if (result == AppUpdateResult.Suggested)
                        goOnLoading = true;

                    StartCoroutine(CommonUtils.DelayWork(1f, () =>
                    {
                        OnVersionCheckOver(result);
                    }));
                },
                CancelCallback = () =>
                {
                    if (result == AppUpdateResult.Suggested)
                        goOnLoading = true;
                }
            });

            goOnLoading = false;
        }
    }

    // 检查资源版本:每个场景必须的资源
    private IEnumerator CheckNextSceneResourceVersion(float begin, float end)
    {
        bool VersionFileLoaded = false;
        bool ResourceCheckedOver = false;

        //-------- 新手保护期间，如果不能离线进入下个场景，说明要开始下载资源了，退出保护 --------//
        if (VersionManager.Instance.ProtectMode)
        {
            if (!CanOfflineEnterNextScene())
            {
                VersionManager.Instance.EndProtect();
            }
        }

        //---------------------------------- end -------------------------------------//
        Dictionary<string, string> needDownloadFiles = new Dictionary<string, string>();
        for (int i = 0; i < CHECK_VERSION_FILE_TRY_TIME; i++)
        {
            var hasResult = false;
            needDownloadFiles.Clear();
            if (Application.internetReachability == NetworkReachability.NotReachable ||
                VersionManager.Instance.OfflineMode) //无网，或者已进入离线模式
            {
                VersionManager.Instance.LoadVersionFile((bool success) =>
                {
                    // 离线模式下，不管success的值，只用确保localverison加载成功了就行
                    if (CanOfflineEnterNextScene())
                    {
                        // 下个场景可以离线进入
                        VersionFileLoaded = true;
                    }
                    else
                    {
                        // 下个场景不可以离线进入，弹框让玩家选择
                        if (i == CHECK_VERSION_FILE_TRY_TIME - 1)
                        {
                            TipNetworkError();
                        }
                    }

                    hasResult = true;
                });
            }
            else
            {
                // 先更新version文件
                VersionManager.Instance.LoadVersionFile((bool success) =>
                {
                    if (success)
                    {
                        //ui
                        var homeUIFiles =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.UI], null);
                        if (homeUIFiles.Count > 0) needDownloadFiles.Merge(homeUIFiles);

                        // data
                        var homeDataFIles =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.Data]);
                        if (homeDataFIles.Count > 0) needDownloadFiles.Merge(homeDataFIles);
                        
                        // localAtlas
                        var localAtlas =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.LocalSpriteAtlas]);
                        if (localAtlas.Count > 0) needDownloadFiles.Merge(localAtlas);
                        
                        
                        // tmatch
                        var tmatch =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.TMatch]);
                        if (tmatch.Count > 0) needDownloadFiles.Merge(tmatch);
                        
                        // onePath
                        var onePath =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.OnePath]);
                        if (onePath.Count > 0) needDownloadFiles.Merge(onePath);
                        
                        // //asmr 
                        // var asmrAssets = ASMR.Model.Instance.GetBaseDownLoadAssets();
                        // if (asmrAssets.Count > 0) needDownloadFiles.Merge(asmrAssets);
                        
                        //stimulate
                        var stimulate = StimulateModel.Instance.GetBaseDownLoadAssets();
                        if (stimulate.Count > 0) needDownloadFiles.Merge(stimulate);
                        
                        // //Psychology
                        // var psychology = PsychologyModel.Instance.GetBaseDownLoadAssets();
                        // if (psychology.Count > 0) needDownloadFiles.Merge(psychology);
                        
                        //挖沟
                        var digTrenchAssets = DigTrench.Model.Instance.GetBaseDownLoadAssets();
                        if (digTrenchAssets.Count > 0) needDownloadFiles.Merge(digTrenchAssets);
                        
                        // var cardCollection =
                        //     VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.CardCollection]);
                        // if (cardCollection.Count > 0) needDownloadFiles.Merge(cardCollection);
                        
                        var extraOrderRewardCoupon =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.ExtraOrderRewardCoupon]);
                        if (extraOrderRewardCoupon.Count > 0) needDownloadFiles.Merge(extraOrderRewardCoupon);
                        
                        var keepPet =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.KeepPet]);
                        if (keepPet.Count > 0) needDownloadFiles.Merge(keepPet);
                        
                        var blueBlock =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.BlueBlock]);
                        if (blueBlock.Count > 0) needDownloadFiles.Merge(blueBlock);
                        
                        var blindBox =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.BlindBox]);
                        if (blindBox.Count > 0) needDownloadFiles.Merge(blindBox);
                        
                        var screw =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.Screw]);
                        if (screw.Count > 0) needDownloadFiles.Merge(screw);
                        
                        var farm =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.Farm]);
                        if (farm.Count > 0) needDownloadFiles.Merge(farm);
                        
                        var digTrenchNewAssets =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.DigTrenchNew]);
                        if (digTrenchNewAssets.Count > 0) needDownloadFiles.Merge(digTrenchNewAssets);
                        
                        var tileMatchAssets =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.TileMatch]);
                        if (tileMatchAssets.Count > 0) needDownloadFiles.Merge(tileMatchAssets);
                        
                        
                        var activityLocalAssets =
                            VersionManager.Instance.GetUpdateFilesDict("ActivityLocal");
                        if (activityLocalAssets.Count > 0) needDownloadFiles.Merge(activityLocalAssets);
                        
                        var saveTheWhalesLocalAssets =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.SaveTheWhales]);
                        if (saveTheWhalesLocalAssets.Count > 0) needDownloadFiles.Merge(saveTheWhalesLocalAssets);
                        
                        var pigBoxLocalAssets =
                            VersionManager.Instance.GetUpdateFilesDict(AssetBundleEnum.GroupNames[GroupEnum.PigBox]);
                        if (pigBoxLocalAssets.Count > 0) needDownloadFiles.Merge(pigBoxLocalAssets);
                            
                        //当前world的资源
                        DecorationConfigManager.Instance.InitConfigs();
                        var worldDecorations = AssetCheckManager.Instance.GetNeedDownloadFiles();
                        if (worldDecorations != null && worldDecorations.Count > 0) 
                            needDownloadFiles.Merge(worldDecorations);

                        if (DebugCmdExecute.isFBVersion)
                        {
                            needDownloadFiles.Clear();
                        }

                        VersionFileLoaded = true;
                    }
                    else
                    {
                        if (i == CHECK_VERSION_FILE_TRY_TIME - 1)
                        {
                            TipNetworkError();
                        }
                    }

                    hasResult = true;
                });
            }

            while (!hasResult)
            {
                yield return new WaitForEndOfFrame();
            }

            if (VersionFileLoaded)
            {
                break;
            }
        }

        while (!VersionFileLoaded)
        {
            yield return new WaitForEndOfFrame();
        }

        needDownloadFiles = this.FiterUpdateFiles(needDownloadFiles);
        List<DownloadInfo> allTask = new List<DownloadInfo>();
        float deltaProgress = end - begin;
        if (needDownloadFiles.Count > 0) // 去下载
        {
            int resCount = needDownloadFiles.Count;
            foreach (KeyValuePair<string, string> kv in needDownloadFiles)
            {
                DownloadInfo info = DownloadManager.Instance.DownloadInSeconds(kv.Key, kv.Value, (downloadinfo) =>
                {
                    if (downloadinfo.result == DownloadResult.Success)
                    {
                        VersionManager.Instance.RefreshRemoteToLocal(new List<string>(new[] {kv.Key}));
                        resCount--;
                        if (resCount <= 0) // 所有文件都成功下载到本地了
                        {
                            ResourceCheckedOver = true;
                        }
                    }
                    else
                    {
                        // 超时，或者 重试3次后依然下载错误
                        if (downloadinfo.result != DownloadResult.ForceAbort) // 强制终止下载的不算
                            TipNetworkError();
                    }
                });
                allTask.Add(info);
            }
        }
        else //不需要更新任何文件
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.SPEED_UP_LOADING_ANIMATION);
            ResourceCheckedOver = true;
        }

        while (!ResourceCheckedOver)
        {
            // 更新进度
            int taskCount = allTask.Count;
            if (taskCount > 0)
            {
                float downloadedBytes = 0f;
                float totalBytes = 0f;
                for (int i = 0; i < taskCount; i++)
                {
                    if (allTask[i].downloadSize > allTask[i].downloadedSize) //确保get httphead之后，才开始算进度
                    {
                        totalBytes += allTask[i].downloadSize;
                        downloadedBytes += allTask[i].downloadedSize;
                    }
                }

                if (totalBytes > 0)
                {
                    float rate = downloadedBytes / totalBytes;
                    toProgress = begin + rate * deltaProgress;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        loadResEnd = true;
    }

    private IEnumerator LoadConfig(float begin, float end)
    {
        // 如果是第一次进loading，加载配置表 
        if (!SceneFsm.mInstance.ClientInited || ChangeableConfig.Instance.HasNewRemoteData)
        {
#if ENABLE_INJECTFIX
            IFixManager.Init();
#endif
            
            AdConfigManager.Instance.InitLocal();
            AdLocalConfigHandle.Instance.InitConfig();
            AdLocalConfigHandle.Instance.InitGroup();
            AutoPopupManager.AutoPopupManager.Instance.Init();
            DragonPlus.ConfigHub.Ad.AdConfigManager.Instance.Init();
            AdConfigManager.Instance.IsRemote = false;
            DragonPlus.ConfigHub.ConfigHubManager.Instance.Init();
            UserGroupManager.Instance.Init();
            NodeBubbleManager.Instance.UnLoadBubble();
            NodeBubbleManager.Instance.Init();

            RuntimeAnimatorManager.Instance.InitRuntimeAnimators();
            RuntimeEffectManager.Instance.InitEffect();
            
            AdRemoteConfigLogic.Instance.Init();
            FunctionsSwitchManager.Instance.Init();
            UpdateRewardManager.Instance.Init();
            ResourcesManager.Instance.Clear();
            ContactUsConfigManager.Instance.InitConfigs();

            Utils.InitServerConfig();
            TimeOrderModel.Instance.CheckJoinEnd();
            CrazeOrderModel.Instance.CheckJoinEnd();
            LimitTimeOrderModel.Instance.CheckJoinEnd();
            MatreshkasModel.Instance.CheckJoinEnd();

            toProgress += 1;
            LocaleConfigManager.Instance.InitConfigs(true);
            toProgress += 1;
            GlobalConfigManager.Instance.InitTableConfigs();
            EnergyModel.Instance.OnGameResReady();
            CardCollectConfigManager.Instance.InitConfig();
            CardCollectionModel.Instance.InitConfig();
            GameConfigManager.Instance.InitConfigs();
            LevelUpPackageModel.Instance.InitConfig();
            AdConfigExtendConfigManager.Instance.InitConfig();
           
            MergeConfigManager.Instance.InitMaxLevelId();
            DecorationConfigManager.Instance.InitConfigs(false);
            PathMapConfigManager.Instance.InitConfig();
            OrderConfigManager.Instance.InitTableConfigs();
            ConnectLineConfigManager.Instance.InitTableConfigs();
            KeepPetModel.Instance.InitConfig();
            DogPlayModel.Instance.InitConfig();
            BlindBoxModel.Instance.InitConfig();
            ClimbTowerConfigManager.Instance.InitConfig();
            VipStoreModel.Instance.Init();
            SaveTheWhalesModel.Instance.InitConfigLocal();
            //---------------------------TM---------------------------
            DragonPlus.Config.TMatch.TMatchConfigManager.Instance.InitConfig();
            // DragonPlus.Config.TMatchControl.TMatchControlConfigManager.Instance.InitConfig();
            DragonPlus.Config.TMatchShop.TMatchShopConfigManager.Instance.InitConfig();
            // DragonPlus.Config.TMatchStore.TMatchStoreConfigManager.Instance.InitConfig();
            DragonPlus.Config.OutsideGuide.OutsideGuideConfigManager.Instance.InitConfig();
            TMatch.ItemModel.Instance.InitCfg();
            TMatch.ItemAutoClaimModel.Instance.InitCfg();
            TMatch.ItemAutoClaimModel.Instance.Init();
            TMatch.ReviveGiftPackController.Instance.InitModel();
            
            TeamConfigManager.Instance.InitConfig();
            ABTestManager.Instance.Init();
            
            MiniGameConfigManager.Instance.InitConfig();
            MiniGameModel.Instance.Init();
            
            
            FarmConfigManager.Instance.InitConfig();

            
            DragonPlus.Config.Screw.GameConfigManager.Instance.InitConfig();
            
            //-------------------------------TileMatch--------------------------------
            TileMatchConfigManager.Instance.InitConfig();
            //------------------------------------------------------------------------
            
            TipRewardConfigManager.Instance.InitConfig();
            
            //闪购数据初始化
            StoreModel.Instance.GetFlashSaleItems();

            PigBankModel.Instance.InitLocal();

            EndlessEnergyGiftBagModel.Instance.Init();
                
            
            /*
             * 去掉所有老的小游戏
             */
            // MakeoverConfigManager.Instance.InitConfig();
            // DigTrenchConfigManager.Instance.InitConfig();
            // FishEatFishConfigManager.Instance.InitConfig();
            // FishEatFishInnerConfigManager.Instance.InitConfig();
            // FishEatFishInnerTwoConfigManager.Instance.InitConfig();
            // StimulateConfigManager.Instance.InitConfig();
            // PsychologyConfigManager.Instance.InitTableConfigs();
            // OnePathConfigManager.Instance.InitTableConfigs();
            // OnePathConfigManager.Instance.GetConfig(1);
            // FilthyConfigManager.Instance.InitConfig();
            // DitchConfigManager.Instance.InitConfig();
            
            
            ActivityManager.Instance.RequestActivityInfosFromServer();
            yield return new WaitForSeconds(0.4f);
            try
            {
                InitIAP();
            }
            catch (Exception e)
            {
                DebugUtil.LogError("iap " + e.Message);
            }

            toProgress += 1;
            LanguageModel.Instance.Init();
            //LocalizationManager.Instance.SetCurrentLocale(LanguageModel.Instance.GetLocale());
            toProgress += 1;

            if (!SceneFsm.mInstance.ClientInited)
            {
                DragonU3DSDK.DragonNativeBridge.InitSound("Export/Audios/Sound");
                toProgress += 1;
            }

            MergeManager.Instance.Refresh(MergeBoardEnum.Main);
            MergeBoard.InitPool();

            NewEventModel.Instance.GetUpdateInfo();
            PayLevelModel.Instance.UpdateState();
            TeamManager.Instance.Init();
        }

        if (AccountManager.Instance.HasLogin)
        {
            DeepLinkModel.Instance.TryClaimCoupon();
        }

#if !UNITY_EDITOR
        ChangeableConfig.Instance.FetchDataAsync();
#endif
        yield break;
    }

    void PreloadHomeAtlas()
    {
        //加载图集
        // ResourcesManager.Instance.LoadSpriteAtlasVariant(AtlasName.Normal);
        // ResourcesManager.Instance.LoadSpriteAtlasVariant(AtlasName.UIDecorationMain_IconBrush);
        //ResourcesManager.Instance.LoadSpriteAtlasVariant(AtlasName.UIStory);
    }

    private IEnumerator PreloadResources(float begin, float end)
    {
        UIManager.Instance.CloseUI(UINameConst.MergeMain, true);
        UIManager.Instance.OpenUI(UINameConst.MergeMain);
        UIManager.Instance.CloseUI(UINameConst.MergeMain);
        
        StorageHome home = StorageManager.Instance.GetStorage<StorageHome>();
        if (!home.IsFirstLogin)
            CapybaraManager.Instance.Init();

        PlayerManager.Instance.UnLoad();

        DecoManager.Instance.LoadWorlds();
        yield return new WaitForEndOfFrame();
        
        DecoManager.Instance.EnterWorld(DecoManager.Instance.CurrentWorldId, (process) =>
        {
            //var rProcess=Mathf.Lerp(0f,0.95f,process*4f);
            //loadingCtr?.SetProcess(rProcess);
        }, (success) =>
        {
            DragonU3DSDK.DebugUtil.Log($"EnterWolrd,success={success},");
            //isUpdate = true;
            //loadingCtr?.FakeProcess(0.95f,0.99f,2f);
        });

        PlayerManager.Instance.InitPlayers();
        
        while (!DecoManager.Instance.IsWorldReady)
        {
            yield return new WaitForEndOfFrame();
        }
        
        UIRoot.Instance.mWorldUIRoot.gameObject.SetActive(false);
        DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(false);
        DecoManager.Instance.ShowWorld(DecoManager.Instance.CurrentWorldId,true,true);
        DecoManager.Instance.EnableUpdate = false;
        WorldFogManager.Instance.Init();
        yield return new WaitForEndOfFrame();
        DynamicMapManager.Instance.InitDynamicObject(DecoSceneRoot.Instance.mSceneCamera.gameObject);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.3f);
    }

    private void SetLoadingPrecentage(float num)
    {
        float x = num * 0.01f;
        slider.value = x;

        //string info = LocalizationManager.Instance.GetLocalizedStringWithFormats("&key.UI_loading_progress_text", num.ToString("#0.0"));
        string info =
            $"{LocalizationManager.Instance.GetLocalizedString("&key.UI_loading_loading")}{num.ToString("#0.0")}%";
        loadingText.SetText(info);

        if (loadingText.gameObject.activeSelf == false)
        {
            loadingText?.gameObject.SetActive(LocaleConfigManager.Instance.IsInit);
        }
    }

    private void LoadingText()
    {
        const float EPSINON = 0.000001f;

        if (Math.Abs(lastUpdate) < EPSINON)
        {
            SetLoadingPrecentage(0f);
        }

        if (Math.Abs(lastUpdate) < EPSINON || Time.unscaledTime > (lastUpdate + 5f))
        {
            numElipses = numElipses == 3 ? 0 : numElipses + 1;
            lastUpdate = Time.unscaledTime;
            loadingTips.SetText(GameTextUtils.GetRandomTip());
        }
    }

    void InitIAP()
    {
        List<TableShop> configs = GlobalConfigManager.Instance.GetTableShop();
        List<string> consumableProducts = new List<string>();
        List<string> nonconsumableProducts = new List<string>();
        List<string> subscribeProducts = new List<string>();
        foreach (TableShop config in configs)
        {
#if UNITY_IOS
            if (!config.product_id_ios.IsEmptyString())
            {
                if (config.purchaseType == 1)
                {
                    nonconsumableProducts.Add(config.product_id_ios);
                }
                else if (config.purchaseType == 0)
                {
                    consumableProducts.Add(config.product_id_ios);
                }
                else
                {
                    subscribeProducts.Add(config.product_id_ios);
                }
            }
#elif UNITY_ANDROID
            if (!config.product_id.IsEmptyString())
            {
                if (config.purchaseType == 1)
                {
                    nonconsumableProducts.Add(config.product_id);
                }
                else if (config.purchaseType == 0)
                {
                    consumableProducts.Add(config.product_id);
                }
                else
                {
                    subscribeProducts.Add(config.product_id);
                }
            }
#endif
        }

        Dlugin.SDK.GetInstance().iapManager.Init(consumableProducts, nonconsumableProducts, subscribeProducts);
    }

    #region 离线模式处理

    private bool CanOfflineEnterNextScene()
    {
        if (SceneFsm.mInstance?.GetCurrSceneType() == StatusType.Home)
        {
            DecorationConfigManager.Instance.InitConfigs();
            var worldDecorations = AssetCheckManager.Instance.GetNeedDownloadFiles();
            if (worldDecorations != null && worldDecorations.Count > 0)
            {
                return false;
            }
            return true;
        }

        return true;
    }

    /// <summary>
    /// 离线模式下，要去往的场景资源不存在，回退到world
    /// </summary>
    private void OfflineBackToWorld()
    {
        VersionManager.Instance.StartOfflineMode();
    }

    #endregion

    #region retry和cancel的逻辑

    /// <summary>
    /// 提示用户下载错误
    /// </summary>
    private void TipNetworkError()
    {
        DownloadManager.Instance.AbortAllDownloadTask();
        UIPopupNoticeController noticeCtr =
            UIManager.Instance.GetOpenedUIByPath<UIPopupNoticeController>("Common/Notice");
        if (noticeCtr != null && noticeCtr.mIsOpen)
        {
            //DebugUtil.Log(CKer.admin, "loading界面，还有其他Notice弹框！！！！严重");
        }
        else
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_download_resources_error"),
                OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_retry_text"),
                CancelButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok"),
                sortingOrder = 200,

                OKCallback = () => { OnChooseRetry(); },
                CancelCallback = () => { OnChooseCancel(); },
                HasCancelButton = true,
                NoTweenClose = true,
                HasCloseButton = false
            });
        }
    }

    /// <summary>
    /// 点击retry的处理
    /// </summary>
    private void OnChooseRetry()
    {
        StopAllCoroutines();
        StartLoading();
    }

    private void OnChooseCancel()
    {
        if (CanOfflineEnterNextScene())
        {
            StartCoroutine(LoadingConfigs());
            VersionManager.Instance.EndProtect();
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable ||
                VersionManager.Instance.OfflineMode) //无网，或者已进入离线模式
            {
                TipNetworkError();
            }
            else
            {
                StopAllCoroutines();
                StartLoading();
            }
        }
    }

    private IEnumerator LoadingConfigs()
    {
        yield return StartCoroutine(LoadConfig(20f, 80f));
        yield return StartCoroutine(PreloadResources(80f, 99f));
            
        StopAllCoroutines();
        OfflineBackToWorld();

        SceneFsm.mInstance.ClientInited = true;
        MyMain.Game.InitManager(false);
        EventDispatcher.Instance.DispatchEvent(EventEnum.LOADING_FINISH);
    }

    #endregion

    #region 资源下载优化

    private Dictionary<string, string> FiterUpdateFiles(Dictionary<string, string> srcDict)
    {
        Dictionary<string, string> destDict = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> kv in srcDict)
        {
            string localFilePath = string.Format("{0}/{1}", FilePathTools.persistentDataPath_Platform, kv.Key);
            string localFileMd5 = AssetUtils.BuildFileMd5(localFilePath);
            if (localFileMd5 == null || (localFileMd5.Trim() != kv.Value?.Trim()))
            {
                destDict.Add(kv.Key, kv.Value);
            }
        }

        return destDict;
    }

    #endregion

    public static LoadingController ShowLoading()
    {
        if (loadingController != null)
            return loadingController;

        GameObject prefab = null;
        if(CommonUtils.IsLE_16_10())
            prefab = Resources.Load<GameObject>("Loading/UILoading_Pad");
        
        if(prefab == null)
            prefab = Resources.Load<GameObject>("Loading/UILoading");
        
        var loadingObj = GameObject.Instantiate(prefab);
        loadingObj.transform.SetParent(UIRoot.Instance.mRootCanvas.transform);
        (loadingObj.transform as RectTransform).SetFullRect();
        loadingObj.name = "UILoading";
        loadingController = loadingObj.AddComponent<LoadingController>();

        return loadingController;
    }

    public static void HideLoading()
    {
        if (loadingController == null)
            return;

        GameObject.Destroy(loadingController.gameObject);
        loadingController = null;
    }   
}