using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class ZoneManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The list of sounds to mute when quitting the park")]
    private GameObject[] parkSounds;

    [SerializeField]
    [Tooltip("The list of sounds to mute when entering the park")]
    private GameObject[] roadSounds;

    public enum ZONE { TUTORIAL, ROADS, PARK, RESIDENCES, DOWNTOWN, MARKET };
    private ZONE playerZone;
    public ZONE PlayerZone { get { return playerZone; } }

    // Start is called before the first frame update
    void Start()
    {
        playerZone = ZONE.TUTORIAL;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerZone = ZONE.PARK;

            // activate park sounds
            foreach (GameObject parkSound in parkSounds)
            {
                parkSound.GetComponent<PostWwiseEventObstacle>().Play();
            }
            // deactivate road sounds
            foreach (GameObject roadSound in roadSounds)
            {
                roadSound.GetComponent<PostWwiseEventObstacle>().Stop();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerZone = ZONE.ROADS;

        if (other.CompareTag("Player"))
        {
            // deactivate park sounds
            foreach (GameObject parkSound in parkSounds)
            {
                parkSound.GetComponent<PostWwiseEventObstacle>().Stop();
            }
            // activate road sounds
            foreach (GameObject roadSound in roadSounds)
            {
                roadSound.GetComponent<PostWwiseEventObstacle>().Play();
            }
        }
    }
}
