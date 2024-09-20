using UnityEngine;

namespace TLab.Android.WebView
{
    [System.Serializable]
    public class JSConfig
    {
        [Tooltip("Variable name that is used temporarily in javascript")]
        public string varTmp = "__tmp";
    }
}
