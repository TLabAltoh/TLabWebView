using UnityEngine;

namespace TLab.WebView.Test
{
    public class OnMessageTest : MonoBehaviour
    {
        public void OnMessage(string message)
        {
            Debug.Log($"{nameof(OnMessage)}: {message}");
        }
    }
}
