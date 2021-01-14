using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class Shop : MonoBehaviour
{
    [SerializeField]
    [Tooltip("List of spline coming from this shop")]
    private Spline[] exitPaths;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Spline GetExitPath()
    {
        return exitPaths[Random.Range(0, exitPaths.Length)];
    }
}
