using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(NamedAttributeElements))]
public class NamedElementsPropertyEditor : PropertyDrawer
{
    NamedAttributeElements namedAttribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.PropertyField(position, property, new GUIContent(namedAttribute.names[pos]));
        }
        catch
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
