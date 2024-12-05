#define DEBUG
#undef DEBUG

using System.Collections.Generic;
using UnityEngine;
using TLab.WebView.Widget;

namespace TLab.WebView
{
	public abstract class Browser : FragmentCapture, IBrowser
	{
		[Header("Web Settings")]
		[SerializeField] private string m_url = "https://youtube.com";
		[SerializeField] private Download.Option m_downloadOption;
		[SerializeField] private EventCallback m_eventCallback;
		[SerializeField] private string[] m_intentFilters;

		public string url => m_url;

		public Download.Option downloadOption => m_downloadOption;

		public string[] intentFilters => m_intentFilters;

		public EventCallback eventCallback => m_eventCallback;

		private string THIS_NAME => "[" + this.GetType() + "] ";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="url">URL that loads first</param>
		/// <param name="fps">Fps fo rendering</param>
		/// <param name="downloadOption">The directory of the device to which the content is being downloaded.</param>
		public void InitOption(string url, int fps, Download.Option downloadOption)
		{
			m_url = url;
			m_fps = fps;
			m_downloadOption = downloadOption;
		}

		/// <summary>
		/// Launch initialize task if WebView is not initialized yet.
		/// </summary>
		/// <param name="viewSize">Web Size</param>
		/// <param name="texSize">Tex Size</param>
		/// <param name="url">URL that loads first</param>
		/// <param name="fps">Fps fo rendering</param>
		/// <param name="downloadOption">The directory of the device to which the content is being downloaded</param>
		public void Init(Vector2Int viewSize, Vector2Int texSize, string url, int fps, Download.Option downloadOption)
		{
			InitOption(url, fps, downloadOption);

			Init(viewSize, texSize);
		}

		protected override void InitNativePlugin()
		{
			SetDownloadOption(m_downloadOption);

			SetIntentFilters(m_intentFilters);

			SetFps(m_fps);

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(InitNativePlugin),
				m_viewSize.x, m_viewSize.y,
				m_texSize.x, m_texSize.y,
				m_screenFullRes.x, m_screenFullRes.y,
				m_url, m_isVulkan, (int)m_captureMode);
#endif
		}

		public void EvaluateJS(string js)
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(EvaluateJS), js);
#endif
		}

		public string GetUrl()
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>(nameof(GetUrl));
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
			if (m_state != State.Initialized)
				return;

			m_url = url;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(LoadUrl), url);
#endif
		}

		public void SetIntentFilters(string[] filters)
		{
			m_intentFilters = filters;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(SetIntentFilters), filters);
#endif
		}

		public string GetAsyncResult(int id)
		{
			if (m_state != State.Initialized) return "";

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<string>(nameof(GetAsyncResult), id);
#else
			return "";
#endif
		}

		public void CancelAsyncResult(int id)
		{
			if (m_state != State.Initialized) return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(CancelAsyncResult), id);
#endif
		}

		public IEnumerator<AsyncString> GetUserAgent()
		{
			if (m_state != State.Initialized)
				yield break;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			var id = m_NativePlugin.Call<int>(nameof(GetUserAgent));
			if (id == -1) yield break;
			while (true)
			{
				var @object = GetAsyncResult(id);
				if (@object == "")
				{
					yield return new AsyncString(null, JavaAsyncResult.Status.WAITING);
					continue;
				}
				var result = new JavaAsyncResult(@object);
				yield return new AsyncString(result.s, JavaAsyncResult.Status.COMPLETE);
				break;
			}
			yield break;
#else
			yield break;
#endif
		}

		public void SetUserAgent(string ua, bool reload)
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(SetUserAgent), ua, reload);
#endif
		}

		public int GetScrollX()
		{
			if (m_state != State.Initialized)
				return 0;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<int>(nameof(GetScrollX));
#else
			return 0;
#endif
		}

		public int GetScrollY()
		{
			if (m_state != State.Initialized)
				return 0;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<int>(nameof(GetScrollY));
#else
			return 0;
#endif
		}

		public void ScrollTo(int x, int y)
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(ScrollTo), x, y);
#endif
		}

		public void ScrollBy(int x, int y)
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(ScrollBy), x, y);
#endif
		}

		public void DispatchMessageQueue()
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			var result = m_NativePlugin.Call<string[]>(nameof(DispatchMessageQueue));
			foreach (var @object in result)
			{
				var message = new EventCallback.Message(@object);
				switch ((EventCallback.Type)message.type)
				{
					case EventCallback.Type.OnPageFinish:
						m_eventCallback.onPageFinish.Invoke(message.payload);
						break;
					case EventCallback.Type.OnDownload:
						{
							m_eventCallback.onDownload.Invoke(new Download.Request(message.payload));
						}
						break;
					case EventCallback.Type.OnDownloadStart:
						{
							m_eventCallback.onDownloadStart.Invoke(new Download.EventInfo(message.payload));
						}
						break;
					case EventCallback.Type.OnDownloadError:
						{
							m_eventCallback.onDownloadError.Invoke(new Download.EventInfo(message.payload));
						}
						break;
					case EventCallback.Type.OnDownloadFinish:
						{
							m_eventCallback.onDownloadFinish.Invoke(new Download.EventInfo(message.payload));
						}
						break;
					case EventCallback.Type.OnDialog:
						{
							m_eventCallback.onDialog.Invoke(new AlertDialog.Init(message.payload), null);
						}
						break;
				}
			}
#endif
		}

		public void GoForward()
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(GoForward));
#endif
		}

		public void GoBack()
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(GoBack));
#endif
		}

		public long TouchEvent(int x, int y, int action, long downTime)
		{
			if (m_state != State.Initialized)
				return 0;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<long>(nameof(TouchEvent), x, y, action, downTime);
#else
			return 0;
#endif
		}

		public void KeyEvent(char key)
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(KeyEvent), key);
#endif
		}

		public void KeyEvent(int keyCode)
		{
			if (m_state != State.Initialized)
				return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(KeyEvent), keyCode);
#endif
		}

		public void DownloadFromUrl(string url, string userAgent,
			string contentDisposition, string mimetype)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(DownloadFromUrl), url, userAgent, contentDisposition, mimetype);
#endif
		}

		public void DownloadFromUrl(Download.Request request) => DownloadFromUrl(request.url, request.userAgent, request.contentDisposition, request.mimeType);

		public void SetDownloadOption(Download.Option downloadOption)
		{
			m_downloadOption = downloadOption;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(SetDownloadOption), (int)m_downloadOption.directory, m_downloadOption.subDirectory);
#endif
		}

		public float GetDownloadProgress(long id)
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			return m_NativePlugin.Call<float>(nameof(GetDownloadProgress), id);
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

		public void PostDialogResult(AlertDialog.Result result, string json = "")
		{
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
			m_NativePlugin.Call(nameof(PostDialogResult), (int)result, json != null ? json : "");
#endif
		}
	}
}
