using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains extensions to libraries and APIs
/// </summary>
public static class UnityExtensions
{
    /// <summary>
    /// Method that searches the MonoBehaviours in the scene and finds
    /// all objects assignable to a given type.<br></br>
    /// Functionally, an expanded and more flexible version of Unity's builtin FindObjectsByType
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sortMode"></param>
    /// <returns></returns>
    public static List<T> FindObjectsAssignableTo<T>(FindObjectsSortMode sortMode) where T : class
    {
        // search all MonoBehaviours
        var behaviours = Object.FindObjectsByType<MonoBehaviour>(sortMode);
        List<T> result = new();
        foreach (var behaviour in behaviours)
        {
            if (behaviour is T r)
            {
                result.Add(r);
                break;
            }
        }

        return result;
    }
}