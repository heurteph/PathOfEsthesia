using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCamera : MonoBehaviour
{
    private static GameObject instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (gameObject != instance)
        {
            Destroy(instance);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
