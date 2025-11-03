using DragonU3DSDK;
using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour
{
    private readonly float fpsMeasuringDelta = 1.0f;
    public Text mAndroidIDText;
    public Text mDeviceIDText;
    private float mFps;
    public Text mFpsText;
    public Transform DebugInfoRoot;

    private int mFrameCount = 30;

    private float mTime = 1;

    private void Awake()
    {
        if (ConfigurationController.Instance.version != VersionStatus.RELEASE)
        {
            mAndroidIDText.text = string.Format("ANDROID_ID:{0}", DragonNativeBridge.getAndroidID());
            mDeviceIDText.text = string.Format("DEVICEID:{0}", DragonNativeBridge.getDeivceId());
        }
        else
        {
            mAndroidIDText.gameObject.SetActive(false);
            mDeviceIDText.gameObject.SetActive(false);
            mFpsText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (ConfigurationController.Instance.version == VersionStatus.RELEASE) return;

        mTime += Time.deltaTime;
        mFrameCount++;
        if (mTime > fpsMeasuringDelta)
        {
            mFps = mFrameCount / mTime;
            mFpsText.text = string.Format("FPS:{0}", mFps.ToString("f1"));
            mTime = 0;
            mFrameCount = 0;
        }

        mAndroidIDText.gameObject.SetActive(!DebugModel.Instance.CloseDebug && !DebugModel.Instance.CloseFPS);
        mDeviceIDText.gameObject.SetActive(!DebugModel.Instance.CloseDebug && !DebugModel.Instance.CloseFPS);
        mFpsText.gameObject.SetActive(!DebugModel.Instance.CloseDebug && !DebugModel.Instance.CloseFPS);
        //DebugInfoRoot.gameObject.SetActive(!DebugModel.Instance.CloseDebug && !DebugModel.Instance.CloseFPS);
    }
}