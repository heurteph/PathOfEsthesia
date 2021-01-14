using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class TunnelTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            AkSoundEngine.SetState("Dans_Tunnel","Oui");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            AkSoundEngine.SetState("Dans_Tunnel", "Non");
        }
    }
}
