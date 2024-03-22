using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TLab.Android.WebView;

public class DownloadEventTest : MonoBehaviour
{
    [SerializeField] private TLabWebView m_webview;

    private bool m_downloading = false;

    private IEnumerator DownloadProgress()
    {
        m_downloading = true;

        while (m_downloading)
        {
            m_webview.RequestCaptureDownloadProgress();

            Debug.Log("[dltest] progress: " + m_webview.GetDownloadProgress());

            yield return null;
        }
    }

    public void OnDownloadStart(string message)
    {
        //argment:
        //var unity_webview_dl_url (string),
        //var unity_webview_dl_id (long)

        //window.TLabWebViewActivity.unitySendMessage('Download Event Test', 'OnDownloadStart', 'download start. url: ' + unity_webview_dl_url + ', id: ' + unity_webview_dl_id);

        Debug.Log($"[dltest] message receive: {message}");

        StartCoroutine(DownloadProgress());
    }

    public void OnDownloadFinish(string message)
    {
        //argment:
        //var unity_webview_dl_uri (string),
        //var unity_webview_dl_id (long)

        //window.TLabWebViewActivity.unitySendMessage('Download Event Test', 'OnDownloadFinish', 'download finish. uri: ' + unity_webview_dl_uri + ', id: ' + unity_webview_dl_id);

        m_downloading = false;

        Debug.Log($"[dltest] message receive: {message}");
    }
}
