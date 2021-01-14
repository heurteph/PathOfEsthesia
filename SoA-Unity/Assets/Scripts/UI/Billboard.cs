using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private GameObject mainCamera;

    //private float zAngle;
    ParticleSystem.ShapeModule shapeModule;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Assert(GetComponent<ParticleSystemRenderer>() != null);
        //GetComponent<ParticleSystemRenderer>().material.renderQueue = 4001;
        //zAngle = transform.rotation.eulerAngles.z;
        //shapeModule = GetComponent<ParticleSystem>().shape;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Assert(mainCamera != null);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Vector3.zero);
        transform.LookAt(mainCamera.transform.position);
        //transform.rotation = Quaternion.LookRotation(mainCamera.transform.position - transform.position);
        //shapeModule.rotation = Quaternion.LookRotation(mainCamera.transform.position - shapeModule.position).eulerAngles;
        //transform.LookAt(Camera.main.transform.position);
    }
}
