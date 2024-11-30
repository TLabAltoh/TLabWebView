using UnityEngine;
using UnityEngine.SceneManagement;

namespace TLab.WebView.Test
{
    public class LoadUnloadTest : MonoBehaviour
    {
        private const string MOBILE_SAMPLE = "Mobile Sample";

        public void UnloadScene() => SceneManager.UnloadSceneAsync(MOBILE_SAMPLE, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        public void LoadScene() => SceneManager.LoadSceneAsync(MOBILE_SAMPLE, LoadSceneMode.Additive);
    }
}
