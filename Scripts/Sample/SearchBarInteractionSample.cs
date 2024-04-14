using UnityEngine;
using TLab.InputField;
using TLab.Android.WebView;

public class SearchBarInteractionSample : MonoBehaviour
{
    [SerializeField] private TLabWebView m_webview;
    [SerializeField] private TLabVKeyborad m_keyborad;

#if false
    /// <summary>
    /// Press button to execute
    /// </summary>
    public void AddEventListener()
    {
        string jsCode = @"
            var elements = [];

            function searchShadowRoot(node, id) {
                if (node == null) {
                    return;
                }

                if (node.shadowRoot != undefined && node.shadowRoot != null) {
                    if (!elements.includes(node.shadowRoot)) {
                        elements.push(node.shadowRoot);
                    }
                    searchShadowRoot(node.shadowRoot, id);
                }

                for (var i = 0; i < node.childNodes.length; i++) {
                    searchShadowRoot(node.childNodes[i], id);
                }
            }

            elements.push(document);
            searchShadowRoot(document, 'input');
            searchShadowRoot(document, 'textarea');

            function focusin (e) {
                const target = e.target;
                if (target.tagName == 'INPUT' || target.tagName == 'TEXTAREA') {
                    window.TLabWebViewActivity.unitySendMessage('SearchBar', 'OnMessage', 'Foucusin');
                }
            }

            function focusout (e) {
                const target = e.target;
                if (target.tagName == 'INPUT' || target.tagName == 'TEXTAREA') {
                    window.TLabWebViewActivity.unitySendMessage('SearchBar', 'OnMessage', 'Foucusout');
                }
            }

            for (var i = 0; i < elements.length; i++) {
                elements[i].removeEventListener('focusin', focusin);
                elements[i].removeEventListener('focusout', focusout);

                elements[i].addEventListener('focusin', focusin);
                elements[i].addEventListener('focusout', focusout);
            }
            ";

        m_webview.EvaluateJS(jsCode);
    }
#endif

    public void OnMessage(string message)
    {
        Debug.Log("OnMessage: " + message);

        switch (message)
        {
            case "Foucusin":
                m_keyborad.HideKeyborad(false);
                break;
            case "Foucusout":
                m_keyborad.HideKeyborad(true);
                break;
        }
    }
}