using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Render_PostProcess : MonoBehaviour
{
    private GameObject player;

    [SerializeField]
    public float coef_blur = 1000.0f;
    [SerializeField]
    private float coef_intensity = 16.0f;

    [SerializeField]
    public float radius = 0.6f;
    [SerializeField]
    private Vector3 offSetColor;
    [SerializeField]
    private bool state_blur;
    [SerializeField]
    private bool state_chromatique;
    [SerializeField]
    private bool state_feedBack;
    [SerializeField]
    private bool state_vignette_pleine;
    [SerializeField]
    private float lerp_effet;
    [SerializeField]
    public Vector3 offsetChroma;

    //partie pour back up visuel pour son
    [SerializeField]
    private GameObject head;
    [SerializeField]
    private Vector2 position;
    [SerializeField]
    private float radius_head_min;
    [SerializeField]
    private float radius_head_max;
    [SerializeField]
    private bool head_activate;
    [SerializeField]
    private Vector4 color_sense;


    public bool shader_actif = false;
    private Material mat;
    public string shader_name = "PostProcessV2";
    private Camera cam;
    private RenderTexture RT;
    int it = 0;

    float time_actual = 0.0f;
    float time_refresh = 1.0f;

    public float coef = 0.001f;

    public float offsetX = 0.0f;
    public float offsetY = 0.0f;

    // Start is called before the first frame update
    //a l'init ici
    void Awake()
    {
        state_blur = true;
        state_chromatique = true;
        //courbe blur a gérer en fonction de l'état de vie

        player = GameObject.FindGameObjectWithTag("Player");

        if(player == null)
        {
            throw new System.NullReferenceException("Missing an object tagged Player in the scene");
        }

        lerp_effet = 1.0f;

        color_sense = new Vector4(1.0f,1.0f,1.0f,1.0f);

        cam = GetComponent<Camera>();
        changeShader("PostProcessV2");
    }

    public void changeShader(string name)
    {
        shader_name = name;
        mat = new Material(Shader.Find("Shaders/" + shader_name));
    }

    public void SetBlur(float blur)
    {
        //mat.SetFloat("_CoefBlur", blur);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        //non activé
        if (!shader_actif)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            mat.SetFloat("type", 0);

            changeShader("PostProcessV2");
            //mat.SetFloat("width", coef_blur);
            //mat.SetFloat("height", coef_blur);
            mat.SetFloat("width", source.width);
            mat.SetFloat("height", source.height);
            mat.SetFloat("life", player.GetComponent<EnergyBehaviour>().Energy / 10); // 0-1000 -> 0-100
            mat.SetFloat("_CoefBlur", coef_intensity);
            mat.SetFloat("_Radius", radius);
            mat.SetVector("_OffsetColor",new Vector4(offSetColor.x, offSetColor.y, offSetColor.z,1.0f));
            mat.SetInt("_StateBlur",(state_blur ? 1 : 0));
            mat.SetInt("_StateChromatique", (state_chromatique ? 1 : 0));
            mat.SetInt("_StateFeedBack", (state_feedBack ? 1 : 0));
            mat.SetInt("_VignettePleine", (state_vignette_pleine ? 1 : 0));
            mat.SetFloat("_LerpEffect", lerp_effet);
            mat.SetVector("_OffsetColor", new Vector4(offsetChroma.x, offsetChroma.y, offsetChroma.z, 1.0f));
            //mat.SetVector("_Position_Head",new Vector4(head.transform.position.x, head.transform.position.y, head.transform.position.z,(head_activate ? 1.0f : 0.0f)));
            Vector3 position = Camera.current.WorldToScreenPoint(head.transform.position);
            mat.SetVector("_Position_Head", new Vector4(position.x, position.y, position.z, (head_activate ? 1.0f : 0.0f)));
            //mat.SetVector("_Position_Head", new Vector4(position.x, position.y, 0.0f, (head_activate ? 1.0f : 0.0f)));
            mat.SetFloat("_Radius_Head_Min",radius_head_min);
            mat.SetFloat("_Radius_Head_Max", radius_head_max);
            mat.SetVector("_Color_Sense", color_sense);

            Debug.Log("Marker head est de " + head.transform.position);

            Graphics.Blit(source, destination,mat);
        }
    }
}
