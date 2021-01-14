using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    List<Light> lights;
    Light[] ls;
    MeshRenderer[] renderers;

    [SerializeField]
    int nb_part;
    // Start is called before the first frame update
    void Start()
    {
        lights = new List<Light>();
        ls = GameObject.FindObjectsOfType<Light>();
        float minx, maxx, minz, maxz;
        minx = maxx = minz = maxz = 0.0f;
        int cmpt = 0;
        foreach(Light l in ls)
        {
            Vector2 pos = new Vector2(l.transform.position.x, l.transform.position.z);
            Debug.Log("Ici " + l);
            if (l.type != LightType.Directional)
            {
                if (cmpt == 0)
                {
                    minx = maxx = pos.x;
                    minz = maxz = pos.y;
                }
                else
                {
                    if (minx > pos.x)
                        minx = pos.x;
                    if (maxx < pos.x)
                        maxx = pos.x;
                    if (minz > pos.y)
                        minz = pos.y;
                    if (maxz < pos.y)
                        maxz = pos.y;
                }
                switch (l.type)
                {
                    case LightType.Point:
                        CapsuleCollider cap = l.gameObject.GetComponent<CapsuleCollider>();
                        //if(cap != null)
                        //    cap.radius = l.range/2.0f;
                        //a changer avec intensity
                        break;
                    case LightType.Spot:
                        BoxCollider box = l.gameObject.GetComponent<BoxCollider>();
                        if (box != null)
                        {
                            box.center = new Vector3(0.0f, 0.0f, l.range / 2.0f);
                            box.size = new Vector3(l.range / 2.0f, l.range / 2.0f, l.range);
                        }
                        break;
                    default:
                        //
                        break;
                }
                lights.Add(l);
                cmpt++;
            }
        }
        /*Debug.Log("Compteur est de " + cmpt);
        Debug.Log("Minx "+minx+" Maxx "+maxx+" Minz "+minz+" Maxz "+maxz);

        float padding_x, padding_z;
        padding_x = (maxx - minx) / nb_part;
        padding_z = (maxz - minz) / nb_part;

        List<Light> [][] tableau;
        tableau = new List<Light>[nb_part][];
        for(int i = 0; i < nb_part; i++)
        {
            tableau[i] = new List<Light>[nb_part];
        }


        for (int i = 0; i < nb_part; i++)
        {
            float z0 = minz +  (i * padding_z);
            float z1 = minz + ((i+1) * padding_z);
            for (int j = 0; j < nb_part; j++)
            {
                float x0 = minx + (j * padding_x);
                float x1 = minx + ((j+1) * padding_x);
                int k = 0;
                tableau[i][j] = new List<Light>();
                while (k < lights.Count )
                {
                    Vector2 pos = new Vector2(lights[k].transform.position.x, lights[k].transform.position.z);
                    if (pos.x >= x0 && pos.x <= x1 && pos.y >= z0 && pos.y <= z1)
                    {
                        Debug.Log(i+" et "+j+" Add light"+lights[k].type);
                        tableau[i][j].Add(lights[k]);
                        lights.RemoveAt(k);
                    }
                    else
                        k++;
                }
                //Debug.Log("X : " + x0 + " " + x1 + " Z : " + z0 + " " + z1);
            }
        }

        //Mesh Renderer
        //tags => static et dynamique
        renderers = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach(MeshRenderer render in renderers)
        {
            Debug.Log("Render "+render.gameObject.name);
            Material[] mat = render.materials;
            List<Vector4> vector_pos = new List<Vector4>();
            List<Vector4> vector_col = new List<Vector4>();
            List<Vector4> vector_dir = new List<Vector4>();
            List<Vector4> vector_opt = new List<Vector4>();
            cmpt = 0;

            while (cmpt < 8 && cmpt < ls.Length)
            {
                //4eme argument angles pour spot et -1.0f pour point
                float value = ls[cmpt].range * (ls[cmpt].type == LightType.Point ? -1.0f : 1.0f);
                value = -1.0f;

                vector_pos.Add(new Vector4(ls[cmpt].transform.position.x, ls[cmpt].transform.position.y, ls[cmpt].transform.position.z,value));
                vector_col.Add(ls[cmpt].color);
                Vector3 dir = ls[cmpt].transform.rotation.eulerAngles;

                //angle
                float outerRad = Mathf.Deg2Rad * 0.5f * ls[cmpt].spotAngle;
                float outerCos = Mathf.Cos(outerRad);
                float outerTan = Mathf.Tan(outerRad);
                float innerCos = Mathf.Cos(Mathf.Atan(((64.0f - 18.0f) / 64.0f) * outerTan));
                float angleRange = Mathf.Max(innerCos - outerCos, 0.001f);

                float X = 1.0f / Mathf.Max(ls[cmpt].range * ls[cmpt].range, 0.00001f);
                float Z = 1.0f / angleRange;
                float W = -outerCos * Z;

                vector_opt.Add(new Vector4(X, 0.0f, Z, W));


                Debug.Log("Dir est de " + dir);

                vector_dir.Add(new Vector4((dir.x * Mathf.PI) / 180.0f, (dir.y * Mathf.PI) / 180.0f, (dir.z * Mathf.PI) / 180.0f, 0.0f));


                cmpt++;
            }
            mat[0].SetVectorArray("vector_pos",vector_pos);
            mat[0].SetVectorArray("vector_dir", vector_dir);
            mat[0].SetVectorArray("vector_col", vector_col);
            mat[0].SetVectorArray("vector_opt", vector_opt);
            mat[0].SetFloat("vector_lenght", cmpt);
        }*/
    }

    /*void Start()
    {
        
    }*/

    //penser a serialiser ce traitement de base pour avoir données au tout débuts sans besoin de recalculer
    //plus penser aussi aux vérification blocs adjacents
    //utilisation de tags => static et dynamique pour différencier traitement et rafraichissement
    //gestion dans les scripts avec vérifications de la positions par rapport aux blocs
    //découpages avec limites de lumières dans blocs
    //et tags aussi pour limiter nombre d'objets a traitement (genre les petits mesh comme serrure)

    // Update is called once per frame
    void Update()
    {
        /*foreach (MeshRenderer render in renderers)
        {
            Debug.Log("Render " + render.gameObject.name);
            Material[] mat = render.materials;
            List<Vector4> vector_pos = new List<Vector4>();
            List<Vector4> vector_col = new List<Vector4>();
            List<Vector4> vector_dir = new List<Vector4>();
            List<Vector4> vector_opt = new List<Vector4>();
            int cmpt = 0;

            while (cmpt < 8 && cmpt < ls.Length)
            {
                //4eme argument angles pour spot et -1.0f pour point
                float value = ls[cmpt].range * (ls[cmpt].type == LightType.Point ? -1.0f : 1.0f);
                value = -1.0f;

                vector_pos.Add(new Vector4(ls[cmpt].transform.position.x, ls[cmpt].transform.position.y, ls[cmpt].transform.position.z, value));
                vector_col.Add(ls[cmpt].color);
                Vector3 dir = ls[cmpt].transform.rotation.eulerAngles;

                //angle
                float outerRad = Mathf.Deg2Rad * 0.5f * ls[cmpt].spotAngle;
                float outerCos = Mathf.Cos(outerRad);
                float outerTan = Mathf.Tan(outerRad);
                float innerCos = Mathf.Cos(Mathf.Atan(((64.0f - 18.0f) / 64.0f) * outerTan));
                float angleRange = Mathf.Max(innerCos - outerCos, 0.001f);

                float X = 1.0f / Mathf.Max(ls[cmpt].range * ls[cmpt].range, 0.00001f);
                float Z = 1.0f / angleRange;
                float W = -outerCos * Z;

                vector_opt.Add(new Vector4(X, 0.0f, Z, W));


                Debug.Log("Dir est de " + dir);

                vector_dir.Add(new Vector4((dir.x * Mathf.PI) / 180.0f, (dir.y * Mathf.PI) / 180.0f, (dir.z * Mathf.PI) / 180.0f, 0.0f));


                cmpt++;
            }
            mat[0].SetVectorArray("vector_pos", vector_pos);
            mat[0].SetVectorArray("vector_dir", vector_dir);
            mat[0].SetVectorArray("vector_col", vector_col);
            mat[0].SetVectorArray("vector_opt", vector_opt);
            mat[0].SetFloat("vector_lenght", cmpt);
        }*/
    }
}
