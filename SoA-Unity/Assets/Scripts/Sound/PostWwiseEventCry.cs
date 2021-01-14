using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEventCry : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event MyEvent;

    [SerializeField]
    private GameObject player;

    // Use this for initialization.
    void Start()
    {
        player.GetComponent<EnergyBehaviour>().EnterDamageStateEvent += PlayCrySound;
    }

    // Update is called once per frame.
    private void Update()
    {
        
    }

    public void PlayCrySound()
    {
        if (!player.GetComponent<PlayerFirst>().IsProtectingEars && !player.GetComponent<PlayerFirst>().IsProtectingEyes)
        {
            MyEvent.Post(gameObject);
        }
    }
}
