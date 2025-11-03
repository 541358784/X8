using Framework;
using DragonU3DSDK.Storage;
using Game.Config;
using UnityEngine;

public static class DebugCmdExecute
{
    public static bool OpenAllModule;
    public static bool ShowDebugInfo;
    public static bool isFBVersion = false;
    public static bool isFBFriendVersion = true;

    ///
    ///
    ///  home word2
    ///
    ///
    public static void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    public static void ClearStorage()
    {
        StorageManager.Instance.GetStorage<StorageCommon>().Clear();
        StorageManager.Instance.GetStorage<StorageHome>().Clear();
        StorageManager.Instance.GetStorage<StorageGame>().Clear();
        StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.Decoration.StorageDecoration>().Clear();
        StorageManager.Instance.GetStorage<StorageTMatch>().Clear();
        StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel = 1;
        StorageManager.Instance.GetStorage<StorageCurrencyTMatch>().Clear();
        StorageManager.Instance.GetStorage<StorageDecorationGuide>().Clear();
        StorageManager.Instance.GetStorage<StorageScrew>().Clear();
        StorageManager.Instance.GetStorage<StorageMiniGames>().Clear();
        StorageManager.Instance.GetStorage<StorageFarm>().Clear();
        
        StorageManager.Instance.GetStorage<StorageASMR>().Clear();
        StorageManager.Instance.GetStorage<StorageMiniGameVersion>().Clear();
        // PlayerPrefs.DeleteKey ("RunOnce"); //StorageManager.runOnceKey
        // StorageManager.Instance.ClearAll();
    }


    public static void ClearPopupInterval()
    {
        //PopupSubSystem.Instance.ClearPopupIntervalTime();
    }

    public static void TryShowInterstitial()
    {
        Dlugin.SDK.GetInstance().m_AdsManager.PlayInterstitial(() =>
        {
            DragonU3DSDK.DebugUtil.LogP("PlayInterstitial is showing");
        });
    }

    public static void TryShowRewardedVideo()
    {
        Dlugin.SDK.GetInstance().m_AdsManager.PlayReward((needReward) =>
        {
            DragonU3DSDK.DebugUtil.LogP("PlayReward is showing");
        });
    }

    public static string GetAdBufferStatus()
    {
        return string.Format("chaping:{0},video:{1}_{2}", Dlugin.SDK.GetInstance().m_AdsManager.InterstitialStatus(),
            Dlugin.SDK.GetInstance().m_AdsManager.RewardVideoStatus(),
            Dlugin.SDK.GetInstance().m_AdsManager.RewardBufferCount());
    }

    public static void SetRenderTextureAntiAliasing(int level)
    {
        RenderTextureFactory.Instance.SetAntiAliasing(level);
        CameraManager.ReRenderAll();
    }
}