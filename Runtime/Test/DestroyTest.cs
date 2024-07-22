using UnityEngine;

namespace TLab.Android.WebView.Test
{
    public class DestroyTest : MonoBehaviour
    {
        [SerializeField] private TLabWebView m_webview;
        [SerializeField] private GameObject m_prefab;

        public void OnButtonClick()
        {
            if (m_prefab != null)
            {
                GameObject.Destroy(m_prefab);

                Debug.Log($"{m_prefab.name} has been destroyed.");
            }
        }
    }
}
