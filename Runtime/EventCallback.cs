using UnityEngine;

namespace TLab.Android.WebView
{
	[System.Serializable]
	public class CatchDownloadUrlEventCallback
	{
		[Tooltip("Name of the game object to which the function is attached")]
		public string go;

		[Tooltip("C# function name that is called before the download event starts")]
		public string func;
	}

	[System.Serializable]
	public class DownloadEventCallback
	{
		[TextArea(minLines: 5, maxLines: 15)]
		[Tooltip("Javascript called when download event started")]
		public string onStart;

		[TextArea(minLines: 5, maxLines: 15)]
		[Tooltip("Javascript called when download event finished")]
		public string onFinish;

		[Tooltip("Variable name for uri (The destination for the downloaded file) to use in javascript's download event callback")]
		public string varUri = "download_uri";

		[Tooltip("Variable name for url (URL of the file to be downloaded) to use in javascript's download event callback")]
		public string varUrl = "download_url";

		[Tooltip("Variable name for download id (The ID of the download event) to use in javascript's download event callback")]
		public string varId = "download_id";
	}

	[System.Serializable]
	public class EventCallback
	{
		[TextArea(minLines: 5, maxLines: 15)]
		[Tooltip("Javascript called when page is finished loading")]
		public string onPageFinish;

		public DownloadEventCallback downloadEvent = new DownloadEventCallback();

		public CatchDownloadUrlEventCallback catchDownloadUrlEvent = new CatchDownloadUrlEventCallback();
	}
}
