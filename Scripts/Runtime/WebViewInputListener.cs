using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TLab.Android.WebView
{
    public class WebViewInputListener : MonoBehaviour,
        IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private TLabWebView m_webview;

        private bool m_pointerDown = false;
        private RenderMode m_renderMode;
        private Vector2Int m_inputPosition;

        private enum WebTouchEvent
        {
            DOWN,
            UP,
            DRAG
        };

        private string THIS_NAME => "[" + this.GetType() + "] ";

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

            Debug.Log(THIS_NAME + $"x: {x}, y: {y}");

            if (Math.Range(x, 0, 1) && Math.Range(y, 0, 1))
            {
                m_inputPosition = new Vector2Int((int)(x * m_webview.WebWidth), (int)(y * m_webview.WebHeight));

                return true;
            }

            m_inputPosition = Vector2Int.zero;

            return false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!m_pointerDown && GetInputPosition(eventData))
            {
                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.DOWN);

                m_pointerDown = true;

                Debug.Log(THIS_NAME + "pointer down");
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_pointerDown && GetInputPosition(eventData))
            {
                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.DRAG);

                Debug.Log(THIS_NAME + "pointer drag");
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_pointerDown && GetInputPosition(eventData))
            {
                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.UP);

                m_pointerDown = false;

                Debug.Log(THIS_NAME + "pointer up");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_pointerDown)
            {
                m_webview.TouchEvent(m_inputPosition.x, m_inputPosition.y, (int)WebTouchEvent.UP);

                m_pointerDown = false;

                Debug.Log(THIS_NAME + "pointer exit");
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

            m_pointerDown = false;
        }

        private void OnDisable()
        {
            m_pointerDown = false;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public static class Math
    {
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
