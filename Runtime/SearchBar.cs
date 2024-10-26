using UnityEngine;
using TMPro;
using TLab.VKeyborad;

namespace TLab.Android.WebView
{
    public class SearchBar : InputFieldBase
    {
        [Header("WebView")]
        [SerializeField] private TLabWebView m_webview;

        [Header("TextMeshPro")]
        [SerializeField] private TextMeshProUGUI m_searchBar;

        [System.NonSerialized] public string m_text = "";

        private string THIS_NAME => "[" + this.GetType() + "] ";

        #region KEY_EVENT

        public override void OnBackSpacePressed()
        {
            if (m_text != "")
            {
                m_text = m_text.Remove(m_text.Length - 1);
                Display();
            }

            base.OnBackSpacePressed();
        }

        public override void OnEnterPressed()
        {
            LoadUrl();

            base.OnEnterPressed();
        }

        public override void OnSpacePressed()
        {
            AddKey(" ");

            base.OnSpacePressed();
        }

        public override void OnTabPressed()
        {
            AddKey("    ");

            base.OnTabPressed();
        }

        public override void OnKeyPressed(string input)
        {
            AddKey(input);

            base.OnKeyPressed(input);
        }

        #endregion KEY_EVENT

        public void LoadUrl()
        {
            const string HTTPS_PREFIX = "https://";
            const string HTTP_PREFIX = "http://";

            string url;

            if (m_searchBar.text.StartsWith(HTTPS_PREFIX) || m_searchBar.text.StartsWith(HTTP_PREFIX))
            {
                url = m_searchBar.text;
            }
            else
            {

                url = $"https://www.google.com/search?q={m_searchBar.text}";
            }

            m_webview.LoadUrl(url);
        }

        public void Display() => m_searchBar.text = m_text;

        public void AddKey(string key)
        {
            m_text += key;
            Display();
        }
    }
}
