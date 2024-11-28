using UnityEngine;
using UnityEngine.Events;

namespace TLab.WebView.Widget
{
    public class AuthWidget : Widget
    {
        [System.Serializable]
        public new class Init : Widget.Init
        {
            public Init() : base() { }

            public Init(string init) : base(init) { }

            public string username;
            public string password;
            public int inputType;
            public bool onlyPassword;
        }

        [SerializeField] private UnityEvent<Init, AuthWidget> m_onDialog;
        [SerializeField] private UnityEvent m_onClose;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        [System.Serializable]
        public class Result
        {
            public string username;
            public string password;
        }

        private Result m_result = new Result();

        public void OnUserNameChanged(string username) => m_result.username = username;

        public void OnPasswordChanged(string password) => m_result.password = password;

        public override void Close() => m_onClose?.Invoke();

        public override void OnDialog(Widget.Init raw, Widget parent)
        {
            if (raw is not Init)
            {
                Debug.LogError(THIS_NAME + $"Type must be {nameof(Init)}");
                return;
            }

            var init = raw as Init;

            m_result.username = init.username;
            m_result.password = init.password;

            m_onDialog?.Invoke(init, this);
        }

        public override string Marshall() => JsonUtility.ToJson(m_result);
    }
}
