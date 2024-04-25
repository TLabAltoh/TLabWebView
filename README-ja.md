# TLabWebView  

Unityで動作するWebViewのプラグイン．WebViewの結果をTexture2Dとして表示できます  
- ハードウェアアクセラレーションによる描画を取得可能  
- キーボード入力をサポート  
- ファイルのダウンロードをサポート(blob url, data url, download manger ...)  
- javascriptの実行に対応  
- webviewとテクスチャのリサイズに対応

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/tlabaltoh)

## Document
[ドキュメントはこちら](https://tlabgames.gitbook.io/tlabwebview)

## 対応しているUnityのバージョン
- [x] Unity 2021
- [x] Unity 2022

## スクリーンショット  
Android13, Adreno 619で実行した画面  

<img src="Media/tlab-webview.png" width="256">

## 動作環境

|       |                          |
| ----- | ------------------------ |
| OS    | Android 10 ~ 13          |
| GPU   | Qualcomm Adreno 505, 619 |
| Unity | 2021.3                   |

## スタートガイド

### 依存するライブラリ

- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad)

### インストール

リポジトリをクローン，またはリリースからダウンロードし，UnityのAssetフォルダに配置してください

### セットアップ

- Build Settings

| Property      | Value   |
| ------------- | ------- |
| Platform      | Android |

- Project Settings

| Property          | Value                                 |
| ----------------- | ------------------------------------- |
| Color Space       | Linear                                |
| Graphics          | OpenGLES3                             |
| Minimum API Level | 26                                    |
| Target API Level  | 30 (Unity 2021), 31 ~ 32 (Unity 2022) |


-  Project Settings --> Player --> Other Settings に以下のシンボルを追加(ビルド時に使用)

```
UNITYWEBVIEW_ANDROID_USES_CLEARTEXT_TRAFFIC
```
```
UNITYWEBVIEW_ANDROID_ENABLE_CAMERA
```
```
UNITYWEBVIEW_ANDROID_ENABLE_MICROPHONE
```

## お知らせ
- VRでのプレイに対応しました([link](https://github.com/TLabAltoh/TLabWebViewVR))

## TODO
- Vulkanのサポート

## リンク
[使用したJavaプラグインのソースコード](https://github.com/TLabAltoh/TLabWebViewPlugin)
