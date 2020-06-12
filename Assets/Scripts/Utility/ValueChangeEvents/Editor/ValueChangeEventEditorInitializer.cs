using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Scarblab.VCE
{
    [InitializeOnLoad]
    public class ValueChangeEventEditorInitializer
    {
        private static Scene activeScene;

        static ValueChangeEventEditorInitializer()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChange;
            EditorApplication.hierarchyChanged += OnHierarchyChange;
        }

        private static void OnPlayModeChange(PlayModeStateChange mode)
        {
            if (mode == PlayModeStateChange.EnteredEditMode) InitializeAllValueChangeEvents();
        }

        private static void OnHierarchyChange()
        {
            Scene getScene = SceneManager.GetActiveScene();

            if (getScene != activeScene)
            {
                InitializeAllValueChangeEvents();
                activeScene = getScene;
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnReloadScripts()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode == false)
                InitializeAllValueChangeEvents();
        }

        private static void InitializeAllValueChangeEvents()
        {
            Component[] allComponent = Object.FindObjectsOfType<Component>();
            foreach (Component anyComponent in allComponent)
            {
                ValueChangeEvent[] vces = ValueChangeEventID.FindValueChangeEvents(anyComponent);
                foreach (ValueChangeEvent vce in vces)
                    vce.ListenToMasters();
            }
        }
    }
}