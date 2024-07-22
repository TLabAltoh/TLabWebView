using UnityEngine;

namespace TLab.Android.WebView.Sample
{
    public class CreateNewInRuntime : MonoBehaviour
    {
        [SerializeField] private GameObject m_prefab;

        public void CreateNew()
        {
            var instance = Instantiate(m_prefab);

            instance.transform.parent = null;
        }
    }
}
