using UnityEngine;
using UnityEditor;

namespace e23.RotorcraftController.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(RotorcraftBehaviour))]
    public class RotorcraftBehaviourEditor : UnityEditor.Editor
    {
        private SerializedProperty rotorcraftModel;
        private SerializedProperty rotorcraftRigidbody;
        private SerializedProperty rotorcraftSettings;

        private void OnEnable()
        {
            rotorcraftModel = serializedObject.FindProperty("_rotorcraftModel");
            rotorcraftRigidbody = serializedObject.FindProperty("_rotorcraftRigidbody");

            rotorcraftSettings = serializedObject.FindProperty("_rotorcraftSettings");
        }

        public override void OnInspectorGUI()
        {
            RotorcraftBehaviour rotorcraftBehaviour = (RotorcraftBehaviour)target;

            this.serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(rotorcraftModel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(rotorcraftRigidbody);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(rotorcraftSettings);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Update Rotorcraft Settings", GUILayout.MinHeight(100), GUILayout.Height(50)))
            {
                rotorcraftBehaviour.SetRotorcraftSettings();
            }
        }
    }
}