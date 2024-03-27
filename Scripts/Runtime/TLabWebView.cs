#define DEBUG
#undef DEBUG

using System.Collections;
using System;
using System.Runtime.InteropServices;
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
			applicationFolder,

			/// <summary>
			/// https://developer.android.com/reference/android/os/Environment#DIRECTORY_DOWNLOADS
			/// </summary>
			downloadFolder
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

		public int WebWidth { get => m_webWidth; }

		public int WebHeight { get => m_webHeight; }

		public int TexWidth { get => m_texWidth; }

		public int TexHeight { get => m_texHeight; }

		public DownloadOption DlOption { get => m_dlOption; }

		public string SubDir { get => m_subDir; }

		public JsEventCallback jsEventCallback { get => m_jsEventCallback; }

		public bool Visuble
		{
			get => m_enabled;
			set
			{
				m_enabled = value && m_initialized;
			}
		}

		public bool Destroyed { get => m_destroyed; }

		private bool m_destroyed = false;
		private bool m_enabled = false;
		private bool m_initialized = false;
		private Texture2D m_webViewTexture;
		private Coroutine m_webviewInitTask;

		private IntPtr m_texId = IntPtr.Zero;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
		private static AndroidJavaClass m_NativeClass;
		private AndroidJavaObject m_NativePlugin;
#endif

		private delegate void CheckEGLContextExist();
		private delegate void RenderEventDelegate(int eventIDint);
		private static RenderEventDelegate RenderThreadHandle = new RenderEventDelegate(RunOnRenderThread);
		private static IntPtr RenderThreadHandlePtr = Marshal.GetFunctionPointerForDelegate(RenderThreadHandle);

		private static jvalue[] m_jniArgs;

		[AOT.MonoPInvokeCallback(typeof(RenderEventDelegate))]
		private static void RunOnRenderThread(int eventID)
		{
			/*
			 * Perhaps Java Env is already attached to the render thread.
			 */

			//AndroidJNI.AttachCurrentThread();

			IntPtr jniClass = AndroidJNI.FindClass("com/tlab/libwebview/UnityConnect");
			if (jniClass == IntPtr.Zero || jniClass == null)
			{
				Debug.Log($"jni class not found: {jniClass}");
				return;
			}

			IntPtr jniFunc;
			jniFunc = AndroidJNI.GetStaticMethodID(jniClass, "generateSharedTexture", "(II)V");
			if (jniFunc == IntPtr.Zero || jniFunc == null)
			{
				Debug.Log($"jni function not found !:{jniFunc} ");
				return;
			}

			AndroidJNI.CallStaticVoidMethod(jniClass, jniFunc, m_jniArgs);

			//AndroidJNI.DetachCurrentThread();
		}

		public void Init()
		{
			if (m_webviewInitTask == null && !m_initialized)
			{
				m_webviewInitTask = StartCoroutine(InitTask());
			}
		}

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

		private void UpdateSurface()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("updateSurface");
#endif
		}

		/// <summary>
		/// This function is obsolete. It returns an array of 1 elements
		/// </summary>
		/// <returns></returns>
		public byte[] GetWebTexturePixel()
		{
			// If textures can be updated with a pointer, this will not be used.

			if (!m_enabled)
			{
				return new byte[0];
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return (byte[])(Array)m_NativePlugin.Call<sbyte[]>("getPixel");
#else
			return new byte[0];
#endif
		}

		public IntPtr GetTexturePtr()
		{
			if (!m_enabled)
			{
				return IntPtr.Zero;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return (IntPtr)m_NativePlugin.Call<int>("getTexturePtr");
#else
			return IntPtr.Zero;
#endif
		}

		public string GetUrl()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>("getCurrentUrl");
#else
			return null;
#endif
		}

		public void CaptureHTMLSource()
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("capturePage");
#endif
		}

		public void CaptureElementById(string id)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("captureElementById", id);
#endif
		}

		public string CurrentHTMLCaptured()
		{
			if (!m_enabled)
			{
				return null;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>("getCaptured");
#else
			return null;
#endif
		}

		public void CaptureUserAgent()
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("captureUserAgent");
#endif
		}

		public string GetUserAgent()
		{
			if (!m_enabled)
			{
				return "";
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>("getUserAgent");
#else
			return "";
#endif
		}

		public void SetUserAgent(string ua, bool reload)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setUserAgent", ua, reload);
#endif
		}

		public void LoadUrl(string url)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("loadUrl", url);
#endif
		}

		public void LoadHTML(string html, string baseURL)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("loadHtml", html, baseURL);
#endif
		}

		public void ZoomIn()
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("zoomIn");
#endif
		}

		public void ZoomOut()
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("zoomOut");
#endif
		}

		public void RegisterOnPageFinishCallback(string js)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("registerOnPageFinishCallback", js);
#endif
		}

		public void EvaluateJS(string js)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("evaluateJS", js);
#endif
		}

		public void GoForward()
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("goForward");
#endif
		}

		public void GoBack()
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("goBack");
#endif
		}

		public void TouchEvent(int x, int y, int eventNum)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("touchEvent", x, y, eventNum);
#endif
		}

		public void KeyEvent(char key)
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("keyEvent", key);
#endif
		}

		public void BackSpace()
		{
			if (!m_enabled)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("backSpaceKey");
#endif
		}

		public void SetVisible(bool visible)
		{
			Visuble = visible;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setVisible", visible);
#endif
		}

		public void ClearCache(bool includeDiskFiles)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearCash", includeDiskFiles);
#endif
		}

		public void ClearCookie()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearCookie");
#endif
		}

		public void ClearHistory()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("clearHistory");
#endif
		}

		public void SetOnPageFinish(string onPageFinish)
		{
			m_jsEventCallback.onPageFinish = onPageFinish;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setOnPageFinish", m_jsEventCallback.onPageFinish);
#endif
		}

		public void SetOnDownloadFinish(string onDownloadFinish)
		{
			m_jsEventCallback.onDownloadFinish = onDownloadFinish;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setOnDownloadFinish", m_jsEventCallback.onDownloadFinish);
#endif
		}

		public void SetOnDownloadStart(string onDownloadStart)
		{
			m_jsEventCallback.onDownloadStart = onDownloadStart;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setOnDownloadStart", m_jsEventCallback.onDownloadStart);
#endif
		}

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

		public void SetDlOption(DownloadOption option)
		{
			m_dlOption = option;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setDlOption", (int)m_dlOption);
#endif
		}

		public void SetSubDir(string subdir)
		{
			m_subDir = subdir;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("setSubDir", m_subDir);
#endif
		}

		public void RequestCaptureDownloadProgress()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call("requestCaptureDownloadProgress");
#endif
		}

		public float GetDownloadProgress()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<float>("getDownloadProgress");
#else
			return 0.0f;
#endif
		}

		public bool CheckForPermission(UnityEngine.Android.Permission permission)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission.ToString());
#else
			return false;
#endif
		}

		public void RequestPermission(UnityEngine.Android.Permission permission)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			UnityEngine.Android.Permission.RequestUserPermission(permission.ToString());
#endif
		}

		private static void SetJNIParam(int[] args)
		{
			m_jniArgs = new jvalue[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				m_jniArgs[i] = new jvalue { i = args[i] };
			}
		}

		private IEnumerator InitTask()
		{
			yield return new WaitForEndOfFrame();

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin = new AndroidJavaObject("com.tlab.libwebview.UnityConnect");

			yield return new WaitForEndOfFrame();

			switch (SystemInfo.renderingThreadingMode)
			{
				case UnityEngine.Rendering.RenderingThreadingMode.MultiThreaded:
					SetJNIParam(new int[] { m_texWidth, m_texHeight });
					GL.IssuePluginEvent(RenderThreadHandlePtr, 0);
					break;
				default:
					m_NativePlugin.CallStatic("generateSharedTexture", m_texWidth, m_texHeight);
					break;
			}

			yield return new WaitForEndOfFrame();

			m_webViewTexture = new Texture2D(m_texWidth, m_texHeight, TextureFormat.ARGB32, false, false);
			m_webViewTexture.name = "WebImage";

			m_rawImage.texture = m_webViewTexture;

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

			yield return new WaitForEndOfFrame();

			while (!IsInitialized())
			{
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForEndOfFrame();

			m_initialized = true;
			m_enabled = true;

			m_webviewInitTask = null;
#endif
		}

		public void UpdateFrame()
		{
			if (!m_enabled)
			{
				return;
			}

			IntPtr texId = GetTexturePtr();
			if (m_texId != texId)
			{
				m_texId = texId;
				m_webViewTexture.UpdateExternalTexture(texId);
			}
		}

		private void Awake()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			if (m_NativeClass == null)
			{
				m_NativeClass = new AndroidJavaClass("com.tlab.libwebview.UnityConnect");
			}
#endif
		}

		private void Destroy()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			if (m_NativePlugin == null)
			{
				return;
			}

			m_NativePlugin.Call("Destroy");
			m_NativePlugin = null;
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
