using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Transitions : MonoBehaviour
{
    private static GameObject singleton;

    [SerializeField]
    private GameObject black;

    // Start is called before the first frame update
    void Awake()
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
    }

    public bool IsOn()
    {
        return black.GetComponent<Image>().color.a == 1;
    }

    public bool IsOff()
    {
        return black.GetComponent<Image>().color.a == 0;
    }


    public void FadeIn()
    {
        black.GetComponent<Animation>().Play("TransitionsFadeIn");
    }

    public IEnumerator FadeOut(string sceneName)
    {
        black.GetComponent<Animation>().Play("TransitionsFadeOut");

        yield return new WaitUntil(() => IsOn());

        if (GameObject.FindGameObjectWithTag("GameManager") == null)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName); // idem

            // DoDestroyOnLoad
            Destroy(GameObject.FindGameObjectWithTag("SaveManager"));
            Destroy(GameObject.FindGameObjectWithTag("MainCanvas"));
            Destroy(GameObject.FindGameObjectWithTag("GameManager"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
