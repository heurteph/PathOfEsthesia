using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderStaticLarge : MonoBehaviour
{
    private static readonly int NB_BUFFER_SHADER = 128;
    MeshRenderer mesh;
    HashSet<GameObject> lights;
    int cmpt = 0;
    bool collider = true;

    // Start is called before the first frame update
    void Awake()
    {
        lights = new HashSet<GameObject>();
        mesh = GetComponent<MeshRenderer>();
        List <Vector4> vec = new List <Vector4>();
        for(int i = 0; i < NB_BUFFER_SHADER; i++)
        {
            vec.Add(new Vector4());
        }
        mesh.material.SetFloat("vector_lenght", 0);
        mesh.material.SetVectorArray("vector_pos",vec);
        mesh.material.SetVectorArray("vector_dir", vec);
        mesh.material.SetVectorArray("vector_col", vec);
        mesh.material.SetVectorArray("vector_opt", vec);
    }

    void Update()
    {
        if(Time.realtimeSinceStartup > 4.0f && collider)
        {
            Debug.Log("Debut " + gameObject.name);
            collider = false;
            CapsuleCollider [] colliders = gameObject.GetComponents<CapsuleCollider>();
            if(colliders != null && colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                }
            }
            else
            {
                Debug.Log("Box " + gameObject.name);
                BoxCollider[] boxes = gameObject.GetComponents<BoxCollider>();
                for (int i = 0; i < boxes.Length; i++)
                {
                    boxes[i].enabled = false;
                }
            }

        }    
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {

        /*if (gameObject.name.Equals("Downtown Terrain") && !other.name.Equals("Light_Lampadaire"))
        {
            return;
        }*/

        Light l = other.gameObject.GetComponent<Light>();

        if (l != null && lights.Count < NB_BUFFER_SHADER && !lights.Contains(other.gameObject))
        {
            Debug.Log("Static Enter " + other.name + " " + gameObject.name);

            lights.Add(l.gameObject);
            Vector4 pos_;
            Vector4 dir_;
            Vector4 opt_;
            Vector4 col_;

            col_ = l.color;
            float value = l.range;
            opt_ = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            dir_ = new Vector4(0.0f, 0.0f, 0.0f, l.intensity);
            switch (l.type)
            {
                case LightType.Point:
                    value *= -1;
                    //opt_ = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                    //dir_ = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                    break;
                case LightType.Spot:
                    Vector3 direction = l.transform.rotation.eulerAngles;
                    dir_ = new Vector4((direction.x * Mathf.PI) / 180.0f, (direction.y * Mathf.PI) / 180.0f, (direction.z * Mathf.PI) / 180.0f, l.intensity);
                    //angle
                    float outerRad = Mathf.Deg2Rad * 0.5f * l.spotAngle;
                    float outerCos = Mathf.Cos(outerRad);
                    float outerTan = Mathf.Tan(outerRad);
                    float innerCos = Mathf.Cos(Mathf.Atan(((64.0f - 18.0f) / 64.0f) * outerTan));
                    float angleRange = Mathf.Max(innerCos - outerCos, 0.001f);

                    float X = 1.0f / Mathf.Max(l.range * l.range, 0.00001f);
                    float Z = 1.0f / angleRange;
                    float W = -outerCos * Z;

                    opt_ = new Vector4(X, 0.0f, Z, W);
                    break;
                default:
                    //
                    break;
            }
            pos_ = new Vector4(l.gameObject.transform.position.x, l.gameObject.transform.position.y, l.gameObject.transform.position.z, value);


            float len = mesh.material.GetFloat("vector_lenght");
            if (cmpt == 0)
                len = 0;
            int size = (int)len;
            Vector4[] pos = mesh.material.GetVectorArray("vector_pos");
            Vector4[] dir = mesh.material.GetVectorArray("vector_dir");
            Vector4[] col = mesh.material.GetVectorArray("vector_col");
            Vector4[] opt = mesh.material.GetVectorArray("vector_opt");

            List<Vector4> positions = new List<Vector4>();
            List<Vector4> directions = new List<Vector4>();
            List<Vector4> colors = new List<Vector4>();
            List<Vector4> options = new List<Vector4>();
            //a voir si dans un seule array a la suite
            /*if(pos != null)
            {
                positions.AddRange(pos);
                directions.AddRange(dir);
                colors.AddRange(col);
                options.AddRange(opt);
            }*/

            positions.AddRange(pos);
            directions.AddRange(dir);
            colors.AddRange(col);
            options.AddRange(opt);

            /*positions.Add(pos_);
            directions.Add(dir_);
            colors.Add(col_);
            options.Add(opt_);*/

            positions[size] = pos_;
            directions[size] = dir_;
            colors[size] = col_;
            options[size] = opt_;

            mesh.material.SetFloat("vector_lenght",size+1);
            mesh.material.SetVectorArray("vector_pos",positions);
            mesh.material.SetVectorArray("vector_dir", directions);
            mesh.material.SetVectorArray("vector_col", colors);
            mesh.material.SetVectorArray("vector_opt", options);

            Debug.Log("Toucher couler !!!!!!");
            cmpt++;
        }
    }
}
