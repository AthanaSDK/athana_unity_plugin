using System;
using System.Collections.Generic;
using UnityEngine;
using Athana;
using Athana.Api;
using Athana.Callbacks;

#if UNITY_ANDROID

#nullable enable
public class AthanaAndroid : AthanaInterface
{

    private static AndroidJavaClass AthanaUnityPluginClass;

    private static readonly SdkCallbackProxy SdkCallback = new SdkCallbackProxy();
    private static readonly AdEventListener AdListener = new AdEventListener();

    private static bool _isInitialized = false;

    public static void Initialize(
        long appId,
        string appKey,
        string appSecret,
        AthanaServiceConfig? serviceConfig = null,
        bool testMode = false,
        bool debug = false)
    {
        if (_isInitialized)
        {
            AthanaLogger.D("Athana is initialized");
            return;
        }
        DebugMode = debug;

        AthanaUnityPluginClass = new AndroidJavaClass("com.inonesdk.athana.unity.AthanaUnityPlugin");
        try
        {
            AthanaUnityPluginClass.CallStatic("setSdkCallback", SdkCallback);
        }
        catch (Exception e)
        {
            AthanaLogger.W("Failed to set SdkCallback");
            AthanaLogger.LogException(e);
        }

        string? accountConfigJSON = null;
        string? adConfigJSON = null;
        string? conversionConfigJSON = null;

        if (serviceConfig != null)
        {
            AthanaLogger.D("serviceConfig: " + JsonUtility.ToJson(serviceConfig));

            var accountConfig = serviceConfig.AccountConfig;
            accountConfigJSON = accountConfig == null ? null : JsonUtility.ToJson(accountConfig);

            var adConfig = serviceConfig.AdServiceConfigs;
            adConfigJSON = adConfig == null ? null : JsonUtility.ToJson(adConfig);

            var conversionConfig = serviceConfig.ConversionServiceConfigs;
            conversionConfigJSON = conversionConfig == null ? null : JsonUtility.ToJson(conversionConfig);
        }


        try
        {
            AthanaUnityPluginClass.CallStatic(
                "init",
                appId, appKey, appSecret, testMode, debug,
                accountConfigJSON, adConfigJSON, conversionConfigJSON);
            _isInitialized = true;

            AthanaUnityPluginClass.CallStatic("setAdListener", AdListener);
        }
        catch (Exception e)
        {
            AthanaLogger.W("Failed to initialize Athana");
            AthanaLogger.LogException(e);
        }
    }

    public static void Start(bool privacyGrant = false)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }
        try
        {
            AthanaUnityPluginClass.CallStatic("start", privacyGrant);
        }
        catch (Exception e)
        {
            AthanaLogger.W("Failed to start Athana");
            AthanaLogger.LogException(e);
        }
    }

    public static void CurrentUser()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }
        AthanaUnityPluginClass.CallStatic("currentUser");
    }

    public static void RegistryUser(
        AthanaInterface.SignInType signInType = AthanaInterface.SignInType.ANONYMOUS,
        string? ua = null,
        string? deviceId = null,
        long? customUserId = null,
        Dictionary<string, object>? extra = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }
        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        try
        {
            AthanaUnityPluginClass.CallStatic("registryUser", (int)signInType, ua, deviceId, customUserId, extraMap);
        }
        catch (Exception e)
        {
            AthanaLogger.W("Failed to call registryUser");
            AthanaLogger.LogException(e);
        }
        extraMap?.Dispose();
    }

    public static void SignIn(
        AthanaInterface.SignInType signInType = AthanaInterface.SignInType.ANONYMOUS,
        string? ua = null,
        string? deviceId = null,
        long? customUserId = null,
        Dictionary<string, object>? extra = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("signIn", (int)signInType, ua, deviceId, customUserId, extraMap);

        extraMap?.Dispose();
    }

    public static void SignInWithUI(
        List<AthanaInterface.SignInType>? enabledSignInTypes = null,
        long? customUserId = null,
        string? privacyPolicyUrl = null,
        string? termsOfServiceUrl = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? enabledTypes = null;
        if (enabledSignInTypes != null && enabledSignInTypes.Count > 0)
        {
            enabledTypes = new AndroidJavaObject("java.util.ArrayList");
                foreach (var item in enabledSignInTypes)
            {
                using (AndroidJavaObject itemObj = new AndroidJavaObject("java.lang.Integer", (int)item))
                {
                    enabledTypes.Call<bool>("add", itemObj);
                }
            }
        }
        else
        {
            enabledTypes = null;
        }
        AthanaUnityPluginClass.CallStatic("signInWithUI", enabledTypes, customUserId, privacyPolicyUrl, termsOfServiceUrl);
        enabledTypes?.Dispose();
    }

    public static void SignOut()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AthanaUnityPluginClass.CallStatic("signOut");
    }

    public static void AccountBinding(
        AthanaInterface.SignInType signInType,
        Dictionary<string, object>? extra = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("accountBinding", (int)signInType, extraMap);

        extraMap?.Dispose();
    }

    public static void AccountUnbind(
        AthanaInterface.SignInType signInType,
        string triOpenID,
        Dictionary<string, object>? extra = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("accountUnbind", (int)signInType, triOpenID, extraMap);

        extraMap?.Dispose();
    }

    public static void QueryAllAccountBind(Dictionary<string, object>? extra = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("queryAllAccountBind", extraMap);

        extraMap?.Dispose();
    }

    // AppOpenAd
    public static bool LoadAppOpenAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("loadAppOpenAd", adUnitId);
    }

    public static bool IsReadyAppOpenAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("isReadyAppOpenAd", adUnitId);
    }

    public static bool ShowAppOpenAd(string adUnitId, string? placement = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("showAppOpenAd", adUnitId, placement);
    }

    // RewardedAd
    public static bool LoadRewardedAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("loadRewardedAd", adUnitId);
    }

    public static bool IsReadyRewardedAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("isReadyRewardedAd", adUnitId);
    }

    public static bool ShowRewardedAd(string adUnitId, string? placement = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("showRewardedAd", adUnitId, placement);
    }

    // Interstitial
    public static bool LoadInterstitialAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("loadInterstitialAd", adUnitId);
    }

    public static bool IsReadyInterstitialAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("isReadyInterstitialAd", adUnitId);
    }

    public static bool ShowInterstitialAd(string adUnitId, string? placement = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("showInterstitialAd", adUnitId, placement);
    }

    // Banner
    public static BannerAd? CreateBanner(string adUnitId, AdSize size, string? placement = null, AdAlignment alignment = AdAlignment.BOTTOM_CENTER)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return null;
        }
        var bannerObj = AthanaUnityPluginClass.CallStatic<AndroidJavaObject>("createBanner", adUnitId, AdSize2J(size), placement, AdAlignment2J(alignment));
        return new BannerAdProxy(bannerObj);
    }

    public static void QueryProducts(HashSet<string> keys)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject keySet = new AndroidJavaObject("java.util.HashSet");
        foreach (var key in keys)
        {
            keySet.Call<bool>("add", key);
        }
        AthanaUnityPluginClass.CallStatic("queryProducts", keySet);
        keySet.Dispose();
    }

    public static void Purchase(
        AthanaInterface.IapProduct product,
        long? clientOrderId = null,
        bool consumable = true,
        Dictionary<string, object>? extra = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("purchase", product.key, product.subsInex, clientOrderId, consumable, extraMap);
    }

    public static void QueryPurchaseHistory()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AthanaUnityPluginClass.CallStatic("queryPurchaseHistory");
    }

    public static void VerifyOrder(
        AthanaInterface.IapPurchase purchase,
        bool consumable = true,
        Dictionary<string, object>? extra = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("verifyOrder", purchase.purchaseId, consumable, extraMap);
        extraMap?.Dispose();
    }

    public static void RequestReview()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AthanaUnityPluginClass.CallStatic("requestReview");
    }

    private static AndroidJavaObject? toJavaMap(Dictionary<string, object> extra)
    {

        AndroidJavaObject extraMap = new AndroidJavaObject("java.util.HashMap");
        foreach (var item in extra)
        {
            using (var javaKeyObj = new AndroidJavaObject("java.lang.String", item.Key))
            {

                var value = item.Value;
                AndroidJavaObject javaValueObj;
                if (value is string)
                {
                    javaValueObj = new AndroidJavaObject("java.lang.String", value);
                }
                else if (value is long)
                {
                    javaValueObj = new AndroidJavaObject("java.lang.Long", value);
                }
                else if (value is double)
                {
                    javaValueObj = new AndroidJavaObject("java.lang.Double", value);
                }
                else if (value is int)
                {
                    javaValueObj = new AndroidJavaObject("java.lang.Integer", value);
                }
                else if (value is bool)
                {
                    javaValueObj = new AndroidJavaObject("java.lang.Boolean", value);
                }
                else
                {
                    javaValueObj = new AndroidJavaObject("java.lang.String", value.ToString());
                }

                extraMap.Call<AndroidJavaObject>("put", javaKeyObj, javaValueObj);
                javaValueObj.Dispose();
            }
        }
        return extraMap;
    }

    internal class AdEventListener : AndroidJavaProxy
    {

        public AdEventListener() : base("com.inonesdk.athana.unity.AdEventListener") { }

        void onLoaded(string ad)
        {
            var adObj = JsonUtility.FromJson<ProxyAd>(ad);
            AthanaCallbacks.SendAdLoadedEvent(adObj);
        }

        void onLoadFailed(string ad, string? error)
        {
            var adObj = JsonUtility.FromJson<ProxyAd>(ad);
            var errorObj = error == null ? null : JsonUtility.FromJson<AdError>(error);
            AthanaCallbacks.SendAdLoadFailedEvent(adObj, errorObj);
        }

        void onDisplayed(string ad)
        {
            var adObj = JsonUtility.FromJson<ProxyAd>(ad);
            AthanaCallbacks.SendAdDisplayedEvent(adObj);
        }

        void onDisplayFailed(string ad, string? error)
        {
            var adObj = JsonUtility.FromJson<ProxyAd>(ad);
            var errorObj = error == null ? null : JsonUtility.FromJson<AdError>(error);
            AthanaCallbacks.SendAdDisplayFailedEvent(adObj, errorObj);
        }

        void onRewarded(string ad)
        {
            var adObj = JsonUtility.FromJson<ProxyAd>(ad);
            AthanaCallbacks.SendAdRewardedEvent(adObj);
        }

        void onClick(string ad)
        {
            var adObj = JsonUtility.FromJson<ProxyAd>(ad);
            AthanaCallbacks.SendAdClickEvent(adObj);
        }

        void onClosed(string ad)
        {
            var adObj = JsonUtility.FromJson<ProxyAd>(ad);
            AthanaCallbacks.SendAdClosedEvent(adObj);
        }
    }

    internal class BannerAdProxy : BannerAd, IDisposable
    {
        private readonly AndroidJavaObject _bannerObj;
        private bool _disposed = false;

        public BannerAdProxy(AndroidJavaObject bannerObj)
        {
            _bannerObj = bannerObj;
        }

        public void Show()
        {
            if (_disposed) return;
            _bannerObj.Call("show");
        }

        public void Hide()
        {
            if (_disposed) return;
            _bannerObj.Call("hide");
        }

        public void UpdateAlignment(AdAlignment alignment)
        {
            if (_disposed) return;
            _bannerObj.Call("updateAlignment", AdAlignment2J(alignment));
        }

        public void UpdateSize(AdSize size)
        {
            if (_disposed) return;
            _bannerObj.Call("updateSize", AdSize2J(size));
        }

        public void Destroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _bannerObj.Call("destroy");
            _bannerObj.Dispose();
        }

    }

    internal static AndroidJavaObject AdAlignment2J(AdAlignment alignment)
    {
        using (AndroidJavaClass jEnums = new AndroidJavaClass("com.inonesdk.athana.core.service.ad.AdAlignment"))
        {
            switch (alignment)
            {
                case AdAlignment.TOP_START:
                    return jEnums.GetStatic<AndroidJavaObject>("TOP_START");
                case AdAlignment.TOP_END:
                    return jEnums.GetStatic<AndroidJavaObject>("TOP_END");
                case AdAlignment.TOP_CENTER:
                    return jEnums.GetStatic<AndroidJavaObject>("TOP_CENTER");
                case AdAlignment.BOTTOM_START:
                    return jEnums.GetStatic<AndroidJavaObject>("BOTTOM_START");
                case AdAlignment.BOTTOM_END:
                    return jEnums.GetStatic<AndroidJavaObject>("BOTTOM_END");
                case AdAlignment.BOTTOM_CENTER:
                    return jEnums.GetStatic<AndroidJavaObject>("BOTTOM_CENTER");

                default:
                    return jEnums.GetStatic<AndroidJavaObject>("BOTTOM_CENTER");
            }
        }
    }

    internal static AndroidJavaObject AdSize2J(AdSize size)
    {
        return new AndroidJavaObject("com.inonesdk.athana.core.service.ad.AdSize", size.width, size.height);
    }

    internal class SdkCallbackProxy : AndroidJavaProxy
    {

        public SdkCallbackProxy() : base("com.inonesdk.athana.unity.SdkCallback") { }

        public void onResult(string content)
        {
            HandleSdkCallbackResult(content);
        }
    }

}

#endif