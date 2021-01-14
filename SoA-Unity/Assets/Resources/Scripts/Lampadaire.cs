using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lampadaire : MonoBehaviour
{
    [SerializeField]
    GameObject light;
    Material mat;
    Light l;

    // Start is called before the first frame update
    void Start()
    {
        l = light.GetComponent<Light>();
        mat = GetComponent<MeshRenderer>().material;
    }
    public void Update()
    {
        mat.SetFloat("height_scale", transform.localScale.y);
        mat.SetColor("_color", l.color);
    }

}
