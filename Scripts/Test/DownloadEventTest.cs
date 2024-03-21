using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadEventTest : MonoBehaviour
{
    public void OnDownloadFinish(string message)
    {
        //window.TLabWebViewActivity.unitySendMessage('Download Event Test', 'OnDownloadFinish', 'download finish')

        Debug.Log($"message receive: {message}");
    }
}
