using UnityEngine;

namespace TLab.Android.WebView
{
	public class TLabWebViewSample : MonoBehaviour
	{
		[SerializeField] private TLabWebView m_webView;

		/// <summary>
		/// 
		/// </summary>
		public void StartWebView()
		{
			m_webView.Init();
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
