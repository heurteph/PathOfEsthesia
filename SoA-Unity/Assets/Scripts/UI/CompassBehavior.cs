using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompassBehavior : MonoBehaviour
{
    private GameObject compassMagnet;

    private GameObject mainCamera;

    private GameObject compassBackground;

    private float degrees = 0;
    private Vector3 axis;
    private Quaternion orientation;

    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        compassMagnet = GameObject.FindGameObjectWithTag("CompassMagnet");
        compassBackground = transform.parent.gameObject;

        Debug.Assert(compassBackground != null, "No child found in Compass");
        Debug.Assert(mainCamera != null, "Missing main camera");
        Debug.Assert(compassMagnet != null, "Missing compass magnet");

        //axis = (Quaternion.Euler(-45, 0, 0) * Vector3.up).normalized;
        axis = Vector3.forward;
        orientation = Quaternion.FromToRotation(Vector3.forward, axis);

        compassBackground.transform.rotation = orientation;
    }

    // Update is called once per frame
    void Update()
    {
        Redraw();
    }

    private void OnEnable()
    {
        Redraw();
    }

    public void Redraw()
    {
        degrees = Vector3.SignedAngle(Vector3.ProjectOnPlane(compassMagnet.transform.position - mainCamera.transform.position, Vector3.up), Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up), Vector3.up);
        transform.rotation = Quaternion.AngleAxis(degrees, axis) * orientation;
    }

    public void ReloadReferences()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        compassMagnet = GameObject.FindGameObjectWithTag("CompassMagnet");

        compassBackground = transform.parent.gameObject;
        compassBackground.transform.rotation = orientation;
    }
}
