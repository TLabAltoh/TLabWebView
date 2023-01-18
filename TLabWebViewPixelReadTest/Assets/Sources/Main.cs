using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
	private static Main instance = null;

	private Text m_FpsText;
	private float m_FpsTimer;
	private int m_FpsCounter;

	public void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		// ---------------------------------------------------------------------------
		// Get state log text
		//

		var state = GameObject.Find("RenderCanvas/State").GetComponent<Text>();
		state.text = "Text found";

		// ---------------------------------------------------------------------------
		// FPS Settings
		//

		m_FpsText = GameObject.Find("RenderCanvas/FPS").GetComponent<Text>();

		m_FpsTimer = 0;
		m_FpsCounter = 0;

		Application.targetFrameRate = 60;

		// ---------------------------------------------------------------------------
		// Get this class's instance
		//

		instance = this;
	}

	void Update ()
	{
		// ----------------------------------------------------------------
		// Count frame rate
		//

		m_FpsTimer += Time.deltaTime;
		if (m_FpsTimer >= 1.0f)
		{
			m_FpsText.text = "FPS:" + m_FpsCounter;
			m_FpsTimer -= 1.0f;
			m_FpsCounter = 0;
		}
		m_FpsCounter++;
	}

	public void OnDisable()
	{
		if (instance == this) instance = null;
	}
}
