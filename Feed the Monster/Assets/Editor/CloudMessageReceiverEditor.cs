using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CloudMessageReceiver))]
public class CloudMessageReceiverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CloudMessageReceiver receiver = (CloudMessageReceiver)target;
        if(GUILayout.Button("Send Test Message"))
        {
            receiver.displayTestNotification();
        }
    }
}
