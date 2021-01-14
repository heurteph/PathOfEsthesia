using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using System.Linq;

[System.Serializable]
struct Flow
{
    public Spline spline;
    public float timeSpan; // in seconds
    public float count;
    public float averageFastSpeed;
    public float averageNormalSpeed;
    public float averageSlowSpeed;
    public float averageCautiousSpeed;
    public float variability;
}

struct Schedule
{
    public float startTime;
    public float endTime;
    public List<float> spawnTimes;
}

public class FlowsManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The list of the flows")]
    private Flow[] flows;

    [SerializeField]
    [Tooltip("The street users manager")]
    private GameObject streetUsersManager;

    private Schedule[] schedules;

    private List<float> spawnTimesWatcher;
    private int vehicleCount; //s

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)System.DateTime.UtcNow.Ticks);

        vehicleCount = streetUsersManager.GetComponent<StreetUsersManager>().StreetUsers.Length;

        spawnTimesWatcher = new List<float>();

        if(streetUsersManager == null)
        {
            throw new System.ArgumentNullException("The FlowsManager is not linked to the StreetUsersManager");
        }
        if (streetUsersManager.GetComponent<StreetUsersManager>() == null)
        {
            throw new System.ArgumentNullException("The StreetUsersManager doesn't have the correct script attached to it");
        }

        schedules = new Schedule[flows.Length];
        for (int i = 0; i < flows.Length; i++)
        {
            if(flows[i].timeSpan == 0)
            {
                throw new System.ArgumentOutOfRangeException("Time span of the flow cannot be zero");
            }
            schedules[i] = new Schedule();
            schedules[i].spawnTimes = new List<float>();
            Reschedule(ref schedules[i], flows[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < schedules.Length; i++)
        {
            // if one schedule reach its limit, reschedule immediately
            if (Time.time >= schedules[i].endTime)
            {
                Debug.Log("RESCHEDULE NOW FOR " + flows[i].spline.name + " !!!!!!!!!!!!! PREVIOUS END TIME : " + schedules[i].endTime);
                Reschedule(ref schedules[i], flows[i]);
                Debug.Log("RESCHEDULED FOR " + flows[i].spline.name + " !!!!!!!!!!!!! NEXT END TIME : " + schedules[i].endTime);

            }
            // spawn cars
            foreach(float spawnTime in schedules[i].spawnTimes.ToList()) // work on a copy not to remove item on an foreach enumeration
            {
                if(Time.time >= spawnTime)
                {
                    Debug.Log("SPAWN CAR ON " + flows[i].spline.name + " AT " + spawnTime);
                    schedules[i].spawnTimes.Remove(spawnTime);

                    float fastSpeed = flows[i].averageFastSpeed + Random.Range(-flows[i].variability, flows[i].variability);
                    float normalSpeed = flows[i].averageNormalSpeed + Random.Range(-flows[i].variability, flows[i].variability);
                    float slowSpeed = flows[i].averageSlowSpeed + Random.Range(-flows[i].variability, flows[i].variability);
                    float cautiousSpeed = flows[i].averageCautiousSpeed + Random.Range(-flows[i].variability, flows[i].variability);
                    GameObject car = streetUsersManager.GetComponent<StreetUsersManager>().PopCar();
                    if (car != null)
                    {
                        car.GetComponent<StreetUser>().SpecificSet(flows[i].spline, fastSpeed, normalSpeed, slowSpeed, cautiousSpeed);
                    }
                }
            }
        }
    }
    private void Reschedule(ref Schedule schedule, Flow flow)
    {
        // general spawn times
        foreach (float spawnTime in schedule.spawnTimes)
        {
            spawnTimesWatcher.Remove(spawnTime);
        }

        schedule.startTime = Time.time;
        schedule.endTime = Time.time + flow.timeSpan;
        schedule.spawnTimes.Clear();
        for (int j = 0; j < flow.count; j++)
        {
            // choose a time that is not too close to a reserved one
            bool accepted = false;
            float time = 0;
            while (!accepted)
            {
                time = Random.Range(Time.time, Time.time + flow.timeSpan); // TO DO : Check if correct
                accepted = true;
                Debug.Log("2");
                
                foreach (float reservedTime in spawnTimesWatcher)
                {
                    if (time >= reservedTime - flow.timeSpan / (vehicleCount + 2) && time <= reservedTime + flow.timeSpan / (vehicleCount + 2))
                    {
                        //accepted = false;
                        //break;
                    }
                }
            }
            Debug.Log("Time reserved : " + time);
            schedule.spawnTimes.Add(time);

            // general spawn times
            spawnTimesWatcher.Add(time);
        }
    }
}
