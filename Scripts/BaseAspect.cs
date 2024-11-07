using UnityEngine.Rendering.Universal;
using UnityEngine;

public abstract class BaseAspect : IAspect
{
    public abstract int Priority { get; }
    public abstract void ApplyMeshEffect(ref Vector3[] vertices, float radius);
    public abstract Material GetMaterial();
    public abstract void ApplyRenderingEffect();
    public abstract void InstantiateEffects(Transform parent);
}
