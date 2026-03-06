using UnityEngine;

public enum PerspectiveProfile
{
    Human,
    Cat
}

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
            Debug.LogError($"{gameObject.name} is missing {nameof(T)} component. AsymProperty will be disabled.");
            enabled = false;
        }
    }
}