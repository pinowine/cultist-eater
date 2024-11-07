using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;
using Vuforia;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    [Header("Grid Settings")]
    [Range(1, 5)]
    public int Resolution = 1;

    [Header("Sphere Settings")]
    public float radius = 1f;

    [Header("Aspect Parameters (1-5)")]
    [Range(1, 5)] public int Flame = 1;
    [Range(1, 5)] public int Tree = 1;
    [Range(1, 5)] public int Forge = 1;
    [Range(1, 5)] public int Edge = 1;
    [Range(1, 5)] public int Winter = 1;
    [Range(1, 5)] public int Earth = 1;
    [Range(1, 5)] public int Apple = 1;
    [Range(1, 5)] public int Stump = 1;

    [Header("Aspect Symbols")]
    public Sprite flameSprite;
    public Sprite treeSprite;
    public Sprite forgeSprite;
    public Sprite edgeSprite;
    public Sprite winterSprite;
    public Sprite earthSprite;
    public Sprite appleSprite;
    public Sprite stumpSprite;

    [Header("UI Settings")]
    public Transform aspectSymbolsContainer;

    // Previous parameter values to detect changes
    private int prevFlame;
    private int prevTree;
    private int prevForge;
    private int prevEdge;
    private int prevWinter;
    private int prevEarth;
    private int prevApple;
    private int prevStump;

    private float prevRadius;
    private int prevResolution;

    // Materials
    public Material defaultMaterial;
    public Material flameMaterial;
    public Material forgeMaterial;
    public Material transparentMaterial;
    public Material toonMaterial;
    public Material oilPaintMaterial;
    public Material pixelatedMaterial;
    public Material pixelatedShaderMaterial;
    public Material earthMaterial;
    public Material cyberpunkMaterial;

    // Additional effects
    public GameObject flameEffectPrefab;
    public GameObject bloodEffectPrefab;
    public GameObject frostEffectPrefab;

    // List to keep track of instantiated effects
    private List<GameObject> instantiatedEffects = new List<GameObject>();

    // Original mesh data
    private Vector3[] originalVertices;
    private Color[] originalColors;

    // Aspect Manager
    private AspectManager aspectManager;

    private float[] previousWeights = new float[8];
    private bool isInitialized = false;

    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);
        for (int i = 0; i < Display.displays.Length; i++)
        {
            //开启存在的屏幕显示，激活显示器
            Display.displays[i].Activate();
            Screen.SetResolution(Display.displays[i].renderingWidth, Display.displays[i].renderingHeight, true);
            Screen.fullScreen = true;
        }
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Create the sphere mesh with the desired resolution
        CreateSphereMesh(Resolution * 10, Resolution * 10, radius);

        // Initialize AspectManager and Aspects
        InitializeAspects();

        // Generate the mesh
        UpdateAll();
    }

    // Edit Mode Update
    void OnValidate()
    {
        InitializeAspects();
        if (Application.isPlaying)
        {
            UpdateAll();
        }
    }

    // Play Mode Update
    void Update()
    {
        UpdateAll();
    }

    void InitializeAspects()
    {
        if (aspectManager == null)
        {
            aspectManager = new AspectManager();
        }
        else
        {
            aspectManager.ClearAspects();
        }

        // Create instances of each aspect with their parameters
        aspectManager.AddAspect(new FlameAspect(Flame, flameMaterial, radius));
        aspectManager.AddAspect(new TreeAspect(Tree, toonMaterial));
        aspectManager.AddAspect(new EarthAspect(Earth, earthMaterial));
        aspectManager.AddAspect(new ForgeAspect(Forge, forgeMaterial));
        aspectManager.AddAspect(new EdgeAspect(Edge, cyberpunkMaterial));
        aspectManager.AddAspect(new WinterAspect(Winter, transparentMaterial));
        aspectManager.AddAspect(new AppleAspect(Apple, oilPaintMaterial));
        aspectManager.AddAspect(new StumpAspect(Stump, pixelatedMaterial, pixelatedShaderMaterial));
        // aspectManager.AddAspect(new TreeAspect(Tree, treeMaterial, ...));
        // ...
    }

    void UpdateAll()
    {
        InitializeAspects();
        if (ArduinoReader.Instance != null)
        {
            UpdateParametersFromData();
        }
        UpdateMesh();
        UpdateMaterial();
        UpdateRendering();
        HandleEffects();
        UpdateUISymbols();
    }

    void UpdateMesh()
    {
        // reset verticies and colors
        if (originalVertices == null || originalColors == null)
        {
            // Initialize original vertices and colors
            originalVertices = (Vector3[])mesh.vertices.Clone();
            originalColors = (Color[])mesh.colors.Clone();
        }

        vertices = (Vector3[])originalVertices.Clone();
        colors = (Color[])originalColors.Clone();

        // Apply mesh effects from aspects
        aspectManager.ApplyMeshEffects(ref vertices, radius);

        // Update the mesh
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles; // Ensure triangles are set
        mesh.RecalculateNormals();

        // Update MeshCollider
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }

    void UpdateMaterial()
    {
        // Get the highest priority material from aspects
        Material selectedMaterial = aspectManager.GetHighestPriorityMaterial() ?? defaultMaterial;
        GetComponent<MeshRenderer>().material = selectedMaterial;
    }

    void UpdateRendering()
    {
        aspectManager.ApplyRenderingEffects();
    }

    void UpdateUISymbols()
    {
        if (aspectSymbolsContainer == null)
            return;

        foreach (Transform child in aspectSymbolsContainer)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // 创建符号的方法
        void CreateSymbols(int count, Sprite sprite)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject symbolGO = new GameObject("Symbol", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
                symbolGO.transform.SetParent(aspectSymbolsContainer);
                UnityEngine.UI.Image image = symbolGO.GetComponent<UnityEngine.UI.Image>();
                image.sprite = sprite;

                // 可根据需要调整符号的位置和大小
                RectTransform rectTransform = symbolGO.GetComponent<RectTransform>();
                rectTransform.localScale = Vector3.one;
                rectTransform.sizeDelta = new Vector2(50, 50); // 设置符号的大小
                rectTransform.anchoredPosition = new Vector2(i * 60, 0); // 调整符号的位置，避免重叠
            }
        }

        // 根据属性值创建符号
        CreateSymbols((Flame-1), flameSprite);
        CreateSymbols((Tree-1), treeSprite);
        CreateSymbols((Forge - 1), forgeSprite);
        CreateSymbols((Edge - 1), edgeSprite);
        CreateSymbols((Winter - 1), winterSprite);
        CreateSymbols((Earth - 1), earthSprite);
        CreateSymbols((Apple - 1), appleSprite);
        CreateSymbols((Stump - 1), stumpSprite);
    }


    void HandleEffects()
    {
        aspectManager.ClearEffects();
        aspectManager.InstantiateEffects(transform);
    }

    void CreateSphereMesh(int longitudeSegments, int latitudeSegments, float radius)
    {
        List<Vector3> vertList = new List<Vector3>();
        List<int> triList = new List<int>();
        List<Color> colorList = new List<Color>();

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float a1 = Mathf.PI * lat / latitudeSegments;
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float a2 = 2 * Mathf.PI * lon / longitudeSegments;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                Vector3 normal = new Vector3(sin1 * cos2, cos1, sin1 * sin2);
                vertList.Add(normal * radius);
                colorList.Add(Color.white);
            }
        }

        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int current = lat * (longitudeSegments + 1) + lon;
                int next = current + longitudeSegments + 1;

                triList.Add(current);
                triList.Add(current + 1);
                triList.Add(next);

                triList.Add(next);
                triList.Add(current + 1);
                triList.Add(next + 1);
            }
        }

        vertices = vertList.ToArray();
        triangles = triList.ToArray();
        colors = colorList.ToArray();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        // MeshCollider
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }

        originalVertices = (Vector3[])vertices.Clone();
        originalColors = (Color[])colors.Clone();
    }

    void UpdateParametersFromData()
    {
        // Get the weights from DataReceiver
        float[] weights = ArduinoReader.Instance.weights;
        float[] currentWeights = new float[weights.Length];

        lock (weights)
        {
            weights.CopyTo(currentWeights, 0);
        }

        // Check if we have valid data
        if (!isInitialized)
        {
            // Initialize previousWeights with the first valid readings
            previousWeights = new float[weights.Length];
            Array.Copy(currentWeights, previousWeights, weights.Length);

            isInitialized = true; // Set the flag to true
            Debug.Log("Initialization complete. Variables are locked at initial values.");
            return; // Skip processing until next update
        }

        // Update variables based on weights
        UpdateVariable(ref Flame, currentWeights[0], ref previousWeights[0], 1, 5, 0);
        UpdateVariable(ref Tree, currentWeights[1], ref previousWeights[1], 1, 5, 1);
        UpdateVariable(ref Forge, currentWeights[2], ref previousWeights[2], 1, 5, 2);
        UpdateVariable(ref Edge, currentWeights[3], ref previousWeights[3], 1, 5, 3);
        UpdateVariable(ref Winter, currentWeights[4], ref previousWeights[4], 1, 5, 4);
        UpdateVariable(ref Earth, currentWeights[5], ref previousWeights[5], 1, 5, 5);
        UpdateVariable(ref Apple, currentWeights[6], ref previousWeights[6], 1, 5, 6);
        UpdateVariable(ref Stump, currentWeights[7], ref previousWeights[7], 1, 5, 7);
    }

    void UpdateVariable(ref int variable, float currentWeight, ref float previousWeight, int minValue, int maxValue, int index)
    {
        float weightChangeThreshold = 50f; // Threshold to detect addition/removal of a stone
        float weightDifference = currentWeight - previousWeight;

        if (Mathf.Abs(weightDifference) >= weightChangeThreshold)
        {
            if (weightDifference > 0)
            {
                // Weight increased significantly, increment variable by 1
                int newValue = Mathf.Min(variable + 1, maxValue);
                if (variable != newValue)
                {
                    variable = newValue;
                    Debug.Log($"Variable {index} increased to: {newValue} based on weight change of {weightDifference}g");
                }
            }
            else if (weightDifference < 0)
            {
                // Weight decreased significantly, decrement variable by 1
                int newValue = Mathf.Max(variable - 1, minValue);
                if (variable != newValue)
                {
                    variable = newValue;
                    Debug.Log($"Variable {index} decreased to: {newValue} based on weight change of {weightDifference}g");
                }
            }

            // Update the previous weight after adjustment
            previousWeight = currentWeight;
        }
    }

}
