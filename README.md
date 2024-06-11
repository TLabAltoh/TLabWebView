# TLabWebView

[日本語版READMEはこちら](README-ja.md)

Plug-in for WebView that runs in Unity and can display WebView results as Texture2D  
- Hardware-accelerated rendering is available  
- Key input support
- File Download Support (blob url, data url, download manager ...)
- Supports javascript execution  
- Resize webview and texture

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/tlabaltoh)

## Document
[document is here](https://tlabgames.gitbook.io/tlabwebview)

## Unity version this plugin supports
- [x] Unity 2021
- [x] Unity 2022

## Graphics api this plugin supports
- [x] OpenGLES
- [x] Vulkan (with some limitations)

## Screenshot  
Screenshot run on Android 13, Adreno 619  

<img src="Media/tlab-webview.png" width="256">

## Operating Environment

|       |                          |
| ----- | ------------------------ |
| OS    | Android 10 ~ 13          |
| GPU   | Qualcomm Adreno 505, 619 |
| Unity | 2021.3                   |

## Getting Started

### Requirements
- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad)

### Installing

Clone the repository or download it from the release and place it in the Asset folder of Unity

### Set Up

- Build Settings

| Property      | Value   |
| ------------- | ------- |
| Platform      | Android |

- Project Settings

| Property          | Value                                 |
| ----------------- | ------------------------------------- |
| Color Space       | Linear                                |
| Minimum API Level | 26                                    |
| Target API Level  | 30 (Unity 2021), 31 ~ 32 (Unity 2022) |

- Add the following symbols to Project Settings --> Player --> Other Settings (to be used at build time)

```
UNITYWEBVIEW_ANDROID_USES_CLEARTEXT_TRAFFIC
```
```
UNITYWEBVIEW_ANDROID_ENABLE_CAMERA
```
```
UNITYWEBVIEW_ANDROID_ENABLE_MICROPHONE
```

## NOTICE
- If you want to access files that are in external storage (like download, picture). you need to add follow manifest in Android 11 ([detail](https://developer.android.com/training/data-storage/manage-all-files?hl=en)).
```.xml
<uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
```

- This plugin supports both Vulkan and OpenGLES, but if you are building an application that uses a Vulkan graphics API, the Android device must support OpenGLES as well as Vulkan. This is because some processes in this plugin depend on the GLES API.

- Now supports play in VR ([link](https://github.com/TLabAltoh/TLabWebViewVR)).

## TODO
- Remove the GLES API dependency from the Vulkan use case.

## Link
[Source code of the java plugin used](https://github.com/TLabAltoh/TLabWebViewPlugin)

