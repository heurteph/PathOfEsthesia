using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightVFXManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeactivateLightVFX());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DeactivateLightVFX()
    {
        // Wait 1 second for all prefabs to spawn
        // TO DO : Find a cleaner way

        yield return new WaitForSeconds(1f);

        // Deactivate all brightness VFX during daylight

        if (SceneManager.GetActiveScene().name == "GameElise")
        {
            foreach (GameObject o in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if (o.name == "LightVisualEffect")
                {
                    o.SetActive(false);
                }
            }
        }
    }
}
