using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLight : MonoBehaviour
{
    Light l;
    Vector4 pos_;
    Vector4 dir_;
    Vector4 opt_;
    Vector4 col_;
    // Start is called before the first frame update
    void Awake()
    {
        l = gameObject.GetComponent<Light>();
        col_ = l.color;
        float value = l.range;
        switch (l.type)
        {
            case LightType.Point:
                value *= -1;
                opt_ = new Vector4(0.0f,0.0f,0.0f,0.0f);
                dir_ = new Vector4(0.0f,0.0f,0.0f,l.intensity);
                break;
            case LightType.Spot:
                Debug.Log("Spot light");
                Vector4 direction = new Vector4(0.0f,0.0f,0.0f,0.0f);//l.transform.rotation.eulerAngles;
                dir_ = direction;//new Vector4((direction.x * Mathf.PI) / 180.0f, (direction.y * Mathf.PI) / 180.0f, (direction.z * Mathf.PI) / 180.0f, l.intensity);
                //angle
                float outerRad;// = Mathf.Deg2Rad * 0.5f * l.spotAngle;
                //float outerCos = Mathf.Cos(outerRad);
                //float outerTan = Mathf.Tan(outerRad);
                //float innerCos = Mathf.Cos(Mathf.Atan(((64.0f - 18.0f) / 64.0f) * outerTan));
                //float angleRange = Mathf.Max(innerCos - outerCos, 0.001f);

                outerRad = 17.5f * Mathf.Deg2Rad;
                float inner = 12.5f * Mathf.Deg2Rad;

                float X = inner;//1.0f / Mathf.Max(l.range * l.range, 0.00001f);
                float Y = outerRad;//1.0f / angleRange;
                float Z = 0.0f;
                float W = 0.0f;//-outerCos * Z;
                X = l.spotAngle * Mathf.Deg2Rad;
                opt_ = new Vector4(Mathf.Cos(X), Y, Z, W);
                
                
                break;
            default:
                //
                break;
        }
        pos_ = new Vector4(transform.position.x, transform.position.y, transform.position.z, value);
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {

        //if(other.gameObject.GetComponent<Light>() == null && other.gameObject.GetComponent<ColliderStatic>() == null)
        if (other.gameObject.GetComponent<RandomizeNPC>() != null || other.gameObject.name.Equals("Player") || other.gameObject.GetComponent<Dynamic>() != null)
        {
            Debug.Log("Collider Light Enter " + other.name + " " + gameObject.name);
            //MeshRenderer mesh = other.gameObject.GetComponent<MeshRenderer>();
            SkinnedMeshRenderer mesh_skin = other.gameObject.GetComponent<SkinnedMeshRenderer>();
            MeshRenderer mesh_ren = null;


            if (mesh_skin == null)
            {
                mesh_skin = other.transform.Find("body")?.GetChild(0)?.gameObject.GetComponent<SkinnedMeshRenderer>();
                if (other.gameObject.name.Equals("Player"))
                {
                    mesh_skin = other.transform.Find("Avatar_Esthesia_mergedAnims/CharaEsthesia_geo/charaBody_Esthesia_geo").GetComponent<SkinnedMeshRenderer>();
                    if(mesh_skin == null)
                        other.transform.Find("Avatar_Esthesia_mergedAnims/CharaEsthesia_geo/charaHair_Esthesia_geo").GetComponent<SkinnedMeshRenderer>();
                }
                if(mesh_skin == null)
                {
                    mesh_ren = other.transform.Find("body")?.GetChild(0)?.gameObject.GetComponent<MeshRenderer>();
                }
            }

            if(mesh_skin != null || mesh_ren != null)
            {
                Vector4[] pos;
                Vector4[] dir;
                Vector4[] col;
                Vector4[] opt;
                float len = 0.0f;

                Material mat;
                if (mesh_skin != null)
                {
                    //normalement je pense que c'est déjà géré mais bon on sait jamais
                    mat = mesh_skin.material;
                    if (mat == null)
                        mat = mesh_skin.materials[0];
                    pos = mat.GetVectorArray("vector_pos");
                    dir = mat.GetVectorArray("vector_dir");
                    col = mat.GetVectorArray("vector_col");
                    opt = mat.GetVectorArray("vector_opt");
                    len = mat.GetFloat("vector_lenght");
                }
                else
                {
                    mat = mesh_ren.material;
                    if (mat == null)
                        mat = mesh_ren.materials[0];
                    pos = mat.GetVectorArray("vector_pos");
                    dir = mat.GetVectorArray("vector_dir");
                    col = mat.GetVectorArray("vector_col");
                    opt = mat.GetVectorArray("vector_opt");
                    len = mat.GetFloat("vector_lenght");
                }

                List<Vector4> positions = new List<Vector4>();
                List<Vector4> directions = new List<Vector4>();
                List<Vector4> colors = new List<Vector4>();
                List<Vector4> options = new List<Vector4>();
                //a voir si dans un seule array a la suite

                int size = (int)len;
                if (pos != null && len > 0)
                {
                    positions.AddRange(pos);
                    directions.AddRange(dir);
                    colors.AddRange(col);
                    options.AddRange(opt);

                    positions[size] = pos_;
                    directions[size] = dir_;
                    colors[size] = col_;
                    options[size] = opt_;
                }
                else
                {
                    positions.Add(pos_);
                    directions.Add(dir_);
                    colors.Add(col_);
                    options.Add(opt_);
                }

                bool trouver = false;
                int i = 0;

                if (mesh_skin != null)
                {
                    mat.SetFloat("vector_lenght", size + 1);
                    mat.SetVectorArray("vector_pos", positions);
                    mat.SetVectorArray("vector_dir", directions);
                    mat.SetVectorArray("vector_col", colors);
                    mat.SetVectorArray("vector_opt", options);
                }
                else
                {
                    mat.SetFloat("vector_lenght", size + 1);
                    mat.SetVectorArray("vector_pos", positions);
                    mat.SetVectorArray("vector_dir", directions);
                    mat.SetVectorArray("vector_col", colors);
                    mat.SetVectorArray("vector_opt", options);
                }

            }
        }
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {
        //if (other.gameObject.GetComponent<Light>() == null && other.gameObject.GetComponent<ColliderStatic>() == null)
        if(other.gameObject.GetComponent<RandomizeNPC>() != null || other.gameObject.name.Equals("Player") || other.gameObject.GetComponent<Dynamic>() != null)
        {
            Debug.Log("Collider Light Exit " + other.name + " " + gameObject.name);
            SkinnedMeshRenderer mesh_skin = other.gameObject.GetComponent<SkinnedMeshRenderer>();
            MeshRenderer mesh_ren = null;

            if (mesh_skin == null)
            {
                if (other.gameObject.name.Equals("Player"))
                {
                    mesh_skin = other.transform.Find("Avatar_Esthesia_mergedAnims/CharaEsthesia_geo/charaBody_Esthesia_geo").GetComponent<SkinnedMeshRenderer>();
                    if (mesh_skin == null)
                        other.transform.Find("Avatar_Esthesia_mergedAnims/CharaEsthesia_geo/charaHair_Esthesia_geo").GetComponent<SkinnedMeshRenderer>();
                }
                else
                    mesh_skin = other.transform.Find("body")?.GetChild(0)?.gameObject.GetComponent<SkinnedMeshRenderer>();
                if (mesh_skin == null)
                {
                    mesh_ren = other.transform.Find("body")?.GetChild(0)?.gameObject.GetComponent<MeshRenderer>();
                }
            }


            if(mesh_skin != null || mesh_ren != null)
            {
                Vector4[] pos;
                Vector4[] dir;
                Vector4[] col;
                Vector4[] opt;
                float len = 0.0f;

                Material mat;

                if (mesh_skin != null)
                {
                    mat = mesh_skin.material;
                    if (mat == null)
                        mat = mesh_skin.materials[0];
                    pos = mat.GetVectorArray("vector_pos");
                    dir = mat.GetVectorArray("vector_dir");
                    col = mat.GetVectorArray("vector_col");
                    opt = mat.GetVectorArray("vector_opt");
                    len = mat.GetFloat("vector_lenght");
                }
                else
                {
                    mat = mesh_ren.material;
                    if (mat == null)
                        mat = mesh_ren.materials[0];
                    pos = mat.GetVectorArray("vector_pos");
                    dir = mat.GetVectorArray("vector_dir");
                    col = mat.GetVectorArray("vector_col");
                    opt = mat.GetVectorArray("vector_opt");
                    len = mat.GetFloat("vector_lenght");
                }

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

                bool trouver = false;
                int i = 0;
                while (i < pos.Length && !trouver)
                {
                    if (pos[i].x == other.transform.position.x && pos[i].y == other.transform.position.y && pos[i].z == other.transform.position.z)
                    {
                        trouver = true;
                        positions.RemoveAt(i);
                        directions.RemoveAt(i);
                        colors.RemoveAt(i);
                        options.RemoveAt(i);
                    }
                    i++;
                }

                if (mesh_skin != null)
                {
                    mat.SetFloat("vector_lenght", len - 1);
                    mat.SetVectorArray("vector_pos", positions);
                    mat.SetVectorArray("vector_dir", directions);
                    mat.SetVectorArray("vector_col", colors);
                    mat.SetVectorArray("vector_opt", options);
                }
                else
                {
                    mat.SetFloat("vector_lenght", len - 1);
                    mat.SetVectorArray("vector_pos", positions);
                    mat.SetVectorArray("vector_dir", directions);
                    mat.SetVectorArray("vector_col", colors);
                    mat.SetVectorArray("vector_opt", options);
                }
            }

            
        }
    }
}
