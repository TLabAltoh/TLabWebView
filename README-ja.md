# TLabWebView  

Unityで動作するWebViewのプラグイン．WebViewの結果をTexture2Dとして表示できます  
ハードウェアアクセラレーションによる描画も取得可能  

## スクリーンショット  
Android10, Adreno 505で実行した画面  


![Screenshot_20230119-055237](https://user-images.githubusercontent.com/121733943/213294032-29502633-2f48-4f9e-91e4-269316920855.png)


更新後，検索タブに対応しました


<img src="https://user-images.githubusercontent.com/121733943/236137674-27b8f81e-7fc6-401b-b3f0-c80f72ada14d.png" width="256">


Android13, Adreno 619で実行した画面  


![capture](https://user-images.githubusercontent.com/121733943/235582195-ba33dafc-5773-48cd-8068-4e3303749870.gif)


## 動作環境
OS: Android 10 ~ 13  
GPU: Qualcomm Adreno 505, 619  
Unity: 2021.23f1  

## スタートガイド
### 必要な要件
- Unity 2021.3.23f1  
- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad)
### インストール
UnityProjectのAssets配下で任意のディレクトリに以下のコマンドからリポジトリをクローン
```
git clone https://github.com/TLabAltoh/TLabWebView.git
```
UnityProjectが既にgitの管理下の場合は以下のコマンドでクローン
```
git submodule add https://github.com/TLabAltoh/TLabWebView.git
```
### セットアップ
1. Build Settingsからプラットフォームを Androidに変更  
2. Project Settings --> Player --> Other Settings に以下のシンボルを追加(ビルド時に使用)
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
3. Assets/Scenes/mainを開く
4. ヒエラルキーからTLabWebViewSample/WebView にアタッチされている TLabWebViewのパラメータを任意で変更  
- Url: WebViewの初期化時にロードするURL
- WebWidth, WebHeight: Webページのサイズ
- TexWidth, TexHeight: Texture2Dのサイズ

## TODO
- VRで動作するようにする
- ブラウザへの入力(キーコードなど)の送信
