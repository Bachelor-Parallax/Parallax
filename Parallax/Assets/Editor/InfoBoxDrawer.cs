using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InfoBoxAttribute))]
public class InfoBoxDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        InfoBoxAttribute infoBox = (InfoBoxAttribute)attribute;

        return EditorStyles.helpBox.CalcHeight(
            new GUIContent(infoBox.Message),
            EditorGUIUtility.currentViewWidth
        ) + 10f;
    }

    public override void OnGUI(Rect position)
    {
        InfoBoxAttribute infoBox = (InfoBoxAttribute)attribute;
        EditorGUI.HelpBox(position, infoBox.Message, infoBox.Type);
    }
}