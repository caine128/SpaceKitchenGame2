using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(DrawIfAttribute),useForChildren:true)]
public class DrawIfPropertyEditor : PropertyDrawer
{
    #region fields
    DrawIfAttribute drawIf;
    SerializedProperty comparedField;
    #endregion

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DontDraw)
            return 0f;

        if (property.isExpanded)
        {

            return EditorGUIUtility.singleLineHeight * (property.CountInProperty()+ EditorGUIUtility.standardVerticalSpacing/2);
        }


         return base.GetPropertyHeight(property, label);
    }
  
    private bool ShowMe(SerializedProperty property)
    {
        drawIf = attribute as DrawIfAttribute;

        //int position = property.propertyPath.IndexOf("data");
        //string path = property.propertyPath.Remove(position + 8);
        //path = path.Insert(position + 8, drawIf.comparedPropertyName);
        string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;
        
        comparedField = property.serializedObject.FindProperty(path);

        if (comparedField == null)
        {
            Debug.LogError("Cannot find property with name: " + path);
            return true;
        }

        switch (comparedField.type)
        {
            case "bool":
                return comparedField.boolValue.Equals(drawIf.comparedValue);
            case "Enum":
                return comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
            default:
                Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                return true;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.BeginProperty(position, label, property);
        //Rect rectFoldOut = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        //property.isExpanded = EditorGUI.Foldout(rectFoldOut, property.isExpanded, label);
        //int lines = 1;
        //if (property.isExpanded)
        //{
        //    property.
        //}


        if (ShowMe(property))
        {
            EditorGUI.PropertyField(position, property,label,true);    
        }


        
        else if (drawIf.disablingType == DrawIfAttribute.DisablingType.ReadOnly)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }


}
