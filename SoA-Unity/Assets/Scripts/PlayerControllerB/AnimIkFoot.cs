using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimIkFoot : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject ground;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    [Range(0, 1)]
    private float distanceToGround; 

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float skinWidth = 0.2f;

/*

    [SerializeField]
    private float ikWeight = 1;

    [SerializeField]
    private Transform leftIKTarget, rightIKTarget;

    [SerializeField]
    private Transform hintLeft, hintRight;

    Vector3 lFpos, rFpos;

    Quaternion lFrot, rFrot;

    float lFWeight, rFWeight;

    Transform leftFoot, rightFoot;

    public float offsetY;

    */

    // Start is called before the first frame update
    void Start()
    {
     /*   anim = GetComponent<Animator>();

        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);

        lFrot = leftFoot.rotation;
        rFrot = rightFoot.rotation;
        */
    }


    // Update is called once per frame
    void Update()
    {
       /* RaycastHit leftHit;
        RaycastHit rightHit;

        Vector3 lpos = leftFoot.TransformPoint(Vector3.zero);
        Vector3 rpos = rightFoot.TransformPoint(Vector3.zero);

        if(Physics.Raycast(lpos, -Vector3.up, out leftHit, 1, layerMask))
        {
            lFpos = leftHit.point;
            lFrot = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
        }

        if (Physics.Raycast(rpos, -Vector3.up, out rightHit, 1, layerMask))
        {
            rFpos = rightHit.point;
            rFrot = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
        } */
    }


/*
    private void OnAnimatorIK(int layerIndex)
    {
        lFWeight = anim.GetFloat("LeftFoot");
        rFWeight = anim.GetFloat("RightFoot");

    
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, lFpos + new Vector3(0, offsetY, 0));
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rFpos + new Vector3(0, offsetY, 0));

        
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, ikWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, ikWeight);

        anim.SetIKHintPosition(AvatarIKHint.LeftKnee, hintLeft.position);
        anim.SetIKHintPosition(AvatarIKHint.RightKnee, hintLeft.position);
        


        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKRotation(AvatarIKGoal.LeftFoot, lFrot);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rFrot);

    }

*/





           private void OnAnimatorIK(int layerIndex)
              {
                  
                      RaycastHit hit;


            //Stand Still
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;

                    Vector3 prevPos = ground.transform.position;
                    prevPos.y = footPosition.y - skinWidth;
                    ground.transform.position = prevPos;
                    
                }
            }
        if(anim)
                  {
            //Left Foot
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
                      anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

                      // Left Foot Up
                      Ray leftRay = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

                      if (Physics.Raycast(leftRay, out hit, distanceToGround + 1f, layerMask))
                      {
                          if (hit.transform.tag == "Walkable")
                          {
                              Vector3 footPosition = hit.point;
                              footPosition.y += distanceToGround;

                              anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);

                              anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);

                          }
                      }



                      // Right Foot
                      anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
                      anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

                      //Right Foot Up
                      Ray rightRay = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

                      if (Physics.Raycast(rightRay, out hit, distanceToGround + 1f, layerMask))
                      {
                          if (hit.transform.tag == "Walkable")
                          {
                             Vector3 footPosition = hit.point;
                             footPosition.y += distanceToGround;

                              anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);

                              anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);


                          }
                      }


                  }
              }

          

        } //FINISH
