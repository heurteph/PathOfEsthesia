using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    private static GameObject singleton;

    private GameObject gameManager;

    private PostProcessVolume postProcessVolume;
    private Vignette vignette;

    //[SerializeField]
    //[Tooltip("Reference to the difficulty label")]
    //private GameObject difficultyLabel;

    //[SerializeField]
    //[Tooltip("Reference to the difficulty slider")]
    //private GameObject difficultySlider;

    [SerializeField]
    [Tooltip("Reference to the pause menu")]
    private GameObject pauseMenu;

    [SerializeField]
    [Tooltip("Reference to the easy mode button")]
    private GameObject easyModeButton;

    [SerializeField]
    [Tooltip("Reference to the medium mode button")]
    private GameObject mediumModeButton;

    [SerializeField]
    [Tooltip("Reference to the medium mode button")]
    private GameObject hardModeButton;

    [SerializeField]
    [Tooltip("Reference to the resume button")]
    private GameObject resumeButton;

    [SerializeField]
    [Tooltip("Reference to the back to menu button")]
    private GameObject backButton;

    // Start is called before the first frame update
    void Start()
    {
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

        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        postProcessVolume = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out vignette);

        Debug.Assert(pauseMenu != null, "Missing pause menu reference");
        Debug.Assert(gameManager != null, "Missing Game Manager reference");
        //Debug.Assert(difficultyLabel != null, "Missing difficulty label reference");
        //Debug.Assert(difficultySlider != null, "Missing difficulty slider reference");
        Debug.Assert(resumeButton != null, "Missing resume button reference");
        Debug.Assert(backButton != null, "Missing quit button reference");
        Debug.Assert(easyModeButton != null, "Missing easy mode button reference");
        Debug.Assert(mediumModeButton != null, "Missing medium mode button reference");
        Debug.Assert(hardModeButton != null, "Missing hard mode button reference");

        // Link menu visibility to the game manager
        gameManager.GetComponent<GameManager>().GamePausedEvent += DisplayPauseMenu;
        gameManager.GetComponent<GameManager>().GameResumedEvent += HidePauseMenu;

        // Link buttons to the game manager

        //difficultySlider.GetComponent<Slider>().onValueChanged.AddListener(gameManager.GetComponent<GameManager>().ChangeDifficulty);

        easyModeButton.GetComponent<Button>().onClick.AddListener(gameManager.GetComponent<GameManager>().ChangeToEasy);
        mediumModeButton.GetComponent<Button>().onClick.AddListener(gameManager.GetComponent<GameManager>().ChangeToMedium);
        hardModeButton.GetComponent<Button>().onClick.AddListener(gameManager.GetComponent<GameManager>().ChangeToHard);

        resumeButton.GetComponent<Button>().onClick.AddListener(gameManager.GetComponent<GameManager>().ResumeGame);
        backButton.GetComponent<Button>().onClick.AddListener(gameManager.GetComponent<GameManager>().GoToTitle);

        // when update from the game manager, change the slider without notifying events, or else we would go into an infinite loop

        //gameManager.GetComponent<GameManager>().EasyDifficultyEvent += () => { difficultyLabel.GetComponent<TextMeshProUGUI>().SetText("Easy"); difficultyLabel.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 1); difficultySlider.GetComponent<Slider>().SetValueWithoutNotify(1); };
        //gameManager.GetComponent<GameManager>().MediumDifficultyEvent += () => { difficultyLabel.GetComponent<TextMeshProUGUI>().SetText("Medium"); difficultyLabel.GetComponent<TextMeshProUGUI>().color = new Color(0, 1, 0); difficultySlider.GetComponent<Slider>().SetValueWithoutNotify(2); };
        //gameManager.GetComponent<GameManager>().HardDifficultyEvent += () => { difficultyLabel.GetComponent<TextMeshProUGUI>().SetText("Hard"); difficultyLabel.GetComponent<TextMeshProUGUI>().color = new Color(1, 0, 0); difficultySlider.GetComponent<Slider>().SetValueWithoutNotify(3); };
        //gameManager.GetComponent<GameManager>().OneshotDifficultyEvent += () => { difficultyLabel.GetComponent<TextMeshProUGUI>().SetText("One Shot"); difficultyLabel.GetComponent<TextMeshProUGUI>().color = new Color(128 / 255f, 0 / 255f, 0 / 255f); difficultySlider.GetComponent<Slider>().SetValueWithoutNotify(3); }; // shouldn't happen in normal game

        gameManager.GetComponent<GameManager>().EasyDifficultyEvent    += UpdateDifficultySprites;
        gameManager.GetComponent<GameManager>().MediumDifficultyEvent  += UpdateDifficultySprites;
        gameManager.GetComponent<GameManager>().HardDifficultyEvent    += UpdateDifficultySprites;
        gameManager.GetComponent<GameManager>().OneshotDifficultyEvent += UpdateDifficultySprites;

        HidePauseMenu();
    }

    private void UpdateDifficultySprites()
    {
        easyModeButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures\\UI\\difficulty-1" + (gameManager.GetComponent<GameManager>().GetDifficulty() == DIFFICULTY.EASY ? "-selected" : "" ));
        mediumModeButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures\\UI\\difficulty-2" + (gameManager.GetComponent<GameManager>().GetDifficulty() == DIFFICULTY.MEDIUM ? "-selected" : ""));
        hardModeButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures\\UI\\difficulty-3" + (gameManager.GetComponent<GameManager>().GetDifficulty() == DIFFICULTY.HARD ? "-selected" : ""));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DisplayPauseMenu()
    {
        pauseMenu.SetActive(true);
        vignette.active = true;
    }

    private void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
        vignette.active = false;
    }

    public void DestroySingleton()
    {
        singleton = null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameElise" || scene.name == "Game" || scene.name == "CutZonesScene" || scene.name == "GameNight")
        {
            // Reload references of the compass
            GameObject.FindGameObjectWithTag("Compass").transform.GetChild(0).GetComponent<CompassBehavior>().ReloadReferences();
        }
    }
}
