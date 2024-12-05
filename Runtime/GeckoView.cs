#define DEBUG
#undef DEBUG

namespace TLab.WebView
{
    public class GeckoView : Browser
    {
        public override string package => "com.tlab.webkit.gecko.UnityConnect";

        /// <summary>
        /// Loads the given HTML.
        /// </summary>
        /// <param name="html">The HTML of the resource to load</param>
        public void LoadHTML(string html)
        {
            if (m_state != State.Initialized)
                return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(LoadHTML), html);
#endif
        }

        public void ClearData(int flag)
        {
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(ClearData), flag);
#endif
        }
    }
}
