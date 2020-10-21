using UnityEngine;
using UnityEditor;

using Scarblab.VCE;

[CustomEditor(typeof(EventsAnimator))]
public class EventsAnimatorInspector : Editor
{
    private RuntimeAnimatorController animatorController;
    private ValueChangeEventMastersEditor[] mastersEditors;

    public override void OnInspectorGUI()
    {
        EventsAnimator eventsAnimator = target as EventsAnimator;

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(eventsAnimator), typeof(EventsAnimator), false);
        GUI.enabled = true;

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
        EditorGUILayout.LabelField("Animator parameters:");
        resetVCEs = GUILayout.Button("Refresh", GUILayout.Width(60f));
        EditorGUILayout.EndHorizontal();

        int vceCount = eventsAnimator.ParameterCount;
        if (vceCount < 0) resetVCEs = true;
        else if (mastersEditors == null || mastersEditors.Length != vceCount) mastersEditors = new ValueChangeEventMastersEditor[vceCount];

        if (resetVCEs)
        {
            eventsAnimator.Init();
        }
        else
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < vceCount; i++)
                ParameterEventGUILayout(eventsAnimator.GetParameterEvent(i), ref mastersEditors[i]);
            EditorGUI.indentLevel--;

            EditorUtility.SetDirty(target);
        }        
    }

    private void ParameterEventGUILayout(AnimatorParameterEvent target, ref ValueChangeEventMastersEditor mastersEditor)
    {
        bool inspectMasters = (mastersEditor != null);

        EditorGUILayout.BeginHorizontal();
        inspectMasters = EditorGUILayout.Foldout(inspectMasters, target.name, true);
        ParameterEventControlGUILayout(target);
        EditorGUILayout.EndHorizontal();

        if (inspectMasters)
        {
            Rect position = EditorGUILayout.GetControlRect();
            bool changeCheck = false;

            if (mastersEditor == null || mastersEditor.target != target.VCE) mastersEditor = new ValueChangeEventMastersEditor(target.VCE);
            mastersEditor.OnGUI(position, ref changeCheck);

            GUILayout.Space(mastersEditor.GetHeight());
        }
        else if (mastersEditor != null)
            mastersEditor = null;
    }

    private void ParameterEventControlGUILayout(AnimatorParameterEvent target)
    {
        Rect controlRect = EditorGUILayout.GetControlRect();
        EditorGUI.BeginChangeCheck();
        switch (target.Type)
        {
            case AnimatorControllerParameterType.Trigger:
                Rect triggerRect = EditorGUI.IndentedRect(controlRect);
                triggerRect.x--;
                if (GUI.Button(triggerRect, "", EditorStyles.radioButton)) GUI.changed = true;
                break;                
            case AnimatorControllerParameterType.Bool:
                target.VCE.Value = EditorGUI.Toggle(controlRect, GUIContent.none, (bool)target.VCE.Value);                
                break;
            case AnimatorControllerParameterType.Int:
                target.VCE.Value = EditorGUI.IntField(controlRect, GUIContent.none, (int)target.VCE.Value);
                break;
            case AnimatorControllerParameterType.Float:
                target.VCE.Value = EditorGUI.FloatField(controlRect, GUIContent.none, (float)target.VCE.Value);
                break;
        }

        if (EditorGUI.EndChangeCheck()) target.VCE.Trigger();
    }
}
