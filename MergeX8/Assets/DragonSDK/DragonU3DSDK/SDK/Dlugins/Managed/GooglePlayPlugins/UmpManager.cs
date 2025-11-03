/*
 * https://alidocs.dingtalk.com/i/nodes/N7dx2rn0Jby3xxGECjjEoB3jVMGjLRb3?utm_scene=person_space
*/

#if GOOGLE_UMP_ENABLE
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;
using System;
using UnityEngine;
//using System.Net.NetworkInformation;
using System.Collections;
using System.Runtime.InteropServices;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.API;
using System.Text.RegularExpressions;
using DragonU3DSDK;
using GoogleMobileAds.Api;

public class UmpManager : Manager<UmpManager>
{
    public void Init()
    {
#if DEBUG || DEVELOPMENT_BUILD
        var testDevices = new List<string>();
        var testDevice = Resources.Load<TextAsset>("UMP_TEST_DEVICE");
        if (testDevice != null)
        {
            if (!string.IsNullOrEmpty(testDevice.text))
            {
                var devices = testDevice.text.Split('\n');
                testDevices.AddRange(devices);
            }
        }

        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = testDevices
        };
#endif

        // Set tag for under age of consent.
        // Here false means users are not under age.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
#if DEBUG || DEVELOPMENT_BUILD
            ConsentDebugSettings = debugSettings,
#endif
        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    public void Reset()
    {
        ConsentInformation.Reset();
    }

    void OnConsentInfoUpdated(FormError error)
    {
        Log("OnConsentInfoUpdated");
        if (error != null)
        {
            // Handle the error.
            LogError(error.Message);
            LogError(error.ErrorCode.ToString());

#if UNITY_IOS && !UNITY_EDITOR
        Dlugin.iOSATTManager.Instance.AutoTracking();
#endif

            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadForm();
        }
        else
        {
#if UNITY_IOS && !UNITY_EDITOR
        Dlugin.iOSATTManager.Instance.AutoTracking();
#endif

            Log("OnConsentInfoUpdated not available");
        }
    }

    private ConsentForm _consentForm;

    void LoadForm()
    {
        Log("LoadForm");

        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        Log("OnLoadConsentForm");

        if (error != null)
        {
            // Handle the error.
            LogError(error.Message);
            LogError(error.ErrorCode.ToString());

#if UNITY_IOS && !UNITY_EDITOR
        Dlugin.iOSATTManager.Instance.AutoTracking();
#endif

            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
        }
        else
        {
#if UNITY_IOS && !UNITY_EDITOR
        Dlugin.iOSATTManager.Instance.AutoTracking();
#endif

            Log("OnConsentInfoUpdated " + ConsentInformation.ConsentStatus);
        }
    }

    void OnShowForm(FormError error)
    {
        Log("OnLoadConsentForm");

#if UNITY_IOS && !UNITY_EDITOR
                Dlugin.iOSATTManager.Instance.AutoTracking();
#endif

        if (error != null)
        {
            // Handle the error.
            LogError(error.Message);
            LogError(error.ErrorCode.ToString());
            return;
        }

        // Handle dismissal by reloading form.
        LoadForm();
    }

    private void Log(string s)
    {
        DebugUtil.Log($"UMP: {s}");
    }

    private void LogError(string s)
    {
        DebugUtil.LogError($"UMP: {s}");
    }
}
#endif