#define DEBUG
#undef DEBUG

using System.Collections.Generic;

namespace TLab.WebView
{
    public class WebView : Browser
    {
        public override string package => "com.tlab.webkit.chromium.UnityConnect";

        /// <summary>
        /// Performs zoom in in this WebView.
        /// </summary>
        public void ZoomIn()
        {
            if (m_state != State.Initialized)
                return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(ZoomIn));
#endif
        }

        /// <summary>
        /// Performs zoom out in this WebView.
        /// </summary>
        public void ZoomOut()
        {
            if (m_state != State.Initialized)
                return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(ZoomOut));
#endif
        }

        /// <summary>
        /// Scrolls the contents of this WebView up by half the view size.
        /// </summary>
        /// <param name="top">True to jump to the top of the page</param>
        public void PageUp(bool top)
        {
            if (m_state != State.Initialized)
                return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(PageUp), top);
#endif
        }

        /// <summary>
        /// Scrolls the contents of this WebView down by half the page size.
        /// </summary>
        /// <param name="bottom">True to jump to bottom of page</param>
        public void PageDown(bool bottom)
        {
            if (m_state != State.Initialized)
                return;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(PageDown), bottom);
#endif
        }

        /// <summary>
        /// Retrieve buffers that allocated in order to map javascript buffer to java 
        /// <see cref="Sample.DownloadHandlerSample.FetchBlobData(string, string)">example is here</see>
        /// </summary>
        /// <param name="id">Name of buffers that allocated in order to map javascript buffer to java</param>
        /// <returns>current buffer value</returns>
        public byte[] GetJSBuffer(string id)
        {
            if (m_state != State.Initialized)
                return new byte[0];

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            return m_NativePlugin.Call<byte[]>(nameof(GetJSBuffer), id);
#else
            return new byte[0];
#endif
        }

        public IEnumerable<JavaAsyncResult> EvaluateJSForResult(string varNameOfResultId, string js)
        {
            if (m_state != State.Initialized)
                yield break;

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            var id = m_NativePlugin.Call<int>(nameof(EvaluateJSForResult), varNameOfResultId, js);
            if (id == -1) yield break;
            while (true)
            {
                var @object = GetAsyncResult(id);
                if (@object == "")
                {
                    yield return null;
                    continue;
                }
                yield return new JavaAsyncResult(@object);
                break;
            }
            yield break;
#else
            yield break;
#endif
        }

        /// <summary>
        /// Clear WebView Cache.
        /// </summary>
        /// <param name="includeDiskFiles">If false, only the RAM cache will be cleared</param>
        public void ClearCache(bool includeDiskFiles)
        {
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(ClearCache), includeDiskFiles);
#endif
        }

        /// <summary>
        /// Clear WebView Cookie.
        /// </summary>
        public void ClearCookie()
        {
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(ClearCookie));
#endif
        }

        /// <summary>
        /// Clear WebView History.
        /// </summary>
        public void ClearHistory()
        {
#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(ClearHistory));
#endif
        }

        /// <summary>
        /// Loads the given HTML.
        /// </summary>
        /// <param name="html">The HTML of the resource to load</param>
        /// <param name="baseURL">baseURL</param>
        public void LoadHTML(string html, string baseURL)
        {
            if (m_state != State.Initialized)
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR || DEBUG
            m_NativePlugin.Call(nameof(LoadHTML), html, baseURL);
#endif
        }
    }
}
