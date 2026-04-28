//
//  AthanaLibrariesHeaderBridge.swift
//  Unity-iPhone
//
//  Created by CWJoy on 6/2/2026.
//
import AppTrackingTransparency
import AthanaCore
import AthanaSDK
#if canImport(AthanaAdapterApple)
import AthanaAdapterApple
#endif
#if canImport(AthanaAdapterAppLovin)
import AthanaAdapterAppLovin
#endif
#if canImport(AthanaAdapterAppsFlyer)
import AthanaAdapterAppsFlyer
#endif
#if canImport(AthanaAdapterFirebase)
import AthanaAdapterFirebase
#endif
#if canImport(AthanaAdapterGoogle)
import AthanaAdapterGoogle
#endif
#if canImport(AthanaAdapterGameCenter)
import AthanaAdapterGameCenter
#endif
#if canImport(AthanaAdapterMeta)
import AthanaAdapterMeta
#endif

private let TAG = "ATHANA-Unity"

@objc public class AthanaLibrariesHeaderBridge: NSObject {
    
    @objc public static let shared = AthanaLibrariesHeaderBridge()
    
    var sdkCallback: ((String) -> Void)? = nil
    
    var adEventCallback: ((String, String, String?) -> Void)? = nil
    
    var adListener: _AdListener? = nil
    
    override private init() {}
    
    @objc public func sdkVersion() -> String {
        return Athana.sdkVersion
    }
    
    @objc public func setSdkCallback(_ callback: @escaping (String) -> Void) {
        self.sdkCallback = callback
    }
    
    @objc public func setAdEventCallback(_ callback: @escaping (String, String, String?) -> Void) {
        self.adEventCallback = callback
        if (adListener == nil) {
            adListener = _AdListener()
            adListener?.callback = callback
        }
    }
    
    @objc public func initializeSdk(
        appId: Int,
        appKey: String,
        appSecret: String,
        accountConfigJSON: String?,
        adConfigJSON: String?,
        conversionConfigJSON: String?,
        testMode: Bool = false,
        debug: Bool = false,
        readClipBoard: Bool = false
    ) {
        let conConfigs: ConversionServiceConfigs?
        if let conJsonData = conversionConfigJSON?.data(using: .utf8) {
            conConfigs = try? JSONDecoder().decode(ConversionServiceConfigs.self, from: conJsonData)
        } else {
            conConfigs = nil
        }
        
        let adConfigs: AdServiceConfigs?
        if let adJsonData = adConfigJSON?.data(using: .utf8) {
            adConfigs = try? JSONDecoder().decode(AdServiceConfigs.self, from: adJsonData)
        } else {
            adConfigs = nil
        }
        
        let accountConfig: AccountServiceConfig?
        if let accountJsonData = accountConfigJSON?.data(using: .utf8) {
            accountConfig = try? JSONDecoder().decode(AccountServiceConfig.self, from: accountJsonData)
        } else {
            accountConfig = nil
        }
        
#if canImport(AthanaAdapterApple)
        Athana.shared.registerOf(
            service: AccountService.shared,
            provider: AppleAccountServiceProvider()
        )
#endif
        
#if canImport(AthanaAdapterAppLovin)
        if let maxAdConfig = adConfigs?.max {
            Athana.shared.registerOf(
                service: AdService.shared,
                provider: MaxAdServiceProvider(),
                config: MaxAdServiceProviderConfig(
                    devKey: maxAdConfig.sdkKey,
                    privacyPolicyUrl: maxAdConfig.privacyPolicyUrl,
                    termsOfServiceUrl: maxAdConfig.termsOfServiceUrl,
                    preloadAds: maxAdConfig.typedPreloadAds,
                    debug: maxAdConfig.debug
                )
            )
        }
#endif
        
#if canImport(AthanaAdapterAppsFlyer)
        if let appsFlyerConConfig = conConfigs?.appsflyer {
            // 配置 AppsFlyer
            Athana.shared.registerOf(
                service: ConversionService.shared,
                provider: AppsFlyerConversionServiceProvider(),
                config: AppsFlyerServiceProviderConfig(
                    devKey: appsFlyerConConfig.sdkKey,
                    appId: appsFlyerConConfig.appId,
                    debug: debug
                )
            )
        }
#endif
        
#if canImport(AthanaAdapterFirebase)
        Athana.shared.registerOf(
            service: ErrorRecordService.shared,
            provider: FirebaseErrorRecordServiceProvider()
        )
        Athana.shared.registerOf(
            service: LoggingService.shared,
            provider: FirebaseLoggingServiceProvider()
        )
        Athana.shared.registerOf(
            service: ConversionService.shared,
            provider: FirebaseConversionServiceProvider()
        )
        Athana.shared.registerOf(
            service: EventService.shared,
            provider: FirebaseEventServiceProvider()
        )
        Athana.shared.registerOf(
            service: PushService.shared,
            provider: FirebasePushServiceProvider()
        )
#endif
        
#if canImport(AthanaAdapterGoogle)
        Athana.shared.registerOf(
            service: AccountService.shared,
            provider: GoogleAccountServiceProvider()
        )
#endif

#if canImport(AthanaAdapterGameCenter)
        Athana.shared.registerOf(
            service: AccountService.shared,
            provider: GameCenterAccountServiceProvider()
        )
        if #available(iOS 14.0, *) {
            Athana.shared.registerOf(
                service: GamingService.shared,
                provider: GameCenterGamingServiceProvider()
            )
        }
#endif
        
#if canImport(AthanaAdapterMeta)
        Athana.shared.registerOf(
            service: AccountService.shared,
            provider: FacebookAccountServiceProvider()
        )
        Athana.shared.registerOf(
            service: ConversionService.shared,
            provider: MetaConversionServiceProvider()
        )
#endif
        
        // 配置 Athana
        Athana.shared.initialize(
            UIApplication.shared,
            config: AthanaConfig(
                appId: appId,
                appKey: appKey,
                appSecret: appSecret,
                debugMode: debug,
                readClipBoard: readClipBoard
            ),
            didFinishLaunchingWithOptions: LaunchDataManager.launchOptions
        )
    }
    
    @objc public func start(privacyGrant: Bool = false) {
        requestATT()
        Athana.shared.start(privacyGrant: privacyGrant)
        if let listener = adListener {
            Athana.shared.getAdService()?.setAdListener(listener: listener)
        }
    }
    
    /// 获取当前用户信息
    @objc public func getCurrentUser() {
        Task {
            let user = await Athana.shared.currentUser()
            let result = AthanaSdkResult(
                functionName: "onCurrentUserResult",
                data: user?.toJSONString(),
                message: nil,
                error: nil
            )
            sdkCallback?(result.toJSONString() ?? "")
        }
    }
    
    /// 登录 - 游客
    @objc public func registryUser(
        signInType: Int = SignInType.ANONYMOUS.rawValue,
        customUserId: Int = -1,
        extra: String? = nil
    ) {
        Task {
            let functionName = "onRegistryUserResult"
            guard let type = signInType.toSignInType() else {
                let error = AthanaError(
                    .SDK_REQUEST_ERROR,
                    message: "Invalid signInType: \(signInType)"
                )
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
                return
            }
            
            let extraDict: [String : Any]?
            if let extraData = extra?.data(using: .utf8) {
                let anyDict = try defaultJSONDecoder.decode([String : AnyCodable]?.self, from: extraData)
                extraDict = anyDict?.toDict()
            } else {
                extraDict = nil
            }
            
            do {
                let result = try await Athana.shared.signIn(
                    signInType: type,
                    customUserId: customUserId,
                    extra: extraDict
                )
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: result.toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 登录
    @objc public func signIn(
        signInType: Int = SignInType.ANONYMOUS.rawValue,
        customUserId: Int = -1,
        extra: String? = nil
    ) {
        Task {
            let functionName = "onSignInResult"
            guard let type = signInType.toSignInType() else {
                let error = AthanaError(
                    .SDK_REQUEST_ERROR,
                    message: "Invalid signInType: \(signInType)"
                )
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
                return
            }
            
            let extraDict: [String : Any]?
            if let extraData = extra?.data(using: .utf8) {
                let anyDict = try defaultJSONDecoder.decode([String : AnyCodable]?.self, from: extraData)
                extraDict = anyDict?.toDict()
            } else {
                extraDict = nil
            }
            
            do {
                let result = try await Athana.shared.signIn(
                    signInType: type,
                    customUserId: customUserId,
                    extra: extraDict
                )
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: result.toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 登录 - 使用内置登录UI
    @objc public func signInWithUI(
        enabledSignInTypes: [Int]? = nil,
        customUserId: Int = -1,
        privacyPolicyUrl: String? = nil,
        termsOfServiceUrl: String? = nil
    ) {
        Task {
            let functionName = "onSignInWithUIResult"
            do {
                let result = try await Athana.shared.signInWithUI(
                    ppUrl: privacyPolicyUrl,
                    usUrl: termsOfServiceUrl,
                    customUserId: customUserId,
                    enabledsTypes: enabledSignInTypes
                )
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: result.toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 更新游戏用户ID
    @objc public func updateUserInfo(customUserId: Int) {
        Task {
            let functionName = "onUpdateUserInfoResult"
            do {
                try await Athana.shared.updateUserInfo(customUserId: customUserId)
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: "true",
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 登出
    @objc public func signOut() {
        Task {
            let functionName = "onSignOutResult"
            do {
                try await Athana.shared.signOut()
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: nil,
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 绑定
    @objc public func accountBinding(signInType: Int) {
        Task {
            let functionName = "onAccountBindingResult"
            guard let type = signInType.toSignInType() else {
                let error = AthanaError(
                    .SDK_REQUEST_ERROR,
                    message: "Invalid signInType: \(signInType)"
                )
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
                return
            }
            do {
                let result = try await Athana.shared.accountBinding(
                    signInType: type
                )
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: "\(result)",
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 解绑
    @objc public func accountUnbind(signInType: Int, openId: String) {
        Task {
            let functionName = "onAccountUnbindResult"
            guard let type = signInType.toSignInType() else {
                let error = AthanaError(
                    .SDK_REQUEST_ERROR,
                    message: "Invalid signInType: \(signInType)"
                )
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
                return
            }
            do {
                let result = try await Athana.shared.accountUnbind(
                    signInType: type,
                    openId: openId
                )
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: "\(result)",
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 获取绑定关系
    @objc public func queryAllAccountBind() {
        Task {
            let functionName = "onQueryAllAccountBindResult"
            do {
                let result = try await Athana.shared.queryAllAccountBind()
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: result.toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 加载应用启动广告
    @objc public func loadAppOpenAd(adUnitId: String) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to loadAppOpenAd, AdService is nil")
            return false
        }
        return adService.loadAppOpenAd(adUnitId: adUnitId)
    }
    
    /// 查询应用启动广告是否加载完成
    @objc public func isReadyAppOpenAd(adUnitId: String) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to isReadyAppOpenAd, AdService is nil")
            return false
        }
        return adService.isReadyAppOpenAd(adUnitId: adUnitId)
    }
    
    /// 展示应用启动广告
    @objc public func showAppOpenAd(adUnitId: String, placement: String? = nil) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to showAppOpenAd, AdService is nil")
            return false
        }
        return adService.showAppOpenAd(adUnitId: adUnitId, placement: placement)
    }
    
    /// 加载奖励广告
    @objc public func loadRewardedAd(adUnitId: String) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to loadRewardedAd, AdService is nil")
            return false
        }
        return adService.loadRewardedAd(adUnitId: adUnitId)
    }
    
    /// 查询奖励广告是否加载完成
    @objc public func isReadyRewardedAd(adUnitId: String) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to isReadyRewardedAd, AdService is nil")
            return false
        }
        return adService.isReadyRewardedAd(adUnitId: adUnitId)
    }
    
    /// 展示奖励广告
    @objc public func showRewardedAd(adUnitId: String, placement: String? = nil) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to showRewardedAd, AdService is nil")
            return false
        }
        return adService.showRewardedAd(adUnitId: adUnitId, placement: placement)
    }
    
    /// 加载插屏广告
    @objc public func loadInterstitialAd(adUnitId: String) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to loadInterstitialAd, AdService is nil")
            return false
        }
        return adService.loadInterstitialAd(adUnitId: adUnitId)
    }
    
    /// 查询插屏广告是否加载完成
    @objc public func isReadyInterstitialAd(adUnitId: String) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to isReadyInterstitialAd, AdService is nil")
            return false
        }
        return adService.isReadyInterstitialAd(adUnitId: adUnitId)
    }
    
    /// 展示插屏广告
    @objc public func showInterstitialAd(adUnitId: String, placement: String? = nil) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to showInterstitialAd, AdService is nil")
            return false
        }
        return adService.showInterstitialAd(adUnitId: adUnitId, placement: placement)
    }
    
    private var _banner: AdBanner? = nil
    
    /// 创建横幅广告
    @objc public func createBanner(
        adUnitId: String,
        placement: String? = nil,
        size: String? = nil,
        alignment: String? = nil
    ) -> Bool {
        guard let adService = Athana.shared.getAdService() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to createBanner, AdService is nil")
            return false
        }
        if _banner != nil {
            LoggingService.shared.warn(tag: TAG, message: "Banner already created")
            return false
        }
        let adSize: AdSize
        if let sizeJsonData = size?.data(using: .utf8) {
            do {
                adSize = try defaultJSONDecoder.decode(AdSize.self, from: sizeJsonData)
            } catch {
                LoggingService.shared.warn(tag: TAG, message: "Failed to decode adSize on create banner")
                ErrorRecordService.shared.recordError(error)
                adSize = .adaptiveSize()
            }
        } else {
            adSize = .adaptiveSize()
        }
        let adAlignment: AdAlignment = alignment?.toAdAlignment() ?? AdAlignment.BOTTOM_CENTER
        
        _banner = adService.createBanner(
            adUnitId: adUnitId,
            placement: placement,
            size: adSize,
            alignment: adAlignment
        )
        return _banner != nil
    }
    
    /// 展示横幅广告
    @objc public func showBanner() -> Bool {
        guard let banner = _banner else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to showBanner, Banner is nil")
            return false
        }
        do {
            try banner.show()
            return true
        } catch {
            LoggingService.shared.warn(tag: TAG, message: "Failed to show banner")
            ErrorRecordService.shared.recordError(error)
            return false
        }
    }
    
    /// 隐藏横幅广告
    @objc public func hideBanner() -> Bool {
        guard let banner = _banner else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to hideBanner, Banner is nil")
            return false
        }
        banner.hide()
        return true
    }
    
    /// 更新横幅广告大小
    @objc public func updateBannerSize(size: String? = nil) -> Bool {
        guard let banner = _banner else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to updateBannerSize, Banner is nil")
            return false
        }
        let adSize: AdSize
        if let sizeJsonData = size?.data(using: .utf8) {
            do {
                adSize = try defaultJSONDecoder.decode(AdSize.self, from: sizeJsonData)
            } catch {
                LoggingService.shared.warn(tag: TAG, message: "Failed to decode adSize on update size")
                ErrorRecordService.shared.recordError(error)
                return false
            }
        } else {
            adSize = .adaptiveSize()
        }
        banner.updateSize(adSize)
        return true
    }
    
    /// 更新横幅广告对齐方式
    @objc public func updateBannerAlignment(alignment: String? = nil) -> Bool {
        guard let banner = _banner else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to updateBannerAlignment, Banner is nil")
            return false
        }
        guard let adAlignment = alignment?.toAdAlignment() else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to updateBannerAlignment, alignment is nil")
            return false
        }
        banner.updateAlignment(adAlignment)
        return true
    }
    
    /// 销毁横幅广告
    @objc public func destroyBanner() -> Bool {
        _banner?.destroy()
        _banner = nil
        return true
    }
    
    /// 查询本地商店是否可用
    @objc public func storeIsAvailable() -> Bool {
        return Athana.shared.storeIsAvailable()
    }
    
    /// 查询商品信息
    @objc public func queryProducts(keys: Set<String>) {
        Task {
            let functionName = "onQueryProductsResult"
            do {
                let result = try await Athana.shared.queryProducts(keys)
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: ItemPgk(items: result).toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 购买商品
    @objc public func purchase(
        productId: String,
        clientOrderId: Int = -1
    ) {
        Task {
            let functionName = "onPurchaseResult"
            do {
                // 查询商品
                let products = try await Athana.shared.queryProducts(Set([productId]))
                guard let product = products.first else {
                    let error = AthanaError(
                        .SDK_REQUEST_ERROR,
                        message: "Product not found: \(productId)"
                    )
                    error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
                    return
                }
                
                let result = try await Athana.shared.purchase(product, clientOrderId)
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: "\(result)",
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 查询购买历史
    @objc public func queryPurchaseHistory() {
        Task {
            let functionName = "onQueryPurchaseHistoryResult"
            do {
                let result = try await Athana.shared.queryPurchaseHistory()
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: ItemPgk(items: result).toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 验证订单
    @objc public func verifyOrder(purchaseId: String) {
        Task {
            let functionName = "onVerifyOrderResult"
            do {
                let queryResult = try await Athana.shared.queryPurchaseHistory()
                let matchResult = queryResult.first { $0.purchaseId == purchaseId }
                guard let purchase = matchResult else {
                    let error = AthanaError(
                        .SDK_REQUEST_ERROR,
                        message: "Purchase not found: \(purchaseId)"
                    )
                    error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
                    return
                }
                
                let result = try await Athana.shared.verifyOrder(purchase)
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: "\(result)",
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 发送事件
    @objc public func sendEvent(key: String, type: String = "game", paramMap: String? = nil) {
        
        let paramDict: [String : Any]?
        if let paramData = paramMap?.data(using: .utf8) {
            do {
                let anyDict = try defaultJSONDecoder.decode([String : AnyCodable]?.self, from: paramData)
                paramDict = anyDict?.toDict()
            } catch {
                LoggingService.shared.warn(tag: TAG, message: "[\(TAG)] Failed to decode paramMap: \(error) on Event: \(key)")
                paramDict = nil
            }
        } else {
            paramDict = nil
        }
        
        let event = EventsUtils.buildEvent(key, params: paramDict, type: type)
        Athana.shared.logEvent(event: event)
    }
    
    /// 应用内评分
    @objc public func requestAppReview() {
        Task { @MainActor in
            Athana.shared.requestReview()
            let sdkResult = AthanaSdkResult(
                functionName: "onRequestReviewResult",
                data: nil,
                message: nil,
                error: nil
            )
            sdkCallback?(sdkResult.toJSONString() ?? "")
        }
    }
    
    // MARK: - Gaming Service - Leaderboard
    
    private var playersCache: [String: PlayerProfile] = [:]

    private func cachePlayers(_ playerList: [PlayerProfile]?) {
        guard let list = playerList else { return }
        for player in list {
            playersCache[player.playerId] = player
        }
    }

    /// 打开排行榜 UI
    @objc public func openLeaderboardUI(
        leaderboardId: String?,
        playerScope: Int,
        timeScope: Int
    ) {
        let functionName = "onOpenLeaderboardUIResult"
        let scope = LeaderboardPlayerScope(rawValue: playerScope) ?? .ALL
        let time = LeaderboardTimeSpan(rawValue: timeScope) ?? .ALL_TIME
        
        Task { @MainActor in
            let result = await GamingService.shared.openLeaderboardUI(
                leaderboardId: leaderboardId,
                playerScope: scope,
                timeScope: time
            )
            let sdkResult = AthanaSdkResult(
                functionName: functionName,
                data: String(result),
                message: nil,
                error: nil
            )
            sdkCallback?(sdkResult.toJSONString() ?? "")
        }
    }
    
    /// 获取排行榜信息
    @objc public func getLeaderboardInfo(leaderboardId: String?) {
        Task {
            let functionName = "onGetLeaderboardInfoResult"
            
            do {
                let result = try await GamingService.shared.getLeaderboardInfo(leaderboardId: leaderboardId)
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: ItemPgk(items: result).toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 提交分数
    @objc public func submitScore(
        leaderboardId: String,
        score: Int,
        context: Int
    ) {
        Task {
            let functionName = "onSubmitScoreResult"
            
            do {
                let result = try await GamingService.shared.submitScore(
                    leaderboardId: leaderboardId,
                    score: score,
                    context: context
                )
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: String(result),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    private var lastLBLoadStart: Int = 1
    private var lastLBLoadPlayerScope: Int = 0
    private var lastLBLoadTimeScope: Int = 0

    /// 获取排行榜数据
    @objc public func loadLeaderboardScores(
        leaderboardId: String,
        playerScope: Int,
        timeScope: Int,
        length: Int
    ) {
        let functionName = "onLoadLeaderboardDataResult"
        lastLBLoadPlayerScope = playerScope
        lastLBLoadTimeScope = timeScope
        lastLBLoadStart = 1
        loadLeaderboardScores(
            functionName: functionName,
            leaderboardId: leaderboardId,
            playerScope: playerScope,
            timeScope: timeScope,
            length: length
        )
    }

    /// 加载更多排行榜数据
    @objc public func loadMoreLeaderboardScores(
        leaderboardId: String,
        length: Int
    ) {
        let functionName = "onLoadMoreLeaderboardDataResult"
        loadLeaderboardScores(
            functionName: functionName,
            leaderboardId: leaderboardId,
            playerScope: lastLBLoadPlayerScope,
            timeScope: lastLBLoadTimeScope,
            length: length
        )
    }

    private func loadLeaderboardScores(
        functionName: String,
        leaderboardId: String,
        playerScope: Int,
        timeScope: Int,
        length: Int
    ) {
        let start = lastLBLoadStart
        Task {
            let scope = LeaderboardPlayerScope(rawValue: playerScope) ?? .ALL
            let time = LeaderboardTimeSpan(rawValue: timeScope) ?? .ALL_TIME
            let range = NSRange(location: start, length: length)
            
            do {
                let result = try await GamingService.shared.loadLeaderboardScores(
                    leaderboardId: leaderboardId,
                    scope: scope,
                    timeScope: time,
                    range: range
                )
                
                cachePlayers(result.scoreList.map { $0.player})

                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: result.toJSONString(),
                    message: nil,
                    error: nil
                )

                if result.hasMore {
                    lastLBLoadStart += length
                }
                
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 获取当前玩家分数
    @objc public func getScore(
        leaderboardId: String,
        playerScope: Int,
        timeScope: Int
    ) {
        Task {
            let functionName = "onGetScoreResult"
            let scope = LeaderboardPlayerScope(rawValue: playerScope) ?? .ALL
            let time = LeaderboardTimeSpan(rawValue: timeScope) ?? .ALL_TIME
            
            do {
                let result = try await GamingService.shared.getScore(
                    leaderboardId: leaderboardId,
                    scope: scope,
                    timeScope: time
                )

                if let player = result?.player {
                    cachePlayers([player])
                }
                
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: result?.toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    // MARK: - Gaming Service - Achievement
    
    /// 打开成就 UI
    @objc public func openAchievementUI() {
        Task { @MainActor in
            let functionName = "onOpenAchievementUIResult"
            let result = await GamingService.shared.openAchievementUI()
            let sdkResult = AthanaSdkResult(
                functionName: functionName,
                data: String(result),
                message: nil,
                error: nil
            )
            sdkCallback?(sdkResult.toJSONString() ?? "")
        }
    }
    
    /// 获取成就列表
    @objc public func getAchievementList() {
        Task {
            let functionName = "onGetAchievementDataResult"
            
            do {
                let result = try await GamingService.shared.getAchievementList()
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: ItemPgk(items: result).toJSONString(),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 解锁成就
    @objc public func unlockAchievement(achievementId: String) {
        Task {
            let functionName = "onUnlockAchievementResult"
            
            do {
                let result = try await GamingService.shared.unlockAchievement(achievementId: achievementId)
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: String(result),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 更新分步成就进度
    @objc public func updateAchievementProgress(
        achievementId: String,
        currentValue: Int
    ) {
        Task {
            let functionName = "onUpdateAchievementProgressResult"
            
            do {
                let result = try await GamingService.shared.updateAchievementProgress(
                    achievementId: achievementId,
                    currentValue: currentValue
                )
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: String(result),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    // MARK: - Gaming Service - Friend
    
    /// 申请好友列表权限
    @objc public func requestFriendListPermission() {
        Task {
            let functionName = "onRequestFriendListPermissionResult"
            
            do {
                let result = try await GamingService.shared.requestFriendListPermission()
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: String(result),
                    message: nil,
                    error: nil
                )
                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    private var lastFriendsLoadStart: Int = 1

    /// 获取好友列表
    @objc public func loadFriendList(length: Int) {
        let functionName = "onLoadFriendsResult"
        lastFriendsLoadStart = 1
        loadFriendList(functionName: functionName, length: length)
    }

    /// 加载更多好友列表
    @objc public func loadMoreFriendList(length: Int) {
        let functionName = "onLoadMoreFriendsResult"
        loadFriendList(functionName: functionName, length: length)
    }

    private func loadFriendList(functionName: String, length: Int) {
        let start = lastFriendsLoadStart
        Task {
            let range = NSRange(location: start, length: length)
            
            do {
                let result = try await GamingService.shared.loadFriendList(range: range)
                let sdkResult = AthanaSdkResult(
                    functionName: functionName,
                    data: result.toJSONString(),
                    message: nil,
                    error: nil
                )
                cachePlayers(result.friends)

                if result.hasMore {
                    lastFriendsLoadStart = start + length
                }

                sdkCallback?(sdkResult.toJSONString() ?? "")
            } catch {
                error.sendUnity(functionName: functionName, sdkCallback: sdkCallback)
            }
        }
    }
    
    /// 打开玩家资料 UI
    @objc public func openPlayerProfileUI(playerId: String) {
        guard let player = playersCache[playerId] else {
            LoggingService.shared.warn(tag: TAG, message: "Failed to openPlayerProfileUI, PlayerProfile not found: \(playerId)")
            return
        }
        
        Task { @MainActor in
            await GamingService.shared.openPlayerProfileUI(player: player)
        }
    }

    /// 查询是否已授权发送通知权限
    @objc public func checkPostNotificationPermission(_ callback: @escaping (Bool) -> Void) {
        Task { @MainActor in
            let isAuthorized = await Athana.shared.checkNotificationPermission()
            callback(isAuthorized)
        }
    }

    /// 请求发送通知权限
    @objc public func requestPostNotificationPermission() {
        Athana.shared.requestNotifications()
    }
    
    @objc public func getAppLang() -> String? {
        return getAppLanguage()
    }
    
    @objc public func getSysLang() -> String {
        return getSysLanguage()
    }
    
    @objc public func getSysCountryCode() -> String? {
        return getSysCountry()
    }
    
    @objc public func openAppStoreDetail(appId: String) {
        guard let url = URL(string: "itms-apps://itunes.apple.com/app/id\(appId)") else {
            LoggingService.shared.warn(tag: TAG, message: "[\(TAG)] Failed to build app store url: \(appId)")
            return
        }
        UIApplication.shared.open(url)
    }
    
    @objc public func application(
        _ app: UIApplication,
        open url: URL,
        options: [UIApplication.OpenURLOptionsKey: Any] = [:]
    ) -> Bool {
        return Athana.shared.application(app, open: url, options: options)
    }
    
    @objc public func application(_ application: UIApplication, didRegisterForRemoteNotificationsWithDeviceToken deviceToken: Data) {
        Athana.shared.application(application, didRegisterForRemoteNotificationsWithDeviceToken: deviceToken)
    }
    
    private func requestATT() {
        Task {
            if #available(iOS 14.0, *) {
                var state = ATTrackingManager.trackingAuthorizationStatus
                switch state {
                case .authorized, .denied, .restricted:
                    break
                case .notDetermined:
                    state = await ATTrackingManager.requestTrackingAuthorization()
                    break
                default:
                    break
                }
            }
        }
    }
}

extension Error {
    
    func sendUnity(functionName: String, sdkCallback: ((String) -> Void)?) {
        ErrorRecordService.shared.recordError(self)
        let sdkResult = AthanaSdkResult(
            functionName: functionName,
            data: nil,
            message: nil,
            error: transformError(self)
        )
        sdkCallback?(sdkResult.toJSONString() ?? "")
    }
    
    private func transformError(_ error: any Error) -> String {
        if let ae = error as? AthanaError {
            return ae.toJSONString() ?? "{\"detailMessage\": \"\(ae.localizedDescription)\"}"
        }
        return "{\"detailMessage\": \"\(error.localizedDescription)\"}"
    }
}

extension Encodable {
    
    func toJSONString() -> String? {
        do {
            let jsonData = try defaultJsonEncoder.encode(self)
            return String(data: jsonData, encoding: .utf8)
        } catch {
            ErrorRecordService.shared.recordError(error)
            return nil
        }
    }
    
}

public class ItemPgk<T: Codable>: Codable {
    public let items: [T]
    
    public init(items: [T]) {
        self.items = items
    }
}

public enum AnyCodable: Codable {
    case string(String)
    case number(Double)
    case bool(Bool)
    case dictionary([String: AnyCodable])
    case array([AnyCodable])
    case nilValue
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.singleValueContainer()
        
        if let x = try? container.decode(String.self) {
            self = .string(x)
        } else if let x = try? container.decode(Double.self) {
            self = .number(x)
        } else if let x = try? container.decode(Bool.self) {
            self = .bool(x)
        } else if let x = try? container.decode([String: AnyCodable].self) {
            self = .dictionary(x)
        } else if let x = try? container.decode([AnyCodable].self) {
            self = .array(x)
        } else if container.decodeNil() {
            self = .nilValue
        } else {
            throw DecodingError.typeMismatch(AnyCodable.self, DecodingError.Context(codingPath: decoder.codingPath, debugDescription: "不支持的类型"))
        }
    }
    
    public func encode(to encoder: Encoder) throws {
        var container = encoder.singleValueContainer()
        switch self {
        case .string(let x): try container.encode(x)
        case .number(let x): try container.encode(x)
        case .bool(let x): try container.encode(x)
        case .dictionary(let x): try container.encode(x)
        case .array(let x): try container.encode(x)
        case .nilValue: try container.encodeNil()
        }
    }
    
    func take() -> Any? {
        return switch self {
        case .string(let x): x
        case .number(let x): x
        case .bool(let x): x
        case .dictionary(let x): x.mapValues { $0.take() }
        case .array(let x): x.map { $0.take() }
        case .nilValue: nil
        }
    }
}

extension [String: AnyCodable] {
    
    func toDict() -> [String: Any] {
        var newValues: [String: Any] = [:]
        self.forEach {
            let key = $0.key
            let value = $0.value
            
            switch value {
            case .nilValue:
                break
            default:
                newValues[key] = value.take()
                break
            }
        }
        return newValues
    }
    
}

class _AdListener: BaseAdServiceListener {

    var callback: ((String, String, String?) -> Void)? = nil
    
    private func send(event: String, ad: ProxyAd, error: AdError? = nil) {
        guard let cb = callback else {
            LoggingService.shared.debug(tag: TAG, message: "[\(TAG)] Failed to send \(event). not set callback in AdListener")
            return
        }
        
        if let adJson: String = ad.toJSONString() {
            let errorJson: String? = error?.toJSONString()
            if (error != nil && errorJson == nil) {
                LoggingService.shared.warn(tag: TAG, message: "[\(TAG)] Failed to encode error on send \(event)")
            }
            callback?(event, adJson, errorJson)
        } else {
            LoggingService.shared.warn(tag: TAG, message: "[\(TAG)] Failed to send \(event). failed to encode ad")
        }
    }

    override func onLoaded(ad: ProxyAd) {
        super.onLoaded(ad: ad)
        let event = "onLoaded"
        send(event: event, ad: ad)
    }

    override func onLoadFailed(ad: ProxyAd, error: AdError?) {
        super.onLoadFailed(ad: ad, error: error)
        let event = "onLoadFailed"
        send(event: event, ad: ad, error: error)
    }

    override func onDisplayed(ad: ProxyAd) {
        super.onDisplayed(ad: ad)
        let event = "onDisplayed"
        send(event: event, ad: ad)
    }

    override func onDisplayFailed(ad: ProxyAd, error: AdError?) {
        super.onDisplayFailed(ad: ad, error: error)
        let event = "onDisplayFailed"
        send(event: event, ad: ad, error: error)
    }

    override func onRewarded(ad: ProxyAd) {
        super.onRewarded(ad: ad)
        let event = "onRewarded"
        send(event: event, ad: ad)
    }

    override func onClick(ad: ProxyAd) {
        super.onClick(ad: ad)
        let event = "onClick"
        send(event: event, ad: ad)
    }

    override func onClosed(ad: ProxyAd) {
        super.onClosed(ad: ad)
        let event = "onClosed"
        send(event: event, ad: ad)
    }
}