using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Bus : MonoBehaviour
{
    [SerializeField]
    private GameObject axis;

    [SerializeField]
    private GameObject stop;

    [SerializeField]
    [Range(1,100)]
    private float speed = 5;

    [SerializeField]
    private float waitTime = 7;

    private Vector3 startPosition;
    private float radius;
    private float angle;

    private float accumulator = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        radius = (transform.position - axis.transform.position).magnitude;
        StartCoroutine("StopAndGo");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator StopAndGo()
    {
        for(; ;)
        {
            //float arc = Mathf.PI * 2f * radius * angle / 360;
            accumulator = 0;
            while(accumulator != 360)
            {
                angle = -Time.deltaTime * speed;
                accumulator = Mathf.Min(accumulator + Mathf.Abs(angle), 360);
                transform.RotateAround(axis.transform.position, axis.transform.up, angle);
                yield return null;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
