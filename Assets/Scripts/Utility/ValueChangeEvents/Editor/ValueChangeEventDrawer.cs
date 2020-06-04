using UnityEngine;
using UnityEditor;

using VCE;

[CustomPropertyDrawer(typeof(ValueChangeEvent), true)]
public class ValueChangeEventDrawer : PropertyDrawer
{
    private bool changeCheck;
    private ValueChangeEventExplorer masterExplorer;

    private bool highlightProperty;
    private float highlightStartTime;
    const float highlightDuration = 1f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ValueChangeEvent target = ValueChangeEventID.FindValueChangeEvent(property.serializedObject.targetObject as Component, property.name);

        float height = EditorGUIUtility.singleLineHeight;
        if (target.inspectorShowMasters)
        {
            height += (target.MasterCount + 1f) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if (masterExplorer != null) height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
        else
            masterExplorer = null;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ValueChangeEvent target = ValueChangeEventID.FindValueChangeEvent(property.serializedObject.targetObject as Component, property.name);

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

        Rect arrowRect = position;
        arrowRect.height = EditorGUIUtility.singleLineHeight;
        arrowRect.width = position.width - controlRect.width;
        target.inspectorShowMasters = EditorGUI.Foldout(arrowRect, target.inspectorShowMasters, GUIContent.none, true);


        SerializedProperty valueProperty = property.FindPropertyRelative("value");
        if (valueProperty != null)
            ValueGUI(controlRect, valueProperty);
        else
            TriggerGUI(controlRect);

        if (target.inspectorShowMasters)
            MastersGUI(position, target);


        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();                
    }

    private void TriggerGUI(Rect position)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        position.y += EditorGUIUtility.singleLineHeight - position.height;
        changeCheck = GUI.Button(position, "");
    }

    private void ValueGUI(Rect position, SerializedProperty value)
    {
        EditorGUI.BeginChangeCheck();
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, value, GUIContent.none);
        changeCheck = EditorGUI.EndChangeCheck();
    }

    private void MastersGUI(Rect position, ValueChangeEvent target)
    {
        int masterCount = target.MasterCount;

        EditorGUI.indentLevel++;

        Rect lineRect = position;
        lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        lineRect.height = EditorGUIUtility.singleLineHeight;

        Rect masterBoxRect = position;
        masterBoxRect.height += 2f * EditorGUIUtility.standardVerticalSpacing - EditorGUIUtility.singleLineHeight;
        masterBoxRect.y += EditorGUIUtility.singleLineHeight;

        if (target.MasterCount > 0)
        {
            GUI.Box(lineRect, "");
            EditorGUI.LabelField(lineRect, "Follows:");
        }
        else
            EditorGUI.HelpBox(lineRect, "No masters", MessageType.Info);

        EditorGUI.indentLevel++;

        Rect button1Rect = lineRect;
        button1Rect.width = 30f;
        button1Rect.x = lineRect.width - button1Rect.width/2f;
        Rect button2Rect = button1Rect;
        button2Rect.x -= button2Rect.width + 1f;

        lineRect.width -= button1Rect.width + button2Rect.width + 1f;

        if (masterExplorer == null && GUI.Button(button1Rect, "+"))
            InitMasterExplorer(target);
        else if (masterExplorer != null && GUI.Button(button1Rect, "x"))
            masterExplorer = null;

        if (masterCount > 0)
        {
            lineRect.width -= button1Rect.width + 1f;

            for (int i = 0; i < masterCount; i++)
            {
                lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(lineRect, target.GetMaster(i).ToString());

                button1Rect.y = lineRect.y;
                if (GUI.Button(button1Rect, "-")) target.RemoveMasterAt(i);

                button2Rect.y = button1Rect.y;
                if (GUI.Button(button2Rect, "...")) FocusOn(target.GetMaster(i));
            }
        }

        if (masterExplorer != null)
        {
            lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            masterExplorer.ExplorerGUI(lineRect, out bool dirty);

            if (masterExplorer.HasSelection)
            {
                button1Rect.y = lineRect.y;
                if (GUI.Button(button1Rect, "Ok"))
                {
                    target.AddMaster(masterExplorer.SelectedVceID);
                    masterExplorer = null;
                }
            }

            if (dirty) changeCheck = true;
        }

        EditorGUI.indentLevel -= 2;
    }

    private void InitMasterExplorer(ValueChangeEvent target)
    {
        masterExplorer = new ValueChangeEventExplorer();

        int masterCount = target.MasterCount;
        if (target.MasterCount == 0)
            masterExplorer.selectedGameObject = Selection.activeGameObject;
        else
            masterExplorer.SetSelection(target.GetMaster(target.MasterCount - 1));

        masterExplorer.filter = new System.Predicate<ValueChangeEventID>(
            id =>   id.ValueChangeEvent != null
            &&      (target.ValueType == null || id.ValueChangeEvent.ValueType == target.ValueType)
            &&      id.ValueChangeEvent != target);
    }

    private void FocusOn(ValueChangeEventID vceID)
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