# Athana SDK for Unity

此插件适用于 Unity Editor 平台，最低支持 Unity Editor 2023.1

## 安装

- 方式一：本地 UnityPackage 安装
    - 从链接中下载：[Release Page](https://github.com/AthanaSDK/athana_unity_plugin/releases)
- 方式二：通过 Git URL 安装
    - 打开 Package Manager (`Window -> Package Manger`)
    - 点击左上角 + 图标，选择：`Install Package form git URL`
    - 输入：`https://github.com/AthanaSDK/athana_unity_plugin.git`

## 集成配置

1. 确认 `Edit -> Project Settings -> Player -> Settings for Android -> Publishing Settings -> Build` 中，勾选：
    - `Custom Launcher Gradle Template`
    - `Custom Base Gradle Template`

2. 编辑 `${UNITY_PROJEECT}\Assets\Plugins\Android\baseProjectTemplate.gradle`

```groovy
plugins {
    ...

    // 插入插件声明↓
    id "org.jetbrains.kotlin.android"  version "2.0.21" apply false
    id "com.google.gms.google-services" version "4.4.2" apply false
    id "com.google.firebase.crashlytics" version "3.0.4" apply false
    
    **BUILD_SCRIPT_DEPS**
}

...
```


3. 编辑 `${UNITY_PROJEECT}\Assets\Plugins\Android\launcherTemplate.gradle`

```groovy
apply plugin: 'com.android.application'
// 插入插件应用 ↓
apply plugin: 'org.jetbrains.kotlin.android'
apply from: 'athana_options.gradle'

...
```


## 文档

详见：[Unity 集成文档](https://athana.inonesdk.com/docs/Unity-Install)