using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#nullable enable
[Serializable]
public class SdkConfigData
{

    private static string newConfigPath = Path.Combine(Application.dataPath, "Plugins", "athana-sdk-config.json");

    private static string oldConfigPath = Path.Combine(Application.dataPath, "Plugins", "Android", "athana-sdk-config.json");

    private static string configPath
    {
        get
        {
            if (File.Exists(newConfigPath))
            {
                return newConfigPath;
            }
            else
            {
                return oldConfigPath;
            }
        }
    }

    public static string miniAndroidSdkVersion = "1.5.1";
    public static string miniIOSSdkVersion = "1.1.0";
    public static string[] iosDepsManagers = new string[] { "SwiftPM", "CocoaPods" };

    public bool AdServiceEnabled = false;
    public bool AdMaxEnabled = false;

    public bool ConversionServiceEnabled = false;
    public bool ConversionAppsFlyerEnabled = false;
    public bool ConversionFacebookEnabled = false;
    public bool ConversionFirebaseEnabled = false;

    public bool AccountServiceEnabled = false;
    public string GooglePlayGamesProjectId = "";
    public string GoogleWebClientId = "";

    public bool PushServiceEnabled = false;
    public bool PushFirebaseEnabled = false;

    public bool GamingServiceEnabled = false;
    public bool GamingGPGSEnabled = false;
    public bool GamingGameCenterEnabled = false;

    // -------- 通用
    public string FacebookAppId = "";
    public string FacebookClientToken = "";

    public string AndroidDepsVersion = miniAndroidSdkVersion;

    public string IosDepsVersion = miniIOSSdkVersion;

    public int IosDepsManager = 0; // 0: SwiftPM, 1: CocoaPods

    public static SdkConfigData ReadForFile()
    {
        if (!File.Exists(configPath))
        {
            return new SdkConfigData();
        }
        var json = File.ReadAllText(configPath);
        if (json == null || json.Length == 0)
        {
            return new SdkConfigData();
        }
        else
        {
            return JsonConvert.DeserializeObject<SdkConfigData>(json);
        }
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(this);
        if (!File.Exists(configPath))
        {
            File.Create(configPath).Close();
        }
        File.WriteAllText(configPath, json, Encoding.UTF8);
    }

    public bool ImportAdMax()
    {
        return AdServiceEnabled && AdMaxEnabled;
    }

    public bool ImportConversionAppsFlyer()
    {
        return ConversionServiceEnabled && ConversionAppsFlyerEnabled;
    }

    public bool ImportConversionFacebook()
    {
        return ConversionServiceEnabled && ConversionFacebookEnabled;
    }
    public bool ImportConversionFirebase()
    {
        return ConversionServiceEnabled && ConversionFirebaseEnabled;
    }

    public bool ImportAccount()
    {
        return AccountServiceEnabled;
    }

    public bool ImportPushFirebase()
    {
        return PushServiceEnabled && PushFirebaseEnabled;
    }

    public bool ImportGamingGPGS()
    {
        return GamingServiceEnabled && GamingGPGSEnabled;
    }

    public bool ImportGamingGameCenter()
    {
        return GamingServiceEnabled && GamingGameCenterEnabled;
    }

    public bool CheckAndroidVersion()
    {
        var androidSdkVer = AndroidDepsVersion.Replace("-SNAPSHOT", "");
        var verArray = convertVer(androidSdkVer);
        if (verArray == null)
        {
            return false;
        }
        var miniVerArray = convertVer(miniAndroidSdkVersion);
        if (miniVerArray == null)
        {
            return false;
        }

        var miniMajorVer = miniVerArray[0];
        var miniMinorVer = miniVerArray[1];
        var miniPatchVer = miniVerArray[2];
        var majorVer = verArray[0];
        var minorVer = verArray[1];
        var patchVer = verArray[2];

        var checkResult = false;
        if (majorVer > miniMajorVer)
        {
            checkResult = true;
        }
        else if (majorVer == miniMajorVer && minorVer > miniMinorVer)
        {
            checkResult = true;
        }
        else if (majorVer == miniMajorVer && minorVer == miniMinorVer && patchVer >= miniPatchVer)
        {
            checkResult = true;
        }

        return checkResult;
    }

    public bool CheckIOSVersion()
    {
        var iosSdkVer = IosDepsVersion.Replace("-SNAPSHOT", "");
        var verArray = convertVer(iosSdkVer);
        if (verArray == null)
        {
            return false;
        }
        var miniVerArray = convertVer(miniIOSSdkVersion);
        if (miniVerArray == null)
        {
            return false;
        }

        var miniMajorVer = miniVerArray[0];
        var miniMinorVer = miniVerArray[1];
        var miniPatchVer = miniVerArray[2];
        var majorVer = verArray[0];
        var minorVer = verArray[1];
        var patchVer = verArray[2];

        var checkResult = false;
        if (majorVer > miniMajorVer)
        {
            checkResult = true;
        }
        else if (majorVer == miniMajorVer && minorVer > miniMinorVer)
        {
            checkResult = true;
        }
        else if (majorVer == miniMajorVer && minorVer == miniMinorVer && patchVer >= miniPatchVer)
        {
            checkResult = true;
        }

        return checkResult;
    }

    private int[]? convertVer(String verString)
    {
        var verArray = verString.Split('.');
        if (verArray.Length != 3)
        {
            return null;
        }
        var majorVer = int.Parse(verArray[0]);
        var minorVer = int.Parse(verArray[1]);
        var patchVer = int.Parse(verArray[2]);
        return new int[] { majorVer, minorVer, patchVer };
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        SdkConfigData data = (SdkConfigData)obj;
        return AdServiceEnabled == data.AdServiceEnabled &&
               AdMaxEnabled == data.AdMaxEnabled &&
               ConversionServiceEnabled == data.ConversionServiceEnabled &&
               ConversionAppsFlyerEnabled == data.ConversionAppsFlyerEnabled &&
               ConversionFacebookEnabled == data.ConversionFacebookEnabled &&
               ConversionFirebaseEnabled == data.ConversionFirebaseEnabled &&
               AccountServiceEnabled == data.AccountServiceEnabled &&
               GooglePlayGamesProjectId == data.GooglePlayGamesProjectId &&
               GoogleWebClientId == data.GoogleWebClientId &&
               PushServiceEnabled == data.PushServiceEnabled &&
               PushFirebaseEnabled == data.PushFirebaseEnabled &&
               GamingServiceEnabled == data.GamingServiceEnabled &&
               GamingGPGSEnabled == data.GamingGPGSEnabled &&
               GamingGameCenterEnabled == data.GamingGameCenterEnabled &&
               FacebookAppId == data.FacebookAppId &&
               FacebookClientToken == data.FacebookClientToken &&
               AndroidDepsVersion == data.AndroidDepsVersion &&
               IosDepsVersion == data.IosDepsVersion &&
               IosDepsManager == data.IosDepsManager;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(AdServiceEnabled);
        hash.Add(AdMaxEnabled);
        hash.Add(ConversionServiceEnabled);
        hash.Add(ConversionAppsFlyerEnabled);
        hash.Add(ConversionFacebookEnabled);
        hash.Add(ConversionFirebaseEnabled);
        hash.Add(AccountServiceEnabled);
        hash.Add(GooglePlayGamesProjectId);
        hash.Add(GoogleWebClientId);
        hash.Add(PushServiceEnabled);
        hash.Add(PushFirebaseEnabled);
        hash.Add(GamingServiceEnabled);
        hash.Add(GamingGPGSEnabled);
        hash.Add(GamingGameCenterEnabled);
        hash.Add(FacebookAppId);
        hash.Add(FacebookClientToken);
        hash.Add(AndroidDepsVersion);
        hash.Add(IosDepsVersion);
        hash.Add(IosDepsManager);
        return hash.ToHashCode();
    }

    public static bool operator ==(SdkConfigData left, SdkConfigData right)
    {
        return EqualityComparer<SdkConfigData>.Default.Equals(left, right);
    }

    public static bool operator !=(SdkConfigData left, SdkConfigData right)
    {
        return !(left == right);
    }
}