using System.Collections;
using UnityEngine;

namespace TLab.Android.WebView.Test
{
    public class DownloadEventTest : MonoBehaviour
    {
        [SerializeField] private TLabWebView m_webview;

        private bool m_downloading = false;

        private string THIS_NAME => "[dltest] ";

        private IEnumerator DownloadProgress()
        {
            m_downloading = true;

            while (m_downloading)
            {
                m_webview.RequestCaptureDownloadProgress();

                Debug.Log(THIS_NAME + "progress: " + m_webview.GetDownloadProgress());

                yield return null;
            }
        }

        public void OnDownloadStart(string message)
        {
            /**
             * argments:
             * var unity_webview_dl_url : string
             * var unity_webview_dl_id : long
             * 
             * js code:
             * window.TLabWebViewActivity.unitySendMessage('Download Event Test', 'OnDownloadStart', 'download start. url: ' + unity_webview_dl_url + ', id: ' + unity_webview_dl_id);
             */

            Debug.Log(THIS_NAME + $"message receive: {message}");

            StartCoroutine(DownloadProgress());
        }

        public void OnDownloadFinish(string message)
        {
            /**
             * argments: 
             * var unity_webview_dl_uri : string
             * var unity_webview_dl_id : long
             * 
             * js code:
             * window.TLabWebViewActivity.unitySendMessage('Download Event Test', 'OnDownloadFinish', 'download finish. uri: ' + unity_webview_dl_uri + ', id: ' + unity_webview_dl_id);
             */

            m_downloading = false;

            Debug.Log(THIS_NAME + $"message receive: {message}");
        }
    }
}
