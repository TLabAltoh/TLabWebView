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

        public void BlobToDataUrlCallback(string argument)
        {
            var commands = argument.Split("\n");
            var bufferName = commands[0];
            var mimeType = commands[1];
            Debug.Log(THIS_NAME + $"message receive: {bufferName}, {mimeType}");

            // data:[<mediatype>][;base64],<data>
            byte[] buf = m_webview.GetWebBuffer(bufferName);

            //Debug.Log(THIS_NAME + $"message receive: {buf[0]}, {buf[1]}, {buf[2]}, {buf[3]}, {buf[4]}, length: {buf.Length}");    // data:
            //Debug.Log(THIS_NAME + $"message receive: {buf[buf.Length - 1]}, {buf[3999999]}, {buf[4000000]}, {buf[4000001]}, {buf[4000002]}, {buf[4500]}, {buf[100]}, {buf[600]}, {buf[500]}");

            string js = $"window.TLabWebViewActivity.free('{bufferName}');";

            m_webview.EvaluateJS(js);
        }

        public void BlobToDataUrl(string url, string mimeType)
        {
            // https://developer.mozilla.org/ja/docs/Web/API/FileReader/readAsDataURL
            string js =
                    "function writeBuffer(buffer, bufferName, segmentSize, offset)" +
                    "{" +
                    "    if (segmentSize === 0) return;" +
                    "" +
                    "    var i = offset;" +
                    "    while(i + segmentSize <= buffer.length)" +
                    "    {" +
                    "       window.TLabWebViewActivity.write(bufferName, buffer.slice(i, i + segmentSize));" +
                    "       i += segmentSize" +
                    "    }" +
                    "" +
                    "    writeBuffer(buffer, bufferName, parseInt(segmentSize / 2), i);" +
                    "}" +
                    "var xhr = new XMLHttpRequest();" +
                    "xhr.open('GET', '" + url + "', true);" +
                    "xhr.setRequestHeader('Content-type','" + mimeType + ";charset=UTF-8');" +
                    "xhr.responseType = 'blob';" +
                    "xhr.onload = function(e) {" +
                    "    if (this.status == 200) {" +
                    "        var blobFile = this.response;" +
                    "        var reader = new FileReader();" +
                    "        reader.readAsDataURL(blobFile);" +
                    "        reader.onloadend = function() {" +
                    "            base64data = reader.result;" +
                    "            bufferName = 'data-url';" +
                    "            buffer = new TextEncoder().encode(base64data);" +
                    "            window.TLabWebViewActivity.malloc(bufferName, buffer.length);" +
                    "            writeBuffer(buffer, bufferName, 500000, 0);" +
                    "            window.TLabWebViewActivity.unitySendMessage('" + this.gameObject.name + "','BlobToDataUrlCallback', bufferName + '\\n' + '" + mimeType + "');" +
                    "        }" +
                    "    }" +
                    "};" +
                    "xhr.send();";

            m_webview.EvaluateJS(js);
        }

        public void Log(string message)
        {
            Debug.Log(THIS_NAME + $"message receive: {message}");
        }

        public void OnCatchDownloadUrl(string argument)
        {
            var commands = argument.Split("\n");
            var url = commands[0];
            var userAgent = commands[1];
            var contentDisposition = commands[2];
            var mimeType = commands[3];

            Debug.Log(THIS_NAME + $"message receive: catch download url ... url:{url}, userAgent:{userAgent}, contentDisposition:{contentDisposition}, mimeType:{mimeType}");

            if (url.StartsWith("blob:"))
            {
                BlobToDataUrl(url, mimeType);
            }
            else
            {
                m_webview.DownloadFromUrl(url, userAgent, contentDisposition, mimeType);
            }
        }
    }
}
