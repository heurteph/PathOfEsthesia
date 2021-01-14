using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class SpawnParkTreesSpline : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The model of the park tree to spawn")]
    private GameObject parkTreePrefab;

    [SerializeField]
    [Tooltip("The number of park trees in the spline")]
    [Range(1,100)]
    private int splineLength = 10;

    [SerializeField]
    [Tooltip("The spline used to spawn the park trees")]
    private Spline spline;

    [SerializeField]
    private Vector3 rotationFixer;
    private Quaternion qRotationFixer;

    // Start is called before the first frame update
    void Start()
    {
        if(parkTreePrefab == null)
        {
            throw new System.NullReferenceException("No prefab set for the park tree");
        }
        if(spline == null)
        {
            throw new System.NullReferenceException("No spline set for the park tree");
        }

        for (int i = 0; i < splineLength; i++)
        {
            qRotationFixer = Quaternion.Euler(rotationFixer);
            Vector3 pos = spline.GetPosition((i + 1f) / (splineLength + 1f));
            //Quaternion rot = qRotationFixer * Quaternion.LookRotation(spline.GetDirection((i + 1f) / (splineLength + 1f)));
            Quaternion randomize = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Quaternion rot = qRotationFixer * randomize * Quaternion.LookRotation(transform.forward);
            GameObject parkTree = Object.Instantiate(parkTreePrefab, pos, rot);
            parkTree.transform.SetParent(transform, true);
            parkTree.name = "ParkTree " + i.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
