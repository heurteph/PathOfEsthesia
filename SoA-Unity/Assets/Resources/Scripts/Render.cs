using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct StructTexture
{
    public Texture2D tex;
}

public class Render : MonoBehaviour
{

    [SerializeField]
    private float coef_blur = 300.0f;
    [SerializeField]
    private float coef_intensity = 16.0f;

    [SerializeField]
    private float radius = 0.6f;
    [SerializeField]
    private Vector3 offSetColor;
    [SerializeField]
    private bool state_blur;
    [SerializeField]
    private bool state_chromatique;
    [SerializeField]
    private bool state_vignette_pleine;
    [SerializeField]
    private float lerp_effet;

    private float vie;
    public bool shader_actif = false;
    private Material mat;
    public string shader_name = "NoVision";
    private Camera cam;
    private RenderTexture RT;
    int it = 0;

    float time_actual = 0.0f;
    float time_refresh = 1.0f;

    public float coef = 0.001f;

    public float offsetX = 0.0f;
    public float offsetY = 0.0f;

    Matrix4x4 projection_base;

    // Start is called before the first frame update
    //a l'init ici
    void Awake()
    {
        state_blur = false;
        state_chromatique = false;
        //courbe blur a gérer en fonction de l'état de vie

        lerp_effet = 1.0f;

        cam = GetComponent<Camera>();
        projection_base = new Matrix4x4();
        for(int i= 0; i < 4; i++)
        {
            projection_base.SetRow(i, cam.projectionMatrix.GetRow(i));
        }

        
        RT = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32, 4);
        RT.volumeDepth = 3;

        RT.useMipMap = true;
        RT.autoGenerateMips = true;
        RT.enableRandomWrite = true;
        RT.Create();

        if (RT.useMipMap)
        {
            Debug.Log("MipMap activate !"+RT.width+ " "+RT.height);
        }

        changeShader("PostProcessV2");
        mat.SetFloat("height", Screen.currentResolution.height);
        mat.SetFloat("width", Screen.currentResolution.width);
    }

    public void changeShader(string name)
    {
        shader_name = name;
        mat = new Material(Shader.Find("Shaders/" + shader_name));
    }

    private void OnPreRender()
    {
        //cam.targetTexture = RT;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!shader_actif)
        {
            Graphics.Blit(source, destination);
        }
        else
        {

            mat.SetFloat("type", 0);

            if (Time.realtimeSinceStartup - time_actual > time_refresh)
            {
               ComputeShader shader = Resources.Load<ComputeShader>("Shaders/Test");
               
                RenderTexture RT_0 = new RenderTexture(32, 32, source.depth, source.format, 0);
                RT_0.volumeDepth = 3;

                RT_0.enableRandomWrite = true;
                RT_0.Create();

                Graphics.Blit(source, RT_0);

                RT = RT_0;

                int w = source.width;
                int h = source.height;

                float ratio = 16.0f / 9.0f;
                float n_w = 128.0f;
                float n_h = n_w / ratio;

                n_w = RT.width;
                n_h = RT.height;

                shader.SetFloat("width", w);
                shader.SetFloat("height", h);

                int nb_element = 2;

                float mipLevel = 1;

                uint[] values = new uint[nb_element];
                values[0] = 0;

                values[1] = 0;

                //taille d'un element pareil a cuda ou ogl
                ComputeBuffer cb = new ComputeBuffer(nb_element, sizeof(uint));
                cb.SetData(values);


                n_w /= mipLevel;
                n_h /= mipLevel;

                //pour RT
                shader.SetFloat("width", n_w);
                shader.SetFloat("height", n_h);

                //qd pas trouvé erreur compilation
                int indexKernel = shader.FindKernel("CSMain");

                shader.SetBuffer(indexKernel, "res", cb);

                shader.SetTexture(indexKernel, "Result", RT, (int)(mipLevel - 1));
                
                //comme en cuda
                int x, y, z;
                
                float reducer = 32.0f;

                float tmp = n_w / reducer;
                //pour tronquer à l'entier le plus haut chiffre quand reste non null
                x = (int)( ((int)(tmp)) != tmp ? tmp+1 : tmp );
                tmp = n_h / reducer;
                y = (int)(((int)(tmp)) != tmp ? tmp+1 : tmp);
                z = 1;
                
                shader.Dispatch(indexKernel, x, y, z);

                cb.GetData(values);
                cb.Release();

                double total = values[0];
                total /= 100.0f;
                total /= (n_w * n_h);

                if (total > 0.5f)
                {
                    mat.SetFloat("type", 1);
                }

                //Debug.Log("Values est de " + total);
                //Debug.Log("Cmpt est de " + values[1]);
                time_actual = Time.realtimeSinceStartup;
            }





                

            changeShader("PostProcessV2");
            mat.SetFloat("width", coef_blur);
            mat.SetFloat("height", coef_blur);
            mat.SetFloat("life", vie);
            mat.SetFloat("_CoefBlur", coef_intensity);
            mat.SetFloat("_Radius", radius);
            mat.SetVector("_OffsetColor",new Vector4(offSetColor.x, offSetColor.y, offSetColor.z,1.0f));
            mat.SetInt("_StateBlur",(state_blur ? 1 : 0));
            mat.SetInt("_StateChromatique", (state_chromatique ? 1 : 0));
            mat.SetInt("_VignettePleine", (state_vignette_pleine ? 1 : 0));
            mat.SetFloat("_LerpEffect", lerp_effet);


            Graphics.Blit(source, destination,mat);
        }
    }
    private void OnPostRender()
    {
        //Camera.main.targetTexture = null;
        //Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), RT);
    }

    public void Update()
    {
        Matrix4x4 shear = Matrix4x4.identity;
        shear.SetRow(2, new Vector4(offsetX, offsetY, 1, 0));
        cam.projectionMatrix = projection_base * shear;


        
        float angle_horizontal = cam.fieldOfView;

        float near, far, left, right, bottom, top;
        near = cam.nearClipPlane;
        far = cam.farClipPlane;
        right = -Mathf.Cos(angle_horizontal / 2.0f) * near;
        left = Mathf.Cos(angle_horizontal / 2.0f) * near;
        
        float angle_vertical = angle_horizontal / cam.aspect;
        bottom = Mathf.Sin(-angle_vertical) * near;
        top = Mathf.Sin(angle_vertical) * near;

        Matrix4x4 mat = new Matrix4x4();
        //2n/r-l 0 r+l/r-l 0
        mat.SetRow(0,new Vector4((2*near)/(right-left),0,(right+left)/(right- left),0));
        //0 2n/t-b t+b/t-b 0
        mat.SetRow(1, new Vector4(0,(2*near)/(top-bottom),(top+bottom)/(top - bottom),0));
        //0 0 -f+n/f-n -2fn/f-n
        mat.SetRow(2, new Vector4(0,0,-(far+near)/(far-near),-(2*far*near)/(far-near)));
        //0 0 -1 0
        mat.SetRow(3, new Vector4(0,0,-1.0f,0));


        cam.projectionMatrix = projection_base;// * shear;        
    }
}
