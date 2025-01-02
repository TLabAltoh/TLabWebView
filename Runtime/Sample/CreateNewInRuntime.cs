using System.Collections.Generic;
using UnityEngine;

namespace TLab.WebView.Sample
{
    public class CreateNewInRuntime : MonoBehaviour
    {
        [SerializeField] private GameObject m_prefab;
        [SerializeField] private Transform m_anchor;

        private Queue<GameObject> m_instances = new Queue<GameObject>();

        public void CreateNew()
        {
            var instance = (m_anchor == null) ? Instantiate(m_prefab) : Instantiate(m_prefab, m_anchor.position, m_anchor.rotation);

            instance.transform.parent = null;

            m_instances.Enqueue(instance);
        }

        public void Delete()
        {
            if (m_instances.Count > 0)
            {
                var instance = m_instances.Dequeue();

                Destroy(instance);
            }
        }
    }
}
