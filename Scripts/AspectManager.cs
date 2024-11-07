// AspectManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AspectManager
{
    private List<IAspect> aspects = new List<IAspect>();

    public void AddAspect(IAspect aspect)
    {
        aspects.Add(aspect);
    }

    public void ClearAspects()
    {
        aspects.Clear();
    }

    public void ApplyMeshEffects(ref Vector3[] vertices, float radius)
    {
        // Sort aspects by priority
        var activeAspects = aspects.Where(a => a.Priority > 1).OrderByDescending(a => a.Priority);

        foreach (var aspect in activeAspects)
        {
            aspect.ApplyMeshEffect(ref vertices, radius);
        }
    }

    public Material GetHighestPriorityMaterial()
    {
        var activeAspect = aspects.Where(a => a.Priority > 1).OrderByDescending(a => a.Priority).FirstOrDefault();
        return activeAspect?.GetMaterial();
    }

    public void ApplyRenderingEffects()
    {
        foreach (var aspect in aspects.OrderByDescending(a => a.Priority))
        {
            aspect.ApplyRenderingEffect();
        }
    }

    public void InstantiateEffects(Transform parent)
    {
        foreach (var aspect in aspects)
        {
            if (aspect is FlameAspect flameAspect)
            {
                flameAspect.InstantiateEffects(parent);
            }
            // Handle other aspects with effects
        }
    }

    public void ClearEffects()
    {
        foreach (var aspect in aspects)
        {
            if (aspect is FlameAspect flameAspect)
            {
                flameAspect.ClearEffects();
            }
            // Handle other aspects with effects
        }
    }

}
