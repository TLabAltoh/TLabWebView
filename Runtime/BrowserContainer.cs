using UnityEngine;

namespace TLab.WebView
{
    public class BrowserContainer : MonoBehaviour
    {
        [SerializeField] private Browser m_browser;

        public Browser browser => m_browser;
    }
}
