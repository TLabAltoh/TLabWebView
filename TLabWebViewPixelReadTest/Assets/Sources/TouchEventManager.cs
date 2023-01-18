using UnityEngine;

public class TouchEventManager : MonoBehaviour
{
    [SerializeField] RectTransform screenRect;
    private float[] screenEdge;
    private const int LEFT_IDX = 0;
    private const int RIGHT_IDX = 1;
    private const int BOTTOM_IDX = 2;
    private const int TOP_IDX = 3;
    private float[] screenSize;
    private const int VERTICAL_IDX = 0;
    private const int HORIZONTAL_IDX = 1;

    private WebViewTexture webViewTexture;

    private bool touching = false;

    void Start()
    {
        if(screenRect == null)
        {
            Debug.Log("TouchEventManager.Start: screenRect is null");
            return;
        }

        // Screen Corners[0] : Left bottom
        // Screen Corners[1] : Left tops
        // Screen Corners[2] : Right tops
        // Screen Corners[3] : Right bottom
        Vector3[] screenCorners = new Vector3[4];
        screenRect.GetWorldCorners(screenCorners);

        for(int i = 0; i < screenCorners.Length; i++)
        {
            screenCorners[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, screenCorners[i]);
            Vector3 tmp = screenCorners[i];
            Debug.Log("screenCorners[" + i + "]: " + tmp.x + ", " + tmp.y);
        }
        
        screenEdge = new float[4];
        screenEdge[LEFT_IDX] = screenCorners[0].x;
        screenEdge[RIGHT_IDX] = screenCorners[2].x;
        screenEdge[BOTTOM_IDX] = screenCorners[3].y;
        screenEdge[TOP_IDX] = screenCorners[2].y;

        screenSize = new float[2];
        screenSize[VERTICAL_IDX] = screenEdge[TOP_IDX] - screenEdge[BOTTOM_IDX];
        screenSize[HORIZONTAL_IDX] = screenEdge[RIGHT_IDX] - screenEdge[LEFT_IDX];

        webViewTexture = GameObject.Find("RenderCanvas/WebView").GetComponent<WebViewTexture>();
    }

    void Update()
    {
        if (webViewTexture == null) return;

        if(Input.touchCount == 0)
        {
            touching = false;
            return;
        }

        Touch t = Input.touches[0];
        float x, y;

        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            touching = false;
            return;
        }

        if (touching == true) return;

        touching = true;

        x = t.position.x;
        y = t.position.y;
        if (x >= screenEdge[LEFT_IDX] && x <= screenEdge[RIGHT_IDX])
        {
            if (y >= screenEdge[BOTTOM_IDX] && y <= screenEdge[TOP_IDX])
            {
                webViewTexture.TouchEvent(
                    (int)((x - screenEdge[LEFT_IDX]) / screenSize[HORIZONTAL_IDX] * webViewTexture.texWidth),
                    (int)((y - screenEdge[BOTTOM_IDX]) / screenSize[VERTICAL_IDX] * webViewTexture.texHeight)
                );
                Debug.Log("hogehoge" + x.ToString() + ", " + y.ToString());
            }
        }
    }
}
