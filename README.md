# TLabWebView

[日本語版READMEはこちら](README-ja.md)

Plug-in for WebView that runs in Unity and can display WebView results as Texture2D  
Hardware-accelerated rendering is also available  

## Screenshot  
Screenshot run on Android 10, Adreno 505  


![Screenshot_20230119-055237](https://user-images.githubusercontent.com/121733943/213294032-29502633-2f48-4f9e-91e4-269316920855.png)


Screenshot run on Android 13, Adreno 619  


![capture](https://user-images.githubusercontent.com/121733943/235582195-ba33dafc-5773-48cd-8068-4e3303749870.gif)


After the update, the search tab is now supported


## Operating Environment
OS: Android 10 ~ 13  
GPU: Qualcomm Adreno 505, 619  
Unity: 2021.23f1  

## Getting Started
### Prerequisites
- Unity 2021.3.23f1  
- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad)
### Installing
Clone the repository to any directory under Assets in the Unity project that will use the assets with the following command  
```
git clone https://github.com/TLabAltoh/TLabWebView.git
```
If you are adding to an existing git project, use the following command instead
```
git submodule add https://github.com/TLabAltoh/TLabWebView.git
```
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

3. Open "Assets/Scenes/main"
4. Change any parameter of TLabWebView attached to TLabWebViewSample/WebView from the hierarchy  
- Url: URL to load during WebView initialization
- WebWidth, WebHeight: Web page size
- TexWidth, TexHeight: Texture2D size

## TODO
- Make it work in VR.
- Sending input (e.g. key codes) to the browser
