using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EarthAspect : IAspect
{
    private int _value;
    private Material _material;
    private List<GameObject> _instantiatedEffects = new List<GameObject>();

    public EarthAspect(int value, Material material)
    {
        _value = value;
        _material = material;
    }

    public int Priority => _value;

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1)
        {
            // 控制蛇形曲线的频率和幅度，随着 _value 增加，频率和幅度变大
            float curveFrequency = (_value - 1) * 2f; // 曲线的频率
            float curveAmplitude = (_value - 1) * 0.5f; // 曲线的幅度

            for (int i = 0; i < vertices.Length; i++)
            {
                // 计算每个顶点的 y 值位置，作为曲线的参考点
                float yPos = vertices[i].y;

                // 使用正弦函数使顶点沿 x 和 z 轴产生摆动
                float sinX = Mathf.Sin(yPos * curveFrequency) * curveAmplitude;
                float sinZ = Mathf.Cos(yPos * curveFrequency) * curveAmplitude;

                // 根据正弦波函数调整顶点的 x 和 z 位置
                vertices[i].x += sinX;
                vertices[i].z += sinZ;
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

