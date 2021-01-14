using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Duck : MonoBehaviour
{
    [SerializeField]
    GameObject pivot = null;

    [SerializeField]
    float speed = 10.0f;

    //sert pour le verrou pour inc nb_id
    static Object obj_lock = new Object();

    static int nb_id = 0;
    int id;
    Material mat;

    public void setId(int id)
    {
        this.id = id;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //pour id 
        lock (obj_lock)
        {
            id = nb_id;
            Interlocked.Increment(ref nb_id);
        }
        //ici on prend le vecteur
        if(pivot != null)
        {
            Vector3 dir = (transform.position - pivot.transform.position).normalized;
            transform.forward = new Vector3(dir.x, transform.forward.y, dir.z);
            transform.Rotate(0.0f, (speed > 0 ? 90.0f : -90.0f), 0.0f,Space.Self);
        }
        mat = GetComponent<MeshRenderer>().material;
        mat.SetFloat("_Id", id);
    }

    void Update()
    {
        if(pivot != null)
        {
            transform.RotateAround(pivot.transform.position, Vector3.up, speed * Time.deltaTime);
        }
    }
}
