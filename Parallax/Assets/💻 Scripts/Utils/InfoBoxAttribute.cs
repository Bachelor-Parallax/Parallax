using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class InfoBoxAttribute : PropertyAttribute
{
    public readonly string Message;
    public readonly MessageType Type;

    public InfoBoxAttribute(string message, MessageType type = MessageType.Info)
    {
        Message = message;
        Type = type;
    }
}

#endif