using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventsAnimator))]
public class EventsAnimatorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        (target as IValueChangeEventsComponent).GetValueChangeEvents(out ValueChangeEvent[] vces);

        foreach(ValueChangeEvent vce in vces)
        {
            Rect position = EditorGUILayout.GetControlRect();
            GUIContent labelContent = new GUIContent(vce.ToString());
            ValueChangeEventDrawer.Draw(position, vce, labelContent);
        }

        EditorUtility.SetDirty(target);
    }
}
