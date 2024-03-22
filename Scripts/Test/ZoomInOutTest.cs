using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TLab.Android.WebView;

public class ZoomInOutTest : MonoBehaviour
{
    [SerializeField] private TLabWebView m_webview;

    public void ZoomIn()
    {
        m_webview.ZoomIn();
    }

    public void ZoomOut()
    {
        m_webview.ZoomOut();
    }
}
