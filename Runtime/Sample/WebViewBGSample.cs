using UnityEngine;

namespace TLab.Android.WebView
{
	public class WebViewBGSample : MonoBehaviour
	{
		[SerializeField] private TLabWebView m_webView;

		public string THIS_NAME => "[" + this.GetType() + "] ";

		/// <summary>
		/// 
		/// </summary>
		public void StartWebView()
		{
			if (m_webView.state == TLabWebView.State.NONE)
			{
				StartCoroutine(m_webView.InitTask());
			}
		}

		void Start()
		{
			StartWebView();
		}

		void Update()
		{
#if UNITY_ANDROID
			m_webView.UpdateFrame();
#endif
		}
	}
}
