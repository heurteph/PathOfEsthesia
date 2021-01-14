using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsthesiaAnimation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The reference to the player")]
    private GameObject player;

    [SerializeField]
    [Tooltip("Duration of the transition from idle to walk in seconds")]
    [Range(0.01f, 0.5f)]
    private float idleToWalkDuration = 0.2f; // s

    [SerializeField]
    [Tooltip("Duration of the transition from walk to idle in seconds")]
    [Range(0.01f,0.5f)]
    private float walkToIdleDuration = 0.05f; // s

    [SerializeField]
    [Tooltip("Minimum delay in seconds between two damage animation")]
    [Range(1, 3)]
    private float delayBetweenDamages;
    private float delay = 0;

    [Header("Eyes set")]
    [Space]
    [SerializeField]
    [Tooltip("The model of the calm eyes")]
    private GameObject eyesCalm;
    [SerializeField]
    [Tooltip("The model of the happy eyes")]
    private GameObject eyesHappy;
    [SerializeField]
    [Tooltip("The model of the neutral eyes")]
    private GameObject eyesNeutral;
    [SerializeField]
    [Tooltip("The model of the pain eyes")]
    private GameObject eyesPain;
    [SerializeField]
    [Tooltip("The model of the worried eyes")]
    private GameObject eyesWorry;

    private void Awake()
    {
        InitializeAnimationLayers();
        GetComponent<Animator>().SetBool("IsDamageAvailable", true);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetEyesNeutral();

        if (player == null)
        {
            throw new System.NullReferenceException("No reference to the player set in FinishAnimation script");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FinishedDamageAnimation()
    {
        GetComponent<Animator>().SetBool("IsDamageAvailable", false);
        delay = delayBetweenDamages;
        StartCoroutine("StartNextDamageAnimationDelay");
        //player.GetComponent<PlayerFirst>().IsDamagedEars = false;
        //SelectAnimationLayer(0);
    }

    /*
    public void SelectBaseLayer()
    {
        SelectAnimationLayer(0);
    }

    public void SelectEyesDamageLayer()
    {
        SelectAnimationLayer(1);
    }

    public void SelectEarsDamageLayer()
    {
        SelectAnimationLayer(2);
    }

    private void SelectAnimationLayer(int index)
    {
        GetComponent<Animator>().SetLayerWeight(0, index == 0 ? 1 : 0);
        GetComponent<Animator>().SetLayerWeight(1, index == 1 ? 1 : 0);
        GetComponent<Animator>().SetLayerWeight(2, index == 2 ? 1 : 0);
    }
    */

    public void InitializeAnimationLayers()
    {
        GetComponent<Animator>().SetLayerWeight(0, 1);
        GetComponent<Animator>().SetLayerWeight(1, 0);
    }

    public void SelectIdleLayer()
    {
        if (GetComponent<Animator>().GetLayerWeight(1) == 1)
        {
            StartCoroutine("WalkToIdleTransition");
        }
    }

    public void SelectWalkLayer()
    {
        if (GetComponent<Animator>().GetLayerWeight(0) == 1)
        {
            StartCoroutine("IdleToWalkTransition");
        }
    }

    private IEnumerator IdleToWalkTransition()
    {
        float weight = 0;
        while(weight < 1)
        {
            weight = Mathf.Min(weight + Time.deltaTime / idleToWalkDuration, 1f);
            GetComponent<Animator>().SetLayerWeight(0, 1 - weight);
            GetComponent<Animator>().SetLayerWeight(1, weight);
            yield return null;
        }
    }

    private IEnumerator WalkToIdleTransition()
    {
        float weight = 0;
        while (weight < 1)
        {
            weight = Mathf.Min(weight + Time.deltaTime / walkToIdleDuration, 1f);
            GetComponent<Animator>().SetLayerWeight(0, weight);
            GetComponent<Animator>().SetLayerWeight(1, 1 - weight);
            yield return null;
        }
    }

    private IEnumerator StartNextDamageAnimationDelay()
    {
        while (delay > 0)
        {
            delay = Mathf.Max(delay - Time.deltaTime, 0);
            yield return null;
        }
        GetComponent<Animator>().SetBool("IsDamageAvailable", true);
    }

    private void SetEyesCalm()
    {
        eyesCalm.SetActive(true);
        eyesHappy.SetActive(false);
        eyesNeutral.SetActive(false);
        eyesPain.SetActive(false);
        eyesWorry.SetActive(false);
    }

    private void SetEyesHappy()
    {
        eyesCalm.SetActive(false);
        eyesHappy.SetActive(true);
        eyesNeutral.SetActive(false);
        eyesPain.SetActive(false);
        eyesWorry.SetActive(false);
    }

    private void SetEyesNeutral()
    {
        eyesCalm.SetActive(false);
        eyesHappy.SetActive(false);
        eyesNeutral.SetActive(true);
        eyesPain.SetActive(false);
        eyesWorry.SetActive(false);
    }

    private void SetEyesPain()
    {
        eyesCalm.SetActive(false);
        eyesHappy.SetActive(false);
        eyesNeutral.SetActive(false);
        eyesPain.SetActive(true);
        eyesWorry.SetActive(false);
    }

    private void SetEyesWorry()
    {
        eyesCalm.SetActive(false);
        eyesHappy.SetActive(false);
        eyesNeutral.SetActive(false);
        eyesPain.SetActive(false);
        eyesWorry.SetActive(true);
    }

} //FINISH
