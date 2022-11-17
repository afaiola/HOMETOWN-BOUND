using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    public Transform waypointRoot;      // parent for waypoint

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        if (waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("Add");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        if (GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
        if (Selection.activeGameObject != null & Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if (GUILayout.Button("Create Branch"))
            {
                CreateBranch();
            }
            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if (GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if (GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }
    }

    Waypoint NewWaypoint()
    {
        GameObject waypointObj = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObj.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObj.GetComponent<Waypoint>();
        waypoint.branches = new List<Waypoint>();
        return waypoint;
    }

    void CreateBranch()
    {
        Waypoint waypoint = NewWaypoint();

        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        branchedFrom.branches.Add(waypoint);

        waypoint.transform.forward = branchedFrom.transform.forward;
        waypoint.transform.position = branchedFrom.transform.position;

        Selection.activeGameObject = waypoint.gameObject;
    }

    void CreateWaypoint()
    {
        Waypoint waypoint = NewWaypoint();
        if (waypointRoot.childCount > 1)
        {
            waypoint.prevWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.prevWaypoint.nextWaypoint = waypoint;
            // Place the waypoint at the last position
            waypoint.transform.position = waypoint.prevWaypoint.transform.position;
            waypoint.transform.forward = waypoint.prevWaypoint.transform.forward; 
        }

        Selection.activeGameObject = waypoint.gameObject;
    }

    void CreateWaypointBefore()
    {
        Waypoint newWaypoint = NewWaypoint();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        newWaypoint.width = selectedWaypoint.width;
        newWaypoint.transform.forward = selectedWaypoint.transform.forward;
        newWaypoint.transform.position = selectedWaypoint.transform.position - selectedWaypoint.transform.forward;

        if (selectedWaypoint.prevWaypoint != null)
        {
            newWaypoint.prevWaypoint = selectedWaypoint.prevWaypoint;
            selectedWaypoint.prevWaypoint.nextWaypoint = newWaypoint;
        }

        newWaypoint.nextWaypoint = selectedWaypoint;

        selectedWaypoint.prevWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        selectedWaypoint.transform.SetSiblingIndex(newWaypoint.transform.GetSiblingIndex()+1);

        Selection.activeGameObject = newWaypoint.gameObject;

    }

    void CreateWaypointAfter()
    {
        Waypoint newWaypoint = NewWaypoint();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        newWaypoint.width = selectedWaypoint.width;
        newWaypoint.transform.forward = selectedWaypoint.transform.forward;
        newWaypoint.transform.position = selectedWaypoint.transform.position + selectedWaypoint.transform.forward * 2f;

        newWaypoint.prevWaypoint = selectedWaypoint;

        if (selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.prevWaypoint = newWaypoint;
            newWaypoint.nextWaypoint = selectedWaypoint;
        }

        selectedWaypoint.nextWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        selectedWaypoint.transform.SetSiblingIndex(newWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if (selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.prevWaypoint = selectedWaypoint.prevWaypoint;
        }

        if (selectedWaypoint.prevWaypoint != null)
        {
            selectedWaypoint.prevWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            Selection.activeGameObject = selectedWaypoint.prevWaypoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

}
