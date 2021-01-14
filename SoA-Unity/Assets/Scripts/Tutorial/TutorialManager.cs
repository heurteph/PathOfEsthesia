using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Message to update")]
    private GameObject message;

    [SerializeField]
    [Tooltip("The skip button of the message box")]
    private Image skipButton;

    private Inputs inputs;

    private GameObject transitions;

    // Start is called before the first frame update
    void Start()
    {
        // Transitions
        transitions = GameObject.FindGameObjectWithTag("Transitions");
        transitions.GetComponent<Transitions>().FadeIn();

        inputs = InputsManager.Instance.Inputs;
        inputs.Player.Enable();

        inputs.Player.SkipDialog.performed += StartGame;

        Debug.Assert(skipButton != null, "Missing skip button");
        if (PlayerPrefs.GetString("controls").Equals("gamepad"))
        {
            skipButton.sprite = Resources.Load<Sprite>("Cutscene\\Images\\cutscene 1920\\skip-button-white");
        }
        else
        {
            skipButton.sprite = Resources.Load<Sprite>("Cutscene\\Images\\cutscene 1920\\skip-key-white");
        }

        //message.GetComponent<TextMeshProUGUI>().text = "Quand vous ne pouvez éviter un <color=#adb1d0><b>obstacle</b></color>, utilisez <color=#adb1d0><b>" + inputs.Player.ProtectEyes.GetBindingDisplayString() + "</b></color> ou <color=#adb1d0><b>" + inputs.Player.ProtectEars.GetBindingDisplayString() + "</b></color> pour vous protéger";
        message.GetComponent<TextMeshProUGUI>().text = "When you can't avoid an <color=#adb1d0><b>obstacle</b></color>, use <color=#adb1d0><b>" + inputs.Player.ProtectEyes.GetBindingDisplayString() + "</b></color> or <color=#adb1d0><b>" + inputs.Player.ProtectEars.GetBindingDisplayString() + "</b></color> to protect yourself";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartGame(InputAction.CallbackContext ctx)
    {
        Debug.Log("Loading game elise");

        StartCoroutine(transitions.GetComponent<Transitions>().FadeOut("GameElise"));
        //SceneManager.LoadScene("GameElise");
    }

    private void OnDestroy()
    {
        Debug.Log("On Destroy");

        inputs.Player.SkipDialog.performed -= StartGame;
    }
}
