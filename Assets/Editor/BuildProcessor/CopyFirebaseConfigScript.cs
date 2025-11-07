using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

// 实现 IPostprocessBuildWithReport 接口以执行构建后操作
public class CopyFirebaseConfigScript : IPostprocessBuildWithReport
{
    // 定义脚本的执行顺序，数值越小越早执行
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        // 确保只在 Android 平台构建时执行
        if (report.summary.platform != BuildTarget.Android)
        {
            return;
        }
        var SdkConfig = SdkConfigData.ReadForFile();

        // 获取 Android Studio 项目的导出路径
        string outputPath = report.summary.outputPath;

        // 目标路径：在 Android 项目的 launcher 模块下
        string targetPath = Path.Combine(outputPath, "launcher", "google-services.json");

        if (!SdkConfig.ImportConversionFirebase() && !SdkConfig.ImportPushFirebase())
        {
            Debug.Log("检测未引入 Firebase 依赖，忽略导出 google-services.json");
            //if (File.Exists(targetPath))
            //{
            //    File.Delete(targetPath);
            //}
            return;
        }

        // 源文件路径：Assets 目录下的 google-services.json
        string sourcePath = Path.Combine(Application.dataPath, "Plugins/Android/google-services.json");

        // 检查源文件是否存在
        if (!File.Exists(sourcePath))
        {
            Debug.LogWarning("Assets 目录下找不到 google-services.json 文件，请确保文件存在。");
            return;
        }
        Debug.Log("导出 google-services.json 至 Android Studio 工程");

        

        // 检查目标目录是否存在，如果不存在则创建
        string targetDirectory = Path.GetDirectoryName(targetPath);
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        // 复制文件
        File.Copy(sourcePath, targetPath, true);
        Debug.Log("导出 google-services.json 完成");
    }
}
