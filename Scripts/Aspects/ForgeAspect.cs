using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class ForgeAspect : IAspect
{
    private int _value;
    private Material _material;
    public ForgeAspect(int value, Material material)
    {
        _value = value;
        _material = material;
    }

    public int Priority => _value;

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1)
        {
            // 控制硬边效果的过渡，随着 value 增加，网格逐渐变成正十六面体
            float polyhedronT = (_value - 1) / 4f; // 当 value 为 5 时，polyhedronT 为 1，达到正十六面体效果

            // 正十六面体的 12 个顶点方向向量
            Vector3[] icosahedronDirections = new Vector3[] {
            new Vector3(1, 1, 1).normalized,
            new Vector3(1, 1, -1).normalized,
            new Vector3(1, -1, 1).normalized,
            new Vector3(1, -1, -1).normalized,
            new Vector3(-1, 1, 1).normalized,
            new Vector3(-1, 1, -1).normalized,
            new Vector3(-1, -1, 1).normalized,
            new Vector3(-1, -1, -1).normalized,
            new Vector3(0, Mathf.Sqrt(2), 1).normalized,
            new Vector3(0, Mathf.Sqrt(2), -1).normalized,
            new Vector3(0, -Mathf.Sqrt(2), 1).normalized,
            new Vector3(0, -Mathf.Sqrt(2), -1).normalized
        };

            for (int i = 0; i < vertices.Length; i++)
            {
                // 获取当前顶点的法线方向（即单位化后的方向向量）
                Vector3 direction = vertices[i].normalized;

                // 找到距离当前顶点最近的正十六面体方向向量
                Vector3 closestDirection = icosahedronDirections[0];
                float closestDot = Vector3.Dot(direction, closestDirection);
                for (int j = 1; j < icosahedronDirections.Length; j++)
                {
                    float dot = Vector3.Dot(direction, icosahedronDirections[j]);
                    if (dot > closestDot)
                    {
                        closestDot = dot;
                        closestDirection = icosahedronDirections[j];
                    }
                }

                // 逐渐插值让顶点从球形过渡到正十六面体形状
                Vector3 newDirection = Vector3.Lerp(direction, closestDirection, polyhedronT);

                // 将顶点移动到新的方向，并保持半径不变
                vertices[i] = newDirection * radius;
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

}
