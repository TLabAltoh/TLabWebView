using UnityEngine;
using TLab.VKeyborad;

namespace TLab.WebView
{
    public class BrowserInputField : BaseInputField
    {
        [SerializeField] private BrowserContainer m_container;

        #region KEY_EVENT

        public override void OnBackSpaceKey()
        {
            m_container.browser?.KeyEvent(67);

            AfterOnBackSpaceKey();
        }

        public override void OnEnterKey()
        {
            AddKey("\n");

            AfterOnEnterKey();
        }

        public override void OnTabKey()
        {
            AddKey("\t");

            AfterOnTabKey();
        }

        #endregion KEY_EVENT

        public override void AddKey(string key) => m_container.browser.KeyEvent(key.ToCharArray()[0]);
    }
}
