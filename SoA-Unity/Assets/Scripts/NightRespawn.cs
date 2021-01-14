using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightRespawn : MonoBehaviour
{
    private Inputs inputs;

    private Camera mainCamera;

    [SerializeField]
    private GameObject cameraHolder;

    [SerializeField]
    private GameObject respawnPoint;

    private GameObject ambianceManager;

    private GameObject compass;

    private Image shade;

    private float transitionDuration = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        inputs = InputsManager.Instance.Inputs;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        ambianceManager = GameObject.FindGameObjectWithTag("AmbianceManager");
        compass = GameObject.FindGameObjectWithTag("Compass");
        shade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();

        StartCoroutine(Respawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Respawn()
    {
        // suspend interaction both with player and world
        inputs.Player.Disable();

        GetComponent<EnergyBehaviour>().Invincibility(true);

        GetComponent<PlayerFirst>().IsInsideShelter = false;

        // Reset Character Position and Speed

        if (transform.GetComponent<PlayerFirst>().isActiveAndEnabled)
        {
            Transform warp = respawnPoint.transform; // change to supermarket position
            transform.GetComponent<PlayerFirst>().ResetTransform(warp.position, warp.rotation.eulerAngles.y);
            transform.GetComponent<PlayerFirst>().ResetSpeed();
        }

        // Reset Camera

        GetComponent<PlayerFirst>().IsInsideShelter = false;
        mainCamera.enabled = true;

        if (cameraHolder.GetComponent<CameraFollow>().isActiveAndEnabled)
        {
            cameraHolder.GetComponent<CameraFollow>().ResetCameraToFrontView();
        }
        //shelterCamera.enabled = false;

        // Wait for one frame to have a correct camera angle
        yield return new WaitForEndOfFrame();

        // Reset sound
        mainCamera.gameObject.GetComponent<AkAudioListener>().enabled = true;
        //shelterCamera.gameObject.GetComponent<AkAudioListener>().enabled = false;

        AkSoundEngine.SetState("Dans_Lieu_Repos", "Non");

        // Ambiance sound
        ambianceManager.GetComponent<AmbianceManager>().PlayCityAmbiance();

        // UI
        compass.GetComponent<Image>().enabled = true;
        compass.SetActive(true);

        //GetComponent<PostWwiseAmbiance>().ParkAmbianceEventStop.Post(gameObject);
        //GetComponent<PostWwiseAmbiance>().ShelterAmbianceEventPlay.Post(gameObject);

        while (shade.color.a > 0)
        {
            shade.color = new Color(shade.color.r, shade.color.g, shade.color.b, Mathf.Max(shade.color.a - Time.deltaTime / (transitionDuration * 0.5f), 0));
            yield return null;
        }
        shade.color = new Color(shade.color.r, shade.color.g, shade.color.b, 0);
        //energyBehaviour.IsReloading = false;

        //inputs.Player.Interact.performed -= WorldToShelter;
        inputs.Player.Enable();

        GetComponent<EnergyBehaviour>().Invincibility(false);

        GetComponent<ExitShelter>().enabled = false;
        GetComponent<EnterShelter>().enabled = false;
    }
}
