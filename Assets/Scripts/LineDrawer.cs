using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class LineDrawer : MonoBehaviour, IDragHandler
{


    [System.Serializable]
    public class DrawEvent : UnityEvent<RenderTexture>
    {
    }


    [SerializeField] Vector2Int imageDimention = new Vector2Int(28, 28);
    [SerializeField] Color paintColor = Color.white;
    [SerializeField] RenderTextureFormat format = RenderTextureFormat.R8;
    [SerializeField] Shader shader;
    [SerializeField] private RawImage sample;
    
    [SerializeField] DrawEvent OnDraw = new DrawEvent();


    RawImage imageView = null;

    Mesh lineMesh;
    Material lineMaterial;
    Texture2D clearTexture;

    RenderTexture texture;
    private float lastSample;

    void OnEnable()
    {
        imageView = GetComponent<RawImage>();

        lineMesh = new Mesh();
        lineMesh.MarkDynamic();
        lineMesh.vertices = new Vector3[2];
        lineMesh.SetIndices(new[] { 0, 1 }, MeshTopology.Lines, 0);
        lineMaterial = new Material(shader);
        lineMaterial.SetColor("_Color", paintColor);

        texture = new RenderTexture(imageDimention.x, imageDimention.y, 0, format);
        texture.filterMode = FilterMode.Bilinear;
        imageView.texture = texture;

        clearTexture = Texture2D.blackTexture;

        var trigger = GetComponent<EventTrigger>();
    }

    void Start()
    {
        ClearTexture();
    }

    void OnDisable()
    {
        texture?.Release();
        Destroy(lineMesh);
        Destroy(lineMaterial);
    }

    public void ClearTexture()
    {
        Graphics.Blit(clearTexture, texture);
    }


    private RenderTexture rt1;
    private Texture2D t1;

    public void OnDrag(PointerEventData data)
    {
        data.Use();

        var area = data.pointerDrag.GetComponent<RectTransform>();
        var p0 = area.InverseTransformPoint(data.position - data.delta);
        var p1 = area.InverseTransformPoint(data.position);

        var scale = new Vector3(2 / area.rect.width, -2 / area.rect.height, 0);
        p0 = Vector3.Scale(p0, scale);
        p1 = Vector3.Scale(p1, scale);

        DrawLine(p0, p1);
        
        // if ((Time.time - lastSample) < .2f)
        //     return;
        //
        // lastSample = Time.time;

        rt1 = texture;
        RenderTexture.active = rt1;
        t1 = new Texture2D(28, 28, TextureFormat.R8, false);
        t1.ReadPixels(new Rect(0, 0, 28, 28), 0, 0);
        t1.Apply();
        RenderTexture.active = null;
        // sample.texture = t1;
        sample.texture = CenterPixels(t1);

        // for (int x = 0; x < imageDimention.x; x++)
        // {
        //     string output = "";
        //     for (int y = 0; y < imageDimention.y; y++)
        //     {
        //         if (t1.GetPixel(x, y).r > 0f)
        //         {
        //             output += t1.GetPixel(x, y) + " ";
        //         }
        //     }
        //     Debug.Log(output);
        // }

        OnDraw.Invoke(TextureToRenderTexture(t1));
    }

    void DrawLine(Vector3 p0, Vector3 p1)
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = texture;

        lineMesh.SetVertices(new List<Vector3>() { p0, p1 });
        lineMaterial.SetPass(0);
        Graphics.DrawMeshNow(lineMesh, Matrix4x4.identity);
        // Graphics.DrawMeshNow(lineMesh, (p0+p1)/2, Quaternion.identity);
        

        RenderTexture.active = prevRT;
    }

    private Texture2D CenterPixels(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

// Calculate the bounds of the colored pixels
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > 0f)
            {
                if (bounds.size == Vector3.zero)
                {
                    bounds = new Bounds(new Vector3(i % texture.width, i / texture.width, 0f), Vector3.one);
                }
                else
                {
                    bounds.Encapsulate(new Vector3(i % texture.width, i / texture.width, 0f));
                }
            }
        }

// Calculate the offset to center the bounds in the texture
        Vector2 offset = new Vector2((texture.width - bounds.size.x) / 2f - bounds.min.x,
            (texture.height - bounds.size.y) / 2f - bounds.min.y);

// Shift the texture
        Color[] shiftedPixels = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % texture.width;
            int y = i / texture.width;
            int shiftedX = Mathf.RoundToInt(x + offset.x);
            int shiftedY = Mathf.RoundToInt(y + offset.y);
            if (shiftedX >= 0 && shiftedX < texture.width && shiftedY >= 0 && shiftedY < texture.height)
            {
                shiftedPixels[shiftedY * texture.width + shiftedX] = pixels[i];
            }
        }

        texture.SetPixels(shiftedPixels);
        texture.Apply();
        return texture;
    }

    private RenderTexture TextureToRenderTexture(Texture texture)
    {
        RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0);
        RenderTexture.active = renderTexture;
        Graphics.Blit(texture, renderTexture);
        RenderTexture.active = null;
        return renderTexture;
    }
}

