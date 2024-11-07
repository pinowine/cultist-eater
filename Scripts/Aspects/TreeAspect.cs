using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TreeAspect : IAspect
{
    private int _value;
    private Material _material;
    private List<GameObject> _instantiatedEffects = new List<GameObject>();

    public TreeAspect(int value, Material material)
    {
        _value = value;
        _material = material;
    }

    public int Priority => _value;

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1)
        {
            // cylindricalT 控制圆柱化的程度，随着 _value 增加，形状更接近圆柱体
            float cylindricalT = (_value - 1) / 4f;

            for (int i = 0; i < vertices.Length; i++)
            {
                // 计算 x 和 z 的极坐标半径（仅影响 x 和 z 轴的分布）
                float distance = Mathf.Sqrt(vertices[i].x * vertices[i].x + vertices[i].z * vertices[i].z);

                // 如果 distance 为0，意味着顶点在中心，不需要调整
                if (distance > 0)
                {
                    // 计算单位向量，拉向圆柱表面
                    float normalizedX = vertices[i].x / distance;
                    float normalizedZ = vertices[i].z / distance;

                    // 将 x 和 z 拉向圆柱表面，保持 y 值不变
                    float newX = Mathf.Lerp(vertices[i].x, normalizedX * radius, cylindricalT);
                    float newZ = Mathf.Lerp(vertices[i].z, normalizedZ * radius, cylindricalT);

                    // 保持 y 值不变，顶点仅在 x 和 z 轴上调整
                    vertices[i] = new Vector3(newX, vertices[i].y, newZ);
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
