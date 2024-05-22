using UnityEngine;
using UnityEngine.EventSystems;

namespace TLab.Android.WebView
{
    public class WebViewInputListener : MonoBehaviour,
        IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private TLabWebView m_webview;

        private bool m_pointerDown = false;
        private int? m_pointerId = null;
        private RenderMode m_renderMode;
        private Vector2Int m_inputPosition;

        private enum WebTouchEvent
        {
            DOWN,
            UP,
            DRAG
        };

        private string THIS_NAME => "[" + this.GetType() + "] ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        private bool GetInputPosition(PointerEventData eventData)
        {
            Vector2 localPosition = Vector2.zero;

            RectTransform rectTransform = (RectTransform)transform;

            switch (m_renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    localPosition = transform.InverseTransformPoint(eventData.position);
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPosition);
                    break;
            }

            float x = localPosition.x / rectTransform.rect.width + rectTransform.pivot.x;
            float y = 1f - (localPosition.y / rectTransform.rect.height + rectTransform.pivot.y);

            if (Math.Range(x, 0, 1) && Math.Range(y, 0, 1))
            {
                m_inputPosition = new Vector2Int((int)(x * m_webview.webWidth), (int)(y * m_webview.webHeight));

                return true;
            }

            m_inputPosition = Vector2Int.zero;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_pointerId == null && !m_pointerDown && GetInputPosition(eventData))
            {
                m_pointerId = eventData.pointerId;

                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.DOWN);

                m_pointerDown = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if ((m_pointerId == eventData.pointerId) && m_pointerDown && GetInputPosition(eventData))
            {
                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.DRAG);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if ((m_pointerId == eventData.pointerId) && m_pointerDown && GetInputPosition(eventData))
            {
                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.UP);

                m_pointerId = null;

                m_pointerDown = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if ((m_pointerId == eventData.pointerId) && m_pointerDown)
            {
                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.UP);

                m_pointerId = null;

                m_pointerDown = false;
            }
        }

        private void OnEnable()
        {
            Canvas canvas = GetComponentInParent<Canvas>();

            if (canvas == null)
            {
                Debug.LogError(THIS_NAME + "canvas not found");
                return;
            }

            m_renderMode = canvas.renderMode;

            m_pointerId = null;

            m_pointerDown = false;
        }

        private void OnDisable()
        {
            m_pointerId = null;

            m_pointerDown = false;
        }
    }

    public static class Math
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Range(float i, float min, float max)
        {
            if (min >= max)
            {
                return false;
            }

            return i >= min && i <= max;
        }
    }
}
