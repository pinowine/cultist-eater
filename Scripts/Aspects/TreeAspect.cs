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
            // cylindricalT ����Բ�����ĳ̶ȣ����� _value ���ӣ���״���ӽ�Բ����
            float cylindricalT = (_value - 1) / 4f;

            for (int i = 0; i < vertices.Length; i++)
            {
                // ���� x �� z �ļ�����뾶����Ӱ�� x �� z ��ķֲ���
                float distance = Mathf.Sqrt(vertices[i].x * vertices[i].x + vertices[i].z * vertices[i].z);

                // ��� distance Ϊ0����ζ�Ŷ��������ģ�����Ҫ����
                if (distance > 0)
                {
                    // ���㵥λ����������Բ������
                    float normalizedX = vertices[i].x / distance;
                    float normalizedZ = vertices[i].z / distance;

                    // �� x �� z ����Բ�����棬���� y ֵ����
                    float newX = Mathf.Lerp(vertices[i].x, normalizedX * radius, cylindricalT);
                    float newZ = Mathf.Lerp(vertices[i].z, normalizedZ * radius, cylindricalT);

                    // ���� y ֵ���䣬������� x �� z ���ϵ���
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
