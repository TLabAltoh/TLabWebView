using UnityEngine;

public class DestroyTest : MonoBehaviour
{
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
