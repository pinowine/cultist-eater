using UnityEngine.Rendering.Universal;
using UnityEngine;

public interface IAspect
{
    int Priority { get; } // Determines the order of application based on parameter value
    void ApplyMeshEffect(ref Vector3[] vertices, float radius);
    Material GetMaterial();
    void ApplyRenderingEffect();
    void InstantiateEffects(Transform parent);
}
