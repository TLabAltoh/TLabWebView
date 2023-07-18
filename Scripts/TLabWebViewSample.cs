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
		m_webView.UpdateFrame();
	}
}
