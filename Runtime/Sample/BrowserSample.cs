using UnityEngine;

namespace TLab.WebView
{
	public class BrowserSample : MonoBehaviour
	{
		[SerializeField] private BrowserContainer m_container;

		private string THIS_NAME => "[" + this.GetType() + "] ";

		void Start() => m_container.browser.Init();

		void Update()
		{
			m_container.browser.UpdateFrame();
			m_container.browser.DispatchMessageQueue();
		}
	}
}
