using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("VFX")]
    [Space]

    [SerializeField]
    [Tooltip("VFX to play when loud sound is emitted")]
    private GameObject loudVFX;

    [SerializeField]
    [Tooltip("Initial delay to start the VFX")]
    [Range(0,1)]
    private float initialDelay = 0;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Assert(loudVFX != null);
        loudVFX.GetComponent<ParticleSystem>().Stop();
        ParticleSystem.MainModule mainModule = loudVFX.GetComponent<ParticleSystem>().main;
        mainModule.startLifetime = 0.25f; // One quarter of the default value
        StartCoroutine(VFX());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator VFX()
    {
        if(initialDelay != 0)
        {
            yield return new WaitForSeconds(initialDelay);
        }
        for (; ;)
        {
            loudVFX.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(0.05f);
            loudVFX.GetComponent<ParticleSystem>().Stop();
            yield return new WaitForSeconds(0.60f);
        }
    }
}
