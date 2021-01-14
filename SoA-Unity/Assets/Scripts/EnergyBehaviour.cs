using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnergyBehaviour : MonoBehaviour
{
    private Inputs inputs;

    [SerializeField]
    [Range(0,1000)]
    private float energy;
    public float Energy { get { return energy; } set { energy = value; } }

    public GameObject gameManager;

    private MonoBehaviour script;

    [SerializeField]
    private DebuggerBehaviour debuggerBehaviour;

    public delegate void EnergyChangedHandler(float e);
    public event EnergyChangedHandler EnergyChangedEvent;

    public delegate void EnterDamageStateHandler();
    public event EnterDamageStateHandler EnterDamageStateEvent;

    public delegate void OutOfEnergyHandler();
    public event OutOfEnergyHandler OutOfEnergyEvent;

    private bool isReloading;
    public bool IsReloading { get { return isReloading; } set { isReloading = value; } }
    
    [SerializeField]
    [Tooltip("Refilling speed in hp/second")]
    private int refillRate = 10;

    private bool godMode;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        if(gameManager == null)
        {
            throw new System.NullReferenceException("No game manager attached to energy script");
        }

        isReloading = false;

        godMode = false;
        debuggerBehaviour.transform.Find("GodMode").GetComponent<Text>().enabled = false;

        inputs = InputsManager.Instance.Inputs;

        // TO DO : Comment it out for final release
        //inputs.Player.GodMode.performed += _ctx => GodMode();

        script = GetComponent<PlayerFollow>() ? GetComponent<PlayerFollow>() : (MonoBehaviour)GetComponent<PlayerFirst>();
        OutOfEnergyEvent += gameManager.GetComponent<GameManager>().GameOver;
        EnergyChangedEvent += debuggerBehaviour.DisplayEnergy;
        EnergyChangedEvent += GetComponent<PlayerFirst>().Hurry;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       if(isReloading)
       {
            IncreaseEnergy(refillRate);
       }
    }

    public void DecreaseEnergy(float e)
    {
        if(godMode)
        {
            return;
        }

        if (energy != 0)
        {
            energy -= e;

            EnergyChangedEvent(energy);

            if (energy <= 0)
            {
                energy = 0;
                OutOfEnergy();
            }
        }
    }

    public void IncreaseEnergy(float e)
    {
        energy += e;

        EnergyChangedEvent(energy);

        if(energy > 1000)
        {
            energy = 1000;
        }
    }

    void OutOfEnergy() // pour l'instant
    {
        OutOfEnergyEvent?.Invoke();
        script.enabled = false;
    }

    public bool IsFull()
    {
        return energy == 1000;
    }

    private void GodMode()
    {
        godMode = !godMode;
        debuggerBehaviour.transform.Find("GodMode").GetComponent<Text>().enabled = godMode;
    }

    public void Invincibility(bool invincibility)
    {
        godMode = invincibility;
    }

}   // FINISH
