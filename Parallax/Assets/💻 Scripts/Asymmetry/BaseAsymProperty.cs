using System;
using UnityEngine;

public abstract class BaseAsymProperty : MonoBehaviour
{
    /// <summary>
    /// Configures this Asymmetrical Game Object to a given role
    /// </summary>
    /// <param name="role"></param>
    public abstract void ApplyPerspectiveProfile(CharacterRole role);
}

public abstract class BaseAsymProperty<T> : BaseAsymProperty where T : Component
{
    protected T AsymComponent;

    private void Awake()
    {
        AsymComponent = GetComponent<T>();
        if (AsymComponent == null)
        {
            throw new NullReferenceException($"'{gameObject.name}' is trying to access a '{typeof(T)}' component, but GameObject has no such component attached");
        }
    }
}