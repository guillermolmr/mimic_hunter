using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
public class EnumMaskDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.intValue = EditorGUI.MaskField(
            position,
            label,
            property.intValue,
            property.enumDisplayNames
        );
        
    }
}
