using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace GameBrains.Actuators.Motion.Navigation.SearchGraph
{
	public static class GraphEditingSupport
	{
		public static bool DetermineIfLocked(
			SerializedObject serializedObject,
			string singularLabel,
			string pluralLabel,
			bool isLockedExternally)
		{
			SerializedProperty iterator = serializedObject.GetIterator();

			bool isLocked = false;
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				enterChildren = false;

				using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
				{
					EditorGUILayout.PropertyField(iterator, true);
				}

				if ("m_Script" == iterator.propertyPath)
				{
					isLocked = CheckIfLocked(serializedObject, singularLabel, pluralLabel, isLockedExternally);
				}

				if (isLocked || isLockedExternally) { break; }
			}

			return isLocked;
		}
		static bool CheckIfLocked(
			SerializedObject serializedObject,
			string singularLabel,
			string pluralLabel,
			bool isLockedExternally)
		{
			bool locked;
			SerializedProperty lockedProperty = serializedObject.FindProperty("locked");
			string lockStatusText;

			if (serializedObject.isEditingMultipleObjects)
			{
				MultipleBoolInfo mbi = GetMultipleBoolInfo(serializedObject, "locked");
				locked = mbi.anyTrue;

				if (isLockedExternally) { return locked; }

				lockStatusText
					= (mbi.allSame ? "All" : "Some")
					  + " selected "
					  + pluralLabel
					  + " are "
					  + (locked ? "locked. " : "unlocked. ")
					  + (locked
						? "Unlock to enable multi-editing. "
						: "Lock to disable multi-editing.");
			}
			else
			{
				locked = lockedProperty.boolValue;

				if (isLockedExternally) { return locked; }

				lockStatusText
					= (locked
						? "Selected " + singularLabel + " is locked. "
						: "Selected " + singularLabel + " is unlocked. ")
					  + (locked
						? "Unlock to enable editing."
						: "Lock to disable editing.");
			}

			ShowLockStatus(serializedObject, lockedProperty, locked, lockStatusText);

			return locked;
		}

		static void ShowLockStatus(
			SerializedObject serializedObject,
			SerializedProperty lockedProperty,
			bool locked,
			string lockStatusText)
		{
			//TODO: Is there a better way to find the Inspector width?
			//TODO: Is there any way to get the indent width (15f) from Unity.
			// Width of the inspector minus 2 * 15 for the content margin and minus 2 * 15 for
			// every level of indent.
			float lineWidthInPixels
				= EditorGUIUtility.currentViewWidth - 2 * (EditorGUI.indentLevel + 1) * 15f;

			var firstLine
				= SplitFirstLineUsingLineWidth(
					lockStatusText,
					lineWidthInPixels - 15f, // Account for checkbox
					EditorStyles.boldLabel,
					out string remainingText);

			var result = EditorGUILayout.ToggleLeft(
				firstLine,
				false,
				EditorStyles.boldLabel);

			var lines
				= SplitTextUsingLineWidth(
					remainingText,
					lineWidthInPixels,
					EditorStyles.boldLabel);

			foreach (string line in lines)
			{
				EditorGUILayout.LabelField(line, EditorStyles.boldLabel);
			}

			if (!locked) { EditorGUILayout.Space(); }

			if (result)
			{
				lockedProperty.boolValue = !locked;
				serializedObject.ApplyModifiedProperties();
			}
		}

		public static void HandleExternallyLocked(
			SerializedObject serializedObject,
			string singularText,
			string pluralText,
			string ancestorSingularText,
			string ancestorPluralText,
			string isExternallyLockedBy)
		{
			string lockStatusText;

			if (serializedObject.isEditingMultipleObjects)
			{
				MultipleBoolInfo mbi
					= GetMultipleBoolInfo(
						serializedObject,
						isExternallyLockedBy,
						"locked");

				lockStatusText
					= "The " +
					  pluralText +
					  " are locked because " +
					  (mbi.allSame ? "all " : "some ") +
					  ancestorPluralText +
					  " are locked. Unlock the " +
					  ancestorPluralText +
					  " to multi-edit.";
			}
			else
			{
				lockStatusText
					= "The " +
					  singularText +
					  " is locked because its " +
					  ancestorSingularText +
					  " is locked. Unlock the " +
					  ancestorSingularText +
					  " to edit.";
			}

			//TODO: Is there a better way to find the Inspector width?
			//TODO: Is there any way to get the indent width (15f) from Unity.
			// Width of the inspector minus 2 * 15 for the content margin and minus 2 * 15 for
			// every level of indent.
			float lineWidthInPixels
				= EditorGUIUtility.currentViewWidth - 2 * (EditorGUI.indentLevel + 1) * 15f;
			var lines
				= SplitTextUsingLineWidth(
					lockStatusText,
					lineWidthInPixels,
					EditorStyles.boldLabel);

			foreach (string line in lines)
			{
				EditorGUILayout.LabelField(line, EditorStyles.boldLabel);
			}
		}

		public struct MultipleBoolInfo
		{
			public bool anyTrue;
			public bool anyFalse;
			public bool allTrue;
			public bool allFalse;
			public bool allSame;
		}
		public static MultipleBoolInfo GetMultipleBoolInfo(
			SerializedObject serializedObject,
			string propertyName)
		{
			MultipleBoolInfo mbi;
			mbi.anyTrue = false;
			mbi.anyFalse = false;

			var objectCount = serializedObject.targetObjects.Length;

			for (int i = 0; i < objectCount; i++)
			{
				var so = new SerializedObject(serializedObject.targetObjects[i]);
				var sp = so.FindProperty(propertyName);
				mbi.anyFalse |= !sp.boolValue;
				mbi.anyTrue |= sp.boolValue;
			}

			mbi.allTrue = mbi.anyTrue && !mbi.anyFalse;
			mbi.allFalse = mbi.anyFalse && !mbi.anyTrue;
			mbi.allSame = mbi.allTrue || mbi.allFalse;

			return mbi;
		}

		public static MultipleBoolInfo GetMultipleBoolInfo(
			SerializedObject serializedObject,
			string propertyName,
			string subPropertyName)
		{
			MultipleBoolInfo mbi;
			mbi.anyTrue = false;
			mbi.anyFalse = false;

			var objectCount = serializedObject.targetObjects.Length;

			for (int i = 0; i < objectCount; i++)
			{
				var so = new SerializedObject(serializedObject.targetObjects[i]);
				var sp = so.FindProperty(propertyName);
				if (sp.objectReferenceValue)
				{
					var spo = new SerializedObject(sp.objectReferenceValue);
					var ssp = spo.FindProperty(subPropertyName);
					mbi.anyFalse |= !ssp.boolValue;
					mbi.anyTrue |= ssp.boolValue;
				}
			}

			mbi.allTrue = mbi.anyTrue && !mbi.anyFalse;
			mbi.allFalse = mbi.anyFalse && !mbi.anyTrue;
			mbi.allSame = mbi.allTrue || mbi.allFalse;

			return mbi;
		}

		public static string SplitFirstLineUsingLineWidth(
			string text,
			float limitInPixels,
			GUIStyle style,
			out string remaining)
		{
			string[] splitSeparator = {" ", "\r\n", "\n"};
			string[] words = text.Split(splitSeparator, StringSplitOptions.None);
			string line = string.Empty;

			foreach (string word in words)
			{
				if (string.IsNullOrWhiteSpace(word)) continue;

				var newLine = string.Join(" ", line, word).Trim();
				float newLineLength = style.CalcSize(new GUIContent(newLine)).x;
				if (newLineLength >= limitInPixels)
				{
					remaining = text.Remove(0, line.Length);
					return line;
				}

				line = newLine;
			}

			remaining = string.Empty;

			return line;
		}

		public static List<string> SplitTextUsingLineWidth(
			string text,
			float limitInPixels,
			GUIStyle style)
		{
			string[] splitSeparator = {" ", "\r\n", "\n"};
			string[] words = text.Split(splitSeparator, StringSplitOptions.None);

			List<string> lines = new List<string>();

			string line = string.Empty;
			foreach (string word in words)
			{
				if (string.IsNullOrWhiteSpace(word)) continue;

				var newLine = string.Join(" ", line, word).Trim();
				float newLineLength = style.CalcSize(new GUIContent(newLine)).x;
				if (newLineLength >= limitInPixels)
				{
					lines.Add(line);
					line = word;
				}
				else
				{
					line = newLine;
				}
			}

			if (line.Length > 0) lines.Add(line);

			return lines;
		}
	}
}

#endif