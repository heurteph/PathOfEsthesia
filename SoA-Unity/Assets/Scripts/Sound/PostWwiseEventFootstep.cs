using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEventFootstep : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event MyEvent;

    [SerializeField]
    private GameObject rightFoot;

    [SerializeField]
    private GameObject leftFoot;

    private LayerMask ground;

    private Vector3 verticalOffset;
    private Vector3 forwardOffset;

    // Use this for initialization.
    void Start()
    {
        ground = LayerMask.GetMask("AsphaltGround") | LayerMask.GetMask("GrassGround") | LayerMask.GetMask("ConcreteGround") | LayerMask.GetMask("SoilGround");

        if (rightFoot == null)
        {
            throw new System.Exception("No reference to the right foot position");
        }
        else if(leftFoot == null)
        {
            throw new System.Exception("No reference to the left foot position");
        }

        verticalOffset = new Vector3(0, 0.5f, 0);
        forwardOffset = new Vector3(0.25f, 0, 0);

        GameObject rightFootRaycaster = new GameObject("R_Raycaster");
        rightFootRaycaster.transform.SetParent (rightFoot.transform);
        rightFootRaycaster.transform.position = rightFoot.transform.position + rightFoot.transform.TransformDirection(forwardOffset + verticalOffset);
        rightFootRaycaster.transform.rotation = rightFoot.transform.rotation;

        GameObject leftFootRaycaster = new GameObject("L_Raycaster");
        leftFootRaycaster.transform.SetParent (leftFoot.transform);
        leftFootRaycaster.transform.position = leftFoot.transform.position + leftFoot.transform.TransformDirection( - (forwardOffset + verticalOffset));
        leftFootRaycaster.transform.rotation = Quaternion.Euler(0, 90, 180) * leftFoot.transform.rotation;
    }

    // Update is called once per frame.
    void Update()
    {

    }

    private static string GetGroundType(GameObject o)
    {
        if (o.layer == LayerMask.NameToLayer("AsphaltGround")) { return "Asphalt"; }
        if (o.layer == LayerMask.NameToLayer("GrassGround")) { return "Herbe"; }
        if (o.layer == LayerMask.NameToLayer("SoilGround")) { return "Terre"; }
        if (o.layer == LayerMask.NameToLayer("ConcreteGround")) { return "Beton"; }
        else { return "None"; }
    }

    public void PlayRightFootstepSound()
    {
        AKRESULT result = AkSoundEngine.SetSwitch("Droit_Gauche", "Droit", gameObject); // Right foot step sounds
        if(result == AKRESULT.AK_Fail)
        {
            throw new System.Exception("Could set the side of the footstep wwise sound");
        }

        if (Physics.Raycast(rightFoot.transform.position, -Vector3.up, out RaycastHit hit, Mathf.Infinity, ground))
        {
            // Load the correct sound according to the ground
            Debug.Log("IM WALKING ON = " + GetGroundType(hit.transform.gameObject));
            result = AkSoundEngine.SetSwitch("Pas_Matiere", GetGroundType(hit.transform.gameObject), gameObject);
        }

        MyEvent.Post(gameObject);
    }

    public void PlayLeftFootstepSound()
    {
        AKRESULT result = AkSoundEngine.SetSwitch("Droit_Gauche", "Gauche", gameObject); // Left foot step sounds

        if (result == AKRESULT.AK_Fail)
        {
            throw new System.Exception("Could set the side of the footstep wwise sound");
        }

        if (Physics.Raycast(leftFoot.transform.position, -Vector3.up, out RaycastHit hit, Mathf.Infinity, ground))
        {
            // Load the correct sound according to the ground
            result = AkSoundEngine.SetSwitch("Pas_Matiere", GetGroundType(hit.transform.gameObject), gameObject);
        }

        MyEvent.Post(gameObject);
    }
}