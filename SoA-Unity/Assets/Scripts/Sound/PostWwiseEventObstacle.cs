using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEventObstacle : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The play event of the obstacle")]
    private AK.Wwise.Event obstacleEventPlay;
    public AK.Wwise.Event ObstacleEventPlay { get { return obstacleEventPlay; } }

    [SerializeField]
    [Tooltip("The stop event of the obstacle")]
    private AK.Wwise.Event obstacleEventStop;
    public AK.Wwise.Event ObstacleEventStop { get { return obstacleEventStop; } }

    // Start is called before the first frame update
    void Start()
    {
        if(obstacleEventPlay == null)
        {
            throw new System.NullReferenceException("No play event for the obstacle " + transform.name);
        }
        if(obstacleEventStop == null)
        {
            throw new System.NullReferenceException("No stop event for the obstacle " + transform.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        obstacleEventPlay.Post(gameObject);
    }

    public void Stop()
    {
        obstacleEventStop.Post(gameObject);
    }
}
