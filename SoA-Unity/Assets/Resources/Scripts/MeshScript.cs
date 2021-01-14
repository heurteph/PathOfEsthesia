using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshScript : MonoBehaviour
{
    [SerializeField]
    Color color;
    void Awake()
    {
        Material mat = gameObject.GetComponent<MeshRenderer>().material;
        mat = new Material(mat);
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }
}
