using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(string.Concat(name, " entre en collision avec le rigidbody ", collision.rigidbody.name));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(string.Concat(name, " entre en collision avec le trigger ", other.name));
    }
}
