using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMaterialsManager : MonoBehaviour
{
    private List<Material> hairEyebrowsMats;
    private List<Material> hatMats;
    private List<Material> femaleNPCMats;
    private List<Material> maleNPCMats;

    // Start is called before the first frame update
    void Awake()
    {
        Random.InitState(Random.Range(0,1000));

        hairEyebrowsMats = new List<Material>();
        hatMats = new List<Material>();
        femaleNPCMats = new List<Material>();
        maleNPCMats = new List<Material>();

        string materialName;
        for(int i = 1; i <= 14; i++)
        {
            materialName = "Materials/NPC/material_hair_eyebrows_" + i.ToString().PadLeft(2, '0');
            hairEyebrowsMats.Add(Resources.Load<Material>(materialName));
        }
        for(int i = 1; i <= 6; i++)
        {
            materialName = "Materials/NPC/material_hat_" + i.ToString().PadLeft(2, '0');
            hatMats.Add(Resources.Load(materialName, typeof(Material)) as Material);
        }
        for (int i = 1; i <= 10; i++)
        {
            materialName = "Materials/NPC/material_NPC_F_" + i.ToString().PadLeft(2, '0');
            femaleNPCMats.Add(Resources.Load(materialName, typeof(Material)) as Material);
        }
        for (int i = 1; i <= 10; i++)
        {
            materialName = "Materials/NPC/material_NPC_M_" + i.ToString().PadLeft(2, '0');
            maleNPCMats.Add(Resources.Load(materialName, typeof(Material)) as Material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Material GetMaleNPCMaterial()
    {
        return maleNPCMats[Random.Range(0, maleNPCMats.Count)];
    }

    public Material GetFemaleNPCMaterial()
    {
        return femaleNPCMats[Random.Range(0, femaleNPCMats.Count)];
    }
    public Material GetHatMaterial()
    {
        return hatMats[Random.Range(0, hatMats.Count)];
    }
    public Material GetHairEyebrowsMaterial()
    {
        return hairEyebrowsMats[Random.Range(0, hairEyebrowsMats.Count)];
    }
}
