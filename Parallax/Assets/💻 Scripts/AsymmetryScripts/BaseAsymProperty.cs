using UnityEngine;

public enum PerspectiveProfile
{
    A,
    B
}

public abstract class BaseAsymProperty<T> : MonoBehaviour where T : Component
{
    protected T AsymComponent;

    private void Awake()
    {
        AsymComponent = GetComponent<T>();
        if (AsymComponent == null)
        {
            Debug.LogError($"{gameObject.name} is missing {nameof(T)} component! Disabling script...");
            enabled = false;
        }
    }

    /// <summary>
    /// Configures this Asymmetrical Game Object to a given profile
    /// </summary>
    /// <param name="profile"></param>
    public abstract void ApplyPerspectiveProfile(PerspectiveProfile profile);
}