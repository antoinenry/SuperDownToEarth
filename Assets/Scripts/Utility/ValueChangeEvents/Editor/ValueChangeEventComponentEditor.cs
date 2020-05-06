using UnityEngine;
using UnityEditor;

public struct ValueChangeEventComponentEditor
{
    public Component component;
    public ValueChangeEventEditor[] vceEditors;

    public ValueChangeEventComponentEditor(Component comp)
    {
        if (comp is IValueChangeEventsComponent)
        {
            component = comp;
            vceEditors = null;

            if (component != null)
            {
                int vceCount = (component as IValueChangeEventsComponent).GetValueChangeEvents(out ValueChangeEvent[] vces);
                string[] vceNames = (component as IValueChangeEventsComponent).GetValueChangeEventsNames();

                if (vces != null)
                {
                    vceEditors = new ValueChangeEventEditor[vceCount];
                    for (int i = 0; i < vceCount; i++)
                        vceEditors[i] = new ValueChangeEventEditor(vces[i], new ValueChangeEventID(component, vceNames[i])) { hasSlaveEditor = true };
                }
            }
        }
        else
        {
            component = null;
            vceEditors = null;
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
                vceEditor.OnEditorGUILayout();
        }
        else
        {
            EditorGUILayout.HelpBox("No events found.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }
}
