using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class TrafficManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The street splines to be watched")]
    private Spline[] streetSplines;

    [SerializeField]
    [Range(10,100)]
    [Tooltip("The distance below which the object must tailor its speed to the predecessor")]
    private float minWatchDistance = 20f;

    [SerializeField]
    [Range(0, 5f)]
    [Tooltip("How much the speed of the object impact its safety distance")]
    private float minSafetySpeedFactor = 2f/3f;

    [SerializeField]
    [Range(1, 100)]
    [Tooltip("The distance below which the object must stop")]
    private float minSafetyDistance = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // Compute the length of every splines before using them
        foreach(Spline streetSpline in streetSplines)
        {
            streetSpline.CalculateLength();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForward();
        CheckCrossroad();

        // compare which one is closer !

    }

    void CheckForward()
    {
        foreach (Spline spline in streetSplines)
        {
            foreach (GameObject user in spline.GetComponent<StreetMap>().Users)
            {
                // Set default state, except if in FREEZE mode
                if(user.GetComponent<StreetUser>().MovingState != StreetUser.STATE.FREEZE)
                {
                    user.GetComponent<StreetUser>().MovingState = StreetUser.STATE.NORMAL;
                }

                foreach (GameObject otherUser in spline.GetComponent<StreetMap>().Users)
                {
                    if ( ! GameObject.ReferenceEquals(user, otherUser))
                    {
                        //Debug.Log("Comparing " + user.name + " at " + user.GetComponent<SplineStreetUser>().Percentage + " and " + otherUser.name + " at " + otherUser.GetComponent<SplineStreetUser>().Percentage + " !!!!!!!!!!!!!");
                        float distance = otherUser.GetComponent<StreetUser>().Percentage - user.GetComponent<StreetUser>().Percentage;

                        // TO DO : ADD Forward/Backward Distinction
                        // minSafetyDistance proportional to speed of the object
                        float minWatchPercentage = Mathf.Max(user.GetComponent<StreetUser>().Speed * minSafetySpeedFactor, minWatchDistance) / spline.Length;

                        // Join the end and the start of the spline
                        if ((0 <= distance && distance <= minWatchPercentage) || (-1 <= distance && distance <= -1 + minWatchPercentage))
                        {
                            //Debug.Log(otherUser.name + " IS IN FRONT OF " + user.name + " !!!!");
                            user.GetComponent<StreetUser>().FrontSpeed = otherUser.GetComponent<StreetUser>().Speed;
                            user.GetComponent<StreetUser>().MovingState = StreetUser.STATE.STAYBEHIND;
                            float minSafetyPercentage = minSafetyDistance / spline.Length;
                            user.GetComponent<StreetUser>().ObstaclePercentage = Mathf.Repeat(otherUser.GetComponent<StreetUser>().Percentage - minSafetyPercentage + 1, 1);  // +1 because Repeat does not take negative numbers into argument
                            // TO DO : When there are two vehicules in front ! Choose the closer one !!!
                        }
                    }
                }

                foreach (Colinearity colinearity in spline.GetComponent<StreetMap>().Colinearities)
                {
                    // TO DO : Take into account the minWatchPercentage to check colinearity before we're in
                    float minWatchPercentage = Mathf.Max(user.GetComponent<StreetUser>().Speed * minSafetySpeedFactor, minWatchDistance) / spline.Length;
                    float colinearityPercentageForward = Mathf.Repeat(colinearity.percentageStart - minWatchPercentage + 1, 1);

                    if (IsInRange(user.GetComponent<StreetUser>().Percentage, colinearityPercentageForward, colinearity.percentageEnd)) // used to be : colinearity.percentageStart
                    {
                        foreach (GameObject otherUser in colinearity.otherSpline.GetComponent<StreetMap>().Users)
                        {
                            // TO DO : Take into account the minWatchPercentage to check colinearity before we're in
                            float otherMinWatchPercentage = Mathf.Max(otherUser.GetComponent<StreetUser>().Speed * minSafetySpeedFactor, minWatchDistance) / spline.Length;
                            float colinearityOtherPercentageForward = Mathf.Repeat(colinearity.otherPercentageStart - otherMinWatchPercentage + 1, 1);

                            if (IsInRange(otherUser.GetComponent<StreetUser>().Percentage, colinearityOtherPercentageForward, colinearity.otherPercentageEnd))
                            {
                                // Règle de trois
                                //float otherUserMappedPercentage = Remap(otherUser.GetComponent<StreetUser>().Percentage, colinearity.otherPercentageStart, colinearity.otherPercentageEnd, colinearity.percentageStart, colinearity.percentageEnd);
                                float otherUserMappedPercentage = Remap(otherUser.GetComponent<StreetUser>().Percentage, colinearityOtherPercentageForward, colinearity.otherPercentageEnd, colinearityPercentageForward, colinearity.percentageEnd);

                                Debug.Log("COLINEARITY OF TWO VEHICLES : " + user.name + " at " + user.GetComponent<StreetUser>().Percentage + " and the other " + otherUser.name + " at " + otherUserMappedPercentage);

                                float distance = otherUserMappedPercentage - user.GetComponent<StreetUser>().Percentage;

                                // TO DO : ADD Forward/Backward Distinction -> Remove Backward handling
                                minWatchPercentage = Mathf.Max(user.GetComponent<StreetUser>().Speed * minSafetySpeedFactor, minWatchDistance) / spline.Length;
                                if ((0 <= distance && distance < minWatchPercentage) || (-1 <= distance && distance <= -1 + minWatchPercentage))
                                {
                                    Debug.Log(otherUser.name + " IS IN FRONT OF " + user.name + " BY COLINEARITY !!!!");
                                    user.GetComponent<StreetUser>().MovingState = StreetUser.STATE.STAYBEHIND;
                                    user.GetComponent<StreetUser>().FrontSpeed = otherUser.GetComponent<StreetUser>().Speed; // TO DO : Remap too ?
                                    float minSafetyPercentage = minSafetyDistance / spline.Length;
                                    user.GetComponent<StreetUser>().ObstaclePercentage = Mathf.Repeat(otherUserMappedPercentage - minSafetyPercentage + 1, 1); // +1 because Repeat does not take negative numbers into argument
                                    Debug.Log("COLINEARITY TRIGGER WITH OBSTACLE PERCENTAGE AT " + user.GetComponent<StreetUser>().ObstaclePercentage);
                                    // TO DO : When there are two vehicules in front ! Choose the closer !!!
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static bool IsInRange(float value, float start, float end)
    {
        if (!(0 <= value && value <= 1 && 0 <= start && start <= 1 && 0 <= end && end <= 1))
        {
            throw new System.ArgumentOutOfRangeException("Values are not in a valid range [0,1] : start = " + start + ", end = " + end + ", value = " + value);
        }
        if(start < end)
        {
            return start <= value && value <= end;
        }
        if(end < start)
        {
            return start <= value || value <= end;
        }
        else // end == start
        {
            return value == end && start == value;
        }
    }

    public static float Remap(float value, float start1, float end1, float start2, float end2)
    {
        if (start1 < end1 && start2 < end2)
        {
            return start2 + ((value - start1) / (end1 - start1)) * (end2 - start2);
        }

        else if (end1 < start1 && start2 < end2)
        {
            float x = 0;
            if(start1 <= value)
            {
                x = (value - start1) / (1 - start1 + end1);
            }
            else if(value <= end1)
            {
                x = (1 - start1 + value) / (1 - start1 + end1);
            }
            else
            {
                throw new System.ArgumentOutOfRangeException("Wrong value for remapping");
            }
            return start2 + x * (end2 - start2);
        }

        else if (start1 < end1 && end2 < start2)
        {
            return Mathf.Repeat(start2 + ((value - start1) / (end1 - start1)) * (1 - start2 + end2), 1);
        }

        else if (end1 < start1 && end2 < start2)
        {
            float x = 0;
            if (value >= start1)
            {
                x = (value - start1) / (1 - start1 + end1);
            }
            else if (value <= end1)
            {
                x = (1 - start1 + value) / (1 - start1 + end1);
            }
            return Mathf.Repeat(start2 + x * (1 - start2 + end2), 1);
        }
        return value;
    }

    void CheckCrossroad()
    {
        foreach(Spline spline in streetSplines)
        {
            foreach (GameObject user in spline.GetComponent<StreetMap>().Users)
            {
                // No need to set the default mode, as CheckCrossroad is called after CheckForward

                foreach (Intersection intersection in spline.GetComponent<StreetMap>().Intersections)
                {
                    float distanceToIntersection = intersection.percentage - user.GetComponent<StreetUser>().Percentage;
                    float minWatchPercentage = Mathf.Max(user.GetComponent<StreetUser>().Speed * minSafetySpeedFactor, minWatchDistance) / spline.Length;

                    if ((0 <= distanceToIntersection && distanceToIntersection <= minWatchPercentage) || (-1 <= distanceToIntersection && distanceToIntersection <= -1 + minWatchPercentage))
                    {
                        foreach (GameObject otherUser in intersection.otherSpline.GetComponent<StreetMap>().Users)
                        {
                            //Debug.Log("Comparing " + user.name + " on " + spline.name + " at " + user.GetComponent<SplineStreetUser>().Percentage + " with " + otherUser.name + " on " + intersection.otherSpline.name + " at " + otherUser.GetComponent<SplineStreetUser>().Percentage + " !!!!!!!!!!!!!");

                            float otherDistanceToIntersection = intersection.otherPercentage - otherUser.GetComponent<StreetUser>().Percentage;
                            float otherMinWatchPercentage = Mathf.Max(otherUser.GetComponent<StreetUser>().Speed * minSafetySpeedFactor, minWatchDistance) / spline.Length;

                            // Join the end and the start of the spline
                            if ((0 <= otherDistanceToIntersection && otherDistanceToIntersection <= otherMinWatchPercentage) || (-1 <= otherDistanceToIntersection && otherDistanceToIntersection <= -1 + otherMinWatchPercentage))
                            {
                                user.GetComponent<StreetUser>().MovingState = StreetUser.STATE.STOP;
                                float minSafetyPercentage = minSafetyDistance / spline.Length;
                                user.GetComponent<StreetUser>().IntersectionPercentage = Mathf.Repeat(intersection.percentage - minSafetyPercentage + 1, 1);
                            }
                        }
                    }
                }
            }
        }
    }
}
