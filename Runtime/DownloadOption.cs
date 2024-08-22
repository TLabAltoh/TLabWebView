namespace TLab.Android.WebView
{
	public enum DownloadOption
	{
		/// <summary>
		/// Application's external files directory.
		/// https://developer.android.com/reference/android/app/DownloadManager.Request#setDestinationInExternalFilesDir(android.content.Context,%20java.lang.String,%20java.lang.String)
		/// </summary>
		APPLICATION_FOLDER,

		/// <summary>
		/// Standard directory in which to place files that have been downloaded by the user.
		/// https://developer.android.com/reference/android/os/Environment#DIRECTORY_DOWNLOADS
		/// </summary>
		DOWNLOAD_FOLDER
	}
}
