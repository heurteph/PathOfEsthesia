using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfficheScript : MonoBehaviour
{
    [SerializeField]
    Light l;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        //mat = new Material(mat);
        //mat.SetVector("_Color",new Vector4(color.r, color.g, color.b, color.a));
        //gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        //mat.SetVector("_LightPosition",gameObject.transform.parent.position);
        mat.SetFloat("_LightIntensity",l.intensity);
        mat.SetColor("_ContourColor",l.color);
    }
}
