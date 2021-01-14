using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamic : MonoBehaviour
{
    private static readonly int NB_BUFFER_SHADER = 16;
    MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        if(mesh == null)
        {
            SkinnedMeshRenderer skinmesh = GetComponent<SkinnedMeshRenderer>();
            List < Vector4 > vec = new List<Vector4>();
            for (int i = 0; i < NB_BUFFER_SHADER; i++)
            {
                vec.Add(new Vector4());
            }
            skinmesh.materials[0].SetFloat("vector_lenght", 0);
            skinmesh.materials[0].SetVectorArray("vector_pos", vec);
            skinmesh.materials[0].SetVectorArray("vector_dir", vec);
            skinmesh.materials[0].SetVectorArray("vector_col", vec);
            skinmesh.materials[0].SetVectorArray("vector_opt", vec);
        }
        else
        {
            List<Vector4> vec = new List<Vector4>();
            for (int i = 0; i < NB_BUFFER_SHADER; i++)
            {
                vec.Add(new Vector4());
            }
            mesh.material.SetFloat("vector_lenght", 0);
            mesh.material.SetVectorArray("vector_pos", vec);
            mesh.material.SetVectorArray("vector_dir", vec);
            mesh.material.SetVectorArray("vector_col", vec);
            mesh.material.SetVectorArray("vector_opt", vec);
        }
    }

}
