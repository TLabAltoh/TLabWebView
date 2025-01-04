#define TEST
//#undef TEST

using System.Collections;
using UnityEngine;

namespace TLab.WebView.Test
{
    public class BrowserAPITest : MonoBehaviour
    {
        [SerializeField] private BrowserContainer m_container;

        [SerializeField] private GameObject m_prefab;

        public void LoadUrl(string url) => m_container.browser.LoadUrl(url);

        public void ScrollTo() => m_container.browser.ScrollTo(0, 50);

        public void ScrollBy() => m_container.browser.ScrollBy(0, 50);

        public void GetScroll() => Debug.Log($"X: {m_container.browser.GetScrollX()}, Y: {m_container.browser.GetScrollY()}");

        public void ResizeTex() => m_container.browser.ResizeTex(m_container.browser.texSize / 2);

        public void ResizeWeb() => m_container.browser.ResizeView(m_container.browser.viewSize / 2);

        public void UpSize() => m_container.browser.Resize(m_container.browser.texSize * 2, m_container.browser.viewSize * 2);

        public void DownSize() => m_container.browser.Resize(m_container.browser.texSize / 2, m_container.browser.viewSize / 2);

        public void SetUserAgent()
        {
            string ua = "Mozilla/5.0 (X11; Linux i686; rv:10.0) Gecko/20100101 Firefox/10.0";
            bool reload = true;
            m_container.browser.SetUserAgent(ua, reload);
        }

        private IEnumerator GetUserAgentTask()
        {
            var task = m_container.browser.GetUserAgent();
            yield return task;
            var result = task.Current;
            if (result.status == JavaAsyncResult.Status.COMPLETE)
                Debug.Log($"UserAgent: {result.value}");
        }

        public void GetUserAgent() => StartCoroutine(GetUserAgentTask());

#if TEST
        public void RenderContent2TmpSurface() => m_container.browser.RenderContent2TmpSurface();

        public void RemoveSurface() => m_container.browser.RemoveSurface();
#endif

        public void Quit() => Application.Quit();

        public void Destroy()
        {
            if (m_prefab != null)
                Destroy(m_prefab);
        }
    }
}
