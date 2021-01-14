using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class EnterShelter : MonoBehaviour
{
    private Inputs inputs;

    [SerializeField]
    private EnergyBehaviour energyBehaviour;

    private Image shade;

    private GameObject shelter;

    [SerializeField]
    private ShelterManager shelterManager;

    [SerializeField]
    private Camera mainCamera;

    private GameObject ambianceManager;

    private string shelterTag;

    private GameObject saveManager;

    private GameObject compass;

    private void Awake()
    {
        inputs = InputsManager.Instance.Inputs;
        inputs.Player.Interact.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ExitShelter>().enabled = false;

        ambianceManager = GameObject.FindGameObjectWithTag("AmbianceManager");
        saveManager = GameObject.FindGameObjectWithTag("SaveManager");
        shade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        compass = GameObject.FindGameObjectWithTag("Compass");
        Debug.Assert(compass != null, "Missing gameobject with compass tag");

        if (ambianceManager == null)
        {
            throw new System.NullReferenceException("Missing game object tagged with tag \"AmbianceManager\"");
        }
        if (saveManager == null)
        {
            throw new System.NullReferenceException("Missing game object tagged with tag \"SaveManager\"");
        }

        // Load shelter from save
        shelter = null;
        switch(saveManager.GetComponent<SaveManager>().SaveShelterIndex)
        {
            case SHELTER.HOME:
                shelter = shelterManager.GetComponent<ShelterManager>().ShelterInsides.Where(s => s.tag == "Home").First();
                break;
            case SHELTER.SHED:
                shelter = shelterManager.GetComponent<ShelterManager>().ShelterInsides.Where(s => s.tag == "Shed").First();
                break;
            case SHELTER.BAR:
                shelter = shelterManager.GetComponent<ShelterManager>().ShelterInsides.Where(s => s.tag == "Bar").First();
                break;
        }
        StartCoroutine("Respawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (shelterManager && inputs.Player.enabled)
        {
            inputs.Player.Interact.performed -= WorldToShelter;
            inputs.Player.Interact.Disable();

            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Shelter Entrance");

            if (Physics.Raycast(transform.position, transform.forward, out hit, shelterManager.MaxDistanceToDoor, mask))
            {
                shelter = shelterManager.GoInside(hit.collider.transform.parent.gameObject);
                inputs.Player.Interact.performed += WorldToShelter;
                inputs.Player.Interact.Enable();
            }
        }
    }

    IEnumerator Respawn()
    {
        // suspend interaction both with player and world
        inputs.Player.Disable();

        GetComponent<EnergyBehaviour>().Invincibility(true);

        GetComponent<PlayerFirst>().IsInsideShelter = true;

        shelterTag = shelter.tag;

        // Reset Character Position and Speed

        if (transform.GetComponent<PlayerFirst>().isActiveAndEnabled)
        {
            Transform warp = shelter.transform.Find("Warp Position");
            transform.GetComponent<PlayerFirst>().ResetTransform(warp.position, warp.rotation.eulerAngles.y);
            transform.GetComponent<PlayerFirst>().SetShelterSpeed();
        }

        // Reset Camera

        mainCamera.enabled = false;

        // TO DO : Do not follow inside

        shelter.transform.Find("Shelter Camera").GetComponent<Camera>().enabled = true;

        // Reset sound

        shelter.transform.Find("Shelter Camera").GetComponent<AkAudioListener>().enabled = true;
        mainCamera.gameObject.GetComponent<AkAudioListener>().enabled = false;

        AkSoundEngine.SetState("Dans_Lieu_Repos", "Oui");

        // Ambiance sound
        if (shelterTag == "Home")
        {
            ambianceManager.GetComponent<AmbianceManager>().PlayHomeAmbiance();
        }
        else if (shelterTag == "Shed")
        {
            ambianceManager.GetComponent<AmbianceManager>().PlayShedAmbiance();
        }
        else if (shelterTag == "Bar")
        {
            ambianceManager.GetComponent<AmbianceManager>().PlayBarAmbiance();
        }

        //UI
        compass.GetComponent<Image>().enabled = false;
        //compass.GetComponent<CompassBehavior>().enabled = false;
        compass.SetActive(false);

        //GetComponent<PostWwiseAmbiance>().ParkAmbianceEventStop.Post(gameObject);
        //GetComponent<PostWwiseAmbiance>().ShelterAmbianceEventPlay.Post(gameObject);

        while (shade.color.a > 0)
        {
            shade.color = new Color(shade.color.r, shade.color.g, shade.color.b, Mathf.Max(shade.color.a - Time.deltaTime / (shelterManager.TransitionDuration * 0.5f), 0));
            yield return null;
        }
        shade.color = new Color(shade.color.r, shade.color.g, shade.color.b, 0);
        energyBehaviour.IsReloading = true;

        inputs.Player.Interact.performed -= WorldToShelter;
        inputs.Player.Enable();

        GetComponent<EnergyBehaviour>().Invincibility(false);

        GetComponent<ExitShelter>().enabled = true;
        GetComponent<EnterShelter>().enabled = false;

        // do the save
        saveManager.GetComponent<SaveManager>().Save(shelter);
    }

    void WorldToShelter(InputAction.CallbackContext ctx)
    {
        StartCoroutine("Enter");
    }

    IEnumerator Enter()
    {
        // suspend interactions with both the player and the world
        inputs.Player.Disable();
        GetComponent<EnergyBehaviour>().Invincibility(true);

        shelterTag = shelter.tag;

        // Play sound
        switch (shelterTag)
        {
            case "Home":
                AkSoundEngine.PostEvent("Play_Shelter_House_Door_Open", gameObject);
                break;
            case "Shed":
                AkSoundEngine.PostEvent("Play_Shelter_Parc_Door_Open", gameObject);
                break;
            case "Bar":
                AkSoundEngine.PostEvent("Play_Shelter_Bar_Door_Open", gameObject);
                break;
            default:
                Debug.LogError("Shelter not recognized: " + shelterTag);
                break;
        }

        while (!Mathf.Approximately(shade.color.a, 1))
        {
            shade.color = new Color(shade.color.r, shade.color.g, shade.color.b, Mathf.Min(shade.color.a + Time.deltaTime / (shelterManager.TransitionDuration * 0.5f), 1));
            yield return null;
        }

        // Reset Character Position and Speed

        if (transform.GetComponent<PlayerFirst>().isActiveAndEnabled)
        {
            Transform warp = shelter.transform.Find("Warp Position");
            transform.GetComponent<PlayerFirst>().ResetTransform(warp.position, warp.rotation.eulerAngles.y);
            transform.GetComponent<PlayerFirst>().SetShelterSpeed();
        }

        // Reset Camera

        mainCamera.enabled = false;
        GetComponent<PlayerFirst>().IsInsideShelter = true;
        shelter.transform.Find("Shelter Camera").GetComponent<Camera>().enabled = true;

        // Reset sound

        AkSoundEngine.SetState("Dans_Lieu_Repos", "Oui");

        shelter.transform.Find("Shelter Camera").GetComponent<AkAudioListener>().enabled = true;
        mainCamera.gameObject.GetComponent<AkAudioListener>().enabled = false;

        // Ambiance sound
        if (shelterTag == "Home")
        {
            ambianceManager.GetComponent<AmbianceManager>().PlayHomeAmbiance();
        }
        else if (shelterTag == "Shed")
        {
            ambianceManager.GetComponent<AmbianceManager>().PlayShedAmbiance();
        }
        else if (shelterTag == "Bar")
        {
            ambianceManager.GetComponent<AmbianceManager>().PlayBarAmbiance();
        }

        //UI
        compass.GetComponent<Image>().enabled = false;
        //compass.GetComponent<CompassBehavior>().enabled = false;
        compass.SetActive(false);

        //GetComponent<PostWwiseAmbiance>().ParkAmbianceEventStop.Post(gameObject);
        //GetComponent<PostWwiseAmbiance>().ShelterAmbianceEventPlay.Post(gameObject);

        while (shade.color.a > 0)
        {
            shade.color = new Color(shade.color.r, shade.color.g, shade.color.b, Mathf.Max(shade.color.a - Time.deltaTime / (shelterManager.TransitionDuration * 0.5f), 0));
            yield return null;
        }
        shade.color = new Color(shade.color.r, shade.color.g, shade.color.b, 0);
        energyBehaviour.IsReloading = true;

        inputs.Player.Interact.performed -= WorldToShelter;
        inputs.Player.Enable();

        GetComponent<ExitShelter>().enabled = true;
        GetComponent<EnterShelter>().enabled = false;

        GetComponent<EnergyBehaviour>().Invincibility(false);

        // do the save
        saveManager.GetComponent<SaveManager>().Save(shelter);
    }

    void OnDisable()
    {
        //inputs.Player.Disable();
    }

    private void OnDestroy()
    {
        inputs.Player.Interact.performed -= WorldToShelter;
    }

}  //FINISH
