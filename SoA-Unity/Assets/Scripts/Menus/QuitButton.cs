using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    GameObject menuManager;

    Navigation navigation;

    private ParticleSystem sunSpots;
    ParticleSystem.EmissionModule emission;
    ParticleSystem.VelocityOverLifetimeModule velocity;
    float rateOverTime = 5;

    public delegate void ButtonHandler();
    public event ButtonHandler EnterButtonEvent;
    public event ButtonHandler ValidateButtonEvent;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = GameObject.FindGameObjectWithTag("MenuManager");
        if (menuManager == null)
        {
            throw new System.NullReferenceException("Missing MenuManager object");
        }

        sunSpots = transform.GetChild(1).GetComponent<ParticleSystem>();
        emission = sunSpots.emission;
        emission.rateOverTime = 0;
        velocity = sunSpots.velocityOverLifetime;
        velocity.enabled = true;

        EnterButtonEvent += menuManager.GetComponent<MenuManager>().PlayHoverSound;
        ValidateButtonEvent += menuManager.GetComponent<MenuManager>().PlayClickSound;
    }

    // Update is called once per frame
    void Update()
    {
        // take the particle system to the center of the button
        Vector2 center = transform.GetChild(0).GetComponent<RectTransform>().rect.center;
        transform.GetChild(1).position = transform.GetComponent<RectTransform>().TransformPoint(new Vector3(center.x, center.y, -250));
    }

    /* Mouse */

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        EnterButtonAnimation();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        ExitButtonAnimation();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        ValidateButtonAnimation();
    }

    /* Keyboard */

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        EnterButtonAnimation();
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        ExitButtonAnimation();
    }

    void ISubmitHandler.OnSubmit(BaseEventData eventData)
    {
        ValidateButtonAnimation();
    }

    /* Generic */

    private void EnterButtonAnimation()
    {
        //transform.GetChild(0).GetComponent<Animation>().Play("MenuItemPop");
        transform.GetChild(0).GetComponent<Animation>().Play("MenuItemColorIn");
        emission.rateOverTime = rateOverTime;
        transform.GetChild(1).GetComponent<MenuParticleSystem>().PlayParticleSound();

        EnterButtonEvent();
    }

    private void ExitButtonAnimation()
    {
        transform.GetChild(0).GetComponent<Animation>().Play("MenuItemColorOut");
        emission.rateOverTime = 0;
        transform.GetChild(1).GetComponent<MenuParticleSystem>().StopParticleSound();
    }

    private void ValidateButtonAnimation()
    {
        /*
        if (creditsPanel.GetComponent<CanvasGroup>().alpha != 0)
        {
            creditsPanel.GetComponent<Animation>().Play("CreditsFadeOut");
        }*/
        transform.GetChild(0).GetComponent<Animation>().Play("MenuItemFlash");
        transform.GetChild(1).GetComponent<MenuParticleSystem>().StopParticleSound();

        StartCoroutine("BurstSpots");

        // Handle navigation

        navigation = GetComponent<Button>().navigation;
        navigation.mode = Navigation.Mode.None;

        ValidateButtonEvent();

        if (menuManager.GetComponent<MenuManager>().MenuState == MENU_STATE.CREDITS)
        {
            menuManager.GetComponent<MenuManager>().HideCredits();
            transform.parent.GetChild(2).GetChild(0).GetComponent<Animation>().Play("MenuItemUngreyed");

            // Restore navigation

            //navigation = transform.parent.GetChild(1).GetComponent<Button>().navigation;
            //navigation.mode = Navigation.Mode.Automatic | Navigation.Mode.Vertical | Navigation.Mode.Horizontal;

        }
        else if (menuManager.GetComponent<MenuManager>().MenuState == MENU_STATE.CONTROLS)
        {
            menuManager.GetComponent<MenuManager>().HideControls();
            transform.parent.GetChild(1).GetChild(0).GetComponent<Animation>().Play("MenuItemUngreyed");

            // Restore navigation

            //navigation = transform.parent.GetChild(1).GetComponent<Button>().navigation;
            //navigation.mode = Navigation.Mode.Automatic | Navigation.Mode.Vertical | Navigation.Mode.Horizontal;
        }

        Application.Quit();
    }

    private IEnumerator BurstSpots()
    {
        emission.rateOverTime = 30;
        yield return null;
        emission.rateOverTime = 0; // ou rateOverTime si encore dessus
        velocity.radial = 15;
        while (velocity.radial.constant > 0)
        {
            velocity.radial = Mathf.Max(0, velocity.radial.constant - Time.deltaTime * 10);
            yield return null;
        }
    }
    private void OnDestroy()
    {
        transform.GetChild(1).GetComponent<MenuParticleSystem>().StopParticleSound();
    }
}
