using System.Collections.Generic;
using UnityEngine;
using Athana.Api;

#nullable enable
public class AthanaUnityEditor : AthanaInterface
{
    public static void Initialize(
        long appId, 
        string appKey, 
        string appSecret,
        AthanaServiceConfig? serviceConfig = null,
        bool testMode = false, 
        bool debug = false)
    {
        DebugMode = debug;
        AthanaLogger.D("Calling Init on Unity Editor");
        AthanaLogger.D("serviceConfig: " + JsonUtility.ToJson(serviceConfig));
    }

    public static void Start(bool privacyGrant)
    {
        AthanaLogger.D("Calling Start on Unity Editor");
    }

    public static void CurrentUser()
    {
        AthanaLogger.D("Calling CurrentUser on Unity Editor");
    }

    public static void RegistryUser(
        AthanaInterface.SignInType signInType = AthanaInterface.SignInType.ANONYMOUS,
        string? ua = null,
        string? deviceId = null,
        long? customUserId = null,
        Dictionary<string, object>? extra = null
        )
    {
        AthanaLogger.D("Calling RegistryUser on Unity Editor");
    }

    public static void SignIn(
        AthanaInterface.SignInType signInType = AthanaInterface.SignInType.ANONYMOUS,
        string? ua = null,
        string? deviceId = null,
        long? customUserId = null,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling SignIn on Unity Editor");
    }

    public static void SignInWithUI(
        List<AthanaInterface.SignInType>? enabledSignInTypes = null,
        long? customUserId = null,
        string? privacyPolicyUrl = null,
        string? termsOfServiceUrl = null)
    {
        AthanaLogger.D("Calling SignInWithUI on Unity Editor");
    }

    public static void SignOut()
    {
        AthanaLogger.D("Calling SignOut on Unity Editor");
    }

    public static void AccountBinding(
        AthanaInterface.SignInType signInType,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling AccountBinding on Unity Editor");
    }

    public static void AccountUnbind(
        AthanaInterface.SignInType signInType,
        string triOpenID,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling AccountUnbind on Unity Editor");
    }

    public static void QueryAllAccountBind(Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling QueryAllAccountBind on Unity Editor");
    }


    public static AndroidJavaObject? GetAdService()
    {
        AthanaLogger.D("Calling GetAdService on Unity Editor");
        return null;
    }


    // AppOpenAd
    public static bool LoadAppOpenAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadAppOpenAd on Unity Editor");
        return false;
    }

    public static bool IsReadyAppOpenAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyAppOpenAd on Unity Editor");
        return false;
    }

    public static bool ShowAppOpenAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowAppOpenAd on Unity Editor");
        return false;
    }

    // RewardedAd
    public static bool LoadRewardedAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadRewardedAd on Unity Editor");
        return false;
    }

    public static bool IsReadyRewardedAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyRewardedAd on Unity Editor");
        return false;
    }

    public static bool ShowRewardedAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowRewardedAd on Unity Editor");
        return false;
    }

    // Interstitial
    public static bool LoadInterstitialAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadInterstitialAd on Unity Editor");
        return false;
    }

    public static bool IsReadyInterstitialAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyInterstitialAd on Unity Editor");
        return false;
    }

    public static bool ShowInterstitialAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowInterstitialAd on Unity Editor");
        return false;
    }

    // Banner
    public static BannerAd? CreateBanner(string adUnitId, AdSize size, string? placement = null, AdAlignment alignment = AdAlignment.BOTTOM_CENTER)
    {
        AthanaLogger.D("Calling CreateBanner on Unity Editor");
        return null;
    }

    public static void QueryProducts(HashSet<string> keys)
    {
        AthanaLogger.D("Calling QueryProducts on Unity Editor");
    }

    public static void Purchase(
        AthanaInterface.IapProduct product,
        long? clientOrderId = null,
        bool consumable = true,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling Purchase on Unity Editor");
    }

    public static void QueryPurchaseHistory()
    {
        AthanaLogger.D("Calling QueryPurchaseHistory on Unity Editor");
    }

    public static void VerifyOrder(
        AthanaInterface.IapPurchase purchase,
        bool consumable = true,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling VerifyOrder on Unity Editor");
    }

    public static void RequestReview()
    {
        AthanaLogger.D("Calling RequestReview on Unity Editor");
    }

}