using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using System.Linq;

public enum SPEEDLIMIT { FAST, NORMAL, SLOW, CAUTIOUS }

[System.Serializable]
public struct Colinearity
{
    public float percentageStart;
    public float percentageEnd;
    public Spline otherSpline;
    public float otherPercentageStart;
    public float otherPercentageEnd;
}

[System.Serializable]
public struct Intersection
{
    public float percentage;
    public Spline otherSpline;
    public float otherPercentage;
    //public bool priority;
}


public class StreetMap : MonoBehaviour
{
    [Header("Spline zones")]
    [Space]

    [SerializeField]
    [Tooltip("The speed zones along the spline, anchor by anchor")]
    private SPEEDLIMIT[] speedLimits;
    public SPEEDLIMIT[] SpeedLimits { get { return speedLimits; } set { speedLimits = value; } }

    [Header("Interactions with other splines")]
    [Space]

    [SerializeField]
    [Tooltip("The intersections along the spline")]
    private Intersection[] intersections;
    public Intersection[] Intersections { get { return intersections; } set { intersections = value; } }

    [SerializeField]
    [Tooltip("The colinearities with others splines")]
    private Colinearity[] colinearities;
    public Colinearity[] Colinearities { get { return colinearities; } set { colinearities = value; } }

    private List<GameObject> users;
    public List<GameObject> Users { get { return users; } set { users = value; } }

    private Dictionary<float, SPEEDLIMIT> speedZones;
    public Dictionary<float, SPEEDLIMIT> SpeedZones { get { return speedZones; } set { speedZones = value; } }

    private void Awake()
    {
        users = new List<GameObject>();

        float[] percentages = new float[transform.childCount];
        int anchorChildCount = 0;
        foreach(Transform child in transform)
        {
            if(child.name.Contains("Anchor"))
            {
                percentages[anchorChildCount] = GetComponent<Spline>().ClosestPoint(child.position);
                anchorChildCount++;
            }
        }
        if (speedLimits.Length != anchorChildCount)
        {
            throw new System.Exception(transform.name + " : Anchor percentages and speed zones arrays not of the same length");
        }

        speedZones = percentages.Zip(speedLimits, (first, second) => new { first, second }).ToDictionary(val => val.first, val => val.second);
    }

    public void RegisterUser(GameObject user)
    {
        users.Add(user);
    }

    public void UnregisterUser(GameObject user)
    {
        users.Remove(user);
    }

    public override string ToString()
    {
        return gameObject.name + " has currently " + users.Count + " vehicles taking it";
    }
}
