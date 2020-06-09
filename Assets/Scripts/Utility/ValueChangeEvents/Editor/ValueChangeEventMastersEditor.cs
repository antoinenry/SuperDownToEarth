﻿using UnityEngine;
using UnityEditor;

namespace VCE
{
    public class ValueChangeEventMastersEditor
    {
        public ValueChangeEvent target;

        private ValueChangeEventExplorer masterExplorer;

        public ValueChangeEventMastersEditor(ValueChangeEvent vce)
        {
            target = vce;
        }

        public float GetHeight()
        {
            float height = 0f;
            if (target != null)
            {
                height += (target.MasterCount) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (masterExplorer != null) height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
            return height;
        }

        public void OnGUI(Rect position, ref bool changeCheck)
        {
            int masterCount = target.MasterCount;

            EditorGUI.indentLevel++;

            Rect lineRect = position;            
            lineRect.height = EditorGUIUtility.singleLineHeight;

            Rect masterBoxRect = position;
            masterBoxRect.height += 2f * EditorGUIUtility.standardVerticalSpacing - EditorGUIUtility.singleLineHeight;
            masterBoxRect.y += EditorGUIUtility.singleLineHeight;

            Rect button1Rect = lineRect;
            button1Rect.width = 30f;
            button1Rect.x = lineRect.width - button1Rect.width / 2f;
            Rect button2Rect = button1Rect;
            button2Rect.x -= button2Rect.width + 1f;
            lineRect.width -= button1Rect.width + button2Rect.width + 1f;

            if (target.MasterCount > 0)
            {
                GUI.Box(lineRect, "");
                EditorGUI.LabelField(lineRect, "Follows:");
            }
            else
                EditorGUI.HelpBox(lineRect, "No masters", MessageType.Info);

            if (masterExplorer == null && GUI.Button(button1Rect, "+"))
                InitMasterExplorer(target);
            else if (masterExplorer != null && GUI.Button(button1Rect, "x"))
                masterExplorer = null;

            if (masterCount > 0)
            {
                lineRect.width -= button1Rect.width + 1f;

                for (int i = 0; i < masterCount; i++)
                {
                    lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.LabelField(lineRect, target.GetMaster(i).ToString());

                    button1Rect.y = lineRect.y;
                    if (GUI.Button(button1Rect, "-")) target.RemoveMasterAt(i);

                    button2Rect.y = button1Rect.y;
                    if (GUI.Button(button2Rect, "...")) ValueChangeEventDrawer.FocusOn(target.GetMaster(i));
                }
            }

            if (masterExplorer != null)
            {
                lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                masterExplorer.ExplorerGUI(lineRect, out bool dirty);

                if (masterExplorer.HasSelection)
                {
                    button1Rect.y = lineRect.y;
                    if (GUI.Button(button1Rect, "Ok"))
                    {
                        target.AddMaster(masterExplorer.SelectedVceID);
                        masterExplorer = null;
                    }
                }

                if (dirty) changeCheck = true;
            }

            EditorGUI.indentLevel --;
        }

        private void InitMasterExplorer(ValueChangeEvent target)
        {
            masterExplorer = new ValueChangeEventExplorer();

            int masterCount = target.MasterCount;
            if (target.MasterCount == 0)
                masterExplorer.selectedGameObject = Selection.activeGameObject;
            else
                masterExplorer.SetSelection(target.GetMaster(target.MasterCount - 1));

            masterExplorer.filter = new System.Predicate<ValueChangeEventID>(
                id => id.ValueChangeEvent != null
                && (target.ValueType == null || id.ValueChangeEvent.ValueType == target.ValueType)
                && id.ValueChangeEvent != target);
        }
    }
}