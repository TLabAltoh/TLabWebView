using System.Collections.Generic;
using UnityEngine;

namespace TLab.WebView.Sample
{
    public class CreateNewInRuntime : MonoBehaviour
    {
        [SerializeField] private GameObject m_prefab;

        private Queue<GameObject> m_instances = new Queue<GameObject>();

        public void CreateNew()
        {
            var instance = Instantiate(m_prefab);

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
