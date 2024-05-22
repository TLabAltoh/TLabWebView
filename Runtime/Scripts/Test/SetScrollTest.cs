using UnityEngine;

namespace TLab.Android.WebView.Test
{
    public class SetScrollTest : MonoBehaviour
    {
        [SerializeField] private TLabWebView m_webview;

        /// <summary>
        /// 
        /// </summary>
        public void ScrollTo()
        {
            m_webview.ScrollTo(0, 50);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ScrollBy()
        {
            m_webview.ScrollBy(0, 50);
        }
    }
}
