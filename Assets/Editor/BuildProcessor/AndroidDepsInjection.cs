using System.Collections.Generic;
using System.IO;
using Unity.Android.Gradle;
using UnityEditor.Android;
using UnityEngine;

#nullable enable
class AndroidDepsInjection : AndroidProjectFilesModifier
{
    private static readonly string SDK_DEPS_OPTION_NAME = "athana_options.gradle";
    private static readonly string FIREBASE_CONFIG_NAME = "google-services.json";
    private static readonly string _sdkDepsOptionBuildGradle = "launcher/" + SDK_DEPS_OPTION_NAME;
    private static readonly string _firebaseConfigJSON = "launcher/" + FIREBASE_CONFIG_NAME;

    private static SdkConfigData? SdkConfig;

    public override int callbackOrder { get { return 0; } }

    public override AndroidProjectFilesModifierContext Setup()
    {
        AndroidProjectFilesModifierContext projectFilesContext = new AndroidProjectFilesModifierContext();
        projectFilesContext.Outputs.AddBuildGradleFile(_sdkDepsOptionBuildGradle);

        projectFilesContext.Dependencies.DependencyFiles = new[] {
            Path.Combine("Assets", "Plugins", "Android", "athana-sdk-config.json")
        };
        if (SdkConfig != null)
        {
            var NewValue = SdkConfigData.ReadForFile();
            if (NewValue != SdkConfig)
            {
                SdkConfig = SdkConfigData.ReadForFile();
                Debug.Log("Athana-SdkConfig is changed");
            }
        }
        else
        {
            SdkConfig = SdkConfigData.ReadForFile();
        }
        if (SdkConfig.ImportConversionFirebase() || SdkConfig.ImportPushFirebase())
        {
            var sourceFile = Path.Combine("Assets", "Plugins", "Android", FIREBASE_CONFIG_NAME);
            if (File.Exists(sourceFile))
            {
                projectFilesContext.AddFileToCopy(sourceFile, _firebaseConfigJSON);
            }
            else
            {
                Debug.LogWarning($"{sourceFile} 文件不存在！");
            }
        }
        return projectFilesContext;
    }

    public override void OnModifyAndroidProjectFiles(AndroidProjectFiles projectFiles)
    {

        List<string> Components = Transfor(SdkConfig);

        var CustomGradleFile = new ModuleBuildGradleFile();

        // ----- Plugins
        if (SdkConfig.ImportConversionFirebase() || SdkConfig.ImportPushFirebase())
        {
            
            // 如果需要投放至 Google Ads，则需要添加 Google-Services 和 Firebase 插件
            CustomGradleFile.ApplyPluginList.AddPluginByName("com.google.gms.google-services");
            CustomGradleFile.ApplyPluginList.AddPluginByName("com.google.firebase.crashlytics");
        }

        // ----- Android
        if (SdkConfig.ImportConversionFacebook() || SdkConfig.ImportAccount())
        {
            var DefaultConfig = CustomGradleFile.Android.DefaultConfig;
            DefaultConfig.AddElement(new Element($"resValue \"string\", \"facebook_app_id\", \"{SdkConfig.FacebookAppId}\""));
            DefaultConfig.AddElement(new Element($"resValue \"string\", \"facebook_scheme\", \"fb{SdkConfig.FacebookAppId}\""));

            DefaultConfig.AddElement(new Element($"manifestPlaceholders += [FB_APP_ID: \"@string/facebook_app_id\"]"));
            DefaultConfig.AddElement(new Element($"manifestPlaceholders += [FB_CLIENT_TOKEN: \"{SdkConfig.FacebookClientToken}\"]"));
        }
        if (SdkConfig.ImportAccount())
        {
            var DefaultConfig = CustomGradleFile.Android.DefaultConfig;
            DefaultConfig.AddElement(new Element($"resValue \"string\", \"gpg_project_id\", \"{SdkConfig.GooglePlayGamesProjectId}\""));
            

            DefaultConfig.AddElement(new Element($"manifestPlaceholders += [FB_LABEL: \"@string/app_name\"]"));
            DefaultConfig.AddElement(new Element($"manifestPlaceholders += [FB_SCHEME: \"@string/facebook_scheme\"]"));
            DefaultConfig.AddElement(new Element($"manifestPlaceholders += [GMS_GAMES_ID: \"@string/gpg_project_id\"]"));
        }
        if (SdkConfig.ImportConversionFirebase() || SdkConfig.ImportPushFirebase())
        {
            var buildTypes = new[] { "debug", "release" };
            foreach (var BuildType in buildTypes)
            {
                var BuildTypes = CustomGradleFile.Android.BuildTypes;
                var buildTypeBlock = BuildTypes.AddBuildTypeByName(BuildType);

                var FirebaseCrashlyticsBlock = new Block("firebaseCrashlytics");
                FirebaseCrashlyticsBlock.AddElement(new Element("mappingFileUploadEnabled false"));
                FirebaseCrashlyticsBlock.AddElement(new Element("nativeSymbolUploadEnabled false"));
                buildTypeBlock.AddElement(FirebaseCrashlyticsBlock);
            }
        }

        // ----- Dependencies
        Dependencies Dependencies = CustomGradleFile.Dependencies;
        Dependencies.AddElement(new Element($"var sdkVersion = \"{SdkConfig.AndroidDepsVersion}\""));
        if (SdkConfig.AndroidDepsVersion.EndsWith("SNAPSHOT"))
        {
            Dependencies.AddDependencyImplementationRaw("\"com.inonesdk.athana:athana-dev:${sdkVersion}\"");
        }
        else
        {
            Dependencies.AddDependencyImplementationRaw("\"com.inonesdk.athana:athana:${sdkVersion}\"");
        }

        foreach (var dep in Components)
        {
            Dependencies.AddDependencyImplementationRaw("\"com.inonesdk.athana:" + dep + ":${sdkVersion}\"");
        }

        projectFiles.SetBuildGradleFile(_sdkDepsOptionBuildGradle, CustomGradleFile);
        Debug.Log("Athana-SdkConfig updated");
    }

    private List<string> Transfor(SdkConfigData data)
    {
        List<string> Components = new();
        if (data.ImportAdMax())
        {
            // api("com.inonesdk.athana:ad-max:${sdkVersion}")
            Components.Add("ad-max");
        }
        if (data.ImportConversionAppsFlyer())
        {
            // api("com.inonesdk.athana:conversion-appsflyer:${sdkVersion}")
            Components.Add("conversion-appsflyer");
        }
        if (data.ImportConversionFacebook())
        {
            // api("com.inonesdk.athana:conversion-meta:${sdkVersion}")
            Components.Add("conversion-meta");
        }
        if (data.ImportConversionFirebase())
        {
            // api("com.inonesdk.athana:conversion-firebase:${sdkVersion}")
            Components.Add("conversion-firebase");
        }
        if (data.ImportAccount())
        {
            // api("com.inonesdk.athana:account-athana:${sdkVersion}")
            Components.Add("account-athana");
        }
        if (data.ImportPushFirebase())
        {
            // api("com.inonesdk.athana:push-firebase:${sdkVersion}")
            Components.Add("push-firebase");
        }
        return Components;
    }
}