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
            // ����Ӳ��Ч���Ĺ��ɣ����� value ���ӣ������𽥱����ʮ������
            float polyhedronT = (_value - 1) / 4f; // �� value Ϊ 5 ʱ��polyhedronT Ϊ 1���ﵽ��ʮ������Ч��

            // ��ʮ������� 12 �����㷽������
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
                // ��ȡ��ǰ����ķ��߷��򣨼���λ����ķ���������
                Vector3 direction = vertices[i].normalized;

                // �ҵ����뵱ǰ�����������ʮ�����巽������
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

                // �𽥲�ֵ�ö�������ι��ɵ���ʮ��������״
                Vector3 newDirection = Vector3.Lerp(direction, closestDirection, polyhedronT);

                // �������ƶ����µķ��򣬲����ְ뾶����
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
