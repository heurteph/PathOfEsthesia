using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using story;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;

public class MessagesManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The cutscene manager")]
    private GameObject cutsceneManager;

    public delegate void MessageHandler();
    public event MessageHandler MessageShownEvent;

    //private string text;

    public delegate IEnumerator DisplayMessage(string text);
    DisplayMessage del;

    [Space]

    [SerializeField]
    [Tooltip("The message box")]
    private TextMeshProUGUI messageBoxLower;

    [SerializeField]
    [Tooltip("The header of the message box")]
    private Text nameBoxLower;

    [Space]

    [SerializeField]
    [Tooltip("The message box")]
    private TextMeshProUGUI messageBoxUpper;

    [SerializeField]
    [Tooltip("The header of the message box")]
    private Text nameBoxUpper;

    [SerializeField]
    [Tooltip("The skip button of the message box")]
    private Image skipButton;

    [SerializeField]
    [Tooltip("The vibrations of the message")]
    private ParticleSystem vibrations;
    private ParticleSystem.EmissionModule emissionModule;

    [Space]
    [Header("Message Animation Options")]

    [SerializeField]
    [Tooltip("The delay between two letters")]
    [Range(1,5)]
    private float messageSpeed = 1;

    [SerializeField]
    [Tooltip("The number of characters appearing at the same time")]
    [Range(1, 10)]
    private float rolloverCharacterSpread = 5;

    [SerializeField]
    [Tooltip("How long it takes for a message to start appearing")]
    [Range(0f, 5)]
    private float preMessageDuration = 0.5f;

    [SerializeField]
    [Tooltip("How long a message remains in seconds once it's been displayed")]
    [Range(0f, 5)]
    private float postMessageDuration = 1.25f;

    [SerializeField]
    [Tooltip("How long a pause last (dot, comma)")]
    [Range(0f, 2f)]
    private float shortPauseDuration = 0.25f;

    [SerializeField]
    [Tooltip("How long a pause last (dot, comma)")]
    [Range(0f, 2f)]
    private float longPauseDuration = 0.35f;

    private TextMeshProUGUI textMesh;
    private Text textName;

    private bool useVibrations = false;

    private bool skip = false;
    private bool next = false;

    private Inputs inputs;

    private bool upper = true;

    // Start is called before the first frame update
    void Awake()
    {
        inputs = cutsceneManager.GetComponent<CutsceneManager>().GetInputs();
        Debug.Assert(inputs != null, "Inputs not instantiated");

        textMesh = messageBoxUpper.GetComponent<TextMeshProUGUI>();
        Debug.Assert(textMesh != null, "No TextMeshProUGUI attached to " + transform.name);

        textMesh.text = string.Empty;

        emissionModule = vibrations.emission;
        EndVibrating();

        Debug.Assert(skipButton != null, "Missing reference to skip button in message manager");
        if(PlayerPrefs.GetString("controls").Equals("gamepad"))
        {
            skipButton.sprite = Resources.Load<Sprite>("Cutscene\\Images\\cutscene 1920\\next-button-white");
        }
        else
        {
            skipButton.sprite = Resources.Load<Sprite>("Cutscene\\Images\\cutscene 1920\\next-key-white");
        }

        skipButton.GetComponent<Animation>().Play("SkipButtonFadeIn");

        del = new DisplayMessage(RevealLetterByLetter);
    }


    // Possible runtime change of the display method
    // DisplayMessage del = new DisplayMessage(callback);
    public void WriteMessage(string name, string message /*, callback */)
    {
        if (upper)
        {
            textName = nameBoxUpper;
        }
        else
        {
            textName = nameBoxLower;
        }
        textName.text = name;
        StartCoroutine(del(message));
    }

    IEnumerator RevealFade(string text)
    {
        textMesh.text = text;
        textMesh.alpha = 0;
        textMesh.ForceMeshUpdate();

        TMP_TextInfo textInfo = textMesh.textInfo;
        Color32[] newVertexColors;

        int currentCharacterIndex = 0;
        int startingCharacterIndex = currentCharacterIndex;
        bool isRangeMax = false;

        yield return new WaitForSeconds(preMessageDuration);

        while (!isRangeMax)
        {
            int characterCount = textInfo.characterCount;

            // Spread should not exceed the number of characters.
            //float RolloverCharacterSpread = 5; // how many character appearing at once
            byte fadeSteps = (byte)Mathf.Max(1, 255 / rolloverCharacterSpread);

            for (int i = startingCharacterIndex; i < currentCharacterIndex + 1; i++)
            {
                // Skip characters that are not visible
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the vertex colors of the mesh used by this text element (character or sprite).
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the current character's alpha value.
                byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps, 0, 255);

                // Set new alpha values.
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;

                // Tint vertex colors
                // Note: Vertex colors are Color32 so we need to cast to Color to multiply with tint which is Color.
                //newVertexColors[vertexIndex + 0] = (Color)newVertexColors[vertexIndex + 0] * ColorTint;
                //newVertexColors[vertexIndex + 1] = (Color)newVertexColors[vertexIndex + 1] * ColorTint;
                //newVertexColors[vertexIndex + 2] = (Color)newVertexColors[vertexIndex + 2] * ColorTint;
                //newVertexColors[vertexIndex + 3] = (Color)newVertexColors[vertexIndex + 3] * ColorTint;

                if (alpha == 255)
                {
                    startingCharacterIndex += 1;

                    if (startingCharacterIndex == characterCount)
                    {
                        isRangeMax = true; // Would end the coroutine.

                        /*
                        
                        // Update mesh vertex data one last time.
                        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                        yield return new WaitForSeconds(1.0f);

                        // Reset the text object back to original state.
                        textMesh.ForceMeshUpdate();

                        yield return new WaitForSeconds(1.0f);

                        // Reset our counters.
                        currentCharacterIndex = 0;
                        startingCharacterIndex = 0;

                        */
                    }
                }
            }

            // Upload the changed vertex colors to the Mesh.
            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            if (currentCharacterIndex + 1 < characterCount) currentCharacterIndex += 1;

            yield return new WaitForSeconds(0.01f / messageSpeed);
        }

        yield return new WaitForSeconds(postMessageDuration);

        MessageShownEvent();
    }

    IEnumerator RevealLetterByLetter(string text)
    {
        inputs.Player.SkipDialog.performed += SkipDialog;

        if(upper)
        {
            textMesh = messageBoxUpper.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            textMesh = messageBoxLower.GetComponent<TextMeshProUGUI>();
        }
        upper = false;

        textMesh.text = text;
        textMesh.maxVisibleCharacters = 0;

        skip = false;
        next = false;

        float delay;
        textMesh.ForceMeshUpdate(); // update textInfo to get actual characterCount

        yield return new WaitForSeconds(preMessageDuration);

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            textMesh.maxVisibleCharacters = i + 1;
            
            if (!skip)
            {
                if (IsShortPause(textMesh.text[i]))
                {
                    if (useVibrations)
                        EndVibrating();
                    delay = shortPauseDuration;
                }
                else if (IsLongPause(textMesh.text[i]))
                {
                    if (useVibrations)
                        EndVibrating();
                    delay = longPauseDuration;
                }
                else
                {
                    // Don't put sound on space characters
                    if (textMesh.text[i] != ' ')
                    {
                        if (textName.text == "Ange" || textName.text == "Zeous")
                            AkSoundEngine.PostEvent("Play_Ecriture_Animation_Ange", gameObject);
                        else
                            AkSoundEngine.PostEvent("Play_Ecriture_Animation", gameObject);
                    }

                    if (useVibrations)
                        StartVibrating();
                    delay = 0.1f / (2 * messageSpeed);
                }
            }
            else
            {
                textMesh.maxVisibleCharacters = textMesh.textInfo.characterCount;
                break;
            }

            yield return new WaitForSeconds(delay);
        }

        if (useVibrations)
            EndVibrating();

        // Wait until input from user

        inputs.Player.SkipDialog.performed += NextEvent;

        while(!next)
        {
            yield return new WaitForEndOfFrame();
        }

        // Erase message before next event, maybe in some case it could stay
        textMesh.text = "";
        textName.text = "";

        MessageShownEvent();
    }

    IEnumerator RevealWordByWord(string text)
    {
        textMesh.text = text;
        textMesh.maxVisibleWords = 0;

        float delay;
        textMesh.ForceMeshUpdate(); // update textInfo to get actual wordCount

        yield return new WaitForSeconds(preMessageDuration);

        for (int i = 0; i < textMesh.textInfo.wordCount + 1; i++)
        {
            textMesh.maxVisibleWords = i + 1;
            delay = 0.1f / (2 * messageSpeed);
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(postMessageDuration);

        MessageShownEvent();
    }

    public void StartVibrating()
    {
        emissionModule.rateOverTime = 8f;
    }

    public void EndVibrating()
    {
        emissionModule.rateOverTime = 0f;
    }

    public static bool IsShortPause(char c)
    {
        return c == ',' || c == ':';
    }

    public static bool IsLongPause(char c)
    {
        return c == '.' || c == '!';
    }

    private void SkipDialog(InputAction.CallbackContext ctx)
    {
        inputs.Player.SkipDialog.performed -= SkipDialog;
        skip = true;
    }

    private void NextEvent(InputAction.CallbackContext ctx)
    {
        AkSoundEngine.PostEvent("Play_Touche_Next", gameObject);

        inputs.Player.SkipDialog.performed -= NextEvent;
        next = true;
    }

    public void HideSkipButton()
    {
        skipButton.enabled = false;
        //skipButton.GetComponent<Animation>().Play("SkipButtonFadeOut");
    }

    private void OnDestroy()
    {
        inputs.Player.SkipDialog.performed -= SkipDialog;
        inputs.Player.SkipDialog.performed -= NextEvent;
    }
}
