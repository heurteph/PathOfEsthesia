using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerDissapear : MonoBehaviour
{
    [SerializeField]
    private GameObject[] passengers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<StreetUser>().HasStopped)
        {
            StartCoroutine("Dissappear");
        }
    }

    IEnumerator Dissappear()
    {
        yield return new WaitForSeconds(2);
        foreach (GameObject passenger in passengers)
        {
            passenger.SetActive(false);
        }
    }
}
