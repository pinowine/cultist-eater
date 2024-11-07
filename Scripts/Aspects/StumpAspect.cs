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
        // �����Ѻ۵�ǿ�Ⱥ���ȣ����� _value ���ӣ��Ѻ۱�ø���
        float crackDepth = (_value - 1) * 0.3f;  // �Ѻ����
        float crackFrequency = (_value - 1) * 0.8f;  // �Ѻ۵�Ƶ�ʣ�ֵԽ�ߣ��Ѻ�Խ��

        for (int i = 0; i < vertices.Length; i++)
        {
            // ��ȡ��ǰ����ķ��߷�����Ϊ���ݷ���
            Vector3 normal = vertices[i].normalized;

            // ʹ�� Perlin �������������Ѻ۵�����ԣ������Ѻ۵ķֲ�
            float noiseValue = Mathf.PerlinNoise(vertices[i].x * crackFrequency, vertices[i].z * crackFrequency);

            // �������ֵ����ĳ����ֵ����ʾ�˴������Ѻ�
            if (noiseValue < 0.4f)  // ������ֵ�Կ����Ѻ۵�����
            {
                // �����Ѻ���ȣ��������ط��߷�����������
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
