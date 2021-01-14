using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Façade: MonoBehaviour
{
    private GameObject[] windows;
    private int index;

    [SerializeField]
    [Range(0,100)]
    private float lightIntensity = 1;

    [SerializeField]
    private float lightAngle = 20;

    [SerializeField]
    private float lightRange = 10;

    [SerializeField]
    private Material darkMat;

    [SerializeField]
    private Material lightMat;

    // Start is called before the first frame update
    void Start()
    {
        windows = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            windows[i] = transform.GetChild(i).gameObject;
            if(!windows[i].TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                throw new System.NullReferenceException("No MeshRender for the window " + windows[i].name + " of the building " + transform.name);
            }
            if(windows[i].transform.GetChild(0) == null)
            {
                throw new System.NullReferenceException("No light child for the window " + windows[i].name + " of the building " + transform.name);
            }
            if(!windows[i].transform.GetChild(0).TryGetComponent<Light>(out Light light))
            {
                throw new System.NullReferenceException("No light for the window " + windows[i].name + " of the building " + transform.name);
            }
            if (windows[i].transform.GetChild(1) == null)
            {
                throw new System.NullReferenceException("No halo child for the window " + windows[i].name + " of the building " + transform.name);
            }
            if (!windows[i].transform.GetChild(1).TryGetComponent<MeshRenderer>(out MeshRenderer mesRenderer))
            {
                throw new System.NullReferenceException("No halo for the window " + windows[i].name + " of the building " + transform.name);
            }
        }
        index = Random.Range(0, transform.childCount);

        LightsOut(); // TO DO : Some lights might be highlighted from the start

        StartCoroutine("Flash");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Highlight()
    {
        int newIndex = Random.Range(0, transform.childCount);
        index = (newIndex != index) ? newIndex : (newIndex + 1) % transform.childCount;
        
        windows[index].GetComponent<MeshRenderer>().material = lightMat;
        windows[index].transform.GetChild(0).GetComponent<Light>().enabled = true;
        windows[index].transform.GetChild(0).GetComponent<Light>().spotAngle = 20;
        windows[index].transform.GetChild(0).GetComponent<Light>().spotAngle = 10;
        windows[index].transform.GetChild(0).GetComponent<Light>().intensity = 0.5f;
        windows[index].transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false; // CILYNDRE A METRE TRUUUUUE CACA
    }

    private void LightsOut()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            windows[i].GetComponent<MeshRenderer>().material = darkMat;
            windows[i].transform.GetChild(0).GetComponent<Light>().enabled = false;
            windows[i].transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        }
    }

    IEnumerator Flash()
    {
        float darkDuration, lightDuration;
        for (; ; )
        {
            darkDuration  = Random.Range(2, 6);
            lightDuration = Random.Range(8, 16);

            yield return new WaitForSeconds(darkDuration);

            Highlight();

            yield return new WaitForSeconds(lightDuration);

            LightsOut();
        }
    }
}
