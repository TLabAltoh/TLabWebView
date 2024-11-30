using System.Collections.Generic;
using TLab.WebView.Widget;

namespace TLab.WebView
{
    public interface IBrowser
    {
        /// <summary>
        /// Run javascript on the current web page.
        /// </summary>
        /// <param name="js">javascript</param>
        void EvaluateJS(string js);

        void DispatchMessageQueue();

        /// <summary>
        /// Request file download to Download Manager.
        /// </summary>
        /// <param name="url">The full url to the content that should be downloaded</param>
        /// <param name="userAgent">The user agent to be used for the download</param>
        /// <param name="contentDisposition">Content-disposition http header, if present</param>
        /// <param name="mimetype">The mimetype of the content reported by the server</param>
        void DownloadFromUrl(string url, string userAgent, string contentDisposition, string mimetype);

        /// <summary>
        /// Set the directory in which the file will be downloaded.
        /// </summary>
        void SetDownloadOption(Download.Option downloadOption);

        /// <summary>
        /// Get the progress of the download event currently being recorded.
        /// </summary>
        /// <returns>Current download progress (0 ~ 1)</returns>
        float GetDownloadProgress(long id);

        /// <summary>
        /// Update userAgent with the given userAgent string.
        /// </summary>
        /// <param name="ua">UserAgent string</param>
        /// <param name="reload">If true, reload web page when userAgent is updated.</param>
        void SetUserAgent(string ua, bool reload);

        /// <summary>
        /// Capture current userAgent async.
        /// </summary>
        IEnumerator<AsyncString> GetUserAgent();

        string GetAsyncResult(int id);

        void CancelAsyncResult(int id);

        /// <summary>
        /// Get current url that the WebView instance is loading
        /// </summary>
        /// <returns>Current url that the WebView instance is loading</returns>
        string GetUrl();

        /// <summary>
        /// Loads the given URL.
        /// </summary>
        /// <param name="url">The URL of the resource to load.</param>
        void LoadUrl(string url);

        /// <summary>
        /// Register url patterns to treat as deep links
        /// </summary>
        /// <param name="intentFilters">Url patterns that are treated as deep links (regular expression)</param>
        void SetIntentFilters(string[] intentFilters);

        /// <summary>
        /// Get content's scroll position x
        /// </summary>
        /// <returns>Page content's current scroll position x</returns>
        int GetScrollX();

        /// <summary>
        /// Get content's scroll position y
        /// </summary>
        /// <returns>Page content's current scroll position y</returns>
        int GetScrollY();

        /// <summary>
        /// Set content's scroll position.
        /// </summary>
        /// <param name="x">Scroll position x of the destination</param>
        /// <param name="y">Scroll position y of the destination</param>
        void ScrollTo(int x, int y);

        /// <summary>
        /// Move the scrolled position of WebView
        /// </summary>
        /// <param name="x">The amount of pixels to scroll by horizontally</param>
        /// <param name="y">The amount of pixels to scroll by vertically</param>
        void ScrollBy(int x, int y);

        /// <summary>
        /// Dispatch of a touch event.
        /// </summary>
        /// <param name="x">Touch position x</param>
        /// <param name="y">Touch position y</param>
        /// <param name="action">Touch event type (TOUCH_DOWN: 0, TOUCH_UP: 1, TOUCH_MOVE: 2)</param>
        /// <param name="downTime">Start time of touch event</param>
        public long TouchEvent(int x, int y, int action, long downTime);

        /// <summary>
        /// Dispatch of a basic keycode event.
        /// </summary>
        /// <param name="key">'a', 'b', 'A' ....</param>
        void KeyEvent(char key);

        void KeyEvent(int keyCode);

        /// <summary>
        /// Goes back in the history of this WebView.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Goes forward in the history of this WebView.
        /// </summary>
        void GoForward();

        public void PostDialogResult(AlertDialog.Result result, string json = "");
    }
}