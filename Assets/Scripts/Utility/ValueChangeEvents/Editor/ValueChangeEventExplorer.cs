using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Scarblab.VCE
{
    public class ValueChangeEventExplorer
    {
        public struct ValueChangeComponent
        {
            public Component component;
            public ValueChangeEventID[] vceIDs;
        }

        public Predicate<ValueChangeEventID> filter;
        public GameObject selectedGameObject;

        public bool HasSelection { get => (vceIDOptions != null && selectedComponentIndex >= 0 /*&& selectedComponentIndex < vceIDOptions.Count*/); }
        public ValueChangeEventID SelectedVceID { get => HasSelection ? vceIDOptions[selectedVceIndex] : ValueChangeEventID.None; }
        public Component SelectedComponent { get => (componentOptions != null && selectedComponentIndex >= 0 && selectedComponentIndex < componentOptions.Count) ? componentOptions[selectedComponentIndex].component : null; }

        private List<ValueChangeComponent> componentOptions;
        private List<string> componentOptionNames;
        private int selectedComponentIndex;

        private List<ValueChangeEventID> vceIDOptions;
        private List<string> vceOptionNames;
        private int selectedVceIndex;

        public void SetSelection(ValueChangeEventID selectionID)
        {
            if (selectionID.Component != null)
            {
                selectedGameObject = selectionID.Component.gameObject;

                GetComponentOptions();
                selectedComponentIndex = componentOptions.FindIndex(c => c.component == selectionID.Component);

                GetVCEOptions();
                selectedVceIndex = vceIDOptions.IndexOf(selectionID);
            }
        }

        public void ExplorerGUI(Rect position, out bool dirty)
        {
            dirty = false;

            position.width /= 3f;
            GameObject go = EditorGUI.ObjectField(position, selectedGameObject, typeof(GameObject), true) as GameObject;
            if (go != selectedGameObject)
            {
                selectedGameObject = go;
                dirty = true;
            }

            position.x += position.width;
            if (GetComponentOptions())
            {
                int componentIndex = EditorGUI.Popup(position, selectedComponentIndex, componentOptionNames.ToArray());
                if (componentIndex != selectedComponentIndex)
                {
                    selectedComponentIndex = componentIndex;
                    dirty = true;
                }
            }

            position.x += position.width;
            if (GetVCEOptions())
            {
                int vceIndex = EditorGUI.Popup(position, selectedVceIndex, vceOptionNames.ToArray());
                if (vceIndex != selectedVceIndex)
                {
                    selectedVceIndex = vceIndex;
                    dirty = true;
                }
            }
        }

        private bool GetComponentOptions()
        {
            if (selectedGameObject == null)
            {
                componentOptions = null;
                componentOptionNames = null;
                selectedComponentIndex = 0;
                return false;
            }
            else
            {
                componentOptions = new List<ValueChangeComponent>();

                Component[] componentsInSelection = selectedGameObject.GetComponents<Component>();
                foreach (Component c in componentsInSelection)
                {
                    List<ValueChangeEventID> vcesIDsInComponent;
                    if (filter == null) vcesIDsInComponent = new List<ValueChangeEventID>(ValueChangeEventID.FindValueChangeEventsIDs(c));
                    else vcesIDsInComponent = new List<ValueChangeEventID>(ValueChangeEventID.FindValueChangeEventsIDs(c)).FindAll(filter);

                    if (vcesIDsInComponent.Count > 0)
                        componentOptions.Add(new ValueChangeComponent() { component = c, vceIDs = vcesIDsInComponent.ToArray() });
                }

                if (componentOptions.Count == 0)
                {
                    componentOptions = null;
                    componentOptionNames = null;
                }
                else
                    componentOptionNames = componentOptions.ConvertAll(option => option.component.GetType().ToString());
            }

            return componentOptions != null;
        }

        private bool GetVCEOptions()
        {
            if (SelectedComponent == null)
            {
                vceIDOptions = null;
                vceOptionNames = null;
                selectedVceIndex = 0;
            }
            else
            {
                vceIDOptions = new List<ValueChangeEventID>(componentOptions[selectedComponentIndex].vceIDs);
                if (vceIDOptions.Count == 0)
                    vceOptionNames = null;
                else
                    vceOptionNames = new List<ValueChangeEventID>(vceIDOptions).ConvertAll(id => id.Name);
            }

            return vceIDOptions != null;
        }
    }
}