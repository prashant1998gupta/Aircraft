using UnityEditor;
using UnityEngine;

namespace e23.RotorcraftController.Editor
{
    [CustomEditor(typeof(RotorcraftGroundFollow))]
    public class GroundFollowEditor : UnityEditor.Editor
    {
        private SerializedProperty distanceFromGround;
        private SerializedProperty forwardRay;
        private SerializedProperty rearRay;

        private float groundDistance;
        private void OnEnable()
        {
            distanceFromGround = serializedObject.FindProperty("distanceFromGround");
            forwardRay = serializedObject.FindProperty("forwardRay");
            rearRay = serializedObject.FindProperty("rearRay");
        }

        private void OnSceneGUI()
        {
            var target = base.target as RotorcraftGroundFollow;
            var targetTransform = target.transform;
            var position = new Vector3(targetTransform.position.x, targetTransform.position.y + 0.5f, targetTransform.position.z);
            var forwardPos = targetTransform.TransformPoint(targetTransform.localPosition.x + forwardRay.vector3Value.x, targetTransform.localPosition.x + forwardRay.vector3Value.y, targetTransform.localPosition.x + forwardRay.vector3Value.z);
            var rearPos = targetTransform.TransformPoint(targetTransform.localPosition.x + rearRay.vector3Value.x, targetTransform.localPosition.y + rearRay.vector3Value.y, targetTransform.localPosition.z + rearRay.vector3Value.z);

            Handles.color = Color.yellow;
            
            Handles.DrawLine(forwardPos, forwardPos + (-Vector3.up * (distanceFromGround.floatValue * 2f)));
            Handles.DrawLine(position, position + (-Vector3.up * (distanceFromGround.floatValue * 2f)));
            Handles.DrawLine(rearPos, rearPos + (-Vector3.up * (distanceFromGround.floatValue * 2f)));
            Handles.DrawLine(forwardPos, rearPos);
        }
    }
}