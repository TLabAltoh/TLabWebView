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

## 対応するグラフィックスAPI
- [x] OpenGLES
- [x] Vulkan (一部制限あり)

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

- [TLabVKeyborad](https://github.com/TLabAltoh/TLabVKeyborad)

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

## お知らせ
- ダウンロードフォルダのような外部ストレージにアクセスしたい場合，android 11以降はこちらのパーミッションを```AndroidManifest.xml```に追加してください ([詳細](https://developer.android.com/training/data-storage/manage-all-files?hl=ja))．
```.xml
<uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
```

- Android WebViewは [WebXR API](https://developer.mozilla.org/ja/docs/Web/API/WebXR_Device_API/Fundamentals) をサポートしません．

- OculusQuestはいくつかのHTML5 input タグをサポートしていません(下記を参照してください)．もしそれらを使用したい場合は，```TLabWebView```クラスの```useCustomWidget```を有効にしてください．Androidで標準に使用されているウィジェットの代わりに，プラグイン側で実装されたウィジェットがWebView上に表示されます．下記は，OculusQuestがサポートしていないHTML5 input タグに対するプラグインの対応状況です．

    ### 動作を検証済み

    - [x] [datetime-local](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/datetime-local)
    - [x] [date](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/date)
    - [x] [time](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/time)
    - [x] [color](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/color)
    - [ ] [week](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/week)
    - [ ] [month](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/month)

    ### 動作未検証

    - [ ] [image](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/image)
    - [ ] [file](https://developer.mozilla.org/ja/docs/Web/HTML/Element/input/file)

    また，プラグイン側で実装されたウィジェットは，JavascriptからHTML5 input タグのポインターイベント([```onmousedown```](https://developer.mozilla.org/ja/docs/Web/API/Element/mousedown_event), [```onclick```](https://developer.mozilla.org/ja/docs/Web/API/Element/click_event))を無効にすることで実装されています．この実装は，いくつかのウェブサイトでは問題を引き起こす可能性があることを留意してください．

- このプラグインは，OpenGLESとVulkanの両方をサポートしていますが，Vulkanでビルドをする場合はAndroidデバイスがVulkanだけでなくOpenGLESもサポートしていることが必要になります．これは，プラグインの一部の処理がGLES APIに依存しているためです．

- VRでのプレイに対応しました([link](https://github.com/TLabAltoh/TLabWebViewVR))

## TODO
- Vulkanのユースケースにおいて，GLES APIへの依存を削除する．

## リンク
- [スニペット](https://gist.github.com/TLabAltoh/e0512b3367c25d3e1ec28ddbe95da497)  
- [使用したJavaプラグインのソースコード](https://github.com/TLabAltoh/TLabWebViewPlugin)
