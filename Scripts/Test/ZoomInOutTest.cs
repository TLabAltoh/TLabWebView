using UnityEngine;

namespace TLab.Android.WebView.Test
{
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
}
