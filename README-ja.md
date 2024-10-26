# TLabWebView  

Android WebView を uGUI (Texture2D) として利用するためのプラグイン

- [x] キーボード入力
- [x] タッチ操作
- [x] ファイルダウンロード (blob, data urlを含む)
- [x] リサイズ
- [x] Javascriptの実行

[ドキュメントはこちら](https://tlabgames.gitbook.io/tlabwebview)  
[スニペットはこちら](https://gist.github.com/TLabAltoh/e0512b3367c25d3e1ec28ddbe95da497#file-tlabwebview-snippets-md)  
[Javaプラグインのソースコードはこちら](https://github.com/TLabAltoh/TLabWebViewPlugin)

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/tlabaltoh)

## 対応しているUnityのバージョン
- [x] Unity 2021
- [x] Unity 2022

## 対応するグラフィックスAPI
- [x] OpenGLES
- [x] Vulkan

## スクリーンショット  
Android13, Adreno 619で実行した画面  

<img src="Media/tlab-webview.png" width="256">

## 動作環境

|       |                          |
| ----- | ------------------------ |
| OS    | Android 10 ~ 14          |
| GPU   | Qualcomm Adreno 505, 619 |
| Unity | 2021.3                   |

## スタートガイド

### 依存するライブラリ

- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad) ```v0.0.4```

### インストール
<details><summary>こちらをご覧ください</summary>

#### Submodule
以下のコマンドでリポジトリをクローンしてください
```
git clone https://github.com/TLabAltoh/TLabWebView.git
```
or
```
git submodule add https://github.com/TLabAltoh/TLabWebView.git
```

#### UPM
Unity Package Managerで```add package from git ...```から以下のurlでパッケージをダウンロードしてください
```
https://github.com/TLabAltoh/TLabWebView.git#upm
```

</details>

### セットアップ

<details><summary>こちらをご覧ください</summary>

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

</details>

### Prefab
以下に置いてあるプレハブをCanvasに追加することでWebViewを実装できます
```
/Resources/TLabWebView.prefab
```

## 
> [!NOTE]
> 外部ストレージ(```/Download```や```/Picture```など)にファイルをダウンロードしたい場合，以下のパーミッションを```AndroidManifest.xml```に追加してください．これは，Android 11以降のデバイスで必要になります (詳細は[こちら](https://developer.android.com/training/data-storage/manage-all-files?hl=ja))．
> ```.xml
> <uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
> ```

> [!WARNING]
> Android WebViewは [WebXR API](https://developer.mozilla.org/ja/docs/Web/API/WebXR_Device_API/Fundamentals) をサポートしません．

> [!WARNING]
> OculusQuestはいくつかのHTML5 input タグをサポートしていません(下記参照)．もしそれらを使いたい場合は，```TLabWebView```クラスの```useCustomWidget```を有効にしてください．Androidで標準に使用されているウィジェットの代わりに，このプラグインが実装したウィジェットをWebView上に表示します．以下は，このプラグインによる，HTML5 inpu タグの対応状況です．
> 
> - [x] [datetime-local](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/datetime-local)
> - [x] [date](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/date)
> - [x] [time](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/time)
> - [x] [color](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/color)
> - [ ] [week](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/week)
> - [ ] [month](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/month)
> - [ ] [image](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/image)
> - [ ] [file](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/file)
> 
> また，このプラグインで実装されたカスタムウィジェットは，JavascriptからHTML5 input タグのポインターイベント([```onmousedown```](https://developer.mozilla.org/ja/docs/Web/API/Element/mousedown_event), [```onclick```](https://developer.mozilla.org/ja/docs/Web/API/Element/click_event))を無効にしています．この実装は，いくつかのウェブサイトでは問題を引き起こす可能性があることを留意してください．

> [!WARNING]
> このプラグインは，```OpenGLES```と```Vulkan```の両方をサポートしていますが，```Vulkan API``` を使用する場合は，デバイスが```OpenGLES 3.1```以上をサポートしている必要があることに留意してください．
