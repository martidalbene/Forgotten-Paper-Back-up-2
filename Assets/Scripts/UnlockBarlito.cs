using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controlo si la transformación a Barco debe poder utilizarses
public class UnlockBarlito : MonoBehaviour
{
    public bool hasBarlito;
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
            hasBarlito = true;
        }
    }
}
