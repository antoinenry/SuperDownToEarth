using UnityEngine;
using UnityEditor;

public struct ValueChangeEventComponentEditor
{
    public Component component;
    public ValueChangeEventEditor[] vceEditors;
    public GameObject lastSelectedGameObject;

    public ValueChangeEventComponentEditor(Component comp, GameObject lastSelected)
    {
        Debug.Log("New vceComponent editor");

        if (comp is IValueChangeEventsComponent)
        {
            (comp as IValueChangeEventsComponent).SetValueChangeEventsID();
            component = comp;
            vceEditors = null;
            lastSelectedGameObject = lastSelected;

            if (component != null)
            {
                (component as IValueChangeEventsComponent).GetValueChangeEvents(out ValueChangeEvent[] vces);
                if (vces != null)
                {
                    int vceCount = vces.Length;
                    vceEditors = new ValueChangeEventEditor[vceCount];
                    for (int i = 0; i < vceCount; i++)
                        vceEditors[i] = new ValueChangeEventEditor(vces[i]) { hasSlaveEditor = true };
                }
            }
        }
        else
        {
            component = null;
            vceEditors = null;
            lastSelectedGameObject = null;
        }
    }

    public void EditorGUI()
    {
        if (vceEditors == null) return;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(component.ToString(), EditorStyles.boldLabel);

        if (vceEditors != null && vceEditors.Length > 0)
        {
            foreach (ValueChangeEventEditor vceEditor in vceEditors)
            {
                vceEditor.OnEditorGUILayout();
                if (vceEditor.vceExplorer != null && vceEditor.vceExplorer.selectedGameObject != null)
                    lastSelectedGameObject = vceEditor.vceExplorer.selectedGameObject;
            }                
        }
        else
        {
            EditorGUILayout.HelpBox("No events found.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }
}
