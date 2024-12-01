using System.Collections;
using System.Text;
using System.Linq;
using UnityEngine;

namespace TLab.WebView.Sample
{
    public class DownloadHandlerSample : MonoBehaviour
    {
        [SerializeField] private BrowserContainer m_container;

        private bool m_downloading = false;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        private IEnumerator DownloadProgress(long id)
        {
            m_downloading = true;

            while (m_downloading)
            {
                Debug.Log(THIS_NAME + "progress: " + m_container.browser.GetDownloadProgress(id));
                yield return null;
            }
        }

        public void OnDownloadError(Download.EventInfo info) => Debug.Log(THIS_NAME + $"Error !");

        public void OnDownloadStart(Download.EventInfo info)
        {
            Debug.Log(THIS_NAME + $"Start !");

            StartCoroutine(DownloadProgress(info.id));
        }

        public void OnDownloadFinish(Download.EventInfo info)
        {
            m_downloading = false;

            Debug.Log(THIS_NAME + $"Finish !");
        }

        private class BlobDataFechInfo : JSONSerialisable
        {
            public string id;
            public string mimeType;

            public BlobDataFechInfo(string id, string mimeType)
            {
                this.id = id;
                this.mimeType = mimeType;
            }
        }

        public void OnFetchBlobData(string argment)
        {
            const string THIS_NAME = nameof(OnFetchBlobData);

            var info = JsonUtility.FromJson<BlobDataFechInfo>(argment);
            Debug.Log(THIS_NAME + $"{nameof(BlobDataFechInfo)}: {info.id}, {info.mimeType}");

            if (m_container.browser is not WebView)
                return;

            // data:[<mediatype>][;base64],<data>

            var browser = m_container.browser as WebView;
            var buf = browser.GetJSBuffer(info.id);

            var index = buf.Select((x, i) => (x, i)).First((c) => c.x == ',').i + 1;

            Debug.Log(THIS_NAME + $"Buffer: {buf[0]}, {buf[1]}, {buf[2]}, {buf[3]}, {buf[4]}, {buf[buf.Length - 1]}, length: {buf.Length}");

            var base64Encoded = System.Convert.FromBase64String(Encoding.UTF8.GetString(buf, index, buf.Length - index));
            var stringEncoded = Encoding.UTF8.GetString(base64Encoded);
            const int OFFSET = 20;

            for (index = 0; index < stringEncoded.Length - OFFSET; index += OFFSET)
                Debug.Log(THIS_NAME + $"String: {stringEncoded.Substring(index, OFFSET)}");
            Debug.Log(THIS_NAME + $"String: {stringEncoded.Substring(index, stringEncoded.Length - index)}");

            var js = $"window.tlab.free('{info.id}');";

            m_container.browser.EvaluateJS(js);
        }

        public void FetchBlobData(string url, string mimeType)
        {
            // https://developer.mozilla.org/ja/docs/Web/API/FileReader/readAsDataURL
            var js = JSUtil.ToVariable("url", url) + JSUtil.ToVariable("mimetype", mimeType);
            js += JSUtil.ToVariable("go", gameObject.name) + JSUtil.ToVariable("method", nameof(OnFetchBlobData)) + JSUtil.ToVariable("argments", new BlobDataFechInfo(url, mimeType).Marshall());
            js += Resources.Load<TextAsset>("TLab/WebView/Samples/Scripts/JS/fetch-blob")?.ToString();

            m_container.browser.EvaluateJS(js);
        }

        public void Log(string message) => Debug.Log(THIS_NAME + $"message receive: {message}");

        public void OnDownlaod(Download.Request request)
        {
            Debug.Log(THIS_NAME + $"OnDownload ... url:{request.url}, userAgent:{request.userAgent}, contentDisposition:{request.contentDisposition}, mimeType:{request.mimeType}");

#if true
            m_container.browser.DownloadFromUrl(request);
#else
            if (request.url.StartsWith("blob:"))
                FetchBlobData(request.url, request.mimeType);
            else
                m_container.browser.DownloadFromUrl(request);
#endif
        }
    }
}
