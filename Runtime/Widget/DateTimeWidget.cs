using UnityEngine;
using UnityEngine.Events;

namespace TLab.WebView.Widget
{
    public class DateTimeWidget : Widget
    {
        [System.Serializable]
        public new class Init : Widget.Init
        {
            public Init() : base() { }
            public Init(string init) : base(init) { }

            public int type;

            public bool date;
            public bool time;

            public long minDate = -1;
            public long maxDate = -1;

            public int year;
            public int month;
            public int dayOfMonth;

            public int hour;
            public int minutes;
        }

        [SerializeField] private UnityEvent<Init, DateTimeWidget> m_onDialog;
        [SerializeField] private UnityEvent m_onClose;

        [System.Serializable]
        public class Result
        {
            public int year;
            public int month;
            public int dayOfMonth;
            public int hour;
            public int minutes;

            public void Init(Init init)
            {
                year = init.year;
                month = init.month;
                dayOfMonth = init.dayOfMonth;
                dayOfMonth = init.dayOfMonth;
                hour = init.hour;
                minutes = init.minutes;
            }
        }

        private Result m_result = new Result();

        public void OnYearChanged(int year) => m_result.year = year;
        public void OnMonthChanged(int month) => m_result.month = month;
        public void OnDayOfMonthChanged(int dayOfMonth) => m_result.dayOfMonth = dayOfMonth;
        public void OnHourChanged(int hour) => m_result.hour = hour;
        public void OnMinutesChanged(int minutes) => m_result.minutes = minutes;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public override void Close() => m_onClose?.Invoke();

        public override void OnDialog(Widget.Init raw, Widget parent)
        {
            if (raw is not Init)
            {
                Debug.LogError(THIS_NAME + $"Type must be {nameof(Init)}");
                return;
            }

            var init = raw as Init;

            Debug.Log(THIS_NAME + init.Marshall());

            m_result.Init(init);

            m_onDialog?.Invoke(init, this);
        }

        public override string Marshall() => JsonUtility.ToJson(m_result);
    }
}
