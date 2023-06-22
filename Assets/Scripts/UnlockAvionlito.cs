using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controlo si la transformación a Avion debe poder utilizarse
public class UnlockAvionlito : MonoBehaviour
{
    public bool hasAvionlito;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            hasAvionlito = true;
        }
    }
}
