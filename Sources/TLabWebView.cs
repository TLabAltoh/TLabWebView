using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class TLabWebView : MonoBehaviour
{
	[SerializeField] private RawImage WebViewRawImage;

	[SerializeField] public string url = "https://youtube.com";
	[SerializeField] public int webWidth = 1024;
	[SerializeField] public int webHeight = 1024;
	[SerializeField] public int texWidth = 512;
	[SerializeField] public int texHeight = 512;

	private bool m_WebViewEnable;
	private Texture2D webViewTexture;

#if UNITY_ANDROID
	private AndroidJavaClass m_NativePlugin;
#endif

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

	public void StartWebView()
    {
		if (m_WebViewEnable == true)
			return;

		m_WebViewEnable = true;

#if UNITY_ANDROID
		Init(webWidth, webHeight, texWidth, texHeight, Screen.width, Screen.height, url);
		webViewTexture = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
		webViewTexture.name = "WebImage";
		WebViewRawImage.texture = webViewTexture;
#endif
	}

	public void UpdateFrame()
    {
		if (!m_WebViewEnable)
		{
			return;
		}

		byte[] data = GetWebTexturePixel();

		if (data.Length > 0)
		{
			webViewTexture.LoadRawTextureData(data);
			webViewTexture.Apply();
		}
	}

	protected virtual void OnDestroy()
	{
#if UNITY_ANDROID
		if (m_NativePlugin == null) return;

		m_NativePlugin.Call("Destroy");
		m_NativePlugin = null;
#endif
	}
}
