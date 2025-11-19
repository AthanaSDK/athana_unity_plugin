using System;
using System.Collections.Generic;
using Athana.Callbacks;

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
        }

        public enum AdAlignment
        {
            TOP_START,
            TOP_CENTER,
            TOP_END,
            BOTTOM_START,
            BOTTOM_CENTER,
            BOTTOM_END
        }

        [Serializable]
        public class AdSize
        {
            public int width;

            public int height;

            public static AdSize fullWidth(int height)
            {
                return new()
                {
                    width = -1,
                    height = height
                };
            }
        }

        public interface BannerAd
        {
            void Show();

            void Hide();

            void UpdateSize(AdSize size);

            void UpdateAlignment(AdAlignment alignment);

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
            public int? subsInex;
        }

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

        public class AthanaServiceConfig
        {
            public AccountServiceConfig? AccountConfig;
            public AdServiceConfigs? AdServiceConfigs;
            public ConversionServiceConfigs? ConversionServiceConfigs;
        }

        [Serializable]
        public class AccountServiceConfig
        {
            public string googleWebClientId;
            public List<SignInType>? enabledSignInTypes = null;
        }

        [Serializable]
        public class AdServiceConfigs
        {
            public MaxAdServiceConfig? max;
        }

        [Serializable]
        public class MaxAdServiceConfig
        {
            public string sdkKey;
            public string? privacyPolicyUrl;
            public string? termsOfServiceUrl;
            public bool debug = false;
            public string? preloadAds;
            public bool autoLoadNext = true;
        }

        [Serializable]
        public class ConversionServiceConfigs
        {
            public AppsFlyerServiceConfig appsflyer;
        }

        [Serializable]
        public class AppsFlyerServiceConfig
        {
            public string sdkKey;
            public bool manualStart = false;
        }

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

        [Serializable]
        public class SdkError
        {
            public ErrorType type;
            public string message;
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