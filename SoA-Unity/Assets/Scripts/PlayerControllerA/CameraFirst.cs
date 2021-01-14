using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFirst : MonoBehaviour
{

    private Inputs inputs;

    [Space]
    [Header("Camera Settings")]
    [Space]

    [SerializeField]
    private GameObject player;

    [SerializeField]
    [Tooltip("Camera's angular offset from the player's orientation")]
    private Vector3 cameraAngularOffset;

    [Space]
    [Header("Position (Spherical Coordinates)")]
    [Space]

    [SerializeField]
    [Tooltip("Distance to the player")]
    private float radius = 10f;
    private float storedRadius;

    [SerializeField]
    [Tooltip("Longitude in degrees")]
    [Range(-180, 180)]
    private float longitude = 20f;
    private float storedLongitude;

    [SerializeField]
    [Tooltip("Latitude in degrees")]
    [Range(-90, 90)]
    private float latitude = -45f;
    private float storedLatitude;

    [Space]
    [Header("Position (Cartesian Coordinates)")]
    [Space]

    [SerializeField]
    [Tooltip("Camera's position in cartesians coordinates relative to the player's position")]
    private Vector3 cameraOffset;
    private Vector3 storedCameraOffset;

    private Vector3 lastPlayerPosition;

    [Space]
    [Header("Look around")]
    [Space]

    [SerializeField]
    [Tooltip("The held camera (must be a child of the camera holder)")]
    private GameObject heldCamera;

    [SerializeField]
    [Tooltip("Max horizontal angle of a look around")]
    [Range(5, 90)]
    private float maxHorizontalAngle = 45; // degrees

    [SerializeField]
    [Tooltip("Max vertical angle of a look around")]
    [Range(5, 90)]
    private float maxVerticalAngle = 45; // degrees

    [SerializeField]
    [Tooltip("Duration to reach the maxHorizontalLookAroundAngle when the input is pushed at max")]
    [Range(0.1f, 5)]
    private float horizontalDuration = 0.5f; // seconds

    [SerializeField]
    [Tooltip("Duration to reach the maxVerticalLookAroundAngle when the input is pushed at max")]
    [Range(0.1f, 5)]
    private float verticalDuration = 0.5f; // seconds

    private Vector2 accumulator = Vector2.zero;

    enum STATE
    {
        NORMAL,
        HURRY,
        PROTECTED,
        NORMAL_TO_HURRY,
        HURRY_TO_NORMAL,
        HURRY_TO_PROTECTED,
        NORMAL_TO_PROTECTED,
        PROTECTED_TO_NORMAL,
        PROTECTED_TO_HURRY
    };
    STATE cameraState;

    [Space]
    [Header("Normal Mode")]

    [SerializeField]
    [Tooltip("The delay to switch from hurry to normal view, in seconds")]
    [Range(0.1f, 5)]
    private float timeHurryToNormal = 1.5f;

    [SerializeField]
    [Tooltip("The delay to switch from protected to normal view, in seconds")]
    [Range(0.1f, 5)]
    private float timeProtectedToNormal = 0.7f;

    [Space]
    [Header("Hurry Mode")]

    [SerializeField]
    [Tooltip("Z-Offset when player is in hurry mode (-closer, +farther)")]
    [Range(-10, 10)]
    private float Z_OffsetHurry = -4f;

    [SerializeField]
    [Tooltip("Y-Offset when player is in hurry mode (-closer, +farther)")]
    [Range(-10, 10)]
    private float Y_OffsetHurry = 0;

    [SerializeField]
    [Tooltip("The delay to switch from normal to hurry view, in seconds")]
    [Range(0.1f, 5)]
    private float timeNormalToHurry = 0.7f;

    [SerializeField]
    [Tooltip("The delay to switch from protected to hurry view, in seconds")]
    [Range(0.1f, 5)]
    private float timeProtectedToHurry = 0.4f;

    [Space]
    [Header("Protected Mode")]

    [SerializeField]
    [Tooltip("Z-Offset when player is in protected mode (-closer, +farther)")]
    [Range(-10, 10)]
    private float Z_OffsetProtected = -10f;

    [SerializeField]
    [Tooltip("Y-Offset when player is in hurry mode (-closer, +farther)")]
    [Range(-10, 10)]
    private float Y_OffsetProtected = 4;

    [SerializeField]
    [Tooltip("The delay to switch from normal to protected view, in seconds")]
    [Range(0.1f, 5)]
    private float timeNormalToProtected = 0.7f;

    [SerializeField]
    [Tooltip("The delay to switch from hurry to protected view, in seconds")]
    [Range(0.1f, 5)]
    private float timeHurryToProtected = 0.4f;

    private float zoomTimer;

    private float angleFromNormalToHorizon = 0;
    private float angleFromHurryToHorizon = 0;
    private float angleFromProtectedToHorizon = 0;

    private void Awake()
    {
        inputs = InputsManager.Instance.Inputs;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeConverter();

        transform.position = player.transform.position + Quaternion.Euler(90, 0, 0) * new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * latitude) * Mathf.Cos(Mathf.Deg2Rad * longitude),
                                                                      radius * Mathf.Cos(Mathf.Deg2Rad * latitude) * Mathf.Sin(Mathf.Deg2Rad * longitude),
                                                                      radius * Mathf.Sin(Mathf.Deg2Rad * latitude));

        angleFromNormalToHorizon = Vector3.Angle(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up).normalized, (player.transform.position - transform.position).normalized);

        Vector3 hurryPosition = transform.position - Z_OffsetHurry * Vector3.ProjectOnPlane((player.transform.position - transform.position).normalized, Vector3.up) + Y_OffsetHurry * Vector3.up;
        angleFromHurryToHorizon = Vector3.Angle(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up).normalized, (player.transform.position - hurryPosition).normalized);

        Vector3 protectedPosition = transform.position - Z_OffsetProtected * Vector3.ProjectOnPlane((player.transform.position - transform.position).normalized, Vector3.up) + Y_OffsetProtected * Vector3.up;
        angleFromProtectedToHorizon = Vector3.Angle(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up).normalized, (player.transform.position - protectedPosition).normalized);

        cameraState = STATE.NORMAL;
        zoomTimer = 0;

        lastPlayerPosition = player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateCamera();

        LookAround(inputs.Player.LookAround.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        inputs.Player.Enable();
    }

    private void OnDisable()
    {
        inputs.Player.Disable();
    }

    private void UpdateCamera ()
    {
        UpdateRecoilPosition();

        transform.position += (player.transform.position - lastPlayerPosition);
        lastPlayerPosition = player.transform.position;

        transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);

        transform.rotation *= Quaternion.Euler(cameraAngularOffset.x, cameraAngularOffset.y, cameraAngularOffset.z);
    }

    void UpdateRecoilPosition()
    {
        if (cameraState == STATE.NORMAL)
        {
            if (player.GetComponent<PlayerFollow>().IsProtectingEyes || player.GetComponent<PlayerFirst>().IsProtectingEars)
            {
                zoomTimer = timeNormalToProtected;
                cameraState = STATE.NORMAL_TO_PROTECTED;
            }

            else if (player.GetComponent<PlayerFollow>().IsHurry)
            {
                zoomTimer = timeNormalToHurry;
                cameraState = STATE.NORMAL_TO_HURRY;
            }
        }

        else if (cameraState == STATE.HURRY)
        {
            if (player.GetComponent<PlayerFollow>().IsProtectingEyes || player.GetComponent<PlayerFirst>().IsProtectingEars)
            {
                zoomTimer = timeHurryToProtected;
                cameraState = STATE.HURRY_TO_PROTECTED;
            }
            else if (!player.GetComponent<PlayerFollow>().IsHurry)
            {
                zoomTimer = timeHurryToNormal;
                cameraState = STATE.HURRY_TO_NORMAL;
            }
        }

        else if (cameraState == STATE.PROTECTED)
        {
            if (!player.GetComponent<PlayerFollow>().IsProtectingEyes && !player.GetComponent<PlayerFirst>().IsProtectingEars)
            {
                if (player.GetComponent<PlayerFollow>().IsHurry)
                {
                    zoomTimer = timeProtectedToHurry;
                    cameraState = STATE.PROTECTED_TO_HURRY;
                }
                else
                {
                    zoomTimer = timeProtectedToNormal;
                    cameraState = STATE.PROTECTED_TO_NORMAL;
                }
            }
        }

        else if (cameraState == STATE.NORMAL_TO_HURRY)
        {
            Vector3 startPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * (timeNormalToHurry - zoomTimer) / timeNormalToHurry; // recreate original position
            Vector3 endPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * zoomTimer / timeNormalToHurry; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeNormalToHurry - zoomTimer) / timeNormalToHurry);

            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                cameraState = STATE.HURRY;
            }
        }

        else if (cameraState == STATE.HURRY_TO_NORMAL)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * (timeHurryToNormal - zoomTimer) / timeHurryToNormal; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * zoomTimer / timeHurryToNormal; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeHurryToNormal - zoomTimer) / timeHurryToNormal);

            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                cameraState = STATE.NORMAL;
            }
        }

        else if (cameraState == STATE.NORMAL_TO_PROTECTED)
        {
            Vector3 startPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * (timeNormalToProtected - zoomTimer) / timeNormalToProtected; // recreate original position
            Vector3 endPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * zoomTimer / timeNormalToProtected; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeNormalToProtected - zoomTimer) / timeNormalToProtected);

            if (zoomTimer <= 0)
            {
                transform.position = endPosition;  // should wait one frame more
                cameraState = STATE.PROTECTED;
            }
        }

        else if (cameraState == STATE.PROTECTED_TO_NORMAL)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * zoomTimer / timeProtectedToNormal; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal);

            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                cameraState = STATE.NORMAL;
            }
        }

        else if (cameraState == STATE.HURRY_TO_PROTECTED)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetHurry - Z_OffsetProtected) + Vector3.up * (Y_OffsetHurry - Y_OffsetProtected)) * (timeHurryToProtected - zoomTimer) / timeHurryToProtected; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetHurry - Z_OffsetProtected) + Vector3.up * (Y_OffsetHurry - Y_OffsetProtected)) * zoomTimer / timeHurryToProtected; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeHurryToProtected - zoomTimer) / timeHurryToProtected);

            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                cameraState = STATE.PROTECTED;
            }
        }

        else if (cameraState == STATE.PROTECTED_TO_HURRY)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetProtected - Z_OffsetHurry) + Vector3.up * (Y_OffsetProtected - Y_OffsetHurry)) * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetProtected - Z_OffsetHurry) + Vector3.up * (Y_OffsetProtected - Y_OffsetHurry)) * zoomTimer / timeProtectedToHurry; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry);

            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                cameraState = STATE.HURRY;
            }
        }
    }

    void LookAround(Vector2 v)
    {
        float smoothx = 0;
        float smoothy = 0;

        bool lookingAround = false;

        if(Mathf.Approximately(v.magnitude, 0))
        {
            lookingAround = false;
        }
        else
        {
            lookingAround = true;
        }

        if (!Mathf.Approximately(v.x, 0)) // then increase accumulator.x
        {
            accumulator.x = Mathf.Clamp(accumulator.x + v.x * Time.deltaTime / horizontalDuration, -1, 1);
            smoothx = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
        }
        else // then decrease accumulator.x
        {
            accumulator.x = (1 - Mathf.Sign(accumulator.x)) / 2f * Mathf.Min(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0) + (1 + Mathf.Sign(accumulator.x)) / 2f * Mathf.Max(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0);
            smoothx = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
        }

        if (!Mathf.Approximately(v.y, 0)) // then increase accumulator.y
        {
            accumulator.y = Mathf.Clamp(accumulator.y + v.y * Time.deltaTime / verticalDuration, -1, 1);
            smoothy = Mathf.Sign(accumulator.y) * Mathf.Sin(Mathf.Abs(accumulator.y) * Mathf.PI * 0.5f);
        }
        else // then decrease accumulator.y
        {
            accumulator.y = (1 - Mathf.Sign(accumulator.y)) / 2f * Mathf.Min(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0) + (1 + Mathf.Sign(accumulator.y)) / 2f * Mathf.Max(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0);
            smoothy = Mathf.Sign(accumulator.y) * Mathf.Sin(Mathf.Abs(accumulator.y) * Mathf.PI * 0.5f);
        }

        // Stabilization of the look around
        float y_stabilization = 0;
        if (cameraState == STATE.NORMAL) { y_stabilization = -angleFromNormalToHorizon; } // this state needed by this camera

        else if (cameraState == STATE.HURRY)                { y_stabilization = -angleFromHurryToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_HURRY)      { y_stabilization = -(angleFromNormalToHorizon * zoomTimer / timeNormalToHurry + angleFromHurryToHorizon * (timeNormalToHurry - zoomTimer) / timeNormalToHurry); }
        else if (cameraState == STATE.HURRY_TO_NORMAL)      { y_stabilization = -(angleFromHurryToHorizon * zoomTimer / timeHurryToNormal + angleFromNormalToHorizon * (timeHurryToNormal - zoomTimer) / timeHurryToNormal); }

        else if (cameraState == STATE.PROTECTED)           { y_stabilization = -angleFromProtectedToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_PROTECTED) { y_stabilization = -(angleFromNormalToHorizon * zoomTimer / timeNormalToProtected + angleFromProtectedToHorizon * (timeNormalToProtected - zoomTimer) / timeNormalToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_NORMAL) { y_stabilization = -(angleFromProtectedToHorizon * zoomTimer / timeProtectedToNormal + angleFromNormalToHorizon * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal); }

        else if (cameraState == STATE.HURRY_TO_PROTECTED) { y_stabilization = -(angleFromHurryToHorizon * zoomTimer / timeHurryToProtected + angleFromProtectedToHorizon * (timeHurryToProtected - zoomTimer) / timeHurryToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_HURRY) { y_stabilization = -(angleFromProtectedToHorizon * zoomTimer / timeProtectedToHurry + angleFromHurryToHorizon * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry); }

        // Must be separated in three because unity's order for euler is ZYX and we want X-Y-X
        heldCamera.transform.localRotation  = Quaternion.Euler( Mathf.Abs(smoothx) * y_stabilization, 0, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(0, smoothx * maxHorizontalAngle, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(-smoothy * maxVerticalAngle, 0, 0);

        //heldCamera.transform.localRotation = Quaternion.Euler(-smoothy * maxLookAroundAngle, smoothx * maxLookAroundAngle, 0);
    }

    /* Spherical-Cartesian conversion functions */

    private void InitializeConverter()
    {
        storedCameraOffset = cameraOffset;
        storedLatitude     = latitude;
        storedLongitude    = longitude;
        storedRadius       = radius;
    }

    private void SphericalToCartesian()
    {
        Debug.Log("Spherical -> Cartesian");
        cameraOffset.x = radius * Mathf.Cos(longitude * Mathf.Deg2Rad) * Mathf.Cos(latitude * Mathf.Deg2Rad);
        cameraOffset.y = radius * Mathf.Sin(longitude * Mathf.Deg2Rad) * Mathf.Cos(latitude * Mathf.Deg2Rad);
        cameraOffset.z = radius * Mathf.Sin(latitude * Mathf.Deg2Rad);

        storedCameraOffset = cameraOffset;
    }

    public void CartesianToSpherical()
    {
        Debug.Log("Cartesian -> Spherical");
        radius = cameraOffset.magnitude;
        longitude = Mathf.Atan(Mathf.Sqrt(Mathf.Pow(cameraOffset.x,2) + Mathf.Pow(cameraOffset.y,2)) / cameraOffset.z) * Mathf.Rad2Deg;
        latitude = Mathf.Atan(cameraOffset.x / cameraOffset.y) * Mathf.Rad2Deg;

        storedLatitude = latitude;
        storedLongitude = longitude;
        storedRadius = radius;
    }

    // OnValidate is called before Awake
    private void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            if (storedCameraOffset != cameraOffset)
            {
                transform.position += (cameraOffset - storedCameraOffset); // update the game

                storedCameraOffset = cameraOffset;
                CartesianToSpherical();
            }

            else if (storedRadius != radius)
            {
                // TO DO : update the game from changed value

                storedRadius = radius;
                SphericalToCartesian();
            }
            else if (storedLongitude != longitude)
            {
                // TO DO : update the game from changed value

                storedLongitude = longitude;
                SphericalToCartesian();
            }
            else if (storedLatitude != latitude)
            {
                // TO DO : update the game from changed value

                storedLatitude = latitude;
                SphericalToCartesian();
            }
        }
    }

} //FINISH
