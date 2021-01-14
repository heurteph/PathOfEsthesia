using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MessageManager : MonoBehaviour
{
    Inputs inputs;

    private GameObject tutorialCanvas;
    public GameObject TutorialCanvas { get { return tutorialCanvas; } }

    private void Awake()
    {
        inputs = InputsManager.Instance.Inputs;

        tutorialCanvas = GameObject.FindGameObjectWithTag("TutorialCanvas");
        if (tutorialCanvas == null)
        {
            throw new System.NullReferenceException("Missing a TutorialCanvas in the scene");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayMessage(string message)
    {
        // TO DO : Ensure no message are overlapping when triggered close to each other

        message = message
            .Replace("(eyeprotectkey)", inputs.Player.ProtectEyes.GetBindingDisplayString())
            .Replace("(earprotectkey)", inputs.Player.ProtectEars.GetBindingDisplayString());

        tutorialCanvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = message;
        tutorialCanvas.transform.GetChild(0).GetComponent<Animation>().Play("CanvasGroupFadeInOut");
    }
}
