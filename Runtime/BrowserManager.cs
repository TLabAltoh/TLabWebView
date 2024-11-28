using UnityEngine;

namespace TLab.WebView
{
    public class BrowserManager : MonoBehaviour
    {
        private void Update()
        {
            FragmentCapture.GarbageCollect();
        }
    }
}
