using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public interface IAnimable
{
    Animator Anim { get; }
    bool IsProtectingEyes { get; set; }
    bool IsProtectingEars { get; set; }
}
public class PlayerFirst : MonoBehaviour, IAnimable
{

    private Inputs inputs;

    private GameObject gameManager;

    [Space]
    [Header("Player Settings")]
    [Space]

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private CharacterController characterController;

    [Space]

    [SerializeField]
    [Range(1.0f, 20.0f)]
    [Tooltip("Speed of the character")]
    private float normalSpeed = 18;

    private float speed;

    [SerializeField]
    [Tooltip("Speed of turning right or left")]
    [Range(1.0f, 360.0f)]
    private float rotationSpeed = 150;

    [SerializeField]
    [Tooltip("Duration of the half-turn")]
    [Range(0.1f,10)]
    private float turnBackTime = 0.35f; // seconds

    [SerializeField]
    [Tooltip("Sensitivity of the Y-Axis to trigger the half-turn")]
    [Range(-1,0)]
    private float turnBackThreshold = -0.5f;

    [SerializeField]
    [Tooltip("Minimum Y-Axis value")]
    [Range(0, 1)]
    private float walkingForwardThreshold = 0.6f;

    [SerializeField]
    [Tooltip("Minimum X-Axis value")]
    [Range(0, 1)]
    private float turningAngleThreshold = 0.6f;

    [SerializeField]
    [Tooltip("Total angle of the joystick (in degrees) that trigger the turn back")]
    [Range(0, 180)]
    float turnBackAngle = 30; // degs

    [SerializeField]
    [Tooltip("Total angle of the joystick (in degrees) that trigger full forward movement")]
    [Range(0, 180)]
    float fullForwardAngle = 60; // degs

    private float steeringAngle;
    public float SteeringAngle { get { return steeringAngle; } set { steeringAngle = value; } }
    private bool isTurningBack;

    private bool turningBackPressed;

    [Space]
    [Header("Hurry State")]

    [SerializeField]
    [Range(1.0f, 100.0f)]
    [Tooltip("Speed when the character is losing energy")]
    private float hurrySpeed = 36;

    [SerializeField]
    [Tooltip("Delay in seconds to transition to normal state after the danger has passed")]
    [Range(0,30)]
    private float delayToNormalState  = 3; // s

    private float backToNormalSpeedTimer = 0; // s


    private bool isHurry;
    public bool IsHurry { get { return isHurry; } }

    private bool isProtectingEyes;
    public bool IsProtectingEyes { get { return isProtectingEyes; } set { isProtectingEyes = value; } }

    private bool isProtectingEars;
    public bool IsProtectingEars { get { return isProtectingEars; } set { isProtectingEars = value; } }

    private bool isDamagedEyes;
    public bool IsDamagedEyes { get { return isDamagedEyes; } set { isDamagedEyes = value; } }

    private bool isDamagedEars;
    public bool IsDamagedEars { get { return isDamagedEars; } set { isDamagedEars = value; } }

    private float eyesDamageSources;
    public float EyesDamageSources { get { return eyesDamageSources; } set { eyesDamageSources = value; } }

    private bool isRunning;
    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }

    private bool isUncomfortableEyes;
    public bool IsUncomfortableEyes { get { return isUncomfortableEyes; } set { isUncomfortableEyes = value; } }

    private bool isUncomfortableEars;
    public bool IsUncomfortableEars { get { return isUncomfortableEars; } set { isUncomfortableEars = value; } }

    private float eyesUncomfortableSources;
    public float EyesUncomfortableSources { get { return eyesUncomfortableSources; } set { eyesUncomfortableSources = value; } }

    private bool isInsideShelter;
    public bool IsInsideShelter {  get { return isInsideShelter; } set { isInsideShelter = value; } }

    [SerializeField]
    private Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField]
    private GameObject esthesia;

    private Vector3 movement;

    [Space]
    [Header("Ground Detector")]

    [SerializeField]
    private Transform groundLevelPosition;

    [SerializeField]
    private Transform raycastPosition;

    [SerializeField]
    private GameObject wwiseGameObjectFootstep;

    [SerializeField]
    private GameObject wwiseGameObjectBreath;

    [Header("Shelter")]
    [Space]

    [SerializeField]
    [Tooltip("The speed of the character when inside the shelter")]
    [Range(0, 10)]
    private float shelterSpeed = 6;
    public float ShelterSpeed { get { return shelterSpeed; } }

    void Awake()
    {
        steeringAngle = player.transform.rotation.eulerAngles.y;

        inputs = InputsManager.Instance.Inputs;

        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        if(gameManager == null)
        {
            throw new System.NullReferenceException("No GameManager found in the scene");
        }

        isTurningBack = false;
        isHurry = false;
        isProtectingEyes = false;
        isProtectingEars = false;
        isRunning = false;
        turningBackPressed = false;

        // Make sure we spawn at home
        isInsideShelter = true;

        isUncomfortableEyes = false;
        isUncomfortableEars = false;
        
        eyesDamageSources = 0;

        movement = Vector3.zero;

        if(wwiseGameObjectFootstep == null)
        {
            throw new System.Exception("Missing reference to a Wwise GameObject");
        }

        if (esthesia.GetComponent<Animator>() == null)
        {
            throw new System.NullReferenceException("No Animator attached to Esthesia game object");
        }
        if (esthesia.GetComponent<EsthesiaAnimation>() == null)
        {
            throw new System.NullReferenceException("No Esthesia animation script attached to Esthesia game object");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = normalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.GetComponent<GameManager>().IsGameOver)
        {
            anim.SetBool("isGameOver", true);

            // deactivate protections if used
            anim.SetBool("isProtectingEyes", false);
            anim.SetBool("isProtectingEars", false);
        }
        
        if (inputs.Player.enabled) // Compulsory, as Disabling or Enabling an Action also Enable the ActionGroup !!!
        {
            StickToGround();

            Walk(inputs.Player.Walk.ReadValue<Vector2>());

            Debug.Log("Movement : " + movement);
            characterController.Move(movement);
            movement = Vector3.zero;

            // Manage the two kinds of vision

            if (eyesDamageSources > 0) { isDamagedEyes = true; }
            else { isDamagedEyes = false; }
            eyesDamageSources = 0;

            if (eyesUncomfortableSources > 0) { isUncomfortableEyes = true; }
            else { isUncomfortableEyes = false; }
            eyesUncomfortableSources = 0;

            if (isDamagedEyes || isDamagedEars) // TO DO : Remove ???
            {
                //inputs.Player.Walk.Disable();
                //inputs.Player.ProtectEyes.Enable();
                //inputs.Player.ProtectEars.Enable();
                AKRESULT result;
                result = AkSoundEngine.SetSwitch("Court_Marche", "Court", wwiseGameObjectFootstep); // Running step sounds
                result = AkSoundEngine.SetSwitch("Court_Marche", "Court", wwiseGameObjectBreath); // Running step sounds
            }

            if (isProtectingEyes || isProtectingEars)
            {
                //inputs.Player.Walk.Enable();
                isDamagedEyes = false; // To check
                isDamagedEars = false; // To check
                AKRESULT result;
                result = AkSoundEngine.SetSwitch("Court_Marche", "Court", wwiseGameObjectFootstep); // Running step sounds
                result = AkSoundEngine.SetSwitch("Court_Marche", "Court", wwiseGameObjectBreath); // Running step sounds
            }

            if (!isDamagedEyes && !isDamagedEars) // TO DO : Remove ???
            {
                inputs.Player.Walk.Enable();
            }

            anim.SetBool("isProtectingEyes", isProtectingEyes);
            anim.SetBool("isProtectingEars", isProtectingEars);
            anim.SetBool("isDamagedEyes", isDamagedEyes);
            anim.SetBool("isDamagedEars", isDamagedEars);
            anim.SetBool("isUncomfortableEyes", isUncomfortableEyes);
            anim.SetBool("isUncomfortableEars", isUncomfortableEars);
        }
        else // World-shelter transition
        {
            anim.SetBool("isWalking", false); // stop animation when warping
            // handle animation layer
            esthesia.GetComponent<EsthesiaAnimation>().SelectIdleLayer();

            AKRESULT result;
            result = AkSoundEngine.SetSwitch("Court_Marche", "Idle", wwiseGameObjectFootstep); // Idle step sounds
            result = AkSoundEngine.SetSwitch("Court_Marche", "Idle", wwiseGameObjectBreath); // Idle step sounds
        }
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
        if (!isTurningBack)
        {
            if (IsInTurnBackSector(v) && !turningBackPressed)  { turningBackPressed = true; StartCoroutine("TurnBack"); return; }
            if (turningBackPressed && v.y > turnBackThreshold) { turningBackPressed = false; }
            
            if (v.y > 0)
            {
                movement += player.transform.forward * Mathf.Clamp(v.y, walkingForwardThreshold, 1f) * speed * Time.deltaTime;
            }
            if (!IsInFullForwardSector(v) && !IsInTurnBackSector(v))
            {
                steeringAngle += Mathf.Sign(v.x) * Mathf.Clamp(Mathf.Abs(v.x), turningAngleThreshold, 1f) * rotationSpeed * Time.deltaTime; // TO DO : Minimum rotation speed threshold, just like the walkingForwardThreshold
                characterController.transform.rotation = Quaternion.Euler(0, steeringAngle, 0);
                if(v.y <= 0)
                {
                    movement += player.transform.forward * walkingForwardThreshold * speed * Time.deltaTime;
                }
            }

            if (v.magnitude == 0)
            {
                isRunning = false;
                anim.SetBool("isWalking", false);
                // handle animation layer
                esthesia.GetComponent<EsthesiaAnimation>().SelectIdleLayer();

                AKRESULT result;
                result = AkSoundEngine.SetSwitch("Court_Marche", "Idle", wwiseGameObjectFootstep); // Idle step sounds
                result = AkSoundEngine.SetSwitch("Court_Marche", "Idle", wwiseGameObjectBreath); // Idle step sounds
            } else
            {
                isRunning = true;
                anim.SetBool("isWalking", true);
                // handle animation layer
                esthesia.GetComponent<EsthesiaAnimation>().SelectWalkLayer();

                AKRESULT result;
                result = AkSoundEngine.SetSwitch("Court_Marche", "Marche", wwiseGameObjectFootstep); // Walking step sounds
                result = AkSoundEngine.SetSwitch("Court_Marche", "Marche", wwiseGameObjectBreath); // Walking step sounds
            }
        }
    }

    private bool IsInTurnBackSector(Vector2 point)
    {
        return point.y <= turnBackThreshold && Mathf.Abs(Mathf.Atan2(point.x, -point.y)) <= turnBackAngle / 2f * Mathf.Deg2Rad;
    }

    private bool IsInFullForwardSector(Vector2 point)
    {
        return Mathf.Abs(Mathf.Atan2(point.x, point.y)) <= fullForwardAngle / 2f * Mathf.Deg2Rad;
    }

    public void ResetTransform(Vector3 position, float angle)
    {
        transform.position = position;
        StickToGround();
        transform.position += movement;
        movement = Vector3.zero;
        steeringAngle = angle;
        characterController.transform.rotation = Quaternion.Euler(0, steeringAngle, 0);
    }

    void StickToGround()
    {
        if (raycastPosition && groundLevelPosition)
        {
            LayerMask ground = LayerMask.GetMask("AsphaltGround") | LayerMask.GetMask("GrassGround") | LayerMask.GetMask("ConcreteGround") | LayerMask.GetMask("SoilGround");

            if (Physics.Raycast(raycastPosition.position, -Vector3.up, out RaycastHit hit, Mathf.Infinity, ground))
            {
                movement = new Vector3(0, (hit.point - groundLevelPosition.position).y, 0);
            }
            else
            {
                movement = Vector3.zero;
            }
        }
    }

    IEnumerator TurnBack()
    {
        isTurningBack = true;

        // TO DO : verifiy these two
        isRunning = true;
        anim.SetBool("isWalking", true);

        // handle animation layer
        esthesia.GetComponent<EsthesiaAnimation>().SelectWalkLayer();

        AKRESULT result;
        result = AkSoundEngine.SetSwitch("Court_Marche", "Marche", wwiseGameObjectFootstep);
        result = AkSoundEngine.SetSwitch("Court_Marche", "Marche", wwiseGameObjectBreath);

        Vector3 beginForward =  transform.forward;
        Vector3 endForward   = -transform.forward;
        Vector3 newForward   = beginForward;

        float elapsedTime  = 0;

        while (elapsedTime != turnBackTime)
        {
            elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, turnBackTime);
            newForward = Vector3.RotateTowards(beginForward, endForward, Mathf.PI * elapsedTime / turnBackTime, 0.0f); // turn back = PI
            transform.rotation = Quaternion.LookRotation(newForward);
            yield return null;
        }
        steeringAngle -= 180; // synchronize with update
        isTurningBack = false;

        // BUG : Esthesia keep on walking when playing with the game pad
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
   
    public void SetShelterSpeed()
    {
        this.speed = shelterSpeed;
    }

    public void ResetSpeed()
    {
        this.speed = normalSpeed;
    }

} // FINISH
