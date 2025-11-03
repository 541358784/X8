using System;
using System.Collections;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.Networking;

namespace Dlugin
{
    public class iOSATTManager : Manager<iOSATTManager>, IATTResponsor
    {
        private const string KEY_ATT_REQUEST_ALREADY = "ATTREQUESTALREADY";
        private const string KEY_ATT_FORBID = "ATTFORBIDKeyVersion2";
        private const string KEY_ATT_STATE_VALUE = "ATTSTATEVALUE";
        private const string ATT_STATE_URL = "http://res.dragonplus.com/att.version";

        public void AutoTracking()
        {
            if (PlayerPrefs.HasKey(KEY_ATT_FORBID) && PlayerPrefs.GetInt(KEY_ATT_FORBID).Equals(1))
            {
                DebugUtil.Log("[iOSATTManager] alrady forbid.");
                return;
            }
            
            if (GetNeedTrackingAuthorizationType() != NeedTrackingType.NEED)
            {
                return;
            }

            //if (PlayerPrefs.HasKey(KEY_ATT_STATE_VALUE) && PlayerPrefs.GetString(KEY_ATT_STATE_VALUE).Equals("1"))
            {
                DragonNativeBridge.RequestTrackingAuthorization("");
            }
            //else
            //{
            //    StartCoroutine(RequestATTState());
            //}
        }

        //state ：true-禁止；false-不禁止
        public void SetForbidAutoModeState(bool state)
        {
            PlayerPrefs.SetInt(KEY_ATT_FORBID, state ? 1 : 0);
        }
        
        private IEnumerator RequestATTState() 
        {
            Debug.Log($"[iOSATTManager] Star Request {ATT_STATE_URL}");
            
            UnityWebRequest www = new UnityWebRequest(ATT_STATE_URL);
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
 
            if(www.isNetworkError || www.isHttpError)
            {
                DebugUtil.LogError($"[iOSATTManager] Error : {www.error}");
            }
            else
            {
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    PlayerPrefs.SetString(KEY_ATT_STATE_VALUE, www.downloadHandler.text);
                    
                    DebugUtil.Log($"[iOSATTManager] State Value : {www.downloadHandler.text}");
                }
                else
                {
                    DebugUtil.LogError($"[iOSATTManager] text is null");
                }
            }
        }

        public enum NeedTrackingType
        {
            NULL,    //系统小于14.5的情况，不需要进行ATT授权请求
            NEED,    //需要进行ATT授权请求
            ALREADY, //已经进行过ATT授权请求
        }
        
        public NeedTrackingType GetNeedTrackingAuthorizationType()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (!DragonNativeBridge.IOSSAvailableFourteenFive())
            {
                return NeedTrackingType.NULL;
            }

            if (DragonNativeBridge.ATTStatus() == 0)
            {
                return NeedTrackingType.NEED;
            }

            if (DragonNativeBridge.ATTStatus() == 3)
            {
                return NeedTrackingType.ALREADY;
            }

            if (PlayerPrefs.HasKey(KEY_ATT_REQUEST_ALREADY))
            {
                return NeedTrackingType.ALREADY;
            }
            else
            {
                return NeedTrackingType.NEED;
            }
#endif
            return NeedTrackingType.NULL;
        }

        public void OnATTAccepted(string message)
        {
            PlayerPrefs.SetString(KEY_ATT_REQUEST_ALREADY, "1");
            
            DebugUtil.Log("[iOSATTManager] OnATTAccepted");
        }

        public void OnATTRefused(string message)
        {
            PlayerPrefs.SetString(KEY_ATT_REQUEST_ALREADY, "1");
            
            DebugUtil.Log("[iOSATTManager] OnATTRefused");
        }
    }
}