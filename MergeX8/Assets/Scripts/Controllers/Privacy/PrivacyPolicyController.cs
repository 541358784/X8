/*
 * @file PrivacyPolicyController
 * 隐私协议
 * @author lu
 */

using System.Collections;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class PrivacyPolicyController : UIWindow
{
    Button Button;

    public override void PrivateAwake()
    {
        if (CommonUtils.IsLE_16_10())
        {
            gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }

        Button = transform.Find("PrivacyPolicy/UI/AgreeButton").GetComponent<Button>();
        Button.onClick.AddListener(OnClickButton);
        Button privacyBtn = transform.Find("PrivacyPolicy/UI/BtnPrivacyPolicy").GetComponent<Button>();
        privacyBtn.onClick.AddListener(OnBtnPrivacy);      
        Button serviceBtn = transform.Find("PrivacyPolicy/UI/BtnTermsOfService").GetComponent<Button>();
        serviceBtn.onClick.AddListener(OnBtnService);
    }


    // 打开界面时调用(每次打开都调用)
    protected override void OnOpenWindow(params object[] objs)
    {
        isPlayDefaultAudio = false;

        //CommonUtils.TweenOpen(transform.Find("PrivacyPolicy"));
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtuePrivacyPop);
    }

    // 重新加载界面时调用
    protected override void OnReloadWindow()
    {
    }

    // 关闭界面时调用(onDestroy)
    protected override void OnCloseWindow(bool destroy = false)
    {
    }

    void OnClickButton()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtuePrivacyAccept);

        EventDispatcher.Instance.DispatchEvent(EventEnum.GDPR_ACCEPTED);

        CloseWindowWithinUIMgr(true);
    }

    public void OnBtnPrivacy()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        Application.OpenURL(ConfigurationController.Instance.PrivacyPolicyURL);
        AppIconChangerSystem.Instance.IsShowingPrivacyPollcy = true;
    }    
    
    public void OnBtnService()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        Application.OpenURL(ConfigurationController.Instance.TermsOfServiceURL);
        AppIconChangerSystem.Instance.IsShowingService = true;
    }
}