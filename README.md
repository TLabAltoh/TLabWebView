# TLabWebView

[日本語版READMEはこちら](README-ja.md)

Plug-in for WebView that runs in Unity and can display WebView results as Texture2D  
Hardware-accelerated rendering is also available  
Key input support
File Download Support
Supports javascript execution  

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/tlabaltoh)

## Screenshot  
Screenshot run on Android 13, Adreno 619  

<img src="Media/tlab-webview.png" width="256">

## Operating Environment
OS: Android 10 ~ 13  
GPU: Qualcomm Adreno 505, 619  
Unity: 2021.23f1  

## Getting Started
### Prerequisites
- Unity 2021.3.23f1  
- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad)
### Installing
Clone the repository or download it from the release and place it in the Asset folder of Unity
### Set up
1. Change platform to Android from Build Settings  
2. Add the following symbols to Project Settings --> Player --> Other Settings (to be used at build time)  
```
UNITYWEBVIEW_ANDROID_USES_CLEARTEXT_TRAFFIC
```
```
UNITYWEBVIEW_ANDROID_ENABLE_CAMERA
```
```
UNITYWEBVIEW_ANDROID_ENABLE_MICROPHONE
```
- Color Space: Linear
- Graphics: OpenGLES3
- Minimux API Level: 23 
  
- Create Assets/Plugins/Android/AndroidManifest.xml and copy the following text
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest
    xmlns:android="http://schemas.android.com/apk/res/android"
    package="com.unity3d.player"
    xmlns:tools="http://schemas.android.com/tools">
    <application>
        <activity android:name="com.unity3d.player.UnityPlayerActivity"
                  android:theme="@style/UnityThemeSelector">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
            <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
        </activity>
    </application>

	<!-- For Unity-WebView -->
	<application android:allowBackup="true"/>
	<application android:supportsRtl="true"/>
	<application android:hardwareAccelerated="true"/>
	<application android:usesCleartextTraffic="true"/>
	
	<uses-permission android:name="android.permission.INTERNET" />
	<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
	<uses-permission android:name="android.permission.CAMERA" />
	<uses-permission android:name="android.permission.MICROPHONE" />
	<uses-permission android:name="android.permission.MODIFY_AUDIO_SETTINGS" />
	<uses-permission android:name="android.permission.RECORD_AUDIO" />
	
	<uses-feature android:name="android.hardware.camera" />
	<uses-feature android:name="android.hardware.microphone" />
	<!-- For Unity-WebView -->
</manifest>
```
3. Add TLabWebView/TLabWebView.prefab to scene
4. Change the setting of WebView
Setting items in TLabWebView.cs (located in TLabWebView.prefab/WebView)  

<img src="Media/tlab-webview-settings.png" width="256">  

- Url: URL to load during WebView initialization  
- DlOption: Whether to download to the application folder or the downloads folder  
- SubDir: In case of setting download to application folder, it is downloaded to ```{Application folder}/{files}/{SubDir}```  
- Web (Width/Height):  Web page resolution (default 1024 * 1024)  
- Tex (Width/Height): Texture2D resolution used within Unity (default 512 * 512)  

## Scripting API
### Initialize
- void Init(int webWidth, int webHeight, int tWidth, int tHeight, int sWidth, int sHeight, string url, int dlOption, string subDir)
- void StartWebView()
### Update Frame
- byte[] GetWebTexturePixel()
- void UpdateFrame()
### Capture Element
- void CaptureHTMLSource()
- void CaptureElementById(string id)
- string CurrentHTMLCaptured()
### Load URL
- void LoadUrl(string url)
- void GoForward()
- void GoBack()
### Zoom In/Out
- void ZoomIn()
- void ZoomOut()
### Evaluate Javascript
- void EvaluateJS(string js)
### Touch Event
- void TouchEvent(int x, int y, int eventNum)
### Key Event
- void KeyEvent(char key)
- void BackSpace()

## NOTICE
- Now supports play in VR ([link](https://github.com/TLabAltoh/TLabWebViewVR)).

## Link
[Source code of the java plugin used](https://github.com/TLabAltoh/TLabWebViewPlugin)
