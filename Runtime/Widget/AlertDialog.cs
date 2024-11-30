using UnityEngine;
using UnityEngine.Events;

namespace TLab.WebView.Widget
{
    public interface IWidgetHandler<T0, T1> where T0 : Widget.Init where T1 : Widget
    {
        void OnDialog(T0 init, T1 widget);

        void Close();
    }

    public interface IAlertDialogHandler
    {
        void Positive();

        void Neutral();

        void Negative();
    }

    public class AlertDialog : Widget
    {
        [System.Serializable]
        public new class Init : Widget.Init
        {
            public Init() : base() { }
            public Init(string init) : base(init) { }

            [System.Serializable]
            public class OverlayInit
            {
                public Type type;
                public string init;
            }

            public int reason;
            public bool positive;
            public bool neutral;
            public bool negative;
            public string positiveLabel = "";
            public string neutralLabel = "";
            public string negativeLabel = "";
            public OverlayInit overlay;
        }

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public enum Result
        {
            POSITIVE = 1,
            NEUTRAL = 0,
            NEGATIVE = -1,
            DISMISS = -2,
        };

        public enum Reason
        {
            PROMPT = 0,
            ERROR = 1,
        };

        [SerializeField] private BrowserContainer m_container;
        [SerializeField] private UnityEvent<Init, AlertDialog> m_onDialog;
        [SerializeField] private UnityEvent m_onClose;

        private Widget m_overlay;

        public bool hasOverlay => m_overlay != null;

        public void Post(Result result)
        {
            var json = m_overlay?.Marshall();

            Close();

            m_container.browser?.PostDialogResult(result, json);
        }

        public override void OnDialog(Widget.Init raw, Widget parent)
        {
            if (raw is not Init)
            {
                Debug.LogError(THIS_NAME + $"Type must be {nameof(Init)}");
                return;
            }

            var init = raw as Init;

            m_onDialog?.Invoke(init, this);

            m_overlay = null;

            switch (init.overlay.type)
            {
                case Type.None:
                    break;
                case Type.Auth:
                    {
                        m_overlay = GetComponentInChildren<AuthWidget>();
                        m_overlay?.OnDialog(new AuthWidget.Init(init.overlay.init), this);
                    }
                    break;
                case Type.Color:
                    {
                        m_overlay = GetComponentInChildren<ColorWidget>();
                        m_overlay?.OnDialog(new ColorWidget.Init(init.overlay.init), this);
                    }
                    break;
                case Type.DateTime:
                    {
                        m_overlay = GetComponentInChildren<DateTimeWidget>();
                        m_overlay?.OnDialog(new DateTimeWidget.Init(init.overlay.init), this);
                    }
                    break;
                case Type.Text:
                    {
                        m_overlay = GetComponentInChildren<TextWidget>();
                        m_overlay?.OnDialog(new TextWidget.Init(init.overlay.init), this);
                    }
                    break;
                case Type.Select:
                    {
                        m_overlay = GetComponentInChildren<SelectWidget>();
                        m_overlay?.OnDialog(new SelectWidget.Init(init.overlay.init), this);
                    }
                    break;
            }

            Debug.Log(THIS_NAME + init.Marshall());
        }

        public override string Marshall()
        {
            throw new System.NotImplementedException();
        }

        public override void Close()
        {
            m_overlay?.Close();
            m_overlay = null;

            m_onClose?.Invoke();
        }
    }
}
