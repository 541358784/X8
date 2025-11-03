using Dlugin;
using Google.Android.PerformanceTuner;
using UnityEngine;

public class APTPlugin : IServiceProvider
{
    private bool init;
    private AndroidPerformanceTuner<FidelityParams, Annotation> tuner = new AndroidPerformanceTuner<FidelityParams, Annotation>();
    public AndroidPerformanceTuner<FidelityParams, Annotation> Tuner { get { return tuner; } }

    public override void DisposePlugin(string pluginName)
    {
        throw new System.NotImplementedException();
    }
    
    public void Initialize()
    {
        if (init) return;
        init = true;
        ErrorCode startErrorCode = tuner.Start();
        Debug.Log("[APTPlugin] Android Performance Tuner started with code: " + startErrorCode);

        tuner.onReceiveUploadLog += request =>
        {
            Debug.Log("[APTPlugin] Telemetry uploaded with request name: " + request.name);
        };
    }
}