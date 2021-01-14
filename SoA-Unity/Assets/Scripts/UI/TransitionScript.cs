using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    private static GameObject singleton;

    private GameObject gameManager;

    private bool nightDone;
    private bool creditsDone;

    private void Awake()
    {
        // Must exist between scenes to achieve transitions
        if (singleton == null)
        {
            singleton = transform.parent.gameObject;
            DontDestroyOnLoad(transform.parent.gameObject); // save all canvas
        }
        else if (singleton != transform.parent.gameObject)
        {
            Destroy(transform.parent.gameObject); // destroy all canvas
            return;
        }

        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        if (gameManager == null)
        {
            throw new System.NullReferenceException("Missing reference to the game manager");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        nightDone = false;
        creditsDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerNight()
    {
        if (!nightDone)
        {
            Debug.Log("Fin de l'anim de fade out avant la night");
            gameManager.GetComponent<GameManager>().GoToNight();
            nightDone = true;
        }
    }

    public void TriggerCredits()
    {
        if (!creditsDone)
        {
            Debug.Log("Fin de l'anim de fade out avant les crédits");
            gameManager.GetComponent<GameManager>().DisplayCredits();
            creditsDone = true;
        }
    }

    public void DestroySingleton()
    {
        singleton = null;   
    }
}
