using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lito : MonoBehaviour
{

    public LitoMovement pjMovement;

    public AnimationLito animLito;

    public bool water = false; // Controlador de si Lito tocó o no el agua
    public bool Dirty = false;
    private float dirtyTimer;

    public bool HasBarlito; // Controlo si puedo transformame en Barco
    public bool IsBarlito = false; // Controlo si estoy transformado en Barco

    public bool HasAvionlito; // Controlo si puedo transformame en Avion
    public bool IsAvionlito = false; // Controlo si estoy transformado en Avion
    

    public int TransformTo; // Controlo en qué me voy a transformar
    
    public bool GrandpaIsTalking = false;

    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void dirtyWater()
    {
        if (Dirty)
        {
            dirtyTimer += Time.deltaTime;
        }
        else
        {
            dirtyTimer = 0;
        }

        if(dirtyTimer >= 5)
        {
            BackToSpawnPoint();
            dirtyTimer = 0;
            water = false;
        }
        else if(dirtyTimer < 0)
        {
            dirtyTimer = 0;
        }
    }

    public void BackToSpawnPoint()
    {
        transform.position = spawnPoint.transform.position;
        TransformTo = 0;
        water = false;
        IsBarlito = false;
        IsAvionlito = false;
        pjMovement.rb.velocity = new Vector2(0, pjMovement.rb.velocity.y); //reseteo velocidades en X y no en Y
        pjMovement.StatChange();
        animLito.TransformingLito();
    }
}
