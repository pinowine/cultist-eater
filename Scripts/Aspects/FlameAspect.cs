using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlameAspect : IAspect
{
    private int _value;
    private Material _material;
    private List<GameObject> _instantiatedEffects = new List<GameObject>();

    // 定义融化点的位置列表
    private List<Vector3> _meltingPoints = new List<Vector3>();
    private int _numMeltingPoints = 0;
    private float _radius;

    public FlameAspect(int value, Material material, float radius)
    {
        _value = value;
        _material = material;
        _radius = radius;
        InitializeMeltingPoints();
    }

    public int Priority => _value;

    // 初始化融化点，根据 value 确定数量
    private void InitializeMeltingPoints()
    {
        _meltingPoints.Clear();
        _numMeltingPoints = Mathf.Clamp(_value - 1, 0, 4); // 根据 value 定义最多4个融化点

        for (int i = 0; i < _numMeltingPoints; i++)
        {
            // 随机生成下半球表面上的融化点
            Vector3 point = RandomPointOnLowerHemisphere();
            _meltingPoints.Add(point);
        }
    }

    // 在下半球表面随机生成一个点
    private Vector3 RandomPointOnLowerHemisphere()
    {
        float theta = Random.Range(0f, Mathf.PI * 2);
        float phi = Random.Range(Mathf.PI / 2, Mathf.PI); // 下半球

        float x = _radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = _radius * Mathf.Cos(phi);
        float z = _radius * Mathf.Sin(phi) * Mathf.Sin(theta);

        return new Vector3(x, y, z).normalized * _radius;
    }

    // 当 value 变化时，重新初始化融化点
    public void UpdateValue(int newValue)
    {
        if (_value != newValue)
        {
            _value = newValue;
            InitializeMeltingPoints();
        }
    }

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1 && _meltingPoints.Count > 0)
        {
            // 控制下垂的强度，随着 value 增加，下垂越明显
            float meltingT = (_value - 1) / 2f; // value 1-5 对应 T 0-1

            // 控制下垂的最大距离
            float maxDrop = radius * 0.7f * meltingT;

            // 控制噪声的频率和幅度
            float noiseFrequency = 5f * meltingT;
            float noiseAmplitude = 0.5f * meltingT;

            for (int i = 0; i < vertices.Length; i++)
            {
                // 只处理下半球的顶点
                if (vertices[i].y < 0)
                {
                    Vector3 vertexWorldPos = vertices[i];

                    // 找到最近的融化点
                    float minDistance = float.MaxValue;
                    Vector3 closestMeltingPoint = Vector3.zero;

                    foreach (var meltingPoint in _meltingPoints)
                    {
                        float distance = Vector3.Distance(vertexWorldPos, meltingPoint);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestMeltingPoint = meltingPoint;
                        }
                    }

                    // 定义一个影响半径，只有在影响半径内的顶点才会被下垂
                    float influenceRadius = radius * 0.4f;

                    if (minDistance < influenceRadius)
                    {
                        // 计算下垂量，距离越近，下垂越多
                        float dropFactor = 1f - (minDistance / influenceRadius);

                        // 基于 dropFactor 和 meltingT 计算实际下垂量
                        float dropAmount = dropFactor * maxDrop;

                        // 应用下垂到 y 轴
                        vertices[i].y -= dropAmount;

                        // 添加一些水平噪声，模拟自然流动
                        float noiseX = Mathf.PerlinNoise(vertices[i].x * noiseFrequency, vertices[i].z * noiseFrequency);
                        float noiseZ = Mathf.PerlinNoise(vertices[i].z * noiseFrequency, vertices[i].x * noiseFrequency);

                        vertices[i].x += (noiseX - 0.5f) * noiseAmplitude;
                        vertices[i].z += (noiseZ - 0.5f) * noiseAmplitude;
                    }
                }
            }
        }
    }

    public Material GetMaterial()
    {
        return _value > 1 ? _material : null;
    }

    public void ApplyRenderingEffect()
    {
        // Implement any rendering effects specific to FlameAspect
    }

    // Additional methods for handling effects
    public void InstantiateEffects(Transform parent)
    {
        // Waiting
    }

    public void ClearEffects()
    {
        foreach (var effect in _instantiatedEffects)
        {
            if (Application.isPlaying)
                Object.Destroy(effect);
            else
                Object.DestroyImmediate(effect);
        }
        _instantiatedEffects.Clear();
    }

}
