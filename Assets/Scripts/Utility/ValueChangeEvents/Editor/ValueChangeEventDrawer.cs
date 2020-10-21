using UnityEngine;
using UnityEditor;

using Scarblab.VCE;

[CustomPropertyDrawer(typeof(ValueChangeEvent), true)]
public class ValueChangeEventDrawer : PropertyDrawer
{
    private bool changeCheck;
    private ValueChangeEventMastersEditor mastersEditor;

    private bool highlightProperty;
    private float highlightStartTime;
    const float highlightDuration = 1f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ValueChangeEvent target = ValueChangeEventID.FindValueChangeEvent(property.serializedObject.targetObject as Component, property.name);        

        float height = EditorGUIUtility.singleLineHeight;
        if (target != null && mastersEditor != null) height += mastersEditor.GetHeight() + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ValueChangeEvent target = ValueChangeEventID.FindValueChangeEvent(property.serializedObject.targetObject as Component, property.name);

        if (target == null)
        {
            EditorGUI.LabelField(position, "(" + property.type + ")");
            return;
        }

        if (changeCheck == true)
        {
            changeCheck = false;
            target.Trigger();            
            EditorUtility.SetDirty(property.serializedObject.targetObject as Component);
        }

        if (target.inspectorHighlight)
        {
            highlightProperty = true;
            highlightStartTime = Time.realtimeSinceStartup;
            target.inspectorHighlight = false;
        }

        if (highlightProperty == true) HighlightFade(position, property);
        
        Rect controlRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.BeginProperty(position, label, property);        

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;           

        SerializedProperty valueProperty = property.FindPropertyRelative("value");
        SerializedProperty enumNamesProperty = property.FindPropertyRelative("enumNames");
        if (valueProperty != null)
            ValueGUI(controlRect, valueProperty, enumNamesProperty);
        else
            TriggerGUI(controlRect);

        Rect arrowRect = position;
        arrowRect.height = EditorGUIUtility.singleLineHeight;
        arrowRect.width = position.width - controlRect.width;
        if (target.MasterCount == 0)
            target.inspectorUnfold = EditorGUI.Foldout(arrowRect, target.inspectorUnfold, GUIContent.none, true, GUIStyle.none);
        else
            target.inspectorUnfold = EditorGUI.Foldout(arrowRect, target.inspectorUnfold, GUIContent.none, true);

        if (target.inspectorUnfold)
        {
            if (mastersEditor == null || mastersEditor.target != target) mastersEditor = new ValueChangeEventMastersEditor(target);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            mastersEditor.OnGUI(position, ref changeCheck);
        }
        else if (mastersEditor != null)
            mastersEditor = null;

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();                
    }

    private void TriggerGUI(Rect position)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        position.y += EditorGUIUtility.singleLineHeight - position.height;
        changeCheck = GUI.Button(position, "", EditorStyles.radioButton);
    }

    private void ValueGUI(Rect position, SerializedProperty value, SerializedProperty enumNames)
    {
        EditorGUI.BeginChangeCheck();
        position.height = EditorGUIUtility.singleLineHeight;
        if (enumNames == null)
        {
            EditorGUI.PropertyField(position, value, GUIContent.none);
        }
        else
        {
            if (enumNames.isArray)
            {
                int enumNamesCount = enumNames.arraySize;
                string[] enumNamesArray = new string[enumNamesCount];
                for (int i = 0; i < enumNamesCount; i++)
                    enumNamesArray[i] = enumNames.GetArrayElementAtIndex(i).stringValue;
                value.intValue = EditorGUI.Popup(position, value.intValue, enumNamesArray);
            }
            else
                EditorGUI.HelpBox(position, "enumNames property must be of type string[]", MessageType.Error);
        }
        changeCheck = EditorGUI.EndChangeCheck();
    }    

    public static void FocusOn(ValueChangeEventID vceID)
    {
        ValueChangeEvent vce = vceID.ValueChangeEvent;

        if (vce != null)
        {
            Selection.activeGameObject = vceID.Component.gameObject;
            vce.inspectorHighlight = true;
        }
    }

    private void HighlightFade(Rect position, SerializedProperty property)
    {
        float timer = Time.realtimeSinceStartup - highlightStartTime;
        if (timer < highlightDuration)
        {
            Color normalBackColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.Lerp(Color.yellow, Color.clear, timer / highlightDuration);
            GUI.Box(position, GUIContent.none);
            GUI.backgroundColor = normalBackColor;
            EditorUtility.SetDirty(property.serializedObject.targetObject as Component);
        }
        else
            highlightProperty = false;
    }
}