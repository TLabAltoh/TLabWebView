using UnityEngine;
using TMPro;

public class TLabWebViewSample : MonoBehaviour
{
	[SerializeField] private TLabWebView m_webView;

	public void StartWebView()
    {
		m_webView.StartWebView();
	}

	void Start()
	{
		StartWebView();
	}

	void Update ()
	{
#if UNITY_ANDROID
		m_webView.UpdateFrame();
#endif
	}
}
