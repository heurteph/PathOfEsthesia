using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class ControlsManager : MonoBehaviour
{
    Inputs inputs;

    private GameObject menuManager;

    [SerializeField]
    [Tooltip("The button to select the mouse-keyboard controls")]
    private GameObject mouseKeyboardButton;

    [SerializeField]
    [Tooltip("The button to select the gamepad controls")]
    private GameObject gamepadButton;

    [SerializeField]
    [Tooltip("The button to rebind the move up action to another key")]
    private GameObject moveUpButton;

    [SerializeField]
    [Tooltip("The button to rebind the move left action to another key")]
    private GameObject moveLeftButton;

    [SerializeField]
    [Tooltip("The button to rebind the move down action to another key")]
    private GameObject moveDownButton;

    [SerializeField]
    [Tooltip("The button to rebind the move right action to another key")]
    private GameObject moveRightButton;

    [SerializeField]
    [Tooltip("The button to rebind the interact action to another key")]
    private GameObject interactButton;

    [SerializeField]
    [Tooltip("The button to rebind the eye protect action to another key")]
    private GameObject eyeProtectButton;

    [SerializeField]
    [Tooltip("The button to rebind the ear protect action to another key")]
    private GameObject earProtectButton;

    [SerializeField]
    [Tooltip("The button to restore the default bindings")]
    private GameObject restoreBindingsButton;

    [SerializeField]
    [Tooltip("The button to save the current bindings")]
    private GameObject saveBindingsButton;

    // Rebinding

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    private List<string> reservedPaths;

    // US-Layout
    private Dictionary<string, string> defaultPaths = new Dictionary<string, string> {
        { "forward", "<Keyboard>/w" },
        { "turnleft", "<Keyboard>/a" },
        { "turnback", "<Keyboard>/s" },
        { "turnright", "<Keyboard>/d" },
        { "interact", "<Keyboard>/e" },
        { "protecteyes", "<Mouse>/leftButton" },
        { "protectears", "<Mouse>/rightButton" }
    };

    // Navigation

    private Navigation navigationOff = new Navigation();
    private Navigation navigationAuto = new Navigation();

    private Color reactivatedColor = new Color(1, 1, 1);
    private Color deactivatedColor = new Color(0.1f, 0.1f, 0.1f);

    // Start is called before the first frame update
    void Awake()
    {
        inputs = InputsManager.Instance.Inputs;

        menuManager = GameObject.FindGameObjectWithTag("MenuManager");

        navigationOff.mode = Navigation.Mode.None;
        navigationAuto.mode = Navigation.Mode.Automatic;

        if (PlayerPrefs.HasKey("controls") && PlayerPrefs.GetString("controls").Equals("gamepad"))
        {
            UseGamepadControls();
        }
        else
        {
            UseMouseKeyboardControls();
        }

        InitBindings();

        reservedPaths = new List<string> {
                                      "<Keyboard>/AnyKey",
                                      "<Keyboard>/Alt",
                                      "<Keyboard>/LeftAlt",
                                      "<Keyboard>/PrintScreen",
                                      "<Pointer>/Press",
                                      inputs.Player.Interact.bindings[0].path,
                                      inputs.Player.ProtectEyes.bindings[0].path,
                                      inputs.Player.ProtectEars.bindings[0].path,
                                      inputs.Player.Walk.bindings[1].path,
                                      inputs.Player.Walk.bindings[2].path,
                                      inputs.Player.Walk.bindings[3].path,
                                      inputs.Player.Walk.bindings[4].path
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Controller type */

    public void SelectGamepadControls()
    {
        PlayNewKeySound();

        UseGamepadControls();
    }

    public void UseGamepadControls()
    {
        inputs.bindingMask = InputBinding.MaskByGroup(inputs.GamepadScheme.bindingGroup);

        DeactivateButton(gamepadButton);
        ReactivateButton(mouseKeyboardButton);
        menuManager.GetComponent<MenuManager>().SwitchToGamepadControls();

        PlayerPrefs.SetString("controls", "gamepad");
    }

    public void SelectMouseKeyboardControls()
    {
        PlayNewKeySound();

        UseMouseKeyboardControls();
    }

    public void UseMouseKeyboardControls()
    {
        inputs.bindingMask = InputBinding.MaskByGroup(inputs.MouseKeyboardScheme.bindingGroup);

        DeactivateButton(mouseKeyboardButton);
        ReactivateButton(gamepadButton);
        menuManager.GetComponent<MenuManager>().SwitchToMouseKeyboardControls();

        PlayerPrefs.SetString("controls", "mousekeyboard");
    }

    private void ReactivateButton(GameObject button)
    {
        button.transform.GetChild(0).GetComponent<Text>().color = reactivatedColor;
        button.GetComponent<Button>().interactable = true;
        button.GetComponent<EventTrigger>().enabled = true;
    }

    private void DeactivateButton(GameObject button)
    {
        button.transform.GetChild(0).GetComponent<Text>().color = deactivatedColor;
        button.GetComponent<Button>().interactable = false;
        button.GetComponent<EventTrigger>().enabled = false;
    }

    /* Key bindings */

    public void RebindMoveUp() { RebindAction(inputs.Player.Walk, moveUpButton, 1); }

    public void RebindMoveLeft() { RebindAction(inputs.Player.Walk, moveLeftButton, 2); }

    public void RebindMoveDown() { RebindAction(inputs.Player.Walk, moveDownButton, 3); }

    public void RebindMoveRight() { RebindAction(inputs.Player.Walk, moveRightButton, 4); }

    public void RebindInteract() { RebindAction(inputs.Player.Interact, interactButton, 0); }

    public void RebindEyeProtect() { RebindAction(inputs.Player.ProtectEyes, eyeProtectButton, 0); }

    public void RebindEarsProtect() { RebindAction(inputs.Player.ProtectEars, earProtectButton, 0); }

    private void RebindAction(InputAction action, GameObject button, int index)
    {
        PlayRemoveKeySound();

        if (rebindOperation != null && !rebindOperation.completed)
        {
            rebindOperation.Cancel(); // in case of two successive clicks without key press
        }

        // Disable click and submit on the button
        button.GetComponent<EventTrigger>().enabled = false;

        // Disable navigation on the button
        button.GetComponent<Button>().navigation = navigationOff;

        button.transform.GetChild(0).GetComponent<Text>().text = "...";

        action.Disable();

        Action<InputActionRebindingExtensions.RebindingOperation> callback = context =>
        {
            action.Enable();
            rebindOperation.Dispose();
            button.transform.GetChild(0).GetComponent<Text>().text = action.GetBindingDisplayString(index);
            reservedPaths.Add(action.bindings[index].overridePath);
            button.GetComponent<EventTrigger>().enabled = true;
            button.GetComponent<Button>().navigation = navigationAuto;
            PlayNewKeySound();
        };

        rebindOperation = action.PerformInteractiveRebinding()
            .OnComplete(callback)
            .OnCancel(callback)
            .WithControlsHavingToMatchPath("<Keyboard>")
            .WithControlsHavingToMatchPath("<Mouse>")
            .WithCancelingThrough("<Keyboard>/escape")
            // next two lines compulsory for a composite binding
            .WithTargetBinding(index)
            .WithExpectedControlType("Button")
            //.Start()
            ;

        // forbid utilization of a key already in use
        reservedPaths.Remove(action.bindings[index].overridePath);
        foreach (var path in reservedPaths)
        {
            //Debug.Log(path);
            rebindOperation.WithControlsExcluding(path);
        }

        rebindOperation.Start();
    }

    private void InitBindings()
    {
        if (PlayerPrefs.HasKey("forward"))
        {
            inputs.Player.Walk.ApplyBindingOverride(1, PlayerPrefs.GetString("forward"));
            inputs.Player.Walk.ApplyBindingOverride(2, PlayerPrefs.GetString("turnleft"));
            inputs.Player.Walk.ApplyBindingOverride(3, PlayerPrefs.GetString("turnback"));
            inputs.Player.Walk.ApplyBindingOverride(4, PlayerPrefs.GetString("turnright"));
            inputs.Player.Interact.ApplyBindingOverride(0, PlayerPrefs.GetString("interact"));
            inputs.Player.ProtectEyes.ApplyBindingOverride(0, PlayerPrefs.GetString("protecteyes"));
            inputs.Player.ProtectEars.ApplyBindingOverride(0, PlayerPrefs.GetString("protectears"));
        }
        else
        {
            ResetBindings();
        }

        UpdateLabels();
    }

    public void ResetBindings()
    {
        PlayRemoveKeySound();

        inputs.Player.Walk.ApplyBindingOverride(1, defaultPaths["forward"]);
        inputs.Player.Walk.ApplyBindingOverride(2, defaultPaths["turnleft"]);
        inputs.Player.Walk.ApplyBindingOverride(3, defaultPaths["turnback"]);
        inputs.Player.Walk.ApplyBindingOverride(4, defaultPaths["turnright"]);
        inputs.Player.Interact.ApplyBindingOverride(0, defaultPaths["interact"]);
        inputs.Player.ProtectEyes.ApplyBindingOverride(0, defaultPaths["protecteyes"]);
        inputs.Player.ProtectEars.ApplyBindingOverride(0, defaultPaths["protectears"]);

        UpdateLabels();
    }

    public void SaveBindings()
    {
        PlayNewKeySound();

        PlayerPrefs.SetString("forward", inputs.Player.Walk.bindings[1].overridePath);
        PlayerPrefs.SetString("turnleft", inputs.Player.Walk.bindings[2].overridePath);
        PlayerPrefs.SetString("turnback", inputs.Player.Walk.bindings[3].overridePath);
        PlayerPrefs.SetString("turnright", inputs.Player.Walk.bindings[4].overridePath);
        PlayerPrefs.SetString("interact", inputs.Player.Interact.bindings[0].overridePath);
        PlayerPrefs.SetString("protecteyes", inputs.Player.ProtectEyes.bindings[0].overridePath);
        PlayerPrefs.SetString("protectears", inputs.Player.ProtectEars.bindings[0].overridePath);
    }

    private void UpdateLabels()
    {
        moveUpButton.transform.GetChild(0).GetComponent<Text>().text     = inputs.Player.Walk.GetBindingDisplayString(1);
        moveLeftButton.transform.GetChild(0).GetComponent<Text>().text   = inputs.Player.Walk.GetBindingDisplayString(2);
        moveDownButton.transform.GetChild(0).GetComponent<Text>().text   = inputs.Player.Walk.GetBindingDisplayString(3);
        moveRightButton.transform.GetChild(0).GetComponent<Text>().text  = inputs.Player.Walk.GetBindingDisplayString(4);
        interactButton.transform.GetChild(0).GetComponent<Text>().text   = inputs.Player.Interact.GetBindingDisplayString(0);
        eyeProtectButton.transform.GetChild(0).GetComponent<Text>().text = inputs.Player.ProtectEyes.GetBindingDisplayString(0);
        earProtectButton.transform.GetChild(0).GetComponent<Text>().text = inputs.Player.ProtectEars.GetBindingDisplayString(0);

        restoreBindingsButton.transform.GetChild(0).GetComponent<Text>().text = "Reset Keys";
        saveBindingsButton.transform.GetChild(0).GetComponent<Text>().text = "Save Keys";
    }

    /* Wwise functions */

    private void PlayRemoveKeySound()
    {
        AkSoundEngine.PostEvent("Play_Clavier_Suppression", gameObject);
    }

    private void PlayNewKeySound()
    {
        AkSoundEngine.PostEvent("Play_Clavier_Assignation", gameObject);
    }

    private void OnEnable()
    {
        inputs.Player.Enable();
    }
}
