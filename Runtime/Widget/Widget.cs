using UnityEngine;

namespace TLab.WebView.Widget
{
    [System.Serializable]
    public abstract class Widget : MonoBehaviour
    {
        [System.Serializable]
        public enum Type
        {
            None,
            Auth,
            Color,
            DateTime,
            Select,
            Text,
        };

        [System.Serializable]
        public class Init : JSONSerialisable
        {
            public string title;
            public string message;

            public Init() { }

            public Init(string json) : base(json) { }
        }

        public abstract void OnDialog(Init raw, Widget parent);

        public abstract void Close();

        public abstract string Marshall();
    }
}