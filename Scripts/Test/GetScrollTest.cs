using UnityEngine;

namespace TLab.Android.WebView.Test
{
    public class GetScrollTest : MonoBehaviour
    {
        [SerializeField] private TLabWebView m_webview;

        private string THIS_NAME => "[scroll] ";

        void Update()
        {
            if (m_webview.IsInitialized())
            {
                Debug.Log(THIS_NAME + $"{m_webview.GetScrollX()}, {m_webview.GetScrollY()}");
            }
        }
    }
}
