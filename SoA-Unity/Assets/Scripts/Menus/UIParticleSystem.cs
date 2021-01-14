using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // take the particle system to the center of the button
        Vector2 center = transform.GetChild(0).GetComponent<RectTransform>().rect.center;
        transform.GetChild(1).position = transform.GetComponent<RectTransform>().TransformPoint(new Vector3(center.x, center.y, -300));
    }

}
