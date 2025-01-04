using UnityEngine;

namespace TLab.WebView.Sample
{
    public class JSSnippets : MonoBehaviour
    {
        [SerializeField] private BrowserContainer m_container;

        public void RemoveEventFromBeforeUnload()
        {
            var js = Resources.Load<TextAsset>("TLab/WebView/Samples/Scripts/JS/remove-event-from-beforunload")?.ToString();
            m_container.browser.EvaluateJS(js);
        }
    }
}
