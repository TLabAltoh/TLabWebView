using UnityEngine;

namespace TLab.WebView
{
	public class BrowserBGSample : MonoBehaviour
	{
		[SerializeField] private BrowserContainer m_container;

		public string THIS_NAME => "[" + this.GetType() + "] ";

		public void StartWebView()
		{
			if (m_container.browser.state == FragmentCapture.State.None)
				StartCoroutine(m_container.browser.InitTask());
		}

		void Start() => StartWebView();

		void Update()
		{
			m_container.browser.UpdateFrame();
			m_container.browser.DispatchMessageQueue();
		}
	}
}