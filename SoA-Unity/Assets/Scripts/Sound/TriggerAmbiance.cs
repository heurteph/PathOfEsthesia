using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AMBIANCE { PARK, CITY, SHELTER, NONE };

public class TriggerAmbiance : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The type of ambiance that is triggered when crossing this collider")]
    private AMBIANCE ambiance = AMBIANCE.NONE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.GetComponent<PostWwiseAmbiance>() == null)
            {
                throw new System.NullReferenceException("No PostWwiseAmbiance script set on the player");
            }

            if (ambiance == AMBIANCE.PARK)
            {
                other.GetComponent<PostWwiseAmbiance>().CityAmbianceEventStop.Post(other.gameObject);
                other.GetComponent<PostWwiseAmbiance>().ParkAmbianceEventPlay.Post(other.gameObject);
            }
            else if (ambiance == AMBIANCE.CITY)
            {
                other.GetComponent<PostWwiseAmbiance>().ParkAmbianceEventStop.Post(other.gameObject);
                other.GetComponent<PostWwiseAmbiance>().CityAmbianceEventPlay.Post(other.gameObject);
            }
        }
    }
}
