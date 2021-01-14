using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class FindGameObjectsWithMesh : EditorWindow
{
    [MenuItem("Tools/ProBuilder/Debug/Find GameObjects with Mesh Name")]
    private static void MenuInit()
    {
        GetWindow<FindGameObjectsWithMesh>(true, "Find GameObjects with Name", true);
    }

    private string m_MeshName = "";
    private MeshFilter[] m_Matches = new MeshFilter[0];

    private void OnGUI()
    {
        m_MeshName = EditorGUILayout.TextField("Mesh", m_MeshName);

        if (GUILayout.Button("Find"))
            m_Matches = Resources.FindObjectsOfTypeAll<MeshFilter>().Where(
                x => x.sharedMesh != null && x.sharedMesh.name.Contains(m_MeshName))
                    .ToArray();

        GUILayout.Label("GameObjects with Mesh Name", EditorStyles.boldLabel);

        foreach (MeshFilter mf in m_Matches)
        {
            if (GUILayout.Button(mf.sharedMesh.name))
                EditorGUIUtility.PingObject(mf);
        }
    }
}
