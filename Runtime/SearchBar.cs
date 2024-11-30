using UnityEngine;
using TMPro;
using TLab.VKeyborad;

namespace TLab.WebView
{
    public class SearchBar : BaseInputField
    {
        [SerializeField] private BrowserContainer m_container;

        [Header("TextMeshPro")]
        [SerializeField] private TextMeshProUGUI m_searchBar;

        [System.NonSerialized] public string m_text = "";

        private string THIS_NAME => "[" + this.GetType() + "] ";

        #region KEY_EVENT

        protected override void HandlingOnBackSpaceKey()
        {
            if (m_text != "")
            {
                m_text = m_text.Remove(m_text.Length - 1);
                Display();
            }
        }

        protected override void HandlingOnEnterKey() => LoadUrl();

        #endregion KEY_EVENT

        public void LoadUrl()
        {
            const string HTTPS_PREFIX = "https://";
            const string HTTP_PREFIX = "http://";

            string url;

            if (m_searchBar.text.StartsWith(HTTPS_PREFIX) || m_searchBar.text.StartsWith(HTTP_PREFIX))
                url = m_searchBar.text;
            else
                url = $"https://www.google.com/search?q={m_searchBar.text}";

            m_container.browser.LoadUrl(url);
        }

        public void Display() => m_searchBar.text = m_text;

        public override void AddKey(string key)
        {
            m_text += key;
            Display();
        }
    }
}
