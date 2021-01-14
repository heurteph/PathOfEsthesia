using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour, IAnimable
{

    private Inputs inputs;

    [Space]
    [Header("Player settings")]
    [Space]

    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    [Tooltip("The camera from which the player's forward direction is given")]
    private GameObject cameraHolder;

    [Space]
    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float normalSpeed = 1;

    [SerializeField]
    [Tooltip("Speed when the character is losing energy")]
    [Range(1.0f, 10.0f)]
    private float hurrySpeed = 10;

    private float speed;

    [SerializeField]
    [Tooltip("Time in seconds to transition from hurry state to normal state")]
    [Range(0, 5)]
    private float delayToNormalState = 3; // s

    private float backToNormalSpeedTimer = 0; // s

    private bool isHurry;
    public bool IsHurry { get { return isHurry; } }

    private bool isProtectingEyes;
    public bool IsProtectingEyes { get { return isProtectingEyes; } set { isProtectingEyes = value; } }

    private bool isProtectingEars;
    public bool IsProtectingEars { get { return isProtectingEars; } set { isProtectingEars = value; } }

    [SerializeField]
    private Animator anim;
    public Animator Anim { get { return anim; } }

    private Vector3 movement;

    [SerializeField]
    private Transform groundedPosition;

    void Awake()
    {
        //    angle = player.transform.rotation.eulerAngles.y;
        inputs = InputsManager.Instance.Inputs;

        isHurry = false;
        isProtectingEyes = false;
        IsProtectingEars = false;

        movement = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = normalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        StickToGround();

        if (inputs.Player.Walk != null)
        {
            Walk(inputs.Player.Walk.ReadValue<Vector2>());
        }

        characterController.Move(movement);
        movement = Vector3.zero;
    }

    void OnEnable()
    {
        inputs.Player.Enable();
    }

    void OnDisable()
    {
        inputs.Player.Disable();
    }

    void Walk(Vector2 v)
    {
        if (v.magnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.Euler(0, cameraHolder.transform.rotation.eulerAngles.y + Mathf.Rad2Deg * Mathf.Atan2(v.x, v.y), 0);   // cartesian to polar, starting from the Y+ axis as it's the one mapped to the camera's forward, thus using tan-1(x,y) and not tan-1(y,x) / No rotationSpeed * Time.deltaTime as it takes absolute orientation
            movement += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * v.magnitude * speed * Time.deltaTime;  // projection normalized to have the speed independant from the camera angle

            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void StickToGround()
    {
        RaycastHit hit;
        LayerMask ground = LayerMask.GetMask("Ground");

        if (Physics.Raycast(groundedPosition.position, -Vector3.up, out hit, Mathf.Infinity, ground) || Physics.Raycast(groundedPosition.position, Vector3.up, out hit, Mathf.Infinity, ground))
        {
            movement = (hit.point - groundedPosition.position);
        }
        else
        {
            movement = Vector3.zero;
        }
    }

    public void Hurry(float energy)
    {
        isHurry = true;
        backToNormalSpeedTimer = delayToNormalState;
        if (speed != hurrySpeed)
        {
            StartCoroutine("TransitionToNormalSpeed");
        }
        speed = hurrySpeed;
    }

    IEnumerator TransitionToNormalSpeed()
    {
        while (backToNormalSpeedTimer > 0)
        {
            backToNormalSpeedTimer -= Time.deltaTime;
            yield return null;
        }
        backToNormalSpeedTimer = 0;
        speed = normalSpeed;
        isHurry = false;
    }

} // FINISH
