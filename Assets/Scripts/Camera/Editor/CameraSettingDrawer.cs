using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public static class CameraSettingDrawer
{
    public static void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty actionProperty = property.FindPropertyRelative("action");
        int action = actionProperty.enumValueIndex;

        Rect actionRect = new Rect(position.x, position.y, 100, position.height);
        EditorGUI.PropertyField(actionRect, actionProperty, GUIContent.none);
               
        if (action == (int)CameraSettings.ActionType.UseValue)
        {
            Rect valueRect = new Rect(position.x + 105, position.y, position.width - 105, position.height);
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(CameraSettings.FloatSetting))]
public class CameraFloatSettingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CameraSettingDrawer.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(CameraSettings.Vector2Setting))]
public class CameraVector2SettingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CameraSettingDrawer.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(CameraSettings.TransformSetting))]
public class CameraTransformSettingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CameraSettingDrawer.OnGUI(position, property, label);
    }
}