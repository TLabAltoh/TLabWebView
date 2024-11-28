using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TLab.WebView.Widget
{
    public class SelectWidget : Widget
    {
        [System.Serializable]
        public new class Init : Widget.Init
        {
            public Init() : base() { }
            public Init(string init) : base(init) { }

            public int type;
            public ModifiableChoice[] options;
        }

        public static new class Type
        {
            public const int MENU = 1;

            public const int SINGLE = 2;

            public const int MULTIPLE = 3;
        }

        [System.Serializable]
        public class ModifiableChoice
        {
            public static class Type
            {
                public const int DEFAULT = 1;
                public const int GROUP = 2;
                public const int SEPARATOR = 3;
            }

            public bool selected;
            public int type;
            public string label;
        }

        [SerializeField] private UnityEvent<Init, SelectWidget> m_onDialog;
        [SerializeField] private UnityEvent m_onClose;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        private HashSet<int> m_select = new HashSet<int>();

        public bool Push(int index)
        {
            if (m_select.Contains(index))
            {
                m_select.Remove(index);
                return false;
            }

            if (m_type == Type.MENU || m_type == Type.SINGLE)
                m_select.Clear();

            m_select.Add(index);

            return true;
        }

        public int type => m_type;

        private int m_type;

        [System.Serializable]
        public class Result
        {
            public int[] positions;
        }

        private Result m_result = new Result();

        public override void OnDialog(Widget.Init raw, Widget parent)
        {
            if (raw is not Init)
            {
                Debug.LogError(THIS_NAME + $"Type must be {nameof(Init)}");
                return;
            }

            var init = raw as Init;

            Debug.Log(THIS_NAME + init.Marshall());

            for (int i = 0; i < init.options.Length; i++)
                if (init.options[i].selected) m_select.Add(i);

            m_type = init.type;

            m_onDialog.Invoke(init, this);
        }

        public override void Close() => m_onClose?.Invoke();

        public override string Marshall()
        {
            m_result.positions = m_select.ToArray();
            return JsonUtility.ToJson(m_result);
        }
    }
}
