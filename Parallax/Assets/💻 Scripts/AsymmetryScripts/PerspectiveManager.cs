using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PerspectiveManager : MonoBehaviour
{
    public PerspectiveProfile Profile;

    private List<MonoBehaviour> _asymComponents = new();
    private readonly Dictionary<System.Type, MethodInfo> _applyMethodCache = new();

    private void Start()
    {
        // Find all asymmetrical objects in the scene, and store them
        _asymComponents = new();
        var all = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var mb in all)
        {
            var type = mb.GetType();

            if (IsDerivedFromGenericBase(type, typeof(BaseAsymProperty<>)))
            {
                _asymComponents.Add(mb);
            }
        }
        Debug.Log($"SplitScreenManager found {_asymComponents.Count} asym components.");

        ApplyPerspective(Profile);
    }

    /// <summary>
    /// Helper function used to find asymmetric objects
    /// </summary>
    private bool IsDerivedFromGenericBase(System.Type type, System.Type genericBase)
    {
        while (type != null && type != typeof(MonoBehaviour))
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == genericBase)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Applies all perspective settings for a profile
    /// </summary>
    public void ApplyPerspective(PerspectiveProfile profile)
    {
        foreach (var asymComponent in _asymComponents)
        {
            var type = asymComponent.GetType();

            // Cache the method for performance
            if (!_applyMethodCache.TryGetValue(type, out var method))
            {
                method = type.GetMethod("ApplyPerspectiveProfile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method == null)
                {
                    Debug.LogWarning($"ApplyPerspectiveProfile not found on {type}");
                    continue;
                }

                _applyMethodCache[type] = method;
            }

            method.Invoke(asymComponent, new object[] { profile });
        }
    }
}