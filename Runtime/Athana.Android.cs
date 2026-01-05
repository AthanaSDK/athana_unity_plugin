using System;
using System.Collections.Generic;
using UnityEngine;
using Athana.Api;
using Athana.Callbacks;
using Newtonsoft.Json;

#if UNITY_ANDROID

#nullable enable
public class AthanaAndroid : AthanaInterface
{

    private static AndroidJavaClass AthanaUnityPluginClass;

    private static readonly SdkCallbackProxy SdkCallback = new SdkCallbackProxy();
    private static readonly AdEventListener AdListener = new AdEventListener();

    private static bool _isInitialized = false;

    /// <summary>
    /// SDK 初始化
    /// </summary>
    /// <param name="appId">应用ID</param>
    /// <param name="appKey">应用Key</param>
    /// <param name="appSecret">应用Secret</param>
    /// <param name="serviceConfig">服务配置</param>
    /// <param name="testMode">支付测试模式</param>
    /// <param name="debug">SDK调试模式，默认为 false，设置 true 将在日志中输出调试日志</param>
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

    /// <summary>
    /// 启动SDK，请在SDK初始化后调用
    /// </summary>
    /// <param name="privacyGrant">用户对隐私协议的确认结果</param>
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

    /// <summary>
    /// 获取当前登入的账户信息，如返回 null 则表示未登入或凭证失效
    /// </summary>
    public static void CurrentUser()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }
        AthanaUnityPluginClass.CallStatic("currentUser");
    }

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
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("signIn", (int)signInType, ua, deviceId, customUserId, extraMap);

        extraMap?.Dispose();
    }

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

    /// <summary>
    /// 登出
    /// </summary>
    public static void SignOut()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AthanaUnityPluginClass.CallStatic("signOut");
    }

    /// <summary>
    /// 绑定三方账号
    /// </summary>
    /// <param name="signInType">登入方式</param>
    /// <param name="extra">额外参数</param>
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
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("accountUnbind", (int)signInType, triOpenID, extraMap);

        extraMap?.Dispose();
    }

    /// <summary>
    /// 查询三方账号绑定信息
    /// </summary>
    /// <param name="extra">额外参数，可选参数</param>
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

    /// <summary>
    /// 加载应用启动广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadAppOpenAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("loadAppOpenAd", adUnitId);
    }

    /// <summary>
    /// 判断应用启动广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyAppOpenAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("isReadyAppOpenAd", adUnitId);
    }

    /// <summary>
    /// 展示应用启动广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns>调用结果</returns>
    public static bool ShowAppOpenAd(string adUnitId, string? placement = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("showAppOpenAd", adUnitId, placement);
    }

    /// <summary>
    /// 加载激励广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadRewardedAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("loadRewardedAd", adUnitId);
    }

    /// <summary>
    /// 判断激励广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyRewardedAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("isReadyRewardedAd", adUnitId);
    }

    /// <summary>
    /// 展示激励广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns></returns>
    public static bool ShowRewardedAd(string adUnitId, string? placement = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("showRewardedAd", adUnitId, placement);
    }

    /// <summary>
    /// 加载插屏广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadInterstitialAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("loadInterstitialAd", adUnitId);
    }

    /// <summary>
    /// 判断插屏广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyInterstitialAd(string adUnitId)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("isReadyInterstitialAd", adUnitId);
    }

    /// <summary>
    /// 展示插屏广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns></returns>
    public static bool ShowInterstitialAd(string adUnitId, string? placement = null)
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("showInterstitialAd", adUnitId, placement);
    }

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
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return null;
        }
        var bannerObj = AthanaUnityPluginClass.CallStatic<AndroidJavaObject>("createBanner", adUnitId, AdSize2J(size), placement, AdAlignment2J(alignment));
        return new BannerAdProxy(bannerObj);
    }

    /// <summary>
    /// 查询商店服务是否可用
    /// </summary>
    /// <returns></returns>
    public static bool StoreIsAvailable()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return false;
        }
        return AthanaUnityPluginClass.CallStatic<bool>("storeIsAvailable");
    }

    /// <summary>
    /// 查询商品信息
    /// </summary>
    /// <param name="keys">待查商品Key</param>
    public static void QueryProducts(HashSet<string> keys)
    {
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

        AndroidJavaObject keySet = new AndroidJavaObject("java.util.HashSet");
        foreach (var key in keys)
        {
            keySet.Call<bool>("add", key);
        }
        AthanaUnityPluginClass.CallStatic("queryProducts", keySet);
        keySet.Dispose();
    }

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
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("purchase", product.key, product.subsInex, clientOrderId, consumable, extraMap);
    }

    /// <summary>
    /// 查询购买历史记录，只返回：未确认的订单、有效期内的订阅项、非一次性消耗类商品
    /// </summary>
    public static void QueryPurchaseHistory()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AthanaUnityPluginClass.CallStatic("queryPurchaseHistory");
    }

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
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AndroidJavaObject? extraMap = extra == null ? null : toJavaMap(extra);

        AthanaUnityPluginClass.CallStatic("verifyOrder", purchase.purchaseId, consumable, extraMap);
        extraMap?.Dispose();
    }

    /// <summary>
    /// 拉起应用内评价
    /// </summary>
    public static void RequestReview()
    {
        if (!_isInitialized)
        {
            AthanaLogger.W("Athana is not initialized");
            return;
        }

        AthanaUnityPluginClass.CallStatic("requestReview");
    }

    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="key">事件名</param>
    /// <param name="type">事件类型，默认为 game</param>
    /// <param name="paramMap">事件参数</param>
    public static void SendEvent(string key, string type = "game", Dictionary<string, object>? paramMap = null)
    {
        AthanaLogger.D($"Calling SendEvent {key}");
        var paramsJavaObj = paramMap == null ? null : toJavaMap(paramMap);
        AthanaUnityPluginClass.CallStatic("sendEvent", key, type, paramsJavaObj);
    }

    /// <summary>
    /// 更新用户信息，适用于玩家首次登入后，补充游戏自有账户体系的账户ID
    /// </summary>
    /// <param name="customUserId">自定义用户ID - 游戏账户ID</param>
    /// <param name="extra">额外参数，可选</param>
    public static void UpdateUserInfo(long customUserId, Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D($"Calling UpdateUserInfo");
        var paramsJavaObj = extra == null ? null : toJavaMap(extra);
        AthanaUnityPluginClass.CallStatic("updateUserInfo", customUserId, paramsJavaObj);
    }

    /// <summary>
    /// 获取应用语言
    /// </summary>
    /// <returns>如未设置可能返回 null </returns>
    public static String? GetAppLanguage()
    {
        return AthanaUnityPluginClass.CallStatic<String?>("getAppLanguage");
    }

    /// <summary>
    /// 获取系统语言
    /// </summary>
    /// <returns>返回系统层级设置的语言</returns>
    public static String GetSysLanguage()
    {
        return AthanaUnityPluginClass.CallStatic<String>("getSysLanguage");
    }

    /// <summary>
    /// 跳转到应用商店详情页
    /// </summary>
    public static void OpenStoreDetail()
    {
        AthanaUnityPluginClass.CallStatic("openStoreDetail");
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
            var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
            AthanaCallbacks.SendAdLoadedEvent(adObj);
        }

        void onLoadFailed(string ad, string? error)
        {
            var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
            var errorObj = error == null ? null : JsonConvert.DeserializeObject<AdError>(error);
            AthanaCallbacks.SendAdLoadFailedEvent(adObj, errorObj);
        }

        void onDisplayed(string ad)
        {
            var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
            AthanaCallbacks.SendAdDisplayedEvent(adObj);
        }

        void onDisplayFailed(string ad, string? error)
        {
            var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
            var errorObj = error == null ? null : JsonConvert.DeserializeObject<AdError>(error);
            AthanaCallbacks.SendAdDisplayFailedEvent(adObj, errorObj);
        }

        void onRewarded(string ad)
        {
            var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
            AthanaCallbacks.SendAdRewardedEvent(adObj);
        }

        void onClick(string ad)
        {
            var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
            AthanaCallbacks.SendAdClickEvent(adObj);
        }

        void onClosed(string ad)
        {
            var adObj = JsonConvert.DeserializeObject<ProxyAd>(ad);
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