using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeNPC : MonoBehaviour
{
    //[SerializeField]
    //[Tooltip("Reference to the NPC material manager")]
    private GameObject NpcMaterialsManager;

    private static readonly int NB_BUFFER_SHADER = 16;

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> props = new List<GameObject>();
        List<GameObject> beards = new List<GameObject>();
        List<GameObject> hairs = new List<GameObject>();
        List<GameObject> eyes = new List<GameObject>();

        foreach (Transform category in transform)
        {
            if(category.name.StartsWith("eyes"))
            {
                foreach (Transform eye in category)
                {
                    eyes.Add(eye.gameObject);
                    eye.gameObject.SetActive(false);
                }
            }
            else if (category.name.StartsWith("hair"))
            {
                foreach (Transform hair in category)
                {
                    hairs.Add(hair.gameObject);
                    hair.gameObject.SetActive(false);
                }
            }
            else if (category.name.StartsWith("beard"))
            {
                foreach (Transform beard in category)
                {
                    beards.Add(beard.gameObject);
                    beard.gameObject.SetActive(false);
                }
            }
            else if (category.name.StartsWith("props"))
            {
                foreach (Transform prop in category)
                {
                    props.Add(prop.gameObject);
                    prop.gameObject.SetActive(false);
                }
            }
        }

        NpcMaterialsManager = GameObject.FindGameObjectWithTag("NPCMaterialsManager");
        if (NpcMaterialsManager == null)
        {
            throw new System.NullReferenceException("No object tagged with NPCMaterialsManager");
        }

        RandomizeFeatures(eyes, beards, props, hairs);

        RandomizeMaterials();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RandomizeMaterials()
    {

    }

    private void RandomizeFeatures(List<GameObject> eyes, List<GameObject> beards, List<GameObject> props, List<GameObject> hairs)
    {
        int index;
        Material hairEyebrowsEyesMaterial = null;

        if (eyes.Count > 0)
        {
            index = Random.Range(0, eyes.Count);
            eyes[index].SetActive(true);

            NpcMaterialsManager.GetComponent<NPCMaterialsManager>();
            // material
            hairEyebrowsEyesMaterial = NpcMaterialsManager.GetComponent<NPCMaterialsManager>().GetHairEyebrowsMaterial();

            if (eyes[index].transform.GetChild(0).GetComponent<MeshRenderer>())
            {
                eyes[index].transform.GetChild(0).GetComponent<MeshRenderer>().material = hairEyebrowsEyesMaterial; // eyebrows
                //eyes[index].transform.GetChild(1).GetComponent<MeshRenderer>().material = hairEyebrowsEyesMaterial; // eyes
            }
            else if (eyes[index].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>())
            {
                eyes[index].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = hairEyebrowsEyesMaterial; // eyebrows
                //eyes[index].transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = hairEyebrowsEyesMaterial; // eyes
            }
        }

        if (beards.Count > 0)
        {
            index = Random.Range(0, beards.Count + 1);
            if (index < beards.Count) // can be beardless
            {
                beards[index]?.SetActive(true);

                // material
                if (beards[index].GetComponent<MeshRenderer>())
                {
                    beards[index].GetComponent<MeshRenderer>().material = hairEyebrowsEyesMaterial;
                }
                else if (beards[index].GetComponent<SkinnedMeshRenderer>())
                {
                    beards[index].GetComponent<SkinnedMeshRenderer>().material = hairEyebrowsEyesMaterial;
                }
            }
        }

        if (props.Count > 0)
        {
            index = Random.Range(0, props.Count + 1);
            if (index < props.Count)  // can wear no hat
            {
                props[index].SetActive(true);

                // material
                if (props[index].GetComponent<MeshRenderer>())
                {
                    props[index].GetComponent<MeshRenderer>().material = NpcMaterialsManager.GetComponent<NPCMaterialsManager>().GetHatMaterial();
                }
                else if (props[index].GetComponent<SkinnedMeshRenderer>())
                {
                    props[index].GetComponent<SkinnedMeshRenderer>().material = NpcMaterialsManager.GetComponent<NPCMaterialsManager>().GetHatMaterial();
                }
            }
        }

        if (hairs.Count > 0)
        {
            bool isFemaleOrKid = transform.Find("body").GetChild(0).name.Contains("npcMkid") || transform.Find("body").GetChild(0).name.Contains("npcF");
            index = Random.Range(0, hairs.Count + (isFemaleOrKid ? 0 : 1));
            if (index < hairs.Count) // can be bald
            {
                hairs[index].SetActive(true);

                // material
                if (hairs[index].GetComponent<MeshRenderer>())
                {
                    hairs[index].GetComponent<MeshRenderer>().material = hairEyebrowsEyesMaterial;
                }
                else if (hairs[index].GetComponent<SkinnedMeshRenderer>())
                {
                    hairs[index].GetComponent<SkinnedMeshRenderer>().material = hairEyebrowsEyesMaterial;
                }
            }
        }

        GameObject body = transform.Find("body")?.GetChild(0)?.gameObject;

        if(body == null)
        {
            throw new System.NullReferenceException("No body gameobject for the NPC " + transform.name);
        }

        if (body.name.Contains("npcM"))
        {
            if (body.GetComponent<MeshRenderer>())
            {
                body.GetComponent<MeshRenderer>().material = NpcMaterialsManager.GetComponent<NPCMaterialsManager>().GetMaleNPCMaterial();
            }
            else if (body.GetComponent<SkinnedMeshRenderer>())
            {
                body.GetComponent<SkinnedMeshRenderer>().material = NpcMaterialsManager.GetComponent<NPCMaterialsManager>().GetMaleNPCMaterial();
            }
        }
        else if (body.name.Contains("npcF"))
        {
            if (body.GetComponent<MeshRenderer>())
            {
                body.GetComponent<MeshRenderer>().material = NpcMaterialsManager.GetComponent<NPCMaterialsManager>().GetMaleNPCMaterial();
            }
            else if (body.GetComponent<SkinnedMeshRenderer>())
            {
                body.GetComponent<SkinnedMeshRenderer>().material = NpcMaterialsManager.GetComponent<NPCMaterialsManager>().GetMaleNPCMaterial();
            }
        }
        else
        {
            throw new System.Exception("No body mesh for the NPC " + transform.name);
        }

        MeshRenderer mesh = body.GetComponent<MeshRenderer>();
        if (mesh == null)
        {
            SkinnedMeshRenderer skinmesh = body.GetComponent<SkinnedMeshRenderer>();
            List<Vector4> vec = new List<Vector4>();
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
