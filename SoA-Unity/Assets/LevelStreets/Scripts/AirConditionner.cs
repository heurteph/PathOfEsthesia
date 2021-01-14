using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AirConditionner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The mesh of the fan's blades")]
    private GameObject fanBlades;

    [SerializeField]
    [Tooltip("The speed of the fan's blades")]
    [Range(0,1000)]
    private float fanSpeed = 500;

    private float fanRotation;

    // Start is called before the first frame update
    void Start()
    {
        if(fanBlades == null)
        {
            throw new System.NullReferenceException("No blades set for the fan of the air conditionner");
        }
        fanRotation = fanBlades.transform.localRotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        fanRotation = (fanRotation + fanSpeed * Time.deltaTime) % 360;
        fanBlades.transform.localRotation = Quaternion.Euler(0, 0, fanRotation);
    }
}
