using UnityEngine;
using UnityEngine.EventSystems;

namespace TLab.WebView
{
    public abstract class BaseInputListener : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler
    {
        private bool m_pointerDown = false;
        private int? m_pointerId = null;
        private RenderMode m_renderMode;
        private Vector2 m_current;
        private Vector2 m_prev;

        private InputEventData m_inputEventData = new InputEventData();

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public class InputEventData
        {
            public Vector2 position;
            public Vector2 delta;

            public InputEventData(Vector2 position, Vector2 delta) => Update(position, delta);

            public InputEventData() { }

            public void Update(Vector2 position, Vector2 delta)
            {
                this.position = position;
                this.delta = delta;
            }
        }

        protected bool GetInputPosition(PointerEventData eventData)
        {
            m_prev = m_current;

            var localPosition = Vector2.zero;

            var rectTransform = (RectTransform)transform;

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

            var x = localPosition.x / rectTransform.rect.width + rectTransform.pivot.x;
            var y = 1f - (localPosition.y / rectTransform.rect.height + rectTransform.pivot.y);

            if (Range(x, 0, 1) && Range(y, 0, 1))
            {
                m_current = new Vector2(x, y);

                return true;
            }

            m_current = Vector2.zero;

            return false;
        }

        protected abstract void OnPointerDown(PointerEventData pointerEventData, InputEventData inputEventData);

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_pointerId == null && !m_pointerDown && GetInputPosition(eventData))
            {
                m_pointerId = eventData.pointerId;

                m_inputEventData.Update(m_current, Vector2.zero);
                OnPointerDown(eventData, m_inputEventData);

                m_pointerDown = true;
            }
        }

        protected abstract void OnDrag(PointerEventData pointerEventData, InputEventData inputEventData);

        public void OnDrag(PointerEventData eventData)
        {
            if ((m_pointerId == eventData.pointerId) && m_pointerDown && GetInputPosition(eventData))
            {
                m_inputEventData.Update(m_current, m_current - m_prev);
                OnDrag(eventData, m_inputEventData);
            }
        }

        protected abstract void OnPointerUp(PointerEventData pointerEventData, InputEventData inputEventData);

        public void OnPointerUp(PointerEventData eventData)
        {
            if ((m_pointerId == eventData.pointerId) && m_pointerDown && GetInputPosition(eventData))
            {
                m_inputEventData.Update(m_current, m_current - m_prev);
                OnPointerUp(eventData, m_inputEventData);

                m_pointerId = null;

                m_pointerDown = false;
            }
        }

        protected abstract void OnPointerExit(PointerEventData pointerEventData, InputEventData inputEventData);

        public void OnPointerExit(PointerEventData eventData)
        {
            if ((m_pointerId == eventData.pointerId) && m_pointerDown)
            {
                m_inputEventData.Update(m_current, m_current - m_prev);
                OnPointerExit(eventData, m_inputEventData);

                m_pointerId = null;

                m_pointerDown = false;
            }
        }

        protected virtual void OnEnable()
        {
            var canvas = GetComponentInParent<Canvas>();

            if (canvas == null)
            {
                Debug.LogError(THIS_NAME + "canvas not found");
                return;
            }

            m_renderMode = canvas.renderMode;

            m_pointerId = null;

            m_pointerDown = false;
        }

        protected virtual void OnDisable()
        {
            m_pointerId = null;

            m_pointerDown = false;
        }

        public static bool Range(float i, float min, float max)
        {
            if (min >= max)
                return false;

            return i >= min && i <= max;
        }
    }
}