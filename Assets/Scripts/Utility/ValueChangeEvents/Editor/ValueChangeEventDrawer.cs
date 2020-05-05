using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ValueChangeEvent))]
public class ValueChangeEventDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty idProperty = property.FindPropertyRelative("ID");        
        Component ID_component = idProperty.FindPropertyRelative("component").objectReferenceValue as Component;
        int ID_indexInComponent = idProperty.FindPropertyRelative("indexInComponent").intValue;

        if (ID_component == null)
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.HelpBox(position, "ID not set", MessageType.Error);

            foreach(GameObject selected in Selection.gameObjects)
                ValueChangeEventID.SetAll(selected);
        }
        else
        {

            ValueChangeEventID.GetValueChangeEvent(ID_component, ID_indexInComponent, out ValueChangeEvent valueChangeEvent);
            Draw(position, valueChangeEvent, label);
            EditorUtility.SetDirty(ID_component);
        }        

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();       
    }

    public static void Draw(Rect position, ValueChangeEvent valueChangeEvent, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        Rect rect = position;
        rect.width = position.width * .75f;
        ValueChangeEventEditor.ValueChangeEventGUI(rect, valueChangeEvent);

        rect.x = position.x + position.width * .9f;
        rect.width = position.width * .1f;
        if (GUI.Button(rect, "...")) ValueChangeEventEditorWindow.ShowWindow();
    }
}
