#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_Animated), true)]
	[CanEditMultipleObjects]
	public class TMP_AnimatedEditor : TMP_BaseEditorPanel
	{
		SerializedProperty speedProp;
		SerializedProperty btwWordProp;
		SerializedProperty writingSFXProp;

		protected override void OnEnable()
		{
			base.OnEnable();
			speedProp = serializedObject.FindProperty("speed");
			btwWordProp = serializedObject.FindProperty("timeBtwWord");
			writingSFXProp = serializedObject.FindProperty("writingSFX");
		}
		protected override void OnUndoRedo() { }
		protected override void DrawExtraSettings()
		{
			EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(speedProp, new GUIContent("     Speed"));
			EditorGUILayout.PropertyField(btwWordProp, new GUIContent("     TimeBtwWord"));
			EditorGUILayout.PropertyField(writingSFXProp, new GUIContent("     WritingSFX"));
		}
		protected override bool IsMixSelectionTypes() => false;
	}
}
#endif
