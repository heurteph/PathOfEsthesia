using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    private GameObject messageManager;

    [SerializeField]
    [Tooltip("Tutorial Message to be displayed")]
    private string infoMessage;

    private bool fired;

    private void Awake()
    {
        messageManager = transform.parent.gameObject;
        fired = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!fired)
        {
            if (other.CompareTag("Player"))
            {
                messageManager.GetComponent<MessageManager>().DisplayMessage(infoMessage);
                fired = true;
            }
        }
    }
}
