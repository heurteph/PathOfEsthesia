using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCity : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Reference to the game manager")]
    private GameObject gameManager;

    private delegate void ExitHandler();
    private event ExitHandler ExitEvent;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            throw new System.NullReferenceException("Missing reference to the game manager");
        }
        ExitEvent += gameManager.GetComponent<GameManager>().WinGame;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ExitEvent();
        }
    }
}
