using Dlugin;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShowNetwork : MonoBehaviour
{
    public Text mNetworkText;
    public Transform mScrollView;

    private float timer;

    // Use this for initialization
    private void Awake()
    {
        if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
        {
            mNetworkText.gameObject.SetActive(false);
            mScrollView.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (CommonUtils.IsWideScreenDevice())
            mNetworkText.transform.localPosition = new Vector3(100, mNetworkText.transform.localPosition.y,
                mNetworkText.transform.localPosition.z);
    }

    private void updateUI()
    {
        var text = "" +
                   "PLAYERID: " + StorageManager.Instance.GetStorage<StorageCommon>().PlayerId + "\n" +
                   "NETWORK: " + APIManager.Instance.HasNetwork + "\n" +
                   "LOGIN: " + AccountManager.Instance.loginStatus.ToString() + "\n" +
                   "LOCAL_VER: " + StorageManager.Instance.LocalVersion + "\n" +
                   "REMOTE_ACK: " + StorageManager.Instance.RemoteVersionACK + "\n" +
                   "REMOTE_LOCAL: " + StorageManager.Instance.RemoteVersionSYN + "\n" +
                   "已拥有商品: " + SDK.GetInstance().iapManager.GetOwnedProductsFormatString() + "\n" +
                   "奖励视频: " + SDK.GetInstance().m_AdsManager.RewardVideoStatus() + "\n" +
                   "插屏广告情况: " + SDK.GetInstance().m_AdsManager.InterstitialStatus() + "\n" +
                   "广告位曝光次数: " + AdLogicManager.Instance.ValidShouleTotalCnt.ToString() + "\n" +
                   "versionName: " + DragonNativeBridge.GetVersionName() + "\n" +
                   "versionCode: " + DragonNativeBridge.GetVersionCode() + "\n" +
                   "group: " + ChangeableConfig.Instance.GetRemoteStringValue("group") + "\n";


        // if (SceneFsm.mInstance.IsInGame())
        // {
        //     var gameUI = UIManager.Instance.GetOpenUI<GameUIController>("Common/GameUI");
        //     if (gameUI != null)
        //     {
        //         text += "\n";
        //         text += gameUI.GetCollectionCustomersForDebug();
        //     }
        // }

        mNetworkText.text = text;
    }


    // Update is called once per frame
    //private void Update()
    //{
    //    if (ConfigurationController.Instance.version == VersionStatus.RELEASE) return;

    //    timer += Time.deltaTime;
    //    if (timer > 0.5)
    //    {
    //        updateUI();
    //        timer = 0;
    //    }


    //mNetworkText.gameObject.SetActive(!DebugModel.Instance.CloseDebug && !DebugModel.Instance.CloseFPS);
    //mScrollView.gameObject.SetActive(!DebugModel.Instance.CloseDebug && !DebugModel.Instance.CloseFPS);
    //}
}