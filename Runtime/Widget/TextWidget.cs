using UnityEngine;

namespace TLab.WebView.Widget
{
    public class TextWidget : Widget
    {
        [System.Serializable]
        public new class Init : Widget.Init
        {
            public Init() : base() { }
            public Init(string init) : base(init) { }

            public string text;
            public string hint;
            public int inputType;
        }

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public override void Close()
        {
            throw new System.NotImplementedException();
        }

        public override void OnDialog(Widget.Init raw, Widget parent)
        {
            if (raw is not Init)
            {
                Debug.LogError(THIS_NAME + $"Type must be {nameof(Init)}");
                return;
            }

            var init = raw as Init;
        }

        public override string Marshall()
        {
            throw new System.NotImplementedException();
        }
    }
}
