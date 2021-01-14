using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neon : MonoBehaviour
{
    [SerializeField]
    Light l;
    [SerializeField]
    bool activate = true;

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
        mat.SetInt("_On", (activate ? 1 : 0));
        mat.SetFloat("_Intensity", l.intensity);
        mat.SetColor("_Color",l.color);
    }
}
