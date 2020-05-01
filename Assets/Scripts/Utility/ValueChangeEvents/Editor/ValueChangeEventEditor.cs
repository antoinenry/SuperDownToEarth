using UnityEngine;
using UnityEditor;

public class ValueChangeEventEditor
{
    public bool hasSlaveEditor;
    public bool showMasters;
    public bool detailedLabel;
    public GameObject defaultGameObject;

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
            valueChangeEvent.Enslave(true);       

            if (hasSlaveEditor) SlaveGUILayout();
        }      
    }

    private void EditorHeaderGUI(Rect rect)
    {
        if (valueChangeEvent == null) return;

        string label ;
        if (detailedLabel && valueChangeEvent.Component != null)
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
                if (defaultGameObject != null)
                    vceExplorer.selectedGameObject = defaultGameObject;
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
                    defaultGameObject = vceExplorer.selectedGameObject;
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

        IValueChangeEvent vce = vceTarget.runtimeEvent;
        if (vce == null)
        {
            EditorGUI.HelpBox(rect, vceTarget.Name + " runtime event is null.", MessageType.Error);
            return;
        }


        if (vce is TriggerEvent) ValueChangeEventGUI(rect, vce as TriggerEvent);
        else if (vce is ValueChangeEvent<bool>) ValueChangeEventGUI(rect, vce as ValueChangeEvent<bool>);
        else if (vce is ValueChangeEvent<int>) ValueChangeEventGUI(rect, vce as ValueChangeEvent<int>);
        else if (vce is ValueChangeEvent<float>) ValueChangeEventGUI(rect, vce as ValueChangeEvent<float>);
        else if (vce is ValueChangeEvent<Vector2>) ValueChangeEventGUI(rect, vce as ValueChangeEvent<Vector2>);
        else if (vce is ValueChangeEvent<GameObject>) ValueChangeEventGUI(rect, vce as ValueChangeEvent<GameObject>);

        else EditorGUILayout.HelpBox("Inspector for ValueChangeEvent<" + vce.GetValueType().Name + "> is not implemented", MessageType.Warning);
    }

    private static void ValueChangeEventGUI(Rect rect, TriggerEvent vce)
    {
        Rect buttonRect = rect;
        rect.width = 20f;
        if (GUI.Button(rect, " ")) vce.ForceInvoke();
    }

    private static void ValueChangeEventGUI(Rect rect, ValueChangeEvent<bool> vce)
    {
        vce.Value = EditorGUI.Toggle(rect, vce.Value);
    }

    private static void ValueChangeEventGUI(Rect rect, ValueChangeEvent<int> vce)
    {
        vce.Value = EditorGUI.IntField(rect, vce.Value);
    }

    private static void ValueChangeEventGUI(Rect rect, ValueChangeEvent<float> vce)
    {
        vce.Value = EditorGUI.FloatField(rect, vce.Value);
    }

    private static void ValueChangeEventGUI(Rect rect, ValueChangeEvent<Vector2> vce)
    {
        vce.Value = EditorGUI.Vector2Field(rect, "", vce.Value);
    }

    private static void ValueChangeEventGUI(Rect rect, ValueChangeEvent<GameObject> vce)
    {
        vce.Value = EditorGUI.ObjectField(rect, vce.Value, typeof(GameObject), true) as GameObject;
    }
}
