using UnityEngine;

namespace TLab.Android.WebView.Test
{
    public class ResizeTest : MonoBehaviour
    {
        [SerializeField] private TLabWebView m_webview;

        /// <summary>
        /// 
        /// </summary>
        public void ResizeTex()
        {
            m_webview.ResizeTex(m_webview.texSize / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeWeb()
        {
            m_webview.ResizeWeb(m_webview.webSize / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DownSize()
        {
            m_webview.Resize(m_webview.texSize / 2, m_webview.webSize / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpSize()
        {
            m_webview.Resize(m_webview.texSize * 2, m_webview.webSize * 2);
        }
    }
}
