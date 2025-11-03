#if USE_TAPJOY_SDK

using System;
using System.Collections;
using System.Collections.Generic;
using Dlugin;
using Dlugin.PluginStructs;
using DragonU3DSDK;
using TapjoyUnity;
using TapjoyUnity.Internal;
using UnityEngine;

public class TapjoyPlugin : IAdsProvider
{
	private bool checkConnect = false;
	private const float CACHE_CD_Reconnect = 5;
	private float m_TimeLeft_Reconnect = 0;
    
	private string placementName;
	private TJPlacement m_Placement;
	
	private bool requesting;
	private Action<bool> show_pre_callback;
	private Action<bool> show_post_callback;

	private int m_Currency = 0;
	
	private const float CACHE_CD_AutoRequest = 1;
	private float m_TimeLeft_AutoRequest = 0;
	
    public override bool DisposeAd(AdsUnitDefine unit)
    {
        throw new System.NotImplementedException();
    }

    public override void DisposePlugin(string pluginName)
    {
        throw new System.NotImplementedException();
    }

    public override void Initialize()
    {
        TapjoyConfigInfo m_ConfigInfo = PluginsInfoManager.Instance.GetPluginConfig<TapjoyConfigInfo>(Constants.Tapjoy);

        if (m_ConfigInfo == null)
        {
            DebugUtil.LogError("Tapjoy config is null. please check SDKEditor");
            return;
        }

#if UNITY_IOS
        m_PluginDefine.m_PluginParam = m_ConfigInfo.iOSSDKKey;
#elif UNITY_ANDROID
        m_PluginDefine.m_PluginParam = m_ConfigInfo.AndroidSDKKey;
#endif
        m_PluginDefine.m_PluginName = Constants.Tapjoy;
        m_PluginDefine.m_PluginVersion = "1.0";

#if UNITY_IOS
        placementName = m_ConfigInfo.iOSOfferWallPlacements[0].key;
#elif UNITY_ANDROID
        placementName = m_ConfigInfo.AndroidOfferWallPlacements[0].key;
#endif
        
        // Connect Delegates
        Tapjoy.OnConnectSuccess += HandleConnectSuccess;
        Tapjoy.OnConnectFailure += HandleConnectFailure;
	    
        // Placement Delegates
        TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
        TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
        TJPlacement.OnContentReady += HandlePlacementContentReady;
        TJPlacement.OnContentShow += HandlePlacementContentShow;
        TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
        TJPlacement.OnClick += HandlePlacementOnClick;
        TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
        TJPlacement.OnRewardRequest += HandleOnRewardRequest;
	    
        // Currency Delegates
        Tapjoy.OnAwardCurrencyResponse += HandleAwardCurrencyResponse;
        Tapjoy.OnAwardCurrencyResponseFailure += HandleAwardCurrencyResponseFailure;
        Tapjoy.OnSpendCurrencyResponse += HandleSpendCurrencyResponse;
        Tapjoy.OnSpendCurrencyResponseFailure += HandleSpendCurrencyResponseFailure;
        Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
        Tapjoy.OnGetCurrencyBalanceResponseFailure += HandleGetCurrencyBalanceResponseFailure;
        
#if DEBUG
        Tapjoy.SetDebugEnabled(true);
#endif  
        
        Tapjoy.Connect(m_PluginDefine.m_PluginParam);    
        
        TimerManager.Instance.AddDelegate(Update);
    }

    void Update(float delta)
    {
        Reconnect(delta);

        AutoRequest(delta);
    }
    
    #region connect
    
    
    private void Reconnect(float delta)
    {
        if (checkConnect)
        {
            if (!Tapjoy.IsConnected)
            {
                m_TimeLeft_Reconnect -= delta;
                if (m_TimeLeft_Reconnect <= 0.0f)
                {
                    checkConnect = false;
                    Tapjoy.Connect();
                    
                    Debug.Log("[Tapjoy][offerwall]: Handle Re Connect");
                }
            }
        }
    }

    public void HandleConnectSuccess() {
        Debug.Log("[Tapjoy][offerwall]: Handle Connect Success");
        checkConnect = true;
        m_TimeLeft_Reconnect = CACHE_CD_Reconnect;
        
        m_Placement = TJPlacement.CreatePlacement(placementName);
    }
	
    public void HandleConnectFailure() {
        Debug.Log("[Tapjoy][offerwall]: Handle Connect Failure");
        checkConnect = true;
        m_TimeLeft_Reconnect = CACHE_CD_Reconnect;
    }
    
    #endregion

    #region Placement Delegate Handlers
    
    //Request成功后回调；IsContentAvailable为true
	public void HandlePlacementRequestSuccess(TJPlacement placement)
	{
		Debug.Log("[Tapjoy][offerwall]: HandlePlacementRequestSuccess");
		
		if (requesting)
		{
			requesting = false;
			
			show_pre_callback?.Invoke(true);
			show_pre_callback = null;
		
			m_Placement.ShowContent();
			
			Debug.Log("[Tapjoy][offerwall] ShowContent in HandlePlacementRequestSuccess");
		}
	}

	//Request失败后回调
	public void HandlePlacementRequestFailure(TJPlacement placement, string error)
	{
		Debug.Log("[Tapjoy][offerwall]: Request for " + placement.GetName() + " has failed because: " + error);

		if (requesting)
		{
			requesting = false;
			
			show_pre_callback?.Invoke(false);
			show_pre_callback = null;
		}
	}
	
	//广告位准备好后回调，一般会紧跟在HandlePlacementRequestSuccess后面；IsContentReady为true
	public void HandlePlacementContentReady(TJPlacement placement)
	{
		Debug.Log("[Tapjoy][offerwall]: HandlePlacementContentReady");
	}

	//显示广告位的时候回调-android是阻塞的，在关掉广告位的时候unity才会响应
	public void HandlePlacementContentShow(TJPlacement placement)
	{
		Debug.Log("[Tapjoy][offerwall]: HandlePlacementContentShow");
	}

	//关掉广告位的时候回调
	public void HandlePlacementContentDismiss(TJPlacement placement)
	{
		Debug.Log("[Tapjoy][offerwall]: HandlePlacementContentDismiss");
		
		show_post_callback?.Invoke(true);
		show_post_callback = null;
		
		/*
		DelayActionManager.Instance.DebounceInMainThread("Tapjoy_Delay_GetCurrencyBalance", 1000, () =>
		{
			Tapjoy.GetCurrencyBalance();
		});
		*/
	}

	public void HandlePlacementOnClick(TJPlacement placement)
	{
		Debug.Log("[Tapjoy][offerwall]: HandlePlacementOnClick");
	}
	
	void HandleOnPurchaseRequest (TJPlacement placement, TJActionRequest request, string productId)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleOnPurchaseRequest");
		//request.Completed();
	}

	void HandleOnRewardRequest (TJPlacement placement, TJActionRequest request, string itemId, int quantity)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleOnRewardRequest");
		//request.Completed();
	}

#endregion

	#region Currency Delegate Handlers

	public void HandleAwardCurrencyResponse(string currencyName, int balance)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleAwardCurrencySucceeded: currencyName: " + currencyName + ", balance: " + balance);
	}

	public void HandleAwardCurrencyResponseFailure(string error)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleAwardCurrencyResponseFailure: " + error);
	}

	public void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleGetCurrencyBalanceResponse: currencyName: " + currencyName + ", balance: " + balance);
		
		m_Currency = balance;
		
		if (m_Currency > 0)
		{
			Tapjoy.SpendCurrency(m_Currency);
		}
		
		//DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.AdsOnCurrencyRefresh>().Data(m_PluginDefine.m_PluginName, m_Currency).Trigger();
	}

	public void HandleGetCurrencyBalanceResponseFailure(string error)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleGetCurrencyBalanceResponseFailure: " + error);
	}

	public void HandleSpendCurrencyResponse(string currencyName, int balance)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleSpendCurrencyResponse: currencyName: " + currencyName + ", balance: " + balance);

		int value = m_Currency - balance;
		m_Currency = balance;
		DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.AdsOnCurrencySpend>().Data(m_PluginDefine.m_PluginName, value).Trigger();
	}

	public void HandleSpendCurrencyResponseFailure(string error)
	{
		Debug.Log("[Tapjoy][offerwall]: HandleSpendCurrencyResponseFailure: " + error);
	}

	#endregion	
    
    public override bool IsAdsReady(AdsUnitDefine unit)
    {
        return m_Placement != null;
    }

    public override List<AdsUnitDefine> PreloadAds()
    {
	    return null;
    }

    private void AutoRequest(float delta)
    {
	    if (Tapjoy.IsConnected)
	    {
		    if (IsAdsReady(null))
		    {
			    if (!m_Placement.IsContentAvailable())
			    {
				    m_TimeLeft_AutoRequest -= delta;
				    if (m_TimeLeft_AutoRequest <= 0)
				    {
					    m_TimeLeft_AutoRequest = CACHE_CD_AutoRequest;
						    
					    m_Placement.RequestContent();
				    }
			    }
		    }
	    }
    }
    
    public override bool PlayAds(AdsUnitDefine unit, string placement = null)
    {
	    if (requesting)
	    {
		    return true;
	    }

	    requesting = true;

	    show_pre_callback = unit.show_pre_callback;
	    show_post_callback = unit.show_post_callback;

	    if (Tapjoy.IsConnected)
	    {
		    Debug.Log("[Tapjoy][offerwall] Is Connected in PlayAds");
		    
		    if (m_Placement.IsContentAvailable())
		    {
			    requesting = false;
		    
			    show_pre_callback?.Invoke(true);
			    show_pre_callback = null;
		    
			    m_Placement.ShowContent();
		    
			    Debug.Log("[Tapjoy][offerwall] ShowContent in PlayAds");
		    }
		    else
		    {
			    DelayActionManager.Instance.DebounceInMainThread("Tapjoy_Delay_PlayAds_1", unit.m_wait_request_millisecond, () =>
			    {
				    if (requesting)
				    {
					    requesting = false;
				    
					    show_pre_callback?.Invoke(false);
					    show_pre_callback = null;
				    }
			    });
		    }
	    }
	    else
	    {
		    Debug.Log("[Tapjoy][offerwall] Is not Connected in PlayAds");
		    
		    DelayActionManager.Instance.DebounceInMainThread("Tapjoy_Delay_PlayAds_2", unit.m_wait_request_millisecond, () =>
		    {
			    if (requesting)
			    {
				    requesting = false;
				    
				    show_pre_callback?.Invoke(false);
				    show_pre_callback = null;
			    }
		    });
	    }
	    
        return true;
    }
    
    public override void SpendCurrency()
    {
	    Debug.Log("[Tapjoy][offerwall]: SpendCurrency");

	    Tapjoy.GetCurrencyBalance();
    }
    
    public override void SetMuted(bool muted)
    {
	    throw new System.NotImplementedException();
    }

    public override bool IsMuted()
    {
	    throw new System.NotImplementedException();
    }
    
    public override void ShowBanner() { }
    public override void HideBanner() { }
    public override void ShowMRec() { }
    public override void HideMRec() { }
    public override void UpdateBannerPosition(float x, float y) { }
    public override void UpdateMRECPosition(float x, float y) { }
    public override void SetBannerWidth(float width) { }
    public override void SetTestDeviceAdvertisingIdentifiers(string adid) { }
    public override void HandleLoad(AD_Type type) { }
    public override double GetLoadedAdRevenue(AD_Type type) { return 0; }
}

#endif