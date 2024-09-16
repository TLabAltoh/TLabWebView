#define DEBUG
#undef DEBUG

using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace TLab.Android.WebView
{
	public class TLabWebView : MonoBehaviour
	{
		public enum State
		{
			INITIALISING,
			INITIALIZED,
			DESTROYED,
			NONE
		}

		[SerializeField] private RawImage m_rawImage;
		[SerializeField] private string m_url = "https://youtube.com";

		[Header("File Download Settings")]
		[SerializeField] private DownloadOption m_dlOption;
		[SerializeField] private string m_dlSubDir = "downloads";

		[Header("Resolution Setting")]
		[SerializeField] private int m_webWidth = 1024;
		[SerializeField] private int m_webHeight = 1024;
		[SerializeField] private int m_texWidth = 512;
		[SerializeField] private int m_texHeight = 512;

		[Header("Event Callback")]
		[SerializeField] private EventCallback m_jsEventCallback = new EventCallback();

		[Header("Other Option")]
		[SerializeField] private string[] m_intentFilters;
		[SerializeField, Min(0)] private int m_fps = 30;
		[SerializeField] private bool m_useCustomWidget = false;
		[SerializeField] private CaptureMode m_captureMode = CaptureMode.HARDWARE_BUFFER;

		#region PROPERTYS

		public int webWidth => m_webWidth;

		public int webHeight => m_webHeight;

		public int texWidth => m_texWidth;

		public int texHeight => m_texHeight;

		public DownloadOption dlOption => m_dlOption;

		public string subDir => m_dlSubDir;

		public State state => m_state;

		public CaptureMode captureMode => m_captureMode;

		public EventCallback jsEventCallback => m_jsEventCallback;

		#endregion PROPERTYS

		private static string THIS_NAME = "[tlabwebview] ";

		private State m_state = State.NONE;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
		private AndroidJavaObject m_NativePlugin;

		private class DeletableNativePlugin
		{
			public AndroidJavaObject androidJavaObject;

			public DeletableNativePlugin(AndroidJavaObject androidJavaObject)
			{
				this.androidJavaObject = androidJavaObject;
			}
		}

		private static Queue<DeletableNativePlugin> m_deletableNativePlugins = new Queue<DeletableNativePlugin>();

		private IntPtr m_rawObject;
#endif

		private delegate void RenderEventDelegate(int id);

		private static RenderEventDelegate DestroyNativePluginHandle = new RenderEventDelegate(DestroyNativePlugin);

		private static IntPtr DestroyNativePluginHandlePtr = Marshal.GetFunctionPointerForDelegate(DestroyNativePluginHandle);

		[AOT.MonoPInvokeCallback(typeof(RenderEventDelegate))]
		private static void DestroyNativePlugin(int id)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			foreach (var deletable in m_deletableNativePlugins)
			{
				if (deletable.androidJavaObject != null)
				{
					deletable.androidJavaObject.Call("Destroy");
					deletable.androidJavaObject.Dispose();
					deletable.androidJavaObject = null;
				}
			}

			m_deletableNativePlugins.Clear();
#endif
		}

		private delegate void UpdateFrameFunc();

		private UpdateFrameFunc m_updateFrameFunc;

		private Texture2D m_loadingView;
		private Texture2D m_contentView;

		private static Vector2Int m_screenFullRes;

		//
		// Initialize
		//

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void PreInitWebView()
		{
			// https://github.com/TLabAltoh/TLabWebView/issues/6
			m_screenFullRes = new Vector2Int(Screen.width, Screen.height);

			//Debug.Log(THIS_NAME + $"screen resolution: {Screen.width}, {Screen.height}");
		}

		/// <summary>
		/// Launch initialize task if WebView is not initialized yet.
		/// </summary>
		public void Init()
		{
			if (m_state == State.NONE)
			{
				StartCoroutine(InitTask());
			}
		}

		/// <summary>
		/// Set resolution for both WebView and texture (called on initialization).
		/// </summary>
		/// <param name="webWidth">WebView width</param>
		/// <param name="webHeight">WebView height</param>
		/// <param name="texWidth">Texture width</param>
		/// <param name="texHeight">Texture height</param>
		public void InitResolution(
			int webWidth, int webHeight,
			int texWidth, int texHeight)
		{
			m_webWidth = webWidth;
			m_webHeight = webHeight;

			m_texWidth = texWidth;
			m_texHeight = texHeight;
		}

		/// <summary>
		/// Launch initialize task if WebView is not initialized yet.
		/// </summary>
		/// <param name="webWidth">WebView width</param>
		/// <param name="webHeight">WebView height</param>
		/// <param name="texWidth">Texture width</param>
		/// <param name="texHeight">Texture height</param>
		public void Init(
			int webWidth, int webHeight,
			int texWidth, int texHeight)
		{
			InitResolution(webWidth, webHeight, texWidth, texHeight);

			Init();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="url">URL that loads first</param>
		/// <param name="dlOption">The directory of the device to which the content is being downloaded.</param>
		/// <param name="subDir">Subdirectory of the directory from which the content is downloaded.</param>
		public void InitOption(
			string url, DownloadOption dlOption, string subDir)
		{
			m_url = url;

			m_dlOption = dlOption;

			m_dlSubDir = subDir;
		}

		/// <summary>
		/// Launch initialize task if WebView is not initialized yet.
		/// </summary>
		/// <param name="webWidth">WebView width</param>
		/// <param name="webHeight">WebView height</param>
		/// <param name="texWidth">Texture width</param>
		/// <param name="texHeight">Texture height</param>
		/// <param name="url">URL that loads first</param>
		/// <param name="dlOption">The directory of the device to which the content is being downloaded</param>
		/// <param name="subDir">Subdirectory of the directory from which the content is downloaded</param>
		public void Init(
			int webWidth, int webHeight,
			int texWidth, int texHeight,
			string url, DownloadOption dlOption, string subDir)
		{
			InitOption(url, dlOption, subDir);

			Init(webWidth, webHeight, texWidth, texHeight);
		}

		/// <summary>
		/// If it returns true, this WebView component is already initialized.
		/// </summary>
		/// <returns>Whether or not this WebView component is initialized</returns>
		public bool IsInitialized()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			if (m_NativePlugin != null)
			{
				return m_NativePlugin.Call<bool>("IsInitialized");
			}

			return false;
#else
			return false;
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator InitTask()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			// I cannot find the way to preload (load on startup)
			// jni shared library. so call library function and
			// load dinamically here. (call unity plugin on load)
			if (SystemInfo.renderingThreadingMode == UnityEngine.Rendering.RenderingThreadingMode.MultiThreaded)
			{
				GL.IssuePluginEvent(NativePlugin.DummyRenderEventFunc(), 0);
			}
			else
			{
				NativePlugin.DummyRenderEvent(0);
			}
#endif

			//Debug.Log(THIS_NAME + "State.INITIALISING");

			m_state = State.INITIALISING;

			yield return new WaitForEndOfFrame();

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG

			m_NativePlugin = new AndroidJavaObject("com.tlab.libwebview.UnityConnect");

			if ((m_captureMode != CaptureMode.SURFACE) && (m_rawImage != null))
			{
				m_loadingView = Texture2D.linearGrayTexture;
				m_contentView = null;

				m_rawImage.texture = m_loadingView;
			}

			m_rawObject = m_NativePlugin.GetRawObject();

			if (m_NativePlugin != null)
			{
				SetDownloadOption(m_dlOption);
				SetDownloadSubDir(m_dlSubDir);

				SetOnPageFinish(m_jsEventCallback.onPageFinish);
				SetOnDownloadStart(m_jsEventCallback.dlEvent.onStart);
				SetOnDownloadFinish(m_jsEventCallback.dlEvent.onFinish);

				SetIntentFilters(m_intentFilters);

				SetOnCatchDownloadUrl(
					m_jsEventCallback.catchDlUrlEvent.go,
					m_jsEventCallback.catchDlUrlEvent.func);

				SetDownloadEventVariableName(
					m_jsEventCallback.dlEvent.varDlUrlName,
					m_jsEventCallback.dlEvent.varDlUriName,
					m_jsEventCallback.dlEvent.varDlIdName);

				var isVulkan = (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Vulkan);

				switch (m_captureMode)
				{
					case CaptureMode.HARDWARE_BUFFER:
						m_updateFrameFunc = isVulkan ? UpdateVulkanFrame : UpdateGLESFrame;
						break;
					case CaptureMode.BYTE_BUFFER:
						m_updateFrameFunc = UpdateFrameWithByteBuffer;
						break;
					case CaptureMode.SURFACE:
						m_updateFrameFunc = UpdateFrameDummy;
						break;
				}

				m_NativePlugin.Call("initialize",
					m_webWidth, m_webHeight,
					m_texWidth, m_texHeight,
					m_screenFullRes.x, m_screenFullRes.y,
					m_url, isVulkan, (int)m_captureMode, m_useCustomWidget, m_fps);
			}

			while (!IsInitialized())
			{
				yield return new WaitForEndOfFrame();
			}

			m_state = State.INITIALIZED;
#endif

			//Debug.Log(THIS_NAME + "State.INITIALIZED");
		}

		//
		// Javascript
		//

		/// <summary>
		/// Retrieve buffers that allocated in order to map javascript buffer to java 
		/// <see href="https://github.com/TLabAltoh/TLabWebView/blob/master/Runtime/Test/DownloadEventTest.cs">example is here</see>
		/// </summary>
		/// <param name="key">Name of buffers that allocated in order to map javascript buffer to java</param>
		/// <returns>current buffer value</returns>
		public byte[] GetWebBuffer(string key)
		{
			if (m_state != State.INITIALIZED)
			{
				return new byte[0];
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<byte[]>("getWebBuffer", key);
#else
			return new byte[0];
#endif
		}

		/// <summary>
		/// Run javascript on the current web page.
		/// <see href="https://github.com/TLabAltoh/TLabWebView/blob/master/Scripts/Sample/WebViewSample.cs">example is here</see>
		/// </summary>
		/// <param name="js">javascript</param>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// // See sample https://github.com/TLabAltoh/TLabWebView/blob/master/Scripts/Sample/WebViewSample.cs
		/// // The destination must be a function that takes a character type as an argument.
		/// function()
		/// {
		///	    window.TLabWebViewActivity.unitySendMessage(String go, String method, String message)
		/// }
		/// ]]>
		/// </code>
		/// </example>
		public void EvaluateJS(string js)
		{
			EvaluateJS("");
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("evaluateJS", js);
#endif
		}

		//
		// View to HardwareBuffer Renderer
		//

		/// <summary>
		/// Return the texture pointer of the WebView frame (NOTE: In Vulkan, the VkImage pointer returned by this function could not be used for UpdateExternalTexture. This issue has not been fixed).
		/// </summary>
		/// <returns>texture pointer of the webview frame (Vulkan: VkImage, OpenGLES: TexID)</returns>
		public IntPtr GetPlatformTextureID()
		{
			if (m_state != State.INITIALIZED)
			{
				return IntPtr.Zero;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return NativePlugin.GetPlatformTextureID((int)m_rawObject);
#else
			return IntPtr.Zero;
#endif
		}

		//
		// URL
		//

		/// <summary>
		/// Get current url that the webview instance is loading.
		/// </summary>
		/// <returns>Current url that the webview instance is loading</returns>
		public string GetUrl()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>("getCurrentUrl");
#else
			return null;
#endif
		}

		/// <summary>
		/// Loads the given URL.
		/// </summary>
		/// <param name="url">The URL of the resource to load</param>
		public void LoadUrl(string url)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("loadUrl", url);
#endif
		}

		/// <summary>
		/// Register Javascript to run when the page is finished loading.
		/// </summary>
		/// <param name="onPageFinish">javascript</param>
		public void SetOnPageFinish(string onPageFinish)
		{
			m_jsEventCallback.onPageFinish = onPageFinish;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(
				"setOnPageFinish",
				m_jsEventCallback.onPageFinish);
#endif
		}

		/// <summary>
		/// Register url patterns to treat as deep links.
		/// </summary>
		/// <param name="filters">Url patterns that are treated as deep links (regular expression)</param>
		public void SetIntentFilters(string[] filters)
		{
			m_intentFilters = filters;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setIntentFilters", filters);
#endif
		}

		//
		// HTML
		//

		/// <summary>
		/// Gaptures HTML currently displayed async.
		/// </summary>
		public void CaptureHTMLSource()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("capturePage");
#endif
		}

		/// <summary>
		/// Capture specific HTML elements currently displayed async.
		/// </summary>
		/// <param name="id">Target HTML element tag</param>
		public void CaptureElementById(string id)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("captureElementById", id);
#endif
		}

		/// <summary>
		/// Gets the HTML value currently captured.
		/// </summary>
		/// <returns>HTML currently captured</returns>
		public string CurrentHTMLCaptured()
		{
			if (m_state != State.INITIALIZED)
			{
				return null;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>("getCaptured");
#else
			return null;
#endif
		}

		/// <summary>
		/// Loads the given HTML.
		/// </summary>
		/// <param name="html">The HTML of the resource to load</param>
		/// <param name="baseURL">baseURL</param>
		public void LoadHTML(string html, string baseURL)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("loadHtml", html, baseURL);
#endif
		}

		//
		// UserAgent
		//

		/// <summary>
		/// Capture current userAgent async.
		/// </summary>
		public void CaptureUserAgent()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("captureUserAgent");
#endif
		}

		/// <summary>
		/// Gets the currently captured userAgent string.
		/// </summary>
		/// <returns>UserAgent String that is currently being captured</returns>
		public string GetUserAgent()
		{
			if (m_state != State.INITIALIZED)
			{
				return "";
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>("getUserAgent");
#else
			return "";
#endif
		}

		/// <summary>
		/// Update userAgent with the given userAgent string.
		/// </summary>
		/// <param name="ua">UserAgent string</param>
		/// <param name="reload">If true, reload web page when userAgent is updated</param>
		public void SetUserAgent(string ua, bool reload)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setUserAgent", ua, reload);
#endif
		}

		//
		// Zoom IN/OUT
		//

		/// <summary>
		/// Performs zoom in in this WebView.
		/// </summary>
		public void ZoomIn()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("zoomIn");
#endif
		}

		/// <summary>
		/// Performs zoom out in this WebView.
		/// </summary>
		public void ZoomOut()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("zoomOut");
#endif
		}

		//
		// Scroll
		//

		/// <summary>
		/// Get content's scroll position x.
		/// </summary>
		/// <returns>Page content's current scroll position x</returns>
		public int GetScrollX()
		{
			if (m_state != State.INITIALIZED)
			{
				return 0;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<int>("getScrollX");
#else
			return 0;
#endif
		}

		/// <summary>
		/// Get content's scroll position y.
		/// </summary>
		/// <returns>Page content's current scroll position y</returns>
		public int GetScrollY()
		{
			if (m_state != State.INITIALIZED)
			{
				return 0;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<int>("getScrollY");
#else
			return 0;
#endif
		}

		/// <summary>
		/// Set content's scroll position.
		/// </summary>
		/// <param name="x">Scroll position x of the destination</param>
		/// <param name="y">Scroll position y of the destination</param>
		public void ScrollTo(int x, int y)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("scrollTo", x, y);
#endif
		}

		/// <summary>
		/// Move the scrolled position of webview.
		/// </summary>
		/// <param name="x">The amount of pixels to scroll by horizontally</param>
		/// <param name="y">The amount of pixels to scroll by vertically</param>
		public void ScrollBy(int x, int y)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("scrollBy", x, y);
#endif
		}

		/// <summary>
		/// Scrolls the contents of this WebView up by half the view size.
		/// </summary>
		/// <param name="top">True to jump to the top of the page</param>
		public void PageUp(bool top)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("pageUp", top);
#endif
		}

		/// <summary>
		/// Scrolls the contents of this WebView down by half the page size.
		/// </summary>
		/// <param name="bottom">True to jump to bottom of page</param>
		public void PageDown(bool bottom)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("pageDown", bottom);
#endif
		}

		//
		// Go forward/back
		//

		/// <summary>
		/// Goes forward in the history of this WebView.
		/// </summary>
		public void GoForward()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("goForward");
#endif
		}

		/// <summary>
		/// Goes back in the history of this WebView.
		/// </summary>
		public void GoBack()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("goBack");
#endif
		}

		//
		// Resize
		//

		/// <summary>
		/// Update WebView texture resolution
		/// </summary>
		/// <param name="texWidth">Texture new width</param>
		/// <param name="texHeight">Texture new height</param>
		public void ResizeTex(int texWidth, int texHeight)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

			if (m_rawImage != null)
				m_rawImage.texture = m_loadingView;

			m_texWidth = texWidth;
			m_texHeight = texHeight;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("resizeTex", texWidth, texHeight);
#endif
		}

		/// <summary>
		/// Update WebView resolution.
		/// </summary>
		/// <param name="webWidth">WebView new width</param>
		/// <param name="webHeight">WebView new height</param>
		public void ResizeWeb(int webWidth, int webHeight)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

			if (m_rawImage != null)
				m_rawImage.texture = m_loadingView;

			m_webWidth = webWidth;
			m_webHeight = webHeight;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("resizeWeb", webWidth, webHeight);
#endif
		}

		/// <summary>
		/// Update resolution for both WebView and Texture.
		/// </summary>
		/// <param name="texWidth">Texture new width</param>
		/// <param name="texHeight">Texture new height</param>
		/// <param name="webWidth">WebView new width</param>
		/// <param name="webHeight">WebView new height</param>
		public void Resize(int texWidth, int texHeight, int webWidth, int webHeight)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

			if (m_rawImage != null)
				m_rawImage.texture = m_loadingView;

			m_texWidth = texWidth;
			m_texHeight = texHeight;
			m_webWidth = webWidth;
			m_webHeight = webHeight;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("resize", texWidth, texHeight, webWidth, webHeight);
#endif
		}

		//
		// Surface
		//

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surfce"></param>
		public void SetSurface(IntPtr surfce)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			var surfaceObj = new AndroidJavaObject(surfce);
			m_NativePlugin.Call("setSurface", surfaceObj);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveSurface()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("removeSurface");
#endif
		}

		//
		// TouchEvent
		//

		/// <summary>
		/// Dispatch of a touch event.
		/// </summary>
		/// <param name="x">Touch position x</param>
		/// <param name="y">Touch position y</param>
		/// <param name="eventNum">Touch event type (TOUCH_DOWN: 0, TOUCH_UP: 1, TOUCH_MOVE: 2)</param>
		public void TouchEvent(int x, int y, int eventNum)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("touchEvent", x, y, eventNum);
#endif
		}

		//
		// KeyEvent
		//

		/// <summary>
		/// Dispatch of a basic keycode event.
		/// </summary>
		/// <param name="key">'a', 'b', 'A' ....</param>
		public void KeyEvent(char key)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("keyEvent", key);
#endif
		}

		/// <summary>
		/// Dispatch of a backspace key event.
		/// </summary>
		public void BackSpace()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("backSpaceKey");
#endif
		}

		//
		// Cache
		//

		/// <summary>
		/// Clear WebView Cache.
		/// </summary>
		/// <param name="includeDiskFiles">If false, only the RAM cache will be cleared</param>
		public void ClearCache(bool includeDiskFiles)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearCash", includeDiskFiles);
#endif
		}

		/// <summary>
		/// Clear WebView Cookie.
		/// </summary>
		public void ClearCookie()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearCookie");
#endif
		}

		/// <summary>
		/// Clear WebView History.
		/// </summary>
		public void ClearHistory()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearHistory");
#endif
		}

		//
		// Download
		//

		/// <summary>
		/// Register Javascript to run when download event finishes.
		/// </summary>
		/// <param name="onFinish">javascript</param>
		public void SetOnDownloadFinish(string onFinish)
		{
			m_jsEventCallback.dlEvent.onFinish = onFinish;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(
				"setOnDownloadFinish",
				m_jsEventCallback.dlEvent.onFinish);
#endif
		}

		/// <summary>
		/// Register Javascript to run when download event starts.
		/// </summary>
		/// <param name="onStart">javascript</param>
		public void SetOnDownloadStart(string onStart)
		{
			m_jsEventCallback.dlEvent.onStart = onStart;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(
				"setOnDownloadStart",
				m_jsEventCallback.dlEvent.onStart);
#endif
		}

		/// <summary>
		/// Defines the download event parameter's name. it can be accessed from javascript when a download event occurs.
		/// </summary>
		/// <param name="varDlUrlName">URL of the file to be downloaded</param>
		/// <param name="varDlUriName">The destination for the downloaded file</param>
		/// <param name="varDlIdName">The ID of the download event</param>
		public void SetDownloadEventVariableName(
			string varDlUrlName, string varDlUriName, string varDlIdName)
		{
			m_jsEventCallback.dlEvent.varDlUrlName = varDlUrlName;
			m_jsEventCallback.dlEvent.varDlUriName = varDlUriName;
			m_jsEventCallback.dlEvent.varDlIdName = varDlIdName;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(
				"setDownloadEventVariableName",
				m_jsEventCallback.dlEvent.varDlUrlName,
				m_jsEventCallback.dlEvent.varDlUriName,
				m_jsEventCallback.dlEvent.varDlIdName);
#endif
		}

		/// <summary>
		/// Set up the callback of the on catch download URL with the given parameters.
		/// </summary>
		/// <param name="go">The name of the game object that has the function of the target instance</param>
		/// <param name="func">Target Instance Function Name</param>
		public void SetOnCatchDownloadUrl(string go, string func)
		{
			m_jsEventCallback.catchDlUrlEvent.go = go;
			m_jsEventCallback.catchDlUrlEvent.func = func;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(
				"setOnCatchDownloadUrl",
				m_jsEventCallback.catchDlUrlEvent.go,
				m_jsEventCallback.catchDlUrlEvent.func);
#endif
		}

		/// <summary>
		/// Request file download to Download Manager.
		/// </summary>
		/// <param name="url">The full url to the content that should be downloaded</param>
		/// <param name="userAgent">The user agent to be used for the download</param>
		/// <param name="contentDisposition">Content-disposition http header, if present</param>
		/// <param name="mimetype">The mimetype of the content reported by the server</param>
		public void DownloadFromUrl(string url, string userAgent,
			string contentDisposition, string mimetype)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("downloadFromUrl", url, userAgent, contentDisposition, mimetype);
#endif
		}

		/// <summary>
		/// Set the directory in which the file will be downloaded.
		/// </summary>
		/// <param name="option">Download location for the files</param>
		public void SetDownloadOption(DownloadOption option)
		{
			m_dlOption = option;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setDlOption", (int)m_dlOption);
#endif
		}

		/// <summary>
		/// Specifies the subdirectory from which the files are to be downloaded.
		/// </summary>
		/// <param name="dlSubDir">The subdirectory from which the files are downloaded. This directory is created under the directory specified in DownloadOption</param>
		public void SetDownloadSubDir(string dlSubDir)
		{
			m_dlSubDir = dlSubDir;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setDownloadSubDir", m_dlSubDir);
#endif
		}

		/// <summary>
		/// Asynchronous capture of download event progress.
		/// </summary>
		public void RequestCaptureDownloadProgress()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("requestCaptureDownloadProgress");
#endif
		}

		/// <summary>
		/// Get the progress of the download event currently being recorded.
		/// </summary>
		/// <returns>Current download progress (0 ~ 1)</returns>
		public float GetDownloadProgress()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<float>("getDownloadProgress");
#else
			return 0.0f;
#endif
		}

		//
		//
		//

		/// <summary>
		/// 
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		public bool CheckForPermission(UnityEngine.Android.Permission permission)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission.ToString());
#else
			return false;
#endif
		}

		//
		// UpdateFrame
		//

		/// <summary>
		/// 
		/// </summary>
		private void UpdateSurface()
		{
			// External texture update behaviour
			// OpenGLES: Use the same texture
			// Vulkan: Create new VkImage and copy buffer to new one,
			// Texture Buffer is not shared so in order to update
			// frame, need to call update frame every frame. (Maybe
			// this processing is too heavy)

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			if (SystemInfo.renderingThreadingMode == UnityEngine.Rendering.RenderingThreadingMode.MultiThreaded)
			{
				GL.IssuePluginEvent(NativePlugin.UpdateSurfaceFunc(), (int)m_NativePlugin.GetRawObject());
			}
			else
			{
				NativePlugin.UpdateSurface((int)m_NativePlugin.GetRawObject());
			}
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateGLESFrame()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			UpdateSurface();

			var flag = NativePlugin.ContentExists((int)m_NativePlugin.GetRawObject());
			if (!flag)
				return;

			int rawObject = (int)m_NativePlugin.GetRawObject();

			flag = NativePlugin.GetSharedBufferUpdateFlag(rawObject);
			if (!flag)
			{
				var texID = GetPlatformTextureID();

				// In OpenGLES API, the texture created by
				// CreateExternalTexture has same texture pointer
				// as function's arguments. And texture size is
				// not same with passed as arguments to display
				// (it seems same to native texture size). In
				// OpenGLES API, is buffer allocated for texture
				// size passed as arguments? If so, do I need to
				// pass zero (or one) for argments of texture size
				// to reduce overhead of memory allocation?

				var tmp = Texture2D.CreateExternalTexture(1, 1, TextureFormat.ARGB32, false, false, texID);

				Debug.Log(THIS_NAME + $"[CreateExternalTexture] size: {tmp.width}, {tmp.height}, id: {texID}, {tmp.GetNativeTexturePtr()}");

				NativePlugin.SetHardwareBufferUpdateFlag(rawObject, true);

				m_rawImage.texture = tmp;
				var release = m_contentView;
				if (release != null)
				{
					Destroy(release);
				}
				m_contentView = tmp;
			}
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateVulkanFrame()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			UpdateSurface();

			var flag = NativePlugin.ContentExists((int)m_NativePlugin.GetRawObject());
			if (!flag)
				return;

			int rawObject = (int)m_NativePlugin.GetRawObject();

			flag = NativePlugin.GetSharedBufferUpdateFlag(rawObject);
			if (!flag)
			{
				// Destroy the shared texture and verify that
				// the native plugin no longer references the
				// Unity texture. Because in Vulkan API, native
				// plugin directly copied buffer to Unity texture.

				var tmp = new Texture2D(m_texWidth, m_texHeight, TextureFormat.RGBA32, false, true);

				NativePlugin.SetUnityTextureID(rawObject, (long)tmp.GetNativeTexturePtr());
				NativePlugin.SetHardwareBufferUpdateFlag(rawObject, true);

				m_rawImage.texture = tmp;
				var release = m_contentView;
				if (release != null)
				{
					Destroy(release);
				}
				m_contentView = tmp;
			}
#endif
		}

		/// <summary>
		/// Update frame from CPU side. This function is for non-hardware buffer use case.
		/// </summary>
		private void UpdateFrameWithByteBuffer()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			var flag = NativePlugin.ContentExists((int)m_NativePlugin.GetRawObject());
			if (!flag)
				return;

			var buf = (byte[])(Array)m_NativePlugin.Call<sbyte[]>("getByteBuffer");
			// Because the content is already validated, there is
			// no need to buffer's null validation here.

			if (m_contentView == null)
			{
				m_contentView = new Texture2D(m_texWidth, m_texHeight, TextureFormat.RGBA32, false, true);
			}
			else
			{
				if (m_contentView.width * m_contentView.height * 4 != buf.Length)
				{
					Destroy(m_contentView);

					m_contentView = new Texture2D(m_texWidth, m_texHeight, TextureFormat.RGBA32, false, true);
				}
			}

			m_contentView.LoadRawTextureData(buf);
			m_contentView.Apply();

			m_rawImage.texture = m_contentView;
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateFrameDummy()
		{

		}

		/// <summary>
		/// Request Webview to update frame.
		/// </summary>
		public void UpdateFrame()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_updateFrameFunc.Invoke();
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		private void Destroy()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG

			var deletable = new DeletableNativePlugin(m_NativePlugin);

			m_deletableNativePlugins.Enqueue(deletable);

			m_NativePlugin = null;

			// I need to call destroy in main thread
			// This may delete the external texture sooner than
			// the native plugin's destroy process, but this is
			// currently not a problem.
			Destroy(m_contentView);
			m_contentView = null;

			// I need to call this function on unity's render thread
			// because releaseSharedTexture() call GLES or Vulkan
			// function and it needs to be called on render thread.

			if (SystemInfo.renderingThreadingMode == UnityEngine.Rendering.RenderingThreadingMode.MultiThreaded)
			{
				GL.IssuePluginEvent(DestroyNativePluginHandlePtr, 0);
			}
			else
			{
				DestroyNativePlugin(0);
			}

			m_state = State.DESTROYED;
#endif
		}

		private void OnDestroy()
		{
			Destroy();
		}

		private void OnApplicationQuit()
		{
			Destroy();
		}
	}
}
