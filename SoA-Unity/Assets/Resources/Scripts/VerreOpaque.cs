using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerreOpaque : MonoBehaviour
{
    [SerializeField]
    Light l;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {
        mat.SetVector("_Light_Pos", new Vector4(l.transform.position.x, l.transform.position.y, l.transform.position.z, 1.0f));
        mat.SetFloat("_Intensity", l.intensity);
        mat.SetColor("_Color",l.color);
    }
}
