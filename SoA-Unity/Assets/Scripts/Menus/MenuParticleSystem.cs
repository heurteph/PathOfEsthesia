using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParticleSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback; // To call OnParticleSystemStopped()
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayParticleSound()
    {
        AkSoundEngine.PostEvent("Play_Texte_Anim_Particule", gameObject);
    }

    public void StopParticleSound()
    {
        AkSoundEngine.PostEvent("Stop_Texte_Anim_Particule", gameObject);
    }

    void OnParticleSystemStopped()
    {
        Debug.Log("System has stopped!");
    }
}
