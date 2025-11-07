using UnityEditor;
using UnityEngine;

class SdkConfigEditorWindow: EditorWindow
{
    [MenuItem("Assets/Athana/SDK Configuration")]
    public static void OpenSdkConfigWindows()
    {
        SdkConfigEditorWindow window = GetWindow<SdkConfigEditorWindow>();
        window.titleContent = new GUIContent("Athana SDK Configuration");
        window.Show();
    }

    private Vector2 scrollPos = Vector2.zero;

    private SdkConfigData SdkConfig;


    private void OnEnable()
    {
        SdkConfig = SdkConfigData.ReadForFile();
    }

    public void OnGUI()
    {
        GUILayout.Space(25);

        GUIStyle textAreaStyle = new(EditorStyles.textArea)
        {
            wordWrap = true
        };

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        // ----------------- Deps Version -----------------
        GUILayout.Label("版本配置", EditorStyles.boldLabel);
        GUILayout.Space(10);
        SdkConfig.AndroidDepsVersion = EditorGUILayout.TextField("Android SDK版本", SdkConfig.AndroidDepsVersion);
        GUILayout.Space(25);

        // ----------------- AD -----------------
        GUILayout.Label("广告服务配置", EditorStyles.boldLabel);
        GUILayout.Space(10);
        SdkConfig.AdServiceEnabled = EditorGUILayout.BeginToggleGroup("集成广告", SdkConfig.AdServiceEnabled);
        SdkConfig.AdMaxEnabled = EditorGUILayout.BeginToggleGroup("AppLovin MAX", SdkConfig.AdMaxEnabled);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(25);

        // ----------------- Conversion -----------------
        GUILayout.Label("归因服务配置", EditorStyles.boldLabel);
        GUILayout.Space(10);
        SdkConfig.ConversionServiceEnabled = EditorGUILayout.BeginToggleGroup("集成归因", SdkConfig.ConversionServiceEnabled);
        SdkConfig.ConversionAppsFlyerEnabled = EditorGUILayout.BeginToggleGroup("AppsFlyer", SdkConfig.ConversionAppsFlyerEnabled);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(10);
        SdkConfig.ConversionFacebookEnabled = EditorGUILayout.BeginToggleGroup("Facebook", SdkConfig.ConversionFacebookEnabled);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(10);
        SdkConfig.ConversionFirebaseEnabled = EditorGUILayout.BeginToggleGroup("Firebase", SdkConfig.ConversionFirebaseEnabled);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(10);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(25);

        // ----------------- Account -----------------
        GUILayout.Label("三方登录服务配置", EditorStyles.boldLabel);
        GUILayout.Space(10);
        SdkConfig.AccountServiceEnabled = EditorGUILayout.BeginToggleGroup("集成三方登录", SdkConfig.AccountServiceEnabled);
        SdkConfig.GooglePlayGamesProjectId = EditorGUILayout.TextField("GP Games ProjectID", SdkConfig.GooglePlayGamesProjectId);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(25);

        // ----------------- Push -----------------
        GUILayout.Label("推送服务配置", EditorStyles.boldLabel);
        GUILayout.Space(10);
        SdkConfig.PushServiceEnabled = EditorGUILayout.BeginToggleGroup("集成推送", SdkConfig.PushServiceEnabled);
        SdkConfig.PushFirebaseEnabled = EditorGUILayout.BeginToggleGroup("Firebase", SdkConfig.PushFirebaseEnabled);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(25);

        // ----------------- 公共 -----------------
        GUILayout.Label("三方SDK公用配置", EditorStyles.boldLabel);
        SdkConfig.FacebookAppId = EditorGUILayout.TextField("Faacebook AppID", SdkConfig.FacebookAppId);
        SdkConfig.FacebookClientToken = EditorGUILayout.TextField("Facebook ClientToken", SdkConfig.FacebookClientToken);
        GUILayout.Space(25);

        if (GUILayout.Button("保存"))
        {
            SdkConfig.Save();
            ShowNotification(new GUIContent("保存成功"));
        }

        EditorGUILayout.EndScrollView();
    }

    void Update()
    {

    }

    void OnDestroy()
    {

    }
}
