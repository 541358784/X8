using DragonPlus;
using Framework;
using Gameplay;
using UnityEngine;
using System.Collections;
using DragonU3DSDK.Account;
using DragonU3DSDK.Asset;
using Game;
using DragonPlus.Haptics;
using DragonPlus.Config;
using DragonPlus.ConfigHub.Ad;
using GamePool;

public class MyGame
{
    protected readonly SubSystemManager _subSystemManager = new SubSystemManager();

    public void Init()
    {
        _subSystemManager.Init();
        _InitSubSystems();
        AccountManager.Instance.Init();
        SettingManager.Instance.Init();
        CrocodileController.Instance.Init();

        ResourcesManager.Instance.SetAlwaysCache(false);

        // //设置英语 
        // LanguageModel.Instance.SetLocale(Locale.ENGLISH);
        // LocalizationManager.Instance.SetCurrentLocale(Locale.ENGLISH);

        LocalizationManager.Instance.MatchLanguage();

        HapticsManager.Init();
        Input.multiTouchEnabled = false;
        //NotificationManager.Instance.ClearNotifications();
    }

    public void Start()
    {
        _subSystemManager.Start();
    }

    public void Update()
    {
        _subSystemManager.Update(Time.deltaTime);
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        _subSystemManager.OnApplicationPause(pauseStatus);
    }

    public void Release()
    {
        _subSystemManager.Release();
    }

    private void _InitSubSystems()
    {
        if (_subSystemManager.AddGroup("Gameplay"))
        {
            _subSystemManager.AddSubSystem<CoroutineManager>();
            
            _subSystemManager.AddSubSystem<GamePauseManager>();
            _subSystemManager.AddSubSystem<StorageSubSystem>();
            _subSystemManager.AddSubSystem<NetworkSubSystem>();
            _subSystemManager.AddSubSystem<FaceBookSubSystem>();
            _subSystemManager.AddSubSystem<DownloadRecorder>();
            _subSystemManager.AddSubSystem<AppleAccountSubSystem>();
            _subSystemManager.AddSubSystem<AdSubSystem>();
            _subSystemManager.AddSubSystem<AppIconChangerSystem>();
            
            //Decoration World
            _subSystemManager.AddSubSystem<Decoration.DecoManager>();
            _subSystemManager.AddSubSystem<Decoration.AssetCheckManager>();
            // _subSystemManager.AddSubSystem<Decoration.RuntimeAtlasManager>();
            
            // 业务
            _subSystemManager.AddSubSystem<GameplayInfoManager>();
            _subSystemManager.AddSubSystem<UserData>();
            _subSystemManager.AddSubSystem<GuideSubSystem>();
            _subSystemManager.AddSubSystem<StorySubSystem>();
            _subSystemManager.AddSubSystem<CGVideoManager>();

            // 表现/UI
            _subSystemManager.AddSubSystem<UISubSystem>();
            
            // 预热
            _subSystemManager.AddSubSystem<PreloadSubSystem>();
            
            
                   
            // TM
            _subSystemManager.AddSubSystem<TMatch.LobbyTaskSystem>();
            _subSystemManager.AddSubSystem<TMatch.UnlimitItemModel>();
            _subSystemManager.AddSubSystem<TMatch.TMatchModel>();
            _subSystemManager.AddSubSystem<TMatch.WeeklyChallengeController>();
            _subSystemManager.AddSubSystem<TMatch.StarCurrencyModel>();
            _subSystemManager.AddSubSystem<TMatch.EnergyModel>();
            _subSystemManager.AddSubSystem<TMatch.ItemModel>();
            _subSystemManager.AddSubSystem<TMatch.RemoveAdModel>();
            _subSystemManager.AddSubSystem<TMatch.IceBreakingPackModel>();
            // _subSystemManager.AddSubSystem<TMatch.ReviveGiftPackModel>();
            _subSystemManager.AddSubSystem<TMatch.ReviveGiftPackController>();
            // TM End
            _subSystemManager.AddSubSystem<LastTimeCountModel>();
        }
        if (_subSystemManager.AddGroup("Framework"))
        {
            _subSystemManager.AddSubSystem<SDKManagerPack>();
            _subSystemManager.AddSubSystem<RenderTextureFactory>();
        }
    }

    public void InitManager(bool isLogin = true)
    {
        UserGroupManager.Instance.Init();

        AdSubSystem.Instance.Init();
        // MasterCardModel.Instance.InitOpenToggle();

        CoolingTimeManager.Instance.Init();
        RedPointManager.Instance.Init();

        DragonU3DSDK.Network.PlayerProperties.PlayerPropertiesManager.Instance.Init();

        if (isLogin && !AdConfigManager.Instance.IsRemote)
            DragonPlus.ConfigHub.ConfigHubManager.Instance.Init();

        InitObjPool();
        AdLocalConfigHandle.Instance.InitGroup();
    }

    private void InitObjPool()
    {
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.CommonTrail, 10,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.CommonHintStars, 10,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.MergeItem, 3,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.ResourceItem, 10,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.CommonNum, 1,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.CommonBgEffect, 3,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.CommonOpen, 3,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        //GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.LevelRankTips, 3,GamePool.ObjectPoolDelegate.CreateGameItem);
        //GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.PopRewardTips, 3,GamePool.ObjectPoolDelegate.CreateGameItem);

        foreach (var spine in OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Normal))
        {
            GamePool.ObjectPoolManager.Instance.CreatePool(string.Format(ObjectPoolName.PortraitSpine, spine.spineName), 1,
                GamePool.ObjectPoolDelegate.CreateGameItem);
        }
        foreach (var spine in OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Time))
        {
            GamePool.ObjectPoolManager.Instance.CreatePool(string.Format(ObjectPoolName.PortraitSpine, spine.spineName), 1,
                GamePool.ObjectPoolDelegate.CreateGameItem);
        }
        foreach (var spine in OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.KeepPet))
        {
            GamePool.ObjectPoolManager.Instance.CreatePool(string.Format(ObjectPoolName.PortraitSpine, spine.spineName), 1,
                GamePool.ObjectPoolDelegate.CreateGameItem);
        }

        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.ShopItemNomalPath, 6,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.ShopItemExchangePath, 6,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.ShopItemDailyPath, 6,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.ShopItemFlashPath, 6,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.ShopItemBundleNomalPath, 6,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.ShopItemBundleNormal1Path, 1,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.TaskAssistBundlePath, 2,
            GamePool.ObjectPoolDelegate.CreateGameItem);       
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.FishBundlePath, 1,
            GamePool.ObjectPoolDelegate.CreateGameItem);    
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.MergePackageUnit, 30,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.vfx_ComboMerge_01, 1,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.vfx_ComboMerge_02, 1,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.vfx_ComboMerge_03, 1,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        GamePool.ObjectPoolManager.Instance.CreatePool(ObjectPoolName.vfx_ComboMerge_04, 1,
            GamePool.ObjectPoolDelegate.CreateGameItem);
        
    }
}