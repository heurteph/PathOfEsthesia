using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterManager : MonoBehaviour
{
    [Header("Shelters List")]

    [SerializeField]
    [Tooltip("List of the shelters' entrances")]
    private GameObject[] shelterOutsides;
    public GameObject[] ShelterOutsides { get { return shelterOutsides; } }

    [SerializeField]
    [Tooltip("List of the shelters' insides")]
    private GameObject[] shelterInsides;
    public GameObject[] ShelterInsides { get { return shelterInsides; } }

    [Space]
    [Header("Shelter Settings")]

    [SerializeField]
    [Tooltip("The maximum distance to open a door")]
    [Range(0.1f, 10f)]
    private float maxDistanceToDoor = 3.0f;
    public float MaxDistanceToDoor { get { return maxDistanceToDoor; } }

    [SerializeField]
    [Tooltip("The duration of a transition")]
    [Range(0.1f,5f)]
    private float transitionDuration = 2.0f;
    public float TransitionDuration { get { return transitionDuration; } }

    // Start is called before the first frame update
    void Start()
    {
        if(shelterOutsides.Length != shelterInsides.Length)
        {
            throw new System.SystemException("Shelter's entrances and exits list do not match together");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GoInside(GameObject o)
    {
        for(int i = 0; i < shelterOutsides.Length; i++)
        {
            if (Object.ReferenceEquals(o, shelterOutsides[i]))
            {
                return shelterInsides[i];
            }
            Debug.Log(o.name + " not equal to " + shelterOutsides[i]);
        }
        throw new System.SystemException(o.name + " not found in the shelter list");
    }

    public GameObject GoOutside(GameObject o)
    {
        for (int i = 0; i < shelterInsides.Length; i++)
        {
            if (Object.ReferenceEquals(o, shelterInsides[i]))
            {
                return shelterOutsides[i];
            }
        }
        throw new System.SystemException(o.name + " not found in the shelter list");
    }
}
