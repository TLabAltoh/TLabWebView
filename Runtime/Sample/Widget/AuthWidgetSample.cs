using UnityEngine;
using TLab.VKeyborad;

namespace TLab.WebView.Widget.Sample
{
    public class AuthWidgetSample : MonoBehaviour, IWidgetHandler<AuthWidget.Init, AuthWidget>
    {
        [SerializeField] private GameObject m_content;
        [SerializeField] private InputField m_username;
        [SerializeField] private InputField m_password;

        private void InitInputField(InputField target, string text, bool active)
        {
            target.text = text;

            var parent = target.transform.parent.gameObject;
            parent.SetActive(active);
        }

        public void Close() => m_content.SetActive(false);

        public void OnDialog(AuthWidget.Init init, AuthWidget widget)
        {
            InitInputField(m_username, init.username, !init.onlyPassword);
            InitInputField(m_password, init.password, true);

            m_content.SetActive(true);
        }
    }
}
