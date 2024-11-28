using UnityEngine;

namespace TLab.WebView
{
    public interface IOffscreen
    {
        void Resize(Vector2Int texSize, Vector2Int viewSize);

        void ResizeTex(Vector2Int texSize);

        void ResizeView(Vector2Int viewSize);

        void SetFps(int fps);
    }
}
