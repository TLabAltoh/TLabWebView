using UnityEngine;
using TMPro;

public class Main : MonoBehaviour
{
	[SerializeField] private TLabWebView m_webView;
	[SerializeField] private TLabInputField m_inputField;
	[SerializeField] private TextMeshProUGUI m_FpsText;
	private float m_FpsTimer;
	private int m_FpsCounter;

	public void LoadUrl()
    {
		string url = m_inputField.text;

		if (url == null)
			return;

		Debug.Log("Load url: " + url);

		//https://www.google.com/search?q=youtube
		if(url.Length > 8)
        {
			if (url.Substring(0, 7) == "https://" || url.Substring(0, 6) == "http://")
			{
				m_webView.LoadUrl(url);
			}
		}

		m_webView.LoadUrl("https://www.google.com/search?q=" + url);
    }

	public void StartWebView()
    {
		m_webView.StartWebView();
	}

	public void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		m_FpsTimer = 0;
		m_FpsCounter = 0;
	}

	void Update ()
	{
		m_webView.UpdateFrame();

		m_FpsTimer += Time.deltaTime;
		if (m_FpsTimer >= 1.0f)
		{
			m_FpsText.text = "FPS:" + m_FpsCounter;
			m_FpsTimer -= 1.0f;
			m_FpsCounter = 0;
		}
		m_FpsCounter++;
	}
}
