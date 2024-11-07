using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlameAspect : IAspect
{
    private int _value;
    private Material _material;
    private List<GameObject> _instantiatedEffects = new List<GameObject>();

    // �����ڻ����λ���б�
    private List<Vector3> _meltingPoints = new List<Vector3>();
    private int _numMeltingPoints = 0;
    private float _radius;

    public FlameAspect(int value, Material material, float radius)
    {
        _value = value;
        _material = material;
        _radius = radius;
        InitializeMeltingPoints();
    }

    public int Priority => _value;

    // ��ʼ���ڻ��㣬���� value ȷ������
    private void InitializeMeltingPoints()
    {
        _meltingPoints.Clear();
        _numMeltingPoints = Mathf.Clamp(_value - 1, 0, 4); // ���� value �������4���ڻ���

        for (int i = 0; i < _numMeltingPoints; i++)
        {
            // ��������°�������ϵ��ڻ���
            Vector3 point = RandomPointOnLowerHemisphere();
            _meltingPoints.Add(point);
        }
    }

    // ���°�������������һ����
    private Vector3 RandomPointOnLowerHemisphere()
    {
        float theta = Random.Range(0f, Mathf.PI * 2);
        float phi = Random.Range(Mathf.PI / 2, Mathf.PI); // �°���

        float x = _radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = _radius * Mathf.Cos(phi);
        float z = _radius * Mathf.Sin(phi) * Mathf.Sin(theta);

        return new Vector3(x, y, z).normalized * _radius;
    }

    // �� value �仯ʱ�����³�ʼ���ڻ���
    public void UpdateValue(int newValue)
    {
        if (_value != newValue)
        {
            _value = newValue;
            InitializeMeltingPoints();
        }
    }

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1 && _meltingPoints.Count > 0)
        {
            // �����´���ǿ�ȣ����� value ���ӣ��´�Խ����
            float meltingT = (_value - 1) / 2f; // value 1-5 ��Ӧ T 0-1

            // �����´���������
            float maxDrop = radius * 0.7f * meltingT;

            // ����������Ƶ�ʺͷ���
            float noiseFrequency = 5f * meltingT;
            float noiseAmplitude = 0.5f * meltingT;

            for (int i = 0; i < vertices.Length; i++)
            {
                // ֻ�����°���Ķ���
                if (vertices[i].y < 0)
                {
                    Vector3 vertexWorldPos = vertices[i];

                    // �ҵ�������ڻ���
                    float minDistance = float.MaxValue;
                    Vector3 closestMeltingPoint = Vector3.zero;

                    foreach (var meltingPoint in _meltingPoints)
                    {
                        float distance = Vector3.Distance(vertexWorldPos, meltingPoint);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestMeltingPoint = meltingPoint;
                        }
                    }

                    // ����һ��Ӱ��뾶��ֻ����Ӱ��뾶�ڵĶ���Żᱻ�´�
                    float influenceRadius = radius * 0.4f;

                    if (minDistance < influenceRadius)
                    {
                        // �����´���������Խ�����´�Խ��
                        float dropFactor = 1f - (minDistance / influenceRadius);

                        // ���� dropFactor �� meltingT ����ʵ���´���
                        float dropAmount = dropFactor * maxDrop;

                        // Ӧ���´��� y ��
                        vertices[i].y -= dropAmount;

                        // ���һЩˮƽ������ģ����Ȼ����
                        float noiseX = Mathf.PerlinNoise(vertices[i].x * noiseFrequency, vertices[i].z * noiseFrequency);
                        float noiseZ = Mathf.PerlinNoise(vertices[i].z * noiseFrequency, vertices[i].x * noiseFrequency);

                        vertices[i].x += (noiseX - 0.5f) * noiseAmplitude;
                        vertices[i].z += (noiseZ - 0.5f) * noiseAmplitude;
                    }
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

    // Additional methods for handling effects
    public void InstantiateEffects(Transform parent)
    {
        // Waiting
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
