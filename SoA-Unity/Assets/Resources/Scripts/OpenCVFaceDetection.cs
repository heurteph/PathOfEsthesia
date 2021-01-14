using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


//3 * 4 (size of int)
//autrement dit taille de la structure
[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct CvCircle
{
    public int X, Y, Radius;
}

internal static class OpenCVInterop
{
    [DllImport("Test_OpenCV")]
    internal static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

    [DllImport("Test_OpenCV")]
    internal static extern int Close();

    [DllImport("Test_OpenCV")]
    internal static extern int SetScale(int downscale);


    [DllImport("Test_OpenCV")]
    internal unsafe static extern void Detect(CvCircle* outFaces, int maxOutFacesCount, ref int outDetectedFacesCount);

}

public class OpenCVFaceDetection : MonoBehaviour
{
    public static readonly int DIST_MAX = 100;
    public static readonly int DIST_MIN = 10;

    public static List<Vector2> NormalizedFacePositions { get; private set; }
    public static Vector2 CameraResolution;
    public static Vector3 positions;
    public static float taille;

    private const int DetectionDownScale = 1;

    //ne marche pas avec static
    [SerializeField]
    private bool activate = true;

    private bool _ready;
    private int _maxFaceDetectCount = 5;
    private bool reset = true;
    private CvCircle[] _faces;
    private Vector2 oldPosition;

    void Start()
    {
        activate = false;
        int camWidth = 0, camHeight = 0;
        int result = OpenCVInterop.Init(ref camWidth, ref camHeight);
        if( result < 0)
        {
            if (result == -1)
            {
                Debug.LogWarningFormat("[{0}] Failed to find cascades definition.", GetType());
            }
            else if (result == -2)
            {
                Debug.LogWarningFormat("[{0}] Failed to open camera stream.", GetType());
            }

            return;
        }
        CameraResolution = new Vector2(camWidth, camHeight);
        _faces = new CvCircle[_maxFaceDetectCount];
        NormalizedFacePositions = new List<Vector2>();
        OpenCVInterop.SetScale(DetectionDownScale);
        _ready = true;
    }

    private void OnApplicationQuit()
    {
        if(_ready)
        {
            OpenCVInterop.Close();
        }
    }

    public void setActivate(bool state)
    {
        this.activate = state;
    }

    public bool getActivate()
    {
        return this.activate;
    }

    void Update()
    {
        if (activate)
        {
            if (!_ready)
                return;

            int detectedFaceCount = 0;
            unsafe
            {
                fixed (CvCircle* outFaces = _faces)
                {
                    OpenCVInterop.Detect(outFaces, _maxFaceDetectCount, ref detectedFaceCount);
                }
            }
            NormalizedFacePositions.Clear();
            int max = 0;
            int num = -1;

            if (detectedFaceCount == 0 || oldPosition == null)
            {
                reset = true;
            }
            else
            {
                reset = false;
            }

            for (int i = 0; i < detectedFaceCount; i++)
            {
                NormalizedFacePositions.Add(new Vector2((_faces[i].X * DetectionDownScale) / CameraResolution.x, 1f - ((_faces[i].Y * DetectionDownScale) / CameraResolution.y)));
                if (max < _faces[i].Radius)
                {
                    if (!reset)
                    {
                        //un premier filtre d'input
                        float distance = _faces[i].Radius;
                        if (distance < 100.0f && distance >= 0.5f)
                        {
                            max = _faces[i].Radius;
                            positions = NormalizedFacePositions[NormalizedFacePositions.Count - 1];
                            positions.z = max;
                            num = i;
                        }
                    }
                    else
                    {
                        max = _faces[i].Radius;
                        positions = NormalizedFacePositions[NormalizedFacePositions.Count - 1];
                        positions.z = max;
                        num = i;
                    }
                }
            }
            taille = max;
            if (num < 0)
            {
                detectedFaceCount = 0;
            }
            else
            {
                oldPosition = new Vector2(positions.x, positions.y);
            }
        }
    }
}
