using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringSystem : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Number of divisions of the full rotation")]
    [Range(5,25)]
    private float divisions = 10;

    [SerializeField]
    [Tooltip("The delay between two bursts in seconds")]
    [Range(0.01f, 1f)]
    private float delay = 0.4f;

    private float deltaAngle;
    private float xAngle;
    private float yAngle;

    private ParticleSystem.EmissionModule emission;

    // Start is called before the first frame update
    void Start()
    {
        xAngle = transform.rotation.eulerAngles.x;
        yAngle = transform.rotation.eulerAngles.y;
        deltaAngle = 360f / divisions;
        emission = GetComponent<ParticleSystem>().emission;
        StartCoroutine("Rotate");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Rotate()
    {
        for(;;)
        {
            yAngle = Mathf.Repeat(yAngle + deltaAngle, 360f);
            transform.rotation = Quaternion.Euler(xAngle, yAngle, transform.rotation.y);
            emission.rateOverTime = 100;
            yield return new WaitForSeconds(delay / 2f);
            emission.rateOverTime = 0;
            yield return new WaitForSeconds(delay / 2f);
        }
    }
}
