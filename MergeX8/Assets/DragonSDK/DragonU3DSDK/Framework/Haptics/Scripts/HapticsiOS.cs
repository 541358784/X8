// author: yunhan.zeng@dragonplus.com
// date: 2020.12.01

#if UNITY_IOS && !UNITY_EDITOR

using System.Runtime.InteropServices;
using UnityEngine.iOS;

namespace DragonPlus.Haptics
{
    public class HapticsiOS : HapticsBase
    {
	    private bool hapticsSupported;

	    [DllImport ("__Internal")] private static extern void FeedbackGeneratorsInit();
	    [DllImport ("__Internal")] private static extern void FeedbackGeneratorsRelease();
	    [DllImport ("__Internal")] private static extern void HapticSelection();
	    [DllImport ("__Internal")] private static extern void HapticSuccess();
	    [DllImport ("__Internal")] private static extern void HapticWarning();
	    [DllImport ("__Internal")] private static extern void HapticFailure();
	    [DllImport ("__Internal")] private static extern void HapticLightImpact();
	    [DllImport ("__Internal")] private static extern void HapticMediumImpact();
	    [DllImport ("__Internal")] private static extern void HapticHeavyImpact();
	    
	    public override bool Init()
	    {
		    if (!base.Init())
			    return true;
		    
		    DeviceGeneration generation = Device.generation;
			if ((generation == DeviceGeneration.iPhone3G)
			|| (generation == DeviceGeneration.iPhone3GS)
			|| (generation == DeviceGeneration.iPodTouch1Gen)
			|| (generation == DeviceGeneration.iPodTouch2Gen)
			|| (generation == DeviceGeneration.iPodTouch3Gen)
			|| (generation == DeviceGeneration.iPodTouch4Gen)
			|| (generation == DeviceGeneration.iPhone4)
			|| (generation == DeviceGeneration.iPhone4S)
			|| (generation == DeviceGeneration.iPhone5)
			|| (generation == DeviceGeneration.iPhone5C)
			|| (generation == DeviceGeneration.iPhone5S)
			|| (generation == DeviceGeneration.iPhone6)
			|| (generation == DeviceGeneration.iPhone6Plus)
			|| (generation == DeviceGeneration.iPhone6S)
			|| (generation == DeviceGeneration.iPhone6SPlus)
            || (generation == DeviceGeneration.iPhoneSE1Gen)
            || (generation == DeviceGeneration.iPad1Gen)
            || (generation == DeviceGeneration.iPad2Gen)
            || (generation == DeviceGeneration.iPad3Gen)
            || (generation == DeviceGeneration.iPad4Gen)
            || (generation == DeviceGeneration.iPad5Gen)
            || (generation == DeviceGeneration.iPadAir1)
            || (generation == DeviceGeneration.iPadAir2)
            || (generation == DeviceGeneration.iPadMini1Gen)
            || (generation == DeviceGeneration.iPadMini2Gen)
            || (generation == DeviceGeneration.iPadMini3Gen)
            || (generation == DeviceGeneration.iPadMini4Gen)
            || (generation == DeviceGeneration.iPadPro10Inch1Gen)
            || (generation == DeviceGeneration.iPadPro10Inch2Gen)
            || (generation == DeviceGeneration.iPadPro11Inch)
            || (generation == DeviceGeneration.iPadPro1Gen)
            || (generation == DeviceGeneration.iPadPro2Gen)
            || (generation == DeviceGeneration.iPadPro3Gen)
            || (generation == DeviceGeneration.iPadUnknown)
            || (generation == DeviceGeneration.iPodTouch1Gen)
            || (generation == DeviceGeneration.iPodTouch2Gen)
            || (generation == DeviceGeneration.iPodTouch3Gen)
            || (generation == DeviceGeneration.iPodTouch4Gen)
            || (generation == DeviceGeneration.iPodTouch5Gen)
            || (generation == DeviceGeneration.iPodTouch6Gen)
			|| (generation == DeviceGeneration.iPhone6SPlus))
			{
			    hapticsSupported = false;
			}
			else
			{
			    hapticsSupported = true;
			}

			FeedbackGeneratorsInit();

			Avaiable = true;
			IsHapticsSupported = hapticsSupported;
			return true;
	    }

	    public override bool Release()
	    {
		    if (!base.Release())
			    return true;
		    
			FeedbackGeneratorsRelease();
			
			Avaiable = false;

		    return true;
	    }
        
        public override void Haptics(HapticTypes type)
        {
	        if (type == HapticTypes.None)
		        return;

	        // this will trigger a standard vibration on all the iOS devices that don't support haptic feedback
	        if (!hapticsSupported)
	        {
		        Vibrate(200);
		        return;
	        }
	        
	        switch (type)
	        {
		        case HapticTypes.Selection: HapticSelection (); break;
		        case HapticTypes.Success: HapticSuccess (); break;
		        case HapticTypes.Warning: HapticWarning (); break;
		        case HapticTypes.Failure: HapticFailure (); break;
		        case HapticTypes.Light: HapticLightImpact (); break;
		        case HapticTypes.Medium: HapticMediumImpact (); break;
		        case HapticTypes.Heavy: HapticHeavyImpact (); break;
		        default: Vibrate(200); break;
	        }
        }
    }
}

#endif