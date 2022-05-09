using System;
using System.Collections.Generic;
using System.Globalization;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using GameBrains.Extensions.DictionaryExtensions;
using UnityEditor;
using UnityEngine;

namespace GameBrains.Editor.PropertyDrawers
{
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        protected static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
        protected static GUIContent moveButtonContent = new GUIContent("\u21b4", "move down");
        protected static GUIContent duplicateButtonContent = new GUIContent("+", "duplicate");
        protected static GUIContent deleteButtonContent = new GUIContent("-", "delete");
        protected static GUIContent addButtonContent = new GUIContent("+", "add element");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUILayout.LabelField(label.text + " (multiple objects selected)",
                    EditorStyles.boldLabel);
            }
            else
            {
                SerializedProperty keysListProperty = property.FindPropertyRelative("keysList");
                SerializedProperty keysArraySizeProp =
                    keysListProperty.FindPropertyRelative("Array.size");
                SerializedProperty valuesListProperty = property.FindPropertyRelative("valuesList");
                SerializedProperty valuesArraySizeProp =
                    keysListProperty.FindPropertyRelative("Array.size");

                EditorGUILayout.BeginHorizontal();
                if (keysArraySizeProp.intValue == 0)
                {
                    if (GUILayout.Button(addButtonContent, EditorStyles.miniButton))
                    {
                        keysListProperty.arraySize += 1;
                        valuesListProperty.arraySize += 1;
                    }

                    EditorGUILayout.LabelField(
                        label,
                        EditorStyles.boldLabel,
                        GUILayout.MaxWidth(150));
                }
                else
                {
                    property.isExpanded
                        = EditorGUILayout.Foldout(property.isExpanded, label,
                            EditorStyles.foldoutHeader);
                }

                GUILayout.FlexibleSpace();
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(
                        keysArraySizeProp,
                        GUIContent.none,
                        GUILayout.MaxWidth(50));
                }

                EditorGUILayout.EndHorizontal();

                if (property.isExpanded)
                {
                    // Don't make child fields be indented
                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;

                    if (keysArraySizeProp.intValue == valuesArraySizeProp.intValue)
                    {
                        EditorGUI.indentLevel++;

                        DrawKeysAndValues(keysArraySizeProp,
                            keysListProperty,
                            valuesArraySizeProp,
                            valuesListProperty);

                        EditorGUI.indentLevel--;
                    }

                    // Set indent back to what it was
                    EditorGUI.indentLevel = indent;
                }
            }
        }

        protected virtual void DrawKeysAndValues(
            SerializedProperty keysArraySizeProp,
            SerializedProperty keysListProperty,
            SerializedProperty valuesArraySizeProp,
            SerializedProperty valuesListProperty)
        {
            //using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < keysArraySizeProp.intValue; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    ShowButtons(keysListProperty, valuesListProperty, i);

                    float saveLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 40;
                    EditorGUILayout.PropertyField(keysListProperty.GetArrayElementAtIndex(i),
                        new GUIContent("K" + i));
                    EditorGUIUtility.labelWidth = saveLabelWidth;

                    saveLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 40;
                    EditorGUILayout.PropertyField(
                        valuesListProperty.GetArrayElementAtIndex(i),
                        new GUIContent("V" + i));
                    EditorGUIUtility.labelWidth = saveLabelWidth;

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        protected virtual void ShowButtons(
            SerializedProperty keys,
            SerializedProperty values,
            int i)
        {
            var dictionaryObject = fieldInfo.GetValue(keys.serializedObject.targetObject);
            var dictionaryType = dictionaryObject.GetType();
            var removeMethod
                = dictionaryType.GetMethod("Remove", new Type[] { typeof(int) });

            if (removeMethod != null &&
                GUILayout.Button(
                    deleteButtonContent,
                    EditorStyles.miniButton,
                    GUILayout.ExpandWidth(false)))
            {
                removeMethod.Invoke(
                    dictionaryObject,
                    new object[] { keys.GetArrayElementAtIndex(i).intValue });
            }
        }
    }

    [CustomPropertyDrawer(typeof(IntIntSerializableDictionary))]
    [CustomPropertyDrawer(typeof(IntFloatSerializableDictionary))]
    [CustomPropertyDrawer(typeof(IntStringSerializableDictionary))]
    // [CustomPropertyDrawer(typeof(/*type of*/ SerializableDictionary))]
    // Add other types here
    public class AnySerializableDictionaryPropertyDrawer
        : SerializableDictionaryPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NodeEdgeSerializableDictionary))]
    public class NodeEdgeSerializableDictionaryPropertyDrawer
        : SerializableDictionaryPropertyDrawer
    {
        Node fromNode;
        List<bool> edgeFoldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            fromNode = property.serializedObject.targetObject as Node;

            if (fromNode)
            {
                if (edgeFoldout == null) edgeFoldout = new List<bool>();
            }

            // ReSharper disable once Unity.PropertyDrawerOnGUIBase
            base.OnGUI(position, property, label);
        }

        protected override void DrawKeysAndValues(
            SerializedProperty keysArraySizeProp,
            SerializedProperty keysListProperty,
            SerializedProperty valuesArraySizeProp,
            SerializedProperty valuesListProperty)
        {
            if (!fromNode) return;

            var savedIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;

            List<Edge> edgeList = fromNode.outEdges.valuesList;

            for (int i = 0; i < edgeList.Count; i++)
            {
                Edge edge = edgeList[i];
                Node toNode = edge.ToNode;
                float cost = edge.Cost;

                while (i >= edgeFoldout.Count) edgeFoldout.Add(false);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15);
                edgeFoldout[i]
                    = EditorGUILayout.BeginFoldoutHeaderGroup(edgeFoldout[i], "Edge " + i);
                EditorGUILayout.EndHorizontal();

                using (new EditorGUI.DisabledScope(true))
                {
                    if (edgeFoldout[i])
                    {
                        EditorGUILayout.LabelField("FromNode", fromNode.name);
                        EditorGUILayout.LabelField("Cost",
                            cost.ToString(CultureInfo.InvariantCulture));
                        EditorGUILayout.LabelField("ToNode", toNode.name);
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            EditorGUI.indentLevel = savedIndent;
        }
    }
}