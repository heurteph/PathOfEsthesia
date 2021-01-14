using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pixelplacement;

public class ClientFactory : MonoBehaviour
{
    [Header("Shops")]
    [Space]

    [SerializeField]
    [Tooltip("List of the shops")]
    private GameObject[] shops;


    [Header("Clients")]
    [Space]

    [SerializeField]
    [Tooltip("List of the prefabs of the clients")]
    private GameObject[] clientPrefabs;

    [SerializeField]
    [Tooltip("Number of clients to spawn")]
    [Range(0,50)]
    private float clientsNumber = 20;

    [SerializeField]
    [Tooltip("Speed of the clients")]
    [Range(1, 50)]
    private float speed = 20;

    [SerializeField]
    [Tooltip("Minimum shopping duration")]
    [Range(5, 20)]
    private float minShoppingDuration = 5;

    [SerializeField]
    [Tooltip("Maximum shopping duration")]
    [Range(5, 20)]
    private float maxShoppingDuration = 20;

    [SerializeField]
    [Tooltip("The duration of the freeze")]
    private float freezeDuration = 1;

    // Start is called before the first frame update
    void Start()
    {
        SpawnClients();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnClients()
    {
        for (int i = 0; i < clientsNumber; i++)
        {
            GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Length)];
            GameObject myShop = shops[Random.Range(0, shops.Length)];
            //CLIENTSTATE myState = (CLIENTSTATE)Random.Range(0, 2);
            CLIENTSTATE myState = CLIENTSTATE.WALKING;

            float myStartPercentage = 1f;
            Spline mySpline = null;

            mySpline = myShop.GetComponent<Shop>().GetExitPath();
            myStartPercentage = Random.Range(0f, 1f);

            GameObject client = Instantiate(clientPrefab, mySpline.GetPosition(myStartPercentage), Quaternion.LookRotation(mySpline.GetDirection(myStartPercentage)));

            client.GetComponent<Client>().SetConstants(speed, minShoppingDuration, maxShoppingDuration);
            client.GetComponent<Client>().Initialize(mySpline, myStartPercentage, myState);
        }
    }
}

