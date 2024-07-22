#define DEBUG
//#undef DEBUG

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

		public int webWidth => m_webWidth;

		public int webHeight => m_webHeight;

		public int texWidth => m_texWidth;

		public int texHeight => m_texHeight;

		public DownloadOption dlOption => m_dlOption;

		public string subDir => m_dlSubDir;

		public State state => m_state;

		private static string THIS_NAME = "[tlabwebview] ";

		public EventCallback jsEventCallback => m_jsEventCallback;

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

		private IntPtr m_prevTexID;

		private delegate void UpdateFrameFunc();

		private UpdateFrameFunc m_updateFrameFunc;

		/// <summary>
		/// 
		/// </summary>
		public void Init()
		{
			if (m_state == State.NONE)
			{
				StartCoroutine(InitTask());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="webWidth"></param>
		/// <param name="webHeight"></param>
		/// <param name="texWidth"></param>
		/// <param name="texHeight"></param>
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
		/// 
		/// </summary>
		/// <param name="webWidth"></param>
		/// <param name="webHeight"></param>
		/// <param name="texWidth"></param>
		/// <param name="texHeight"></param>
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
		/// <param name="url"></param>
		/// <param name="dlOption"></param>
		/// <param name="subDir"></param>
		public void InitOption(
			string url, DownloadOption dlOption, string subDir)
		{
			m_url = url;

			m_dlOption = dlOption;

			m_dlSubDir = subDir;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="webWidth"></param>
		/// <param name="webHeight"></param>
		/// <param name="texWidth"></param>
		/// <param name="texHeight"></param>
		/// <param name="url"></param>
		/// <param name="dlOption"></param>
		/// <param name="subDir"></param>
		public void Init(
			int webWidth, int webHeight,
			int texWidth, int texHeight,
			string url, DownloadOption dlOption, string subDir)
		{
			InitOption(url, dlOption, subDir);

			Init(webWidth, webHeight, texWidth, texHeight);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <returns></returns>
		public IntPtr GetBindedPlatformTextureID()
		{
			if (m_state != State.INITIALIZED)
			{
				return IntPtr.Zero;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return NativePlugin.GetBindedPlatformTextureID((int)m_rawObject);
#else
			return IntPtr.Zero;
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetUrl()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>("getCurrentUrl");
#else
			return null;
#endif
		}

		/// <summary>
		/// 
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
		/// 
		/// </summary>
		/// <param name="id"></param>
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
		/// 
		/// </summary>
		/// <returns></returns>
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
		/// 
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
		/// 
		/// </summary>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <param name="ua"></param>
		/// <param name="reload"></param>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
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
		/// 
		/// </summary>
		/// <param name="html"></param>
		/// <param name="baseURL"></param>
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

		/// <summary>
		/// 
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
		/// 
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
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
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
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
		/// 
		/// </summary>
		/// <param name="js"></param>
		public void EvaluateJS(string js)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("evaluateJS", js);
#endif
		}

		/// <summary>
		/// 
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
		/// 
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="eventNum"></param>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="texWidth"></param>
		/// <param name="texHeight"></param>
		public void ResizeTex(int texWidth, int texHeight)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

			m_texWidth = texWidth;
			m_texHeight = texHeight;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("resizeTex", texWidth, texHeight);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="webWidth"></param>
		/// <param name="webHeight"></param>
		public void ResizeWeb(int webWidth, int webHeight)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

			m_webWidth = webWidth;
			m_webHeight = webHeight;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("resizeWeb", webWidth, webHeight);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="texWidth"></param>
		/// <param name="texHeight"></param>
		/// <param name="webWidth"></param>
		/// <param name="webHeight"></param>
		public void Resize(int texWidth, int texHeight, int webWidth, int webHeight)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

			m_texWidth = texWidth;
			m_texHeight = texHeight;
			m_webWidth = webWidth;
			m_webHeight = webHeight;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("resize", texWidth, texHeight, webWidth, webHeight);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="top"></param>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
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
		/// 
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="includeDiskFiles"></param>
		public void ClearCache(bool includeDiskFiles)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearCash", includeDiskFiles);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public void ClearCookie()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearCookie");
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public void ClearHistory()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearHistory");
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="onPageFinish"></param>
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
		/// 
		/// </summary>
		/// <param name="onFinish"></param>
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
		/// 
		/// </summary>
		/// <param name="onStart"></param>
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
		/// 
		/// </summary>
		/// <param name="varDlUrlName"></param>
		/// <param name="varDlUriName"></param>
		/// <param name="varDlIdName"></param>
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
		/// 
		/// </summary>
		/// <param name="go"></param>
		/// <param name="func"></param>
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
		/// 
		/// </summary>
		/// <param name="filters"></param>
		public void SetIntentFilters(string[] filters)
		{
			m_intentFilters = filters;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setIntentFilters", filters);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="userAgent"></param>
		/// <param name="contentDisposition"></param>
		/// <param name="mimetype"></param>
		public void DownloadFromUrl(string url, string userAgent,
			string contentDisposition, string mimetype)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("downloadFromUrl", url, userAgent, contentDisposition, mimetype);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="option"></param>
		public void SetDownloadOption(DownloadOption option)
		{
			m_dlOption = option;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setDlOption", (int)m_dlOption);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dlSubDir"></param>
		public void SetDownloadSubDir(string dlSubDir)
		{
			m_dlSubDir = dlSubDir;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setDownloadSubDir", m_dlSubDir);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public void RequestCaptureDownloadProgress()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("requestCaptureDownloadProgress");
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public float GetDownloadProgress()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<float>("getDownloadProgress");
#else
			return 0.0f;
#endif
		}

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

			m_rawObject = m_NativePlugin.GetRawObject();

			m_prevTexID = IntPtr.Zero;

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

				m_updateFrameFunc = isVulkan ? UpdateVulkanFrame : UpdateGLESFrame;

				m_NativePlugin.Call("initialize",
					m_webWidth, m_webHeight,
					m_texWidth, m_texHeight,
					Screen.width, Screen.height, m_url, isVulkan);
			}

			while (!IsInitialized())
			{
				yield return new WaitForEndOfFrame();
			}

			m_state = State.INITIALIZED;
#endif

			//Debug.Log(THIS_NAME + "State.INITIALIZED");
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateGLESFrame()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			var texID = GetBindedPlatformTextureID();

			if ((texID != IntPtr.Zero) && (texID != m_prevTexID))
			{
				// Unity's external texture temporarily lacks
				// the native texture reference, but Unity's
				// texture is immediately updated with the
				// existing native texture.

				if (m_rawImage.texture == null)
				{
					m_rawImage.texture = Texture2D.CreateExternalTexture(m_texWidth, m_texHeight, TextureFormat.ARGB32, false, false, texID);
				}
				else
				{
					var tmp0 = (Texture2D)m_rawImage.texture;
					tmp0.UpdateExternalTexture(texID);
				}

				m_prevTexID = texID;
			}
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateVulkanFrame()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			int rawObject = (int)m_NativePlugin.GetRawObject();

			bool flag = NativePlugin.GetSharedBufferUpdateFlag(rawObject);
			if (flag)
			{
				// Destroy the shared texture and verify that
				// the native plugin no longer references the
				// Unity texture.

				var tmp = new Texture2D(m_texWidth, m_texHeight, TextureFormat.RGBA32, false, true);

				NativePlugin.SetUnityTextureID(rawObject, (long)tmp.GetNativeTexturePtr());
				NativePlugin.SetHardwareBufferUpdateFlag(rawObject, false);

				var release = m_rawImage.texture;

				m_rawImage.texture = tmp;

				if (release != null)
				{
					Destroy(release);
				}
			}
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public void UpdateFrame()
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

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
			Destroy(m_rawImage.texture);
			m_rawImage.texture = null;

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
