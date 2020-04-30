﻿using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ValueChangeEvent))]
public class ValueChangeEventDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty idProperty = property.FindPropertyRelative("ID");
        
        Component ID_component = idProperty.FindPropertyRelative("component").objectReferenceValue as Component;
        int ID_indexInComponent = idProperty.FindPropertyRelative("indexInComponent").intValue;

        if (ID_component == null)
        {
            EditorGUI.HelpBox(position, "ID not set", MessageType.Error);

            foreach(GameObject selected in Selection.gameObjects)
                ValueChangeEventID.SetAll(selected);
        }
        else
        {
            ValueChangeEvent valueChangeEvent = ValueChangeEventID.GetValueChangeEvent(ID_component, ID_indexInComponent);
            ValueChangeEventEditor.ValueChangeEventGUI(position, valueChangeEvent);
        }        

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
