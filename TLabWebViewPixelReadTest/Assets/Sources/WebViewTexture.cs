using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class WebViewTexture : MonoBehaviour
{
	// ---------------------------------------------------------------------------
	// Log state
	//

	private Text state;

	// ---------------------------------------------------------------------------
	// Web texture
	//

	private RawImage m_WebView;
	private Texture2D webImage;

	// ---------------------------------------------------------------------------
	// Web settings
	//

	public string url = "https://youtube.com";
	public int webWidth = 1024;
	public int webHeight = 1024;
	public int texWidth = 512;
	public int texHeight = 512;
	private bool m_WebViewEnable;

	// ---------------------------------------------------------------------------
	// Android plugin
	//

#if UNITY_ANDROID
	private AndroidJavaClass m_NativePlugin;
#endif

	private void Start()
	{
		state = GameObject.Find("RenderCanvas/State").GetComponent<Text>();
		state.text = "WebViewTexture start";

		m_WebViewEnable = true;

#if UNITY_ANDROID
		Init(webWidth, webHeight, texWidth, texHeight, Screen.width, Screen.height, url);
		webImage = new Texture2D(
			texWidth,
			texHeight,
			TextureFormat.ARGB32,
			false
		);
		webImage.name = "WebImage";
		m_WebView = GameObject.Find("RenderCanvas/WebView").GetComponent<RawImage>();
		m_WebView.texture = webImage;
#endif
	}

	protected virtual void OnDestroy()
	{
#if UNITY_ANDROID
        if (m_NativePlugin == null) return;

        m_NativePlugin.Call("Destroy");
		m_NativePlugin = null;
#endif
	}

	private void Update()
	{
		if (!m_WebViewEnable)
		{
			state.text = "WebView is null";
			return;
		}

		byte[] data = GetWebTexturePixel();

		if (data.Length > 0)
        {
			webImage.LoadRawTextureData(data);
			webImage.Apply();

			state.text = "texture updated";
        }
        else
        {
			state.text = "wait for texture update";
		}
	}

	// ----------------------------------------------------------------------------------------
	// Unity to Java
	// 

	public void Init(int webWidth, int webHeight, int tWidth, int tHeight, int sWidth, int sHeight, string url)
	{
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin = new AndroidJavaClass("com.tlab.libwebview.UnityConnect");
		m_NativePlugin.CallStatic("initialize", webWidth, webHeight, tWidth, tHeight, sWidth, sHeight, url);
#endif
	}

	public byte[] GetWebTexturePixel()
	{
#if UNITY_ANDROID
		if (Application.isEditor) return new byte[0];

		return m_NativePlugin.CallStatic<byte[]>("getPixel");
#else
		return null;
#endif
	}

	public void LoadUrl(string url)
	{
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin.CallStatic("loadUrl", url);
#endif
	}

	public void ZoomIn()
    {
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin.CallStatic("zoomIn");
#endif
	}

	public void ZoomOut()
	{
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin.CallStatic("zoomOut");
#endif
	}

	public void GoForward()
    {
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin.CallStatic("goForward");
#endif
	}

	public void GoBack()
	{
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin.CallStatic("goBack");
#endif
	}

	public void TouchEvent(int x, int y, int eventNum)
    {
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin.CallStatic("touchEvent", x, y, eventNum);
#endif
	}

	public void SetVisible(bool visible)
	{
		m_WebViewEnable = visible;
#if UNITY_ANDROID
		if (Application.isEditor) return;

		m_NativePlugin.CallStatic("setVisible", visible);
#endif
	}
}
