using UnityEngine;
using UnityEngine.EventSystems;
using TLab.VKeyborad;

namespace TLab.WebView.Sample
{
    public class FocusInOutInteractionSample : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private SearchBar m_searchBar;
        [SerializeField] private BaseInputField m_inputField;
        [SerializeField] private BrowserContainer m_container;

        public void OnPageFinish(string url)
        {
            var js = JSUtil.ToVariable("go", gameObject.name) + JSUtil.ToVariable("method", nameof(OnMessage));
            js += Resources.Load<TextAsset>("TLab/WebView/Samples/Scripts/JS/focus-in-out-interaction")?.ToString();

            m_container.browser.EvaluateJS(js);
        }

        public void OnMessage(string message)
        {
            Debug.Log("OnMessage: " + message);
            switch (message)
            {
                case "Focusin":
                    m_inputField.OnFocus(true);
                    break;
                case "Focusout":
                    m_inputField.OnFocus(false);
                    break;
            }
        }

        public void OnPointerDown(PointerEventData eventData) => m_searchBar.OnFocus(false);
    }
}