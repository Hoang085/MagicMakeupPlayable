using UnityEngine;
using UnityEngine.Rendering;

public class Scratch : MonoBehaviour
{
    private RenderTexture renderTexture;
    private Texture2D texture;
    private Rect rect;
    private int width;
    private int height;
    public SpriteMask spriteMask;
    public Camera spriteCam;
    public bool isRendering;

    private int updateCounter = 0;
    private int frameSkip = 0;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        width = spriteCam.pixelWidth;
        height = spriteCam.pixelHeight;
        rect = new Rect(0, 0, width, height);

        renderTexture = new RenderTexture(width, height, 0);
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        spriteCam.targetTexture = renderTexture;
    }

    public void AssignScreenAsMask()
    {
        if (NeedsUpdate())
        {
            isRendering = true;
            spriteCam.Render();
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGBA32, OnCompleteReadback);
        }

        updateCounter++;
    }

    private void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.LogError("GPU readback error.");
            return;
        }

        var data = request.GetData<Color32>();

        texture.SetPixels32(data.ToArray());
        texture.Apply();
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), pivot);
        spriteMask.sprite = sprite;
        spriteMask.transform.position = new Vector3(spriteCam.transform.position.x, spriteCam.transform.position.y,
            spriteMask.transform.position.z);
        isRendering = false;
    }

    private bool NeedsUpdate()
    {
        return true;
    }
}