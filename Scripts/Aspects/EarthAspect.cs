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
            // �����������ߵ�Ƶ�ʺͷ��ȣ����� _value ���ӣ�Ƶ�ʺͷ��ȱ��
            float curveFrequency = (_value - 1) * 2f; // ���ߵ�Ƶ��
            float curveAmplitude = (_value - 1) * 0.5f; // ���ߵķ���

            for (int i = 0; i < vertices.Length; i++)
            {
                // ����ÿ������� y ֵλ�ã���Ϊ���ߵĲο���
                float yPos = vertices[i].y;

                // ʹ�����Һ���ʹ������ x �� z ������ڶ�
                float sinX = Mathf.Sin(yPos * curveFrequency) * curveAmplitude;
                float sinZ = Mathf.Cos(yPos * curveFrequency) * curveAmplitude;

                // �������Ҳ�������������� x �� z λ��
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

