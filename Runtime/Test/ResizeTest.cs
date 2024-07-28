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
            var texWidth = m_webview.texWidth;
            var texHeight = m_webview.texHeight;

            m_webview.ResizeTex(texWidth / 2, texHeight / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeWeb()
        {
            var webWidth = m_webview.webWidth;
            var webHeight = m_webview.webHeight;

            m_webview.ResizeWeb(webWidth / 2, webHeight / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DownSize()
        {
            var texWidth = m_webview.texWidth;
            var texHeight = m_webview.texHeight;

            var webWidth = m_webview.webWidth;
            var webHeight = m_webview.webHeight;

            m_webview.Resize(texWidth / 2, texHeight / 2, webWidth / 2, webHeight / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpSize()
        {
            var texWidth = m_webview.texWidth;
            var texHeight = m_webview.texHeight;

            var webWidth = m_webview.webWidth;
            var webHeight = m_webview.webHeight;

            m_webview.Resize(texWidth * 2, texHeight * 2, webWidth * 2, webHeight * 2);
        }
    }
}
