using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class AmbianceManager : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if(player == null)
        {
            throw new System.NullReferenceException("Missing game object tagged with \"Player\"");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHomeAmbiance()
    {
        AkSoundEngine.PostEvent("Play_Music_Safe_Zone_Home", player);
        AkSoundEngine.PostEvent("Stop_Pigeon", player);
        AkSoundEngine.PostEvent("Stop_Parc_Oiseaux1", player);
    }

    public void PlayShedAmbiance()
    {
        AkSoundEngine.PostEvent("Play_Music_Safe_Zone_Parc", player);
        AkSoundEngine.PostEvent("Stop_Pigeon", player);
        AkSoundEngine.PostEvent("Stop_Parc_Oiseaux1", player);
    }

    public void PlayBarAmbiance()
    {
        AkSoundEngine.PostEvent("Play_Music_Safe_Zone_Ville", player);
        AkSoundEngine.PostEvent("Stop_Pigeon", player);
        AkSoundEngine.PostEvent("Stop_Parc_Oiseaux1", player);
    }

    public void PlayCityAmbiance()
    {
        AkSoundEngine.PostEvent("Play_Pigeon", player);
        AkSoundEngine.PostEvent("Stop_Parc_Oiseaux1", player);
        AkSoundEngine.PostEvent("Stop_Music_Safe_Zone_Ville", player);
        AkSoundEngine.PostEvent("Stop_Music_Safe_Zone_Parc", player);
        AkSoundEngine.PostEvent("Stop_Music_Safe_Zone_Home", player);
    }

    public void PlayParkAmbiance()
    {
        AkSoundEngine.PostEvent("Play_Parc_Oiseaux1", player);
        AkSoundEngine.PostEvent("Stop_Pigeon", player);
        AkSoundEngine.PostEvent("Stop_Music_Safe_Zone_Ville", player);
        AkSoundEngine.PostEvent("Stop_Music_Safe_Zone_Parc", player);
        AkSoundEngine.PostEvent("Stop_Music_Safe_Zone_Home", player);
    }
}
