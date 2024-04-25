#define DEBUG
#undef DEBUG

using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace TLab.Android.WebView
{
	[System.Serializable]
	public class JsEventCallback
	{
		[SerializeField] public string onPageFinish;
		[SerializeField] public string onDownloadStart;
		[SerializeField] public string onDownloadFinish;

		[SerializeField] public string dl_uri_name = "unity_webview_dl_uri";
		[SerializeField] public string dl_url_name = "unity_webview_dl_url";
		[SerializeField] public string dl_id_name = "unity_webview_dl_id";
	}

	public class TLabWebView : MonoBehaviour
	{
		public enum DownloadOption
		{
			/// <summary>
			/// https://developer.android.com/reference/android/app/DownloadManager.Request#setDestinationInExternalFilesDir(android.content.Context,%20java.lang.String,%20java.lang.String)
			/// </summary>
			APPLICATION_FOLDER,

			/// <summary>
			/// https://developer.android.com/reference/android/os/Environment#DIRECTORY_DOWNLOADS
			/// </summary>
			DOWNLOAD_FOLDER
		}

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
		[SerializeField] private string m_subDir = "downloads";

		[Header("Resolution setting")]
		[SerializeField] private int m_webWidth = 1024;
		[SerializeField] private int m_webHeight = 1024;
		[SerializeField] private int m_texWidth = 512;
		[SerializeField] private int m_texHeight = 512;

		[Header("Javascript callback")]
		[SerializeField] private JsEventCallback m_jsEventCallback = new JsEventCallback();

		public int webWidth => m_webWidth;

		public int webHeight => m_webHeight;

		public int texWidth => m_texWidth;

		public int texHeight => m_texHeight;

		public DownloadOption dlOption => m_dlOption;

		public string subDir => m_subDir;

		public State state => m_state;

		private static string THIS_NAME = "[tlabwebview] ";

		public JsEventCallback jsEventCallback => m_jsEventCallback;

		private State m_state = State.NONE;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
		private AndroidJavaObject m_NativePlugin;

		private IntPtr m_rawObject;
#endif

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
		public void Init(
			int webWidth, int webHeight,
			int texWidth, int texHeight)
		{
			m_webWidth = webWidth;
			m_webHeight = webHeight;

			m_texWidth = texWidth;
			m_texHeight = texHeight;

			Init();
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
			m_url = url;

			m_dlOption = dlOption;

			m_subDir = subDir;

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

		/// <summary>
		/// 
		/// </summary>
		private void UpdateSurface()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("updateSurface");
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IntPtr GetTexturePtr()
		{
			if (m_state != State.INITIALIZED)
			{
				return IntPtr.Zero;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return JNIUtil.GetTexturePtr((int)m_rawObject);
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
		public void SetScroll(int x, int y)
		{
			if (m_state != State.INITIALIZED)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setScroll", x, y);
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
			m_NativePlugin.Call("setOnPageFinish", m_jsEventCallback.onPageFinish);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="onDownloadFinish"></param>
		public void SetOnDownloadFinish(string onDownloadFinish)
		{
			m_jsEventCallback.onDownloadFinish = onDownloadFinish;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setOnDownloadFinish", m_jsEventCallback.onDownloadFinish);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="onDownloadStart"></param>
		public void SetOnDownloadStart(string onDownloadStart)
		{
			m_jsEventCallback.onDownloadStart = onDownloadStart;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setOnDownloadStart", m_jsEventCallback.onDownloadStart);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dl_url_name"></param>
		/// <param name="dl_uri_name"></param>
		/// <param name="dl_id_name"></param>
		public void SetDlEventVariableName(
			string dl_url_name, string dl_uri_name, string dl_id_name)
		{
			m_jsEventCallback.dl_url_name = dl_url_name;
			m_jsEventCallback.dl_uri_name = dl_uri_name;
			m_jsEventCallback.dl_id_name = dl_id_name;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(
				"setDownloadEventVariableName",
				m_jsEventCallback.dl_url_name,
				m_jsEventCallback.dl_uri_name,
				m_jsEventCallback.dl_id_name);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="option"></param>
		public void SetDlOption(DownloadOption option)
		{
			m_dlOption = option;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setDlOption", (int)m_dlOption);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subdir"></param>
		public void SetSubDir(string subdir)
		{
			m_subDir = subdir;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setSubDir", m_subDir);
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
		private IEnumerator InitTask()
		{
			m_state = State.INITIALISING;

			yield return new WaitForEndOfFrame();

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin = new AndroidJavaObject("com.tlab.libwebview.UnityConnect");

			m_rawObject = m_NativePlugin.GetRawObject();

			m_rawImage.texture = new Texture2D(m_texWidth, m_texHeight, TextureFormat.ARGB32, false, false);

			if (m_NativePlugin != null)
			{
				SetDlOption(m_dlOption);
				SetSubDir(m_subDir);

				SetOnPageFinish(m_jsEventCallback.onPageFinish);
				SetOnDownloadStart(m_jsEventCallback.onDownloadStart);
				SetOnDownloadFinish(m_jsEventCallback.onDownloadFinish);

				SetDlEventVariableName(
					m_jsEventCallback.dl_url_name,
					m_jsEventCallback.dl_uri_name,
					m_jsEventCallback.dl_id_name);

				m_NativePlugin.Call("initialize",
					m_webWidth, m_webHeight,
					m_texWidth, m_texHeight,
					Screen.width, Screen.height, m_url);
			}

			while (!IsInitialized())
			{
				yield return new WaitForEndOfFrame();
			}

			m_state = State.INITIALIZED;
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

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			if (SystemInfo.renderingThreadingMode == UnityEngine.Rendering.RenderingThreadingMode.MultiThreaded)
			{
				GL.IssuePluginEvent(JNIUtil.UpdateSurfaceFunc(), (int)m_NativePlugin.GetRawObject());
			}
			else
			{
				JNIUtil.UpdateSurface((int)m_NativePlugin.GetRawObject());
			}
#endif

			var newPtr = GetTexturePtr();
			var oldPtr = m_rawImage.texture.GetNativeTexturePtr();

			if ((newPtr != IntPtr.Zero) && (newPtr != oldPtr))
			{
				((Texture2D)m_rawImage.texture).UpdateExternalTexture(newPtr);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void Destroy()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			if (m_NativePlugin == null)
			{
				return;
			}

			m_NativePlugin.Call("Destroy");
			m_NativePlugin = null;

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
