using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;

public class StreetUser : MonoBehaviour
{
    [SerializeField] [Tooltip("The spline used by the object")]
    private Spline spline;
    public Spline MySpline { get { return spline; } set { spline = value; } }

    [SerializeField]
    private bool onStart = true;

    [Header("Position")]
    [Space]

    [SerializeField] [Range(0, 1)] [Tooltip("Initial position on the spline (in percent)")]
    private float startPercentage = 0;
    private float percentage;
    public float Percentage { get { return percentage; } set { percentage = value; } }

    [SerializeField]
    [Tooltip("The position of the stop")]
    [Range(0, 1)]
    private float stopPercentage;
    private bool hasStopped;
    public bool HasStopped { get { return hasStopped; } }

    private float speed = 50f;
    public float Speed { get { return speed; } set { speed = value; } }

    private float frontSpeed;

    public float FrontSpeed { get { return frontSpeed; } set { frontSpeed = value; } }

    private float obstaclePercentage;
    public float ObstaclePercentage { get { return obstaclePercentage; } set { obstaclePercentage = value; } }

    private float intersectionPercentage;
    public float IntersectionPercentage { get { return intersectionPercentage; } set { intersectionPercentage = value; } }


    [Header("Speed")]
    [Space]

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("The speed of the object on a FAST section")]
    private float fastSpeed = 70f;

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("The speed of the object on a NORMAL section")]
    private float normalSpeed = 60f;

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("The speed of the object on a SLOW section")]
    private float slowSpeed = 20f;

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("The speed of the object on a CAUTIOUS section")]
    private float cautiousSpeed = 10f;

    [Space]

    [SerializeField]
    [Range(1f, 1000f)]
    [Tooltip("The acceleration of the object between two speeds")]
    private float acceleration = 10f;

    [SerializeField]
    [Range(1f, 1000f)]
    [Tooltip("The deceleration of the object between two speeds")]
    private float deceleration = 10f;

    [SerializeField]
    [Range(1f, 1000f)]
    [Tooltip("The deceleration of the object when behind another object")]
    private float decelerationObstacle = 50f;

    [SerializeField]
    [Range(1f, 1000f)]
    [Tooltip("The deceleration of the object to full stop")]
    private float decelerationStop = 50f;

    private Dictionary<SPEEDLIMIT, float> speedValues;

    [Header("Durations")]
    [Space]

    [SerializeField][Range(0f, 10f)][Tooltip("The duration of the pause")]
    private float stopDuration = 0;

    [SerializeField][Range(1f, 5f)][Tooltip("The duration of the pause")]
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

    [Header("VFX")]
    [Space]

    [SerializeField]
    [Tooltip("VFX to play when loud sound is emitted")]
    private GameObject loudVFX;

    public enum STATE { NORMAL, FREEZE, STAYBEHIND, STOPPING, STOP, PARKED, OFF }
    private STATE movingState;
    public STATE MovingState { get { return movingState; } set { movingState = value; } }

    /* Event */

    public delegate void AvailableHandler(GameObject sender);
    public event AvailableHandler AvailableEvent;


    private void Awake()
    {
        groundOffset = transform.position.y - groundLevel.transform.position.y;

        InitializeSound();
        
        movingState = STATE.OFF;

        Debug.Assert(loudVFX != null);
        loudVFX.GetComponent<ParticleSystem>().Stop();
    }
    private void Start()
    {
        //UserSet();
    }

    private void Update()
    {
        UpdateSound();

        if (movingState != STATE.FREEZE && movingState != STATE.OFF)
        {
            /* VFX sound */
            if (speed > 10 && loudVFX.GetComponent<ParticleSystem>().isStopped)
                loudVFX.GetComponent<ParticleSystem>().Play();
            else if (speed <= 10 && loudVFX.GetComponent<ParticleSystem>().isPlaying)
                loudVFX.GetComponent<ParticleSystem>().Stop();

            if (movingState == STATE.NORMAL)
            {
                /* FIND THE SPEED LIMIT OF THE ZONE */

                SPEEDLIMIT speedLimit = SPEEDLIMIT.NORMAL;

                SPEEDLIMIT lastZoneSpeedLimit = SPEEDLIMIT.NORMAL;
                float lastZonePercentage = 0;

                foreach (KeyValuePair<float, SPEEDLIMIT> nextSpeedZone in spline.GetComponent<StreetMap>().SpeedZones)
                {
                    if (percentage >= lastZonePercentage && percentage < nextSpeedZone.Key)
                    {
                        speedLimit = lastZoneSpeedLimit;//speedZone.Value;
                        //Debug.Log("Speed Zone : [" + lastZonePercentage + "," + nextSpeedZone.Key + "]");
                        break;
                    }
                    lastZonePercentage = nextSpeedZone.Key;
                    lastZoneSpeedLimit = nextSpeedZone.Value;
                }

                /* UPDATE SPEED */

                if (speed < speedValues[speedLimit])
                {
                    //speed = Mathf.Min(speed + acceleration * Time.deltaTime / (speedValues[speedLimit] - speed), speedValues[speedLimit]);
                    speed = Mathf.Min(speed + acceleration * Time.deltaTime, speedValues[speedLimit]);
                    Debug.Log("SPEED OF " + gameObject.name + " IS " + speed + " AND CURRENT LIMIT IS " + speedValues[speedLimit]);
                    //speed = speedValues[speedLimit];
                }
                else if (speed > speedValues[speedLimit])
                {
                    //speed = Mathf.Max(speed - deceleration * Time.deltaTime / (speed - speedValues[speedLimit]), speedValues[speedLimit]);
                    speed = Mathf.Max(speed - deceleration * Time.deltaTime, speedValues[speedLimit]);
                    //speed = speedValues[speedLimit];
                }
            }
            else if (movingState == STATE.STAYBEHIND)
            {
                /* UPDATE SPEED ACCORDING TO THE FRONT VEHICLE'S SPEED AND POSITION */

                // TO DO : Use obstaclePercentage to keep moving till it's not reached

                // Debug.Log(transform.name + "IN STAYBEHIND MODE");

                if (speed >= frontSpeed)
                {
                    //speed = frontSpeed;
                    speed = Mathf.Max(speed - (Mathf.Max(speed, decelerationObstacle) + 10 * (obstaclePercentage - percentage)) * Time.deltaTime, frontSpeed); // deceleration proportionnal to speed

                    // move towards the target
                    /*
                    if(speed == frontSpeed && obstaclePercentage > percentage)
                    {
                        speed += (obstaclePercentage - percentage) * Time.deltaTime;
                    }*/
                }
            }
            else if (movingState == STATE.STOP)
            {
                if (speed >= 0)
                {
                    //speed = 0;
                    speed = Mathf.Max(speed - (Mathf.Max(speed, decelerationStop) - 10 * (intersectionPercentage - percentage)) * Time.deltaTime, 0); // deceleration proportionnal to speed

                    // move towards the target
                    /*
                    if (speed == 0 && intersectionPercentage > percentage)
                    {
                        speed = (intersectionPercentage - percentage) * Time.deltaTime;
                    }*/
                }
            }

            /* UPDATE PERCENTAGE */

            //spline.CalculateLength();
            percentage = Mathf.Min(percentage + (speed * Time.deltaTime) / spline.Length, 1);
            Debug.Log(gameObject.name + "'S SPEED IS : " + speed + " AND TRUE SPEED IS " + speed * Time.deltaTime / spline.Length);
            Debug.Log(gameObject.name + "'S PERCENTAGE IS : " + percentage);

            /* UPDATE TRANSFORM */

            Vector3 position = spline.GetPosition(percentage);
            transform.position = StickToTheGround(position);
            transform.rotation = Quaternion.LookRotation(spline.GetDirection(percentage));

            // Debug.Log("PERCENTAGE : " + percentage + "!!!!!!!!!!!!!!!!!!!!!!!!!!");

            /* STOP CONDITION */

            /*
            if (percentage >= stopPercentage && !hasStopped)
            {
                hasStopped = true;
                if (stopDuration > 0)
                {
                    yield return new WaitForSeconds(stopDuration);
                }
            }*/

            /* OFF CONDITION */

            if (percentage == 1)
            {
                MovingState = STATE.OFF;

                //percentage = 0;
                hasStopped = false;

                /* Unregister to the previous spline */
                spline.GetComponent<StreetMap>().UnregisterUser(gameObject);

                /* Inform the StreetUsersManager that I am available */
                AvailableEvent(gameObject);
            }
        }
    }

    public void UserSet()
    {
        // Register to the spline
        spline.GetComponent<StreetMap>().RegisterUser(gameObject);
        spline.CalculateLength();

        // Set the initial position
        percentage = startPercentage;
        Vector3 position = spline.GetPosition(percentage);
        transform.position = StickToTheGround(position);

        // Set the initial rotation
        transform.rotation = Quaternion.LookRotation(spline.GetDirection(Mathf.Clamp(startPercentage, 0.01f, 0.99f), true)); // initial rotation

        // Set the speed according to the zones
        speedValues = new Dictionary<SPEEDLIMIT, float>
        {
            { SPEEDLIMIT.FAST, fastSpeed },
            { SPEEDLIMIT.NORMAL, normalSpeed },
            { SPEEDLIMIT.SLOW, slowSpeed },
            { SPEEDLIMIT.CAUTIOUS, cautiousSpeed }
        };

        // Set the initial speed

        SPEEDLIMIT lastZoneSpeedLimit = SPEEDLIMIT.NORMAL;
        float lastZonePercentage = 0;
        speed = 0;

        foreach (KeyValuePair<float, SPEEDLIMIT> nextSpeedZone in spline.GetComponent<StreetMap>().SpeedZones)
        {
            if (percentage >= lastZonePercentage && percentage < nextSpeedZone.Key)
            {
                speed = speedValues[lastZoneSpeedLimit];
                break;
            }
            lastZonePercentage = nextSpeedZone.Key;
            lastZoneSpeedLimit = nextSpeedZone.Value;
        }

        hasStopped = false;
        if (onStart) { movingState = STATE.NORMAL; }
        else { movingState = STATE.OFF; }
    }

    public void CommonSet(float acceleration, float deceleration, float decelerationObstacle, float decelerationStop, float freezeDuration)
    {
        this.acceleration = acceleration;
        this.deceleration = deceleration;
        this.decelerationObstacle = decelerationObstacle;
        this.decelerationStop = decelerationStop;
        this.freezeDuration = freezeDuration;
    }

    public void SpecificSet(Spline newSpline, float fastSpeed, float normalSpeed, float slowSpeed, float cautiousSpeed)
    {
        /* Register to the spline */
        this.spline = newSpline;
        spline.GetComponent<StreetMap>().RegisterUser(gameObject);
        //spline.CalculateLength();

        /* Set initial position */

        percentage = 0;
        Vector3 position = spline.GetPosition(percentage);
        transform.position = StickToTheGround(position);

        /* Set initial rotation */

        transform.rotation = Quaternion.LookRotation(spline.GetDirection(Mathf.Clamp(percentage, 0.01f, 0.99f), true)); // initial rotation

        /* Set the speeds according to the zones */

        this.fastSpeed = fastSpeed;
        this.normalSpeed = normalSpeed;
        this.slowSpeed = slowSpeed;
        this.cautiousSpeed = cautiousSpeed;

        speedValues = new Dictionary<SPEEDLIMIT, float>
        {
            { SPEEDLIMIT.FAST, this.fastSpeed },
            { SPEEDLIMIT.NORMAL, this.normalSpeed },
            { SPEEDLIMIT.SLOW, this.slowSpeed },
            { SPEEDLIMIT.CAUTIOUS, this.cautiousSpeed }
        };

        /* Set the initial speed */

        SPEEDLIMIT lastZoneSpeedLimit = SPEEDLIMIT.NORMAL;
        float lastZonePercentage = 0;
        speed = 0;

        foreach (KeyValuePair<float, SPEEDLIMIT> nextSpeedZone in spline.GetComponent<StreetMap>().SpeedZones)
        {
            if (percentage >= lastZonePercentage && percentage < nextSpeedZone.Key)
            {
                speed = speedValues[lastZoneSpeedLimit];
                break;
            }
            lastZonePercentage = nextSpeedZone.Key;
            lastZoneSpeedLimit = nextSpeedZone.Value;
        }

        hasStopped = false;
        movingState = STATE.NORMAL;
    }

    public void Trigger()
    {
        if (!onStart && movingState == STATE.OFF) { movingState = STATE.NORMAL; }
    }

    /* COUNTDOWNS */

    /*
    private IEnumerator FreezeCountdown()
    {
        yield return new WaitForSeconds(freezeDuration);
        movingState = STATE.NORMAL;
    }*/

    private IEnumerator CarHornCountdown()
    {
        float delay; // seconds
        float maxDelay = 3; // seconds
        while (movingState == STATE.FREEZE)
        {
            AkSoundEngine.PostEvent("Play_Klaxons", gameObject);
            loudVFX.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(0.5f); // How long last a car horn ? make a guess
            loudVFX.GetComponent<ParticleSystem>().Stop();

            maxDelay = Mathf.Max(maxDelay * 7f/8f, 0);
            delay = Mathf.Max(Random.Range(0, maxDelay), 0.2f); // driver gets annoyed
            yield return new WaitForSeconds(delay);
        }
    }

    /* SOUND RELATED FUNCTIONS */

    private void InitializeSound()
    {
        // Define which car horn type to use
        AkSoundEngine.SetSwitch("Klaxons", new string[5] { "A", "B", "C", "D", "E" }[Random.Range(0, 5)], gameObject);

        AKRESULT result = AkSoundEngine.SetRTPCValue("Quel_Moteur", Random.Range(0,9), gameObject);

        if (result == AKRESULT.AK_Fail)
        {
            throw new System.Exception("WWISE : Could not set the type of the engine");
        }

        /*
        result = AkSoundEngine.SetRTPCValue("Ralenti_Accelere", 1f, gameObject);

        if (result == AKRESULT.AK_Fail)
        {
            throw new System.Exception("WWISE : Could not set the state of the engine");
        }*/
    }

    private void UpdateSound()
    {
        AKRESULT result = AkSoundEngine.SetRTPCValue("VitesseVehicule", speed, gameObject);
        Debug.Log("SPEED IS AT : " + speed);

        //AKRESULT result = AkSoundEngine.SetSwitch("EtatVoiture","C_IDLE", gameObject);

        if (result == AKRESULT.AK_Fail)
        {
            throw new System.Exception("WWISE : Could not set the speed of the vehicule");
        }
    }

    /* PHYSICS RELATED FUNCTIONS */

    private Vector3 StickToTheGround(Vector3 position)
    {
        LayerMask mask = LayerMask.GetMask("AsphaltGround") | LayerMask.GetMask("GrassGround") | LayerMask.GetMask("SoilGround");
        if (Physics.Raycast(raycaster.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
        {
            return new Vector3(position.x, hit.point.y + groundOffset, position.z);
        }
        return position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (movingState != STATE.FREEZE)
            {
                movingState = STATE.FREEZE;
                speed = 0; // TO DO : Find a progressive way
                if (loudVFX.GetComponent<ParticleSystem>().isPlaying)
                {
                    loudVFX.GetComponent<ParticleSystem>().Stop();
                }
                StartCoroutine("CarHornCountdown");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            movingState = STATE.NORMAL;
        }
    }
}
