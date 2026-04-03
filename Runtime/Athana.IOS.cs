using System;
using System.Collections.Generic;
using UnityEngine;
using Athana.Api;
using Athana.Callbacks;
using System.Runtime.InteropServices;
using AOT;
using System.Linq;
using Newtonsoft.Json;

#nullable enable
public class AthanaiOS : AthanaInterface
{
    private delegate void NativeResultDelegate(string jsonResult);

    private delegate void AdEventDelegate(string functionName, string ad, string? error);

    private delegate void NativeBoolResultDelegate(bool result);

    private static AthanaServiceConfig? _serviceConfig = null;

    static AthanaiOS()
    {
#if UNITY_IOS
        // 设置SDK回调函数
        setSdkCallback(SdkCallback);
        // 设置广告事件回调函数
        setAdEventCallback(AdEventCallback);
#endif
    }

#if UNITY_IOS
    [MonoPInvokeCallback(typeof(NativeResultDelegate))]
    private static void SdkCallback(string jsonResult)
    {
        HandleSdkCallbackResult(jsonResult);
    }

    [MonoPInvokeCallback(typeof(AdEventDelegate))]
    private static void AdEventCallback(string functionName, string ad, string? error)
    {
        var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
        var errorObj = error == null ? null : JsonConvert.DeserializeObject<AdError>(error);
        if (adObj == null)
        {
            AthanaLogger.W($"AdEventCallback: adObj is null, functionName: {functionName}, ad: {ad}, error: {error}");
            return;
        }
        switch (functionName)
        {
            case "onLoaded":
                AthanaCallbacks.SendAdLoadedEvent(adObj);
                break;
            case "onLoadFailed":
                AthanaCallbacks.SendAdLoadFailedEvent(adObj, errorObj);
                break;
            case "onDisplayed":
                AthanaCallbacks.SendAdDisplayedEvent(adObj);
                break;
            case "onDisplayFailed":
                AthanaCallbacks.SendAdDisplayFailedEvent(adObj, errorObj);
                break;
            case "onRewarded":
                AthanaCallbacks.SendAdRewardedEvent(adObj);
                break;
            case "onClick":
                AthanaCallbacks.SendAdClickEvent(adObj);
                break;
            case "onClosed":
                AthanaCallbacks.SendAdClosedEvent(adObj);
                break;
        }
    }

    [DllImport("__Internal")]
    private static extern void setSdkCallback(NativeResultDelegate callback);

    [DllImport("__Internal")]
    private static extern void setAdEventCallback(AdEventDelegate callback);

    private static bool _isInitialized = false;

    [DllImport("__Internal")]
    private static extern void athanaInitSdk(
        long appId,
        string appKey,
        string appSecret,
        string? accountConfigJSON,
        string? adConfigJSON,
        string? conversionConfigJSON,
        bool testMode,
        bool debug,
        bool readClipBoard);

    /// <summary>
    /// SDK 初始化
    /// </summary>
    /// <param name="appId">应用ID</param>
    /// <param name="appKey">应用Key</param>
    /// <param name="appSecret">应用Secret</param>
    /// <param name="serviceConfig">服务配置</param>
    /// <param name="testMode">支付测试模式</param>
    /// <param name="debug">SDK调试模式，默认为 false，设置 true 将在日志中输出调试日志</param>
    /// <param name="readClipBoard">SDK访问剪贴板（落地页自归因方案），默认为 false，设置 true 将在注册用户时访问剪贴板</param>
    public static void Initialize(
        long appId,
        string appKey,
        string appSecret,
        AthanaServiceConfig? serviceConfig = null,
        bool testMode = false,
        bool debug = false,
        bool readClipBoard = false)
    {
        AthanaLogger.D("Calling Initialize");
        if (_isInitialized)
        {
            AthanaLogger.D("Athana is initialized");
            return;
        }
        DebugMode = debug;

        // 设置广告监听

        string? accountConfigJSON = null;
        string? adConfigJSON = null;
        string? conversionConfigJSON = null;

        if (serviceConfig != null)
        {
            _serviceConfig = serviceConfig;
            AthanaLogger.D("serviceConfig: " + JsonUtility.ToJson(serviceConfig));

            var accountConfig = serviceConfig.AccountConfig;
            accountConfigJSON = accountConfig == null ? null : JsonUtility.ToJson(accountConfig);

            var adConfig = serviceConfig.AdServiceConfigs;
            adConfigJSON = adConfig == null ? null : JsonUtility.ToJson(adConfig);

            var conversionConfig = serviceConfig.ConversionServiceConfigs;
            conversionConfigJSON = conversionConfig == null ? null : JsonUtility.ToJson(conversionConfig);
        }

        // 初始化 SDK
        athanaInitSdk(
            appId,
            appKey,
            appSecret,
            accountConfigJSON,
            adConfigJSON,
            conversionConfigJSON,
            testMode,
            debug,
            readClipBoard);
        _isInitialized = true;
    }

    [DllImport("__Internal")]
    private static extern void athanaStart(bool privacyGrant);

    /// <summary>
    /// 启动SDK，请在SDK初始化后调用
    /// </summary>
    /// <param name="privacyGrant">用户对隐私协议的确认结果</param>
    public static void Start(bool privacyGrant = false)
    {
        AthanaLogger.D("Calling Start");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }


        athanaStart(privacyGrant);
    }

    [DllImport("__Internal")]
    private static extern void athanaGetCurrentUser();

    /// <summary>
    /// 获取当前登入的账户信息，如返回 null 则表示未登入或凭证失效
    /// </summary>
    public static void CurrentUser()
    {
        AthanaLogger.D("Calling CurrentUser");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }


        athanaGetCurrentUser();
    }

    [DllImport("__Internal")]
    private static extern void athanaRegisterUser(
        int signInType,
        long customUserId,
        string? extra);

    /// <summary>
    /// 注册平台用户
    /// </summary>
    /// <param name="signInType">登入方式</param>
    /// <param name="ua">设备信息，可选参数，不传入则会自动获取设备的WebView内的User-Agent</param>
    /// <param name="deviceId">设备标识，可选参数，不传入则在 Android 平台上会获取 ANDROID_ID</param>
    /// <param name="customUserId">自定义用户ID - 游戏用户ID</param>
    /// <param name="extra">额外参数</param>
    public static void RegistryUser(
        AthanaInterface.SignInType signInType = AthanaInterface.SignInType.ANONYMOUS,
        string? ua = null,
        string? deviceId = null,
        long? customUserId = null,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling RegistryUser");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaRegisterUser(
            (int)signInType,
            customUserId ?? -1,
            extra == null ? null : JsonConvert.SerializeObject(extra));
    }

    [DllImport("__Internal")]
    private static extern void athanaSignIn(
        int signInType,
        long customUserId,
        string? extra);

    /// <summary>
    /// 登入
    /// </summary>
    /// <param name="signInType">登入方式</param>
    /// <param name="ua">设备信息，可选参数，不传入则会自动获取设备的WebView内的User-Agent</param>
    /// <param name="deviceId">设备标识，可选参数，不传入则在 Android 平台上会获取 ANDROID_ID</param>
    /// <param name="customUserId">自定义用户ID - 游戏用户ID</param>
    /// <param name="extra">额外参数</param>
    public static void SignIn(
        AthanaInterface.SignInType signInType = AthanaInterface.SignInType.ANONYMOUS,
        string? ua = null,
        string? deviceId = null,
        long? customUserId = null,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling SignIn");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }


        athanaSignIn(
            (int)signInType,
            customUserId ?? -1,
            extra == null ? null : JsonConvert.SerializeObject(extra));
    }

    [DllImport("__Internal")]
    private static extern void athanaSignInWithUI(
        int[] enabledSignInTypes,
        int size,
        long customUserId,
        string? privacyPolicyUrl,
        string? termsOfServiceUrl);

    /// <summary>
    /// 使用内置UI登入
    /// </summary>
    /// <param name="enabledSignInTypes">配置可供使用的登入方式，默认为 null 标识全开</param>
    /// <param name="customUserId">自定义用户ID - 游戏用户ID</param>
    /// <param name="privacyPolicyUrl"></param>
    /// <param name="termsOfServiceUrl"></param>
    public static void SignInWithUI(
        List<AthanaInterface.SignInType>? enabledSignInTypes = null,
        long? customUserId = null,
        string? privacyPolicyUrl = null,
        string? termsOfServiceUrl = null)
    {
        AthanaLogger.D("Calling SignInWithUI");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }


        athanaSignInWithUI(
            enabledSignInTypes?.Select(x => (int)x).ToArray() ?? Array.Empty<int>(),
            enabledSignInTypes?.Count ?? 0,
            customUserId ?? -1,
            privacyPolicyUrl,
            termsOfServiceUrl);
    }

    [DllImport("__Internal")]
    private static extern void athanaSignOut();

    /// <summary>
    /// 登出
    /// </summary>
    public static void SignOut()
    {
        AthanaLogger.D("Calling SignOut");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }


        athanaSignOut();
    }

    [DllImport("__Internal")]
    private static extern void athanaAccountBinding(int signInType);

    /// <summary>
    /// 绑定三方账号
    /// </summary>
    /// <param name="signInType">登入方式</param>
    /// <param name="extra">额外参数</param>
    public static void AccountBinding(
        AthanaInterface.SignInType signInType,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling AccountBinding");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }


        athanaAccountBinding((int)signInType);
    }

    [DllImport("__Internal")]
    private static extern void athanaAccountUnbind(
        int signInType,
        string triOpenID);

    /// <summary>
    /// 解绑三方账号
    /// </summary>
    /// <param name="signInType">登入方式</param>
    /// <param name="triOpenID">三方OpenID，从查询三方账号绑定信息接口获取</param>
    /// <param name="extra">额外参数，可选参数</param>
    public static void AccountUnbind(
        AthanaInterface.SignInType signInType,
        string triOpenID,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling AccountUnbind");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaAccountUnbind((int)signInType, triOpenID);
    }

    [DllImport("__Internal")]
    private static extern void athanaQueryAllAccountBind();

    /// <summary>
    /// 查询三方账号绑定信息
    /// </summary>
    /// <param name="extra">额外参数，可选参数</param>
    public static void QueryAllAccountBind(Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling QueryAllAccountBind");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaQueryAllAccountBind();
    }

    [DllImport("__Internal")]
    private static extern bool athanaLoadAppOpenAd(string adUnitId);

    /// <summary>
    /// 加载应用启动广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadAppOpenAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadAppOpenAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaLoadAppOpenAd(adUnitId);
    }

    [DllImport("__Internal")]
    private static extern bool athanaIsReadyAppOpenAd(string adUnitId);

    /// <summary>
    /// 判断应用启动广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyAppOpenAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyAppOpenAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaIsReadyAppOpenAd(adUnitId);
    }

    [DllImport("__Internal")]
    private static extern bool athanaShowAppOpenAd(string adUnitId, string? placement);

    /// <summary>
    /// 展示应用启动广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns>调用结果</returns>
    public static bool ShowAppOpenAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowAppOpenAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaShowAppOpenAd(adUnitId, placement);
    }

    [DllImport("__Internal")]
    private static extern bool athanaLoadRewardedAd(string adUnitId);

    /// <summary>
    /// 加载激励广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadRewardedAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadRewardedAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaLoadRewardedAd(adUnitId);
    }

    [DllImport("__Internal")]
    private static extern bool athanaIsReadyRewardedAd(string adUnitId);

    /// <summary>
    /// 判断激励广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyRewardedAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyRewardedAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaIsReadyRewardedAd(adUnitId);
    }

    [DllImport("__Internal")]
    private static extern bool athanaShowRewardedAd(string adUnitId, string? placement);

    /// <summary>
    /// 展示激励广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns></returns>
    public static bool ShowRewardedAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowRewardedAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaShowRewardedAd(adUnitId, placement);
    }

    [DllImport("__Internal")]
    private static extern bool athanaLoadInterstitialAd(string adUnitId);

    /// <summary>
    /// 加载插屏广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadInterstitialAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadInterstitialAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaLoadInterstitialAd(adUnitId);
    }

    [DllImport("__Internal")]
    private static extern bool athanaIsReadyInterstitialAd(string adUnitId);

    /// <summary>
    /// 判断插屏广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyInterstitialAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyInterstitialAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaIsReadyInterstitialAd(adUnitId);
    }

    [DllImport("__Internal")]
    private static extern bool athanaShowInterstitialAd(string adUnitId, string? placement);

    /// <summary>
    /// 展示插屏广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns></returns>
    public static bool ShowInterstitialAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowInterstitialAd");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaShowInterstitialAd(adUnitId, placement);
    }

    [DllImport("__Internal")]
    private static extern bool athanaCreateBanner(string adUnitId, string? placement, string? size, string? alignment);

    /// <summary>
    /// 创建横幅广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="size">横幅尺寸</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <param name="alignment">横幅位置</param>
    /// <returns>返回null表示创建失败</returns>
    public static BannerAd? CreateBanner(string adUnitId, AdSize size, string? placement = null, AdAlignment alignment = AdAlignment.BOTTOM_CENTER)
    {
        AthanaLogger.D("Calling CreateBanner");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return null;
        }

        bool result = athanaCreateBanner(adUnitId, placement, JsonConvert.SerializeObject(size), alignment.ToString());
        if (!result)
        {
            AthanaLogger.W("CreateBanner failed");
            return null;
        }

        return new BannerAdProxy();
    }

    [DllImport("__Internal")]
    private static extern bool athanaStoreIsAvailable();

    /// <summary>
    /// 查询商店服务是否可用
    /// </summary>
    /// <returns></returns>
    public static bool StoreIsAvailable()
    {
        AthanaLogger.D("Calling StoreIsAvailable");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }

        return athanaStoreIsAvailable();
    }

    [DllImport("__Internal")]
    private static extern void athanaQueryProducts(string[] keys, int size);

    /// <summary>
    /// 查询商品信息
    /// </summary>
    /// <param name="keys">待查商品Key</param>
    public static void QueryProducts(HashSet<string> keys)
    {
        AthanaLogger.D("Calling QueryProducts");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        if (keys.Count == 0)
        {
            AthanaLogger.W("Keys is empty");
            return;
        }

        string[] keysArray = keys.ToArray();
        athanaQueryProducts(keysArray, keysArray.Length);
    }

    [DllImport("__Internal")]
    private static extern void athanaPurchase(string productId, long clientOrderId);

    /// <summary>
    /// 购买
    /// </summary>
    /// <param name="product">需购买的商品信息</param>
    /// <param name="clientOrderId">自定义订单 - 游戏后端订单</param>
    /// <param name="consumable">是否可消耗，默认为 true。如购买的商品是限制购买次数，例如：永久去广告，则传入 false</param>
    /// <param name="extra">额外参数，可选参数</param>
    public static void Purchase(
        AthanaInterface.IapProduct product,
        long? clientOrderId = null,
        bool consumable = true,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling Purchase");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaPurchase(product.key, clientOrderId ?? -1L);
    }

    [DllImport("__Internal")]
    private static extern void athanaQueryPurchaseHistory();

    /// <summary>
    /// 查询购买历史记录，只返回：未确认的订单、有效期内的订阅项、非一次性消耗类商品
    /// </summary>
    public static void QueryPurchaseHistory()
    {
        AthanaLogger.D("Calling QueryPurchaseHistory");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaQueryPurchaseHistory();
    }

    [DllImport("__Internal")]
    private static extern void athanaVerifyOrder(string purchaseId);

    /// <summary>
    /// 验证订单，适用于掉单复验
    /// </summary>
    /// <param name="purchase">从查询购买历史记录接口获得的订单信息</param>
    /// <param name="consumable">是否可消耗，默认为 true。如购买的商品是限制购买次数，例如：永久去广告，则传入 false</param>
    /// <param name="extra">额外参数，可选参数</param>
    public static void VerifyOrder(
        AthanaInterface.IapPurchase purchase,
        bool consumable = true,
        Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling VerifyOrder");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        string? purchaseId = purchase.purchaseId;
        if (string.IsNullOrEmpty(purchaseId))
        {
            AthanaLogger.W("PurchaseId is empty");
            return;
        }

        athanaVerifyOrder(purchaseId);
    }

    [DllImport("__Internal")]
    private static extern void athanaRequestAppReview();

    /// <summary>
    /// 拉起应用内评价
    /// </summary>
    public static void RequestReview()
    {
        AthanaLogger.D("Calling RequestReview");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaRequestAppReview();
    }

    private static Action<bool>? cpnpCallback;

    [MonoPInvokeCallback(typeof(NativeBoolResultDelegate))]
    private static void CheckPostNotificationPermissionCallback(bool result)
    {
        // 处理查询结果
        cpnpCallback?.Invoke(result);
    }

    [DllImport("__Internal")]
    private static extern void athanaCheckPostNotificationPermission(NativeBoolResultDelegate callback);

    /// <summary>
    /// 查询是否已授权发送通知权限
    /// </summary>
    /// <param name="callback">回调函数名，用于返回查询结果</param>
    public static void CheckPostNotificationPermission(Action<bool> callback)
    {
        AthanaLogger.D("Calling CheckPostNotificationPermission");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        cpnpCallback = callback;
        athanaCheckPostNotificationPermission(CheckPostNotificationPermissionCallback);
    }

    [DllImport("__Internal")]
    private static extern void athanaRequestPostNotificationPermission();

    /// <summary>
    /// 请求发送通知权限
    /// <summary>
    public static void RequestPostNotificationPermission()
    {
        AthanaLogger.D("Calling RequestPostNotificationPermission");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaRequestPostNotificationPermission();
    }

    [DllImport("__Internal")]
    private static extern void athanaSendEvent(string key, string type, string? paramMap);

    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="key">事件名</param>
    /// <param name="type">事件类型，默认为 game</param>
    /// <param name="paramMap">事件参数</param>
    public static void SendEvent(string key, string type = "game", Dictionary<string, object>? paramMap = null)
    {
        AthanaLogger.D($"Calling SendEvent {key}");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        string paramMapStr = paramMap == null ? "" : JsonConvert.SerializeObject(paramMap);
        athanaSendEvent(key, type, paramMapStr);
    }

    [DllImport("__Internal")]
    private static extern void athanaUpdateUserInfo(long customUserId);

    /// <summary>
    /// 更新用户信息，适用于玩家首次登入后，补充游戏自有账户体系的账户ID
    /// </summary>
    /// <param name="customUserId">自定义用户ID - 游戏账户ID</param>
    /// <param name="extra">额外参数，可选</param>
    public static void UpdateUserInfo(long customUserId, Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D($"Calling UpdateUserInfo");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaUpdateUserInfo(customUserId);
    }

    [DllImport("__Internal")]
    private static extern string athanaGetAppLang();

    /// <summary>
    /// 获取应用语言
    /// </summary>
    /// <returns>如未设置可能返回 null </returns>
    public static String? GetAppLanguage()
    {
        return athanaGetAppLang();
    }

    [DllImport("__Internal")]
    private static extern string athanaGetSysLang();

    /// <summary>
    /// 获取系统语言
    /// </summary>
    /// <returns>返回系统层级设置的语言</returns>
    public static String GetSysLanguage()
    {
        return athanaGetSysLang();
    }

    [DllImport("__Internal")]
    private static extern string athanaGetSysCountryCode();

    /// <summary>
    /// 获取系统国家地区代码
    /// </summary>
    /// <returns>返回系统层级设置的国家地区</returns>
    public static String GetSysCountry()
    {
        return athanaGetSysCountryCode() ?? "";
    }

    [DllImport("__Internal")]
    private static extern void athanaOpenAppStoreDetail(string appId);

    /// <summary>
    /// 跳转到应用商店详情页
    /// </summary>
    public static void OpenStoreDetail()
    {
        string? appId = _serviceConfig?.ConversionServiceConfigs?.appsflyer?.appId;
        if (string.IsNullOrEmpty(appId))
        {
            AthanaLogger.W("AppId is empty");
            return;
        }

        athanaOpenAppStoreDetail(appId);
    }

    #region Gaming Service APIs

    #region Leaderboard

    [DllImport("__Internal")]
    private static extern void athanaOpenLeaderboardUI(string? leaderboardId, int playerScope, int timeScope);

    /// <summary>
    /// 打开排行榜 UI
    /// </summary>
    /// <param name="leaderboardId">排行榜 ID，为空时显示所有排行榜</param>
    /// <param name="scope">玩家范围</param>
    /// <param name="timeScope">时间范围</param>
    public static void OpenLeaderboardUI(string? leaderboardId = null, LeaderboardPlayerScope? scope = null, LeaderboardTimeSpan? timeScope = null)
    {
        AthanaLogger.D($"Calling OpenLeaderboardUI");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaOpenLeaderboardUI(leaderboardId, (int)(scope ?? LeaderboardPlayerScope.ALL), (int)(timeScope ?? LeaderboardTimeSpan.ALL_TIME));
    }

    [DllImport("__Internal")]
    private static extern void athanaGetLeaderboardInfo(string? leaderboardId);

    /// <summary>
    /// 获取排行榜信息
    /// </summary>
    /// <param name="leaderboardId">排行榜 ID，为空时获取所有排行榜信息</param>
    /// <param name="forceReload">是否强制刷新</param>
    public static void GetLeaderboardInfo(string? leaderboardId = null, bool forceReload = false)
    {
        AthanaLogger.D($"Calling GetLeaderboardInfo");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaGetLeaderboardInfo(leaderboardId);
    }

    [DllImport("__Internal")]
    private static extern void athanaSubmitScore(string leaderboardId, int score, int context);

    /// <summary>
    /// 提交分数到排行榜
    /// </summary>
    /// <param name="leaderboardId">排行榜 ID</param>
    /// <param name="score">分数</param>
    /// <param name="extra">额外信息</param>
    /// <param name="immediate">false 为异步提交，true 为实时提交。设置为 true 才会在监听收到回调</param>
    public static void SubmitScore(string leaderboardId, long score, string? extra = null, bool immediate = false)
    {
        AthanaLogger.D($"Calling SubmitScore");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaSubmitScore(leaderboardId, (int)score, immediate ? 1 : 0);
    }

    [DllImport("__Internal")]
    private static extern void athanaLoadLeaderboardScores(string leaderboardId, int playerScope, int timeScope, int length);

    /// <summary>
    /// 加载排行榜数据
    /// </summary>
    /// <param name="leaderboardId">排行榜 ID</param>
    /// <param name="scope">玩家范围</param>
    /// <param name="timeScope">时间范围</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="userCenter">是否以当前用户为中心</param>
    public static void LoadLeaderboardData(string leaderboardId, LeaderboardPlayerScope scope, LeaderboardTimeSpan timeScope, int pageSize, bool userCenter = false)
    {
        AthanaLogger.D($"Calling LoadLeaderboardData");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaLoadLeaderboardScores(leaderboardId, (int)scope, (int)timeScope, pageSize);
    }

    [DllImport("__Internal")]
    private static extern void athanaLoadMoreLeaderboardScores(string leaderboardId, int length);

    /// <summary>
    /// 加载更多排行榜数据
    /// </summary>
    /// <param name="leaderboardId">排行榜 ID</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="pageDirection">分页方向</param>
    public static void LoadMoreLeaderboardData(string leaderboardId, int pageSize, PageDirection pageDirection)
    {
        AthanaLogger.D($"Calling LoadMoreLeaderboardData");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaLoadMoreLeaderboardScores(leaderboardId, pageSize);
    }

    [DllImport("__Internal")]
    private static extern void athanaGetScore(string leaderboardId, int playerScope, int timeScope);

    /// <summary>
    /// 获取玩家分数
    /// </summary>
    /// <param name="leaderboardId">排行榜 ID</param>
    /// <param name="scope">玩家范围</param>
    /// <param name="timeScope">时间范围</param>
    public static void GetScore(string leaderboardId, LeaderboardPlayerScope scope, LeaderboardTimeSpan timeScope)
    {
        AthanaLogger.D($"Calling GetScore");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaGetScore(leaderboardId, (int)scope, (int)timeScope);
    }

    /// <summary>
    /// 释放排行榜数据资源
    /// </summary>
    public static void LeaderboardDataRelease()
    {
        AthanaLogger.D($"Calling LeaderboardDataRelease");
    }

    #endregion

    #region Achievement

    [DllImport("__Internal")]
    private static extern void athanaOpenAchievementUI();

    /// <summary>
    /// 打开成就 UI
    /// </summary>
    public static void OpenAchievementUI()
    {
        AthanaLogger.D($"Calling OpenAchievementUI");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaOpenAchievementUI();
    }

    [DllImport("__Internal")]
    private static extern void athanaGetAchievementList();

    /// <summary>
    /// 获取成就数据
    /// </summary>
    /// <param name="forceReload">是否强制刷新</param>
    public static void GetAchievementData(bool forceReload = false)
    {
        AthanaLogger.D($"Calling GetAchievementData");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaGetAchievementList();
    }

    [DllImport("__Internal")]
    private static extern void athanaUnlockAchievement(string achievementId);

    /// <summary>
    /// 解锁成就
    /// </summary>
    /// <param name="achievementId">成就 ID</param>
    /// <param name="immediate">false 为异步提交，true 为实时提交。设置为 true 才会在监听收到回调</param>
    public static void UnlockAchievement(string achievementId, bool immediate = false)
    {
        AthanaLogger.D($"Calling UnlockAchievement");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaUnlockAchievement(achievementId);
    }

    [DllImport("__Internal")]
    private static extern void athanaUpdateAchievementProgress(string achievementId, int currentValue);

    /// <summary>
    /// 更新成就进度
    /// </summary>
    /// <param name="achievementId">成就 ID</param>
    /// <param name="progress">进度值（0-100）</param>
    /// <param name="immediate">false 为异步提交，true 为实时提交。设置为 true 才会在监听收到回调</param>
    public static void UpdateAchievementProgress(string achievementId, int progress, bool immediate = false)
    {
        AthanaLogger.D($"Calling UpdateAchievementProgress");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaUpdateAchievementProgress(achievementId, progress);
    }

    #endregion

    #region Friend

    [DllImport("__Internal")]
    private static extern void athanaRequestFriendListPermission();

    /// <summary>
    /// 请求好友列表访问权限
    /// </summary>
    public static void RequestFriendListPermission()
    {
        AthanaLogger.D($"Calling RequestFriendListPermission");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaRequestFriendListPermission();
    }

    [DllImport("__Internal")]
    private static extern void athanaLoadFriendList(int length);

    /// <summary>
    /// 加载好友列表
    /// </summary>
    /// <param name="pageSize">每页数量</param>
    /// <param name="forceReload">是否强制刷新</param>
    public static void LoadFriends(int pageSize, bool forceReload = false)
    {
        AthanaLogger.D($"Calling LoadFriends");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaLoadFriendList(pageSize);
    }

    [DllImport("__Internal")]
    private static extern void athanaLoadMoreFriendList(int length);

    /// <summary>
    /// 加载更多好友
    /// </summary>
    /// <param name="pageSize">每页数量</param>
    public static void LoadMoreFriends(int pageSize)
    {
        AthanaLogger.D($"Calling LoadMoreFriends");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaLoadMoreFriendList(pageSize);
    }

    #endregion

    #region Player

    [DllImport("__Internal")]
    private static extern void athanaOpenPlayerProfileUI(string playerId);

    /// <summary>
    /// 打开玩家资料 UI
    /// </summary>
    /// <param name="playerId">玩家 ID</param>
    public static void OpenPlayerProfileUI(string playerId)
    {
        AthanaLogger.D($"Calling OpenPlayerProfileUI");
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        athanaOpenPlayerProfileUI(playerId);
    }

    #endregion

    #endregion


    internal class BannerAdProxy : BannerAd, IDisposable
    {

        private bool _disposed = false;

        [DllImport("__Internal")]
        private static extern bool athanaHideBanner();

        public void Hide()
        {
            if (_disposed) return;
            athanaHideBanner();
        }

        [DllImport("__Internal")]
        private static extern bool athanaShowBanner();

        public void Show()
        {
            if (_disposed) return;
            athanaShowBanner();
        }

        [DllImport("__Internal")]
        private static extern bool athanaUpdateBannerAlignment(string alignment);

        public void UpdateAlignment(AdAlignment alignment)
        {
            if (_disposed) return;
            athanaUpdateBannerAlignment(alignment.ToString());
        }

        [DllImport("__Internal")]
        private static extern bool athanaUpdateBannerSize(string size);

        public void UpdateSize(AdSize size)
        {
            if (_disposed) return;
            athanaUpdateBannerSize(JsonConvert.SerializeObject(size));
        }

        [DllImport("__Internal")]
        private static extern bool athanaDestroyBanner();

        public void Destroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            athanaDestroyBanner();
        }
    }
#endif
}
