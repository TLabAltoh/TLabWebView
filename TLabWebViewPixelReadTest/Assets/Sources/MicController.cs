using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

public class MicController : MonoBehaviour
{
	public enum Status
	{
		UnInitialized,
		Initializing,
		Idle,
		AccessDenied,
		Recording,
		NoDevice,
	}

	public AudioMixerGroup Mixer;

	private AudioSource m_AudioSource;
	private Status m_State;
	private bool m_StartOnInitialized;
	private List<byte> m_Data;
	private int m_MaxCapacity = 64 * 1024;
	private bool m_MicActive;
	private float m_Average;

	// 初期化
	public void Initialize()
	{
		StartCoroutine(StartMicrophone());
	}

	public bool micActive
	{
		set
		{
			m_MicActive = value;
			if (m_State == Status.Idle)
			{
				if (m_MicActive)
				{
					m_AudioSource.Play();
				}
				else
				{
					m_AudioSource.Stop();
				}
			}
		}
		get
		{
			return m_MicActive;
		}
	}

	// 初期化済みかどうか
	public Status GetStatus()
	{
		return m_State;
	}

	public bool startOnInitialized
	{
		get
		{
			return m_StartOnInitialized;
		}
		set
		{
			m_StartOnInitialized = value;
			if (m_State == Status.Idle)
			{
				StartRecord();
			}
		}
	}

	public float GetAverage()
	{
		return m_Average;
	}

	// 波形データの取得
	public byte[] GetWaveData()
	{
		byte[] ret;
		lock (m_Data)
		{
			//Debug.Log("GetWaveData:" + m_Data.Count);
			ret = m_Data.ToArray();
			m_Data.Clear();
		}

		return ret;
	}

	// 波形データの取得開始
	public void StartRecord()
	{
		if (m_State == Status.Idle)
		{
			m_Average = 0;
			m_Data.Clear();
			m_AudioSource.Play();
			m_State = Status.Recording;
			Debug.Log("Audio StartRecord.");
		}
	}

	// 波形データの取得停止
	public void StopRecord()
	{
		if (m_State == Status.Recording)
		{
			if (!m_MicActive)
			{
				m_AudioSource.Stop();
			}
			m_Data.Clear();
			m_State = Status.Idle;
		}
	}

	IEnumerator StartMicrophone()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
		if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
		{
			m_State = Status.AccessDenied;
			yield break;
		}
		m_Data = new List<byte>(m_MaxCapacity);

		var mic = Microphone.Start(null, true, 1, 44100);
		if (mic == null)
		{
			m_State = Status.NoDevice;
			yield break;
		}
		m_AudioSource = gameObject.AddComponent<AudioSource>();
		m_AudioSource.playOnAwake = false;
		m_AudioSource.clip = mic;
		m_AudioSource.loop = true;
		m_AudioSource.outputAudioMixerGroup = Mixer;
		int counter = 0;
        while (Microphone.GetPosition(null) <= 0)
		{
			yield return null;
			counter++;
			if (counter >= 150)
			{
				m_State = Status.NoDevice;
				yield break;
			}
		}

		m_State = Status.Idle;
		if (m_StartOnInitialized)
		{
			StartRecord();
		}
		else if (m_MicActive)
		{
			m_AudioSource.Play();
		}
        Debug.Log("Audio outputSampleRate:" + AudioSettings.outputSampleRate);
    }

    void OnAudioFilterRead(float[] data, int channels)
	{
		if (m_State != Status.Recording)
		{
			m_Average = 0;
			foreach (var sample in data)
			{
				m_Average += sample * sample;
			}
			m_Average = Mathf.Sqrt(m_Average / data.Length);
			return;
		}

		lock (m_Data)
        {
			//Debug.Log("OnAudioFilterRead:" + data.Length + " " + channels + " " + m_Data.Count);
			if ((m_Data.Count + (data.Length * 2)) > m_MaxCapacity)
		    {
                Debug.Log("Drop Wave Data (;_;)");
			    return;
		    }
			m_Average = 0;
			foreach (var sample in data)
			{
				var wave = (int)(sample * 32767);
				m_Data.Add((byte)(wave & 0xff));
				m_Data.Add((byte)(wave >> 8));
				m_Average += sample * sample;
			}
			m_Average = Mathf.Sqrt(m_Average / data.Length);
		}
	}
}
