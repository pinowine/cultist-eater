using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AppleAspect : IAspect
{
    private int _value;
    private Material _material;
    private List<GameObject> _instantiatedEffects = new List<GameObject>();
    public Volume m_volume = GameObject.Find("Global Volume").GetComponent<Volume>();
    private OPVolume m_opVolume;
    //private OPVolume _opVolume;

    public AppleAspect(int value, Material material)
    {
        _value = value;
        _material = material;
        //FindOPVolume();
    }

    //private void FindOPVolume()
    //{
    //    Volume volume = GameObject.FindObjectOfType<Volume>();
    //    if (volume != null && volume.profile.TryGet<OPVolume>(out var opVolume))
    //    {
    //        _opVolume = opVolume;
    //    }
    //}

    public int Priority => _value;

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1)
        {
            // �����°����ƽ����ϵ�������� _value �����ӣ��°����𽥱�ƽ
            float flattenT = (_value - 1) / 4f;

            for (int i = 0; i < vertices.Length; i++)
            {
                // ����������°��� (y < 0)������ y ֵ�� 0 ����
                if (vertices[i].y < 0)
                {
                    // ��ֵ���� y ���ֵ������ 0��ʹ�°벿��������ƽ̹
                    vertices[i].y = Mathf.Lerp(vertices[i].y, 0, flattenT);
                }
            }

            // �������������Ч���������ϰ��򲻱䣬�°����𽥱�ƽ
            float elevationIntensity = (_value - 1) / 4f * 0.5f;
            for (int i = 0; i < vertices.Length; i++)
            {
                // ֻ���ϰ벿�� (y >= 0) �Ķ���Ӧ������Ч��
                if (vertices[i].y >= 0)
                {
                    float noise = Mathf.PerlinNoise(vertices[i].x * 0.5f, vertices[i].z * 0.5f);
                    vertices[i] += vertices[i].normalized * noise * elevationIntensity;
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
        if (_value == 1)
        {
            if (m_volume.profile.TryGet<OPVolume>(out m_opVolume))
            {
                m_opVolume.intensity.Override(0);
                m_opVolume.scatter.Override(0);
            }
        }
        if (_value > 1) 
        {
            if (m_volume.profile.TryGet<OPVolume>(out m_opVolume))
            {
                m_opVolume.intensity.Override((_value - 1) * 3);
                m_opVolume.scatter.Override((_value - 1) * .05f);
            }
        }
    }

    public void InstantiateEffects(Transform parent)
    {
        // Implement any effect instantiation or handling here
    }

    public void ClearEffects()
    {
    }

}
