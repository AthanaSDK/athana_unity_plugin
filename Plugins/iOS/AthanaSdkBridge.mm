#import <UIKit/UIKit.h>
#import <UnityFramework/UnityFramework-Swift.h>

extern "C" {

/// SDK回调函数类型
typedef void (*SdkCallback)(const char *);

typedef void (*AdEventCallback)(const char *, const char *, const char *);

typedef void (*PermissionCheckCallback)(const bool);

void setSdkCallback(SdkCallback callback) {
    [[AthanaLibrariesHeaderBridge shared]
     setSdkCallback:^(NSString *_Nonnull result) {
        callback([result UTF8String]);
    }];
}

void setAdEventCallback(AdEventCallback callback) {
    [[AthanaLibrariesHeaderBridge shared]
     setAdEventCallback:^(NSString *_Nonnull functionName,
                          NSString *_Nonnull ad,
                          NSString *_Nullable error) {
        callback([functionName UTF8String],
                 [ad UTF8String],
                 error == nil ? NULL : [error UTF8String]);
    }];
}

const char* MakeStringCopy(NSString *_Nullable nsString) {
    if (nsString == nil) return NULL;
    const char* cString = [nsString UTF8String];
    return strdup(cString);
}

/// 初始化
void athanaInitSdk(long appId, const char *appKey, const char *appSecret,
                   const char *accountConfigJSON, const char *adConfigJSON,
                   const char *conversionConfigJSON, bool testMode, bool debug,
                   bool readClipBoard) {
    
    NSString *key = appKey ? [NSString stringWithUTF8String:appKey] : @"";
    NSString *secret = appSecret ? [NSString stringWithUTF8String:appSecret] : @"";
    
    NSString *accountJSON = accountConfigJSON ? [NSString stringWithUTF8String:accountConfigJSON] : nil;
    NSString *adJSON = adConfigJSON ? [NSString stringWithUTF8String:adConfigJSON] : nil;
    NSString *convJSON = conversionConfigJSON ? [NSString stringWithUTF8String:conversionConfigJSON] : nil;
    
    [[AthanaLibrariesHeaderBridge shared] initializeSdkWithAppId:appId
                                                          appKey:key
                                                       appSecret:secret
                                               accountConfigJSON:accountJSON
                                                    adConfigJSON:adJSON
                                            conversionConfigJSON:convJSON
                                                        testMode:testMode
                                                           debug:debug
                                                   readClipBoard:readClipBoard];
}

/// 启动
void athanaStart(bool privacyGrant) {
    [[AthanaLibrariesHeaderBridge shared] startWithPrivacyGrant:privacyGrant];
}

/// 获取当前用户
void athanaGetCurrentUser() {
    [[AthanaLibrariesHeaderBridge shared] getCurrentUser];
}

/// 登录 - 游客
void athanaRegisterUser(int signInType, NSInteger customUserId, NSString *extra) {
    [[AthanaLibrariesHeaderBridge shared] registryUserWithSignInType:signInType
                                                        customUserId:customUserId
                                                               extra:extra];
}

/// 登录
void athanaSignIn(int signInType, NSInteger customUserId, NSString *extra) {
    [[AthanaLibrariesHeaderBridge shared] signInSignInType:signInType
                                              customUserId:customUserId
                                                     extra:extra];
}

/// 登录 - 使用内置登录UI
void athanaSignInWithUI(const int* enabledSignInTypes, const int size,
                        NSInteger customId, NSString *privacyUrl, NSString *termsUrl) {
    
    NSArray<NSNumber *> *types = nil;
    
    if (size > 0 && enabledSignInTypes != NULL) {
        NSMutableArray<NSNumber *> *mutableArray = [NSMutableArray arrayWithCapacity:size];
        for (int i = 0; i < size; i++) {
            [mutableArray addObject:@(enabledSignInTypes[i])];
        }
        types = [mutableArray copy];
    }
    
    [[AthanaLibrariesHeaderBridge shared] signInWithUIWithEnabledSignInTypes:types
                                                                customUserId:customId
                                                            privacyPolicyUrl:privacyUrl
                                                           termsOfServiceUrl:termsUrl];
}

/// 更新游戏用户ID
void athanaUpdateUserInfo(NSInteger customUserId) {
    [[AthanaLibrariesHeaderBridge shared] updateUserInfoWithCustomUserId:customUserId];
}

/// 登出
void athanaSignOut() {
    [[AthanaLibrariesHeaderBridge shared] signOut];
}

/// 获取绑定关系
void athanaQueryAllAccountBind() {
    [[AthanaLibrariesHeaderBridge shared] queryAllAccountBind];
}

/// 绑定
void athanaAccountBinding(int signInType) {
    [[AthanaLibrariesHeaderBridge shared] accountBindingWithSignInType:signInType];
}

/// 解绑
void athanaAccountUnbind(int signInType, NSString *openId) {
    [[AthanaLibrariesHeaderBridge shared] accountUnbindWithSignInType:signInType
                                                               openId:openId];
}

/// 查询商定是否可用
bool athanaStoreIsAvailable() {
    return [[AthanaLibrariesHeaderBridge shared] storeIsAvailable];
}

/// 查询商品信息
void athanaQueryProducts(const char** keys, int size) {
    NSMutableSet *keysSet = [NSMutableSet set];
    for (int i = 0; i < size; i++) {
        NSString *key = [NSString stringWithUTF8String:keys[i]];
        [keysSet addObject:key];
    }
    [[AthanaLibrariesHeaderBridge shared] queryProductsWithKeys:keysSet];
}

/// 购买商品
void athanaPurchase(const char *productId, NSInteger clientOrderId) {
    NSString *productIdStr = [NSString stringWithUTF8String:productId];
    [[AthanaLibrariesHeaderBridge shared] purchaseWithProductId:productIdStr
                                                  clientOrderId:clientOrderId];
}

/// 查询购买历史
void athanaQueryPurchaseHistory() {
    [[AthanaLibrariesHeaderBridge shared] queryPurchaseHistory];
}

/// 重新验证订单
void athanaVerifyOrder(const char *purchaseId) {
    NSString *idStr = [NSString stringWithUTF8String:purchaseId];
    [[AthanaLibrariesHeaderBridge shared] verifyOrderWithPurchaseId:idStr];
}

/// 发送事件
void athanaSendEvent(const char *key, const char *type, const char *paramMap) {
    NSString *keyStr = [NSString stringWithUTF8String:key];
    NSString *typeStr = [NSString stringWithUTF8String:type];
    NSString *paramMapStr = [NSString stringWithUTF8String:paramMap];
    [[AthanaLibrariesHeaderBridge shared] sendEventWithKey:keyStr
                                                      type:typeStr
                                                  paramMap:paramMapStr];
}

/// 应用内评分
void athanaRequestAppReview() {
    [[AthanaLibrariesHeaderBridge shared] requestAppReview];
}

/// 查询是否已授权发送通知权限
void athanaCheckPostNotificationPermission(PermissionCheckCallback callback) {
    [[AthanaLibrariesHeaderBridge shared] checkPostNotificationPermission:^(BOOL result) {
        callback(result);
    }];
}

/// 请求发送通知权限
void athanaRequestPostNotificationPermission() {
    [[AthanaLibrariesHeaderBridge shared] requestPostNotificationPermission];
}

/// 获取应用语言
const char * athanaGetAppLang() {
    NSString *result = [[AthanaLibrariesHeaderBridge shared] getAppLang];
    return MakeStringCopy(result);
}

/// 获取系统语言
const char * athanaGetSysLang() {
    NSString *result = [[AthanaLibrariesHeaderBridge shared] getSysLang];
    return MakeStringCopy(result);
}

/// 获取系统国家地区代码
const char * athanaGetSysCountryCode() {
    NSString *result = [[AthanaLibrariesHeaderBridge shared] getSysCountryCode];
    return MakeStringCopy(result);
}

/// 跳转到应用商店详情页
void athanaOpenAppStoreDetail(char* appId) {
    NSString *appIdStr = [NSString stringWithUTF8String:appId];
    [[AthanaLibrariesHeaderBridge shared] openAppStoreDetailWithAppId:appIdStr];
}

/// 请求通知权限

/// 加载应用启动广告
bool athanaLoadAppOpenAd(char* adUnitId) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    return [[AthanaLibrariesHeaderBridge shared] loadAppOpenAdWithAdUnitId:adUnitIdStr];
}

/// 判断应用启动广告是否加载完成
bool athanaIsReadyAppOpenAd(char* adUnitId) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    return [[AthanaLibrariesHeaderBridge shared] isReadyAppOpenAdWithAdUnitId:adUnitIdStr];
}

/// 展示应用启动广告
bool athanaShowAppOpenAd(char* adUnitId, char* placement) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    NSString *placementStr = [NSString stringWithUTF8String:placement];
    return [[AthanaLibrariesHeaderBridge shared] showAppOpenAdWithAdUnitId:adUnitIdStr
                                                                 placement:placementStr];
}

/// 加载激励广告
bool athanaLoadRewardedAd(char* adUnitId) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    return [[AthanaLibrariesHeaderBridge shared] loadRewardedAdWithAdUnitId:adUnitIdStr];
}

/// 判断激励广告是否加载完成
bool athanaIsReadyRewardedAd(char* adUnitId) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    return [[AthanaLibrariesHeaderBridge shared] isReadyRewardedAdWithAdUnitId:adUnitIdStr];
}

/// 展示激励广告
bool athanaShowRewardedAd(char* adUnitId, char* placement) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    NSString *placementStr = [NSString stringWithUTF8String:placement];
    return [[AthanaLibrariesHeaderBridge shared] showRewardedAdWithAdUnitId:adUnitIdStr
                                                                  placement:placementStr];
}

/// 加载插屏广告
bool athanaLoadInterstitialAd(char* adUnitId) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    return [[AthanaLibrariesHeaderBridge shared] loadInterstitialAdWithAdUnitId:adUnitIdStr];
}

/// 判断插屏广告是否加载完成
bool athanaIsReadyInterstitialAd(char* adUnitId) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    return [[AthanaLibrariesHeaderBridge shared] isReadyInterstitialAdWithAdUnitId:adUnitIdStr];
}

/// 展示插屏广告
bool athanaShowInterstitialAd(char* adUnitId, char* placement) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    NSString *placementStr = [NSString stringWithUTF8String:placement];
    return [[AthanaLibrariesHeaderBridge shared] showInterstitialAdWithAdUnitId:adUnitIdStr
                                                                      placement:placementStr];
}

/// 创建横幅广告
bool athanaCreateBanner(char* adUnitId, char* placement, char* size, char* alignment) {
    NSString *adUnitIdStr = [NSString stringWithUTF8String:adUnitId];
    NSString *placementStr = [NSString stringWithUTF8String:placement];
    NSString *sizeStr = [NSString stringWithUTF8String:size];
    NSString *alignmentStr = [NSString stringWithUTF8String:alignment];
    return [[AthanaLibrariesHeaderBridge shared] createBannerWithAdUnitId:adUnitIdStr
                                                                placement:placementStr
                                                                     size:sizeStr
                                                                alignment:alignmentStr];
}

/// 显示横幅广告
bool athanaShowBanner() {
    return [[AthanaLibrariesHeaderBridge shared] showBanner];
}

/// 隐藏横幅广告
bool athanaHideBanner() {
    return [[AthanaLibrariesHeaderBridge shared] hideBanner];
}

/// 更改横幅广告尺寸
bool athanaUpdateBannerSize(char* size) {
    NSString *sizeStr = [NSString stringWithUTF8String:size];
    return [[AthanaLibrariesHeaderBridge shared] updateBannerSizeWithSize:sizeStr];
}

/// 更改横幅广告位置
bool athanaUpdateBannerAlignment(char* alignment) {
    NSString *alignmentStr = [NSString stringWithUTF8String:alignment];
    return [[AthanaLibrariesHeaderBridge shared] updateBannerAlignmentWithAlignment:alignmentStr];
}

/// 销毁横幅广告
bool athanaDestroyBanner() {
    return [[AthanaLibrariesHeaderBridge shared] destroyBanner];
}

// MARK: - Gaming Service - Leaderboard

/// 打开排行榜 UI
void athanaOpenLeaderboardUI(const char *leaderboardId, int playerScope, int timeScope) {
    NSString *leaderboardIdStr = leaderboardId ? [NSString stringWithUTF8String:leaderboardId] : nil;
    [[AthanaLibrariesHeaderBridge shared] openLeaderboardUIWithLeaderboardId:leaderboardIdStr
                                                                 playerScope:playerScope
                                                                   timeScope:timeScope];
}

/// 获取排行榜信息
void athanaGetLeaderboardInfo(const char *leaderboardId) {
    NSString *leaderboardIdStr = leaderboardId ? [NSString stringWithUTF8String:leaderboardId] : nil;
    [[AthanaLibrariesHeaderBridge shared] getLeaderboardInfoWithLeaderboardId:leaderboardIdStr];
}

/// 提交分数
void athanaSubmitScore(const char *leaderboardId, int score, int context) {
    NSString *leaderboardIdStr = [NSString stringWithUTF8String:leaderboardId];
    [[AthanaLibrariesHeaderBridge shared] submitScoreWithLeaderboardId:leaderboardIdStr
                                                                 score:score
                                                              context:context];
}

/// 获取排行榜数据
void athanaLoadLeaderboardScores(const char *leaderboardId, int playerScope, int timeScope, int length) {
    NSString *leaderboardIdStr = [NSString stringWithUTF8String:leaderboardId];
    [[AthanaLibrariesHeaderBridge shared] loadLeaderboardScoresWithLeaderboardId:leaderboardIdStr
                                                                      playerScope:playerScope
                                                                        timeScope:timeScope
                                                                           length:length];
}

/// 加载更多排行榜数据
void athanaLoadMoreLeaderboardScores(const char *leaderboardId, int length) {
    NSString *leaderboardIdStr = [NSString stringWithUTF8String:leaderboardId];
    [[AthanaLibrariesHeaderBridge shared] loadMoreLeaderboardScoresWithLeaderboardId:leaderboardIdStr
                                                                              length:length];
}

/// 获取当前玩家分数
void athanaGetScore(const char *leaderboardId, int playerScope, int timeScope) {
    NSString *leaderboardIdStr = [NSString stringWithUTF8String:leaderboardId];
    [[AthanaLibrariesHeaderBridge shared] getScoreWithLeaderboardId:leaderboardIdStr
                                                       playerScope:playerScope
                                                         timeScope:timeScope];
}

// MARK: - Gaming Service - Achievement

/// 打开成就 UI
void athanaOpenAchievementUI() {
    [[AthanaLibrariesHeaderBridge shared] openAchievementUI];
}

/// 获取成就列表
void athanaGetAchievementList() {
    [[AthanaLibrariesHeaderBridge shared] getAchievementList];
}

/// 解锁成就
void athanaUnlockAchievement(const char *achievementId) {
    NSString *achievementIdStr = [NSString stringWithUTF8String:achievementId];
    [[AthanaLibrariesHeaderBridge shared] unlockAchievementWithAchievementId:achievementIdStr];
}

/// 更新成就进度
void athanaUpdateAchievementProgress(const char *achievementId, int currentValue) {
    NSString *achievementIdStr = [NSString stringWithUTF8String:achievementId];
    [[AthanaLibrariesHeaderBridge shared] updateAchievementProgressWithAchievementId:achievementIdStr
                                                                       currentValue:currentValue];
}

// MARK: - Gaming Service - Friend

/// 申请好友列表权限
void athanaRequestFriendListPermission() {
    [[AthanaLibrariesHeaderBridge shared] requestFriendListPermission];
}

/// 获取好友列表
void athanaLoadFriendList(int length) {
    [[AthanaLibrariesHeaderBridge shared] loadFriendListWithLength:length];
}

/// 加载更多好友列表
void athanaLoadMoreFriendList(int length) {
    [[AthanaLibrariesHeaderBridge shared] loadMoreFriendListWithLength:length];
}

// MARK: - Gaming Service - Player

/// 打开玩家资料 UI
void athanaOpenPlayerProfileUI(const char *playerId) {
    NSString *playerIdStr = [NSString stringWithUTF8String:playerId];
    [[AthanaLibrariesHeaderBridge shared] openPlayerProfileUIWithPlayerId:playerIdStr];
}

}