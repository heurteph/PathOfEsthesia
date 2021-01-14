using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
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
            Destroy(gameObject);
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
