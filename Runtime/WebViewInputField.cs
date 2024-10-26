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

        public override void OnSpacePressed()
        {
            AddKey(" ");

            base.OnSpacePressed();
        }

        public override void OnTabPressed()
        {
            AddKey("\t");

            base.OnTabPressed();
        }

        public override void OnKeyPressed(string input)
        {
            AddKey(input);

            base.OnKeyPressed(input);
        }

        #endregion KEY_EVENT

        public override void OnFocus()
        {
            var notActive = !inputFieldIsActive;

            if (m_keyborad.mobile && notActive)
            {
                m_keyborad.SwitchInputField(this);
                m_keyborad.SetVisibility(true);
                m_onFocus.Invoke(true);
            }
        }

        public void AddKey(string key) => m_webview.KeyEvent(key.ToCharArray()[0]);
    }
}
