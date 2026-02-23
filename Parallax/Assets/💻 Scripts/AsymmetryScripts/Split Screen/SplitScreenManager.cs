using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Debug-only manager that simulates split screen by forcing
/// BaseAsymComponent instances to apply a perspective locally.
/// Does NOT affect runtime architecture.
/// </summary>
///
public class SplitScreenManager : MonoBehaviour
{
    private List<MonoBehaviour> _asymComponents = new();
    private Dictionary<System.Type, MethodInfo> _applyMethodCache = new();

    private void Awake()
    {
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
    }

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