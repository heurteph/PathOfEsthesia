using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using AK.Wwise;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum DIFFICULTY { EASY, MEDIUM, HARD, ONESHOT };

public class GameManager : MonoBehaviour
{
    private static GameObject singleton;

    private Inputs inputs;

    [Header("Transitions")]

    [SerializeField]
    [Tooltip("The animator to transition")]
    private GameObject sceneTransition;

    //private Animator nightTransition;
    //private Animator creditsTransition;

    private GameObject transitions;

    [SerializeField]
    [Tooltip("Duration of the transitions to credits in seconds")]
    [Range(1, 5)]
    private float transitionDuration = 3;

    [SerializeField]
    [Tooltip("Duration of the fade to restart in seconds")]
    [Range(2, 10)]
    private float restartFadeDuration = 5;

    private Image fade;
    private Image gameOverLogo;
    private Text gameOverMessage;

    private GameObject brightness;
    private GameObject loudness;
    private GameObject crowd;

    private bool isGameOver;
    public bool IsGameOver { get { return isGameOver; } }

    //private Camera mainCamera;
    //private Camera transitionCamera;

    bool firstRun;

    [Space]
    [Header("Options")]

    [SerializeField]
    [Tooltip("Default difficulty level")]
    private DIFFICULTY difficulty = DIFFICULTY.MEDIUM;

    //Dictionary<DIFFICULTY, float> brightnessDamages = new Dictionary<DIFFICULTY, float> { { DIFFICULTY.MEDIUM, 10f }, { DIFFICULTY.EASY, 5f }, { DIFFICULTY.HARD, 20f }, { DIFFICULTY.ONESHOT, 1000f } };
    Dictionary<DIFFICULTY, float> brightnessDamages = new Dictionary<DIFFICULTY, float> { { DIFFICULTY.MEDIUM, 50f }, { DIFFICULTY.EASY, 10f }, { DIFFICULTY.HARD, 100f }, { DIFFICULTY.ONESHOT, 1000f } };
    Dictionary<DIFFICULTY, float> loudnessDamages = new Dictionary<DIFFICULTY, float> { { DIFFICULTY.MEDIUM, 50f }, { DIFFICULTY.EASY, 10f }, { DIFFICULTY.HARD, 100f }, { DIFFICULTY.ONESHOT, 1000f } };
    Dictionary<DIFFICULTY, float> crowdDamages   = new Dictionary<DIFFICULTY, float> { { DIFFICULTY.MEDIUM, 10f }, { DIFFICULTY.EASY, 5f }, { DIFFICULTY.HARD, 20f }, { DIFFICULTY.ONESHOT, 1000f } };
    
    public delegate void GamePausedHandler();
    public event GamePausedHandler GamePausedEvent;
    public event GamePausedHandler GameResumedEvent;

    public delegate void DifficultyChangedHandler();
    public event DifficultyChangedHandler EasyDifficultyEvent;
    public event DifficultyChangedHandler MediumDifficultyEvent;
    public event DifficultyChangedHandler HardDifficultyEvent;
    public event DifficultyChangedHandler OneshotDifficultyEvent;

    private void Awake()
    {
        // Transitions
        transitions = GameObject.FindGameObjectWithTag("Transitions");
        transitions.GetComponent<Transitions>().FadeIn();

        firstRun = true;
        isGameOver = false;

        if (singleton == null)
        {
            singleton = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != gameObject)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("A NEW GAME MANAGER IS LOADED ! YEAH !");

        inputs = InputsManager.Instance.Inputs;

        inputs.Player.Quit.performed += PauseGame;

        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        gameOverLogo = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Image>();
        gameOverMessage = GameObject.FindGameObjectWithTag("GameOverMessage").GetComponent<Text>();

        brightness = GameObject.FindGameObjectWithTag("Brightness");
        loudness = GameObject.FindGameObjectWithTag("Loudness");
        crowd = GameObject.FindGameObjectWithTag("Crowd");

        Debug.Assert(brightness != null, "Missing vision reference");
        Debug.Assert(loudness != null, "Missing hearing reference");
        Debug.Assert(crowd != null, "Missing crowd reference");
        if(SceneManager.GetActiveScene().name == "GameNight")
            Debug.Assert(brightness.GetComponent<VisionBehaviour>() != null, "Incorrect vision reference");
        Debug.Assert(loudness.GetComponent<HearingScript>() != null, "Incorrect hearing reference");
        Debug.Assert(crowd.GetComponent<FieldOfView>() != null, "Incorrect crowd reference");

        //mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //transitionCamera = GameObject.FindGameObjectWithTag("TransitionCamera").GetComponent<Camera>();

        if (fade == null)
        {
            throw new System.ArgumentException("No fade found in the UI");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.SetState("Menu_Oui_Non", "Menu_Non");


        if (SceneManager.GetActiveScene().name == "GameElise")
        {
            sceneTransition.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators\\NightTransition");
        }

        if(SceneManager.GetActiveScene().name == "GameNight")
        {
            sceneTransition.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators\\CreditsTransition");
        }

        sceneTransition.GetComponent<Animator>().SetBool("HasWon", false);
        sceneTransition.GetComponent<Animator>().SetBool("CreditsLoaded", false);

        //nightTransition.SetBool("HasWon", false);
        //nightTransition.SetBool("CreditsLoaded", false);

        ChangeDifficulty(difficulty);

        /*
        if (stopAllEvent == null)
        {
            throw new System.NullReferenceException("No reference to the wwise stop all sounds event on the game manager");
        }*/
        Analytics.enabled = false;

        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Pause functions */

    private void PauseGame(InputAction.CallbackContext ctx)
    {
        Time.timeScale = 0;
        AkSoundEngine.Suspend();

        // Switch pause key callback
        inputs.Player.Quit.performed -= PauseGame;
        inputs.Player.Quit.performed += ResumeGame;

        // To avoid triggering the click action
        inputs.Player.Disable();
        inputs.Player.Quit.Enable();

        // Display menu
        GamePausedEvent();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        AkSoundEngine.WakeupFromSuspend();

        // Switch pause key callback
        inputs.Player.Quit.performed -= ResumeGame;
        inputs.Player.Quit.performed += PauseGame;

        inputs.Player.Enable();

        // Hide menu
        GameResumedEvent();
    }
    public void ResumeGame(InputAction.CallbackContext ctx)
    {
        ResumeGame();
    }

    /* Interface for slider */

    public void ChangeDifficulty(float value)
    {
        //Debug.Log("Difficulty changed from slider !");

        Debug.Assert(value >= 1 && value <= 3, "Unknown difficulty level : " + value);
        difficulty = (DIFFICULTY)((int)value - 1);
        ChangeDifficulty(difficulty);
    }

    /* Interface for buttons */

    public void ChangeToEasy()
    {
        difficulty = DIFFICULTY.EASY;
        ChangeDifficulty(difficulty);
    }

    public void ChangeToMedium()
    {
        difficulty = DIFFICULTY.MEDIUM;
        ChangeDifficulty(difficulty);
    }

    public void ChangeToHard()
    {
        difficulty = DIFFICULTY.HARD;
        ChangeDifficulty(difficulty);
    }

    public void ChangeDifficulty(DIFFICULTY difficulty)
    {
        brightness.GetComponent<VisionBehaviour>().SetBrightnessDamage(brightnessDamages[difficulty]);
        loudness.GetComponent<HearingScript>().SetLoudnessDamage(loudnessDamages[difficulty]);
        crowd.GetComponent<FieldOfView>().SetCrowdDamage(crowdDamages[difficulty]);

        switch (difficulty)
        {
            case DIFFICULTY.EASY:
                EasyDifficultyEvent();
                break;
            case DIFFICULTY.MEDIUM:
                MediumDifficultyEvent();
                break;
            case DIFFICULTY.HARD:
                HardDifficultyEvent();
                break;
            case DIFFICULTY.ONESHOT:
                OneshotDifficultyEvent();
                break;
            default:
                throw new System.IndexOutOfRangeException("Unknown difficulty level : " + difficulty);
        }
    }

    public DIFFICULTY GetDifficulty()
    {
        return difficulty;
    }

    public void QuitGame() // Unused
    {
        // TO DO : Add warning message
        Application.Quit();
    }

    public void GoToTitle()
    {
        GoToScene("Title");
    }

    public void GoToCredits()
    {
        GoToScene("CreditsScene");
    }

    public void GoToScene(string sceneName)
    {
        // FIX ????
        firstRun = true;
        isGameOver = false;

        singleton = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Destroy singletons
        GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>().DestroySingleton();
        GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<InGameMenuManager>().DestroySingleton();

        // Unpause
        Time.timeScale = 1;
        AkSoundEngine.WakeupFromSuspend();
        inputs.Player.Enable();

        // Inputs
        inputs.Player.Quit.performed -= PauseGame;
        inputs.Player.Quit.performed -= ResumeGame;

        // Destroy singleton
        InputsManager.Destroy();

        //Wwise
        AkSoundEngine.StopAll();

        // Load title
        StartCoroutine(transitions.GetComponent<Transitions>().FadeOut(sceneName));

        //SceneManager.LoadScene(sceneName);
        
        // DoDestroyOnLoad
        //Destroy(GameObject.FindGameObjectWithTag("SaveManager"));
        //Destroy(GameObject.FindGameObjectWithTag("MainCanvas"));
        //Destroy(GameObject.FindGameObjectWithTag("GameManager"));
    }

    /* Defeat functions */

    public void GameOver()
    {
        isGameOver = true;

        //Disable player inputs
        inputs.Player.Disable();

        //Play defeat sound
        AkSoundEngine.PostEvent("Play_Mort", gameObject, (uint)AkCallbackType.AK_EndOfEvent, CallbackFunction, null);

        // Fade out
        fade.GetComponent<Animation>().Play("BlackScreenFadeIn");

        // Display message
        // gameOverMessage.GetComponent<Animation>().Play("GameOverMessageFadeInOut");

        // Display logo
        // gameOverLogo.GetComponent<Animation>().Play("LogoFadeIn");

        // Display new message
        gameOverMessage.GetComponent<Animation>().Play("GameOverMessageFadeIn");
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        // Stop all sounds
        AkSoundEngine.StopAll();

        Debug.Log("Loading scene");
        //gameOverLogo.GetComponent<Image>().color = new Color(gameOverLogo.GetComponent<Image>().color.r, gameOverLogo.GetComponent<Image>().color.g, gameOverLogo.GetComponent<Image>().color.b, 1);
        
        gameOverMessage.GetComponent<Text>().color = new Color(gameOverMessage.GetComponent<Text>().color.r, gameOverMessage.GetComponent<Text>().color.g, gameOverMessage.GetComponent<Text>().color.b, 1);
        fade.GetComponent<Image>().color = new Color(fade.GetComponent<Image>().color.r, fade.GetComponent<Image>().color.g, fade.GetComponent<Image>().color.b, 1);

        // Switch camera
        //transitionCamera.enabled = true;
        //mainCamera.enabled = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        isGameOver = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameElise" || scene.name == "Game" || scene.name == "CutZonesScene" || scene.name == "GameNight")
        {
            // Reload references

            transitions = GameObject.FindGameObjectWithTag("Transitions"); // not needed

            fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
            gameOverLogo = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Image>();
            gameOverMessage = GameObject.FindGameObjectWithTag("GameOverMessage").GetComponent<Text>();

            brightness = GameObject.FindGameObjectWithTag("Brightness");
            loudness = GameObject.FindGameObjectWithTag("Loudness");
            crowd = GameObject.FindGameObjectWithTag("Crowd");

            if (!firstRun)
            {
                Debug.Log("IT'S NOT A FIRST RUN !");

                // Fade out
                //gameOverLogo.GetComponent<Animation>().Play("LogoFadeOut");

                gameOverMessage.GetComponent<Animation>().Play("GameOverMessageFadeOut");
                fade.GetComponent<Animation>().Play("BlackScreenFadeOut");

                inputs.Player.Enable();
            }
            else
            {
                Debug.Log("IT'S A FIRST RUN");
                firstRun = false;
            }
        }
    }

    /* Victory functions */

    public void WinGame()
    {
        inputs.Player.Disable();

        // TO DO : Esthesia invincibility (and no damage animation)

        //sceneTransition.GetComponent<Animator>().SetBool("HasWon", true);

        if (SceneManager.GetActiveScene().name == "GameElise")
        {
            Debug.Log("Transition to GameNight");
            GoToNight();
        }
        else if (SceneManager.GetActiveScene().name == "GameNight")
        {
            Debug.Log("Transition to CreditsScene");
            GoToCredits();
        }
    }

    public void DisplayCredits()
    {
        //AkSoundEngine.StopAll();
        //SceneManager.LoadScene("CreditsScene");

        GoToCredits();

        /*
        if (SceneManager.GetActiveScene().name != "CreditsScene")
        {
            Debug.Log("Credits loading");
            StartCoroutine("WaitForCreditsLoad");
        }
        else
        {
            Debug.Log("Crédits déjà prêts");
        }*/
    }

    private IEnumerator WaitForCreditsLoad()
    {
        while (SceneManager.GetActiveScene().name != "CreditsScene")
        {
            yield return null;
        }

        if (SceneManager.GetActiveScene().name == "CreditsScene")
        {
            sceneTransition.GetComponent<Animator>().SetBool("CreditsLoaded", true);
        }
    }

    public void GoToNight()
    {
        GoToScene("GameNight");

        /*
        if (SceneManager.GetActiveScene().name != "GameNight")
        {
            Debug.Log("Night loading");
            StartCoroutine("WaitForNightLoad");
        }
        else
        {
            Debug.Log("Night déjà prêt");
        }*/
    }

    private IEnumerator WaitForNightLoad()
    {
        while (SceneManager.GetActiveScene().name != "GameNight")
        {
            yield return null;
        }

        if (SceneManager.GetActiveScene().name == "GameNight")
        {
            sceneTransition.GetComponent<Animator>().SetBool("CreditsLoaded", true);
        }
    }
}
