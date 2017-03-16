using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor {

    CameraController cameraController;

    public override void OnInspectorGUI()
    {
        cameraController = (CameraController)target;

        cameraController.CameraMode = (CameraModes)EditorGUILayout.EnumPopup("Camera Mode", cameraController.CameraMode);

        if (cameraController.CameraMode <= CameraModes.HARDFOLLOW)
        {
            EditorGUILayout.LabelField("");
            cameraController.myObjectToFollow = (GameObject)EditorGUILayout.ObjectField("Object to follow", cameraController.myObjectToFollow, typeof(GameObject), true);

            EditorGUILayout.LabelField("");
            cameraController.offset.x = EditorGUILayout.FloatField("x offset", cameraController.offset.x);
            cameraController.offset.y = EditorGUILayout.FloatField("y offset", cameraController.offset.y);
            cameraController.offset.z = EditorGUILayout.FloatField("z offset", cameraController.offset.z);

            EditorGUILayout.LabelField("");
            cameraController.maxOffset.x = EditorGUILayout.FloatField("Max x offset", cameraController.maxOffset.x);
            cameraController.maxOffset.y = EditorGUILayout.FloatField("Max y offset", cameraController.maxOffset.y);
            cameraController.maxOffset.z = EditorGUILayout.FloatField("Max z offset", cameraController.maxOffset.z);

            if (cameraController.CameraMode == CameraModes.SOFTFOLLOW)
            {
                EditorGUILayout.LabelField("");
                cameraController.followSpeed = EditorGUILayout.FloatField("Follow speed", cameraController.followSpeed);
            }
        }
        else if (cameraController.CameraMode == CameraModes.CUTSCENE)
        {
            // Do some cutscene stuff
        }
        else if (cameraController.CameraMode == CameraModes.NONE)
        {

        }
    }
}
