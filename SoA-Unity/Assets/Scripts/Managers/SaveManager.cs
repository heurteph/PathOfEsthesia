using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SHELTER { HOME, SHED, BAR }
public class SaveManager : MonoBehaviour
{
    private static GameObject singleton;

    private SHELTER saveShelterIndex;
    public SHELTER SaveShelterIndex { get { return saveShelterIndex; } }

    //private GameObject saveShelter;
    //public GameObject SaveShelter { get { return saveShelter; } }

    // Start is called before the first frame update
    void Awake()
    {
        // Do not create more than one save manager on reload scene
        if (singleton == null)
        {
            singleton = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != gameObject)
        {
            Destroy(gameObject);
            return;
        }

        saveShelterIndex = SHELTER.HOME;
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save(GameObject shelter)
    {
        if (shelter.CompareTag("Bar") && (saveShelterIndex == SHELTER.HOME || saveShelterIndex == SHELTER.SHED))
        {
            saveShelterIndex = SHELTER.BAR;
        }
        else if (shelter.CompareTag("Shed") && saveShelterIndex == SHELTER.HOME)
        {
            saveShelterIndex = SHELTER.SHED;
        }
    }

    public void DestroySingleton()
    {
        singleton = null; // Manually destroy static object
    }
}
