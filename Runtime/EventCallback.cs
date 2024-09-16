using UnityEngine;

namespace TLab.Android.WebView
{
	[System.Serializable]
	public class CatchDownloadUrlEventCallback
	{
		public string go;
		public string func;
	}

	[System.Serializable]
	public class DownloadEventCallback
	{
		[TextArea(minLines: 5, maxLines: 15)]
		public string onStart;

		[TextArea(minLines: 5, maxLines: 15)]
		public string onFinish;

		public string varUri = "download_uri";
		public string varUrl = "download_url";
		public string varId = "download_id";
	}

	[System.Serializable]
	public class EventCallback
	{
		[TextArea(minLines: 5, maxLines: 15)]
		public string onPageFinish;

		public DownloadEventCallback downloadEvent = new DownloadEventCallback();

		public CatchDownloadUrlEventCallback catchDownloadUrlEvent = new CatchDownloadUrlEventCallback();
	}
}
