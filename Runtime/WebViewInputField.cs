using UnityEngine;
using TLab.VKeyborad;

namespace TLab.Android.WebView
{
    public class WebViewInputField : InputFieldBase
    {
        [Header("WebView")]
        [SerializeField] private TLabWebView m_webview;

        #region KEY_EVENT

        public override void OnBackSpacePressed()
        {
            m_webview.BackSpace();

            base.OnBackSpacePressed();
        }

        public override void OnEnterPressed()
        {
            AddKey("\n");

            base.OnEnterPressed();
        }

        public override void OnTabPressed()
        {
            AddKey("\t");

            base.OnTabPressed();
        }

        #endregion KEY_EVENT

        public override void AddKey(string key) => m_webview.KeyEvent(key.ToCharArray()[0]);
    }
}
