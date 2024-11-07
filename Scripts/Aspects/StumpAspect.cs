// StumpAspect.cs
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StumpAspect : IAspect
{
    private int _value;
    private Material _material;
    private Material _shaderMaterial;

    public StumpAspect(int value, Material material, Material shaderMaterial)
    {
        _value = value;
        _material = material;
        _shaderMaterial = shaderMaterial;
    }

    public int Priority => _value;

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        // 控制裂痕的强度和深度，随着 _value 增加，裂痕变得更深
        float crackDepth = (_value - 1) * 0.3f;  // 裂痕深度
        float crackFrequency = (_value - 1) * 0.8f;  // 裂痕的频率，值越高，裂痕越多

        for (int i = 0; i < vertices.Length; i++)
        {
            // 获取当前顶点的法线方向，作为塌陷方向
            Vector3 normal = vertices[i].normalized;

            // 使用 Perlin 噪声函数生成裂痕的随机性，控制裂痕的分布
            float noiseValue = Mathf.PerlinNoise(vertices[i].x * crackFrequency, vertices[i].z * crackFrequency);

            // 如果噪声值低于某个阈值，表示此处产生裂痕
            if (noiseValue < 0.4f)  // 调整阈值以控制裂痕的数量
            {
                // 根据裂痕深度，将顶点沿法线方向向内塌陷
                vertices[i] -= normal * crackDepth * noiseValue;
            }
        }
    }

    public Material GetMaterial()
    {
        return _value > 1 ? _material : null;
    }

    public void ApplyRenderingEffect()
    {
        if (_value == 1)
        {
            _shaderMaterial.SetFloat("_PixelCount", 1920);
        }

        if (_value > 1)
        {
            _shaderMaterial.SetFloat("_PixelCount", 1000 -  _value * 150);
        }
    }

    public void InstantiateEffects(Transform parent)
    {
        // If there are any specific effects for Stump, handle them here
    }

}
