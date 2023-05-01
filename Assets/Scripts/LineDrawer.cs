﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

[RequireComponent(typeof(RawImage))]
public class LineDrawer : MonoBehaviour //, IDragHandler
{


    [System.Serializable]
    public class DrawEvent : UnityEvent<Texture>
    {
    }

    public const float RESOLUTION = .2f;

    [SerializeField] Vector2Int imageDimention = new Vector2Int(28, 28);
    [SerializeField] Color paintColor = Color.white;
    [SerializeField] RenderTextureFormat format = RenderTextureFormat.R8;
    [SerializeField] Shader shader;
    [SerializeField] private RawImage sample;
    [SerializeField] private RawImage trimSample;
    [SerializeField] private Line linePrefab;
    [SerializeField] private Transform lineParent;
    [SerializeField] private Camera lineRenderCam;



    [SerializeField] DrawEvent OnDraw = new DrawEvent();


    RawImage imageView = null;

    Mesh lineMesh;
    Material lineMaterial;
    Texture2D clearTexture;

    RenderTexture texture;
    private float lastSample;
    private Line currentLine;
    private Texture2D tex;

    public Texture2D CurrentTexture2D => tex;

    void OnEnable()
    {
        imageView = GetComponent<RawImage>();
        tex = new Texture2D(imageDimention[0], imageDimention[1], TextureFormat.RGB24, false);

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
    }

    void Start()
    {
        SetUpDragTrigger();
        ClearTexture();
    }

    private void SetUpDragTrigger()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
        trigger.triggers.Add(entryDrag);

        EventTrigger.Entry entryEndDrag = new EventTrigger.Entry();
        entryEndDrag.eventID = EventTriggerType.EndDrag;
        entryEndDrag.callback.AddListener((data) => { OnEndDragDelegate((PointerEventData)data); });
        trigger.triggers.Add(entryEndDrag);
    }


    private void OnDragDelegate(PointerEventData data)
    {
        data.Use();

        var area = data.pointerDrag.GetComponent<RectTransform>();
        var p0 = area.InverseTransformPoint(data.position - data.delta);
        var p1 = area.InverseTransformPoint(data.position);

        // Debug.Log(p0 +","+ p1);
        Vector2 targetpos = Camera.main.ScreenToWorldPoint(data.position);

        if (!currentLine)
        {
            currentLine = Instantiate(linePrefab, Vector2.zero, Quaternion.identity, lineParent);
        }

        currentLine.SetPosition(targetpos);
    }

    private void OnEndDragDelegate(PointerEventData data)
    {
        if (currentLine) currentLine = null;

        Texture rt = SampleDrawingToTexture(new[] { 28, 28 });

        OnDraw.Invoke(rt);
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

        for (int i = 0; i < lineParent.childCount; i++)
            Destroy(lineParent.GetChild(i).gameObject);
    }


    private RenderTexture rt1;
    private Texture2D t1;

    // public void OnDrag(PointerEventData data)
    // {
    //     data.Use();
    //
    //     var area = data.pointerDrag.GetComponent<RectTransform>();
    //     var p0 = area.InverseTransformPoint(data.position - data.delta);
    //     var p1 = area.InverseTransformPoint(data.position);
    //
    //     var scale = new Vector3(2 / area.rect.width, -2 / area.rect.height, 0);
    //     p0 = Vector3.Scale(p0, scale);
    //     p1 = Vector3.Scale(p1, scale);
    //
    //     DrawLine(p0, p1);
    //     
    //     // if ((Time.time - lastSample) < .2f)
    //     //     return;
    //     //
    //     // lastSample = Time.time;
    //
    //     rt1 = texture;
    //     RenderTexture.active = rt1;
    //     t1 = new Texture2D(28, 28, TextureFormat.R8, false);
    //     t1.ReadPixels(new Rect(0, 0, 28, 28), 0, 0);
    //     t1.Apply();
    //     RenderTexture.active = null;
    //     // sample.texture = t1;
    //     sample.texture = CenterPixels(t1);
    //
    //     // for (int x = 0; x < imageDimention.x; x++)
    //     // {
    //     //     string output = "";
    //     //     for (int y = 0; y < imageDimention.y; y++)
    //     //     {
    //     //         if (t1.GetPixel(x, y).r > 0f)
    //     //         {
    //     //             output += t1.GetPixel(x, y) + " ";
    //     //         }
    //     //     }
    //     //     Debug.Log(output);
    //     // }
    //
    //     OnDraw.Invoke(TextureToRenderTexture(t1));
    // }

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

    private void OnMouseDrag()
    {
        Debug.Log("OnDrag");

    }

    public Texture SampleDrawingToTexture(int[] dim)
    {
        RenderTexture rt = new RenderTexture(dim[0], dim[1], 24);
        lineRenderCam.targetTexture = rt;
        lineRenderCam.Render();
        RenderTexture.active = rt;
        Rect rectReadPixels = new Rect(0, 0, dim[0], dim[1]);
        tex.ReadPixels(rectReadPixels, 0, 0);
        tex.Apply();
        lineRenderCam.targetTexture = null;
        // sampleMat.SetTexture("_MainTex", tex);
        sample.texture = tex;

        // Find the bounds of the non-transparent pixels
        int left = texture.width;
        int right = 0;
        int top = texture.height;
        int bottom = 0;
        Color[] pixels = tex.GetPixels();

        // SaveColorArray(pixels);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color pixel = pixels[y * texture.width + x];
                if (pixel.r > 0)
                {
                    if (x < left) left = x;
                    if (x > right) right = x;
                    if (y < top) top = y;
                    if (y > bottom) bottom = y;
                }
            }
        }

        // Crop the texture
        int width = right - left + 1 + 6;
        int height = bottom - top + 1 + 6;
        Texture2D croppedTexture = new Texture2D(width, height);
        Color[] croppedPixels = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int originalX = left + x - 3;
                int originalY = top + y - 3;
                Color pixel = new Color(0, 0, 0, 1); 
                if (originalX >= 0 && originalX < texture.width && originalY >= 0 && originalY < texture.height)
                {
                    pixel = pixels[originalY * texture.width + originalX];
                }
                croppedPixels[y * width + x] = pixel;
            }
        }

        croppedTexture.SetPixels(croppedPixels);
        croppedTexture.Apply();

        // Scale the texture to 28x28
        Texture2D scaledTexture = new Texture2D(28, 28);
        Color[] scaledPixels = scaledTexture.GetPixels();

        for (int x = 0; x < 28; x++)
        {
            for (int y = 0; y < 28; y++)
            {
                float u = (float)x / 28f * (float)(width - 1) / (float)width;
                float v = (float)y / 28f * (float)(height - 1) / (float)height;
                Color pixel = croppedTexture.GetPixelBilinear(u, v);
                scaledPixels[y * 28 + x] = pixel;
            }
        }

        scaledTexture.SetPixels(scaledPixels);
        scaledTexture.Apply();

        trimSample.texture = scaledTexture;


        return scaledTexture;
    }

    private void SaveColorArray(Color[] colorArray)
    {
        Color[] colors = colorArray;
        string filePath = Application.persistentDataPath + "/colors.json"; // specify the file path

// Convert the color data to JSON
        ColorData[] colorDataArray = new ColorData[colors.Length];
        for (int i = 0;
             i < colors.Length;
             i++)
        {
            colorDataArray[i].r = colors[i].r;
            colorDataArray[i].g = colors[i].g;
            colorDataArray[i].b = colors[i].b;
            colorDataArray[i].a = colors[i].a;
        }

        JsonClass jc = new JsonClass(colorDataArray);

        string json = JsonUtility.ToJson(jc);
        File.WriteAllText(filePath, json);
    }

    [Serializable]
    public struct ColorData
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

    public class JsonClass
    {
        public ColorData[] colorDataArray;

        public JsonClass(ColorData[] arr)
        {
            this.colorDataArray = arr;
        }
    }
}

