using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventsAnimator))]
public class EventsAnimatorInspector : Editor
{
    /*
    RuntimeAnimatorController animatorController;

    public override void OnInspectorGUI()
    {
        EventsAnimator eventsAnimator = target as EventsAnimator;
        Animator animator = eventsAnimator.GetComponent<Animator>();
        RuntimeAnimatorController currentAC = animator.runtimeAnimatorController;
        bool resetVCEs;

        if (currentAC != animatorController)
        {
            animatorController = currentAC;
            resetVCEs = true;
        }

        if (animatorController == null)
        {
            EditorGUILayout.HelpBox("No animatorController", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginHorizontal();        

        if (GUILayout.Button("...", GUILayout.Width(30f))) ValueChangeEventEditorWindow.ShowWindow();
        resetVCEs = GUILayout.Button("Refresh", GUILayout.Width(60f));

        EditorGUILayout.EndHorizontal();
        
        

        if (resetVCEs) eventsAnimator.SetValueChangeEventsFromAnimator(animator);

        int vceCount = eventsAnimator.GetValueChangeEvents(out ValueChangeEvent[] vces);
        string[] vceNames = eventsAnimator.GetValueChangeEventsNames();

        EditorGUILayout.BeginVertical("box");
        for (int i = 0; i < vceCount; i++)
        {
            ValueChangeEventEditor vceEditor = new ValueChangeEventEditor(vces[i], new ValueChangeEventID(eventsAnimator, vceNames[i]));
            vceEditor.OnEditorGUILayout();
        }
        EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(target);
    }
    */
}
