using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace TLab.WebView.Sample
{
    public class LoadLocalFileSample : MonoBehaviour
    {
        [SerializeField] private BrowserContainer m_container;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        public void RequestPermission()
        {
            // If you want to load a file from an external location
            // (e.g. download folder, image folder...), you need to
            // grant the MANAGE_EXTERNAL_STORAGE permission and
            // request the permission at runtime.

            // https://developer.android.com/training/data-storage/manage-all-files?hl=en

            // HasUserAuthorisedPermission may not reflect these
            // permission changes immediately, so this is not a
            // good implementation.

            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.MANAGE_EXTERNAL_STORAGE"))
                UnityEngine.Android.Permission.RequestUserPermission("android.permission.MANAGE_EXTERNAL_STORAGE");

            // I recommended to use streaming asset to load HTML
            // content archived file (like zip) and extract to
            // it in application folder.
        }

        public void OpenLocalHTML(string filePath)
        {
            // jar:file:///xxxxxxxxxx
            var url = "file://" + Application.persistentDataPath + "/local-html";

            url += ("/" + filePath);

            Debug.Log(THIS_NAME + $"url: {url}");

            m_container.browser.LoadUrl(url);
        }

        private void Start()
        {
            var subPath = "/local-html";

            var srcZip = Application.streamingAssetsPath + subPath + ".zip";

#if UNITY_EDITOR
            var dstPath = Application.dataPath;
#else
            var dstPath = Application.persistentDataPath;
#endif
            var dstZip = dstPath + $"{subPath}/{subPath}.zip";

            var outputDir = dstPath + "/" + subPath;
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            Directory.CreateDirectory(outputDir);

            var request = new WWW(srcZip);
            while (!request.isDone) { }

            if (!File.Exists(dstZip))
                File.WriteAllBytes(dstZip, request.bytes);
            else
            {
                File.Delete(dstZip);
                File.WriteAllBytes(dstZip, request.bytes);
            }

            ZipFile.ExtractToDirectory(dstZip, outputDir);

            Debug.Log(THIS_NAME + $"done !");
        }
    }
}
