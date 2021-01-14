using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class SpawnStreetLampsSpline : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The model of the street light to spawn")]
    private GameObject streetLightPrefab;

    [SerializeField]
    [Tooltip("The number of street lamps in the spline")]
    [Range(1,100)]
    private int splineLength = 10;

    [SerializeField]
    [Tooltip("The spline used to spawn the street lights")]
    private Spline spline;

    [SerializeField]
    private Vector3 rotationFixer;
    private Quaternion qRotationFixer;

    // Start is called before the first frame update
    void Start()
    {
        if(streetLightPrefab == null)
        {
            throw new System.NullReferenceException("No prefab set for the street light");
        }
        if(spline == null)
        {
            throw new System.NullReferenceException("No spline set for the street lights");
        }

        for (int i = 0; i < splineLength; i++)
        {
            qRotationFixer = Quaternion.Euler(rotationFixer);
            Vector3 pos = spline.GetPosition((i + 1f) / (splineLength + 1f));
            //Quaternion rot = qRotationFixer * Quaternion.LookRotation(spline.GetDirection((i + 1f) / (splineLength + 1f)));
            Quaternion rot = qRotationFixer * Quaternion.LookRotation(transform.forward);
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
