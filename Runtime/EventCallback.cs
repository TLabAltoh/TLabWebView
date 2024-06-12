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
		public string onStart;
		public string onFinish;

		public string varDlUriName = "unity_webview_dl_uri";
		public string varDlUrlName = "unity_webview_dl_url";
		public string varDlIdName = "unity_webview_dl_id";
	}

	[System.Serializable]
	public class EventCallback
	{
		public string onPageFinish;

		public DownloadEventCallback dlEvent = new DownloadEventCallback();

		public CatchDownloadUrlEventCallback catchDlUrlEvent = new CatchDownloadUrlEventCallback();
	}
}
