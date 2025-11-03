using System;
using AppleAuth;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;

namespace Gameplay
{
    public class AppleAccountSubSystem : GlobalSystem<AppleAccountSubSystem>
    {
        // public bool TryPopUpAppleAward()
        // {
        //     if (!AccountManager.Instance.HasBindApple())
        //         return false;
        //
        //     var storage = StorageManager.Instance.GetStorage<StorageMain>();
        //     if (!storage.CommonData.GotAppleAward)
        //     {
        //         DebugUtil.Log("CheckAppleAward:PopUp");
        //         var link_fb_reward = DataVisiter.Instance.GetGlobalStringConfig(GlobalStringConfigKey.bind_apple_reward);
        //         var rewards = ItemData.ParseList(link_fb_reward);
        //         var dlg = UIPublicRewardController.ShowAndUpdateUserData(rewards, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.BindFacebook),
        //             ()=>storage.CommonData.GotAppleAward = true
        //         );
        //         dlg.SetTitle(LocalizationManager.Instance.GetLocalizedString("UI_fbapple_bind_reward"));
        //         return true;
        //     }
        //
        //     return false;
        // }
        //
        // public bool TryAddAppleAward()
        // {
        //     try
        //     {
        //         if (!AccountManager.Instance.HasBindApple())
        //             return false;
        //
        //         var storage = StorageManager.Instance.GetStorage<StorageMain>();
        //         if (!storage.CommonData.GotAppleAward)
        //         {
        //             DebugUtil.Log("CheckAppleAward:PopUp");
        //             var link_fb_reward = DataVisiter.Instance.GetGlobalStringConfig(GlobalStringConfigKey.bind_apple_reward);
        //             var rewards = ItemData.ParseList(link_fb_reward);
        //             for (int i = 0; i < rewards.Count; i++)
        //             {
        //                 UserData.Instance.AddRes(rewards[i], new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.BindApple));
        //             }
        //             CurrencyUtils.StaticUpdate();
        //             return true;
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         DebugUtil.LogError(e);
        //     }
        //
        //     return false;
        // }

        public bool SupportAppleLogin()
        {
#if UNITY_IOS
            var support = AppleAuthManager.IsCurrentPlatformSupported;
            if(!support)
            {
                DebugUtil.Log($"Dont support apple login, device: {UnityEngine.iOS.Device.generation}");
            }
            return support;
#else
            return false;
#endif
        }
    }
//#endif
}