using System;
using UnityEngine;

/// <summary>
/// Class that exposes the common method to apply a perspective
/// </summary>
public abstract class BaseAsymProperty : MonoBehaviour
{
    /// <summary>
    /// Configures this Asymmetrical Game Object to a given profile
    /// </summary>
    /// <param name="profile"></param>
    public abstract void ApplyPerspectiveProfile(PerspectiveProfile profile);
}

public abstract class BaseAsymProperty<T> : BaseAsymProperty where T : Component
{
    protected T AsymComponent;

    private void Awake()
    {
        AsymComponent = GetComponent<T>();
        if (AsymComponent == null)
        {
            throw new NullReferenceException($"'{gameObject.name}' is trying to access a '{typeof(T)}' component, but gameobject has no such component attached");
        }
    }
}