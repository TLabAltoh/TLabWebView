using UnityEngine;
using System.Threading.Tasks;

namespace TLab.Android.WebView.Sample
{
    public class WebViewAPISample : MonoBehaviour
    {
        [SerializeField] private TLabWebView m_webView;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        /// <summary>
        /// 
        /// </summary>
        public void LoadHTML()
        {
            string html = "";
            string base_url = "";

            m_webView.LoadHTML(html, base_url);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearCache()
        {
            m_webView.ClearCache(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearCookies()
        {
            m_webView.ClearCookie();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearHistory()
        {
            m_webView.ClearHistory();
        }

        /// <summary>
        /// Capture html source
        /// </summary>
        public void CaptureHTMLSource()
        {
#if true
            m_webView.CaptureHTMLSource();
#else
        m_webView.CaptureElementById("swell_blocks-css");
#endif
        }

        /// <summary>
        /// Get current source captured
        /// </summary>
        public void CurrentHTMLCaptured()
        {
            Debug.Log("Current Captured: " +
#if true
            m_webView.CurrentHTMLCaptured()
#else
            m_webView.CurrentHTMLCaptured().Length.ToString()
#endif
        );
        }

        /// <summary>
        /// 
        /// </summary>
        public async void GetUserAgent()
        {
            m_webView.CaptureUserAgent();

            await Task.Delay(500);  // wait for finish capture element
            Debug.Log("User Agent: " + m_webView.GetUserAgent());
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetUserAgent()
        {
            string ua = "Mozilla/5.0 (X11; Linux i686; rv:10.0) Gecko/20100101 Firefox/10.0";
            bool reload = true;
            m_webView.SetUserAgent(ua, reload);
        }

        /// <summary>
        /// Added a listener to retrieve page scroll events
        /// </summary>
        public void AddEventListener()
        {
            /*
                const scrollNum = document.getElementById('scroll-num');

                window.addEventListener('scroll',function(){
                  scrollNum.textContent = window.pageYOffset;
                });
            */

            string event_name = "scroll";
            string tag_name = "window";
            string callback = tag_name + ".addEventListener('" + event_name + "'), function(){ });";

            m_webView.EvaluateJS(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageFromJavascript(string message)
        {
            string js = $"window.TLabWebViewActivity.unitySendMessage('{this.gameObject.name}', 'OnMessage', '{message}');";

            m_webView.EvaluateJS(js);

            Debug.Log(THIS_NAME + $"message sent: {message}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void OnMessage(string message)
        {
            Debug.Log(THIS_NAME + $"form javascript: {message}");
        }
    }
}