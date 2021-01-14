using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionBehaviour : MonoBehaviour
{
    static int instanceCount = 0;

    [Space]
    [Header("Character Camera")]

    [SerializeField]
    [Tooltip("The offset to the head")]
    private Vector3 cameraOffset = Vector3.zero;
    // = new Vector(0, 0.3f, 0.2f); // Inside Offset
    // = new Vector(0, 0.4f, 0.45f); // Outside Offset

    [SerializeField]
    [Tooltip("The angle to the head")]
    private Vector3 cameraAngle = Vector3.zero;
    // = new Vector(0, 0, 0);    // Inside Angle
    // = new Vector(30, 180, 0); // Outside Angle

    [SerializeField]
    [Tooltip("The texture to render the view")]
    private RenderTexture targetTexture;

    private Camera visionCamera;
    private Transform headMesh;
    private GameObject head;
    //[SerializeField]
    //private GameObject characterMesh;

    [SerializeField]
    private LayerMask ignoreMask;

    [Space]
    [Header("Brightness Detector")]

    [SerializeField]
    [Tooltip("How many times per second the vision is updated (in Hz)")]
    private float refreshFrequency = 4;

    private enum FILTERMODE { BlackAndWhite, GreyLevels }
    [SerializeField]
    [Tooltip("Apply a black and white or a grey levels filter")]
    private FILTERMODE filterMode = FILTERMODE.BlackAndWhite;

    [SerializeField]
    [Range(0, 255)]
    [Tooltip("When using black and white filter, specify the brightness limit between black and white")]
    private float blackAndWhiteThreshold = 220f;

    [SerializeField]
    [Range(0,1)]
    [Tooltip("Brightness level limit before character starts feeling damage in normal mode")]
    private float normalBrightnessThreshold = 0.7f;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Brightness level limit before character starts feeling damage in protected mode")]
    private float protectedBrightnessThreshold = 0.8f;

    private float brightnessThreshold;
    public float BrightnessThreshold { get { return brightnessThreshold; } }

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Brightness level limit before character start feeling disconfort")]
    private float uncomfortableBrightnessThreshold = 0.6f;

    [SerializeField]
    [Range(0, 100)]
    [Tooltip("Damages applied to the character when threshold is reached")]
    private float brightnessDamage = 10f;

    [SerializeField]
    [Tooltip("Are we in the nighttime ?")]
    private bool nighttime = false;

    [Space]
    [Header("References")]

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject esthesia;

    [SerializeField]
    private EnergyBehaviour energyBehaviour;

    [SerializeField]
    private DebuggerBehaviour debuggerBehaviour;

    private delegate void BrightnessThresholdHandler(float b);
    private event BrightnessThresholdHandler brightnessThresholdEvent;

    private event BrightnessThresholdHandler uncomfortableThresholdEvent;

    public delegate void GrayscaleChangedHandler(GameObject sender, Texture2D t, float p);
    public event GrayscaleChangedHandler grayScaleChangedEvent;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if(esthesia.GetComponent<Animator>() == null)
        {
            throw new System.NullReferenceException("No Animator attached to Esthesia game object");
        }
        if (esthesia.GetComponent<EsthesiaAnimation>() == null)
        {
            throw new System.NullReferenceException("No Esthesia animation script attached to Esthesia game object");
        }
        brightnessThreshold = normalBrightnessThreshold;
        brightnessThresholdEvent += energyBehaviour.DecreaseEnergy;
        grayScaleChangedEvent += debuggerBehaviour.DisplayBrightness;

        InjectCameraToFBX(); // create the camera inside the script

        if (visionCamera.targetTexture != null)
        {
            StartCoroutine("UpdateVision");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //head.transform.position = headMesh.transform.position + cameraOffset;
        //head.transform.rotation = Quaternion.Euler(cameraAngle) * headMesh.transform.rotation;
    }

    private IEnumerator UpdateVision()
    {
        for (; ; )
        {
            Texture2D t2D = RenderTexturetoTexture2D(visionCamera.targetTexture);
            ComputeBrightnessAverage(t2D);

            yield return new WaitForSeconds(1f / refreshFrequency);
        }
    }

    private Texture2D RenderTexturetoTexture2D (RenderTexture rt)
    {
        RenderTexture.active = rt;
        Texture2D t2D = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        t2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        t2D.Apply();

        return t2D;
    }

    /* Many methods to compute brightness level of a frame */

    private void ComputeBrightnessAverage (Texture2D t2D)
    {
        float sum = 0;

        for (int i=0; i < t2D.width; i++)
        {
            for (int j=0; j< t2D.height; j++)
            {
                if (filterMode == FILTERMODE.BlackAndWhite)
                {
                    if (t2D.GetPixel(i, j).grayscale * 255 >= blackAndWhiteThreshold)
                    {
                        t2D.SetPixel(i, j, new Color(1, 1, 1));
                        sum++;
                    }
                    else
                    {
                        t2D.SetPixel(i, j, new Color(0, 0, 0));
                    }
                }
                else if (filterMode == FILTERMODE.GreyLevels)
                {
                    float grey = t2D.GetPixel(i, j).grayscale;
                    t2D.SetPixel(i, j, new Color(grey,grey,grey));
                    sum += grey;
                }
            }
        }

        if (nighttime)
        {
            if (sum >= uncomfortableBrightnessThreshold * t2D.width * t2D.height)
            {
                if (!player.GetComponent<PlayerFirst>().IsInsideShelter)
                {
                    player.GetComponent<PlayerFirst>().EyesUncomfortableSources++;
                }
            }
            else
            {
                player.GetComponent<PlayerFirst>().IsUncomfortableEyes = false;
            }

            if (sum >= brightnessThreshold * t2D.width * t2D.height)
            {
                if (!player.GetComponent<PlayerFirst>().IsInsideShelter)
                {
                    brightnessThresholdEvent(brightnessDamage);

                    // Handle animations
                    if (!player.GetComponent<PlayerFirst>().IsDamagedEars)
                    {
                        player.GetComponent<PlayerFirst>().EyesDamageSources++;
                        //player.GetComponent<PlayerFirst>().IsDamagedEyes = true;
                        // Set animation layer weight
                        //esthesia.GetComponent<EsthesiaAnimation>().SelectEyesDamageLayer();
                    }
                }
            }
        }

        t2D.Apply();
        grayScaleChangedEvent(this.gameObject, t2D, sum / (t2D.width * t2D.height));
    }

    public void CoverEyes()
    {
        brightnessThreshold = protectedBrightnessThreshold;
    }

    public void UncoverEyes()
    {
        brightnessThreshold = normalBrightnessThreshold;
    }

    private void InjectCameraToFBX()
    {
        headMesh = GameObject.FindWithTag("Head").transform;
        if(headMesh == null)
        {
            throw new System.Exception("Camera Character Error : No head element found");
        }
        head = new GameObject();
        head.name = "Camera";
        head.transform.position = headMesh.transform.position + transform.rotation * cameraOffset;
        head.transform.rotation = transform.rotation * Quaternion.Euler(cameraAngle);

        visionCamera = head.AddComponent<Camera>();
        visionCamera.nearClipPlane = 0.1f;
        visionCamera.targetTexture = targetTexture;
        visionCamera.cullingMask = ~ignoreMask;
        //visionCamera.targetDisplay = ++instanceCount;
        head.transform.SetParent(headMesh);
    }

    /* For user options */

    public void SetBrightnessDamage(float brightnessDamage)
    {
        this.brightnessDamage = brightnessDamage;
    }

} // FINISH
