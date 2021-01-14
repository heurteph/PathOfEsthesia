using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;

public class SplineParkUser : MonoBehaviour
{
    [SerializeField] [Tooltip("The spline used by the user")]
    private Spline spline;

    [Header("Position")]
    [Space]

    [SerializeField] [Range(0, 1)] [Tooltip("Initial position on the spline (in percent)")]
    private float startPercentage = 0;

    private float percentage;

    [Header("Move")]
    [Space]

    [SerializeField]
    [Tooltip("The speed of the user")]
    private float speed = 10f;

    [SerializeField]
    [Tooltip("Does the user start moving immediately or wait for a trigger ?")]
    private bool onStart = true;

    [Header("Duration")]
    [Space]

    [SerializeField]
    [Range(1f, 5f)]
    [Tooltip("The duration of the freeze")]
    private float freezeDuration = 3f;

    [Header("Ground")]
    [Space]

    [SerializeField]
    [Tooltip("The raycaster")]
    private Transform raycaster;

    [SerializeField]
    [Tooltip("The ground level")]
    private Transform groundLevel;
    private float groundOffset;

    private enum DIRECTION { FORWARD, BACKWARD }
    [SerializeField]
    private DIRECTION directionState;

    public enum STATE { NORMAL, FREEZE }
    private STATE movingState;

    private void Start()
    {
        percentage = startPercentage;
        spline.CalculateLength();

        if(groundLevel != null) groundOffset = transform.position.y - groundLevel.transform.position.y;
        movingState = STATE.NORMAL;

        Vector3 position = Vector3.zero;
        if (directionState == DIRECTION.FORWARD)
        {
            position = spline.GetPosition(percentage);
            transform.rotation = Quaternion.LookRotation(spline.GetDirection(Mathf.Max(percentage, 0.01f), true)); // initial rotation
        }
        else if (directionState == DIRECTION.BACKWARD)
        {
            position = spline.GetPosition(1 - percentage);
            transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, 180, 0) * spline.GetDirection(Mathf.Min(1 - percentage, 0.99f), true)); // initial rotation
        }
        if (raycaster != null && groundLevel != null) transform.position = StickToTheGround(position);
        else transform.position = position;

        if (onStart) { StartCoroutine("Move"); }
    }

    public void Trigger()
    {
        if (!onStart) { StartCoroutine("Move"); }
    }

    void Update()
    {

    }

    private IEnumerator Move()
    {
        for(; ;)
        {
            if (movingState != STATE.FREEZE)
            {
                /* UPDATE POSITION */

                percentage = Mathf.Min(percentage + speed * Time.deltaTime / spline.Length, 1f);

                Vector3 position = Vector3.zero;
                if (directionState == DIRECTION.FORWARD)
                {
                    position = spline.GetPosition(percentage);
                    transform.rotation = Quaternion.LookRotation(spline.GetDirection(percentage, true));
                }
                else if (directionState == DIRECTION.BACKWARD)
                {
                    position = spline.GetPosition(1 - percentage);
                    transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, 180, 0) * spline.GetDirection(1 - percentage, true));
                }
                if(raycaster != null && groundLevel != null) transform.position = StickToTheGround(position);
                else transform.position = position;

                if (percentage == 1)
                {
                    percentage = 0;
                }
            }
            yield return null;
        }
    }

    private IEnumerator ResumeMove()
    {
        yield return new WaitForSeconds(freezeDuration);
        movingState = STATE.NORMAL;
    }

    private Vector3 StickToTheGround(Vector3 position)
    {
        LayerMask mask = LayerMask.GetMask("AsphaltGround") | LayerMask.GetMask("GrassGround") | LayerMask.GetMask("SoilGround");
        if(Physics.Raycast(raycaster.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
        {
            return new Vector3(position.x, hit.point.y + groundOffset, position.z);
        }
        return position;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            if (movingState != STATE.FREEZE)
            {
                movingState = STATE.FREEZE;
                StartCoroutine("ResumeMove");
            }
        }
    }
}
