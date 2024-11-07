using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WinterAspect : IAspect
{
    private int _value;
    private Material _material;
    private List<GameObject> _instantiatedEffects = new List<GameObject>();

    public WinterAspect(int value, Material material)
    {
        _value = value;
        _material = material;
    }

    public int Priority => _value;

    public void ApplyMeshEffect(ref Vector3[] vertices, float radius)
    {
        if (_value > 1)
        {
            float cubeT = (_value - 1) / 4f;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i].normalized;
                v = new Vector3(
                    Mathf.Lerp(v.x, Mathf.Sign(v.x) * 1, cubeT),
                    Mathf.Lerp(v.y, Mathf.Sign(v.y) * 1, cubeT),
                    Mathf.Lerp(v.z, Mathf.Sign(v.z) * 1, cubeT)
                );
                vertices[i] = v * radius;
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
