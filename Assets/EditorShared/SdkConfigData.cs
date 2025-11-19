using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#nullable enable
[Serializable]
public class SdkConfigData
{
    private static string configPath = Path.Combine(Application.dataPath, "Plugins", "Android", "athana-sdk-config.json");

    public bool AdServiceEnabled = false;
    public bool AdMaxEnabled = false;

    public bool ConversionServiceEnabled = false;
    public bool ConversionAppsFlyerEnabled = false;
    public bool ConversionFacebookEnabled = false;
    public bool ConversionFirebaseEnabled = false;

    public bool AccountServiceEnabled = false;
    public string GooglePlayGamesProjectId = "";

    public bool PushServiceEnabled = false;
    public bool PushFirebaseEnabled = false;

    // -------- Õ®”√
    public string FacebookAppId = "";
    public string FacebookClientToken = "";

    public string AndroidDepsVersion = "1.3.2";

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
            return JsonUtility.FromJson<SdkConfigData>(json);
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

    public override bool Equals(object obj)
    {
        return obj is SdkConfigData data &&
               AdServiceEnabled == data.AdServiceEnabled &&
               AdMaxEnabled == data.AdMaxEnabled &&
               ConversionServiceEnabled == data.ConversionServiceEnabled &&
               ConversionAppsFlyerEnabled == data.ConversionAppsFlyerEnabled &&
               ConversionFacebookEnabled == data.ConversionFacebookEnabled &&
               ConversionFirebaseEnabled == data.ConversionFirebaseEnabled &&
               AccountServiceEnabled == data.AccountServiceEnabled &&
               GooglePlayGamesProjectId == data.GooglePlayGamesProjectId &&
               PushServiceEnabled == data.PushServiceEnabled &&
               PushFirebaseEnabled == data.PushFirebaseEnabled &&
               FacebookAppId == data.FacebookAppId &&
               FacebookClientToken == data.FacebookClientToken &&
               AndroidDepsVersion == data.AndroidDepsVersion;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(AdServiceEnabled);
        hash.Add(AdMaxEnabled);
        hash.Add(ConversionServiceEnabled);
        hash.Add(ConversionAppsFlyerEnabled);
        hash.Add(ConversionFacebookEnabled);
        hash.Add(ConversionFirebaseEnabled);
        hash.Add(AccountServiceEnabled);
        hash.Add(GooglePlayGamesProjectId);
        hash.Add(PushServiceEnabled);
        hash.Add(PushFirebaseEnabled);
        hash.Add(FacebookAppId);
        hash.Add(FacebookClientToken);
        hash.Add(AndroidDepsVersion);
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