using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class RemoteCar : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The spline used by the object")]
    private Spline spline;

    [SerializeField]
    [Tooltip("The speed of the remote car")]
    private float speed;

    private float percentage;

    [Header("Ground")]
    [Space]

    [SerializeField]
    [Tooltip("The raycaster")]
    private Transform raycaster;

    [SerializeField]
    [Tooltip("The ground level")]
    private Transform groundLevel;
    private float groundOffset;

    // Start is called before the first frame update
    void Start()
    {
        percentage = 0;
        groundOffset = transform.position.y - groundLevel.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        percentage = Mathf.Repeat(percentage + Time.deltaTime * speed / 100f, 1);
        Vector3 position = spline.GetPosition(percentage);
        transform.position = StickToTheGround(position);
        //transform.position = StickToTheGround(position);
        transform.rotation = Quaternion.LookRotation(spline.GetDirection(percentage));
    }

    private Vector3 StickToTheGround(Vector3 position)
    {
        LayerMask mask = LayerMask.GetMask("AsphaltGround") | LayerMask.GetMask("GrassGround") | LayerMask.GetMask("SoilGround");
        if (Physics.Raycast(raycaster.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
        {
            return new Vector3(position.x, hit.point.y + groundOffset, position.z);
        }
        return position;
    }
}
