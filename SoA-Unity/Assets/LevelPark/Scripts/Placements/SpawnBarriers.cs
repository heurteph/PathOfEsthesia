/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class SpawnBarriers : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The model of the barrier to spawn")]
    private GameObject barrierPrefab;

    [SerializeField]
    [Tooltip("The spline used to spawn the barriers")]
    private Spline spline;

    [SerializeField]
    private Vector3 rotationFixer;
    private Quaternion qRotationFixer;

    // Start is called before the first frame update
    void Start()
    {
        if (barrierPrefab == null)
        {
            throw new System.NullReferenceException("No prefab set for the barrier");
        }
        if (spline == null)
        {
            throw new System.NullReferenceException("No spline set for the barriers");
        }

        float startPercent = 0.01f;
        float endPercent;
        int i = 0;
        while(i < 10)
        {
            qRotationFixer = Quaternion.Euler(rotationFixer);
            Vector3 startPos = spline.GetPosition(startPercent);
            Vector3 barrierEnd = startPos + spline.GetDirection(startPercent) * 2;
            Debug.Log("DIRECTION : " + spline.GetDirection(startPercent));
            endPercent = spline.ClosestPoint(barrierEnd);
            Vector3 endPos = spline.GetPosition(endPercent);
            Quaternion rot = qRotationFixer * Quaternion.LookRotation(endPos - startPos);
            GameObject barrier = Object.Instantiate(barrierPrefab, startPos, rot);
            barrier.transform.SetParent(transform, true);
            barrier.name = "Barrier" + i++.ToString();
            Debug.Log(barrier.name + ", start : " + startPercent + ", end : " + endPercent);
            startPercent = endPercent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
*/