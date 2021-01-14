using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField]
    private GameObject light_prob;
    ReflectionProbe rp;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        rp = light_prob.GetComponent<ReflectionProbe>();

        mat = GetComponent<MeshRenderer>().material;
        mat.SetTexture("_Skybox",rp.texture);
    }

    // Update is called once per frame
    void Update()
    {
        mat = GetComponent<MeshRenderer>().material;
        mat.SetTexture("_Skybox", rp.texture);
    }
}
