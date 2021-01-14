using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FieldOfView : MonoBehaviour
{
    [Space]
    [Header("General Crowd Detector Settings")]
    [Space]

    [SerializeField]
    private float meshResolution;

    [SerializeField]
    public LayerMask targetMask;

    [SerializeField]
    public LayerMask obstacleMask;

    private List<Transform> frontVisibleTargets = new List<Transform>();
    private List<Transform> sideVisibleTargets = new List<Transform>();
    private List<Transform> backVisibleTargets = new List<Transform>();


    [SerializeField]
    private EnergyBehaviour energyBehaviour;

    private delegate void CrowdThresholdHandler(float b);
    private event CrowdThresholdHandler crowdThresholdEvent;

    [Space]
    [Header("Animation Damage Settings")]
    [Space]

    [SerializeField]
    private float idleAnimDanger;
    [SerializeField]
    private float moveAnimDanger, danceAnimDanger; 

    [Space]
    [Header("Front Crowd Detector Settings")]
    [Space]

    [SerializeField]
    [Range(0, 360)]
    public float frontViewAngle;

    [SerializeField]
    private float frontViewRadius, totalFrontTargets, frontPow, frontCrowdInfo, frontDangerThreshold, crowdFrontDanger;

    [SerializeField]
    [Range(0, 100)]
    private float crowdFrontDamage;



    [Space]
    [Header("Side Crowd Detector Settings")]
    [Space]

    [SerializeField]
    [Range(0, 360)]
    public float sideViewAngle;

    [SerializeField]
    private float sideViewRadius, totalSideTargets, sidePow, sideCrowdInfo, sideDangerThreshold, crowdSideDanger;

    [SerializeField]
    [Range(0, 100)]
    private float crowdSideDamage;



    [Space]
    [Header("BackCrowdDetector Settings")]
    [Space]

    [SerializeField]
    [Range(0, 360)]
    public float backViewAngle;

    [SerializeField]
    private float backViewRadius, totalBackTargets, backPow, backCrowdInfo,  backDangerThreshold, crowdBackDanger;

    [SerializeField]
    [Range(0, 100)]
    private float crowdBackDamage;






    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("FindFrontTargetsWithDelay", .2f);

        StartCoroutine("FindSideTargetsWithDelay", .2f);

        StartCoroutine("FindBackTargetsWithDelay", .2f);


        //obstacleMask = ~targetMask;
        obstacleMask = 0;

        crowdThresholdEvent += energyBehaviour.DecreaseEnergy;
    }


    // Update is called once per frame
    void Update()
    {
        //Back View
        //	DrawTotalFieldOfBackView();
        DrawFieldOfBackView();

        //Side View
        //	DrawTotalFieldOfSideView();
        DrawFieldOfSideView();

        //Front View
        //DrawTotalFieldOfFrontView();
        DrawFieldOfFrontView();
    }

    IEnumerator FindFrontTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindFrontVisibleTargets();
        }
    }

    //Adapt to every Views !!!!
    void FindFrontVisibleTargets()
    {
        frontVisibleTargets.Clear();
        frontCrowdInfo = 0;
        crowdFrontDanger = 0;


        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, frontViewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            //Debug.Log("J'ai " + targetsInViewRadius.Length + " cibles dans mon rayon");
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < frontViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    frontVisibleTargets.Add(target);
                    Debug.DrawLine(transform.position, target.position, Color.red);


                    frontCrowdInfo += Mathf.Pow((1 - dstToTarget / frontViewRadius), frontPow);


                    //Get NPC's Animation
                    /*
                    Animator anim = target.GetComponent<Animator>();
                    if (anim.GetBool("isMoving") == true)
                    {
                        crowdFrontDanger += moveAnimDanger * Mathf.Pow((1 - dstToTarget / frontViewRadius), frontPow);
                    } else if (anim.GetBool("isDancing") == true)
                    {
                        crowdFrontDanger += danceAnimDanger * Mathf.Pow((1 - dstToTarget / frontViewRadius), frontPow);
                    } else
                    {
                        crowdFrontDanger += idleAnimDanger * Mathf.Pow((1 - dstToTarget / frontViewRadius), frontPow);
                    }*/
                }

                // Apply Damage
                /*
                if (crowdFrontDanger > frontDangerThreshold)
                {
                    crowdThresholdEvent(crowdFrontDamage);
                }*/

                totalFrontTargets = frontVisibleTargets.Count();
                if (totalFrontTargets > frontDangerThreshold) // max 5 persons before it's considered a crowd
                {
                    crowdThresholdEvent(crowdFrontDamage);
                }
            }
        }
    }



    IEnumerator FindSideTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindSideVisibleTargets();
        }
    }

    //Adapt to every Views !!!!
    void FindSideVisibleTargets()
    {
        sideVisibleTargets.Clear();
        sideCrowdInfo = 0;
        crowdSideDanger = 0;


        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sideViewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < sideViewAngle / 2
                && Vector3.Angle(transform.forward, dirToTarget) > frontViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    sideVisibleTargets.Add(target);
                    Debug.DrawLine(transform.position, target.position, Color.blue);

                    sideCrowdInfo += Mathf.Pow((1 - dstToTarget / sideViewRadius), sidePow);


                    //Get NPC's Animation 
                    Animator anim = target.GetComponent<Animator>();
                    if (anim.GetBool("isMoving") == true)
                    {
                        crowdSideDanger += moveAnimDanger * Mathf.Pow((1 - dstToTarget / sideViewRadius), sidePow);
                    }
                    else if (anim.GetBool("isDancing") == true)
                    {
                        crowdSideDanger += danceAnimDanger * Mathf.Pow((1 - dstToTarget / sideViewRadius), sidePow);
                    }
                    else
                    {
                        crowdSideDanger += idleAnimDanger * Mathf.Pow((1 - dstToTarget / sideViewRadius), sidePow);
                    }
                }


                // Apply Damage
                if (crowdSideDanger > sideDangerThreshold)
                {
                    crowdThresholdEvent(crowdSideDamage);
                }

                totalSideTargets = sideVisibleTargets.Count();
            }
        }
    }




    IEnumerator FindBackTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindBackVisibleTargets();
        }
    }

    //Adapt to every Views !!!!
    void FindBackVisibleTargets()
    {
        backVisibleTargets.Clear();
        backCrowdInfo = 0;
        crowdBackDanger = 0;


        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, backViewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < backViewAngle / 2
                && Vector3.Angle(transform.forward, dirToTarget) > sideViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    backVisibleTargets.Add(target);
                    Debug.DrawLine(transform.position, target.position, Color.green);

                    backCrowdInfo += Mathf.Pow((1 - dstToTarget / backViewRadius), backPow);


                    //Get NPC's Animation 
                    Animator anim = target.GetComponent<Animator>();
                    if (anim.GetBool("isMoving") == true)
                    {
                        crowdBackDanger += moveAnimDanger * Mathf.Pow((1 - dstToTarget / backViewRadius), frontPow);
                    }
                    else if (anim.GetBool("isDancing") == true)
                    {
                        crowdBackDanger += danceAnimDanger * Mathf.Pow((1 - dstToTarget / backViewRadius), frontPow);
                    }
                    else
                    {
                        crowdBackDanger += idleAnimDanger * Mathf.Pow((1 - dstToTarget / backViewRadius), frontPow);
                    }
                }


                // Apply Damage
                if (crowdBackDanger > backDangerThreshold)
                {
                    crowdThresholdEvent(crowdBackDamage);
                }



                totalBackTargets = backVisibleTargets.Count();
            }
        }
    }


    //FrontView
    /* void DrawTotalFieldOfFrontView()
	{


		int stepCount = Mathf.RoundToInt(360 * meshResolution);
		float stepAngleSize = 360 / stepCount;

		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - 360 / 2 + stepAngleSize * i;
			Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * frontViewRadius, Color.white);
		}
	} */
    void DrawFieldOfFrontView()
    {


        int stepCount = Mathf.RoundToInt(frontViewAngle * meshResolution);
        float stepAngleSize = frontViewAngle / stepCount;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - frontViewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * frontViewRadius, Color.yellow);
        }
    }


    //SideView
    /*	void DrawTotalFieldOfSideView()
       {


           int stepCount = Mathf.RoundToInt(360 * meshResolution);
           float stepAngleSize = 360 / stepCount;

           for (int i = 0; i <= stepCount; i++)
           {
               float angle = transform.eulerAngles.y - 360 / 2 + stepAngleSize * i;
               Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * sideViewRadius, Color.white);
           }
       } */
    void DrawFieldOfSideView()
    {


        int stepCount = Mathf.RoundToInt(sideViewAngle * meshResolution);
        float stepAngleSize = sideViewAngle / stepCount;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - sideViewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * sideViewRadius, Color.white);
        }
    }


    //BackView
    /* void DrawTotalFieldOfBackView()
	{


		int stepCount = Mathf.RoundToInt(360 * meshResolution);
		float stepAngleSize = 360 / stepCount;

		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - 360 / 2 + stepAngleSize * i;
			Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * backViewRadius, Color.white);
		}
	} */
    void DrawFieldOfBackView()
    {


        int stepCount = Mathf.RoundToInt(backViewAngle * meshResolution);
        float stepAngleSize = backViewAngle / stepCount;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - backViewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * backViewRadius, Color.gray);
        }
    }




    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    /* For user options */

    public void SetCrowdDamage(float crowdDamage)
    {
        this.crowdFrontDamage = crowdDamage;
    }


} // FINISH
