package com.dragonplus;

import android.util.Base64;
import android.util.Log;

import androidx.annotation.NonNull;

import com.applovin.mediation.MaxAd;
import com.applovin.mediation.MaxAdListener;
import com.applovin.mediation.MaxError;
import com.applovin.mediation.MaxReward;
import com.applovin.mediation.MaxRewardedAdListener;
import com.applovin.mediation.ads.MaxInterstitialAd;
import com.applovin.mediation.ads.MaxRewardedAd;
import com.applovin.mediation.unity.MaxUnityAdManager;
import com.applovin.mediation.unity.MaxUnityPlugin;
import com.unity3d.mediation.interstitial.LevelPlayInterstitialAd;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.nio.charset.StandardCharsets;
import java.util.Map;
import java.util.Objects;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.zip.GZIPInputStream;

public class MaxInfoBridge {
    private static final String _tag = "MaxBridge";
    private static String _callbackObjectName;

    private static String _rewardedVideoUnitId;
    private static String _interstitialUnitId;
    private static String _bannerUnitId;

    public static void initialize(String callbackObjectName,
                                  String rewardedVideoUnitId,
                                  String interstitialUnitId,
                                  String bannerUnitId) {
        logDebug("Initializing...");

        _callbackObjectName = callbackObjectName;
        _rewardedVideoUnitId = rewardedVideoUnitId;
        _interstitialUnitId = interstitialUnitId;
        _bannerUnitId = bannerUnitId;

        setupRewardedVideoCallback(rewardedVideoUnitId);
        setupInterstitialCallback(interstitialUnitId);
        setupBannerCallback(bannerUnitId);

        logDebug("Initialized.");
    }

    public static void disposeRewardedVideoAd(String adUnitId) {
        try {
            Method method = MaxUnityAdManager.class.getDeclaredMethod("retrieveRewardedAd", String.class);
            method.setAccessible(true);

            MaxUnityAdManager adManager = MaxUnityPlugin.getAdManager();
            MaxRewardedAd rewardedAd = (MaxRewardedAd) method.invoke(adManager, adUnitId);
            if (rewardedAd != null) {
                rewardedAd.destroy();
            }

            Field field = MaxUnityAdManager.class.getDeclaredField("rewardedAds");
            field.setAccessible(true);

            Map<String, MaxRewardedAd> rewardedAds = (Map<String, MaxRewardedAd>)field.get(adManager);
            if (rewardedAds != null) {
                rewardedAds.remove(adUnitId);
            }

            setupRewardedVideoCallback(adUnitId);
        } catch (IllegalAccessException |
                 InvocationTargetException |
                 NoSuchMethodException |
                 NoSuchFieldException e) {
            logError(e.getLocalizedMessage(), e);
        }
    }

    public static void disposeInterstitialAd(String adUnitId) {
        try {
            Method method = MaxUnityAdManager.class.getDeclaredMethod("retrieveInterstitial", String.class);
            method.setAccessible(true);

            MaxUnityAdManager adManager = MaxUnityPlugin.getAdManager();
            MaxInterstitialAd interstitialAd = (MaxInterstitialAd) method.invoke(adManager, adUnitId);

            if (interstitialAd != null) {
                interstitialAd.destroy();
            }

            Field field = MaxUnityAdManager.class.getDeclaredField("interstitials");
            field.setAccessible(true);

            Map<String, MaxInterstitialAd> interstitialAds = (Map<String, MaxInterstitialAd>)field.get(adManager);
            if (interstitialAds != null) {
                interstitialAds.remove(adUnitId);
            }

            setupInterstitialCallback(adUnitId);
        } catch (IllegalAccessException |
                 InvocationTargetException |
                 NoSuchMethodException |
                 NoSuchFieldException e) {
            logError(e.getLocalizedMessage(), e);
        }
    }

    private static class CustomRewardedAdListener implements MaxRewardedAdListener {

        private final MaxRewardedAdListener originalListener;

        public CustomRewardedAdListener(MaxRewardedAdListener originalListener) {
            this.originalListener = originalListener;
        }

        @Override
        public void onUserRewarded(@NonNull MaxAd maxAd, @NonNull MaxReward maxReward) {
            originalListener.onUserRewarded(maxAd, maxReward);
        }

        @Override
        public void onAdLoaded(@NonNull MaxAd maxAd) {
            originalListener.onAdLoaded(maxAd);

            JSONObject message = processAd(maxAd);
            if (message != null) {
                UnityPlayer.UnitySendMessage(_callbackObjectName, "OnLoaded", message.toString());
            }
        }

        @Override
        public void onAdDisplayed(@NonNull MaxAd maxAd) {
            originalListener.onAdDisplayed(maxAd);

            JSONObject message = processAd(maxAd);
            if (message != null) {
                UnityPlayer.UnitySendMessage(_callbackObjectName, "OnImpression", message.toString());
            }
        }

        @Override
        public void onAdHidden(@NonNull MaxAd maxAd) {
            originalListener.onAdHidden(maxAd);
        }

        @Override
        public void onAdClicked(@NonNull MaxAd maxAd) {
            originalListener.onAdClicked(maxAd);

            JSONObject message = processAd(maxAd);
            if (message != null) {
                UnityPlayer.UnitySendMessage(_callbackObjectName, "OnClicked", message.toString());
            }
        }

        @Override
        public void onAdLoadFailed(@NonNull String s, @NonNull MaxError maxError) {
            originalListener.onAdLoadFailed(s, maxError);
        }

        @Override
        public void onAdDisplayFailed(@NonNull MaxAd maxAd, @NonNull MaxError maxError) {
            originalListener.onAdDisplayFailed(maxAd, maxError);
        }
    }

    private static class CustomInterstitialAdListener implements MaxAdListener {
        private final MaxAdListener originalListener;

        public CustomInterstitialAdListener(MaxAdListener originalListener) {
            this.originalListener = originalListener;
        }

        @Override
        public void onAdLoaded(@NonNull MaxAd maxAd) {
            originalListener.onAdLoaded(maxAd);

            JSONObject message = processAd(maxAd);
            if (message != null) {
                UnityPlayer.UnitySendMessage(_callbackObjectName, "OnLoaded", message.toString());
            }
        }

        @Override
        public void onAdDisplayed(@NonNull MaxAd maxAd) {
            originalListener.onAdDisplayed(maxAd);

            JSONObject message = processAd(maxAd);
            if (message != null) {
                UnityPlayer.UnitySendMessage(_callbackObjectName, "OnImpression", message.toString());
            }
        }

        @Override
        public void onAdHidden(@NonNull MaxAd maxAd) {
            originalListener.onAdHidden(maxAd);
        }

        @Override
        public void onAdClicked(@NonNull MaxAd maxAd) {
            originalListener.onAdClicked(maxAd);

            JSONObject message = processAd(maxAd);
            if (message != null) {
                UnityPlayer.UnitySendMessage(_callbackObjectName, "OnClicked", message.toString());
            }
        }

        @Override
        public void onAdLoadFailed(@NonNull String s, @NonNull MaxError maxError) {
            originalListener.onAdLoadFailed(s, maxError);
        }

        @Override
        public void onAdDisplayFailed(@NonNull MaxAd maxAd, @NonNull MaxError maxError) {
            originalListener.onAdDisplayFailed(maxAd, maxError);
        }
    }

    private static void setupRewardedVideoCallback(String rewardedVideoUnitId) {
        try {
            Method method = MaxUnityAdManager.class.getDeclaredMethod("retrieveRewardedAd", String.class);
            method.setAccessible(true);

            MaxUnityAdManager adManager = MaxUnityPlugin.getAdManager();
            MaxRewardedAd rewardedAd = (MaxRewardedAd) method.invoke(adManager, rewardedVideoUnitId);

            Field aField = MaxRewardedAd.class.getDeclaredField("a");
            aField.setAccessible(true);

            Field field = com.applovin.impl.mediation.ads.a.class.getDeclaredField("adListener");
            field.setAccessible(true);

            Object a = aField.get(rewardedAd);

            MaxRewardedAdListener originalListener = (MaxRewardedAdListener) field.get(a);
            if (originalListener == null) {
                logDebug("Can't find ads listener");
                return;
            }

            field.set(a, new CustomRewardedAdListener(originalListener));

        } catch (NoSuchMethodException | IllegalAccessException | InvocationTargetException |
                 NoSuchFieldException e) {
            logError(e.getLocalizedMessage(), e);
        }
    }

    private static void setupInterstitialCallback(String interstitialUnitId) {
        try {
            Method method = MaxUnityAdManager.class.getDeclaredMethod("retrieveInterstitial", String.class);
            method.setAccessible(true);

            MaxUnityAdManager adManager = MaxUnityPlugin.getAdManager();
            MaxInterstitialAd interstitialAd = (MaxInterstitialAd) method.invoke(adManager, interstitialUnitId);

            Field aField = MaxInterstitialAd.class.getDeclaredField("a");
            aField.setAccessible(true);

            Field field = com.applovin.impl.mediation.ads.a.class.getDeclaredField("adListener");
            field.setAccessible(true);

            Object a = aField.get(interstitialAd);

            MaxAdListener originalListener = (MaxAdListener) field.get(a);
            if (originalListener == null) {
                logDebug("Can't find ads listener");
                return;
            }

            field.set(a, new CustomInterstitialAdListener(originalListener));

        } catch (NoSuchMethodException | IllegalAccessException | InvocationTargetException |
                 NoSuchFieldException e) {
            logError(e.getLocalizedMessage(), e);
        }
    }

    private static void setupBannerCallback(String bannerUnitId) {
    }

    private static Field findField(Class<?> clazz, String fieldName) {
        Field targetField = null;
        while (targetField == null && clazz != null) {
            try {
                targetField = clazz.getDeclaredField(fieldName);
            } catch (NoSuchFieldException e) {
                clazz = clazz.getSuperclass();

            }
        }

        return targetField;
    }

    private static JSONObject processAd(MaxAd maxAd) {
        try {
            Field field = findField(maxAd.getClass(), "d");
            if (field == null) {
                logDebug("No target field found.");
                return null;
            }

            field.setAccessible(true);
            JSONObject selectedAds = (JSONObject) field.get(maxAd);
            if (selectedAds == null) {
                logDebug("No ads found.");
                return null;
            }

            String networkName = maxAd.getNetworkName();
            String networkPlacement = maxAd.getNetworkPlacement();
            String clickUrl = "";

            if (networkPlacement == null) {
                networkPlacement = "";
            }

            if (!selectedAds.has("bid_response")) {
                logDebug("Not supported network: " + networkName);
            } else {
                String bidResponse = selectedAds.getString("bid_response");
                clickUrl = findClickUrl(networkName, networkPlacement, bidResponse);

                if (clickUrl == null) {
                    clickUrl = "";
                }
            }

            String creativeId =  maxAd.getCreativeId();
            if (creativeId == null) {
                creativeId = "";
            }

            String dspId = maxAd.getDspId();
            String dspName = maxAd.getDspName();
            if (dspId == null) {
                dspId = "";
            }

            if (dspName == null) {
                dspName = "";
            }

            JSONObject result = new JSONObject();
            result.put("name", "MAX");
            result.put("ad_unit_type", maxAd.getFormat().getDisplayName());
            result.put("ad_unit_id", maxAd.getAdUnitId());
            result.put("ecpm", String.valueOf(maxAd.getRevenue()));
            result.put("click_url", clickUrl.replace("\\/", "/"));
            result.put("ecpm_precision", maxAd.getRevenuePrecision());
            result.put("creative_id", creativeId);
            result.put("network_name", networkName);
            result.put("network_placement", networkPlacement);
            result.put("dsp_id", dspId);
            result.put("dsp_name", dspName);

            return result;
        } catch (IllegalAccessException | JSONException e) {
            logError(e.getLocalizedMessage(), e);

            return null;
        }
    }

    private static String findClickUrl(String networkName, String networkPlacement, String bidResponse) {
        if (networkName.equals("AppLovin")) {
            return findApplovinClickUrl(bidResponse);
        } else if (networkName.equals("APPLOVIN_EXCHANGE")) {
            return findApplovinExchangeClickUrl(bidResponse);
        } else if (networkName.equals("Unity Ads")) {
            return findUnityAdsClickUrl(networkPlacement, bidResponse);
        } else if (networkName.equals("Liftoff Monetize")) {
            return findVungleAdsClickUrl(networkPlacement, bidResponse);
//        } else if (networkName.equals("Pangle")) {
//            return findPangleClickUrl(bidResponse);
        } else {
            logDebug("Not supported network " + networkName + ": " + bidResponse);
            return "";
        }
    }

    private static String findApplovinClickUrl(String bidResponse) {
        try {
            String base64encoded = bidResponse.substring("json_v3!".length());
            String jsonString = new String(Base64.decode(base64encoded, Base64.DEFAULT));
            JSONObject data = new JSONObject(jsonString);
            JSONArray realAds = data.getJSONArray("ads");
            JSONObject singleAds = realAds.getJSONObject(0);
            return singleAds.getString("click_url");
        } catch (JSONException e) {
            logError(e.getLocalizedMessage(), e);
            return "";
        }
    }

    private static final Pattern applovinExchangeClickUrlPattern = Pattern.compile("<ClickThrough><!\\[CDATA\\[(https://play\\.google\\.com/store/apps/details.*)]]></ClickThrough>");

    private static String findApplovinExchangeClickUrl(String bidResponse) {
        try {
            String base64encoded = bidResponse.substring("json_v3!".length());
            String jsonString = new String(Base64.decode(base64encoded, Base64.DEFAULT));
            JSONObject data = new JSONObject(jsonString);
            JSONArray realAds = data.getJSONArray("ads");
            JSONObject singleAds = realAds.getJSONObject(0);
            Matcher matcher = applovinExchangeClickUrlPattern.matcher(
                    singleAds.toString().replace("\\/", "/"));
            if (matcher.find()) {
                return Objects.requireNonNull(matcher.group(0))
                        .replace("<ClickThrough><![CDATA[", "")
                        .replace("]]></ClickThrough>", "");
            }
            return "";
        } catch (JSONException e) {
            logError(e.getLocalizedMessage(), e);
            return "";
        }
    }

    private static String findUnityAdsClickUrl(String networkPlacement, String bidResponse) {
        try {
            String jsonString = new String(Base64.decode(bidResponse, Base64.DEFAULT));
            JSONObject data = new JSONObject(jsonString);
            if (!data.has("placementsV2")) {
                return "";
            }

            JSONObject placementsV2 = data.getJSONObject("placementsV2");
            if (!placementsV2.has(networkPlacement)) {
                return "";
            }

            JSONObject placement = placementsV2.getJSONObject(networkPlacement);
            if (!placement.has("mediaId")) {
                return "";
            }

            if (!data.has("media")) {
                return "";
            }

            JSONObject medias = data.getJSONObject("media");

            JSONArray mediaIds = placement.getJSONArray("mediaId");
            for (int i = 0; i < mediaIds.length(); i++) {
                String mediaId = mediaIds.getString(i);

                if (medias.has(mediaId)) {
                    JSONObject media = medias.getJSONObject(mediaId);
                    if (media.has("bundleId")) {
                        String bundleId = media.getString("bundleId");
                        return String.format("https://play.google.com/store/apps/details?id=%s", bundleId);
                    }
                }
            }

            return "";
        } catch (JSONException e) {
            logError(e.getLocalizedMessage(), e);
            return "";
        }
    }

    private static String findVungleAdsClickUrl(String networkPlacement, String bidResponse) {
        try {
            JSONObject data = new JSONObject(bidResponse);
            if (!data.has("adunit")) {
                return "";
            }

            String adUnit = data.getString("adunit");
            byte[] decoded = Base64.decode(adUnit, Base64.DEFAULT);
            try (ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(decoded)) {
                try (GZIPInputStream gzipInputStream = new GZIPInputStream(byteArrayInputStream)) {
                    try (ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream()) {
                        int read = 0;
                        byte[] buffer = new byte[1024];
                        while (read >= 0) {
                            read = gzipInputStream.read(buffer, 0, buffer.length);
                            if (read > 0) {
                                byteArrayOutputStream.write(buffer, 0, read);
                            }
                        }
                        byte[] uncompressed = byteArrayOutputStream.toByteArray();
                        String uncompressedJsonString = new String(uncompressed, StandardCharsets.UTF_8);

                        JSONObject adInfo = new JSONObject(uncompressedJsonString);
                        if (!adInfo.has("ads")) {
                            return "";
                        }

                        JSONArray adsList = adInfo.getJSONArray("ads");
                        for (int i = 0; i < adsList.length(); i++) {
                            JSONObject ads = adsList.getJSONObject(i);
                            if (!ads.has("ad_markup")) {
                                continue;
                            }

                            JSONObject adMarkup = ads.getJSONObject("ad_markup");
                            if (!adMarkup.has("templateSettings")) {
                                continue;
                            }

                            JSONObject templateSettings = adMarkup.getJSONObject("templateSettings");
                            if (!templateSettings.has("normal_replacements")) {
                                continue;
                            }

                            JSONObject normalReplacements = templateSettings.getJSONObject("normal_replacements");
                            if (normalReplacements.has("EC_CTA_URL")) {
                                return normalReplacements.getString("EC_CTA_URL");
                            }

                            if (normalReplacements.has("CTA_BUTTON_URL")) {
                                return normalReplacements.getString("CTA_BUTTON_URL");
                            }
                        }
                    }
                }
            }

            return "";
        } catch (JSONException | IOException e) {
            logError(e.getLocalizedMessage(), e);
            return "";
        }
    }

    // TODO check pangle later
//    private static String findPangleClickUrl(String bidResponse) {
//        try {
//            JSONObject response = new JSONObject(bidResponse);
//            int cypher = response.getInt("cypher");
//            String message = response.getString("message");
//
//            String decoded;
//            if (cypher == 3) {
//                decoded = com.bytedance.sdk.component.utils.YFl.tN(message);
//            } else {
//                Pair<Integer, String> result = PangleEncryptManager.decryptType4(message);
//                decoded = Objects.requireNonNullElse(result.second, "");
//            }
//
//            JSONObject data = new JSONObject(decoded);
//            JSONArray creatives = data.getJSONArray("creatives");
//            JSONObject creative = creatives.getJSONObject(0);
//            return creative.getString("target_url");
//        } catch (JSONException e) {
//            // Keep quiet
//            logError(e.getLocalizedMessage(), e);
//            return "";
//        }
//    }

    private static void logDebug(String message) {
        Log.d(_tag, message);
    }

    private static void logError(String message, Exception exception) {
        Log.e(_tag, message, exception);
    }
}
