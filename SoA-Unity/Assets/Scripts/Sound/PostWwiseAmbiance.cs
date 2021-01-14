using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class PostWwiseAmbiance : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The play event of the park ambiance")]
    private AK.Wwise.Event parkAmbianceEventPlay;
    public AK.Wwise.Event ParkAmbianceEventPlay { get { return parkAmbianceEventPlay; } }

    [SerializeField]
    [Tooltip("The stop event of the park ambiance")]
    private AK.Wwise.Event parkAmbianceEventStop;
    public AK.Wwise.Event ParkAmbianceEventStop { get { return parkAmbianceEventStop; } }

    [SerializeField]
    [Tooltip("The play event of the city ambiance")]
    private AK.Wwise.Event cityAmbianceEventPlay;
    public AK.Wwise.Event CityAmbianceEventPlay { get { return cityAmbianceEventPlay; } }

    [SerializeField]
    [Tooltip("The stop event of the city ambiance")]
    private AK.Wwise.Event cityAmbianceEventStop;
    public AK.Wwise.Event CityAmbianceEventStop { get { return cityAmbianceEventStop; } }

    [SerializeField]
    [Tooltip("The play event of the shelter ambiance")]
    private AK.Wwise.Event shelterAmbianceEventPlay;
    public AK.Wwise.Event ShelterAmbianceEventPlay { get { return shelterAmbianceEventPlay; } }

    [SerializeField]
    [Tooltip("The stop event of the city ambiance")]
    private AK.Wwise.Event shelterAmbianceEventStop;
    public AK.Wwise.Event ShelterAmbianceEventStop { get { return shelterAmbianceEventStop; } }

    [SerializeField]
    [Tooltip("The ambiance the game starts with")]
    private AMBIANCE ambiance = AMBIANCE.NONE;

    // Start is called before the first frame update
    void Start()
    {
        if(parkAmbianceEventPlay == null)
        {
            throw new System.NullReferenceException("No parkAmbiance play sound event set on the script PostWwiseAmbiance");
        }
        if (cityAmbianceEventPlay == null)
        {
            throw new System.NullReferenceException("No cityAmbiance play sound event set on the script PostWwiseAmbiance");
        }
        if (parkAmbianceEventStop == null)
        {
            throw new System.NullReferenceException("No parkAmbiance stop sound event set on the script PostWwiseAmbiance");
        }
        if (cityAmbianceEventStop == null)
        {
            throw new System.NullReferenceException("No cityAmbiance stop sound event set on the script PostWwiseAmbiance");
        }

        switch (ambiance)
        {
            case AMBIANCE.PARK:
                CityAmbianceEventStop.Post(gameObject);
                ParkAmbianceEventPlay.Post(gameObject);
                break;
            case AMBIANCE.CITY:
                ParkAmbianceEventStop.Post(gameObject);
                CityAmbianceEventPlay.Post(gameObject);
                break;
            case AMBIANCE.SHELTER:
                ShelterAmbianceEventPlay.Post(gameObject);
                ParkAmbianceEventPlay.Post(gameObject);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
