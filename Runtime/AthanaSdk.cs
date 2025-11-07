public class AthanaSdk :
#if UNITY_EDITOR
    // Check for Unity Editor first since the editor also responds to the currently selected platform.
    AthanaUnityEditor
#elif UNITY_ANDROID
    AthanaAndroid
#elif UNITY_IPHONE || UNITY_IOS
    AthanaiOS
#else
    AthanaUnityEditor
#endif
{
    private const string _version = "1.2.0";

    /// <summary>
    /// Returns the current plugin version.
    /// </summary>
    public static string Version
    {
        get { return _version; }
    }
}
