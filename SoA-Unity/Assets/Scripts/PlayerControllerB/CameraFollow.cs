using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Inputs inputs;

    private GameObject gameManager;

    [Space]
    [Header("Camera Settings")]
    [Space]

    [SerializeField]
    [Tooltip("The character followed by the camera")]
    private GameObject player;

    private float originalYRotation;
    private Vector3 lastPlayerPosition;

    [SerializeField]
    [Range(0,20)]
    private float maxChromaticAberration = 10;

    [Space]

    [SerializeField]
    [Tooltip("Camera's translation offset from the player's position")]
    private Vector3 cameraOffset = new Vector3(0, 1, -10);
    private Vector3 storedCameraOffset;

    [SerializeField]
    [Tooltip("Camera's angular offset from the player's orientation")]
    private Vector3 cameraAngularOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("Speed at which the camera align itself to the character")]
    [Range(0.1f, 5)]
    private float alignSpeed = 0.6f;

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
    private Vector2 lastInputVector = Vector2.zero;
    private Vector2 startAccumulator = Vector2.zero;
    private Vector2 smoothAccumulator = Vector2.zero;

    // Projective Look-Around

    private float projectiveAccumulator = 0;
    private float projectiveDistance = 6; // unused
    [SerializeField]
    [Tooltip("Duration to reach the projective position in seconds")]
    [Range(0.1f,1f)]
    private float projectiveDuration = 0.3f; //s
    [SerializeField]
    private Vector3 projectiveOffset = new Vector3(0f, 5f, 20f);

    enum STATE { 
        NORMAL,
        HURRY,
        PROTECTED,
        NORMAL_TO_HURRY,
        HURRY_TO_NORMAL,
        HURRY_TO_PROTECTED,
        NORMAL_TO_PROTECTED,
        PROTECTED_TO_NORMAL,
        PROTECTED_TO_HURRY,
        HURRY_TO_GAMEOVER,
        PROTECTED_TO_GAMEOVER,
        GAMEOVER
    };
    STATE cameraState;
    STATE lastCameraState;

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
    private float Z_OffsetHurry = 2.5f;

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

    [Space]
    [Header("Gameover Mode")]

    [SerializeField]
    [Tooltip("Z-Offset when game is over (-closer, +farther)")]
    [Range(-10, 10)]
    private float Z_OffsetGameover = -10f;

    [SerializeField]
    [Tooltip("Y-Offset when game is over (-closer, +farther)")]
    [Range(-10, 10)]
    private float Y_OffsetGameover = 4f;

    [SerializeField]
    [Tooltip("The delay to switch from hurry to game over view, in seconds")]
    [Range(0.1f, 5f)]
    private float timeHurryToGameover = 1f;

    [SerializeField]
    [Tooltip("The delay to switch from protected to game over view, in seconds")]
    [Range(0.1f, 5f)]
    private float timeProtectedToGameover = 1f;

    [SerializeField]
    [Tooltip("The vertical speed of the camera when it flies into the air")]
    [Range(0,10)]
    private float flyAwaySpeed = 2f;

    private float zoomTimer;

    private float angleFromNormalToHorizon = 0;
    private float angleFromHurryToHorizon = 0;
    private float angleFromProtectedToHorizon = 0;

    [Space]
    [Header("Sway")]

    [SerializeField]
    [Tooltip("The sway pivot")]
    private GameObject cameraSway;

    [SerializeField]
    [Tooltip("The minimal sway radius when in Normal Mode")]
    [Range(0.001f, 1f)]
    private float swayNormalRadiusMin = 0.01f;

    [SerializeField]
    [Tooltip("The maximal sway radius when in Normal Mode")]
    [Range(0.001f, 1f)]
    private float swayNormalRadiusMax = 0.05f;

    private float latitude = 0, longitude = 0, swayRadius = 0;

    [SerializeField]
    [Tooltip("The minimum sway duration when in Normal Mode")]
    [Range(2f, 10f)]
    private float swayNormalDurationMin = 4f;

    [SerializeField]
    [Tooltip("The maximum sway duration when in Normal Mode")]
    [Range(2f, 10f)]
    private float swayNormalDurationMax = 6f;

    [SerializeField]
    [Tooltip("The minimal sway radius when in Hurry Mode")]
    [Range(0.001f, 2f)]
    private float swayHurryRadiusMin = 0.8f;

    [SerializeField]
    [Tooltip("The maximal sway radius when in Hurry Mode")]
    [Range(0.001f, 2f)]
    private float swayHurryRadiusMax = 1f;

    [SerializeField]
    [Range(0.001f, 1f)]
    [Tooltip("The minimum sway duration when in Hurry Mode")]
    private float swayHurryDurationMin = 0.01f;

    [SerializeField]
    [Range(0.001f, 1f)]
    [Tooltip("The maximum sway duration when in Hurry Mode")]
    private float swayHurryDurationMax = 0.02f;

    private float swayDuration = 0;
    private float swayTimer = 0;

    private float swayDurationMin, swayDurationMax;
    private float swayRadiusMin, swayRadiusMax;
    private float backToNormalRadiusSpeed;
    private float backToNormalRadiusAcceleration;

    private Vector3 originalSwayPosition = Vector3.zero;
    private Vector3 targetSwayPosition = Vector3.zero;

    // Targeting

    private bool isTargeting = false;
    private Vector3 startForward = Vector3.zero;
    private Vector3 endForward = Vector3.zero;
    private Vector3 storedForward = Vector3.zero;
    private Quaternion storedRotation = Quaternion.identity;
    private float targetingTimer = 0;
    private Quaternion initialParentRotation = Quaternion.identity;
    private Quaternion currentParentRotation = Quaternion.identity;

    [Space]
    [Header("Targeting")]

    [SerializeField]
    private GameObject defaultForward;

    [SerializeField]
    [Range(0.5f,5f)]
    [Tooltip("The total duration of the targeting (pause included) in seconds")]
    private float targetingDuration = 5;

    [SerializeField]
    [Tooltip("The pause on the target in seconds")]
    [Range(0.5f, 2f)]
    private float targetingPause = 1;

    private Vector3 targetPosition = Vector3.zero;

    private bool isAvailable;
    public bool IsAvailable { get { return isAvailable; } set { isAvailable = value; } }

    private bool isPausingAlign;
    public bool IsPausingAlign { get { return isPausingAlign; } set { isPausingAlign = value; } }

    [Space]
    [Header("Collisions")]

    [SerializeField]
    [Range(1,10)]
    [Tooltip("The minimum distance between the camera and an obstacle along its path")]
    private float minDistanceToObstacle = 2;

    [SerializeField]
    [Range(0.1f,5f)]
    [Tooltip("The angle in degrees between the start and end of the LineCasts used to approximate a circular cast")]
    private float maxDegDelta = 1; // degrees

    bool isColliding = false; // TO DO : Remove
    float lastDistanceToCollider = Mathf.Infinity; // TO DO : Remove
    private float collidingAlignSpeed = 20; // TO DO : Remove

    private LayerMask noCollision;

    private void Awake()
    {
        inputs = InputsManager.Instance.Inputs;

        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        if(gameManager == null)
        {
            throw new System.NullReferenceException("No Game Manager found in the scene");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        storedCameraOffset = cameraOffset;
        originalYRotation  = transform.rotation.eulerAngles.y;
        // TO DO : Start from a higher point like an angel coming down
        transform.position = player.transform.position + player.transform.rotation /* * Quaternion.Euler(0,90,0) */ * cameraOffset;
        UpdateRotation();
        lastPlayerPosition = player.transform.position;

        // compute the angle between the camera in normal view and horizon view for the look-around stabilization
        //angleFromNormalToHorizon = Vector3.Angle(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up).normalized, (player.transform.position - transform.position).normalized);
        //angleFromNormalToHorizon -= cameraAngularOffset.x;
        angleFromNormalToHorizon = cameraAngularOffset.x;
        Debug.Log("angleFromNormalToHorizon : " + angleFromNormalToHorizon);

        // compute the angle between the camera in normal view and hurry view for the look-around stabilization
        Vector3 hurryPosition = transform.position - Z_OffsetHurry * Vector3.ProjectOnPlane((player.transform.position - transform.position).normalized, Vector3.up) + Y_OffsetHurry * Vector3.up;
        angleFromHurryToHorizon = Vector3.Angle(Vector3.ProjectOnPlane((player.transform.position - hurryPosition), Vector3.up).normalized, (player.transform.position - hurryPosition).normalized);
        angleFromHurryToHorizon += cameraAngularOffset.x; // TO DO : Check this out
        Debug.Log("angleFromHurryToHorizon : " + angleFromHurryToHorizon);

        // compute the angle between the camera in normal view and protected view for the look-around stabilization
        Vector3 protectedPosition = transform.position - Z_OffsetProtected * Vector3.ProjectOnPlane((player.transform.position - transform.position).normalized, Vector3.up) + Y_OffsetProtected * Vector3.up;
        angleFromProtectedToHorizon = Vector3.Angle(Vector3.ProjectOnPlane((player.transform.position - protectedPosition), Vector3.up).normalized, (player.transform.position - protectedPosition).normalized);
        angleFromProtectedToHorizon += cameraAngularOffset.x; // TO DO : Check this out
        Debug.Log("angleFromProtectedToHorizon : " + angleFromProtectedToHorizon);

        //FX
        heldCamera.transform.GetChild(0).GetComponent<Render_PostProcess>().shader_actif = true;
        heldCamera.transform.GetChild(0).GetComponent<Render_PostProcess>().coef_blur = 1000;
        heldCamera.transform.GetChild(0).GetComponent<Render_PostProcess>().radius = 5;


        cameraState = STATE.NORMAL;
        zoomTimer = 0;
        isAvailable = true;
        isPausingAlign = false;
        noCollision = ~ (LayerMask.GetMask("NoObstacle") | LayerMask.GetMask("Shelter Entrance") | LayerMask.GetMask("Shelter Exit"));

        if (cameraSway != null)
        {
            InitializeSway();
            StartCoroutine("Sway");
        }

        //StartCoroutine("AlignWithCharacter");
        StartCoroutine("AlignWithCharacter2");
    }
    void UpdateFromInspector()
    {
        if (cameraOffset != storedCameraOffset)
        {
            transform.position += (cameraOffset - storedCameraOffset);
            storedCameraOffset = cameraOffset;
        }
    }

    public void ResetCameraToFrontView()
    {
        lastPlayerPosition = player.transform.position; // disable following during the warp
        transform.position = player.transform.position + Quaternion.LookRotation(-player.transform.forward, Vector3.up) * cameraOffset;

        zoomTimer = 0; // check if correct
        cameraState = STATE.NORMAL;

        // do not align until first player move
        isPausingAlign = true;
    }

    private void OnEnable()
    {
        // might trigger inputs back during warp to shelter ???
        // inputs.Player.Enable();
    }

    private void OnDisable()
    {
        inputs.Player.Disable();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateFX();

        if (!player.GetComponent<PlayerFirst>().IsInsideShelter)
        {
            //UpdatePosition();

            UpdateRotation();

            if (!isTargeting)
            {
                //LookAround(inputs.Player.LookAround.ReadValue<Vector2>());
                //ProjectiveLookAround(inputs.Player.ProjectiveLook.ReadValue<float>());
                //ExtendedLookAround(inputs.Player.LookAround.ReadValue<Vector2>());
                SmoothLookAround(inputs.Player.LookAround.ReadValue<Vector2>());
                //SmoothProjectiveLookAround(inputs.Player.LookAround.ReadValue<Vector2>());
            }
            if (cameraSway != null)
            {
                //Sway(); // Let's try this here
            }
        }
    }

    /* Chroma Offset Effect */
    private void UpdateFX()
    {
        float factor = maxChromaticAberration * (1 - player.GetComponent<EnergyBehaviour>().Energy / 1000);
        heldCamera.transform.GetChild(0).GetComponent<Render_PostProcess>().offsetChroma = new Vector3(factor, 0, 0);
        //heldCamera.transform.GetChild(0).GetComponent<Render_PostProcess>().coef_blur = (player.GetComponent<EnergyBehaviour>().Energy / 1000) * 700 + 300;
    }

    private void UpdateRotation ()
    {
        if (cameraState == STATE.NORMAL)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up)); // kind of a lookAt but without the rotation around the x-axis

            transform.rotation *= Quaternion.Euler(cameraAngularOffset.x, cameraAngularOffset.y, cameraAngularOffset.z); // TO DO : ONLY IN NORMAL !!!!!
        }
        else if (cameraState == STATE.NORMAL_TO_HURRY) // focus on the character
        {
            Vector3 startPosition  = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * (timeNormalToHurry - zoomTimer) / timeNormalToHurry; // recreate original position
            Vector3 endPosition    = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * zoomTimer / timeNormalToHurry; // recreate original position
            Vector3 start   = Vector3.ProjectOnPlane((player.transform.position - startPosition), Vector3.up).normalized;
            // TO DO : Check this out
            start = Quaternion.AngleAxis(-cameraAngularOffset.x, Vector3.Cross(start, Vector3.up)).normalized * start;
            Vector3 end     = (player.transform.position - endPosition).normalized;
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToFocus - recoilTimer) / timeToFocus);
            Vector3 current = Vector3.Slerp(start, end, (timeNormalToHurry - zoomTimer) / timeNormalToHurry);
            //Debug.Log("Start : " + start + ", End : " + end + ", Angle : " + current + ", timeToFocus : " + timeToFocus + ", recoilTimer : " + recoilTimer);
            transform.rotation = Quaternion.LookRotation(current);
    

            //heldCamera.GetComponent<Camera>().fieldOfView = 60 - (60 - 50) * (timeToFocus - recoilTimer) / timeToFocus;
        }
        else if (cameraState == STATE.HURRY)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        }

        else if (cameraState == STATE.PROTECTED)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        }
        else if (cameraState == STATE.HURRY_TO_NORMAL)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * (timeHurryToNormal - zoomTimer) / timeHurryToNormal; // recreate original position
            Vector3 endPosition   = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * zoomTimer / timeHurryToNormal; // recreate original position
            Vector3 start   = (player.transform.position - startPosition).normalized;
            Vector3 end     = Vector3.ProjectOnPlane((player.transform.position - endPosition), Vector3.up).normalized;
            // TO DO : Check this out
            end = Quaternion.AngleAxis(-cameraAngularOffset.x, Vector3.Cross(end, Vector3.up)).normalized * end;
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            Vector3 current = Vector3.Slerp(start, end, (timeHurryToNormal - zoomTimer) / timeHurryToNormal);
            //Debug.Log("Start : " + start + ", End : " + end + ", Angle : " + current + ", timeToNormal : " + timeToNormal + ", recoilTimer : " + recoilTimer);
            transform.rotation = Quaternion.LookRotation(current);

            //heldCamera.GetComponent<Camera>().fieldOfView = 60 - (60 - 50) * recoilTimer / timeToNormal;
        }
        else if (cameraState == STATE.NORMAL_TO_PROTECTED) // focus on the character
        {
            Vector3 startPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * (timeNormalToProtected - zoomTimer) / timeNormalToProtected; // recreate original position
            Vector3 endPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * zoomTimer / timeNormalToProtected; // recreate original position
            Vector3 start = Vector3.ProjectOnPlane((player.transform.position - startPosition), Vector3.up).normalized;
            // TO DO : Check this out
            start = Quaternion.AngleAxis(-cameraAngularOffset.x, Vector3.Cross(start, Vector3.up)).normalized * start;
            Vector3 end = (player.transform.position - endPosition).normalized;
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToFocus - recoilTimer) / timeToFocus);
            Vector3 current = Vector3.Slerp(start, end, (timeNormalToProtected - zoomTimer) / timeNormalToProtected);
            //Debug.Log("Start : " + start + ", End : " + end + ", Angle : " + current + ", timeToFocus : " + timeToFocus + ", recoilTimer : " + recoilTimer);
            transform.rotation = Quaternion.LookRotation(current);

            //heldCamera.GetComponent<Camera>().fieldOfView = 60 - (60 - 50) * (timeToFocus - recoilTimer) / timeToFocus;
        }
        else if (cameraState == STATE.PROTECTED_TO_NORMAL)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * zoomTimer / timeProtectedToNormal; // recreate original position
            Vector3 start = (player.transform.position - startPosition).normalized;
            Vector3 end = Vector3.ProjectOnPlane((player.transform.position - endPosition), Vector3.up).normalized;
            // TO DO : Check this out
            end = Quaternion.AngleAxis(-cameraAngularOffset.x, Vector3.Cross(end, Vector3.up)).normalized * end;
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            Vector3 current = Vector3.Slerp(start, end, (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal);
            //Debug.Log("Start : " + start + ", End : " + end + ", Angle : " + current + ", timeToNormal : " + timeToNormal + ", recoilTimer : " + recoilTimer);
            transform.rotation = Quaternion.LookRotation(current);

            //heldCamera.GetComponent<Camera>().fieldOfView = 60 - (60 - 50) * recoilTimer / timeToNormal;
        }
        else if (cameraState == STATE.PROTECTED_TO_HURRY)
        {
            // TO DEBUG
            /*
            Vector3 startPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * -(Z_OffsetProtected - Z_OffsetHurry) + Vector3.up * -(Y_OffsetProtected - Y_OffsetHurry)) * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry; // recreate original position
            Vector3 endPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * -(Z_OffsetProtected - Z_OffsetHurry) + Vector3.up * -(Y_OffsetProtected - Y_OffsetHurry)) * zoomTimer / timeProtectedToHurry; // recreate original position
            Vector3 start = (player.transform.position - startPosition).normalized; // no projection on both start and end
            Vector3 end = (player.transform.position - endPosition).normalized;
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToFocus - recoilTimer) / timeToFocus);
            Vector3 current = Vector3.Slerp(start, end, (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry);
            //Debug.Log("Start : " + start + ", End : " + end + ", Angle : " + current + ", timeToFocus : " + timeToFocus + ", recoilTimer : " + recoilTimer);
            transform.rotation = Quaternion.LookRotation(current);
        */


            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            //heldCamera.GetComponent<Camera>().fieldOfView = 60 - (60 - 50) * (timeToFocus - recoilTimer) / timeToFocus;
        }
        else if (cameraState == STATE.HURRY_TO_PROTECTED)
        {
            // TO DEBUG

            /*
            Vector3 startPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * -(Z_OffsetHurry - Z_OffsetProtected) + Vector3.up * -(Y_OffsetHurry - Y_OffsetProtected)) * (timeHurryToProtected - zoomTimer) / timeHurryToProtected; // recreate original position
            Vector3 endPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * -(Z_OffsetHurry - Z_OffsetProtected) + Vector3.up * -(Y_OffsetHurry - Y_OffsetProtected)) * zoomTimer / timeHurryToProtected; // recreate original position
            Vector3 start = (player.transform.position - startPosition).normalized; // no projection on both start and end
            Vector3 end = (player.transform.position - endPosition).normalized;
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToFocus - recoilTimer) / timeToFocus);
            Vector3 current = Vector3.Slerp(start, end, (timeHurryToProtected - zoomTimer) / timeHurryToProtected);
            //Debug.Log("Start : " + start + ", End : " + end + ", Angle : " + current + ", timeToFocus : " + timeToFocus + ", recoilTimer : " + recoilTimer);
            transform.rotation = Quaternion.LookRotation(current);*/

            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);

            //heldCamera.GetComponent<Camera>().fieldOfView = 60 - (60 - 50) * (timeToFocus - recoilTimer) / timeToFocus;
        }

        else if (cameraState == STATE.HURRY_TO_GAMEOVER)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        }

        else if (cameraState == STATE.PROTECTED_TO_GAMEOVER)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        }

        else if (cameraState == STATE.GAMEOVER)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        }
    }

    private void UpdatePosition()
    {
        // if values have changed in the inspector
        UpdateFromInspector();

        // adapt recoil to normal, hurry or protected mode from the player
        UpdateRecoilPosition();

        if (player.transform.position != lastPlayerPosition)
        {
            //Debug.Log("Camera moving in " + transform.position);
            transform.position += (player.transform.position - lastPlayerPosition);
            lastPlayerPosition = player.transform.position;
        }
    }

    private bool CheckPlayerMovement()
    {
        return (player.transform.position != lastPlayerPosition);
    }

    private IEnumerator AlignWithCharacter()
    {
        for(; ;)
        {
            UpdatePosition(); // called here to avoid desynchronization

            float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized, player.transform.forward, Vector3.up) % 360;

            float smooth = 0.95f * -Mathf.Pow((Mathf.Abs(angle) / 180f - 1), 2) + 1;  // [0.05-1]

            if (Mathf.Abs(angle) >= alignSpeed * smooth * Time.deltaTime)
            {
                transform.RotateAround(player.transform.position, Vector3.up, Mathf.Sign(angle) * alignSpeed * smooth * Time.deltaTime);
                transform.rotation *= Quaternion.Euler(0, originalYRotation, 0);
            }
            else if (!Mathf.Approximately(angle, 0))
            {
                transform.RotateAround(player.transform.position, Vector3.up, angle);
                transform.rotation *= Quaternion.Euler(0, originalYRotation, 0);
            }
            yield return null;
        }
    }

    private IEnumerator AlignWithCharacter2()
    {
        if (!isTargeting) // CHECK IF CORRECT ?? SHOULDN'T IT BE INSIDE THE FOR LOOP ?
        {

            float targetAngle, newTargetAngle;
            float thisPercent, previousPercent;
            float thisSinerp, previousSinerp;

            // alignSpeed = 50; // ONLY FOR DEBUG

            for (; ; )
            {
                while(player.GetComponent<PlayerFirst>().IsInsideShelter)
                { 
                    yield return null;
                }

                while(isTargeting) // Let's try it here, CHECK IF WORKING
                {
                    yield return null;
                }

                while (isPausingAlign)
                {
                    //Debug.Log("Align en pause");
                    yield return null;
                    if(CheckPlayerMovement())
                    {
                        //Debug.Log("Sorti de pause");
                        isPausingAlign = false;
                    }
                }

                Vector3 originalPosition = transform.position; // used in LimitAngleToFirstObstacle

                targetAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized, player.transform.forward, Vector3.up) % 360;
                float lastTargetAngle = targetAngle; // save the last true angle
                Debug.Log("My target angle is " + targetAngle);
                targetAngle = LimitAngleToFirstObstacle(targetAngle, originalPosition);
                Debug.Log("My target angle, taking obstacle into account, is " + targetAngle);

                float factor = 1;

                // if camera not aligned with the closest position to be aligned with the character

                if (!Mathf.Approximately(targetAngle, 0.0f))
                {
                    Debug.Log("Start of an interpolation, angle : " + targetAngle);

                    Vector3 startForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                    thisPercent = 0;
                    thisSinerp = 0;
                    previousPercent = 0;

                    float debugSum = 0;

                    // Launch an interpolation

                    while (thisPercent != 1.0f)
                    {
                        UpdatePosition(); // called here to avoid desynchronization 

                        // Compute the angle from the current camera position to the align with the new character position

                        newTargetAngle = Vector3.SignedAngle(startForward, player.transform.forward, Vector3.up);
                        if (Mathf.Abs(newTargetAngle - lastTargetAngle) < 0.001f) // discard computationnal errors
                        {
                            newTargetAngle = lastTargetAngle;
                        }
                        lastTargetAngle = newTargetAngle;

                        // SignedAngle's return value is in domain [-180;180], so extend it

                        if (Mathf.Sign(newTargetAngle) != Mathf.Sign(targetAngle))
                        {
                            if((targetAngle - newTargetAngle) > 180)
                            {
                                newTargetAngle += 360;
                                Debug.Log("Warp newAngle : " + newTargetAngle);
                            }
                            else if ((newTargetAngle - targetAngle) > 180)
                            {
                                newTargetAngle -= 360;
                                Debug.Log("Warp newAngle : " + newTargetAngle);
                            }
                        }

                        // Next, check if there is a collision along the new rotation path

                        newTargetAngle = LimitAngleToFirstObstacle(newTargetAngle, originalPosition);

                        Debug.Log("New angle is : " + newTargetAngle + ", last one was " + targetAngle);

                        if (!Mathf.Approximately(newTargetAngle, targetAngle))
                        {
                            // Get the original [0-1] from the smooth [0-1] updated with the angle modification, need clamping because Asin's domain is [-1,1]

                            if (!Mathf.Approximately(newTargetAngle, 0)) // avoid dividing by zero
                            {
                                Debug.Log("New angle is different, remapping the percentage ...");
                                thisPercent = InverseSinerp(thisSinerp * Mathf.Abs(targetAngle / newTargetAngle)); // Mathf.Abs to handle change in sign during interpolation
                            }
                            else
                            {
                                thisPercent = 1;
                            }
                        }
                        previousPercent = thisPercent; // VERY IMPORTANT !!!!! NOT THE TRUE PREVIOUS PERCENT, THE ONE RECOMPUTED ACCORDING TO THE NEW ANGLE !!!!

                        // Hurry up if there's an obstacle between the camera and the character

                        if(Physics.Linecast(transform.position, player.transform.position, out RaycastHit hit, noCollision))
                        {
                            if( ! hit.transform.CompareTag("Player"))
                            {
                                //Debug.Log("COLLISION WITH : " + hit.transform.name);
                                factor = Mathf.Min(factor + 10 * Time.deltaTime, 3);
                            }
                            else
                            {
                                factor = Mathf.Max(factor - 10 * Time.deltaTime, 1);
                            }
                        }

                        // TO DO : Equivalent for behind when the character is facing the camera

                        // Increment to next position

                        Debug.Log("Linear evolution this frame : " + factor + " * " + alignSpeed + " * " + Time.deltaTime + " = " + factor * alignSpeed * Time.deltaTime * Mathf.Abs(newTargetAngle) / 180f);
                        thisPercent = Mathf.Min(thisPercent + factor * alignSpeed * Time.deltaTime, 1.0f); // TO CHECK : Dependant to angle gives quite another gamefeel

                        //previousSinerp  = thisSinerp;
                        previousSinerp =  Sinerp(previousPercent); // HOW IS IT DIFFERENT FROM THE LINE ABOVE ????
                        thisSinerp      = Sinerp(thisPercent);

                        //Debug.Log("Rotation delta : " + angle * (thisSinerp - previousSinerp) + " from angle=" + angle + ", thisSinerp=" + thisSinerp + ", previousSinerp=" + previousSinerp + ", thisFrame=" + thisFrame + ", previousFrame=" + previousFrame);
                        debugSum += newTargetAngle * (thisSinerp - previousSinerp);

                        transform.RotateAround(player.transform.position, Vector3.up, newTargetAngle * (thisSinerp - previousSinerp)); // <-- ERROR, THE SUM OF THE FRACTIONS DOESNT RESULT IN newAngle AT THE END
                        transform.rotation *= Quaternion.Euler(0, originalYRotation, 0);

                        Debug.Log("Angle interpolation is at t = " + thisSinerp + ", (" + (newTargetAngle * thisSinerp) + " /" + newTargetAngle + ")");

                        targetAngle = newTargetAngle;

                        yield return null;
                    }
                    Debug.Log("End of the interpolation, angle : " + targetAngle);
                    Debug.Log("True angle done : " + debugSum);
                    Debug.Log("---------------------------------------------");
                }
                else
                {
                    UpdatePosition(); // called here to avoir desynchronization

                    /* Test for collisions */

                    yield return null;
                }
            }
        }
    }

    private float LimitAngleToFirstObstacle(float targetAngle, Vector3 originalPosition)
    {
        float   startAngle    = 0; // degrees
        Vector3 startPosition = originalPosition,
                startForward  = startPosition - player.transform.position;

        float   endAngle    = 0; // degrees
        Vector3 endPosition = originalPosition,
                endForward  = endPosition - player.transform.position;

        int i_forward = 0;

        while (Mathf.Abs(startAngle) < Mathf.Abs(targetAngle)) // SIGN !!!!!
        {
            endAngle       = Mathf.Sign(targetAngle) > 0 ? Mathf.Min(startAngle + maxDegDelta, targetAngle) : Mathf.Max(startAngle - maxDegDelta, targetAngle);
            endForward     = Quaternion.Euler(0, endAngle - startAngle, 0) * startForward;
            endPosition    = player.transform.position + endForward;

            // Hit once, on the first obstacle met
            RaycastHit hit;
            if (Physics.Linecast(startPosition, endPosition, out hit, noCollision))
            {
                Vector3 obstaclePosition = hit.point;
                float distanceToObstacle = ArcLength(endForward.magnitude, Vector3.Angle(startPosition - player.transform.position, obstaclePosition - player.transform.position));

                int i_backward = 0;
                bool visibility = true;

                if (Physics.Linecast(startPosition, player.transform.position, out hit, noCollision)) // CHECK if childs should be on layer Play as well
                {
                    if (hit.transform.CompareTag("Player")){ visibility = true; }
                    else { visibility = false; /*Debug.Log("OCCLUSION WITH " + hit.transform.name + " INSIDE " + hit.transform.parent.name + " INSIDE " + hit.transform.parent.parent.name);*/}
                }

                // Backward tracking if too close to the obstacle, use arc-length for the distances

                while ((distanceToObstacle < minDistanceToObstacle
                ||      visibility == false)
                &&      Mathf.Abs(endAngle) < 180) // No backtrack under -180 or above 180 // startAngle or endAngle ??????
                {
                    // Backtrack to 0, SPECIAL CASE : WHAT HAPPEN WHEN EVEN 0 IS NOT ENOUGH TO RESPECT THE MINDISTANCE !!!!!!!!!!!!!!

                    endAngle    = Mathf.Sign(targetAngle) > 0 ? Mathf.Max(startAngle - maxDegDelta, -180) : Mathf.Min(startAngle + maxDegDelta, 180);
                    endForward  = Quaternion.Euler(0, endAngle - startAngle, 0) * startForward;
                    endPosition = player.transform.position + endForward;

                    distanceToObstacle = ArcLength(endForward.magnitude, Vector3.Angle(obstaclePosition - player.transform.position, endPosition - player.transform.position));

                    startAngle    = endAngle;
                    startForward  = endForward;
                    startPosition = endPosition;

                    // Check for occlusions

                    if (Physics.Linecast(startPosition, player.transform.position, out hit, noCollision)) // CHECK if childs should be on layer Play as well
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            visibility = true;
                        }
                        else
                        {
                            visibility = false;
                            Debug.Log("OCCLUSION");
                        }
                    }

                    //Debug.Log("One node backtracked " + startAngle + "/" + targetAngle); i_backward++;
                }
                Debug.Log("Crossed " + i_forward + " nodes but then backtracked " + i_backward + " nodes from the hit obstacle");
                return startAngle; // CHECK : Should it be startAngle or endAngle ?
            }

            i_forward++;
            //Debug.Log("One node crossed : " + endAngle + "/" + targetAngle);

            // Debug.DrawLine(startPosition, endPosition, Color.blue, 2f, false);

            // Start is now End

            startAngle = endAngle;
            startForward = endForward;
            startPosition = player.transform.position + startForward; // height ???? it's a the ground level not at the camera's height !!!

            /* If reached the target without a single collision,
             * we still have to check for minDistance */

            if (Mathf.Approximately(startAngle, targetAngle))
            {
                Debug.Log("Reached the targetAngle without any obstacle, now looking-forward");

                float forwardCheckingAngle      = Mathf.Sign(targetAngle) * InverseArcLength(startForward.magnitude, minDistanceToObstacle); // SIGN !!!!!!!!!!!!!!!!!!!!! / Should increment on startAngle ?
                Vector3 forwardCheckingForward  = Quaternion.Euler(0, forwardCheckingAngle, 0) * startForward;
                Vector3 forwardCheckingPosition = player.transform.position + forwardCheckingForward;

                // Do a forward-checking to guarantee the minDistance

                int i_lf_backward = 0;
                float distanceToObstacle = Mathf.Infinity;
                Vector3 obstaclePosition = Vector3.positiveInfinity;

                if (Physics.Linecast(startPosition, forwardCheckingPosition, out hit, noCollision))
                {
                    Debug.Log("Hit obstacle during looking-forward");

                    obstaclePosition = hit.point;
                    distanceToObstacle = ArcLength(startForward.magnitude, Vector3.Angle(obstaclePosition - player.transform.position, startPosition - player.transform.position));
                }

                bool visibility = true;

                if (Physics.Linecast(startPosition, player.transform.position, out hit, noCollision)) // CHECK if childs should be on layer Play as well
                {
                    if(hit.transform.CompareTag("Player"))
                    {
                        visibility = true;
                    }
                    else
                    {
                        visibility = false;
                        Debug.Log("OCCLUSION");
                    }
                }

                // If position isn't available, start a backtracking

                while ((distanceToObstacle < minDistanceToObstacle
                      || visibility == false)
                      && Mathf.Abs(endAngle) < 180) // SIGN !!! No backtrack under -180 or above 180 // startAngle or endAngle ??????
                {
                    // Backtrack to 0, SPECIAL CASE : WHEN EVEN 0 IS NOT ENOUGH TO RESPECT THE MINDISTANCE !!!!!!!!!!!!!! WE CAN'T GO UNDER 0 BECAUSE WE DON'T KNOW IF THERE ARE OBSTACLES THERE
                    i_lf_backward++;

                    endAngle = Mathf.Sign(targetAngle) > 0 ? Mathf.Max(startAngle - maxDegDelta, -180) : Mathf.Min(startAngle + maxDegDelta, 180); // 180 degrees beyond 0
                    endForward = Quaternion.Euler(0, endAngle - startAngle, 0) * startForward;
                    endPosition = player.transform.position + endForward;

                    if (distanceToObstacle != Mathf.Infinity)
                        distanceToObstacle = ArcLength(endForward.magnitude, Vector3.Angle(obstaclePosition - player.transform.position, startPosition - player.transform.position)); // CHECK : startPosition or endPosition ???

                    startAngle = endAngle;
                    startForward = endForward;
                    startPosition = endPosition;

                    // Check for occlusions

                    if (Physics.Linecast(startPosition, player.transform.position, out hit, noCollision)) // CHECK if childs should be on layer Play as well
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            visibility = true;
                        }
                        else
                        {
                            visibility = false;
                            Debug.Log("OCCLUSION");
                        }
                    }

                    // If angle has backtracked to the other side, check for collision on this unknown side
                    // No need to check for minDistance as we have already checked every other solution
                    // So on the first opposite obstacle, we know we have no solutions left

                    if ((endAngle < 0 && targetAngle > 0) || (endAngle > 0 && targetAngle < 0))
                    {
                        if (Physics.Linecast(startPosition, endPosition, out hit, noCollision))
                        {
                            // no solution, return default angle
                            Debug.Log("No solution for visibility, reseting angle to 0");
                            return 0;
                        }
                    }
                }

                Debug.Log("Crossed " + i_forward + " nodes and backtracked " + i_lf_backward + " nodes from the obstacle");
                return startAngle; // should it be startAngle or endAngle ?
            }

            // END
        }

        return targetAngle;
    }

    float Sinerp(float x)
    {
        if (!(0 <= x && x <= 1))
        {
            throw new System.Exception("Sinerp argument must be in range [0-1] : " + x);
        }
        return Mathf.Sin(x * Mathf.PI / 2f);
    }

    float InverseSinerp(float x)
    {
        return Mathf.Asin(Mathf.Clamp(x, -1.0f, 1.0f)) * 2f / Mathf.PI; // get the original [0-1] from the smooth [0-1] updated with the angle modification, need clamping because Asin's domain is [-1,1]
    }

    float ArcLength(float radius, float angle)
    {
        return 2f * Mathf.PI * radius * (angle / 360f);
    }

    float InverseArcLength(float radius, float arcLength)
    {
        return (360f * arcLength) / (2f * Mathf.PI * radius);
    }

    void LookAround(Vector2 v)
    {
        float smoothx = 0;
        float smoothy = 0;

        //Debug.Log("Accumulator : " + accumulator);

        if (!Mathf.Approximately(v.x, 0))
        {
            accumulator.x = Mathf.Clamp(accumulator.x + v.x * Time.deltaTime / horizontalDuration, -1, 1);
            smoothx = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
        }
        else
        {
            accumulator.x = (1 - Mathf.Sign(accumulator.x)) / 2f * Mathf.Min(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0) + (1 + Mathf.Sign(accumulator.x)) / 2f * Mathf.Max(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0);
            smoothx = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
        }

        if (!Mathf.Approximately(v.y, 0))
        {
            accumulator.y = Mathf.Clamp(accumulator.y + v.y * Time.deltaTime / verticalDuration, -1, 1);
            smoothy = Mathf.Sign(accumulator.y) * Mathf.Sin(Mathf.Abs(accumulator.y) * Mathf.PI * 0.5f);
        }
        else
        {
            accumulator.y = (1 - Mathf.Sign(accumulator.y)) / 2f * Mathf.Min(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0) + (1 + Mathf.Sign(accumulator.y)) / 2f * Mathf.Max(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0);
            smoothy = Mathf.Sign(accumulator.y) * Mathf.Sin(Mathf.Abs(accumulator.y) * Mathf.PI * 0.5f);
        }

        // Stabilization of the look around
        float y_stabilization = 0;
        if      (cameraState == STATE.NORMAL)          { y_stabilization = Mathf.Abs(smoothx) * -angleFromNormalToHorizon; }

        else if (cameraState == STATE.HURRY)           { y_stabilization = Mathf.Abs(smoothx) * -angleFromHurryToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_HURRY) { y_stabilization = Mathf.Abs(smoothx) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToHurry - angleFromHurryToHorizon * (timeNormalToHurry - zoomTimer) / timeNormalToHurry); }
        else if (cameraState == STATE.HURRY_TO_NORMAL) { y_stabilization = Mathf.Abs(smoothx) * (-angleFromHurryToHorizon * zoomTimer / timeHurryToNormal - angleFromNormalToHorizon * (timeHurryToNormal - zoomTimer) / timeHurryToNormal); ; }

        else if (cameraState == STATE.PROTECTED)           { y_stabilization = Mathf.Abs(smoothx) * -angleFromProtectedToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_PROTECTED) { y_stabilization = Mathf.Abs(smoothx) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToProtected - angleFromProtectedToHorizon * (timeNormalToProtected - zoomTimer) / timeNormalToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_NORMAL) { y_stabilization = Mathf.Abs(smoothx) * (-angleFromProtectedToHorizon * zoomTimer / timeProtectedToNormal - angleFromNormalToHorizon * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal); }

        else if (cameraState == STATE.HURRY_TO_PROTECTED) { y_stabilization = Mathf.Abs(smoothx) * (-angleFromHurryToHorizon * zoomTimer / timeHurryToProtected - angleFromProtectedToHorizon * (timeHurryToProtected - zoomTimer) / timeHurryToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_HURRY) { y_stabilization = Mathf.Abs(smoothx) * (-angleFromProtectedToHorizon * zoomTimer / timeProtectedToHurry - angleFromHurryToHorizon * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry); }

        // Must be separated in two because unity's order for euler is ZYX and we want X-Y-X
        //heldCamera.transform.localRotation = Quaternion.Euler(-smoothy * maxLookAroundAngle, smoothx * maxLookAroundAngle, 0);
        heldCamera.transform.localRotation  = Quaternion.Euler(y_stabilization, 0, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(0, smoothx * maxHorizontalAngle, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(-smoothy * maxVerticalAngle, 0, 0);
    }

    void SmoothLookAround(Vector2 inputVector)
    {
        if (!Mathf.Approximately(inputVector.x, 0))
        {
            if (lastInputVector.x == 0 || inputVector.x < 0 && lastInputVector.x > 0 || inputVector.x > 0 && lastInputVector.x < 0)
            {
                accumulator.x = smoothAccumulator.x;
                startAccumulator.x = accumulator.x;
            }
            else
            {
                // WHY HERE ?
                accumulator.x = Mathf.Clamp(accumulator.x + inputVector.x * Time.deltaTime / horizontalDuration, -1, 1);
            }

            if (!Mathf.Approximately(Mathf.Sign(inputVector.x), startAccumulator.x)) // avoid dividing by zero
            {
                smoothAccumulator.x = Mathf.SmoothStep(startAccumulator.x, Mathf.Sign(inputVector.x), Mathf.Abs(accumulator.x - startAccumulator.x) / Mathf.Abs((Mathf.Sign(inputVector.x) - startAccumulator.x)));
            }
            else
            {
                smoothAccumulator.x = 1.0f;
            }

            lastInputVector.x = inputVector.x;
        }
        else
        {
            if (accumulator.x == 0)
            {
                startAccumulator.x = 0;
            }
            if (lastInputVector.x != 0)
            {
                accumulator.x = smoothAccumulator.x;
                startAccumulator.x = accumulator.x;
            }
            else
            {
                // WHY HERE ?
                accumulator.x = (1 - Mathf.Sign(accumulator.x)) / 2f * Mathf.Min(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0) + (1 + Mathf.Sign(accumulator.x)) / 2f * Mathf.Max(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0);
                }

            if (startAccumulator.x != 0)
            {
                smoothAccumulator.x = Mathf.SmoothStep(startAccumulator.x, 0, Mathf.Abs(accumulator.x - startAccumulator.x) / Mathf.Abs(startAccumulator.x));
            }
            else
            {
                smoothAccumulator.x = 0.0f;
            }

            lastInputVector.x = 0;
        }

        if (!Mathf.Approximately(inputVector.y, 0))
        {
            accumulator.y = Mathf.Clamp(accumulator.y + inputVector.y * Time.deltaTime / verticalDuration, -1, 1);

            if (lastInputVector.y == 0 || inputVector.y < 0 && lastInputVector.y > 0 || inputVector.y > 0 && lastInputVector.y < 0)
            {
                accumulator.y = smoothAccumulator.y;
                startAccumulator.y = accumulator.y;
            }

            if (!Mathf.Approximately(Mathf.Sign(inputVector.y), startAccumulator.y)) // avoid dividing by zero
            {
                smoothAccumulator.y = Mathf.SmoothStep(startAccumulator.y, Mathf.Sign(inputVector.y), Mathf.Abs(accumulator.y - startAccumulator.y) / Mathf.Abs((Mathf.Sign(inputVector.y) - startAccumulator.y)));
            }
            else
            {
                smoothAccumulator.y = 1.0f;
            }
            lastInputVector.y = inputVector.y;
        }
        else
        {
            if (accumulator.y == 0)
            {
                startAccumulator.y = 0;
            }

            if (lastInputVector.y != 0)
            {
                accumulator.y = smoothAccumulator.y;
                startAccumulator.y = accumulator.y;
            }

            if (startAccumulator.y != 0)
            {
                smoothAccumulator.y = Mathf.SmoothStep(startAccumulator.y, 0, Mathf.Abs(accumulator.y - startAccumulator.y) / Mathf.Abs(startAccumulator.y));
            }
            else
            {
                smoothAccumulator.y = 0.0f;
            }

            accumulator.y = (1 - Mathf.Sign(accumulator.y)) / 2f * Mathf.Min(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0) + (1 + Mathf.Sign(accumulator.y)) / 2f * Mathf.Max(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0);

            lastInputVector.y = 0;
        }

        // Stabilization of the look around
        float y_stabilization = 0;
        if (cameraState == STATE.NORMAL) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * -angleFromNormalToHorizon; }

        else if (cameraState == STATE.HURRY) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * -(angleFromHurryToHorizon - angleFromNormalToHorizon); }
        else if (cameraState == STATE.NORMAL_TO_HURRY) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToHurry - (angleFromHurryToHorizon - angleFromNormalToHorizon) * (timeNormalToHurry - zoomTimer) / timeNormalToHurry); }
        else if (cameraState == STATE.HURRY_TO_NORMAL) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-(angleFromHurryToHorizon - angleFromNormalToHorizon) * zoomTimer / timeHurryToNormal - angleFromNormalToHorizon * (timeHurryToNormal - zoomTimer) / timeHurryToNormal); }

        else if (cameraState == STATE.PROTECTED) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * -(angleFromProtectedToHorizon - angleFromNormalToHorizon); }
        else if (cameraState == STATE.NORMAL_TO_PROTECTED) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToProtected - (angleFromProtectedToHorizon - angleFromNormalToHorizon) * (timeNormalToProtected - zoomTimer) / timeNormalToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_NORMAL) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-(angleFromProtectedToHorizon - angleFromNormalToHorizon) * zoomTimer / timeProtectedToNormal - angleFromNormalToHorizon * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal); }

        else if (cameraState == STATE.HURRY_TO_PROTECTED) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-(angleFromHurryToHorizon - angleFromNormalToHorizon) * zoomTimer / timeHurryToProtected - (angleFromProtectedToHorizon - angleFromNormalToHorizon) * (timeHurryToProtected - zoomTimer) / timeHurryToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_HURRY) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-(angleFromProtectedToHorizon - angleFromNormalToHorizon) * zoomTimer / timeProtectedToHurry - (angleFromHurryToHorizon - angleFromNormalToHorizon) * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry); }

        // Must be separated in two because unity's order for euler is ZYX and we want X-Y-X
        //heldCamera.transform.localRotation = Quaternion.Euler(-smoothy * maxLookAroundAngle, smoothx * maxLookAroundAngle, 0);
        
        heldCamera.transform.localRotation = Quaternion.Euler(y_stabilization, 0, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(0, smoothAccumulator.x * maxHorizontalAngle, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(-smoothAccumulator.y * maxVerticalAngle, 0, 0);
    }

    void SmoothProjectiveLookAround(Vector2 inputVector)
    {
        // Projection
        float sinerpForward;
        if (inputVector != Vector2.zero)
        {
            projectiveAccumulator = Mathf.Clamp(projectiveAccumulator + Time.deltaTime / projectiveDuration, 0, 1);
            sinerpForward = Mathf.Sin(projectiveAccumulator * Mathf.PI * 0.5f);
        }
        else
        {
            projectiveAccumulator = Mathf.Clamp(projectiveAccumulator - Time.deltaTime / projectiveDuration, 0, 1);
            sinerpForward = Mathf.Sin(projectiveAccumulator * Mathf.PI * 0.5f);
        }

        if (!Mathf.Approximately(inputVector.x, 0))
        {
            if (lastInputVector.x == 0 || inputVector.x < 0 && lastInputVector.x > 0 || inputVector.x > 0 && lastInputVector.x < 0)
            {
                accumulator.x = smoothAccumulator.x;
                startAccumulator.x = accumulator.x;
            }
            else
            {
                // WHY HERE ?
                accumulator.x = Mathf.Clamp(accumulator.x + inputVector.x * Time.deltaTime / horizontalDuration, -1, 1);
            }

            if (!Mathf.Approximately(Mathf.Sign(inputVector.x), startAccumulator.x)) // avoid dividing by zero
            {
                smoothAccumulator.x = Mathf.SmoothStep(startAccumulator.x, Mathf.Sign(inputVector.x), Mathf.Abs(accumulator.x - startAccumulator.x) / Mathf.Abs((Mathf.Sign(inputVector.x) - startAccumulator.x)));
            }
            else
            {
                smoothAccumulator.x = 1.0f;
            }

            lastInputVector.x = inputVector.x;
        }
        else
        {
            if (accumulator.x == 0)
            {
                startAccumulator.x = 0;
            }
            if (lastInputVector.x != 0)
            {
                accumulator.x = smoothAccumulator.x;
                startAccumulator.x = accumulator.x;
            }
            else
            {
                // WHY HERE ?
                accumulator.x = (1 - Mathf.Sign(accumulator.x)) / 2f * Mathf.Min(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0) + (1 + Mathf.Sign(accumulator.x)) / 2f * Mathf.Max(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0);
            }

            if (startAccumulator.x != 0)
            {
                smoothAccumulator.x = Mathf.SmoothStep(startAccumulator.x, 0, Mathf.Abs(accumulator.x - startAccumulator.x) / Mathf.Abs(startAccumulator.x));
            }
            else
            {
                smoothAccumulator.x = 0.0f;
            }

            lastInputVector.x = 0;
        }

        if (!Mathf.Approximately(inputVector.y, 0))
        {
            accumulator.y = Mathf.Clamp(accumulator.y + inputVector.y * Time.deltaTime / verticalDuration, -1, 1);

            if (lastInputVector.y == 0 || inputVector.y < 0 && lastInputVector.y > 0 || inputVector.y > 0 && lastInputVector.y < 0)
            {
                accumulator.y = smoothAccumulator.y;
                startAccumulator.y = accumulator.y;
            }

            if (!Mathf.Approximately(Mathf.Sign(inputVector.y), startAccumulator.y)) // avoid dividing by zero
            {
                smoothAccumulator.y = Mathf.SmoothStep(startAccumulator.y, Mathf.Sign(inputVector.y), Mathf.Abs(accumulator.y - startAccumulator.y) / Mathf.Abs((Mathf.Sign(inputVector.y) - startAccumulator.y)));
            }
            else
            {
                smoothAccumulator.y = 1.0f;
            }
            lastInputVector.y = inputVector.y;
        }
        else
        {
            if (accumulator.y == 0)
            {
                startAccumulator.y = 0;
            }

            if (lastInputVector.y != 0)
            {
                accumulator.y = smoothAccumulator.y;
                startAccumulator.y = accumulator.y;
            }

            if (startAccumulator.y != 0)
            {
                smoothAccumulator.y = Mathf.SmoothStep(startAccumulator.y, 0, Mathf.Abs(accumulator.y - startAccumulator.y) / Mathf.Abs(startAccumulator.y));
            }
            else
            {
                smoothAccumulator.y = 0.0f;
            }

            accumulator.y = (1 - Mathf.Sign(accumulator.y)) / 2f * Mathf.Min(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0) + (1 + Mathf.Sign(accumulator.y)) / 2f * Mathf.Max(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0);

            lastInputVector.y = 0;
        }

        // Stabilization of the look around
        float y_stabilization = 0;
        if (cameraState == STATE.NORMAL) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * -angleFromNormalToHorizon; }

        if (cameraState == STATE.HURRY) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * -angleFromHurryToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_HURRY) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToHurry - angleFromHurryToHorizon * (timeNormalToHurry - zoomTimer) / timeNormalToHurry); }
        else if (cameraState == STATE.HURRY_TO_NORMAL) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromHurryToHorizon * zoomTimer / timeHurryToNormal - angleFromNormalToHorizon * (timeHurryToNormal - zoomTimer) / timeHurryToNormal); }

        else if (cameraState == STATE.PROTECTED) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * -angleFromProtectedToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_PROTECTED) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToProtected - angleFromProtectedToHorizon * (timeNormalToProtected - zoomTimer) / timeNormalToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_NORMAL) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromProtectedToHorizon * zoomTimer / timeProtectedToNormal - angleFromNormalToHorizon * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal); }

        else if (cameraState == STATE.HURRY_TO_PROTECTED) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromHurryToHorizon * zoomTimer / timeHurryToProtected - angleFromProtectedToHorizon * (timeHurryToProtected - zoomTimer) / timeHurryToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_HURRY) { y_stabilization = Mathf.Abs(smoothAccumulator.x) * (-angleFromProtectedToHorizon * zoomTimer / timeProtectedToHurry - angleFromHurryToHorizon * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry); }

        // Must be separated in two because unity's order for euler is ZYX and we want X-Y-X
        //heldCamera.transform.localRotation = Quaternion.Euler(-smoothy * maxLookAroundAngle, smoothx * maxLookAroundAngle, 0);
        heldCamera.transform.localRotation = Quaternion.Euler(y_stabilization, 0, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(0, smoothAccumulator.x * maxHorizontalAngle, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(-smoothAccumulator.y * maxVerticalAngle, 0, 0);

        // Projection
        heldCamera.transform.position = transform.position + (player.transform.position + Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) * projectiveOffset - transform.position) * sinerpForward;
    }

    void ProjectiveLookAround(float v)
    {
        float sinerpForward = 0;
        Debug.Log("V = " + v);
        if(v != 0)
        {
            projectiveAccumulator = Mathf.Clamp (projectiveAccumulator + Time.deltaTime / projectiveDuration, 0, 1);
            sinerpForward = Mathf.Sin(projectiveAccumulator * Mathf.PI * 0.5f);
        }
        else
        {
            projectiveAccumulator = Mathf.Clamp(projectiveAccumulator - Time.deltaTime / projectiveDuration, 0, 1);
            sinerpForward = Mathf.Sin(projectiveAccumulator * Mathf.PI * 0.5f);
        }
        //heldCamera.transform.position = (1-v)*transform.position + v* (player.transform.position) + (player.transform.forward * forwardDistance) * sinerpForward;
        heldCamera.transform.position = transform.position + (player.transform.position + player.transform.forward * projectiveDistance - transform.position) * sinerpForward;
    }

    void ExtendedLookAround(Vector2 v)
    {
        maxHorizontalAngle = 90; // needed
        float minHorizontalAngle = 45;

        float rotationSmooth = 0;
        float lateralSmooth  = 0;
        float forwardSmooth  = 0;

        float rightProjection = 5;
        float forwardProjection = 12;

        if (!Mathf.Approximately(v.x, 0))
        {
            accumulator.x = Mathf.Clamp(accumulator.x + v.x * Time.deltaTime / horizontalDuration, -1, 1);
            rotationSmooth = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
            //lateralSmooth = Mathf.Sign(accumulator.x) * (1 - Mathf.Sqrt(1 - Mathf.Pow(accumulator.x,2)));
            lateralSmooth = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
        }
        else
        {
            accumulator.x = (1 - Mathf.Sign(accumulator.x)) / 2f * Mathf.Min(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0) + (1 + Mathf.Sign(accumulator.x)) / 2f * Mathf.Max(accumulator.x - Mathf.Sign(accumulator.x) * Time.deltaTime / horizontalDuration, 0);
            rotationSmooth = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
            //lateralSmooth = Mathf.Sign(accumulator.x) * (1 - Mathf.Sqrt(1 - Mathf.Pow(accumulator.x,2)));
            lateralSmooth = Mathf.Sign(accumulator.x) * Mathf.Sin(Mathf.Abs(accumulator.x) * Mathf.PI * 0.5f);
        }

        if (!Mathf.Approximately(v.y, 0))
        {
            //inputs.Player.Walk.Disable();
            accumulator.y = Mathf.Clamp(accumulator.y + v.y * Time.deltaTime / verticalDuration, -1, 1);
            forwardSmooth = Mathf.Sign(accumulator.y) * Mathf.Sin(Mathf.Abs(accumulator.y) * Mathf.PI * 0.5f);
        }
        else
        {
            //inputs.Player.Walk.Enable();
            accumulator.y = (1 - Mathf.Sign(accumulator.y)) / 2f * Mathf.Min(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0) + (1 + Mathf.Sign(accumulator.y)) / 2f * Mathf.Max(accumulator.y - Mathf.Sign(accumulator.y) * Time.deltaTime / verticalDuration, 0);
            forwardSmooth = Mathf.Sign(accumulator.y) * Mathf.Sin(Mathf.Abs(accumulator.y) * Mathf.PI * 0.5f);
        }

        // Stabilization of the look around
        float y_stabilization = 0;
        if (cameraState == STATE.NORMAL) { y_stabilization = Mathf.Abs(rotationSmooth) * -angleFromNormalToHorizon; }

        else if (cameraState == STATE.HURRY) { y_stabilization = Mathf.Abs(rotationSmooth) * -angleFromHurryToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_HURRY) { y_stabilization = Mathf.Abs(rotationSmooth) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToHurry - angleFromHurryToHorizon * (timeNormalToHurry - zoomTimer) / timeNormalToHurry); }
        else if (cameraState == STATE.HURRY_TO_NORMAL) { y_stabilization = Mathf.Abs(rotationSmooth) * (-angleFromHurryToHorizon * zoomTimer / timeHurryToNormal - angleFromNormalToHorizon * (timeHurryToNormal - zoomTimer) / timeHurryToNormal); }

        else if (cameraState == STATE.PROTECTED) { y_stabilization = Mathf.Abs(rotationSmooth) * -angleFromProtectedToHorizon; }
        else if (cameraState == STATE.NORMAL_TO_PROTECTED) { y_stabilization = Mathf.Abs(rotationSmooth) * (-angleFromNormalToHorizon * zoomTimer / timeNormalToProtected - angleFromProtectedToHorizon * (timeNormalToProtected - zoomTimer) / timeNormalToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_NORMAL) { y_stabilization = Mathf.Abs(rotationSmooth) * (-angleFromProtectedToHorizon * zoomTimer / timeProtectedToNormal - angleFromNormalToHorizon * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal); }

        else if (cameraState == STATE.HURRY_TO_PROTECTED) { y_stabilization = Mathf.Abs(rotationSmooth) * (-angleFromHurryToHorizon * zoomTimer / timeHurryToProtected - angleFromProtectedToHorizon * (timeHurryToProtected - zoomTimer) / timeHurryToProtected); }
        else if (cameraState == STATE.PROTECTED_TO_HURRY) { y_stabilization = Mathf.Abs(rotationSmooth) * (-angleFromProtectedToHorizon * zoomTimer / timeProtectedToHurry - angleFromHurryToHorizon * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry); }

        // Must be separated in two because unity's order for euler is ZYX and we want X-Y-X
        //heldCamera.transform.localRotation = Quaternion.Euler(-smoothy * maxLookAroundAngle, smoothx * maxLookAroundAngle, 0);
        heldCamera.transform.localRotation = Quaternion.Euler(y_stabilization, 0, 0);
        heldCamera.transform.localRotation *= Quaternion.Euler(0, rotationSmooth * (minHorizontalAngle + forwardSmooth * (maxHorizontalAngle - minHorizontalAngle)), 0);
        //heldCamera.transform.localRotation *= Quaternion.Euler(-smoothy * maxVerticalAngle, 0, 0);

        heldCamera.transform.position = transform.position + transform.right * rightProjection * lateralSmooth + (player.transform.position + player.transform.forward * forwardProjection - transform.position) * forwardSmooth;
    }

    void UpdateRecoilPosition()
    {
        if (cameraState == STATE.NORMAL)
        {
            if (!isTargeting) //isAvailable ?
            {
                if (player.GetComponent<PlayerFirst>().IsProtectingEyes || player.GetComponent<PlayerFirst>().IsProtectingEars)
                {
                    Debug.Log("CAMERA DETECTED THAT PLAYER IS PROTECTING");
                    zoomTimer = timeNormalToProtected;
                    isAvailable = false;
                    cameraState = STATE.NORMAL_TO_PROTECTED;
                }

                else if (player.GetComponent<PlayerFirst>().IsHurry)
                {
                    zoomTimer = timeNormalToHurry;
                    isAvailable = false;
                    cameraState = STATE.NORMAL_TO_HURRY;
                }
            }
        }

        else if (cameraState == STATE.NORMAL_TO_HURRY)
        {
            Vector3 startPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position,Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * (timeNormalToHurry - zoomTimer) / timeNormalToHurry; // recreate original position
            Vector3 endPosition   = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * zoomTimer / timeNormalToHurry; // recreate original position
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToFocus - recoilTimer) / timeToFocus);
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeNormalToHurry - zoomTimer) / timeNormalToHurry);

            // Transition
            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                isAvailable = true;
                cameraState = STATE.HURRY;
            }
        }

        else if (cameraState == STATE.HURRY)
        {
            if (!isTargeting) // isAvailable ?
            {
                if (gameManager.GetComponent<GameManager>().IsGameOver)
                {
                    zoomTimer = timeHurryToGameover;
                    isAvailable = false;
                    cameraState = STATE.HURRY_TO_GAMEOVER;
                }

                else if (player.GetComponent<PlayerFirst>().IsProtectingEyes || player.GetComponent<PlayerFirst>().IsProtectingEars)
                {
                    zoomTimer = timeHurryToProtected;
                    isAvailable = false;
                    cameraState = STATE.HURRY_TO_PROTECTED;
                }
                else if (!player.GetComponent<PlayerFirst>().IsHurry)
                {
                    zoomTimer = timeHurryToNormal;
                    isAvailable = false;
                    cameraState = STATE.HURRY_TO_NORMAL;
                }
            }
        }

        else if (cameraState == STATE.HURRY_TO_NORMAL)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * (timeHurryToNormal - zoomTimer) / timeHurryToNormal; // recreate original position
            Vector3 endPosition   = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetHurry + Vector3.up * Y_OffsetHurry) * zoomTimer / timeHurryToNormal; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeHurryToNormal - zoomTimer) / timeHurryToNormal);
            
            // Transition
            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                isAvailable = true;
                cameraState = STATE.NORMAL;
            }
        }

        else if (cameraState == STATE.NORMAL_TO_PROTECTED)
        {
            Vector3 startPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * (timeNormalToProtected - zoomTimer) / timeNormalToProtected; // recreate original position
            Vector3 endPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * zoomTimer / timeNormalToProtected; // recreate original position
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToFocus - recoilTimer) / timeToFocus);
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeNormalToProtected - zoomTimer) / timeNormalToProtected);
            
            // Transition
            if (zoomTimer <= 0)
            {
                transform.position = endPosition;  // should wait one frame more
                isAvailable = true;
                cameraState = STATE.PROTECTED;
            }
        }

        else if (cameraState == STATE.PROTECTED)
        {
            if (!isTargeting) // isAvailable ?
            {
                if (gameManager.GetComponent<GameManager>().IsGameOver)
                {
                    zoomTimer = timeProtectedToGameover;
                    isAvailable = false;
                    cameraState = STATE.PROTECTED_TO_GAMEOVER;
                }

                else if (!player.GetComponent<PlayerFirst>().IsProtectingEyes && !player.GetComponent<PlayerFirst>().IsProtectingEars)
                {
                    if (player.GetComponent<PlayerFirst>().IsHurry)
                    {
                        zoomTimer = timeProtectedToHurry;
                        isAvailable = false;
                        cameraState = STATE.PROTECTED_TO_HURRY;
                    }
                    else
                    {
                        zoomTimer = timeProtectedToNormal;
                        isAvailable = false;
                        cameraState = STATE.PROTECTED_TO_NORMAL;
                    }
                }
            }
        }

        else if (cameraState == STATE.PROTECTED_TO_NORMAL)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * Z_OffsetProtected + Vector3.up * Y_OffsetProtected) * zoomTimer / timeProtectedToNormal; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeProtectedToNormal - zoomTimer) / timeProtectedToNormal);

            // Transition
            if (zoomTimer <= 0)
            {
                isAvailable = true;
                transform.position = endPosition; // should wait one frame more
                cameraState = STATE.NORMAL;
            }
        }

        else if (cameraState == STATE.HURRY_TO_PROTECTED)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetHurry - Z_OffsetProtected) + Vector3.up * (Y_OffsetHurry - Y_OffsetProtected)) * (timeHurryToProtected - zoomTimer) / timeHurryToProtected; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetHurry - Z_OffsetProtected) + Vector3.up * (Y_OffsetHurry - Y_OffsetProtected)) * zoomTimer / timeHurryToProtected; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeHurryToProtected - zoomTimer) / timeHurryToProtected);

            // Transition
            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                isAvailable = true;
                cameraState = STATE.PROTECTED;
            }
        }

        else if (cameraState == STATE.PROTECTED_TO_HURRY)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetProtected - Z_OffsetHurry) + Vector3.up * (Y_OffsetProtected - Y_OffsetHurry)) * (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetProtected - Z_OffsetHurry) + Vector3.up * (Y_OffsetProtected - Y_OffsetHurry)) * zoomTimer / timeProtectedToHurry; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeProtectedToHurry - zoomTimer) / timeProtectedToHurry);

            // Transition
            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                isAvailable = true;
                cameraState = STATE.HURRY;
            }
        }

        else if (cameraState == STATE.HURRY_TO_GAMEOVER)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetHurry - Z_OffsetGameover) + Vector3.up * (Y_OffsetHurry - Y_OffsetGameover)) * (timeHurryToGameover - zoomTimer) / timeHurryToGameover; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetHurry - Z_OffsetGameover) + Vector3.up * (Y_OffsetHurry - Y_OffsetGameover)) * zoomTimer / timeHurryToGameover; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeHurryToGameover - zoomTimer) / timeHurryToGameover);

            // Transition
            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                isAvailable = true;
                cameraState = STATE.GAMEOVER; // should be unused
            }
        }

        else if (cameraState == STATE.PROTECTED_TO_GAMEOVER)
        {
            Vector3 startPosition = transform.position + (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetProtected - Z_OffsetGameover) + Vector3.up * (Y_OffsetProtected - Y_OffsetGameover)) * (timeProtectedToGameover - zoomTimer) / timeProtectedToGameover; // recreate original position
            Vector3 endPosition = transform.position - (-Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized * (Z_OffsetProtected - Z_OffsetGameover) + Vector3.up * (Y_OffsetProtected - Y_OffsetGameover)) * zoomTimer / timeProtectedToGameover; // recreate original position
            zoomTimer = Mathf.Max(zoomTimer - Time.deltaTime, 0);
            //float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (timeToNormal - recoilTimer) / timeToNormal);
            transform.position = Vector3.Lerp(startPosition, endPosition, (timeProtectedToGameover - zoomTimer) / timeProtectedToGameover);

            // Transition
            if (zoomTimer <= 0)
            {
                transform.position = endPosition; // should wait one frame more
                isAvailable = true;
                cameraState = STATE.GAMEOVER;
            }
        }

        else if (cameraState == STATE.GAMEOVER)
        {
            transform.position += Vector3.up * flyAwaySpeed;
        }
    }

    /* Floating */

    void InitializeSway()
    {
        latitude      = Random.Range(0, 180);
        longitude     = Random.Range(0, 360);
        swayRadiusMin = swayNormalRadiusMin;
        swayRadiusMax = swayNormalRadiusMax;
        swayDurationMin = swayNormalDurationMin;
        swayDurationMax = swayNormalDurationMax;
        swayRadius    = Random.Range(swayRadiusMin, swayRadiusMax);
        swayDuration  = Random.Range(swayDurationMin, swayDurationMax);
        swayTimer     = swayDuration;

        originalSwayPosition = heldCamera.transform.localPosition;

        targetSwayPosition = heldCamera.transform.localPosition + heldCamera.transform.localRotation * new Vector3
        (
            swayRadius * Mathf.Sin(latitude * Mathf.Deg2Rad) * Mathf.Cos(longitude * Mathf.Deg2Rad),
            swayRadius * Mathf.Sin(latitude * Mathf.Deg2Rad) * Mathf.Sin(longitude * Mathf.Deg2Rad),
            swayRadius * Mathf.Cos(latitude * Mathf.Deg2Rad)
        );
    }
    IEnumerator Sway()
    {
        for (; ;)
        {
            if ((cameraState == STATE.HURRY || cameraState == STATE.NORMAL_TO_HURRY) && lastCameraState != STATE.HURRY && lastCameraState != STATE.NORMAL_TO_HURRY)
            {
                // immediately speed up
                float newSwayDuration = Random.Range(swayHurryDurationMin, swayHurryDurationMax);

                swayTimer *= (newSwayDuration / swayDuration); // rescale timer
                swayDuration = newSwayDuration;

                // for next iteration
                swayDurationMin = swayHurryDurationMin;
                swayDurationMax = swayHurryDurationMax;
                swayRadiusMin = swayHurryRadiusMin;
                swayRadiusMax = swayHurryRadiusMax;
                backToNormalRadiusSpeed = 2f;
                backToNormalRadiusAcceleration = 2.5f;
            }
            else if (cameraState == STATE.NORMAL && lastCameraState != STATE.NORMAL)
            {
                // for next iteration
                swayDurationMin = swayNormalDurationMin;
                swayDurationMax = swayNormalDurationMax;
                swayRadiusMin = swayNormalRadiusMin;
                swayRadiusMax = swayNormalRadiusMax;
            }

            swayTimer = Mathf.Max(swayTimer - Time.deltaTime, 0.0f);
            float smoothstep = Mathf.SmoothStep(0.0f, 1.0f, (swayDuration - swayTimer) / swayDuration);
            cameraSway.transform.localPosition = Vector3.Lerp(originalSwayPosition, targetSwayPosition, smoothstep);

            if (smoothstep == 1)
            {
                latitude  = Random.Range(0, 90) + 90 * (1 - Mathf.Sign(latitude - 90) / 2f);
                longitude = Random.Range(longitude - 90, longitude + 90) % 360;

                // Revenir progressivement à swayNormalRadiusMin, swayNormalRadiusMax
                // WARNING : Here we suppose that radius is greater in hurry than in normal, and conversely for the duration
                // Transition for the radius, but not for the duration
                backToNormalRadiusSpeed = Mathf.Max(backToNormalRadiusSpeed + Time.deltaTime * backToNormalRadiusAcceleration, 0.1f);
                swayRadiusMin   = Mathf.Max(swayRadiusMin   - Time.deltaTime * backToNormalRadiusSpeed, swayNormalRadiusMin);
                swayRadiusMax   = Mathf.Max(swayRadiusMax   - Time.deltaTime * backToNormalRadiusSpeed, swayNormalRadiusMax);
                //swayDurationMin = Mathf.Min(swayDurationMin + Time.deltaTime * backToNormalRadiusSpeed, swayNormalDurationMin);
                //swayDurationMax = Mathf.Min(swayDurationMax + Time.deltaTime * backToNormalRadiusSpeed, swayNormalDurationMax);

                swayRadius   = Random.Range(swayRadiusMin, swayRadiusMax);
                swayDuration = Random.Range(swayDurationMin, swayDurationMax);
                swayTimer    = swayDuration;

                originalSwayPosition = cameraSway.transform.localPosition;
                targetSwayPosition = heldCamera.transform.localPosition + heldCamera.transform.localRotation * new Vector3
                (
                    swayRadius * Mathf.Sin(latitude * Mathf.Deg2Rad) * Mathf.Cos(longitude * Mathf.Deg2Rad),
                    swayRadius * Mathf.Sin(latitude * Mathf.Deg2Rad) * Mathf.Sin(longitude * Mathf.Deg2Rad),
                    swayRadius * Mathf.Cos(latitude * Mathf.Deg2Rad)
                );
            }

            lastCameraState = cameraState;

            yield return null;
        }
    }

    /* Targeting */

    public void TargetingObstacle(GameObject target)
    {
        if (!isTargeting && isAvailable && !player.GetComponent<PlayerFirst>().IsProtectingEyes && !player.GetComponent<PlayerFirst>().IsProtectingEars)
        {
            //isAvailable = false;
            inputs.Player.ProtectEars.Disable();
            inputs.Player.ProtectEyes.Disable();
            isTargeting = true;
            targetingTimer = targetingDuration;

            targetPosition = target.transform.position;

            storedForward = heldCamera.transform.forward;
            storedRotation = heldCamera.transform.localRotation;
            initialParentRotation = this.transform.rotation;

            StartCoroutine("SlerpTo");
        }
    }

    IEnumerator SlerpTo()
    {
        startForward = storedForward;
        endForward = Vector3.ProjectOnPlane((targetPosition - heldCamera.transform.position).normalized, Vector3.up);

        while (targetingTimer > (targetingDuration - targetingPause) * 0.5f + targetingPause)
        {
            currentParentRotation = this.transform.rotation;
            //startForward = Quaternion.Inverse(currentParentRotation) * storedForward;
            startForward = Vector3.ProjectOnPlane(defaultForward.transform.forward, Vector3.up);
            endForward = Vector3.ProjectOnPlane((targetPosition - heldCamera.transform.position).normalized, Vector3.up); // Check if ok
            targetingTimer = Mathf.Max(targetingTimer - Time.deltaTime, (targetingDuration - targetingPause) * 0.5f + targetingPause);
            Vector3 current = Vector3.Slerp(startForward, endForward, ((targetingDuration - targetingPause) * 0.5f - (targetingTimer - ((targetingDuration - targetingPause) * 0.5f + targetingPause))) / ((targetingDuration - targetingPause) * 0.5f));
            //current = new Vector3(current.x, current.y, 0);
            /* Quaternion rotation = Quaternion.LookRotation(transform.up, -current)
                                * Quaternion.AngleAxis(90f, Vector3.right);
            heldCamera.transform.rotation = rotation; */
            //Quaternion look = Quaternion.LookRotation((new Vector3(current.x,current.y,0)).normalized, Vector3.up);
            heldCamera.transform.localRotation = Quaternion.Inverse(heldCamera.transform.parent.rotation) * Quaternion.LookRotation(current, Vector3.up);
            //heldCamera.transform.rotation = Quaternion.LookRotation(current, Vector3.up);
            //float angle = Vector3.Angle(Vector3.ProjectOnPlane(startForward, Vector3.up), Vector3.ProjectOnPlane(endForward, Vector3.up));
            //heldCamera.transform.rotation = Quaternion.Inverse(heldCamera.transform.parent.rotation) * Quaternion.Euler(0, -angle, 0);

            //heldCamera.transform.rotation = Quaternion.LookRotation(current);

            Debug.Log("startForward: " + startForward + ", endForward: " + endForward + ", currentForward: " + current);
            yield return null;
        }
        while (targetingTimer > (targetingDuration - targetingPause) * 0.5f)
        {
            targetingTimer = Mathf.Max(targetingTimer - Time.deltaTime, (targetingDuration - targetingPause) * 0.5f);
            heldCamera.transform.LookAt(targetPosition);
        }
        StartCoroutine("SlerpFrom");
    }

    IEnumerator SlerpFrom()
    {
        startForward = heldCamera.transform.forward;
        endForward = storedForward;

        while (targetingTimer > 0)
        {
            targetingTimer = Mathf.Max(targetingTimer - Time.deltaTime, 0);
            Vector3 current = Vector3.Slerp(startForward, endForward, ((targetingDuration - targetingPause) * 0.5f - targetingTimer) / ((targetingDuration - targetingPause) * 0.5f));
            heldCamera.transform.localRotation = Quaternion.Inverse(heldCamera.transform.parent.rotation) * Quaternion.LookRotation(current);
            yield return null;
        }
        isTargeting = false;
        inputs.Player.ProtectEars.Enable();
        inputs.Player.ProtectEyes.Enable();

        /* Let's try this here */
        /*
        if (!player.GetComponent<PlayerFirst>().IsDamaged)
        {
            StartCoroutine("Timer");
        }
        player.GetComponent<PlayerFirst>().IsDamaged = true;*/
    }

    // CHECK IF OK TO REMOVE THIS :
    /*
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3);
        player.GetComponent<PlayerFirst>().IsDamagedEyes = false;
    }*/

} //FINISH
