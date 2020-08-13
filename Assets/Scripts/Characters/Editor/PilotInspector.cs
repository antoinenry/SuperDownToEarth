/*
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pilot))]
public class PilotInspector : Editor
{
    private bool showTransferGUI;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        showTransferGUI = EditorGUILayout.Foldout(showTransferGUI, "Transfer bodyparts to vehicle");
        if (showTransferGUI)
        {
            EditorGUILayout.BeginVertical("box");
            TranferPartsGUI();
            EditorGUILayout.EndVertical();
        }
    }

    private void TranferPartsGUI()
    {
        Pilot pilot = target as Pilot;
        if (pilot.body != null)
        {
            List<BodyPart> availableParts = new List<BodyPart>(pilot.body.GetComponentsInChildren<BodyPart>(true));
            List<string> availablePartNames = availableParts.ConvertAll<string>(part => part.ToString());
            List<string> options;

            if (pilot.transferPartsToVehicle != null)
            {
                options = new List<string>(availablePartNames);
                options.Add("[remove part]");

                int currentNumParts = pilot.transferPartsToVehicle.Length;
                for (int i = 0; i < currentNumParts; i++)
                {
                    int currentPartIndex = availableParts.IndexOf(pilot.transferPartsToVehicle[i]);
                    int selectedPartIndex = EditorGUILayout.Popup(currentPartIndex, options.ToArray());

                    if (selectedPartIndex != currentPartIndex)
                    {
                        if (selectedPartIndex < availableParts.Count)
                            pilot.transferPartsToVehicle[i] = availableParts[selectedPartIndex];
                        else
                        {
                            List<BodyPart> newParts = new List<BodyPart>(pilot.transferPartsToVehicle);
                            newParts.RemoveAt(i);
                            pilot.transferPartsToVehicle = newParts.ToArray();

                            return;
                        }
                    }
                }
            }
            
            options = new List<string>(availablePartNames);
            options.Add("add part...");
            int newPartIndex = EditorGUILayout.Popup(options.Count - 1, options.ToArray());

            if (newPartIndex < availableParts.Count)
            {
                List<BodyPart> newParts = new List<BodyPart>(pilot.transferPartsToVehicle);
                newParts.Add(availableParts[newPartIndex]);
                pilot.transferPartsToVehicle = newParts.ToArray();

                return;
            }
            
        }
    }
}
*/