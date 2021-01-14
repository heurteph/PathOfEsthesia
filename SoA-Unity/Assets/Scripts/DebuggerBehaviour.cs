using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DebuggerBehaviour : MonoBehaviour
{
    //private static GameObject instance;

    [Header("Indirect Brightness Display")]

    [SerializeField]
    private RawImage indirectBrightnessGreyDisplay;

    [SerializeField]
    private Slider indirectBrightnessGauge;

    [SerializeField]
    private Slider indirectBrightnessThresholdGauge;

    [SerializeField]
    private VisionBehaviour indirectBrightness;

    [SerializeField]
    private Text indirectBrightnessPercentage;

    [Space]
    [Header("Direct Brightness Display")]
    
    [SerializeField]
    private RawImage directBrightnessGreyDisplay;

    [SerializeField]
    private Slider directBrightnessGauge;

    [SerializeField]
    private Slider directBrightnessThresholdGauge;

    [SerializeField]
    private VisionBehaviour directBrightness;

    [SerializeField]
    private Text directBrightnessPercentage;

    [Space]
    [Header("Loudness Display")]

    [SerializeField]
    private Slider loudnessGauge;

    [SerializeField]
    private Slider loudnessThresholdGauge;

    [SerializeField]
    private HearingScript loudnessScript;

    [SerializeField]
    private Text loudnessValue;

    [Space]
    [Header("Energy Display")]

    [SerializeField]
    private Slider energyBar;

    void Awake()
    {
        /*
        if (instance == null)
        {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (gameObject != instance)
        {
            Destroy(gameObject);
            return;
        }
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        energyBar.maxValue = 1000; // TO DO : Place in game manager
        energyBar.value = energyBar.maxValue;

        indirectBrightnessGauge.maxValue = 1;
        indirectBrightnessThresholdGauge.maxValue = indirectBrightnessGauge.maxValue;

        directBrightnessGauge.maxValue = 1;
        directBrightnessThresholdGauge.maxValue = directBrightnessGauge.maxValue;

        loudnessGauge.maxValue = 1;
        loudnessThresholdGauge.maxValue = loudnessGauge.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DisplayBrightness(GameObject sender, Texture2D t2D, float percentage)
    {
        //Debug.Log("Sender : " + sender.name);
        if (String.Equals(sender.name, indirectBrightness.name))
        {
            DisplayIndirectBrightness(t2D, percentage);
            //Debug.Log("Display from inside vision");
        }
        else if (String.Equals(sender.name, directBrightness.name))
        {
            DisplayDirectBrightness(t2D, percentage);
            //Debug.Log("Display from outside vision");
        }
    }

    public void DisplayIndirectBrightness (Texture2D t2D, float percentage) 
    {
        indirectBrightnessGreyDisplay.texture = t2D;
        indirectBrightnessPercentage.text = Mathf.Round(percentage*1000)/10 + "%";

        indirectBrightnessGauge.value = percentage;
        indirectBrightnessThresholdGauge.value = indirectBrightness.BrightnessThreshold;

        if (percentage >= indirectBrightness.BrightnessThreshold)
        {
            indirectBrightnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (percentage >= indirectBrightness.BrightnessThreshold * 0.5f)
        {
            indirectBrightnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            indirectBrightnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.white;
        }
    }

    public void DisplayDirectBrightness(Texture2D t2D, float percentage)
    {
        directBrightnessGreyDisplay.texture = t2D;
        directBrightnessPercentage.text = Mathf.Round(percentage * 1000) / 10 + "%";

        directBrightnessGauge.value = percentage;
        directBrightnessThresholdGauge.value = directBrightness.BrightnessThreshold;

        if (percentage >= directBrightness.BrightnessThreshold)
        {
            directBrightnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (percentage >= directBrightness.BrightnessThreshold * 0.5f)
        {
            directBrightnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            directBrightnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.white;
        }
    }


    public void DisplayLoudness (float volume)
    {
        loudnessGauge.value = volume;
        loudnessThresholdGauge.value = loudnessScript.LoudnessThreshold;

        if (volume >= loudnessScript.LoudnessThreshold)
        {
            loudnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        } 
        else if (volume >= loudnessScript.LoudnessThreshold * 0.5f)
        {
            loudnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            loudnessGauge.fillRect.gameObject.GetComponent<Image>().color = Color.white;
        }

        loudnessValue.text = "" + volume;
    }

    public void DisplayEnergy (float energy)
    {
        energyBar.value = energy;
    }

} // FINISH
