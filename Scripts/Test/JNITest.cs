using UnityEngine;

namespace TLab.Android.WebView.Test
{
    public class JNITest : MonoBehaviour
    {
        void Start()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            Debug.Log($"jniutil [add] : {JNIUtil.add(1, 1)}");
#endif
        }

        void Update()
        {

        }
    }
}
