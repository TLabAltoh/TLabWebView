using UnityEngine;

namespace TLab.WebView.Sample
{
    public class JSSnippets : MonoBehaviour
    {
        [SerializeField] private BrowserContainer m_container;

        public void DisableBeforeUnload()
        {
            var js = Resources.Load<TextAsset>("TLab/WebView/Samples/Scripts/JS/disable-beforunload")?.ToString();
            m_container.browser.EvaluateJS(js);
        }
    }
}
