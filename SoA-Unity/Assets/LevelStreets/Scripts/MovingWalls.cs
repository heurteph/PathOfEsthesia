using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWalls : MonoBehaviour
{
    private float amplitude = 5;
    private float speed = 1;
    private float accumulator = 0;
    private int direction = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        accumulator += direction * Time.deltaTime;
        if(Mathf.Abs(accumulator) >= amplitude)
        {
            accumulator = direction * amplitude;
            direction = -direction;
        }
        Vector3 move = transform.forward * Time.deltaTime * speed * direction;
        transform.position += move;
    }
}
