using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TLab.WebView.Widget
{
    public class NumberPicker : BaseInputListener
    {
        [SerializeField] private TMPro.TextMeshProUGUI m_text;
        [SerializeField] private int m_value = 0;
        [SerializeField, Min(0f)] private float m_power = 0.05f;
        [SerializeField, Min(0f)] private float m_duration = 0.01f;
        [SerializeField] private Vector2Int m_range = new Vector2Int(0, 10);
        [SerializeField] private UnityEvent<int> m_onValueChanged;

        private float m_inertia = 0;
        private float m_wheel = 0;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public Vector2Int range
        {
            get => m_range;
            set
            {
                if (m_range != value)
                {
                    m_range = value;

                    var tmp = Validate(this.value);

                    this.value = tmp;
                }
            }
        }

        public int value
        {
            get => m_value;
            set
            {
                var tmp = Validate(value);

                if (m_value != tmp)
                {
                    m_value = tmp;

                    UpdateText();

                    m_onValueChanged.Invoke(m_value);
                }
            }
        }

        protected override void OnDrag(PointerEventData pointerEventData, InputEventData inputEventData)
        {
            m_inertia = -inputEventData.delta.y * m_power / Time.deltaTime;
        }

        protected override void OnPointerDown(PointerEventData pointerEventData, InputEventData inputEventData)
        {
            m_inertia = 0;
        }

        protected override void OnPointerExit(PointerEventData pointerEventData, InputEventData inputEventData)
        {

        }

        protected override void OnPointerUp(PointerEventData pointerEventData, InputEventData inputEventData)
        {

        }

        private void UpdateText() => m_text.text = m_value.ToString();

        public void Increment(int num)
        {
            m_inertia = 0;

            m_wheel = (int)m_wheel;
            m_wheel += num;

            value = (int)m_wheel;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_wheel = m_value;

            UpdateText();
        }

        private int Validate(int value)
        {
            // min: 0, max: 10
            // x = 11
            // 0 + (11 - 10 + 1) % (10 - 0) = 
            if (value > m_range.y)
                value = m_range.x + (value - m_range.y) % (m_range.y - m_range.x) - 1;

            if (value < m_range.x)
                value = m_range.y - (m_range.x - value) % (m_range.y - m_range.x) + 1;

            return value;
        }

        private void Update()
        {
            m_wheel += m_inertia;

            value = (int)m_wheel;

            m_inertia = System.Math.Sign(m_inertia) * Mathf.Clamp(Mathf.Abs(m_inertia) - m_duration, 0, float.MaxValue);
        }
    }
}
