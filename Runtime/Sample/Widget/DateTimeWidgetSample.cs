using UnityEngine;

namespace TLab.WebView.Widget.Sample
{
    public class DateTimeWidgetSample : MonoBehaviour, IWidgetHandler<DateTimeWidget.Init, DateTimeWidget>
    {
        [SerializeField] private GameObject m_content;

        [Header("Date")]
        [SerializeField] private NumberPicker m_year;
        [SerializeField] private NumberPicker m_month;
        [SerializeField] private NumberPicker m_dayOfMonth;

        [Header("Time")]
        [SerializeField] private NumberPicker m_hour;
        [SerializeField] private NumberPicker m_minutes;

        public static int[] monthOfYear = new int[] { 31, 28 /* 29 */, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private string THIS_NAME => "[" + this.GetType() + "] ";

        private DateTimeWidget m_widget;

        public void OnMonthChanged(int month)
        {
            var prev = m_dayOfMonth.value;
            m_dayOfMonth.range = new Vector2Int(1, monthOfYear[month - 1]);
            if (prev != m_dayOfMonth.value)
                m_widget?.OnDayOfMonthChanged(m_dayOfMonth.value);
        }

        public void Close()
        {
            m_widget = null;
            m_content.SetActive(false);
        }

        private void InitNumberPicker(NumberPicker picker, bool active, int value)
        {
            picker.gameObject.SetActive(active);
            picker.value = value;
        }

        public void OnDialog(DateTimeWidget.Init init, DateTimeWidget widget)
        {
            m_widget = widget;

            InitNumberPicker(m_year, init.date, init.year);
            InitNumberPicker(m_month, init.date, init.month);
            InitNumberPicker(m_dayOfMonth, init.date, init.dayOfMonth);

            InitNumberPicker(m_hour, init.time, init.hour);
            InitNumberPicker(m_minutes, init.time, init.minutes);

            m_content.SetActive(true);
        }
    }
}
