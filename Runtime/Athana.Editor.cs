using System.Collections.Generic;
using UnityEngine;
using Athana.Api;
using System;

#nullable enable
public class AthanaUnityEditor : AthanaInterface
{
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
        DebugMode = debug;
        AthanaLogger.D("Calling Init on Unity Editor");
        AthanaLogger.D("serviceConfig: " + JsonUtility.ToJson(serviceConfig));
    }

    /// <summary>
    /// 启动SDK，请在SDK初始化后调用
    /// </summary>
    /// <param name="privacyGrant">用户对隐私协议的确认结果</param>
    public static void Start(bool privacyGrant)
    {
        AthanaLogger.D("Calling Start on Unity Editor");
    }

    /// <summary>
    /// 获取当前登入的账户信息，如返回 null 则表示未登入或凭证失效
    /// </summary>
    public static void CurrentUser()
    {
        AthanaLogger.D("Calling CurrentUser on Unity Editor");
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
        Dictionary<string, object>? extra = null
        )
    {
        AthanaLogger.D("Calling RegistryUser on Unity Editor");
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
        AthanaLogger.D("Calling SignIn on Unity Editor");
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
        AthanaLogger.D("Calling SignInWithUI on Unity Editor");
    }

    /// <summary>
    /// 登出
    /// </summary>
    public static void SignOut()
    {
        AthanaLogger.D("Calling SignOut on Unity Editor");
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
        AthanaLogger.D("Calling AccountBinding on Unity Editor");
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
        AthanaLogger.D("Calling AccountUnbind on Unity Editor");
    }

    /// <summary>
    /// 查询三方账号绑定信息
    /// </summary>
    /// <param name="extra">额外参数，可选参数</param>
    public static void QueryAllAccountBind(Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D("Calling QueryAllAccountBind on Unity Editor");
    }

    /// <summary>
    /// 加载应用启动广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadAppOpenAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadAppOpenAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 判断应用启动广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyAppOpenAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyAppOpenAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 展示应用启动广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns>调用结果</returns>
    public static bool ShowAppOpenAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowAppOpenAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 加载激励广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadRewardedAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadRewardedAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 判断激励广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyRewardedAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyRewardedAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 展示激励广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    /// <returns></returns>
    public static bool ShowRewardedAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowRewardedAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 加载插屏广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>调用结果</returns>
    public static bool LoadInterstitialAd(string adUnitId)
    {
        AthanaLogger.D("Calling LoadInterstitialAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 判断插屏广告是否已准备好展示
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <returns>true - 可展示；false - 不可展示</returns>
    public static bool IsReadyInterstitialAd(string adUnitId)
    {
        AthanaLogger.D("Calling IsReadyInterstitialAd on Unity Editor");
        return false;
    }

    /// <summary>
    /// 展示插屏广告
    /// </summary>
    /// <param name="adUnitId">广告位ID</param>
    /// <param name="placement">展示位置标识，例如：XXScene - 某场景、XXPage - 某页面</param>
    public static bool ShowInterstitialAd(string adUnitId, string? placement = null)
    {
        AthanaLogger.D("Calling ShowInterstitialAd on Unity Editor");
        return false;
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
        AthanaLogger.D("Calling CreateBanner on Unity Editor");
        return null;
    }

    /// <summary>
    /// 查询商店服务是否可用
    /// </summary>
    /// <returns></returns>
    public static bool StoreIsAvailable()
    {
        AthanaLogger.D("Calling StoreIsAvailable on Unity Editor");
        return false;
    }

    /// <summary>
    /// 查询商品信息
    /// </summary>
    /// <param name="keys">待查商品Key</param>
    public static void QueryProducts(HashSet<string> keys)
    {
        AthanaLogger.D("Calling QueryProducts on Unity Editor");
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
        AthanaLogger.D("Calling Purchase on Unity Editor");
    }

    /// <summary>
    /// 查询购买历史记录，只返回：未确认的订单、有效期内的订阅项、非一次性消耗类商品
    /// </summary>
    public static void QueryPurchaseHistory()
    {
        AthanaLogger.D("Calling QueryPurchaseHistory on Unity Editor");
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
        AthanaLogger.D("Calling VerifyOrder on Unity Editor");
    }

    /// <summary>
    /// 拉起应用内评价
    /// </summary>
    public static void RequestReview()
    {
        AthanaLogger.D("Calling RequestReview on Unity Editor");
    }

    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="key">事件名</param>
    /// <param name="type">事件类型，默认为 game</param>
    /// <param name="paramMap">事件参数</param>
    public static void SendEvent(string key, string type = "game", Dictionary<string, object>? paramMap = null)
    {

        AthanaLogger.D("Calling SendEvent on Unity Editor");
    }

    /// <summary>
    /// 更新用户信息，适用于玩家首次登入后，补充游戏自有账户体系的账户ID
    /// </summary>
    /// <param name="customUserId">自定义用户ID - 游戏账户ID</param>
    /// <param name="extra">额外参数，可选</param>
    public static void UpdateUserInfo(long customUserId, Dictionary<string, object>? extra = null)
    {
        AthanaLogger.D($"Calling UpdateUserInfo on Unity Editor");
    }

    /// <summary>
    /// 获取应用语言
    /// </summary>
    /// <returns>如未设置可能返回 null </returns>
    public static String? GetAppLanguage()
    {
        AthanaLogger.D($"Calling GetAppLanguage on Unity Editor");
        return null;
    }

    /// <summary>
    /// 获取系统语言
    /// </summary>
    /// <returns>返回系统层级设置的语言</returns>
    public static String GetSysLanguage()
    {
        AthanaLogger.D($"Calling GetSysLanguage on Unity Editor");
        return "en";
    }

    /// <summary>
    /// 获取系统国家地区代码
    /// </summary>
    /// <returns>返回系统层级设置的国家地区</returns>
    public static String GetSysCountry()
    {
        AthanaLogger.D($"Calling GetSysCountry on Unity Editor");
        return "US";
    }

    /// <summary>
    /// 跳转至商店详情页
    /// </summary>
    public static void OpenStoreDetail()
    {
        AthanaLogger.D($"Calling OpenStoreDetail on Unity Editor");
    }
}