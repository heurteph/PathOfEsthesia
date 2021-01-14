using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headlights : MonoBehaviour
{
    [SerializeField]
    private Light headlight1;

    [SerializeField]
    private Light headlight2;

    [SerializeField]
    private GameObject cone1;

    [SerializeField]
    private GameObject cone2;

    [SerializeField]
    private GameObject lightSquare1;

    [SerializeField]
    private GameObject lightSquare2;

    [SerializeField]
    private Material darkMat;

    [SerializeField]
    private Material lightMat;

    // Start is called before the first frame update
    void Start()
    {
        headlight1.enabled = false;
        headlight2.enabled = false;
        cone1.GetComponent<MeshRenderer>().enabled = false;
        cone2.GetComponent<MeshRenderer>().enabled = false;
        lightSquare1.GetComponent<MeshRenderer>().material = darkMat;
        lightSquare2.GetComponent<MeshRenderer>().material = darkMat;

        StartCoroutine("Flash");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Flash()
    {
        float darkDuration, lightDuration;
        for (; ; )
        {
            darkDuration = 2; // Random.Range(2f, 6f);
            lightDuration = 3; // Random.Range(0.1f, 4f);

            yield return new WaitForSeconds(darkDuration);

            headlight1.enabled = true;
            headlight2.enabled = true;
            cone1.GetComponent<MeshRenderer>().enabled = true;
            cone2.GetComponent<MeshRenderer>().enabled = true;
            lightSquare1.GetComponent<MeshRenderer>().material = lightMat;
            lightSquare2.GetComponent<MeshRenderer>().material = lightMat;

            yield return new WaitForSeconds(lightDuration);

            headlight1.enabled = false;
            headlight2.enabled = false;
            cone1.GetComponent<MeshRenderer>().enabled = false;
            cone2.GetComponent<MeshRenderer>().enabled = false;
            lightSquare1.GetComponent<MeshRenderer>().material = darkMat;
            lightSquare2.GetComponent<MeshRenderer>().material = darkMat;
        }
    }
}
