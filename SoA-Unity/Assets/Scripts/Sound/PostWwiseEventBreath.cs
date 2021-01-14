using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEventBreath : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event MyEvent;

    [SerializeField]
    private GameObject player;

    private float breathPerMinute;
    private float breathPerMinuteNext;

    private const float transitionSpeedUp = 100f; //= 6f;
    private const float transitionSpeedDown = 100f; //= 2f;

    private const float breathPerMinuteIdle = 15;
    private const float breathPerMinuteRunning = 15;
    private const float breathPerMinuteHurry = 45;
    private const float breathPerMinuteProtected = 45;

    // Use this for initialization.
    void Start()
    {
        if (player == null)
        {
            throw new System.Exception("No reference to the player in the PostWwiseEventBreath script");
        }
        //player.GetComponent<EnergyBehaviour>().EnterDamageStateEvent += HoldBreath; // no breathing over the cry

        breathPerMinute = breathPerMinuteIdle;
        breathPerMinuteNext = breathPerMinute;
        StartCoroutine("Breath");
    }

    // Update is called once per frame.
    void LateUpdate()
    {
        // TO DO : Use events instead

        if (player.GetComponent<PlayerFirst>().IsHurry)
        {
            breathPerMinuteNext = breathPerMinuteHurry;
        }
        else if (player.GetComponent<PlayerFirst>().IsProtectingEars || player.GetComponent<PlayerFirst>().IsProtectingEyes)
        {
            breathPerMinuteNext = breathPerMinuteProtected;
        }
        else if (player.GetComponent<PlayerFirst>().IsRunning)
        {
            breathPerMinuteNext = breathPerMinuteRunning;
        }
        else
        {
            breathPerMinuteNext = breathPerMinuteIdle;
        }

        if (breathPerMinute < breathPerMinuteNext)
        {
            breathPerMinute = Mathf.Min(breathPerMinute + Time.deltaTime * transitionSpeedUp, breathPerMinuteNext);
        }
        else if (breathPerMinute > breathPerMinuteNext)
        {
            breathPerMinute = Mathf.Max(breathPerMinute - Time.deltaTime * transitionSpeedDown, breathPerMinuteNext);
        }
    }

    IEnumerator Breath()
    {
        for (; ; )
        {
            PlayBreathInSound();
            for (float timer = 0; timer < 0.5f * 60f / breathPerMinute; timer += Time.deltaTime) { yield return null; }
            while (player.GetComponent<PlayerFirst>().IsDamagedEyes || player.GetComponent<PlayerFirst>().IsDamagedEars) { yield return null; } // no breathing during a cry

            PlayBreathOutSound();
            for (float timer = 0; timer < 0.5f * 60f / breathPerMinute; timer += Time.deltaTime) { yield return null; }
            while (player.GetComponent<PlayerFirst>().IsDamagedEyes || player.GetComponent<PlayerFirst>().IsDamagedEars) { yield return null; } // no breathing during a cry
        }
    }

    public void PlayBreathInSound()
    {
        AKRESULT result = AkSoundEngine.SetSwitch("Respiration_Ex_or_In", "Inspire", gameObject);
        if (result == AKRESULT.AK_Fail)
        {
            throw new System.Exception("Could set the phase of the breath wwise sound");
        }
        MyEvent.Post(gameObject);
    }

    public void PlayBreathOutSound()
    {
        AKRESULT result = AkSoundEngine.SetSwitch("Respiration_Ex_or_In", "Expire", gameObject);
        if (result == AKRESULT.AK_Fail)
        {
            throw new System.Exception("Could set the phase of the breath wwise sound");
        }
        MyEvent.Post(gameObject);
    }
}