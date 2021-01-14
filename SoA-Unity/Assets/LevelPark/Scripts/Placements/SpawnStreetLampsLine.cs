using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class SpawnStreetLampsLine : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The model of the street light to spawn")]
    private GameObject streetLightPrefab;

    [SerializeField]
    [Tooltip("The number of street lamps in the line")]
    [Range(1,100)]
    private int lineLength = 10;

    [SerializeField]
    [Tooltip("The start position of the line")]
    private Transform startLine;

    [SerializeField]
    [Tooltip("The end position of the line")]
    private Transform endLine;

    private Quaternion rotationFixer = Quaternion.Euler(0, 180, 0);

    // Start is called before the first frame update
    void Start()
    {
        if(streetLightPrefab == null)
        {
            throw new System.NullReferenceException("No prefab set for the street light");
        }
        if(startLine == null)
        {
            throw new System.NullReferenceException("No start position set for the street lights");
        }
        if(endLine == null)
        {
            throw new System.NullReferenceException("No end position set for the street lights");
        }
        if(endLine == startLine)
        {
            throw new System.NullReferenceException("Start and end position are the same for the street lights line");
        }

        Vector3 startPos = startLine.position;
        Vector3 endPos   = endLine.position;

        for (int i = 0; i < lineLength; i++)
        {
            Vector3 pos = Vector3.Lerp(startPos, endPos, (i + 1f) / (lineLength + 1f));
            Quaternion rot = rotationFixer * Quaternion.LookRotation(Vector3.Slerp(startLine.forward, endLine.forward, (i + 1f) / (lineLength + 1f)));
            GameObject streetLight = Object.Instantiate(streetLightPrefab, pos, rot);
            streetLight.transform.SetParent(transform, true);
            streetLight.name = "StreetLamp " + i.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
