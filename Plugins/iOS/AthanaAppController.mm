#import "UnityAppController.h"
#import <UnityFramework/UnityFramework-Swift.h>

@interface AthanaAppController : UnityAppController
@end

@implementation AthanaAppController

- (BOOL)application:(UIApplication *)application
    didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
  [super application:application didFinishLaunchingWithOptions:launchOptions];
  [LaunchDataManager setLaunchOptions:launchOptions];
  return YES;
}

// Athana: 外部调用回调
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options {
    return [[AthanaLibrariesHeaderBridge shared] application:application open:url options:options];
}

// Athana: 推送Token注册（需要集成推送则需要添加）
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken {
    [[AthanaLibrariesHeaderBridge shared] application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(AthanaAppController);