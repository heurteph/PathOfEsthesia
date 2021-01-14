using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace story
{
    public class CutsceneManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Story in JSON format")]
        private TextAsset jsonData;

        [SerializeField]
        [Tooltip("Story deserialized")]
        private Story story;

        private List<Story.Scene>.Enumerator itScenes;
        private List<Story.Scene.Action>.Enumerator itActions;

        private GameObject transitions;

        [Space]

        // TO DO : Use dictionaries or hashmaps instead of list to find them easily

        [SerializeField]
        [Tooltip("List of the images")]
        private Image[] images;

        [SerializeField]
        [Tooltip("List of the sounds")]
        private AkEvent[] sounds;

        [Space]

        [SerializeField]
        [Tooltip("The message manager")]
        private GameObject messagesManager;

        [SerializeField]
        [Tooltip("The images manager")]
        private GameObject imagesManager;

        [SerializeField]
        [Tooltip("The sound manager")]
        private GameObject soundManager;

        public delegate void ReadyHandler(string type, string id);
        public event ReadyHandler NewMessageEvent;
        public event ReadyHandler NewImageEvent;
        public event ReadyHandler NewSoundEvent;
        public event ReadyHandler NewSceneEvent;

        private Inputs inputs;

        // Start is called before the first frame update

        private void Awake()
        {
            inputs = InputsManager.Instance.Inputs;
            inputs.Player.Enable();
        }

        void Start()
        {
            // Transitions
            transitions = GameObject.FindGameObjectWithTag("Transitions");
            transitions.GetComponent<Transitions>().FadeIn();

            // Callback functions
            NewMessageEvent += messagesManager.GetComponent<MessagesManager>().WriteMessage;
            NewImageEvent += imagesManager.GetComponent<ImagesManager>().ChangeImage;
            NewSoundEvent += soundManager.GetComponent<SoundManager>().PlaySound;
            NewSceneEvent += imagesManager.GetComponent<ImagesManager>().ChangeScene;

            messagesManager.GetComponent<MessagesManager>().MessageShownEvent += ExecuteNextAction;
            imagesManager.GetComponent<ImagesManager>().ImageShownEvent += ExecuteNextAction;
            soundManager.GetComponent<SoundManager>().SoundPlayedEvent += ExecuteNextAction;

            // TO DO : Runtime loading of the json file for multilanguage support (EN/FR)
            //jsonData = Resources.Load<TextAsset>("Cutscenes/cutscenes");

            story = new Story(jsonData.text);
            itScenes = story.scenes.GetEnumerator();
            itScenes.MoveNext(); // Initialization ?
            itActions = itScenes.Current.actions.GetEnumerator();

            AkSoundEngine.SetState("Menu_Oui_Non", "Menu_Non");

            // Start music
            AkSoundEngine.PostEvent("Play_Music_Cinematique", gameObject);

            // Start with the first action
            ExecuteNextAction();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Inputs GetInputs()
        {
            return inputs;
        }

        public void ExecuteNextAction()
        {
            ExecuteAction(GetNextAction());
        }

        public Story.Scene.Action GetNextAction()
        {
            if (!itActions.MoveNext())
            {
                if (!itScenes.MoveNext())
                {
                    // TO DO : Close cutscene and start game
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

                    messagesManager.GetComponent<MessagesManager>().HideSkipButton();
                    inputs.Player.Disable();
                    AkSoundEngine.PostEvent("Stop_Music_Cinematique", gameObject);

                    //SceneManager.LoadScene("Tuto");
                    StartCoroutine(transitions.GetComponent<Transitions>().FadeOut("Tuto-EN")); // Replace with Tuto-FR for french version

                    return null;
                }
                //Debug.Log("Scène suivante");
                // A fade is needed

                itActions = itScenes.Current.actions.GetEnumerator();
                itActions.MoveNext(); // Initialization
            }
            return itActions.Current;
        }

        void ExecuteAction(Story.Scene.Action action)
        {
            if (action == null) return;

            switch (action.type)
            {
                case "message":

                    NewMessageEvent(action.metadata, action.data);
                    break;

                case "image":

                    NewImageEvent(action.metadata, action.data);
                    break;

                case "sound":

                    NewSoundEvent(action.metadata, action.data);
                    break;

                case "scene":

                    NewSceneEvent(action.metadata, action.data);
                    break;

                default:

                    ExecuteNextAction(); // Skip unknown actions
                    break;
            }
        }
    }
}
