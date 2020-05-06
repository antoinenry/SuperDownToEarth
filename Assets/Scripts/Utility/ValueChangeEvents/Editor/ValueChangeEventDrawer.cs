using System;

using UnityEngine;
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
        
        UnityEngine.Object component = property.serializedObject.targetObject;

        if(component is IValueChangeEventsComponent)
        {
            ValueChangeEvent vce = (component as IValueChangeEventsComponent).GetValueChangeEventByName(property.name);
            if (vce != null)
            {
                Draw(position, vce, label);
                EditorUtility.SetDirty(component);
            }
            else
            {
                EditorGUI.HelpBox(position, "Error - no vce found (" + property.name + ")", MessageType.Error);
            }
        }
        else
        {
            EditorGUI.HelpBox(position, component.name + " doesn't implement IValueChangeEventsComponent.", MessageType.Error);
        }   

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();       
    }

    public static void Draw(Rect position, ValueChangeEvent valueChangeEvent, GUIContent label)
    {
        Rect rect = position;
        rect.width = position.width * .75f;
        ValueChangeEventEditor.ValueChangeEventGUI(rect, valueChangeEvent);

        rect.x = position.x + position.width * .9f;
        rect.width = position.width * .1f;
        if (GUI.Button(rect, "...")) ValueChangeEventEditorWindow.ShowWindow();
    }
}
