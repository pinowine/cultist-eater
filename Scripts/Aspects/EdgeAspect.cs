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
            // �̵�ǿ�����ӣ����������Ի������Ч��
            float spikeIntensity = (_value - 1) * 1f;

            for (int i = 0; i < vertices.Length; i++)
            {
                // ��ȡ������ָ�򶥵�ķ�������
                Vector3 direction = vertices[i].normalized;

                // ʹ�� 3D ����������������ֵ
                float noise = Mathf.PerlinNoise(direction.x * _value, direction.y * _value);
                noise += Mathf.PerlinNoise(direction.y * _value, direction.z * _value);
                noise += Mathf.PerlinNoise(direction.z * _value, direction.x * _value);
                noise /= 3f; // ȡƽ��ֵ

                // ������ֵ����Ϊ -1 �� 1 �ķ�Χ
                noise = (noise - 0.5f) * 2f;

                // ���㶥���λ����
                float displacement = noise * spikeIntensity;

                // �������ط��������ƶ�
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
