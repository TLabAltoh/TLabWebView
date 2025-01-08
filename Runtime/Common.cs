using UnityEngine;
using UnityEngine.Events;
using TLab.WebView.Widget;

namespace TLab.WebView
{
	[System.Serializable]
	public class JavaAsyncResult : JSONSerialisable
	{
		public JavaAsyncResult() : base() { }
		public JavaAsyncResult(string json) : base(json) { }

		public static class Status
		{
			public const int WAITING = 0;
			public const int FAILED = 1;
			public const int CANCEL = 2;
			public const int COMPLETE = 3;
		}

		public int id;
		public int status;

		public int i;
		public double d;
		public bool b;
		public string s;
	}

	public struct AsyncInteger
	{
		public int value;
		public int status;

		public AsyncInteger(int value, int status)
		{
			this.value = value;
			this.status = status;
		}
	}

	public struct AsyncDouble
	{
		public int status;
		public double value;

		public AsyncDouble(double value, int status)
		{
			this.value = value;
			this.status = status;
		}
	}

	public struct AsyncBool
	{
		public int status;
		public bool value;

		public AsyncBool(bool value, int status)
		{
			this.value = value;
			this.status = status;
		}
	}

	public struct AsyncString
	{
		public int status;
		public string value;

		public AsyncString(string value, int status)
		{
			this.value = value;
			this.status = status;
		}
	}

	public enum CaptureMode
	{
		HardwareBuffer,
		ByteBuffer,
		Surface,
	}

	[System.Serializable]
	public class Download
	{
		public enum Directory
		{
			Applicaiton,
			Download,
		}

		[System.Serializable]
		public class Option
		{
			[SerializeField] private Directory m_directory = Directory.Download;
			[SerializeField] private string m_subDirectory = "downloads";

			public Directory directory => m_directory;

			public string subDirectory => m_subDirectory;

			public void Update(Directory directory) => m_directory = directory;

			public void Update(string subDrectory) => m_subDirectory = subDrectory;

			public void Update(Directory directory, string subDirectory)
			{
				m_directory = directory;
				m_subDirectory = subDirectory;
			}
		}

		[System.Serializable]
		public class Request : JSONSerialisable
		{
			public Request() : base() { }
			public Request(string json) : base(json) { }

			public string url;
			public string userAgent;
			public string contentDisposition;
			public string mimeType;
		}

		[System.Serializable]
		public class EventInfo : JSONSerialisable
		{
			public EventInfo() : base() { }
			public EventInfo(string json) : base(json) { }

			public string url;
			public long id;
		}
	}

	[System.Serializable]
	public class EventCallback
	{
		public enum Type
		{
			Raw,
			OnPageStart,
			OnPageFinish,
			OnDownload,
			OnDownloadStart,
			OnDownloadError,
			OnDownloadFinish,
			OnDialog,
		};

		[System.Serializable]
		public class Message : JSONSerialisable
		{
			public Message() : base() { }
			public Message(string json) : base(json) { }

			public int type;
			public string payload;
		}

		public UnityEvent<string> onPageStart;
		public UnityEvent<string> onPageFinish;
		public UnityEvent<Download.Request> onDownload;
		public UnityEvent<Download.EventInfo> onDownloadError;
		public UnityEvent<Download.EventInfo> onDownloadStart;
		public UnityEvent<Download.EventInfo> onDownloadFinish;
		public UnityEvent<AlertDialog.Init, AlertDialog> onDialog;
	}

	public abstract class JSONSerialisable
	{
		public JSONSerialisable() { }

		public JSONSerialisable(string json) => UnMarshall(json);

		public virtual string Marshall() => JsonUtility.ToJson(this);

		public virtual void UnMarshall(string json) => JsonUtility.FromJsonOverwrite(json, this);
	}

	public static class JSUtil
	{
		public static string ToVariable(string name, string value)
		{
			return "var " + name + " = " + "'" + value + "';\n";
		}

		public static string ToVariable(string name, int value)
		{
			return "var " + name + " = " + value + ";\n";
		}

		public static string ToVariable(string name, float value)
		{
			return "var " + name + " = " + value + ";\n";
		}
	}
}
