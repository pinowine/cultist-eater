using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EdgeAspect : IAspect
{
    private int _value;
    private Material _material;
    private List<GameObject> _instantiatedEffects = new List<GameObject>();

    public EdgeAspect(int value, Material material)
    {
        _value = value;
        _material = material;
    }

    public int Priority => _value;

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1)
        {
            // 刺的强度因子，调整乘数以获得所需效果
            float spikeIntensity = (_value - 1) * 1f;

            for (int i = 0; i < vertices.Length; i++)
            {
                // 获取从中心指向顶点的方向向量
                Vector3 direction = vertices[i].normalized;

                // 使用 3D 噪声函数生成噪声值
                float noise = Mathf.PerlinNoise(direction.x * _value, direction.y * _value);
                noise += Mathf.PerlinNoise(direction.y * _value, direction.z * _value);
                noise += Mathf.PerlinNoise(direction.z * _value, direction.x * _value);
                noise /= 3f; // 取平均值

                // 将噪声值调整为 -1 到 1 的范围
                noise = (noise - 0.5f) * 2f;

                // 计算顶点的位移量
                float displacement = noise * spikeIntensity;

                // 将顶点沿方向向量移动
                vertices[i] += direction * displacement;
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

    public void InstantiateEffects(Transform parent)
    {
        // Implement any effect instantiation or handling here
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
