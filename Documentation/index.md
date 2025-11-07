# Athana Unity Plugin

## 集成配置

1. 确认 Edit -> Project Settings -> Player -> Settings for Android -> Publishing Settings -> Build 中，勾选：
	- Custom Launcher Gradle Template
	- Custom Base Gradle Template

2. 编辑 ${UNITY_PROJEECT}\Assets\Plugins\Android\baseProjectTemplate.gradle

```
plugins {
    ...

    // 插入插件声明
    id "org.jetbrains.kotlin.android"  version "2.0.21" apply false
    id "com.google.gms.google-services" version "4.4.2" apply false
    id "com.google.firebase.crashlytics" version "3.0.4" apply false
    
    **BUILD_SCRIPT_DEPS**
}

...
```


3. 编辑 ${UNITY_PROJEECT}\Assets\Plugins\Android\launcherTemplate.gradle

```
apply plugin: 'com.android.application'
// 插入插件应用
apply plugin: 'org.jetbrains.kotlin.android'
apply from: 'athana_options.gradle'

...
```


4. 进入配置面板（Unity Editor -> Assets -> Athana -> Sdk Configuration），填选数据

![[Pasted image 20251103161710.png]]

## 初始化

```csharp
using Athana;
using Athana.Callbacks;
using Athana.Api;
using static Athana.Api.AthanaInterface;
using static Athana.Callbacks.AthanaCallbacks;

public class MainSceneController : MonoBehaviour
{
    void Start()
    {
        ...
        InitAthanaSdk()
        ...
        // 在合适的时机(隐私政策确认后、用户注册账户时)，启动 Athana
        AthanaSdk.Start(false);
    }

    private void InitAthanaSdk()
    {
        long appId = 00000000000;
        string appKey = "******************";
        string appSecret = "*****************************";

        // 三方SDK配置
        string appsFlyerDevKey = "******************";
        string maxDevKey = "******************";
        string googleWebClintId = "******************";
        
	    // 如果需要开启调试或内购测试模式，请开启对应参数
        bool testMode = false; // 内购测试模式
        bool debug = false; // 调试模式
		
		AthanaSdk.Initialize(appId, appKey, appSecret, testMode: testMode, debug: debug, serviceConfig: new()
		{
		    AccountConfig = new()
		    {
		        googleWebClientId = googleWebClintId,
		    },
		    AdServiceConfigs = new()
		    {
		        max = new()
		        {
		            sdkKey = maxDevKey,
		            debug = true
		        }
		    },
		    ConversionServiceConfigs = new()
		    {
		        appsflyer = new()
		        {
		            sdkKey = appsFlyerDevKey,
		        }
		    }
		});

    }
}
```


## 功能调用

### 注册平台账户


```csharp
using Athana;
using Athana.Callbacks;
using Athana.Api;
using static Athana.Api.AthanaInterface;
using static Athana.Callbacks.AthanaCallbacks;

private Action<SdkCallback<AccountInfo?>>? onCurrentUserResult;
private Action<SdkCallback<AccountInfo>>? onRegistryUserResult;

// 判断当前注册状态
public void CurrentUser()
{
    if (onCurrentUserResult == null)
    {
        onCurrentUserResult = (SdkCallback<AthanaInterface.AccountInfo?> result) =>
        {
            if (result.isSuccess())
            {
                var data = result.data;
                if (data == null)
                {
                    // 未登录或凭证失效
                    RegistryUser()
                }
                else
                {
                    // 已登录
                    // 进入游戏
                }
            }
            else
            {
                // 调用失败
                Debug.Log("Failed to get current user. error: " + result.error?.message);
            }
        };
        AthanaCallbacks.OnCurrentUserResult += onCurrentUserResult;
    }
    
    Athana.CurrentUser();
}

// 注册用户
public void RegistryUser()
{
    if (onRegistryUserResult == null)
    {
        onRegistryUserResult = (SdkCallback<AthanaInterface.AccountInfo> result) =>
        {
            if (result.isSuccess())
            {
                var data = result.data;
                // 用户注册成功
                Debug.Log("User(" + data.userId + ") register is successed.");
            }
            else
            {
                // 调用失败
                Debug.Log("Failed to register user. error: " + result.error?.message);
            }

        };
        AthanaCallbacks.OnRegistryUserResult += onRegistryUserResult;
    }
    var extra = new Dictionary<string, object>();
    Athana.RegistryUser(extra: extra);
}

```


### 三方登录

```csharp
using Athana;
using Athana.Callbacks;
using Athana.Api;
using static Athana.Api.AthanaInterface;
using static Athana.Callbacks.AthanaCallbacks;

private Action<SdkCallback<AccountInfo>>? onSignInResult;
private Action<SdkCallback<AccountInfo>>? onSignInWithUIResult;

// 选择指定登录方式
public void SignIn(AthanaInterface.SignInType type)
{
    if (onSignInResult == null)
    {
        onSignInResult = (SdkCallback<AthanaInterface.AccountInfo> result) =>
        {
            if (result.isSuccess())
            {
                var data = result.data;
                Debug.Log("User(" + data.userId + ") Sign-In is successed.");
                // 登录成功
            }
            else
            {
                Debug.Log("SignIn is failure. error: " + result.error?.message);
                // 调用失败
            }

        };
        AthanaCallbacks.OnSignInResult += onSignInResult;
    }

    Athana.SignIn(signInType: type);
}

// 使用内置登录界面
public void SignInWithUI()
{
    if (onSignInWithUIResult == null)
    {
        onSignInWithUIResult = (SdkCallback<AthanaInterface.AccountInfo> result) =>
        {
            if (result.isSuccess())
            {
                var data = result.data;
                Debug.Log("User(" + data.userId + ") Sign-In is successed.");
                // 登录成功
            }
            else
            {
                Debug.Log("SignIn is failure. error: " + result.error?.message);
                // 调用失败
            }

        };
        AthanaCallbacks.OnSignInWithUIResult += onSignInWithUIResult;
    }

    Athana.SignInWithUI();
}

```


### 广告

启动

```csharp
using Athana;
using Athana.Callbacks;
using Athana.Api;
using static Athana.Api.AthanaInterface;
using static Athana.Callbacks.AthanaCallbacks;

private Action<ProxyAd>? onAdLoadedEvent = null;
private Action<ProxyAd, AdError>? onLoadFailedEvent = null;

public void LoadAppOpenAd()
{
    if (onAdLoadedEvent == null)
    {
        onAdLoadedEvent = (ad) =>
        {
            // 加载成功
            Debug.Log("Load AppOpen Ad is successed.");
        };
        AppOpenAd.OnLoadedEvent += onAdLoadedEvent;
    }
    if (onLoadFailedEvent == null)
    {
        onLoadFailedEvent = (ad, error) =>
        {
            // 加载失败
            Debug.Log("Load AppOpen Ad is failure.");
        };
        AppOpenAd.OnLoadFailedEvent += onLoadFailedEvent;
    }
    AthanaSdk.LoadAppOpenAd(AppOpenAdUnitId);
}

public void ShowAppOpenAd()
{
    if (AthanaSdk.IsReadyAppOpenAd(AppOpenAdUnitId))
    {
        AthanaSdk.ShowAppOpenAd(AppOpenAdUnitId, placement: "AppOpenAd");
    }
    else
    {
        LoadAppOpenAd();
    }
}
```

插屏

```csharp
using Athana;
using Athana.Callbacks;
using Athana.Api;
using static Athana.Api.AthanaInterface;
using static Athana.Callbacks.AthanaCallbacks;

private Action<ProxyAd>? onAdLoadedEvent = null;
private Action<ProxyAd, AdError>? onLoadFailedEvent = null;

public void LoadRewardedAd()
{
    if (onAdLoadedEvent == null)
    {
        onAdLoadedEvent = (ad) =>
        {
            // 加载成功
            Debug.Log("Load Interstitial Ad is successed.");
        };
        AppOpenAd.OnLoadedEvent += onAdLoadedEvent;
    }
    if (onLoadFailedEvent == null)
    {
        onLoadFailedEvent = (ad, error) =>
        {
            // 加载失败
            Debug.Log("Load Interstitial Ad is failure.");
        };
        AppOpenAd.OnLoadFailedEvent += onLoadFailedEvent;
    }
    AthanaSdk.LoadInterstitialAd(InterstitialAdUnitId);
}

public void ShowInterstitialAd()
{
    if (AthanaSdk.IsReadyInterstitialAd(InterstitialAdUnitId))
    {
        AthanaSdk.ShowInterstitialAd(InterstitialAdUnitId, placement: "InterstitialAd");
    }
    else
    {
        LoadInterstitialAd();
    }
}
```

激励

```csharp
using Athana;
using Athana.Callbacks;
using Athana.Api;
using static Athana.Api.AthanaInterface;
using static Athana.Callbacks.AthanaCallbacks;

private Action<ProxyAd>? onAdLoadedEvent = null;
private Action<ProxyAd, AdError>? onLoadFailedEvent = null;

public void LoadRewardedAd()
{
    if (onAdLoadedEvent == null)
    {
        onAdLoadedEvent = (ad) =>
        {
            // 加载成功
            Debug.Log("Load Rewarded Ad is successed.");
        };
        AppOpenAd.OnLoadedEvent += onAdLoadedEvent;
    }
    if (onLoadFailedEvent == null)
    {
        onLoadFailedEvent = (ad, error) =>
        {
            // 加载失败
            Debug.Log("Load Rewarded Ad is failure.");
        };
        AppOpenAd.OnLoadFailedEvent += onLoadFailedEvent;
    }
    AthanaSdk.LoadRewardedAd(RewardedAdUnitId);
}

public void ShowRewardedAd()
{
    if (AthanaSdk.IsReadyRewardedAd(RewardedAdUnitId))
    {
        AthanaSdk.ShowRewardedAd(RewardedAdUnitId, placement: "RewardedAd");
    }
    else
    {
        LoadRewardedAd();
    }
}
```

横幅

```csharp
using Athana;
using Athana.Api;

private BannerAd? _BannerAd = null;

public void CreateBanner()
{
    _BannerAd?.Destroy();
    _BannerAd = AthanaSdk.CreateBanner(BannerAdUnitId, AdSize.fullWidth(60), "Banner", AdAlignment.TOP_CENTER);
}

public void HideBanner()
{
    _BannerAd?.Hide();
}

public void ShowBanner()
{
    _BannerAd?.Show();
}

```

### 支付

```csharp
using Athana;
using Athana.Callbacks;
using Athana.Api;
using static Athana.Api.AthanaInterface;
using static Athana.Callbacks.AthanaCallbacks;

// 内购商品详情列表
private List<IapProduct> products = new List<IapProduct>();

public void QueryProducts()
{
    if (onQueryProductsResult == null)
    {
        onQueryProductsResult = (SdkCallback<List<IapProduct>> result) =>
        {
            if (result.isSuccess())
            {
                var data = result.data;
                if (data == null || data.Count == 0)
                {
                    // 未查询到结果
                }
                else
                {
	                // 持久化查询结果
                    products.AddRange(data);
                    foreach (var product in data)
                    {
                        // 价格回显
                    }
                }
            }
            else
            {
                // 调用失败
                Debug.Log("Failed to query products. error: " + result.error?.message);
            }

        };
        AthanaCallbacks.OnQueryProductsResult += onQueryProductsResult;
    }
    
    var keys = new HashSet<string>();
    // 添加需要查询的商品 Key
    keys.Add("*********");
    ...
    AthanaSdk.QueryProducts(keys);
}

public void Purchase(IapProduct product)
{
    if (onPurchaseResult == null)
    {
        onPurchaseResult = (SdkCallback<object?> result) =>
        {
            if (result.isSuccess())
            {
                // 购买成功
            }
            else
            {
                // 调用失败
                Debug.Log("Failed to Purchase. error: " + result.error?.message);
            }

        };
        AthanaCallbacks.OnPurchaseResult += onPurchaseResult;
    }
    
    AthanaSdk.Purchase(product);
}

public void QueryPurchaseHistory()
{
    if (onQueryPurchaseHistoryResult == null)
    {
        onQueryPurchaseHistoryResult = (SdkCallback<List<IapPurchase>> result) =>
        {
            if (result.isSuccess())
            {
                var data = result.data;
                // 只包含未确认订单、非一次性消耗类商品订单以及有效期内的订阅订单
                foreach (var purchase in data)
                {
                    Debug.Log("Purchase: " + purchase.purchaseId + ", Product: " + purchase.productId + ", finished: " + purchase.isAcknowledged);
                }
            }
            else
            {
                // 调用失败
                Debug.Log("Failed to query PurchaseHistory. error: " + result.error?.message);
            }

        };
        AthanaCallbacks.OnQueryPurchaseHistoryResult += onQueryPurchaseHistoryResult;
    }

    AthanaSdk.QueryPurchaseHistory();
}
```