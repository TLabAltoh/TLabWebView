using UnityEngine;
using UnityEngine.Events;

namespace TLab.WebView.Widget
{
    public class ColorWidget : Widget
    {
        [System.Serializable]
        public new class Init : Widget.Init
        {
            public Init() : base() { }
            public Init(string init) : base(init) { }

            public int color;
        }

        [SerializeField] private UnityEvent<Init, ColorWidget> m_onDialog;
        [SerializeField] private UnityEvent m_onClose;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public static Color UnMarshall(int rgb) => new Color((float)((rgb >> 16) & 0xff) / byte.MaxValue, (float)((rgb >> 8) & 0xff) / byte.MaxValue, (float)((rgb >> 0) & 0xff) / byte.MaxValue);

        public static int Marshall(Color rgb) => ((int)(rgb.r * byte.MaxValue) << 16) | ((int)(rgb.g * byte.MaxValue) << 8) | (int)(rgb.b * byte.MaxValue);

        [System.Serializable]
        public class Result
        {
            public int color;
        }

        private Result m_result = new Result();

        private Color m_rgb;

        public Color rgb
        {
            get => m_rgb;
            set
            {
                if (m_rgb != value)
                {
                    m_rgb = value;
                    m_result.color = Marshall(m_rgb);
                }
            }
        }

        public override void OnDialog(Widget.Init raw, Widget parent)
        {
            if (raw is not Init)
            {
                Debug.LogError(THIS_NAME + $"Type must be {nameof(Init)}");
                return;
            }

            var init = raw as Init;

            Debug.Log(THIS_NAME + init.Marshall());

            Debug.Log(THIS_NAME + UnMarshall(init.color));

            m_result.color = init.color;

            m_onDialog?.Invoke(init, this);
        }

        public override string Marshall() => JsonUtility.ToJson(m_result);

        public override void Close() => m_onClose.Invoke();
    }
}
