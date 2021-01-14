using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class HearingScript : MonoBehaviour
{
    //const int sampleNumber = 1024; // 32768; // 2^15 number of samples for 0,743 seconds
    //const float sampleSeconds = 0.2f; //0.743f; // 32768 hertz / 48000

    //private int sampleTotal;
    //private float[] sampleData;

    //[SerializeField]
    //[Range(1, 5)]
    //private int hearingMultiplier = 1;

    [Header("Loudness Detector")]

    [SerializeField]
    [Tooltip("How many times per second the hearing is updated (in Hz)")]
    private float refreshFrequency = 4;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Loudness level limit before character starts feeling damage in protected mode")]
    private float normalLoudnessThreshold = 0.7f;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Loudness level limit before character starts feeling damage in protected mode")]
    private float protectedLoudnessThreshold = 0.8f;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Brightness level limit before character starts feeling discomfort")]
    private float uncomfortableLoudnessThreshold = 0.6f;

    private float loudnessThreshold;
    public float LoudnessThreshold { get { return loudnessThreshold; } set { loudnessThreshold = value; } }

    [SerializeField]
    [Range(0, 100)]
    private float loudnessDamage = 25;

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

    private delegate void LoudnessHandler(float b);
    private event LoudnessHandler LoudnessThresholdEvent;
    private event LoudnessHandler LoudnessUpdateEvent;

    private delegate void DamagingSourceHandler(GameObject o);
    private event DamagingSourceHandler DamagingSourceEvent;

    private AudioManager audioManager;

    // Awake Function
    void Awake()
    {
        //sampleTotal = hearingMultiplier * sampleNumber;
        //sampleData = new float[sampleTotal];
        
        loudnessThreshold = normalLoudnessThreshold;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            throw new System.NullReferenceException("The audio manager could not be loaded");
        }

        if (esthesia.GetComponent<Animator>() == null)
        {
            throw new System.NullReferenceException("No Animator attached to Esthesia game object");
        }
        if (esthesia.GetComponent<EsthesiaAnimation>() == null)
        {
            throw new System.NullReferenceException("No Esthesia animation script attached to Esthesia game object");
        }

        LoudnessThresholdEvent += energyBehaviour.DecreaseEnergy;
        LoudnessUpdateEvent += debuggerBehaviour.DisplayLoudness;

        // DamagingSourceEvent += cameraFollow.TargetingObstacle;

        //StartCoroutine("Hear");
        StartCoroutine("WwiseHear");
    }    

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    IEnumerator Hear()
    {
        for (; ;)
        {
            float loudness = 0f; 

            AudioListener.GetOutputData(sampleData, 0);
            //AudioListener.GetOutputData(sampleData+1024, 1);

            for (int i = 0; i < sampleTotal; i++)
            {
                float sample = sampleData[i];
                loudness += Mathf.Abs(sample);
            }

            loudness /= sampleTotal; //Average Volume
           
            LoudnessUpdateEvent(loudness);

            if (loudness >= loudnessThreshold)
            {
                LoudnessThresholdEvent(loudnessDamage);

                DamagingSourceEvent(ClosestAudioSource());
            }

            yield return new WaitForSeconds(sampleSeconds * hearingMultiplier);
        }
    }*/

    IEnumerator WwiseHear()
    {
        for (; ; )
        {
            float loudness = 0f;
            int type = 1;
            AKRESULT result;

            result = AkSoundEngine.GetRTPCValue("VolumeEcoutePerso", null, 0, out loudness, ref type);

            if(result == AKRESULT.AK_Fail)
            {
                throw new System.Exception("No input from Wwise Meter");
            }

            // remap loundess to [0-1] range
            loudness = 1 + loudness / 48.01278f;

            LoudnessUpdateEvent(loudness);

            if(loudness >= uncomfortableLoudnessThreshold)
            {
                if (!player.GetComponent<PlayerFirst>().IsInsideShelter)
                {
                    player.GetComponent<PlayerFirst>().IsUncomfortableEars = true;
                }
            }
            else
            {
                player.GetComponent<PlayerFirst>().IsUncomfortableEars = false;
            }

            if (loudness >= loudnessThreshold)
            {
                if (!player.GetComponent<PlayerFirst>().IsInsideShelter)
                {
                    // Handle energy loss
                    LoudnessThresholdEvent(loudnessDamage);

                    // Handle animation
                    if (!player.GetComponent<PlayerFirst>().IsDamagedEyes)
                    {
                        player.GetComponent<PlayerFirst>().IsDamagedEars = true;
                        // Set animation layer weight
                        //esthesia.GetComponent<EsthesiaAnimation>().SelectEarsDamageLayer();
                    }

                    //DamagingSourceEvent?.Invoke(ClosestAudioSource()); // more explicit test of existence needed
                }
            }
            else
            {
                player.GetComponent<PlayerFirst>().IsDamagedEars = false;
            }
            yield return new WaitForSeconds(1f / refreshFrequency);
        }
    }

    public void PlugEars()
    {
        loudnessThreshold = protectedLoudnessThreshold;
    }

    public void UnplugEars()
    {
        loudnessThreshold = normalLoudnessThreshold;
    }

    /* For user options */

    public void SetLoudnessDamage(float loudnessDamage)
    {
        this.loudnessDamage = loudnessDamage;
    }

    /*
    public GameObject ClosestAudioSource()
    {
        float minDistance = Mathf.Infinity;
        GameObject closestAudioSource = null;

        foreach (GameObject o in audioManager.GameObjectWithAudioSources)
        {
            if (o != null)
            {
                float distanceFromAudio = (o.transform.position - transform.parent.transform.parent.transform.position).magnitude;

                if (minDistance > distanceFromAudio)
                {
                    minDistance = distanceFromAudio;
                    closestAudioSource = o;
                }
            }
        }

        Debug.Log(closestAudioSource.transform.name + closestAudioSource.transform.position + " is the closest AudioSource");
        return closestAudioSource;
    }
    */

} // FINISH
