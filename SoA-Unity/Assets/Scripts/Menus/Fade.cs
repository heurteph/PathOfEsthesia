using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    GameObject menuManager;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = GameObject.FindGameObjectWithTag("MenuManager");
        if (menuManager == null)
        {
            throw new System.NullReferenceException("Missing MenuManager object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAnimationFinish()
    {
        menuManager.GetComponent<MenuManager>().StartGame();
    }
}
