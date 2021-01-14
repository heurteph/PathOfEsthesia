using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class StreetUsersManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] streetUsers;
    public GameObject[] StreetUsers { get { return streetUsers; } set { streetUsers = value; } }

    private List<GameObject> availableUsers;

    [Header("Common Values")]
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

    [SerializeField]
    [Range(1f, 5f)]
    [Tooltip("The duration of the freeze")]
    private float freezeDuration = 3f;

    private Vector3 storagePoint = new Vector3(-641, 0, -175); 

    // Start is called before the first frame update
    void Start()
    {
        // Each user register to the event
        foreach(GameObject streetUser in streetUsers)
        {
            if(streetUser.GetComponent<StreetUser>() == null)
            {
                throw new System.Exception(streetUser.name + " doesn't contain a StreetUser script");
            }
            // Initialize common values
            streetUser.GetComponent<StreetUser>().CommonSet(acceleration, deceleration, decelerationObstacle, decelerationStop, freezeDuration);
            streetUser.GetComponent<StreetUser>().AvailableEvent += PushCar;
        }
        availableUsers = new List<GameObject>(streetUsers); // copy constructor
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PushCar(GameObject car)
    {
        // TO DO : Make sure it's not already in there before adding it
        car.transform.position = storagePoint;
        availableUsers.Add(car);
        Debug.Log("One car added to the pool, " + availableUsers.Count + " cars are available");
    }

    public GameObject PopCar()
    {
        if (availableUsers.Count < 1)
        {
            //throw new System.Exception("No available car to supply to this request");
            Debug.Log("No car available for this request");
            return null;
        }
        int randomIndex = Random.Range(0, availableUsers.Count);
        GameObject car = availableUsers[randomIndex];
        availableUsers.RemoveAt(randomIndex);
        Debug.Log("One car removed from the pool, " + availableUsers.Count + " cars are available");
        return car;
    }
}
