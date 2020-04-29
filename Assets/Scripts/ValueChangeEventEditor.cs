using UnityEngine;
using UnityEditor;

public class ValueChangeEventEditor
{
    public bool hasSlaveEditor;
    public bool showMasters;
    public bool detailedLabel;

    private ValueChangeEvent valueChangeEvent;
    private ValueChangeEventExplorer vceExplorer;

    public ValueChangeEventEditor(ValueChangeEvent vce)
    {
        valueChangeEvent = vce;
        showMasters = false;
        vceExplorer = null;
    }

    public void EditorGUI()
    {
        if (valueChangeEvent == null)
        {
            EditorGUILayout.HelpBox("ValueChangeEvent is null", MessageType.Info);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            if (hasSlaveEditor)
            {
                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                style.fixedWidth = 1f;
                showMasters = EditorGUILayout.Foldout(showMasters, "", style);
            }

            HeaderGUI();
            valueChangeEvent.Enslave();           

            EditorGUILayout.EndHorizontal();

            if (hasSlaveEditor) SlaveGUI();
        }      
    }

    private void HeaderGUI()
    {
        if (valueChangeEvent == null) return;

        string label ;
        if (detailedLabel && valueChangeEvent.Component != null)
            label = valueChangeEvent.Component.ToString() + "." + valueChangeEvent.Name;
        else
            label = valueChangeEvent.Name;
        EditorGUILayout.LabelField(label);

        ValueChangeEventGUI(valueChangeEvent, 150f);        

        int masterCount = valueChangeEvent.MasterCount;
        if (valueChangeEvent.RuntimeMasterCount != masterCount)
            EditorGUILayout.HelpBox(valueChangeEvent.RuntimeMasterCount + "/" + masterCount + " master runtime count mismatch", MessageType.Error);
        else if (masterCount > 0)
            EditorGUILayout.LabelField(" (" + masterCount + " masters)", GUILayout.Width(80f));
        else
            EditorGUILayout.LabelField(" ", GUILayout.Width(80f));
    }

    private void SlaveGUI()
    {
        if (showMasters)
        {
            int masterCount = valueChangeEvent.MasterCount;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("is slave to: ");
            if (vceExplorer == null && GUILayout.Button("+", GUILayout.Width(25f)))
            {
                vceExplorer = new ValueChangeEventExplorer(vce => vce.ValueType == valueChangeEvent.ValueType && vce != valueChangeEvent);

                ValueChangeEvent lastMaster = valueChangeEvent.GetMaster(masterCount - 1);
                if (lastMaster != null && lastMaster.Component != null)
                    vceExplorer.selectedGameObject = lastMaster.Component.gameObject;
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
                    slaveEditor.EditorGUI();
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

    private static void ValueChangeEventGUI(ValueChangeEvent valueChangeEvent, float width = -1f)
    {
        if (valueChangeEvent.runtimeEvent == null) EditorGUILayout.HelpBox(valueChangeEvent.Name + " runtime event is null.", MessageType.Error);

        IValueChangeEvent vce = valueChangeEvent.runtimeEvent;

        if (vce is TriggerEvent) ValueChangeEventGUI(vce as TriggerEvent, width);
        else if (vce is ValueChangeEvent<bool>) ValueChangeEventGUI(vce as ValueChangeEvent<bool>, width);
        else if (vce is ValueChangeEvent<int>) ValueChangeEventGUI(vce as ValueChangeEvent<int>, width);
        else if (vce is ValueChangeEvent<float>) ValueChangeEventGUI(vce as ValueChangeEvent<float>, width);
        else if (vce is ValueChangeEvent<Vector2>) ValueChangeEventGUI(vce as ValueChangeEvent<Vector2>, width);
        else if (vce is ValueChangeEvent<GameObject>) ValueChangeEventGUI(vce as ValueChangeEvent<GameObject>, width);

        else EditorGUILayout.HelpBox("Inspector for ValueChangeEvent<" + vce.GetValueType().Name + "> is not implemented", MessageType.Warning);
    }

    private static void ValueChangeEventGUI(TriggerEvent vce, float width = -1f)
    {
        bool triggered = EditorGUILayout.Toggle(vce.triggered, GUILayout.Width(width));
        if (triggered == true && vce.triggered == false)
            vce.Trigger();
    }

    private static void ValueChangeEventGUI(ValueChangeEvent<bool> vce, float width = -1f)
    {
        vce.Value = EditorGUILayout.Toggle(vce.Value, GUILayout.Width(width));
    }

    private static void ValueChangeEventGUI(ValueChangeEvent<int> vce, float width = -1f)
    {
        vce.Value = EditorGUILayout.IntField(vce.Value, GUILayout.Width(width));
    }

    private static void ValueChangeEventGUI(ValueChangeEvent<float> vce, float width = -1f)
    {
        vce.Value = EditorGUILayout.FloatField(vce.Value, GUILayout.Width(width));
    }

    private static void ValueChangeEventGUI(ValueChangeEvent<Vector2> vce, float width = -1f)
    {
        float xInput = EditorGUILayout.FloatField(vce.Value.x, GUILayout.Width(width / 2f));
        float yInput = EditorGUILayout.FloatField(vce.Value.y, GUILayout.Width(width / 2f));
        vce.Value = new Vector2(xInput, yInput);
    }

    private static void ValueChangeEventGUI(ValueChangeEvent<GameObject> vce, float width = -1f)
    {
        vce.Value = EditorGUILayout.ObjectField(vce.Value, typeof(GameObject), true, GUILayout.Width(width)) as GameObject;
    }
}
