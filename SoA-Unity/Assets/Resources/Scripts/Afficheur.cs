using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afficheur : MonoBehaviour
{
    [SerializeField]
    Light l;

    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        //nouvelle instance
        mat = new Material(mat);
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }
    void Update()
    {
        mat.SetFloat("_LightRange", l.range);
        mat.SetFloat("_LightIntensity", l.intensity);
        mat.SetColor("_LightColor", l.color);
        mat.SetVector("_LightPos", l.transform.position);
    }
}
