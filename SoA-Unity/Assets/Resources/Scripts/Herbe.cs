using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbe : MonoBehaviour
{
    [SerializeField]
    GameObject Player;

    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (Player.transform.position - transform.position).magnitude;
        mat.SetVector("_PlayerPosition", new Vector4(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z, 0.0f));
        mat.SetInt("_DynamicRender", 1);
        //ici tess fixer a 50
        //mat.SetFloat("_TessellationUniform", 50);
    }
}
