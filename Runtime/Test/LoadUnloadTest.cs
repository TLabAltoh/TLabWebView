using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TLab.Android.WebView.Test
{
    public class LoadUnloadTest : MonoBehaviour
    {
        private const string MOBILE_SAMPLE = "Mobile Sample";

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public void UnloadScene()
        {
            SceneManager.UnloadSceneAsync(MOBILE_SAMPLE, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }

        public void LoadScene()
        {
            SceneManager.LoadSceneAsync(MOBILE_SAMPLE, LoadSceneMode.Additive);
        }
    }
}
