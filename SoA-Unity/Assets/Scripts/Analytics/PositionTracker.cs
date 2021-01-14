using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pixelplacement.TweenSystem;
using System.IO;
using System;
using System.Text.RegularExpressions;

public class PositionTracker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the player")]
    private GameObject player;

    [SerializeField]
    [Tooltip("The object to track")]
    private GameObject trackedObject;

    [SerializeField]
    [Tooltip("The refreshing frequency in Hz")]
    private float frequency = 2;

    struct Data { public float t; public Vector2 position; }
    List<Data> positionData;

    // Start is called before the first frame update
    void Start()
    {
        positionData = new List<Data>();
        player.GetComponent<EnergyBehaviour>().OutOfEnergyEvent += SaveRun;
        StartCoroutine("TrackPosition");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TrackPosition()
    {
        for (; ; )
        {
            Data data = new Data();

            data.t = Time.time;
            data.position = new Vector2(transform.position.x, transform.position.z);
            positionData.Add(data);

            yield return new WaitForSeconds(1f / frequency);
        }
    }

    string CheckForFilename(string name, int number, string extension)
    {
        if (File.Exists(name + number + extension))
        {
            return CheckForFilename(name, number + 1, extension);
        }
        else
        {
            return name + number.ToString() + extension;
        }
    }

    public void SaveRun()
    {
        string name = Application.dataPath + "/AnalyticsData/run";
        int number = 0;
        string extension = ".csv";
        string filename = CheckForFilename(name, number, extension);
        StreamWriter streamWriter = new StreamWriter(filename, false);
        Regex regex = new Regex(",");
        foreach ( Data data in positionData)
        {
            streamWriter.WriteLine(regex.Replace(data.t.ToString(), ".") + "," + regex.Replace(data.position.x.ToString(), ".") + "," + regex.Replace(data.position.y.ToString(), "."), Environment.NewLine );
         //   streamWriter.NewLine(;
        }
        streamWriter.Close();
        Debug.Log("File written");
    }

} // FINISH
