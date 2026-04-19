using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InfoBoxAttribute))]
public class InfoBoxDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        InfoBoxAttribute infoBox = (InfoBoxAttribute)attribute;
        int lines = infoBox.Message.Split('\n').Length;
        return EditorGUIUtility.singleLineHeight * lines + 1.2f;
    }

    public override void OnGUI(Rect position)
    {
        InfoBoxAttribute infoBox = (InfoBoxAttribute)attribute;
        EditorGUI.HelpBox(position, infoBox.Message, infoBox.Type);
    }
}