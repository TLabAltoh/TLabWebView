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
| OS    | Android 10 ~ 14          |
| GPU   | Qualcomm Adreno 505, 619 |
| Unity | 2021.3                   |

## Getting Started

### Requirements
- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad)

### Installing

<details><summary>Please see here</summary>

#### Submodule
Clone this repository with the following command
```
git clone https://github.com/TLabAltoh/TLabWebView.git
```
or
```
git submodule add https://github.com/TLabAltoh/TLabWebView.git
```

#### UPM
```add package from git URL ...```
```
https://github.com/TLabAltoh/TLabWebView.git#upm
```

</details>

### Set Up

<details><summary>Please see here</summary>

- Build Settings

| Property | Value   |
| -------- | ------- |
| Platform | Android |

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

</details>

### Prefab
Prefab is here. Just add prefab to the canvas to implement webview
```
/Resources/TLabWebView.prefab
```

## NOTICE
- If you want to access files that are in external storage (like download, picture). you need to add follow manifest in Android 11 ([detail](https://developer.android.com/training/data-storage/manage-all-files?hl=en)).
```.xml
<uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
```

- Android WebView doesn't support the [WebXR API](https://developer.mozilla.org/en-US/docs/Web/API/WebXR_Device_API/Fundamentals).

- OculusQuest doesn't support some HTML5 input tags (see below). If you want to use them, please enable the ```useCustomWidget``` property of the ```TLabWebView``` class. It will display a widget implemented by the plugin on the WebView instead of the standard Android widget. Below is the status of support for the html5 input tag, which is not supported in OculusQuest.

    ### Verified

    - [x] [datetime-local](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/datetime-local)
    - [x] [date](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/date)
    - [x] [time](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/time)
    - [x] [color](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/color)
    - [ ] [week](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/week)
    - [ ] [month](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/month)

    ### Unverified
    - [ ] [image](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/image)
    - [ ] [file](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file)

    Also, currently custom input widget implemented by plugin uses javascript and disable pointer event ([```onmousedown```](https://developer.mozilla.org/en-US/docs/Web/API/Element/mousedown_event), [```onclick```](https://developer.mozilla.org/en-US/docs/Web/API/Element/click_event)). Please note that this implementation has possibility to cause problem on some website.

- This plugin supports both Vulkan and OpenGLES, but if you are building an application that uses a Vulkan graphics API, the Android device must support OpenGLES as well as Vulkan. This is because some processes in this plugin depend on the GLES API.

- Now supports play in VR ([link](https://github.com/TLabAltoh/TLabWebViewVR)).

## TODO
- Remove the GLES API dependency from the Vulkan use case.

## Link
[Snippets](https://gist.github.com/TLabAltoh/e0512b3367c25d3e1ec28ddbe95da497#file-xr-composition-layers_rendering-md)
[Source code of the java plugin used](https://github.com/TLabAltoh/TLabWebViewPlugin)

