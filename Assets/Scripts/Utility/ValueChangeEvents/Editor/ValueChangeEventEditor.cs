using UnityEngine;
using UnityEditor;

public class ValueChangeEventEditor
{
    public bool hasSlaveEditor;
    public bool detailedLabel;
    public bool showMasters;

    public ValueChangeEvent valueChangeEvent;
    public ValueChangeEventExplorer vceExplorer;

    public ValueChangeEventEditor(ValueChangeEvent vce)
    {
        valueChangeEvent = vce;
        showMasters = false;
        vceExplorer = null;
    }

    public void OnEditorGUILayout()
    {
        OnEditorGUI(EditorGUILayout.GetControlRect());
    }

    public void OnEditorGUI(Rect rect)
    {
        Rect foldOutRect = rect;
        foldOutRect.width = 20f;

        Rect headerRect = rect;
        headerRect.x += foldOutRect.width;
        headerRect.width -= foldOutRect.width;
        
        if (valueChangeEvent == null)
        {
            EditorGUI.HelpBox(rect, "ValueChangeEvent is null", MessageType.Info);
        }
        else
        {
            if (hasSlaveEditor)
            {
                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                style.fixedWidth = 1f;
                showMasters = EditorGUI.Foldout(foldOutRect, showMasters, "", style);
            }

            EditorHeaderGUI(headerRect);
            //valueChangeEvent.Enslave(true);       

            if (hasSlaveEditor) SlaveGUILayout();
        }      
    }

    private void EditorHeaderGUI(Rect rect)
    {
        if (valueChangeEvent == null)
        {
            EditorGUILayout.HelpBox("Event is null.", MessageType.Warning);
            return;
        }

        if (valueChangeEvent.Component == null)
        {
            EditorGUILayout.HelpBox("Event's ID is corrupted.", MessageType.Warning);
            return;
        }

        string label ;
        if (detailedLabel)
            label = valueChangeEvent.Component.ToString() + "." + valueChangeEvent.Name;
        else
            label = valueChangeEvent.Name;

        Rect labelRect = rect;
        labelRect.width *= .6f;
        Rect vceRect = rect;
        vceRect.x += labelRect.width;
        vceRect.width *= .2f;
        Rect masterCountRect = rect;
        masterCountRect.x += labelRect.width + vceRect.width;
        masterCountRect.width *= .2f;

        EditorGUI.LabelField(labelRect, label);

        ValueChangeEventGUI(vceRect, valueChangeEvent);        

        int masterCount = valueChangeEvent.MasterCount;
        if (valueChangeEvent.RuntimeMasterCount != masterCount)
            EditorGUI.HelpBox(masterCountRect, valueChangeEvent.RuntimeMasterCount + "/" + masterCount + " master runtime count mismatch", MessageType.Error);
        else if (masterCount > 0)
            EditorGUI.LabelField(masterCountRect, " (" + masterCount + " masters)");
        else
            EditorGUI.LabelField(masterCountRect, " ");
    }

    private void SlaveGUILayout()
    {
        if (showMasters)
        {
            int masterCount = valueChangeEvent.MasterCount;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("is slave to: ");
            if (vceExplorer == null && GUILayout.Button("+", GUILayout.Width(25f)))
            {
                vceExplorer = new ValueChangeEventExplorer(vce => vce.ValueType == valueChangeEvent.ValueType && vce != valueChangeEvent);
            }
            else if (vceExplorer != null && GUILayout.Button("Cancel"))
                vceExplorer = null;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");
            if (masterCount == 0 && vceExplorer == null)
                EditorGUILayout.HelpBox("This event has no master", MessageType.None);
            else
            {
                for (int i = 0; i < masterCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    ValueChangeEventEditor slaveEditor = new ValueChangeEventEditor(valueChangeEvent.GetMaster(i)) { detailedLabel = true };
                    slaveEditor.OnEditorGUILayout();
                    if (GUILayout.Button("X", GUILayout.Width(25f)))
                        valueChangeEvent.RemoveMasterAt(i);
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (vceExplorer != null)
            {
                EditorGUILayout.BeginHorizontal();

                vceExplorer.EditorGUI();
                GUI.enabled = (vceExplorer.SelectedVce != null);

                if (GUILayout.Button("Confirm"))
                {
                    valueChangeEvent.AddMaster(vceExplorer.SelectedVce);
                    vceExplorer = null;
                }

                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        else
            vceExplorer = null;
    }

    public static void ValueChangeEventGUI(Rect rect, ValueChangeEvent vceTarget)
    {
        if (vceTarget == null)
        {
            EditorGUI.HelpBox(rect, "ValueChangeEvent is null.", MessageType.Error);
            return;
        }
        
        if (vceTarget.IsValueType<trigger>()) TriggerEventGUI(rect, vceTarget);
        else if (vceTarget.IsValueType<bool>()) BoolEventGUI(rect, vceTarget);
        else if (vceTarget.IsValueType<int>()) IntEventGUI(rect, vceTarget);
        else if (vceTarget.IsValueType<float>()) FloatEventGUI(rect, vceTarget);
        else if (vceTarget.IsValueType<Vector2>()) Vector2EventGUI(rect, vceTarget);
        else if (vceTarget.IsValueType<GameObject>()) GameObjectEventGUI(rect, vceTarget);

        else EditorGUILayout.HelpBox("Inspector for ValueChangeEvent<" + vceTarget.ValueType.Name + "> is not implemented", MessageType.Warning);
    }

    private static void TriggerEventGUI(Rect rect, ValueChangeEvent vce)
    {
        Rect buttonRect = rect;
        rect.width = 20f;
        if (GUI.Button(rect, " ")) vce.Invoke();
    }

    private static void BoolEventGUI(Rect rect, ValueChangeEvent vce)
    {
        vce.Set(EditorGUI.Toggle(rect, vce.Get<bool>()));
    }

    private static void IntEventGUI(Rect rect, ValueChangeEvent vce)
    {
        vce.Set(EditorGUI.IntField(rect, vce.Get<int>()));
    }

    private static void FloatEventGUI(Rect rect, ValueChangeEvent vce)
    {
        vce.Set(EditorGUI.FloatField(rect, vce.Get<float>()));
    }

    private static void Vector2EventGUI(Rect rect, ValueChangeEvent vce)
    {
        vce.Set(EditorGUI.Vector2Field(rect, "", vce.Get<Vector2>()));
    }

    private static void GameObjectEventGUI(Rect rect, ValueChangeEvent vce)
    {
        vce.Set(EditorGUI.ObjectField(rect, vce.Get<GameObject>(), typeof(GameObject), true) as GameObject);
    }
}
