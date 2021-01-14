using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    private GameObject transitions;

    // Start is called before the first frame update
    void Start()
    {
        // Transitions
        transitions = GameObject.FindGameObjectWithTag("Transitions");
        transitions.GetComponent<Transitions>().FadeIn();

        AkSoundEngine.PostEvent("Play_Music_Menu", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToTitle()
    {
        StartCoroutine(transitions.GetComponent<Transitions>().FadeOut("Title"));
    }

    public void OnDestroy()
    {
        AkSoundEngine.PostEvent("Stop_Music_Menu", gameObject);
    }
}
