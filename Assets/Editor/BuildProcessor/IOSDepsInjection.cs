using System;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.Linq;

#nullable enable
class IOSDepsInjection
{

    private static readonly string FIREBASE_CONFIG_NAME = "GoogleService-Info.plist";

    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        SdkConfigData sdkConfig = SdkConfigData.ReadForFile();
        if (!sdkConfig.CheckIOSVersion())
        {
            throw new Exception($"SDK 版本过低，请升级至 {SdkConfigData.miniIOSSdkVersion} 或更高版本");
        }

        var firebaseImport = sdkConfig.ImportConversionFirebase() || sdkConfig.ImportPushFirebase();
        string pluginFirebaseConfigPath = Path.Combine(Application.dataPath, "Plugins", "iOS", FIREBASE_CONFIG_NAME);
        string projFirebaseConfigPath = Path.Combine(pathToBuiltProject, FIREBASE_CONFIG_NAME);
        if (firebaseImport)
        {
            if (!File.Exists(pluginFirebaseConfigPath))
            {
                UnityEngine.Debug.LogWarning($"未找到 {FIREBASE_CONFIG_NAME}，请确保将 {FIREBASE_CONFIG_NAME} 放入 Plugins/iOS 目录");
            }
            else
            {
                if (!File.Exists(projFirebaseConfigPath))
                {
                    File.Copy(pluginFirebaseConfigPath, projFirebaseConfigPath, true);
                    UnityEngine.Debug.Log($"{FIREBASE_CONFIG_NAME} 已复制到 Xcode 项目目录中");
                }
            }
        }

        // 确认 XCode 项目信息
        var projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject proj = new();
        proj.ReadFromFile(projPath);
        string targetGuid = proj.GetUnityMainTargetGuid();

        if (proj.FindFileGuidByProjectPath(FIREBASE_CONFIG_NAME) == null)
        {
            string firebaseGuid = proj.AddFile(FIREBASE_CONFIG_NAME, FIREBASE_CONFIG_NAME, PBXSourceTree.Source);
            proj.AddFileToBuild(targetGuid, firebaseGuid);
            UnityEngine.Debug.Log($"{FIREBASE_CONFIG_NAME} 已添加到 Xcode 项目索引");
        }

        string plistPath = pathToBuiltProject + "/Info.plist";
        PlistDocument plist = new();
        plist.ReadFromFile(plistPath);

        // Get root plist element
        PlistElementDict rootDict = plist.root;

        if (sdkConfig.ImportConversionFacebook())
        {
            rootDict.SetBoolean("FacebookAdvertiserIDCollectionEnabled", true);
            rootDict.SetString("FacebookAppID", sdkConfig.FacebookAppId);
        }

        if (sdkConfig.ImportAccount())
        {
            rootDict.SetString("FacebookClientToken", sdkConfig.FacebookClientToken);
            rootDict.SetString("FacebookDisplayName", Application.productName);
            PlistElementArray queriesSchemesArray;
            if (rootDict.values.ContainsKey("LSApplicationQueriesSchemes"))
            {
                queriesSchemesArray = rootDict["LSApplicationQueriesSchemes"].AsArray();
            }
            else
            {
                queriesSchemesArray = rootDict.CreateArray("LSApplicationQueriesSchemes");
            }
            bool fbApiExists = queriesSchemesArray.values.Any(e => e.AsString() == "fbapi");
            bool fbMsgApiExists = queriesSchemesArray.values.Any(e => e.AsString() == "fb-messenger-share-api");
            if (!fbApiExists)
            {
                queriesSchemesArray.AddString("fbapi");
            }
            if (!fbMsgApiExists)
            {
                queriesSchemesArray.AddString("fb-messenger-share-api");
            }

            PlistDocument? googleServicePlist = null;
            if (File.Exists(pluginFirebaseConfigPath))
            {
                googleServicePlist = new();
                googleServicePlist!.ReadFromFile(pluginFirebaseConfigPath);

                rootDict.SetString("GIDClientID", googleServicePlist!.root["CLIENT_ID"].AsString());
            }
            rootDict.SetString("GIDServerClientID", sdkConfig.GoogleWebClientId);

            PlistElementArray urlTypesArray;
            if (rootDict.values.ContainsKey("CFBundleURLTypes"))
            {
                urlTypesArray = rootDict["CFBundleURLTypes"].AsArray();
            }
            else
            {
                urlTypesArray = rootDict.CreateArray("CFBundleURLTypes");
            }
            if (urlTypesArray.values.Count == 0)
            {
                urlTypesArray.AddDict().CreateArray("CFBundleURLSchemes").AddString($"fb{sdkConfig.FacebookAppId}");
                if (googleServicePlist != null)
                {
                    urlTypesArray.AddDict().CreateArray("CFBundleURLSchemes").AddString(googleServicePlist.root["REVERSED_CLIENT_ID"].AsString());
                }
            }

            if (sdkConfig.ImportGamingGameCenter())
            {
                rootDict.SetString("NSGKFriendListUsageDescription", "");
            }

            plist.WriteToFile(plistPath);
        }

        // 根据 SDK 配置，插入对应的组件
        string entitlementsPath = "Unity-iPhone.entitlements";
        ProjectCapabilityManager capManager = new(projPath, entitlementsPath, "Unity-iPhone", targetGuid);
        capManager.AddInAppPurchase();

        if (sdkConfig.PushServiceEnabled)
        {
            capManager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
            capManager.AddPushNotifications(true);
            capManager.AddPushNotifications(false);
        }

        if (sdkConfig.ImportGamingGameCenter())
        {
            capManager.AddGameCenter();
        }

        if (sdkConfig.ImportAccount())
        {
            capManager.AddSignInWithApple();
        }

        try
        {
            capManager.WriteToFile();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to add capabilities: {e.Message}");
        }

        proj.WriteToFile(projPath);

        // 根据依赖管理方案，使用不同方式生成 xcworkspaace
        string xcworkspacePath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcworkspace");
        if (!Directory.Exists(xcworkspacePath))
        {
            string projectPath = "Unity-iPhone.xcodeproj";
            if (sdkConfig.IosDepsManager == 0) // SwiftPM
            {
                UnityEngine.Debug.Log("Using SwiftPM for iOS dependencies.");
                Directory.CreateDirectory(xcworkspacePath);

                string content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                         "<Workspace version = \"1.0\">\n" +
                         "   <FileRef location = \"container:" + projectPath + "\"></FileRef>\n" +
                         "</Workspace>";

                File.WriteAllText(Path.Combine(xcworkspacePath, "contents.xcworkspacedata"), content);

                UnityEngine.Debug.Log("Workspace created at: " + xcworkspacePath);
            }
            else if (sdkConfig.IosDepsManager == 1) // CocoaPods
            {
                UnityEngine.Debug.Log("Using CocoaPods for iOS dependencies.");
                // Process.Start();
                // TODO: 实现 CocoaPods 依赖管理
            }
            else
            {

            }
        }

    }

}