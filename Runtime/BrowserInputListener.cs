using UnityEngine;
using UnityEngine.EventSystems;

namespace TLab.WebView
{
    public class BrowserInputListener : BaseInputListener
    {
        [SerializeField] private BrowserContainer m_container;

        private long m_downTime;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public enum TouchEvent
        {
            Down,
            Up,
            Drag,
        };

        protected override void OnPointerUp(PointerEventData pointerEventData, InputEventData inputEventData)
        {
            var position = inputEventData.position;
            position.x *= m_container.browser.viewSize.x;
            position.y *= m_container.browser.viewSize.y;
            m_container.browser.TouchEvent((int)position.x, (int)position.y, (int)TouchEvent.Up, m_downTime);
        }

        protected override void OnPointerExit(PointerEventData pointerEventData, InputEventData inputEventData)
        {
            var position = inputEventData.position;
            position.x *= m_container.browser.viewSize.x;
            position.y *= m_container.browser.viewSize.y;
            m_container.browser.TouchEvent((int)position.x, (int)position.y, (int)TouchEvent.Up, m_downTime);
        }

        protected override void OnPointerDown(PointerEventData pointerEventData, InputEventData inputEventData)
        {
            var position = inputEventData.position;
            position.x *= m_container.browser.viewSize.x;
            position.y *= m_container.browser.viewSize.y;
            m_downTime = m_container.browser.TouchEvent((int)position.x, (int)position.y, (int)TouchEvent.Down, m_downTime);
        }

        protected override void OnDrag(PointerEventData pointerEventData, InputEventData inputEventData)
        {
            var position = inputEventData.position;
            position.x *= m_container.browser.viewSize.x;
            position.y *= m_container.browser.viewSize.y;
            m_container.browser.TouchEvent((int)position.x, (int)position.y, (int)TouchEvent.Drag, m_downTime);
        }
    }
}