using Athana.Callbacks;
using System;
using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

#nullable enable
namespace Athana.Api
{
    public abstract class AthanaInterface
    {
        internal static bool DebugMode
        {
            get; set;
        }

        /// <summary>
        /// 登录方式
        /// </summary>
        public enum SignInType
        {
            /// <summary>
            /// 游客
            /// </summary>
            ANONYMOUS,

            /// <summary>
            /// Google 账户
            /// </summary>
            GOOGLE,

            /// <summary>
            /// Facebook 账户
            /// </summary>
            FACEBOOK,

            /// <summary>
            /// Google Play 游戏登录
            /// </summary>
            GOOGLE_PLAY_GAMES,

            /// <summary>
            /// 自校验登录
            /// </summary>
            BY_CLIENT_SELF
        }

        /// <summary>
        /// 账户信息
        /// </summary>
        [Serializable]
        public class AccountInfo
        {
            /// <summary>
            /// 平台用户Id
            /// </summary>
            public long userId;

            /// <summary>
            /// 平台用户访问凭证
            /// </summary>
            public string accessToken;

            /// <summary>
            /// 登录方式
            /// </summary>
            public SignInType signInType;

            /// <summary>
            /// 第三方账户 OpenID
            /// </summary>
            public string? triOpenId;

            /// <summary>
            /// 第三方账户访问凭证
            /// </summary>
            public string? triAccessToken;

            /// <summary>
            /// 用户属性
            /// </summary>
            public UserProperty userProperty;

        }

        /// <summary>
        /// 账户属性
        /// </summary>
        [Serializable]
        public class UserProperty
        {
            /// <summary>
            /// 昵称，来源于第三方账号
            /// </summary>
            public string? nickname;

            /// <summary>
            /// 邮箱
            /// </summary>
            public string? email;

            /// <summary>
            /// 电话
            /// </summary>
            public string? phone;

            /// <summary>
            /// 账户头像图片链接
            /// </summary>
            public string? avatarUrl;

            /// <summary>
            /// 额外属性
            /// </summary>
            public Dictionary<string, object>? extra;

        }

        /// <summary>
        /// 账号绑定状态
        /// </summary>
        [Serializable]
        public class TriAccountBindMap
        {
            /// <summary>
            /// 游客绑定状态，如未绑定则为 null
            /// </summary>
            public TriAccount? Tourist;

            /// <summary>
            /// Facebook 绑定状态，如未绑定则为 null
            /// </summary>
            public TriAccount? Facebook;

            /// <summary>
            /// Apple 绑定状态，如未绑定则为 null
            /// </summary>
            public TriAccount? Apple;

            /// <summary>
            /// Google 绑定状态，如未绑定则为 null
            /// </summary>
            public TriAccount? Google;

            /// <summary>
            /// Google Play Games 绑定状态，如未绑定则为 null
            /// </summary>
            public TriAccount? GoogleGameV2;

            /// <summary>
            /// Firebase Auth 绑定状态，如未绑定则为 null
            /// </summary>
            public TriAccount? Firebase;
        }

        /// <summary>
        /// 三方账号绑定信息
        /// </summary>
        public class TriAccount
        {

            /// <summary>
            /// 昵称
            /// </summary>
            public string nick_name;

            /// <summary>
            /// OpenID
            /// </summary>
            public string open_id;

            /// <summary>
            /// 绑定时间（10位时间戳）
            /// </summary>
            public int bing_time;

            /// <summary>
            /// 主从关系（master/slave）
            /// </summary>
            public string role;
        }

        /// <summary>
        /// 横幅广告展示位置
        /// </summary>
        public enum AdAlignment
        {
            /// <summary>
            /// 顶部靠左
            /// </summary>
            TOP_START,
            /// <summary>
            /// 顶部居中
            /// </summary>
            TOP_CENTER,
            /// <summary>
            /// 顶部靠右
            /// </summary>
            TOP_END,
            /// <summary>
            /// 底部靠左
            /// </summary>
            BOTTOM_START,
            /// <summary>
            /// 底部居中
            /// </summary>
            BOTTOM_CENTER,
            /// <summary>
            /// 底部靠右
            /// </summary>
            BOTTOM_END
        }

        /// <summary>
        /// 横幅广告尺寸
        /// </summary>
        [Serializable]
        public class AdSize
        {
            /// <summary>
            /// 宽度
            /// </summary>
            public int width;

            /// <summary>
            /// 高度
            /// </summary>
            public int height;

            /// <summary>
            /// 宽度自适应
            /// </summary>
            /// <param name="height">高度</param>
            /// <returns></returns>
            public static AdSize fullWidth(int height)
            {
                return new()
                {
                    width = -1,
                    height = height
                };
            }
        }

        /// <summary>
        /// 横幅广告
        /// </summary>
        public interface BannerAd
        {
            /// <summary>
            /// 展示
            /// </summary>
            void Show();

            /// <summary>
            /// 隐藏
            /// </summary>
            void Hide();

            /// <summary>
            /// 更改尺寸
            /// </summary>
            /// <param name="size"></param>
            void UpdateSize(AdSize size);

            /// <summary>
            /// 更改位置
            /// </summary>
            /// <param name="alignment"></param>
            void UpdateAlignment(AdAlignment alignment);

            /// <summary>
            /// 销毁
            /// </summary>
            void Destroy();
        }

        /// <summary>
        /// 广告类型
        /// </summary>
        public enum AdType
        {
            /// <summary>
            /// 未适配广告类型
            /// </summary>
            Unknown,

            /// <summary>
            /// 激励广告
            /// </summary>
            Rewarded,

            /// <summary>
            /// 横幅广告
            /// </summary>
            Banner,

            /// <summary>
            /// 原生广告
            /// </summary>
            Native,

            /// <summary>
            /// 插屏广告
            /// </summary>
            Interstitial,

            /// <summary>
            /// 应用启动广告
            /// </summary>
            AppOpen
        }

        /// <summary>
        /// 广告信息
        /// </summary>
        [Serializable]
        public class ProxyAd
        {
            /// <summary>
            /// 广告类型
            /// </summary>
            /// <value>
            /// 1 - Rewarded;
            /// 2 - MREC;
            /// 3 - Native;
            /// 4 - Interstitial;
            /// 5 - App Open;
            /// </value>
            public int type;

            /// <summary>
            /// 广告展示类型
            /// </summary>
            /// <value>
            /// 1 - FullScreen Ad 全屏;
            /// 2 - Banner Ad 横幅;
            /// </value>
            public int classify;

            /// <summary>
            /// 广告平台
            /// </summary>
            public string platform;

            /// <summary>
            /// 广告渠道
            /// </summary>
            public string source;

            /// <summary>
            /// 广告位
            /// </summary>
            public string adUnitId;

            /// <summary>
            /// 调用广告的位置
            /// </summary>
            public string? placement;

            /// <summary>
            /// 广告收益货币
            /// </summary>
            public string currency;

            /// <summary>
            /// 广告收益
            /// </summary>
            public double revenue;

            /// <summary>
            /// 广告收益数额精度
            /// </summary>
            public string? revenuePrecision;

            /// <summary>
            /// 获取对应的广告类型枚举
            /// </summary>
            /// <returns></returns>
            public AdType getAdType()
            {
                switch (type)
                {
                    case 1:
                        return AdType.Rewarded;
                    case 2:
                        return AdType.Banner;
                    case 3:
                        return AdType.Native;
                    case 4:
                        return AdType.Interstitial;
                    case 5:
                        return AdType.AppOpen;

                    default:
                        return AdType.Unknown;
                }
            }
        }

        /// <summary>
        /// 广告加载、展示错误信息
        /// </summary>
        [Serializable]
        public class AdError
        {
            public int code;
            public string message;
            public int networkErrorCode;
            public string networkErrorMessage;
        }

        /// <summary>
        /// 商品信息
        /// </summary>
        [Serializable]
        public class IapProduct
        {
            /// <summary>
            /// 商品主键
            /// </summary>
            public string key;

            /// <summary>
            /// 商品名称
            /// </summary>
            public string title;

            /// <summary>
            /// 商品介绍
            /// </summary>
            public string description;

            /// <summary>
            /// 价格，附加货币符号和货币代号。例如：US$ 9.99
            /// </summary>
            public string price;

            /// <summary>
            /// 原始价格，例如 ：9.99
            /// </summary>
            public double rawPrice;

            /// <summary>
            /// 货币代号，例如：USD
            /// </summary>
            public string currencyCode;

            /// <summary>
            /// 货币符号，例如：$
            /// </summary>
            public string currencySymbol;

            /// <summary>
            /// * 商品类型
            /// </summary>
            /// <value>
            /// 1 - 消耗类商品;
            /// 2 - 订阅类商品
            /// </value>
            public int productType;

            /// <summary>
            /// 订阅价格下标
            /// </summary>
            public int subsInex;
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public enum PurchaseState
        {
            /// <summary>
            /// 处理中
            /// </summary>
            PENDING,

            /// <summary>
            /// 已购买
            /// </summary>
            PURCHASED,

            /// <summary>
            /// 错误
            /// </summary>
            ERROR,

            /// <summary>
            /// 取消
            /// </summary>
            CANCEL,

            /// <summary>
            /// 恢复购买
            /// </summary>
            RESTORE
        }

        /// <summary>
        /// 订单信息
        /// </summary>
        [Serializable]
        public class IapPurchase
        {
            /// <summary>
            /// 商品Key
            /// </summary>
            public string productId;

            /// <summary>
            /// 订单状体
            /// </summary>
            public PurchaseState state;

            /// <summary>
            /// 商店订单流水号
            /// </summary>
            public string? purchaseId;

            /// <summary>
            /// 商店订单凭证
            /// </summary>
            public string? purchaseToken;

            /// <summary>
            /// 是否已确认
            /// </summary>
            public bool? isAcknowledged;

            /// <summary>
            /// 是否自动续订
            /// </summary>
            public bool? isAutoRenewing;
        }

        /// <summary>
        /// 服务配置
        /// </summary>
        public class AthanaServiceConfig
        {
            /// <summary>
            /// 三方账号服务配置
            /// </summary>
            public AccountServiceConfig? AccountConfig;
            /// <summary>
            /// 广告服务配置
            /// </summary>
            public AdServiceConfigs? AdServiceConfigs;
            /// <summary>
            /// 归因服务配置
            /// </summary>
            public ConversionServiceConfigs? ConversionServiceConfigs;
        }

        /// <summary>
        /// 三方账号服务配置
        /// </summary>
        [Serializable]
        public class AccountServiceConfig
        {
            /// <summary>
            /// Google 登录所需的 WebClientID
            /// </summary>
            public string googleWebClientId;

            /// <summary>
            /// 可供使用的登录方式，不设置表示全开
            /// </summary>
            public List<SignInType>? enabledSignInTypes = null;
        }

        /// <summary>
        /// 广告服务配置
        /// </summary>
        [Serializable]
        public class AdServiceConfigs
        {
            /// <summary>
            /// AppLovin MAX 配置
            /// </summary>
            public MaxAdServiceConfig? max;
        }

        /// <summary>
        /// AppLovin MAX 配置
        /// </summary>
        [Serializable]
        public class MaxAdServiceConfig
        {
            /// <summary>
            /// 开发者Key
            /// </summary>
            public string sdkKey;
            /// <summary>
            /// 隐私政策链接
            /// </summary>
            public string? privacyPolicyUrl;
            /// <summary>
            /// 用户协议链接
            /// </summary>
            public string? termsOfServiceUrl;
            /// <summary>
            /// 调试日志开关
            /// </summary>
            public bool debug = false;
            /// <summary>
            /// 预加载配置
            /// </summary>
            public string? preloadAds;
            /// <summary>
            /// 自动加载下一个广告开关
            /// </summary>
            public bool autoLoadNext = true;
        }

        /// <summary>
        /// 归因服务配置
        /// </summary>
        [Serializable]
        public class ConversionServiceConfigs
        {
            /// <summary>
            /// AppsFlyer配置
            /// </summary>
            public AppsFlyerServiceConfig appsflyer;
        }

        /// <summary>
        /// AppsFlyer配置
        /// </summary>
        [Serializable]
        public class AppsFlyerServiceConfig
        {
            /// <summary>
            /// 开发者key
            /// </summary>
            public string sdkKey;
            /// <summary>
            /// 是否手动启动
            /// </summary>
            public bool manualStart = false;
        }

        [Serializable]
        public class SdkResult
        {
            public string functionName;
            public string? data;
            public int errorCode;
            public string? message;
            public string? error;
        }


        /// <summary>
        /// SDK错误类型
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// SDK 未初始化
            /// </summary>
            SDK_NOT_INITIAL,

            /// <summary>
            /// 网络错误
            /// </summary>
            NETWORK_ERROR,

            /// <summary>
            /// SDK 请求失败
            /// </summary>
            SDK_REQUEST_ERROR,

            /// <summary>
            /// SDK 服务端返回格式异常
            /// </summary>
            SDK_RESPONSE_ERROR,

            /// <summary>
            /// SDK 操作被用户取消
            /// </summary>
            SDK_USER_CANCELLED,
        }

        /// <summary>
        /// SDK错误
        /// </summary>
        [Serializable]
        public class SdkError
        {
            /// <summary>
            /// 错误类型
            /// </summary>
            public ErrorType? type;

            /// <summary>
            /// 平台错误码
            /// 
            /// -1 则表示平台返回null值
            /// </summary>
            public int? errorCode;

            /// <summary>
            /// 错误信息
            /// </summary>
            public string? detailMessage;

            /// <summary>
            /// 额外信息
            /// </summary>
            public string? msg;

        }

        protected static void HandleSdkCallbackResult(string content)
        {
            try
            {
                AthanaCallbacks.DispatchEvent(content);
            }
            catch (Exception exception)
            {
                AthanaLogger.D("Event dispath failed");
                AthanaLogger.LogException(exception);
            }
        }
    }
}